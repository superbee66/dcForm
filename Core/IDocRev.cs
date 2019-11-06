using System;

namespace dCForm.Core
{
    public interface IDocRev_Gen2 : IBaseDoc, IDocTerm
    {
        byte[] TargetDocTypeFiles { get; set; }
        string TargetDocTypeName { get; set; }
        string TargetDocTypeVer { get; set; }
    }

    [Obsolete("Inconsistencies early in development of the DocRev system object lead to properties named differently. In production DocRev_Gen1 objects still hold business data & require this older interface.")]
    public interface IDocRev_Gen1 : IBaseDoc, IDocTerm
    {
        byte[] DocTypeFiles { get; set; }
        string DocTypeVer { get; set; }
    }
}