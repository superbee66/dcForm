using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;
using dCForm.Client;
using dCForm.Client.Util;
using dCForm.Core.Format;
using dCForm.Core.Format.Json;
using dCForm.Core.Template;
using dCForm.Core.Template.Filesystem;
using dCForm.Core.Template.Nosql;
using dCForm.Core.Util;
using dCForm.Core.Util.Cab;
using dCForm.Core.Util.Xsd;
using Microsoft.Deployment.Compression;
using Microsoft.Deployment.Compression.Cab;
using Newtonsoft.Json;

namespace dCForm.Core
{
    public class ImporterLightDoc
    {
        public string DocSrc
        {
            get { return LightDoc.DocSrc; }
        }

        public string DocTitleLi
        {
            get
            {
                return string.Format(
                    "{0} {1} \"{2}\" {3}",
                    string.IsNullOrWhiteSpace(ExceptionMessage)
                        ? "Success, "
                        : "Fail, ",
                    LightDoc.DocSubmitDate,
                    LightDoc.DocTitle,
                    ExceptionMessage);
            }
        }

        public string ExceptionMessage { get; set; }

        public string ImportDocSrc { get; set; }

        public LightDoc LightDoc { get; set; }
    }

    public static class ImporterController
    {
        /// <summary>
        ///     used to check for revision conflicts of ScanContentFolder MD5/Version claims before a DocRev cab is created,
        ///     versioned & imported
        /// </summary>
        private static NosqlTemplateController _NosqlTemplateController = new NosqlTemplateController();
        private static readonly JsonSerializerSettings _JsonSerializerSettings = new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        public static string DirectoryFullName
        {
            get { return RequestPaths.GetPhysicalApplicationPath("import"); }
        }

        static ImporterController() { Reflection.LoadBinDlls(); }

        private static List<ImporterLightDoc> GetImporterLightDocList(string FullPathOfFile)
        {
            FileInfo _FileInfo = new FileInfo(FullPathOfFile);
            DirectoryInfo _DirectoryInfo = _FileInfo.Directory;
            FileInfo _FileInfoLog =
                new FileInfo(string.Format(@"{0}\{1}.import.json", _DirectoryInfo.FullName,
                    Environment.GetEnvironmentVariable("computername")));

            return _FileInfoLog.Exists
                       ? JsonConvert.DeserializeObject<List<ImporterLightDoc>>(File.ReadAllText(_FileInfoLog.FullName))
                       : new List<ImporterLightDoc>();
        }

        public static List<ImporterLightDoc> GetImporterLogByFolder(string FullPathOfFile = null)
        {
            if (string.IsNullOrEmpty(FullPathOfFile))
                FullPathOfFile = DirectoryFullName;

            FileInfo _FileInfoLog =
                new FileInfo(string.Format(@"{0}\{1}.import.json", FullPathOfFile,
                    Environment.GetEnvironmentVariable("computername")));

            return _FileInfoLog.Exists
                       ? JsonConvert.DeserializeObject<List<ImporterLightDoc>>(File.ReadAllText(_FileInfoLog.FullName))
                       : new List<ImporterLightDoc>();
        }

        private static string GetLogFilePath(string TargetImportFile)
        {
            FileInfo _FileInfo = new FileInfo(TargetImportFile);
            DirectoryInfo _DirectoryInfo = _FileInfo.Directory;
            return string.Format(@"{0}\{1}.import.json", _DirectoryInfo.FullName,
                Environment.GetEnvironmentVariable("computername"));
        }

        private static IList<string> GetRelativeFilePathsInDirectoryTree(string dir, bool includeSubdirectories)
        {
            IList<string> list = new List<string>();
            RecursiveGetRelativeFilePathsInDirectoryTree(dir, string.Empty, includeSubdirectories, list);
            return list;
        }

        private static void RecursiveGetRelativeFilePathsInDirectoryTree(string dir, string relativeDir, bool includeSubdirectories, IList<string> fileList)
        {
            string[] files = Directory.GetFiles(dir);
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i];
                string fileName = Path.GetFileName(path);
                fileList.Add(Path.Combine(relativeDir, fileName));
            }
            if (includeSubdirectories)
            {
                string[] directories = Directory.GetDirectories(dir);
                for (int j = 0; j < directories.Length; j++)
                {
                    string path2 = directories[j];
                    string fileName2 = Path.GetFileName(path2);
                    RecursiveGetRelativeFilePathsInDirectoryTree(Path.Combine(dir, fileName2), Path.Combine(relativeDir, fileName2), includeSubdirectories, fileList);
                }
            }
        }

        private static IDictionary<string, string> CreateStringDictionary(IList<string> keys, IList<string> values)
        {
            IDictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            checked
            {
                for (int i = 0; i < keys.Count; i++)
                    dictionary.Add(keys[i], values[i]);
                return dictionary;
            }
        }


        /// <summary>
        ///     Expects the directory to contain an infopath manifest.xsf & template.xml files. The contents are then persisted &
        ///     indexed by DocTypeName & DocTypeRev (aka solutionVersion) for OpenStream & OpenText operations. As of this writing,
        ///     this application must have write access to the parent folder of the given directory for cab compression operations.
        /// </summary>
        /// <param name="importFolderPath"></param>
        /// <param name="workingFolderPath">default is parent of importFolderPath</param>
        public static List<ImporterLightDoc> ImportContentFolder(string sourceFolderPath, string workingFolderPath = null)
        {
            List<ImporterLightDoc> List_ImporterLightDoc = new List<ImporterLightDoc>();

            DirectoryInfo _DirectoryInfo = new DirectoryInfo(sourceFolderPath);

            if (workingFolderPath == null)
                workingFolderPath = RequestPaths.GetPhysicalApplicationPath("import");

            //// ensure the import folder actually exists
            //Task.Factory.StartNew(() =>{
            new DirectoryInfo(workingFolderPath)
                .mkdir()
                .Attributes = FileAttributes.NotContentIndexed | FileAttributes.Hidden;
            //});

            string DocMD5, DocTypeVer;
            string DocTypeName = ScanContentFolder(_DirectoryInfo, out DocTypeVer, out DocMD5);
            if (!ServiceController.LuceneController.List(new List<string> { "DOCREV" }, null, null, DocMD5).Any())
                try
                {
                    IList<string> relativeFilePathsInDirectoryTree = GetRelativeFilePathsInDirectoryTree(_DirectoryInfo.FullName, true);
                    IDictionary<string, string> files = CreateStringDictionary(relativeFilePathsInDirectoryTree, relativeFilePathsInDirectoryTree);
                    //the folder's contents compressed
                    string cabFilePath = string.Format(@"{0}\{1}_{2}.cab", workingFolderPath, DocTypeName, FileSystem.CleanFileName(DocTypeVer));

                    Dictionary<string, string> DocIdKeys = new Dictionary<string, string>
                    {
                        {"TargetDocTypeName", DocTypeName},
                        {"TargetDocTypeVer", DocTypeVer}
                    };

                    //TODO:Unit Test this! Generating in memory cab files has not been tested at all!!!
                    using (CompressionEngine _CompressionEngine = new CabEngine { CompressionLevel = CompressionLevel.Max })
                    using (ArchiveMemoryStreamContext _ArchiveMemoryStreamContext = new ArchiveMemoryStreamContext(cabFilePath, sourceFolderPath, files) { EnableOffsetOpen = true })
                    using (MemoryStream _TargetDocTypeFilesMemoryStream = new MemoryStream())
                    {
                        _CompressionEngine.Pack(_ArchiveMemoryStreamContext, files.Keys);

                        string fileName = Path.GetFileName(cabFilePath);
                        uint fileNameLength = (uint)fileName.Length + 1;
                        byte[] fileNameBytes = Encoding.Unicode.GetBytes(fileName);

                        using (MemoryStream CabFileMemoryStream = _ArchiveMemoryStreamContext.DictionaryStringMemoryStream.Values.First())
                        {
                            CabFileMemoryStream.Position = 0;
                            using (BinaryReader _BinaryReader = new BinaryReader(CabFileMemoryStream))
                            using (BinaryWriter _BinaryWriter = new BinaryWriter(_TargetDocTypeFilesMemoryStream))
                            {
                                // Write the InfoPath attachment signature. 
                                _BinaryWriter.Write(new byte[] { 0xC7, 0x49, 0x46, 0x41 });

                                // Write the default header information.
                                _BinaryWriter.Write((uint)0x14); // size
                                _BinaryWriter.Write((uint)0x01); // version
                                _BinaryWriter.Write((uint)0x00); // reserved

                                // Write the file size.
                                _BinaryWriter.Write((uint)_BinaryReader.BaseStream.Length);

                                // Write the size of the file name.
                                _BinaryWriter.Write(fileNameLength);

                                // Write the file name (Unicode encoded).
                                _BinaryWriter.Write(fileNameBytes);

                                // Write the file name terminator. This is two nulls in Unicode.
                                _BinaryWriter.Write(new byte[] { 0, 0 });

                                // Iterate through the file reading data and writing it to the outbuffer.
                                byte[] data = new byte[64 * 1024];
                                int bytesRead = 1;

                                while (bytesRead > 0)
                                {
                                    bytesRead = _BinaryReader.Read(data, 0, data.Length);
                                    _BinaryWriter.Write(data, 0, bytesRead);
                                }
                            }

                            // these contents will be stored in yet another document as an attached cab file
                            IDocRev_Gen2 DocRevBaseDoc = (IDocRev_Gen2)DocInterpreter.Instance.Create("DOCREV");

                            DocRevBaseDoc.DocChecksum = int.MinValue;
                            DocRevBaseDoc.DocIdKeys = DocIdKeys;
                            DocRevBaseDoc.DocStatus = true;
                            DocRevBaseDoc.DocTitle = String.Format("{0} {1}", DocTypeName, DocTypeVer);
                            DocRevBaseDoc.DocTypeName = "DOCREV";
                            DocRevBaseDoc.TargetDocTypeFiles = _TargetDocTypeFilesMemoryStream.ToArray();
                            DocRevBaseDoc.TargetDocTypeName = DocTypeName;
                            DocRevBaseDoc.TargetDocTypeVer = DocTypeVer;

                            /*
                            BANDAID: DOCREV
                            (search for this though out the code for more on DOCREV snafus in general),
                            earlier implementations of DOCREV did not have a TargetDocMD5 property, the IDocRev must be compatible with all versions of this object
                            */
                            foreach (PropertyInfo p in DocRevBaseDoc.GetType().GetProperties().Where(p => p.Name == "TargetDocMD5"))
                                p.SetValue(DocRevBaseDoc, DocMD5, null);

                            List_ImporterLightDoc.Add(
                                new ImporterLightDoc { LightDoc = ServiceController.Instance.Import(DocRevBaseDoc.GetDocData()) });
                        }
                    }
                }
                catch (ThreadAbortException) { }
                catch (Exception)
                {
                    //TODO:Need to handle this trapped exception correctly
                }
            return List_ImporterLightDoc;
        }

        /// <summary>
        ///     Scans AppDomain for classes implementing the IDocModel & performs all transformations needed to represent them as
        ///     BaseDoc to be served.
        /// </summary>
        /// <param name="DocTypeName">
        ///     Processes only the given DocTypeName the IDocModel represents. If a IDocModel can not be
        ///     located through out the AppDomain nothing will be processed & no IDocRev will be imported. If no DocTypeName is
        ///     specified all IDocModel located will be processed.
        /// </param>
        public static List<ImporterLightDoc> ImportDocModels(string DocTypeName = null)
        {
            List<ImporterLightDoc> List_ImporterLightDoc = new List<ImporterLightDoc>();

            JsonContentBuilder _JsonContentBuilder = new JsonContentBuilder();

            //TODO:Validate POCO utilizes correct title-case underscore separated labeling practices
            //TODO:add a placeholder file describing what goes in the given DocTypeName's form root directory
            foreach (var docTypeDirectoryInfo in
                AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Distinct()
                    .Where(typ =>
                           (typ.GetInterfaces().Any(i => i == typeof(IDocModel)))
                           &&
                           (string.IsNullOrWhiteSpace(DocTypeName) || typ.Name.ToLower() == DocTypeName.ToLower()))
                    .Select(type => new
                    {
                        type,
                        DirectoryInfo = new DirectoryInfo(FilesystemTemplateController.GetWorkingFileSystemPath(type.Name)).mkdir(),
                        myschemaXsd = XsdExporter.ExportSchemas(
                            type.Assembly,
                            new List<string> { type.Name },
                            string.Format("http://schemas.progablab.com/{0}", type.Name)).First()
                    })
                )
            {
                string filepath = string.Format(@"{0}myschema.xsd", docTypeDirectoryInfo.DirectoryInfo.FullName);

                // always (over)write the xsd as this will always be generated by and for dCForm.Core regardless of the IDocInterpreter that is handling
                if (!File.Exists(filepath) || docTypeDirectoryInfo.myschemaXsd != File.ReadAllText(filepath))
                    File.WriteAllText(string.Format(@"{0}myschema.xsd", docTypeDirectoryInfo.DirectoryInfo.FullName), docTypeDirectoryInfo.myschemaXsd);

                string DocMD5, DocTypeVer;

                ScanContentFolder(docTypeDirectoryInfo.DirectoryInfo, out DocTypeVer, out DocMD5);

                // since the DocMD5 is so unique there is no reason to be specific about when querying it from the datastore
                if (!ServiceController.LuceneController.List(new List<string> { "DOCREV" }, null, null, DocMD5).Any())
                {
                    docTypeDirectoryInfo.DirectoryInfo.rAttrib(FileAttributes.NotContentIndexed);

                    // re-version this doc template set if there exists another template registered with the same version numver
                    string DocTypeVerNew = _NosqlTemplateController.OpenRead(docTypeDirectoryInfo.type.Name, DocTypeVer) == null
                        ? DocTypeVer
                        : DateTime.UtcNow.AsDocRev();

                    // this implicitly established the solutionVersion also as the solutionVersion is tightly connected to the namespacing
                    string temporaryNamespace = RuntimeTypeNamer.CalcCSharpFullname(docTypeDirectoryInfo.type.Name, DocTypeVerNew, "ImportDocModel");

                    string myclasses_cs = new Xsd().ImportSchemasAsClasses(
                        new[] { docTypeDirectoryInfo.myschemaXsd },
                        temporaryNamespace,
                        CodeGenerationOptions.GenerateOrder | CodeGenerationOptions.GenerateProperties,
                        new StringCollection());

                    myclasses_cs = Runtime.CustomizeXsdToCSharpOutput(docTypeDirectoryInfo.DirectoryInfo.Name, myclasses_cs);
                    BaseDoc _BaseDoc = Runtime.FindBaseDoc(Runtime.CompileCSharpCode(myclasses_cs, temporaryNamespace), docTypeDirectoryInfo.DirectoryInfo.Name);

                    _BaseDoc.solutionVersion = DocTypeVerNew;
                    _BaseDoc.DocTypeName = docTypeDirectoryInfo.DirectoryInfo.Name;

                    foreach (KeyValuePair<string, string> item in _JsonContentBuilder.ContentFiles(_BaseDoc, docTypeDirectoryInfo.DirectoryInfo))
                    {
                        filepath = Regex.Replace(string.Format(@"{0}\{1}", docTypeDirectoryInfo.DirectoryInfo.FullName, item.Key), @"[\\/]+", @"\", RegexOptions.IgnoreCase);
                        if (!File.Exists(filepath))
                            while (true)
                                try
                                {
                                    File.WriteAllText(filepath, item.Value);
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    Thread.Sleep(5000);
                                }
                    }

                    List_ImporterLightDoc.AddRange(ImportContentFolder(docTypeDirectoryInfo.DirectoryInfo.FullName));

                    TemplateController.Instance.TopDocRev(docTypeDirectoryInfo.DirectoryInfo.Name, true);

                    // notice the true parameter value to clear the cache as well as assert we have the correct DocRev in the system now
                    if (TemplateController.Instance.TopDocRev(docTypeDirectoryInfo.DirectoryInfo.Name, true) != DocTypeVerNew)
                        throw new PocosImportException();
                }
            }


            return List_ImporterLightDoc;
        }


        /// <summary>
        ///     Intended time of use it that of a "post cache reset" time. If cache has been cleared, all the logic behind this
        ///     method will run. Also creates SQL structures by issuing a save with the default values & a DocKey stating the date
        /// </summary>
        public static void ImportDocModelsRunOnce() { CacheMan.Cache(() => ImportDocModels(), false, "ImportDocModelsRunOnce"); }

        /// <summary>
        ///     submit docxml from a file bypass Xml validation
        /// </summary>
        /// <param name="FullPath"></param>
        internal static ImporterLightDoc ImportFile(string FullPath)
        {
            List<ImporterLightDoc> List_LightDoc = GetImporterLightDocList(FullPath);
            ImporterLightDoc _LightDoc = List_LightDoc.FirstOrDefault(m => m.ImportDocSrc == FullPath);
            bool previouslyErrored = _LightDoc != null && !string.IsNullOrWhiteSpace(_LightDoc.ExceptionMessage);

            if (_LightDoc == null || previouslyErrored)
                try
                {
                    string DocXml = File.ReadAllText(FullPath);
                    _LightDoc = new ImporterLightDoc
                    {
                        LightDoc = ServiceController.Instance.Import(DocXml)
                    };
                    _LightDoc.LightDoc.DocSrc = _LightDoc.LightDoc.DocSrc;
                }
                catch (SubmitDeniedException) { }
                catch (Exception ex)
                {
                    _LightDoc = new ImporterLightDoc
                    {
                        LightDoc = new LightDoc
                        {
                            DocSubmitDate = DateTime.Now
                        },
                        ExceptionMessage = ex.Message
                    };
                    if (ex.InnerException != null)
                        _LightDoc.ExceptionMessage += " " + string.Format("{0}", ex.InnerException.Message);
                }
                finally
                {
                    _LightDoc.ImportDocSrc = FullPath;

                    List_LightDoc.Add(_LightDoc);
                    if (!previouslyErrored || string.IsNullOrWhiteSpace(_LightDoc.ExceptionMessage))
                        if (string.Format("{0}", _LightDoc.ExceptionMessage).IndexOf("skipped") == -1)
                            SaveImporterLightDocList(List_LightDoc);
                }

            return _LightDoc;
        }

        public static List<ImporterLightDoc> ImportFolder()
        {
            if (string.IsNullOrWhiteSpace(DirectoryFullName))
                lock (DirectoryFullName)
                {
                    if (!Directory.Exists(DirectoryFullName))
                        return new List<ImporterLightDoc>();

                    string s = DirectoryFullName;

                    return Directory
                        .EnumerateFiles(s, "*.xml")
                        .OrderBy(m => new Rand().int32(DateTime.Now))
                        .OrderBy(m => m.IndexOf("DOCREV") < 0)
                        .Select(m => ImportFile(m))
                        .Where(o => o != null)
                        .ToList();
                }

            return new List<ImporterLightDoc>();
        }

        public static void SaveImporterLightDocList(List<ImporterLightDoc> List_ImporterLightDoc)
        {
            foreach (string FilePath in List_ImporterLightDoc.Select(m => GetLogFilePath(m.ImportDocSrc)).Distinct())
                File.WriteAllText(
                    FilePath,
                    JsonConvert.SerializeObject(
                        List_ImporterLightDoc.Where(m => GetLogFilePath(m.ImportDocSrc) == FilePath).ToList(),
                        Formatting.Indented,
                        _JsonSerializerSettings));
        }

        /// <summary>
        /// </summary>
        /// <param name="_DirectoryInfo"></param>
        /// <param name="TargetDocTypeVer"></param>
        /// <param name="TargetDocMD5"></param>
        /// <returns>DocTypeVer only if the DocTypeName can be interpreted from a same file</returns>
        private static string ScanContentFolder(DirectoryInfo _DirectoryInfo, out string TargetDocTypeVer, out string TargetDocMD5)
        {
            TargetDocTypeVer = string.Empty;
            TargetDocMD5 = FileSystem.calcMd5(_DirectoryInfo.FullName);
            string TargetDocTypeName = string.Empty;

            // run through all the text files at the root of the target directory and try to resolve there revision number/string
            foreach (FileInfo filepath in _DirectoryInfo.EnumerateFiles())
            {
                if (string.IsNullOrWhiteSpace(TargetDocTypeName))
                    try
                    { TargetDocTypeName = DocInterpreter.Instance.ReadDocTypeName(File.ReadAllText(filepath.FullName)); }
                    catch (Exception) { }

                if (string.IsNullOrWhiteSpace(TargetDocTypeVer))
                    try
                    { TargetDocTypeVer = DocInterpreter.Instance.ReadRevision(File.ReadAllText(filepath.FullName)); }
                    catch (Exception) { }

                if (!string.IsNullOrWhiteSpace(TargetDocTypeName) && !string.IsNullOrWhiteSpace(TargetDocTypeVer))
                    break;
            }

            if (string.IsNullOrWhiteSpace(TargetDocTypeName) || string.IsNullOrWhiteSpace(TargetDocTypeVer))
            {
                TargetDocTypeName = string.Empty;
                TargetDocTypeVer = string.Empty;
            }

            return TargetDocTypeName;
        }
    }
}