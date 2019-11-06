using System;
using System.Text.RegularExpressions;

namespace dCForm.Client
{
    [Obsolete("FORM Infopath technologies are no longer supported")]
    public static class DocXmlParser
    {
     
        public static string GetFileName(string DocData)
        {
            return Regex.Match(DocData,"fileName=\"([^\"]+)\"").Groups[1].Value;
        }
    }
}