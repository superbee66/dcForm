using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using dCForm.Format.XsnXml;
//using DocumentFormat.OpenXml.Packaging;

namespace dCForm.Format.Docx
{
    class DocxInterpreter : IDocByteInterpreter
    {
        private static XsnXmlInterpreter _XsnXmlInterpreter = new XsnXmlInterpreter();
        public string ContentFileExtension {
            get {
                return "docx";
            }
        }

        public string ContentType {
            get {
                throw new NotImplementedException();
            }
        }

        public BaseDoc Create(string DocTypeName)
        {
            return _XsnXmlInterpreter.Create(DocTypeName);
        }

        public string GetDescription(string DocTypeName)
        {
            throw new NotImplementedException();
        }

        public string HrefVirtualFilename(string DocTypeName, string DocRev)
        {
            throw new NotImplementedException();
        }

        public bool Processable(string DocTypeName, string DocRev)
        {
            throw new NotImplementedException();
        }

        public BaseDoc Read(byte[] DocData, bool DocRevStrict = false)
        {
            throw new NotImplementedException();
        }

        public DocProcessingInstructions ReadDocPI(byte[] srcDocData)
        {
            throw new NotImplementedException();
        }

        public string ReadDocTypeName(byte[] DocData)
        {
            throw new NotImplementedException();
        }

        public string ReadRevision(byte[] DocData)
        {
            throw new NotImplementedException();
        }

        public void Validate(byte[] DocData)
        {
            throw new NotImplementedException();
        }

        public byte[] Write<T>(T source, bool includeProcessingInformation = true) where T : DocProcessingInstructions
        {
            throw new NotImplementedException();
        }

        public byte[] WritePI(byte[] srcDocData, DocProcessingInstructions pi)
        {
            throw new NotImplementedException();
        }

        //protected static void ReplaceCustomXML(Stream _MemoryStream, string customXML)
        //{
           
        //    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(_MemoryStream, true))

        //    {

        //        MainDocumentPart mainPart = wordDoc.MainDocumentPart;

        //        mainPart.DeleteParts<CustomXmlPart>(mainPart.CustomXmlParts);

        //        //Add a new customXML part and then add content

        //        CustomXmlPart customXmlPart = mainPart.AddNewPart<CustomXmlPart>();

        //        //copy the XML into the new part…

        //        using (StreamWriter ts = new StreamWriter(customXmlPart.GetStream()))

        //            ts.Write(customXML);

        //    }

        //}
    }
}
