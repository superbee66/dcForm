using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using dCForm.Client.Util;
using dCForm.Util.Cab;

namespace dCForm.Template.Nosql
{
    //TODO:Separate FileSystem, NOSQL & embedded resource harvesting into separate IDocResourceController
    public class NosqlTemplateController : ITemplateController
    {
        public MemoryStream OpenRead(string DocTypeName, string DocTypeVer, string filename = TemplateController.FOLDER_CONTENTS_VIRTUAL_CAB_FILE)
        {
            int reaponseFileLength;
            byte[] reaponseFile = null;
            byte[] decodedAttachment = GetCabDecodedAttachment(DocTypeName, DocTypeVer);

            if (decodedAttachment != null)
                if (TemplateController.FOLDER_CONTENTS_VIRTUAL_CAB_FILE == filename)
                    reaponseFile = decodedAttachment;
                else
                    new CabExtract(decodedAttachment).ExtractFile(
                        filename,
                        out reaponseFile,
                        out reaponseFileLength);

            return reaponseFile != null
                       ? new MemoryStream(reaponseFile)
                       : null;
        }

        public string TopDocRev(string DocTypeName)
        {
            return ServiceController.LuceneController.List(
                new List<string> { "DOCREV" },
                new Dictionary<string, List<string>>
                {
                    {
                        "TargetDocTypeName", new List<string> {DocTypeName}
                    }
                })
                                    .Select(_LightDoc => _LightDoc.GetTargetDocVer())
                                    .OrderByDescending(s => new System.Version(s))
                                    .FirstOrDefault();
        }

        private static byte[] GetCabDecodedAttachment(string DocTypeName, string DocTypeVer)
        {
            return CacheMan.Cache(() =>
                                  {
                                      Dictionary<string, string> DocKeys;

                                      const int SP1Header_Size = 20;
                                      const int FIXED_HEADER = 16;

                                      string attachmentName;
                                      byte[] reaponseFile = null;

                                      string DocSrc;

                                      object o = ServiceController.LuceneController.Get(
                                          out DocSrc,
                                          out DocKeys,
                                          "DOCREV",
                                          new Dictionary<string, string> { { "TargetDocTypeVer", DocTypeVer }, { "TargetDocTypeName", DocTypeName } });

                                      if (o == null)
                                          o = ServiceController.LuceneController.Get(
                                              out DocSrc,
                                              out DocKeys,
                                              "DOCREV",
                                              new Dictionary<string, string> { { "DocTypeVer", DocTypeVer }, { "DocTypeName", DocTypeName } });


                                      byte[] decodedAttachment = null;
                                      if (o != null)
                                      {
                                          IDocRev_Gen2 _IDocRev = (IDocRev_Gen2)o;

                                          using (MemoryStream ms = new MemoryStream(_IDocRev.TargetDocTypeFiles))
                                          using (BinaryReader theReader = new BinaryReader(ms))
                                          {
                                              //Position the reader to get the file size.
                                              byte[] headerData = new byte[FIXED_HEADER];
                                              headerData = theReader.ReadBytes(headerData.Length);

                                              int fileSize = (int)theReader.ReadUInt32();
                                              int attachmentNameLength = (int)theReader.ReadUInt32() * 2;

                                              byte[] fileNameBytes = theReader.ReadBytes(attachmentNameLength);
                                              //InfoPath uses UTF8 encoding.
                                              Encoding enc = Encoding.Unicode;

                                              //attachmentName = enc.GetString(fileNameBytes, 0, attachmentNameLength - 2);
                                              decodedAttachment = theReader.ReadBytes(fileSize);
                                          }
                                      }
                                      return decodedAttachment;
                                  }, false, "GetCabDecodedAttachment", DocTypeName, DocTypeVer);
        }
    }
}