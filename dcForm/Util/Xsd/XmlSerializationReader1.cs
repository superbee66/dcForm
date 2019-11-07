using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.Serialization;

namespace dCForm.Util.Xsd
{
    internal class XmlSerializationReader1 : XmlSerializationReader
    {
        private Hashtable _CodeGenerationOptionsValues;

        private string id6_GenerateClasses;

        private string id5_generateSchemas;

        private string id14_type;

        private string id2_Item;

        private string id9_options;

        private string id16_xdr;

        private string id13_schemaImporterExtensions;

        private string id17_assembly;

        private string id23_generateClasses;

        private string id1_xsd;

        private string id12_element;

        private string id11_schema;

        private string id4_Item;

        private string id20_nologo;

        private string id15_xml;

        private string id21_help;

        private string id3_generateObjectModel;

        private string id18_xsdParameters;

        private string id10_uri;

        private string id7_language;

        private string id22_generateDataSet;

        private string id8_namespace;

        private string id19_output;

        private string id24_enableLinqDataSet;

        internal Hashtable CodeGenerationOptionsValues
        {
            get
            {
                if (_CodeGenerationOptionsValues == null)
                    _CodeGenerationOptionsValues = new Hashtable
                    {
                        {
                            "properties",
                            1L
                        },
                        {
                            "order",
                            8L
                        },
                        {
                            "enableDataBinding",
                            16L
                        },
                        {
                            "none",
                            0L
                        }
                    };
                return _CodeGenerationOptionsValues;
            }
        }

        internal object Read7_xsd()
        {
            object result = null;
            Reader.MoveToContent();
            if (Reader.NodeType == XmlNodeType.Element)
            {
                if (Reader.LocalName != id1_xsd || Reader.NamespaceURI != id2_Item)
                    throw CreateUnknownNodeException();
                result = Read6_XsdParameters(false, true);
            } else
                UnknownNode(null);
            return result;
        }

        private XsdParameters Read5_GenerateClasses(XsdParameters xsd, bool isNullable, bool checkType)
        {
            xsd.Classes = true;
            XmlQualifiedName xmlQualifiedName = checkType ? GetXsiType() : null;
            bool flag = false;
            if (isNullable)
                flag = ReadNull();
            if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != id6_GenerateClasses || xmlQualifiedName.Namespace != id2_Item))
                throw CreateUnknownTypeException(xmlQualifiedName);
            if (flag)
                return null;
            StringCollection xsdSchemas = xsd.XsdSchemas;
            StringCollection elements = xsd.Elements;
            StringCollection arg_69_0 = xsd.SchemaImporterExtensions;
            while (Reader.MoveToNextAttribute())
                if (Reader.LocalName == id7_language && Reader.NamespaceURI == id4_Item)
                    xsd.Language = Reader.Value;
                else if (Reader.LocalName == id8_namespace && Reader.NamespaceURI == id4_Item)
                    xsd.Namespace = Reader.Value;
                else if (Reader.LocalName == id9_options && Reader.NamespaceURI == id4_Item)
                    xsd.Options = Read4_CodeGenerationOptions(Reader.Value);
                else if (Reader.LocalName == id10_uri && Reader.NamespaceURI == id4_Item)
                    xsd.Uri = CollapseWhitespace(Reader.Value);
                else if (!IsXmlnsAttribute(Reader.Name))
                    UnknownNode(xsd);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return xsd;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int num = 0;
            int readerCount = ReaderCount;
            while (Reader.NodeType != XmlNodeType.EndElement && Reader.NodeType != XmlNodeType.None)
            {
                if (Reader.NodeType == XmlNodeType.Element)
                    if (Reader.LocalName == id11_schema && Reader.NamespaceURI == id2_Item)
                        xsdSchemas.Add(Reader.ReadElementString());
                    else if (Reader.LocalName == id12_element && Reader.NamespaceURI == id2_Item)
                        elements.Add(Reader.ReadElementString());
                    else if (Reader.LocalName == id13_schemaImporterExtensions && Reader.NamespaceURI == id2_Item)
                    {
                        if (!ReadNull())
                        {
                            StringCollection schemaImporterExtensions = xsd.SchemaImporterExtensions;
                            if (Reader.IsEmptyElement)
                                Reader.Skip();
                            else
                            {
                                Reader.ReadStartElement();
                                Reader.MoveToContent();
                                int num2 = 0;
                                int readerCount2 = ReaderCount;
                                while (Reader.NodeType != XmlNodeType.EndElement && Reader.NodeType != XmlNodeType.None)
                                {
                                    if (Reader.NodeType == XmlNodeType.Element)
                                        if (Reader.LocalName == id14_type && Reader.NamespaceURI == id2_Item)
                                            schemaImporterExtensions.Add(Reader.ReadElementString());
                                        else
                                            UnknownNode(null);
                                    else
                                        UnknownNode(null);
                                    Reader.MoveToContent();
                                    CheckReaderCount(ref num2, ref readerCount2);
                                }
                                ReadEndElement();
                            }
                        }
                    } else
                        UnknownNode(xsd);
                else
                    UnknownNode(xsd);
                Reader.MoveToContent();
                CheckReaderCount(ref num, ref readerCount);
            }
            ReadEndElement();
            return xsd;
        }

        private CodeGenerationOptions Read4_CodeGenerationOptions(string s)
        {
            return (CodeGenerationOptions) ToEnum(s, CodeGenerationOptionsValues, "System.Xml.Serialization.CodeGenerationOptions");
        }

        private XsdParameters Read3_GenerateSchemas(XsdParameters xsd, bool isNullable, bool checkType)
        {
            XmlQualifiedName xmlQualifiedName = checkType ? GetXsiType() : null;
            bool flag = false;
            if (isNullable)
                flag = ReadNull();
            if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != id5_generateSchemas || xmlQualifiedName.Namespace != id2_Item))
                throw CreateUnknownTypeException(xmlQualifiedName);
            if (flag)
                return null;
            StringCollection instances = xsd.Instances;
            StringCollection xdrSchemas = xsd.XdrSchemas;
            StringCollection assemblies = xsd.Assemblies;
            StringCollection types = xsd.Types;
            while (Reader.MoveToNextAttribute())
                if (!IsXmlnsAttribute(Reader.Name))
                    UnknownNode(xsd);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return xsd;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int num = 0;
            int readerCount = ReaderCount;
            while (Reader.NodeType != XmlNodeType.EndElement && Reader.NodeType != XmlNodeType.None)
            {
                if (Reader.NodeType == XmlNodeType.Element)
                    if (Reader.LocalName == id15_xml && Reader.NamespaceURI == id2_Item)
                        instances.Add(Reader.ReadElementString());
                    else if (Reader.LocalName == id16_xdr && Reader.NamespaceURI == id2_Item)
                        xdrSchemas.Add(Reader.ReadElementString());
                    else if (Reader.LocalName == id17_assembly && Reader.NamespaceURI == id2_Item)
                        assemblies.Add(Reader.ReadElementString());
                    else if (Reader.LocalName == id14_type && Reader.NamespaceURI == id2_Item)
                        types.Add(Reader.ReadElementString());
                    else
                        UnknownNode(xsd);
                else
                    UnknownNode(xsd);
                Reader.MoveToContent();
                CheckReaderCount(ref num, ref readerCount);
            }
            ReadEndElement();
            return xsd;
        }

        private XsdParameters Read2_GenerateDataset(XsdParameters xsd, bool isNullable, bool checkType)
        {
            xsd.Dataset = true;
            XmlQualifiedName xmlQualifiedName = checkType ? GetXsiType() : null;
            bool flag = false;
            if (isNullable)
                flag = ReadNull();
            if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != id3_generateObjectModel || xmlQualifiedName.Namespace != id2_Item))
                throw CreateUnknownTypeException(xmlQualifiedName);
            if (flag)
                return null;
            StringCollection xsdSchemas = xsd.XsdSchemas;
            while (Reader.MoveToNextAttribute())
                if (Reader.LocalName == id7_language && Reader.NamespaceURI == id4_Item)
                    xsd.Language = Reader.Value;
                else if (Reader.LocalName == id8_namespace && Reader.NamespaceURI == id4_Item)
                    xsd.Namespace = Reader.Value;
                else if (Reader.LocalName == id24_enableLinqDataSet && Reader.NamespaceURI == id4_Item)
                    xsd.EnableLinqDataSet = XmlConvert.ToBoolean(Reader.Value);
                else if (!IsXmlnsAttribute(Reader.Name))
                    UnknownNode(xsd);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return xsd;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int num = 0;
            int readerCount = ReaderCount;
            while (Reader.NodeType != XmlNodeType.EndElement && Reader.NodeType != XmlNodeType.None)
            {
                if (Reader.NodeType == XmlNodeType.Element)
                    if (Reader.LocalName == id11_schema && Reader.NamespaceURI == id2_Item)
                        xsdSchemas.Add(Reader.ReadElementString());
                    else
                        UnknownNode(xsd);
                else
                    UnknownNode(xsd);
                Reader.MoveToContent();
                CheckReaderCount(ref num, ref readerCount);
            }
            ReadEndElement();
            return xsd;
        }

        private XsdParameters Read6_XsdParameters(bool isNullable, bool checkType)
        {
            XmlQualifiedName xmlQualifiedName = checkType ? GetXsiType() : null;
            bool flag = false;
            if (isNullable)
                flag = ReadNull();
            if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != id18_xsdParameters || xmlQualifiedName.Namespace != id2_Item))
                throw CreateUnknownTypeException(xmlQualifiedName);
            if (flag)
                return null;
            XsdParameters xsdParameters = new XsdParameters();
            bool[] array = new bool[6];
            while (Reader.MoveToNextAttribute())
                if (!array[3] && Reader.LocalName == id19_output && Reader.NamespaceURI == id4_Item)
                {
                    xsdParameters.OutputDir = Reader.Value;
                    array[3] = true;
                } else if (!array[4] && Reader.LocalName == id20_nologo && Reader.NamespaceURI == id4_Item)
                {
                    xsdParameters.Nologo = XmlConvert.ToBoolean(Reader.Value);
                    array[4] = true;
                } else if (!array[5] && Reader.LocalName == id21_help && Reader.NamespaceURI == id4_Item)
                {
                    xsdParameters.Help = XmlConvert.ToBoolean(Reader.Value);
                    array[5] = true;
                } else if (!IsXmlnsAttribute(Reader.Name))
                    UnknownNode(xsdParameters);
            Reader.MoveToElement();
            if (Reader.IsEmptyElement)
            {
                Reader.Skip();
                return xsdParameters;
            }
            Reader.ReadStartElement();
            Reader.MoveToContent();
            int num = 0;
            int readerCount = ReaderCount;
            while (Reader.NodeType != XmlNodeType.EndElement && Reader.NodeType != XmlNodeType.None)
            {
                if (Reader.NodeType == XmlNodeType.Element)
                    if (!array[0] && Reader.LocalName == id22_generateDataSet && Reader.NamespaceURI == id2_Item)
                    {
                        Read2_GenerateDataset(xsdParameters, false, true);
                        array[0] = true;
                    } else if (!array[1] && Reader.LocalName == id5_generateSchemas && Reader.NamespaceURI == id2_Item)
                    {
                        Read3_GenerateSchemas(xsdParameters, false, true);
                        array[1] = true;
                    } else if (!array[2] && Reader.LocalName == id23_generateClasses && Reader.NamespaceURI == id2_Item)
                    {
                        Read5_GenerateClasses(xsdParameters, false, true);
                        array[2] = true;
                    } else
                        UnknownNode(xsdParameters);
                else
                    UnknownNode(xsdParameters);
                Reader.MoveToContent();
                CheckReaderCount(ref num, ref readerCount);
            }
            ReadEndElement();
            return xsdParameters;
        }

        protected override void InitCallbacks() {}

        protected override void InitIDs()
        {
            id6_GenerateClasses = Reader.NameTable.Add("GenerateClasses");
            id5_generateSchemas = Reader.NameTable.Add("generateSchemas");
            id14_type = Reader.NameTable.Add("type");
            id2_Item = Reader.NameTable.Add("http://microsoft.com/dotnet/tools/xsd/");
            id9_options = Reader.NameTable.Add("options");
            id16_xdr = Reader.NameTable.Add("xdr");
            id13_schemaImporterExtensions = Reader.NameTable.Add("schemaImporterExtensions");
            id17_assembly = Reader.NameTable.Add("assembly");
            id23_generateClasses = Reader.NameTable.Add("generateClasses");
            id1_xsd = Reader.NameTable.Add("xsd");
            id12_element = Reader.NameTable.Add("element");
            id11_schema = Reader.NameTable.Add("schema");
            id4_Item = Reader.NameTable.Add("");
            id20_nologo = Reader.NameTable.Add("nologo");
            id15_xml = Reader.NameTable.Add("xml");
            id21_help = Reader.NameTable.Add("help");
            id3_generateObjectModel = Reader.NameTable.Add("generateObjectModel");
            id18_xsdParameters = Reader.NameTable.Add("xsdParameters");
            id10_uri = Reader.NameTable.Add("uri");
            id7_language = Reader.NameTable.Add("language");
            id22_generateDataSet = Reader.NameTable.Add("generateDataSet");
            id8_namespace = Reader.NameTable.Add("namespace");
            id19_output = Reader.NameTable.Add("output");
            id24_enableLinqDataSet = Reader.NameTable.Add("enableLinqDataSet");
        }
    }
}