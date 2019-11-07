using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using dCForm.Client.Util;
using dCForm.Template;

namespace dCForm.Format.XsnXml
{
    internal class SchemaValidator
    {
        private static readonly Regex MatchFieldVal = new Regex("(')(?<schemaNfield>.*)(')(?<desc>.*)");
        private readonly List<string> ValidationMessages = new List<string>();
        private string ValidatingNamespace = string.Empty;

        private void _XmlValidatingReader_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Message.Contains(ValidatingNamespace))
                if (MatchFieldVal.IsMatch(e.Message))
                    ValidationMessages.Add(e.Message);
        }

        /// <summary>
        ///     Runs a given form's xml schema against it throwing an exception if it fails to validate
        /// </summary>
        /// <param name= NavKey.DocTypeName></param>
        /// <param name="xml"></param>
        internal void Validate(string DocData)
        {
            BaseDoc _BaseDoc = DocInterpreter.Instance.Read(DocData, true);

            if (_BaseDoc.DocIdKeys.Count == 0)
                throw new Exception("No DocIdKeys have been defined");

            Type t = _BaseDoc.GetType();

            using (StringReader _StringReader = new StringReader(DocData))
            using (XmlTextReader _XmlTextReader = new XmlTextReader(_StringReader))
            using (XmlValidatingReader _XmlValidatingReader = new XmlValidatingReader(_XmlTextReader)
            {
                ValidationType = ValidationType.Schema
            })
                //TODO:Use XmlReader to perform validation instead of XmlValidatingReader (http://msdn.microsoft.com/en-us/library/hdf992b8%28v=VS.80%29.aspx)
            {
                // Grab the xml namescape that was expressed as an attribute of the class the XsnTransform.cmd auto generated
                ValidatingNamespace = t
                    .GetCustomAttributes(false)
                    .OfType<DataContractAttribute>()
                    .FirstOrDefault()
                    .Namespace;

                using (StringReader _StringReaderXsd = new StringReader(TemplateController.Instance.OpenText(_BaseDoc.DocTypeName, _BaseDoc.solutionVersion, "myschema.xsd")))
                using (XmlTextReader _XmlTextReaderXsd = new XmlTextReader(_StringReaderXsd))
                {
                    // Add that class into .Net XML XSD schema validation
                    _XmlValidatingReader.Schemas.Add(ValidatingNamespace, _XmlTextReaderXsd);

                    _XmlValidatingReader.ValidationEventHandler += _XmlValidatingReader_ValidationEventHandler;
                    //Start validating

                    while (_XmlValidatingReader.Read()) { }
                }
            }

            if (ValidationMessages.Count > 0)
            {
                List<string> FieldErrors = new List<string>();

                Regex regexObj = new Regex(@"http://[^']+:(\w+)' element has an invalid value according to its data type", RegexOptions.IgnoreCase | RegexOptions.Multiline);

                foreach (string _T in ValidationMessages)
                    if (_T.Contains("Signature(s)"))
                        FieldErrors.Add(_T);
                    else if (regexObj.IsMatch(_T))
                        FieldErrors.Add(StringTransform.Wordify(regexObj.Match(_T).Groups[1].Value.Trim().Trim(':')));
                    else
                        foreach (PropertyInfo p in t.GetProperties())
                            if (Regex.IsMatch(_T, string.Format(@"\b(?=\w){0}\b(?!\w)", p.Name)))
                                FieldErrors.Add(StringTransform.Wordify(p.Name).Trim().Trim(':'));

                if (FieldErrors.Count > 0)
                {
                    string ValidationMessageMarkDown =
                        string.Format(
                            "\t\t{0}",
                            string.Join("\r\n\t\t", FieldErrors.Where(m => !string.IsNullOrWhiteSpace(m)).Distinct()));

                    int ValidationMessagesCount = FieldErrors.Count;
                    ValidationMessages.Clear();

                    throw new Exception(
                        string.Format(
                            "TODO:Put back this valiation message from repo as I deleted the resx on accident",
                            ValidationMessagesCount,
                            ValidationMessageMarkDown));
                }
            }
            ValidationMessages.Clear();
        }
    }
}