using System.IO;

namespace dCForm.Core.Template
{
    public interface ITemplateController
    {
        MemoryStream OpenRead(string DocTypeName, string DocRev, string filename);

        string TopDocRev(string DocTypeName);
    }
}