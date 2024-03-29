﻿using System.Collections.Generic;


namespace dCForm
{
    public static class LightDocExtensions
    {
        public static Dictionary<string, string> GetDocKeys(this LightDoc lightdoc) { return DocKey.DocIdToKeys(lightdoc.DocId); }

        /// <summary>
        ///     useful to understand what a LightDoc for a DOCREV's principle "Target Doc Type Name" is actually represents.
        /// </summary>
        /// <param name="lightdoc">LightDoc for a DOCREV document</param>
        /// <returns>
        ///     for IDocRev_Gen2's: TargetDocTypeVer, IDocRev_Gen1's: DocTypeVer, null if we are not dealing with a DOCREV
        ///     LightDoc listing item
        /// </returns>
        public static string GetTargetDocVer(this LightDoc lightdoc)
        {
            Dictionary<string, string> _DocKeys = lightdoc.GetDocKeys();
            return _DocKeys.ContainsKey("TargetDocTypeVer")
                       ? _DocKeys["TargetDocTypeVer"]
                       : _DocKeys.ContainsKey("DocTypeVer")
                             ? _DocKeys["DocTypeVer"]
                             : null;
        }

        /// <summary>
        ///     useful to understand what a LightDoc for a DOCREV's principle "Target Doc Type Name" is actually represents.
        /// </summary>
        /// <param name="lightdoc">LightDoc for a DOCREV document</param>
        /// <returns>
        ///     for IDocRev_Gen2's: TargetDocTypeVer, IDocRev_Gen1's: DocTypeVer, anything not actually a DOCREV
        ///     representative will simple be the DocTypeName
        /// </returns>
        public static string GetTargetDocName(this LightDoc lightdoc)
        {
            Dictionary<string, string> _DocKeys = lightdoc.GetDocKeys();
            return _DocKeys.ContainsKey("TargetDocTypeName")
                       ? _DocKeys["TargetDocTypeName"]
                       : _DocKeys.ContainsKey("DocTypeName")
                             ? _DocKeys["DocTypeName"]
                             : lightdoc.DocTypeName;
        }
    }
}