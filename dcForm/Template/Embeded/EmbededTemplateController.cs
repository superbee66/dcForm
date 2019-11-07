using System.IO;
using System.Linq;
using dCForm.Util;
using dCForm.Util.Cab;

namespace dCForm.Template.Embeded
{
    public class EmbededTemplateController : ITemplateController
    {
        const string MY_ONLY_DOCTYPENAME = "DOCREV";

        public MemoryStream OpenRead(string DocTypeName, string DocRev, string filename)
        {
            if (DocTypeName != MY_ONLY_DOCTYPENAME || !new[]
            {
                "1.0.0.5", "1.0.0.6", "1.0.0.7"
            }.Any(m => DocRev == m))
                return null;


            int reaponseFileLength;
            byte[] reaponseFile = null;


            new CabExtract(
                "1.0.0.5" == DocRev
                    ? DOCREV_SCHEMAS.DOCREV_1_0_0_5
                    : "1.0.0.6" == DocRev
                          ? DOCREV_SCHEMAS.DOCREV_1_0_0_6
                          : DOCREV_SCHEMAS.DOCREV_1_0_0_7
                ).ExtractFile(
                    filename,
                    out reaponseFile,
                    out reaponseFileLength);

            return reaponseFile != null
                       ? new MemoryStream(reaponseFile)
                       : null;
        }

        public string TopDocRev(string DocTypeName)
        {
            return DocTypeName == "DOCREV"
                       ? "1.0.0.7"
                       : null;
        }
    }
}