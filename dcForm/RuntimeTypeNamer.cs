using System;
using System.Linq;
using System.Text.RegularExpressions;
using dCForm.Util;

namespace dCForm
{
    internal class RuntimeTypeNamer
    {
        /// <summary>
        ///     Matches "docLCR_1023AFORPD_0715.rev2016.rev05.rev11.rev01.rev11.rev10" in text bodies like "Could not find type
        ///     'System.Collections.Generic.List`1[[docLCR_1023AFORPD_0715.rev2016.rev05.rev11.rev01.rev11.rev10.ArrayOfRepeaterRepeater,
        ///     4yqztpwf, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]' in assembly 'mscorlib, Version=4.0.0.0,
        ///     Culture=neutral, PublicKeyToken=b77a5c561934e089'."
        /// </summary>
        private static readonly Regex VALID_CSHARP_NAMESPACE_PART_MATCH = new Regex(String.Format(@"({0}[A-Z0-9_]+\.)+(({1}[A-Z0-9_]+)\.*)+", DOCTYPENAME_NS_PREFIX, DOCREV_NS_PREFIX));

        /// <summary>
        ///     Anything not alpha-numeric, underscore or single period; double periods will also invalidate (match)
        /// </summary>
        private static readonly Regex INVALID_CSHARP_NAMESPACE_PART_MATCH = new Regex(@"[^\w_\.]|\.{2,}");

        /// <summary>
        ///     The first segment(s) (usually only the first) of a given DocTypeName/DocRev cSharp namespace's prefix
        /// </summary>
        internal const string DOCTYPENAME_NS_PREFIX = "doc";

        /// <summary>
        ///     Usually starting AFTER the first namespace segment of a given DocTypeName/DocRev cSharp namespace's prefix
        /// </summary>
        internal const string DOCREV_NS_PREFIX = "r";

        /// <summary>
        ///     Any remaining namespace segment prefix(es) not covered by the DocTypeName/DocRev in the cSharp namespace
        /// </summary>
        internal const string ADDITIONAL_NS_PREFIX = "ns";

        /// <summary>
        ///     Creates a namespace (each DocType, DocRev & it's children reside in a dedicated namespace) that is suitable for
        ///     parsing then reversing to it's original string representations. The transformations to and from DocTypeName/DocRev
        ///     & C# namespace is a lossless operation. The inverse of this method is TryParseDocTypeAndRev. Segments for the
        ///     DocType's actual name are prefixed with "doc", for the DocRev: "rev". Example:
        ///     docFORM200.rev1.rev0.rev56.HERE.ARE.SOME.ADDITIONAL.NS_PARTS
        /// </summary>
        /// <param name="DocTypeName"></param>
        /// <param name="DocRev"></param>
        /// <param name="AdditionalRootNames"></param>
        /// <returns>DotTypeName as the root name & DocRev as the second level name if nothing specified in AdditionalRootNames</returns>
        internal static string CalcCSharpFullname(string DocTypeName, string DocRev, params string[] AdditionalRootNames)
        {
            // validate arguments
            if (INVALID_CSHARP_NAMESPACE_PART_MATCH.IsMatch(DocTypeName) | String.IsNullOrWhiteSpace(DocTypeName))
                throw new ArgumentException(String.Format("\"{0}\" is not a valid DocTypeName for namespace code generation operations"), DocTypeName);

            if (INVALID_CSHARP_NAMESPACE_PART_MATCH.IsMatch(DocRev) | String.IsNullOrWhiteSpace(DocRev))
                throw new ArgumentException(String.Format("\"{0}\" is not a valid DocRev for namespace code generation operations"), DocRev);

            if (AdditionalRootNames != null)
                foreach (string test in AdditionalRootNames)
                    if (!String.IsNullOrWhiteSpace(test))
                        if (INVALID_CSHARP_NAMESPACE_PART_MATCH.IsMatch(test))
                            throw new ArgumentException(String.Format("\"{0}\" is not valid for namespace code generation operations"), test);

            string[] DocTypeNameParts = DocTypeName.Split('.').Select(s => String.Format("{0}{1}", DOCTYPENAME_NS_PREFIX, s.ToUpper())).ToArray();
            string[] RevParts = DocRev.Split('.').Select(s => String.Format("{0}{1}", DOCREV_NS_PREFIX, s.ToUpper())).ToArray();
            string[] OtherParts = AdditionalRootNames == null
                                      ? new string[]
                                      {}
                                      : AdditionalRootNames.Where(s => !String.IsNullOrWhiteSpace(s)).Select(s => String.Format("{0}{1}", ADDITIONAL_NS_PREFIX, StringTransform.SafeIdentifier(s ?? "_").ToUpper())).ToArray();

            return String.Join(".", DocTypeNameParts.Concat(RevParts).Concat(OtherParts));
        }

        internal static bool TryParseDocNameAndRev(string NamespaceOrTypeFullname, out string DocTypeName, out string DocRev)
        {
            string fqn = VALID_CSHARP_NAMESPACE_PART_MATCH.Match(NamespaceOrTypeFullname).Value;

            DocTypeName = String.Join(".", fqn.Split('.').Where(s => s.StartsWith(DOCTYPENAME_NS_PREFIX)).Select(s => s.Substring(DOCTYPENAME_NS_PREFIX.Length)));
            DocRev = String.Join(".", fqn.Split('.').Where(s => s.StartsWith(DOCREV_NS_PREFIX)).Select(s => s.Substring(DOCREV_NS_PREFIX.Length)));

            return !String.IsNullOrWhiteSpace(DocTypeName) && !String.IsNullOrWhiteSpace(DocRev);
        }
    }
}