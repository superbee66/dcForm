using System;
using System.Reflection;
using dCForm.Util;
using dCForm.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dCForm.Format.Json
{
    /// <summary>
    ///     Supports a home grown document type that is strongly coupled with the Json serialization format.
    /// </summary>
    public class JsonInterpreter : IDocTextInterpreter
    {
        private static readonly BaseJsonSerializerSettings
            PIInclusionSettings = new BaseJsonSerializerSettings { ContractResolver = new WithPIContractResolver() },
            PIExclutionSettings = new BaseJsonSerializerSettings { ContractResolver = new WithoutPIContractResolver() },
            PIOnlySettings = new BaseJsonSerializerSettings { ContractResolver = new OnlyPiContractResolver() };

        public string ContentType {
            get { return "application/json"; }
        }

        public string ContentFileExtension {
            get { return "json"; }
        }

        public BaseDoc Read(string DocJson, bool DocRevStrict = false)
        {
            string DocTypeName = ReadDocTypeName(DocJson);
            string DocRev = DocRevStrict
                                ? ReadRevision(DocJson)
                                : TemplateController.Instance.TopDocRev(DocTypeName);

            Type type = Runtime.ActivateBaseDoc(DocTypeName, DocRev).GetType();

            BaseDoc o = (BaseDoc)JsonConvert.DeserializeObject(DocJson, type, PIInclusionSettings);
            o.solutionVersion = DocRev;

            return o;
        }

        public DocProcessingInstructions ReadDocPI(string srcDocData) { return JsonConvert.DeserializeObject<DocProcessingInstructions>(srcDocData, PIInclusionSettings); }

        public string WritePI(string srcDocData, DocProcessingInstructions pi)
        {
            JObject DocDataJObject = JObject.Parse(srcDocData);
            JObject PIJObject = JObject.FromObject(
                pi,
                new JsonSerializer
                {
                    ContractResolver = PIOnlySettings.ContractResolver
                });

            DocDataJObject.Merge(PIJObject);
            return DocDataJObject.ToString();
        }

        public string GetDescription(string DocJson) { return "NotImplementedException"; }

        public string ReadDocTypeName(string DocJson) { return SelectToken(DocJson, "DocTypeName"); }

        public string ReadRevision(string DocJson) { return SelectToken(DocJson, "solutionVersion"); }

        public string Write<T>(T doc, bool includeProcessingInformation = true) where T : DocProcessingInstructions
        {
            return JsonConvert.SerializeObject(
                doc,
                Formatting.Indented,
                includeProcessingInformation
                    ? PIInclusionSettings
                    : PIExclutionSettings);
        }

        public BaseDoc Create(string DocTypeName) { return Read(TemplateController.Instance.OpenText(DocTypeName, "template.json")); }

        public bool Processable(string DocTypeName, string DocRev)
        {
            string template_json = TemplateController.Instance.OpenText(DocTypeName, DocRev, "template.json");
            return ReadDocTypeName(template_json) == DocTypeName && ReadRevision(template_json) == DocRev;
        }


        public void Validate(string DocJson) { }

        public string HrefVirtualFilename(string DocTypeName, string DocRev) { return TemplateController.FOLDER_CONTENTS_VIRTUAL_CAB_FILE; }

        private static bool IsValidJson(string strInput)
        {
            strInput = string.Format("{0}", strInput).Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
                try
                {
                    JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            return false;
        }

        public string ReadFileName(string DocJson) { return string.Format("{0}.json", FileSystem.CleanFileName(SelectToken(DocJson, "DocTitle")).Trim()); }

        public string RemoveProcessingInformation(string srcDocJson)
        {
            foreach (PropertyInfo _PropertyInfo in typeof(DocProcessingInstructions).GetProperties())
                if (_PropertyInfo.CanWrite)
                    srcDocJson = RemoveToken(srcDocJson, _PropertyInfo.Name);
            return srcDocJson;
        }

        private static string RemoveToken(string DocJson, string token)
        {
            //TODO:Watch the performance of RemoveToken
            if (!string.IsNullOrWhiteSpace(SelectToken(DocJson, token)))
            {
                JObject _JObject = JObject.Parse(DocJson);
                _JObject.Remove(token);
                DocJson = _JObject.ToString();
            }
            return DocJson;
        }

        private static string SelectToken(string DocJson, string token)
        {
            return IsValidJson(DocJson)
                       ? (string)JObject.Parse(DocJson).SelectToken(token)
                       : string.Empty;
        }

        private class BaseJsonSerializerSettings : JsonSerializerSettings
        {
            public BaseJsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Include;
                MissingMemberHandling = MissingMemberHandling.Ignore;
                Error = (o, args) => { args.ErrorContext.Handled = true; };
            }
        }
    }
}