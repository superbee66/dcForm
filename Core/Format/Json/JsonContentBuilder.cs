using System.Collections.Generic;
using System.IO;
using System.Linq;
using dCForm.Core.Util;
using Newtonsoft.Json;

namespace dCForm.Core.Format.Json
{
    /// <summary>
    ///     All/Any methods return the same things; IDocDataInterpreter specific "template/content" files that support a
    ///     development efforts with a good starting point. All ContentFiles methods should be deterministic in nature; if
    ///     called multiple times with the same exact method parameters the results should match byte for byte.
    /// </summary>
    public class JsonContentBuilder
    {
        private static readonly IDocTextInterpreter _Interpreter = new JsonInterpreter();

        /// <summary>
        ///     The IDocDataInterpreter that when Processable(string,string) is called for the items build will return true
        /// </summary>
        public IDocTextInterpreter Interpreter {
            get { return _Interpreter; }
        }


        /// <summary>
        ///     Similar to Entity Framework Code First in concept, produce content files given a csharp object that represents the
        ///     doc type.
        /// </summary>
        /// <param name="baseDoc"></param>
        /// <param name="directoryInfo">
        ///     The folder these files will be written to if any content "merging" logic needs to take
        ///     place.
        /// </param>
        /// <returns>Relative filename path/name items & content</returns>
        public Dictionary<string, string> ContentFiles(BaseDoc baseDoc, DirectoryInfo directoryInfo)
        {
            string solutionVersion = baseDoc.solutionVersion;
            FileInfo _ExistingTemplateJsonFileInfo = new FileInfo(string.Format(@"{0}\template.json", directoryInfo.FullName));

            // if there is an existing template.js the developer created, try to use as much of it as we can to write a new template.json that will accommodate the properties of the baseDoc passed
            string _TemplateJson = _ExistingTemplateJsonFileInfo.Exists
                                       ? File.ReadAllText(_ExistingTemplateJsonFileInfo.FullName)
                                       : _Interpreter.Write(new Rand().obj(baseDoc, "template.json"));

            //strip away any properties that may not be compatible datatype-wise with previous version of template.js & the new object
            string tempalte_json = _Interpreter.WritePI(
                _Interpreter.Write(
                    (BaseDoc)JsonConvert.DeserializeObject(
                        _TemplateJson,
                        baseDoc.GetType(),
                        new JsonSerializerSettings { Error = (o, args) => { args.ErrorContext.Handled = true; } })),
                new DocProcessingInstructions
                {
                    DocChecksum = 8678309,
                    DocTypeName = baseDoc.GetType().Name,
                    solutionVersion = solutionVersion,
                    DocTitle = string.Format("{0} {1}, Base Line Data", baseDoc.DocTypeName, baseDoc.solutionVersion),
                    DocId = DocKey.DocIdFromKeys(
                        new Dictionary<string, string>
                        {
                            {"DocTitle", baseDoc.DocTitle},
                            {"DocTypeName", baseDoc.DocTypeName},
                            {"solutionVersion", baseDoc.solutionVersion}
                        })
                });


            Dictionary<string, string> _Dictionary = new Dictionary<string, string>
            {
                {
                    "template.json", tempalte_json
                },
                {
                    "default.htm", "<!-- TODO: add example code that could generically bind to the template.json to as an example -->"
                },
                {
                    "readme.txt", "TODO: add overview & links to resources"
                }
            };


            string[] files = Directory
                .EnumerateFiles(directoryInfo.FullName, "*.*", SearchOption.AllDirectories)
                .Union(_Dictionary.Keys)
                .Select(m => string.Format("/{0}", m.Replace(directoryInfo.FullName, string.Empty).Replace("\\", "/").TrimStart('/', '\\')))
                .Distinct()
                .OrderBy(m => m)
                .ToArray();

            File.WriteAllText(_ExistingTemplateJsonFileInfo.FullName, tempalte_json);
            return _Dictionary;
        }
    }
}