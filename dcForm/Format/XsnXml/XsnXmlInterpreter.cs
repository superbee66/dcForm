using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using dCForm.Template;
using dCForm.Util;
using Newtonsoft.Json;

namespace dCForm.Format.XsnXml
{
    /// <summary>
    ///     Support InfoPath documents
    /// </summary>
    public class XsnXmlInterpreter : IDocTextInterpreter
    {
        public const string mso_infoPathSolution = "solutionVersion=\"{0}\" productVersion=\"14.0.0\" PIVersion=\"1.0.0.0\" href=\"{1}\" name=\"{2}\""; //TASK: Code stub the solution version so the InfoPath UI does not complain
        public const string mso_application = "progid=\"InfoPath.Document\" versionProgid=\"InfoPath.Document.3\"";
        public const string ipb_application = "DocId=\"{0}\" DocTitle=\"{1}\" DocTypeName=\"{2}\" DocChecksum=\"{3}\"";
        private const string XmlProcessingInstructionMatch = @"<\?.*\?>";
        internal const string XmlRootAttributeNamespaces = @"(?:xmlns:)(\w+)(?:="")([^""]+)";

        /// <summary>
        ///     "One XML processing instruction tag named mso-infoPathSolution MUST be specified as part of the form file. This XML
        ///     processing instruction tag specifies properties, as defined by the following attributes, of this form file and the
        ///     associated form template."
        /// </summary>
        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <summary>
        ///     The value from the key specified by {0} parsed from the XML processing instructions
        /// </summary>
        private const string parseRegex = @"(?<=(^|[^\w]+){0}="")([^""""]+)(?="".*)";

        /// <summary>
        ///     matches 0001-01-01T00:00:00, 0 & false; things considered default values in this solution
        /// </summary>
        private static readonly Regex
            DocXmlInvalidDateStringRegEx = new Regex("0001-01-01T00:00:00[^<]*", RegexOptions.IgnoreCase),
            DocXmlDefaultValueRegEx = new Regex("(?<=>)(0001-01-01T00:00:00[^<]*)(?=<)", RegexOptions.IgnoreCase),
            XmlEmptyTagWithoutNullableElementsRegEx = new Regex(@"<(([_:A-Za-z][-._:A-Za-z0-9]*))\s*(/>|>\s*</\2>)", RegexOptions.Singleline | RegexOptions.Multiline);

        private static readonly List<Type> XsdIntEgerFixDstBaseDocKnownTypes = new List<Type>();

        public string ContentFileExtension {
            get { return "xml"; }
        }

        public string ReadDocTypeName(string DocData) { return internalReadDocTypeName(DocData); }

        /// <summary>
        /// </summary>
        /// <param name="DocData"></param>
        /// <param name="DocRevStrict"></param>
        /// <returns></returns>
        public BaseDoc Read(string DocData, bool DocRevStrict = false)
        {
            if (String.IsNullOrWhiteSpace(DocData)) return null;

            string
                CollapsedElementsDocXml = CollapseDefaultValueElements(DocData),
                DocTypeName = ReadDocTypeName(DocData),
                DocRev = ReadRevision(DocData);

            BaseDoc dstBaseDoc = null;

            Type BaseDocType = Runtime.ActivateBaseDoc(
                DocTypeName,
                DocRevStrict
                    ? DocRev
                    : TemplateController.Instance.TopDocRev(DocTypeName)).GetType();

            try
            {
                dstBaseDoc = XsdIntEgerFixDstBaseDocKnownTypes.Contains(BaseDocType)
                                 ? TryXsdIntEgerFixDstBaseDoc(DocRevStrict, DocTypeName, DocRev, CollapsedElementsDocXml, BaseDocType)
                                 : (BaseDoc)Serialization.ReadObject(CollapsedElementsDocXml, BaseDocType);
                //} catch (InvalidOperationException)
            }
            catch (Exception)
            {
                dstBaseDoc = TryXsdIntEgerFixDstBaseDoc(DocRevStrict, DocTypeName, DocRev, CollapsedElementsDocXml, BaseDocType);

                if (dstBaseDoc == null)
                    throw;

                XsdIntEgerFixDstBaseDocKnownTypes.Add(dstBaseDoc.GetType());
            }

            dstBaseDoc = (BaseDoc)PropertyOverlay.Overlay(ReadDocPI(CollapsedElementsDocXml), dstBaseDoc);

            return dstBaseDoc;
        }

        /// <summary>
        ///     parses XML processing instructions also
        /// </summary>
        /// <param name="SrcDocXml"></param>
        /// <param name="DstBaseDoc"></param>
        /// <returns></returns>
        public DocProcessingInstructions ReadDocPI(string SrcDocXml)
        {
            DocProcessingInstructions _DocProcessingInstructions = new DocProcessingInstructions();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(SrcDocXml);
            foreach (XmlProcessingInstruction node in doc.ChildNodes.Cast<XmlNode>().Where(m => m is XmlProcessingInstruction))
                foreach (var _Kv in typeof(DocProcessingInstructions)
                    .GetProperties()
                    .Select(m => new
                    {
                        property = m,
                        matcher = new Regex(String.Format(parseRegex,
                            m.Name))
                    }))
                    if (_Kv.property.CanWrite)
                        if (_Kv.matcher.IsMatch(node.InnerText))
                            _Kv.property.SetValue(
                                _DocProcessingInstructions,
                                Convert.ChangeType(
                                    _Kv
                                        .matcher
                                        .Match(node.InnerText)
                                        .Value,
                                    ExpressionParser
                                        .GetNonNullableType(_Kv.property.PropertyType),
                                    null),
                                null);


            _DocProcessingInstructions.solutionVersion = GetDocRev(SrcDocXml);

            return _DocProcessingInstructions;
        }

        public string WritePI(string srcDocData, DocProcessingInstructions _ManifestInfo)
        {
            return string.Format(
                "{0}{1}",
                Write(_ManifestInfo),
                Regex.Replace(srcDocData, XmlProcessingInstructionMatch, ""));
        }

        /// <summary>
        ///     Parses the given form's "solutionVersion" number from the manifest.xsf. Note,
        ///     this string will change every time
        /// </summary>
        /// <param name= NavKey.DocTypeName></param>
        /// <returns></returns>
        public string GetDescription(string DocTypeName) { return ParseAttributeValue(TemplateController.Instance.OpenText(DocTypeName, "manifest.xsf"), "description"); }

        /// <summary>
        ///     Parses the given form's "solutionVersion" number from the manifest.xsf. Note,
        ///     this string will change every time
        /// </summary>
        /// <param name= NavKey.DocTypeName></param>
        /// <returns></returns>
        public string ReadRevision(string DocData) { return ParseAttributeValue(DocData, "solutionVersion"); }

        /// <summary>
        ///     Renders an XML document using an XmlSerializer, applies the given DocTypeName's template.xml
        ///     applies xml namespaces from that template.xml to the new text rendered & optional
        ///     xml processing instructions specific to InfoPath & custom to dCForm.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns>Initial document InfoPath Form Filler will open</returns>
        public string Write<T>(T source, bool includeProcessingInformation = true) where T : DocProcessingInstructions
        {
            using (StringWriter _StringWriter = new StringWriter())
            using (XmlTextWriter _XmlTextWriter = new XmlTextWriter(_StringWriter))
            {
                if (includeProcessingInformation)
                {
                    _XmlTextWriter.WriteProcessingInstruction("mso-infoPathSolution",
                        String.Format(
                            mso_infoPathSolution,
                            source.solutionVersion ?? TemplateController.Instance.TopDocRev(source.DocTypeName),
                            source.href,
                            source.name));

                    _XmlTextWriter.WriteProcessingInstruction("mso-application", mso_application);

                    // there is special instructions for attachments
                    if (TemplateController.Instance.OpenText(source.DocTypeName, source.solutionVersion, "template.xml").IndexOf("mso-infoPath-file-attachment-present") > 0)
                        _XmlTextWriter.WriteProcessingInstruction("mso-infoPath-file-attachment-present", String.Empty);

                    _XmlTextWriter.WriteProcessingInstruction("ipb-application",
                        String.Format(ipb_application,
                            source.DocId,
                            source.DocTitle,
                            source.DocTypeName,
                            source.DocChecksum));
                }

                //TODO:Cache _XmlSerializerNamespaces
                XmlSerializerNamespaces _XmlSerializerNamespaces = new XmlSerializerNamespaces();
                foreach (Match xmlnsMatch in Regex.Matches(TemplateController.Instance.OpenText(source.DocTypeName, source.solutionVersion, "template.xml"), XmlRootAttributeNamespaces))
                    _XmlSerializerNamespaces.Add(xmlnsMatch.Groups[1].Value, xmlnsMatch.Groups[2].Value);

                if (source is BaseDoc)
                    new XmlSerializer(source.GetType()).Serialize(_XmlTextWriter, source, _XmlSerializerNamespaces);

                //TODO:Move regex logic to CollapseDefaultValueElements
                //TODO:Explorer the now open-source serializer to see how they detect uninitialized properties & tell the property overlay to leave them alone.
                return RemoveInvalidDateElementText(
                    source.DocStatus == null // tells us we just rendered this xml, it has not been through the infopath desktop application
                        ? RemoveValueTypeElementDefaults(FormatBooleansTrueFalseOrZeroOne(_StringWriter.ToString(), source.DocTypeName), source.DocTypeName)
                        : _StringWriter.ToString());
            }
        }

        /// <summary>
        ///     Runs a given form's xml schema against it throwing an exception if it fails to validate
        /// </summary>
        /// <param name= NavKey.DocTypeName></param>
        /// <param name="xml"></param>
        public void Validate(string DocData) { new SchemaValidator().Validate(DocData); }

        public BaseDoc Create(string DocTypeName) { return Read(TemplateController.Instance.OpenText(DocTypeName, "template.xml")); }

        public bool Processable(string DocTypeName, string DocRev)
        {
            string template_xml = TemplateController.Instance.OpenText(DocTypeName, DocRev, "template.xml");
            return
                !string.IsNullOrWhiteSpace(template_xml)
                &&
                ReadDocTypeName(template_xml) == DocTypeName
                &&
                GetDocRev(template_xml) == DocRev;
        }

        public string ContentType {
            get { return "application/vnd.ms-infopath"; }
        }

        public string HrefVirtualFilename(string DocTypeName, string DocRev) { return "manifest.xsf"; }

        /// <summary>
        ///     Removes empty elements from xml. Achieving the same rendering (or lack of) DefaultValue(0)
        ///     & DefaultValue(false) interpreted by XmlSerializer by removing elements with those values.
        ///     This is accomplished by altering XmlSerializer's output. Target audience of this application
        ///     is/was business users whom zero & false usually hold the same meaning over the population.
        /// </summary>
        /// <param name="DocData"></param>
        /// <returns></returns>
        public static string CollapseDefaultValueElements(string DocData)
        {
            // remove empty tags from the template.xml so they are not read as blanks when
            // the XmlSerializer reads them to be merged with an incoming create request
            // for a new Doc
            //TODO:Decide if this belongs here (should it go in the DocXmlHandler?)
            int Length = 0;
            string DocTypeName = internalReadDocTypeName(DocData);
            DocData = DocXmlDefaultValueRegEx.Replace(DocData, String.Empty);

            //HACK:Need to observe my xmlns dynamically
            //TODO:Remove only empty tags of integral value types as these are the only ones that mess up the XmlSerializer parser.
            //The fact of the matter is, empty tags are significant to the infopath application and should be kept
            string rootTag = Regex.Match(DocData,
                @"<my:" + DocTypeName + "[^>]+>",
                RegexOptions.Singleline | RegexOptions.IgnoreCase).Value;

            while (Length != DocData.Length)
            {
                Length = DocData.Length;
                // Remove empty tags
                DocData = XmlEmptyTagWithoutNullableElementsRegEx.Replace(DocData, String.Empty);
            }

            // make sure we didn't swipe the root tag out of the file
            if (DocData.IndexOf(rootTag) == -1)
                DocData += rootTag + "</my:" + DocTypeName + ">";

            return DocData;
        }

        /// <summary>
        ///     XmlSerializer writes Booleans as the words "true" & "false". InfoPath can
        ///     potentially write them as "1" & "0".
        /// </summary>
        /// <param name="docXml"></param>
        /// <param name="DocTypeName"></param>
        /// <returns></returns>
        internal static string FormatBooleansTrueFalseOrZeroOne(string docXml, string DocTypeName)
        {
            string templateDocXml = TemplateController.Instance.OpenText(DocTypeName, "template.xml");

            return Regex.Replace(
                docXml,
                @"(<my:)([^>]+)(>)(false|true)(</my:)(\2)(>)",
                match => templateDocXml.IndexOf(match.Groups[0].Value.Replace(">false<", ">0<").Replace(">true<", ">1<")) != -1
                             ? match.Groups[0].Value.Replace(">false<", ">0<").Replace(">true<", ">1<")
                             : match.Groups[0].Value,
                RegexOptions.Singleline | RegexOptions.Multiline);
        }

        /// <summary>
        ///     Parses the given form's "solutionVersion" number from the manifest.xsf. Note,
        ///     this string will change every time
        /// </summary>
        /// <param name= NavKey.DocTypeName></param>
        /// <returns></returns>
        public string GetDocRev(string DocData) { return ReadRevision(DocData); }

        private static string internalReadDocTypeName(string DocData)
        {
            return Regex.Match(DocData,
                @"(urn:schemas-microsoft-com:office:infopath:)(?<DocTypeName>\w+)(:-myXSD-\d{4}-\d{2}-\d{2}T\d{2}-\d{2}-\d{2})",
                RegexOptions.IgnoreCase).Groups["DocTypeName"].Value;
        }

        private static string ParseAttributeValue(string DocData, string attributeName) { return Regex.Match(DocData, String.Format("(?<={0}=\")(.*?)(?=\")", attributeName), RegexOptions.Singleline).Value; }

        public static string RemoveInvalidDateElementText(string DocData) { return DocXmlInvalidDateStringRegEx.Replace(DocData, String.Empty); }

        /// <summary>
        ///     a side-effect of the XmlSerializer working on a PropertyOverlay-ApplyUninitializedObject object processing before
        ///     XmlSerializer are initialized value-typed properties.
        ///     These properties render XML like  "
        ///     <my:field1_46EnvironmentalModifications>false</my:field1_46EnvironmentalModifications>" or "
        ///     <my:TotalNumberOfDirectCareStaff>0</my:TotalNumberOfDirectCareStaff>".
        ///     InfoPath interprets this as an explicitly set element adjusting it's UI accordingly.
        ///     We do not want this. The elements should render like
        ///     "<my:field1_46EnvironmentalModifications></my:field1_46EnvironmentalModifications>" & "
        ///     <my:TotalNumberOfDirectCareStaff></my:TotalNumberOfDirectCareStaff>".
        ///     @"(<my:)([^>]+)(>)(0|false)(</my:)(\2)(>)", "$1$2$3$5$6$7" strips those element values. The template.xml file
        ///     content is also
        ///     analyzed to make sure it has not specified something like "
        ///     <my:field1_46EnvironmentalModifications>false</my:field1_46EnvironmentalModifications>"
        ///     as it;s explicit default; if it has, the element fix/replacement is skipped as it appears the designer of the form
        ///     meant to do this
        ///     This method was established simple so it can be documented with proper XML comments. As of this writing it's only
        ///     referenced once.
        /// </summary>
        /// <param name="docXml"></param>
        /// <param name="DocTypeName"></param>
        /// <returns></returns>
        internal static string RemoveValueTypeElementDefaults(string docXml, string DocTypeName)
        {
            string templateDocXml = TemplateController.Instance.OpenText(DocTypeName, "template.xml");

            return Regex.Replace(
                docXml,
                @"(<my:)([^>]+)(>)(0|false)(</my:)(\2)(>)",
                match => templateDocXml.IndexOf(match.Groups[0].Value) == -1 // checking the template.xml for existence of the matched element
                             ? String.Format("{0}{1}{2}{3}{4}{5}", match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value, match.Groups[5].Value, match.Groups[6].Value, match.Groups[7].Value)
                             : match.Groups[0].Value,
                RegexOptions.Singleline | RegexOptions.Multiline);
        }

        /// <summary>
        ///     undoing the "xsd:integer to xsd:int" hack introduced early in the project to get a temporary .net POCO that may be
        ///     capable of desiealizing the XML correctly
        /// </summary>
        /// <param name="DocRevStrict"></param>
        /// <param name="DocTypeName"></param>
        /// <param name="DocRev"></param>
        /// <param name="CollapsedElementsDocXml"></param>
        /// <param name="dstBaseDoc"></param>
        /// <param name="BaseDocType"></param>
        /// <returns>null if the fix simply can't be applied in this situation</returns>
        private static BaseDoc TryXsdIntEgerFixDstBaseDoc(bool DocRevStrict, string DocTypeName, string DocRev, string CollapsedElementsDocXml, Type BaseDocType)
        {
            string DocXsd = DocRevStrict
                                ? TemplateController.Instance.OpenText(DocTypeName, DocRev, "myschema.xsd")
                                : TemplateController.Instance.OpenText(DocTypeName, "myschema.xsd");

            if (DocXsd.IndexOf("\"xsd:int\"") == -1)
                return null;

            // undoing the "xsd:integer to xsd:int" hack introduced early in the project to get a temporary .net POCO that may be capable of desiealizing the XML correctly
            object IntermediateBaseDoc =
                Runtime.FindBaseDoc(
                    Runtime.MakeBaseDocAssembly(
                        new[] { DocXsd.Replace("xsd:int", "xsd:integer") },
                        DocTypeName,
                        DocRev,
                        "intFixer"), DocTypeName);

            // see if the XmlSerializer can handle it now
            IntermediateBaseDoc = Serialization.ReadObject(CollapsedElementsDocXml, IntermediateBaseDoc.GetType());

            // now that we have a poco filled with data, convert all that data to string
            string json = JsonConvert.SerializeObject(IntermediateBaseDoc);

            // the json parser by design should convert all the values back to something the "hacked" POCO can have it's properties filled with
            return (BaseDoc)JsonConvert.DeserializeObject(json, BaseDocType);
        }
    }
}