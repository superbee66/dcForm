using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace dCForm.Util
{
    public static class Serialization
    {
        public static object ReadObject(string xml, Type t, bool useDataContractSerializer = false)
        {
            using (StringReader _StringReader = new StringReader(xml))
            using (XmlTextReader _XmlTextReader = new XmlTextReader(_StringReader))
                return useDataContractSerializer ?
                           new DataContractSerializer(t).ReadObject(_XmlTextReader, false) :
                           new XmlSerializer(t).Deserialize(_XmlTextReader);
        }
    }
}