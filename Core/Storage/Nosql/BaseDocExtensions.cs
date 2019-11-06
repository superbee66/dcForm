using Lucene.Net.Index;

namespace dCForm.Core.Storage.Nosql
{
    public static class BaseDocExtensions
    {
        public static Term docTermFromBaseDoc(this BaseDoc _BaseDoc) { return new Term("DocTerm", _BaseDoc.AsTermTxt()); }
    }
}