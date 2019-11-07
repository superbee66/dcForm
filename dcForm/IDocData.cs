namespace dCForm
{
    internal interface IDocData
    {
        string GetDocData(out string DocSrc, string DocTypeName, string DocId = null, string RelayUrl = null, long LogSequenceNumber = 0);
    }
}