using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using dCForm.Client.Util;
using dCForm.Template.Embeded;
using dCForm.Template.Filesystem;
using dCForm.Util;

namespace dCForm.Template
{
    /// <summary>
    ///     Handles file content requests on a per DocTypeName & DocRev basis. An orderly search of the local file system
    ///     (DirectoryPath/{DocTypeName}/*.*), the NOSQL datastore (LuceneController) and lastly any other
    ///     IDocResourceController in memory (dCForm.Forms.DocResourceController from it's embedded contents) are searched.
    ///     First one to return something wins.
    /// </summary>
    public class TemplateController : ITemplateController
    {
        /// <summary>
        ///     when this file is requested the entire contents of the given form/doctypename/docrev/*.* will be compressed & sent
        ///     down the wire if "doc/doctypename/docrev/mycontents.cab" is specified
        /// </summary>
        public const string FOLDER_CONTENTS_VIRTUAL_CAB_FILE = "mycontents.cab";

        private static readonly Lazy<TemplateController> _Instance = new Lazy<TemplateController>(() => new TemplateController());

        /// <summary>
        ///     the default controller that will be queried first for the TopDoc
        /// </summary>
        private static readonly FilesystemTemplateController _DefaultTopDocFilesystemTemplateController = new FilesystemTemplateController();

        /// <summary>
        ///     Aids in migrating existing documents request for there content files before the DocTypeVer pattern was implemented.
        ///     At the time of this writing documents server prior did not request a specific rev number (for infopath this is
        ///     the solutionVersion). Thereafter processing instructions are written with an explicit rev number in there href
        ///     to the manifest.
        /// </summary>
        private static readonly Dictionary<string, string> DocTypeNameDocTypeVerDefault = new Dictionary<string, string> { };

        private static ITemplateController[] _OtherIDocResourceControllers;

        /// <summary>
        ///     Seeds the underlying datastore with a single DOCREV document if it does not exist. Without this document not would
        ///     exist.  Scans current AppDomain for BaseDoc instances & fabricates a form/* folder with what is now the emerging
        ///     technique
        ///     of serving the forms, the JsonInterpreter.
        ///     //TODO:Require a custom class attribute to direct this on how to process it
        /// </summary>
        private TemplateController()
        {
            Reflection.LoadBinDlls();

            // locate other instances of IDocResourceControllers available for fallback
            _OtherIDocResourceControllers = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(_Assembly => _Assembly.GetTypes(), (_Assembly, _Type) => new
                {
                    _Assembly,
                    _Type
                })
                .Where(t => t._Type != GetType())
                .Where(t => !t._Type.IsInterface)
                .Where(t => t._Type.GetInterfaces().Any(i => i == typeof(ITemplateController)))
                .Select(t => ((ITemplateController)Activator.CreateInstance(t._Type)))
                .ToArray();
        }

        /// <summary>
        ///     singleton instance safe for multithreading
        /// </summary>
        public static TemplateController Instance {
            get { return _Instance.Value; }
        }


        /// <summary>
        ///     Checks contents processed & persisted by ImportInfoPathXsnContents to find the files first. Goes to the
        ///     ~/forms/[DocTypeName] directory if nothing was found in the previous. If the physical disk-based folder yields the
        ///     requested contents, that content will be of whatever DocRev stored there.
        /// </summary>
        /// <param name="DocTypeName"></param>
        /// <param name="DocTypeVer"></param>
        /// <param name="filename"></param>
        /// <returns>stream of file requested or it's head version if the requested version can't be found</returns>
        public MemoryStream OpenRead(string DocTypeName, string DocTypeVer, string filename)
        {
            MemoryStream _MemoryStream = null;

            foreach (var _OtherIDocResourceController in _OtherIDocResourceControllers
                .Where(m => (DocTypeName == "DOCREV") == (typeof(EmbededTemplateController) == m.GetType())))
                if (_MemoryStream != null)
                    break;
                else
                    _MemoryStream = _OtherIDocResourceController.OpenRead(DocTypeName, DocTypeVer, filename);
            return _MemoryStream;
        }

        public string TopDocRev(string DocTypeName) { return TopDocRev(DocTypeName, false); }


        private static string GetHttpContextFileName(HttpContext context) { return context.Request.Url.Segments[context.Request.Url.Segments.Length - 1].Trim('/'); }

        /// <summary>
        ///     searches other appdomain assemblies for IDocResourceControllers to ask them if they have anything
        /// </summary>
        /// <param name="DocTypeName"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public MemoryStream OpenRead(string DocTypeName, string filename) { return OpenRead(DocTypeName, TopDocRev(DocTypeName), filename); }

        public MemoryStream OpenRead(HttpContext context, out string filename)
        {
            filename = GetHttpContextFileName(context);
            string DocTypeVer = context.Request.Url.Segments[context.Request.Url.Segments.Length - 2].Trim('/');
            string DocTypeName = context.Request.Url.Segments[context.Request.Url.Segments.Length - 3].Trim('/');

            //http://blah.blah/form/FORM001/manifest.xsf
            //trying to get----^
            if (DocTypeName.ToLower() == FilesystemTemplateController.DirectoryName)
            {
                DocTypeName = DocTypeVer;
                DocTypeVer =
                    DocTypeNameDocTypeVerDefault.ContainsKey(DocTypeName)
                        ? DocTypeNameDocTypeVerDefault[DocTypeName]
                        : TopDocRev(DocTypeName);
            }

            return OpenRead(DocTypeName, DocTypeVer, filename);
        }

        public string OpenText(string DocTypeName, string filename)
        {
            return OpenText(
                DocTypeName,
                TopDocRev(DocTypeName),
                filename);
        }

        public string OpenText(string DocTypeName, string DocTypeVer, string filename)
        {
            return CacheMan.Cache(() =>
                                  {
                                      using (MemoryStream _MemoryStream = OpenRead(DocTypeName, DocTypeVer, filename))
                                          return _MemoryStream.AsString();
                                  }, false, "OpenText", DocTypeName, DocTypeVer, filename);
        }

        public string OpenText(HttpContext context, out string filename)
        {
            filename = GetHttpContextFileName(context);
            return CacheMan.Cache(() =>
                                  {
                                      string r;
                                      using (MemoryStream _MemoryStream = OpenRead(context, out r))
                                          return _MemoryStream.AsString();
                                  }, false, "OpenText", context.Request.Url.ToString());
        }

        /// <summary>
        ///     Reads the DOCREV from the local AppDomain working_folder\form\*. When nothing is found the nosql store for the most
        ///     current DOCREV. The first item in descending string order.
        /// </summary>
        /// <param name="DocTypeName"></param>
        /// <returns>string.Empty if nothing is found</returns>
        public string TopDocRev(string DocTypeName, bool forceRefresh)
        {
            //TODO:change solutionVersion properties to System.Version
            return CacheMan.Cache(() =>
                                  _DefaultTopDocFilesystemTemplateController.TopDocRev(DocTypeName)
                                        ?? _OtherIDocResourceControllers
                                        //DOCREVs should always come from the embedded controller
                                        .Where(m => (DocTypeName == "DOCREV") == (typeof(EmbededTemplateController) == m.GetType()))
                                            .Select(m => m.TopDocRev(DocTypeName))
                                            .Where(DocRev => !string.IsNullOrWhiteSpace(DocRev))
                                            .OrderByDescending(DocRev => new Version(DocRev))
                                            .ToArray()
                                            .FirstOrDefault(),
                forceRefresh,
                "TopDocRev",
                DocTypeName);
        }
    }
}