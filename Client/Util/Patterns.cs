using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace dCForm.Client.Util
{
    //TODO:Relocate all non-private pattern matching to this utility class
    public enum Pat
    {
        EmailAddress
    }

    public static class Patterns
    {
        private static readonly Dictionary<Pat, string> DictionaryStringPatternSystemTextRegularExpressionsRegex
            = new Dictionary<Pat, string>
              {
                  {Pat.EmailAddress, @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}\b"}
              };

        public static bool IsMatch(this string s, Pat pattern)
        {
            return CacheMan.Cache(() =>
                                  new Regex(
                                      DictionaryStringPatternSystemTextRegularExpressionsRegex[pattern]),
                false,
                pattern).IsMatch(s);
        }
    }
}