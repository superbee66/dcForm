using System;
using System.Xml;
using System.Xml.Serialization;

namespace dCForm.Core.Util.Xsd
{
    internal sealed class XsdParametersSerializer : XmlSerializer
    {
        protected override XmlSerializationReader CreateReader()
        {
            return new XmlSerializationReader1();
        }

        protected override XmlSerializationWriter CreateWriter()
        {
            throw new InvalidOperationException();
        }

        public override bool CanDeserialize(XmlReader xmlReader)
        {
            return true;
        }

        protected override void Serialize(object objectToSerialize, XmlSerializationWriter writer)
        {
            throw new InvalidOperationException();
        }

        protected override object Deserialize(XmlSerializationReader reader)
        {
            return ((XmlSerializationReader1) reader).Read7_xsd();
        }
    }
}