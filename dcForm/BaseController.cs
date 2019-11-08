using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using dCForm.Format;
using dCForm.Util;
using dCForm.Util.Zip;

namespace dCForm
{
    public class BaseController : InfoController
    {
        public virtual object Create(out string DocSrc, object Doc, Dictionary<string, string> DocKeys = null, string RelayUrl = null)
        {
            return Create(out DocSrc, Doc, DocKeys, RelayUrl, true);
        }

        /// <summary>
        ///     Creates a new instance of the Doc parameter's type passed; merged/automapped/overlayed with another instance of
        ///     that same type constructed straight from the current template.xml located in the ~/form/{DocTypeName}/template.xml
        ///     of this application. Other properties specific not defined in the template.xml's text specific to this solution's
        ///     BaseDoc super-class & the current HttpContext are also inflated.
        /// </summary>
        /// <param name="DocSrc"></param>
        /// <param name="Doc"></param>
        /// <param name="DocKeys"></param>
        /// <param name="RelayUrl"></param>
        /// <param name="ProcessTemplate"></param>
        /// <returns></returns>
        public virtual object Create(out string DocSrc, object Doc, Dictionary<string, string> DocKeys = null, string RelayUrl = null, bool ProcessTemplate = true)
        {
            string DocTypeName = ((BaseDoc)Doc).DocTypeName;

            // apply ~/form/{DocTypeName}/template.xml values to document passed into us
            if (ProcessTemplate)
                Doc = PropertyOverlay.Overlay(Doc, DocInterpreter.Instance.Create(DocTypeName));


            if (DocKeys != null)
                if (DocKeys.Count > 0)
                    ((IDocIdentifiers)Doc).DocId = DocKey.DocIdFromKeys(DocKeys);

            //TODO:need to type-safe all the "object Doc" parameter methods
            ((BaseDoc)Doc).DocChecksum = CalcDocChecksum((BaseDoc)Doc);

            DocSrc = Nav.ToUrl((BaseDoc)Doc, RelayUrl);

            return Doc;
        }

        /// <summary>
        ///     XmlSerialize object without processing instructions, remove all tags & collapse any white-space to a single space,
        ///     convert all apply ToLocalTime to all DateTime properties
        /// </summary>
        /// <param name="baseDoc"></param>
        /// <returns></returns>
        public static int CalcDocChecksum(BaseDoc baseDoc, bool? docStatus = null)
        {
            docStatus = docStatus ?? baseDoc.DocStatus;

            // absolutely necessary the object is not altered in any way shape of form
            baseDoc = baseDoc.Clone();

            // normalize the datetime properties since they are mangled in the XmlSerialization/DataContractSerilaization process(s)
            foreach (PropertyInfo p in
                baseDoc.GetType()
                       .GetProperties()
                       .ToArray()
                       .Where(p => (Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType) == typeof(DateTime)))
                p.SetValue(baseDoc,
                    ((DateTime)(p.GetValue(baseDoc,
                        null) ?? DateTime.MinValue)).ToLocalTime(),
                    null);

            //TODO:Add signature parse logic
            return Regex.Replace(Regex.Replace(
              DocInterpreter.Instance.Write(baseDoc, false),
              "</?[a-z][a-z0-9]*[^<>]*>|<!--.*?-->",
              "",
              RegexOptions.Singleline | RegexOptions.IgnoreCase),
              @"\s+",
              " ",
              RegexOptions.Singleline | RegexOptions.IgnoreCase).GetHashCode() ^ (docStatus ?? false).GetHashCode();
        }

        /// <summary>
        ///     XmlSerialize object without processing instructions, remove all tags & collapse any white-space to a single space
        /// </summary>
        /// <param name="baseDoc"></param>
        /// <returns></returns>
        public static int CalcDocChecksum(string DocData, bool? DocStatus)
        {
            return CalcDocChecksum(DocInterpreter.Instance.Read(DocData, true), DocStatus);
        }

        public virtual object Read(out string DocSrc, string DocData, out Dictionary<string, string> DocKeys, string RelayUrl = null)
        {
            BaseDoc _BaseDoc = DocInterpreter.Instance.Read(DocData);
            DocKeys = DocKey.DocIdToKeys(((IDocIdentifiers)_BaseDoc).DocId);
            return Create(out DocSrc, _BaseDoc, DocKeys, RelayUrl, false);
        }

        internal static string PIRewrite(string DocData, bool? DocStatus = null, DateTime? SubmittedDate = null, Dictionary<string, string> DocKeys = null, string DocTitle = null,String DocSubmittedBy=null)
        {
            //TODO:Rethink this logic & relocate it somewhere better
            if (DocKeys != null || !string.IsNullOrWhiteSpace(DocTitle) || DocStatus != null || !string.IsNullOrWhiteSpace(DocSubmittedBy))
            {
                DocProcessingInstructions _DocProcessingInstructions = DocInterpreter.Instance.ReadDocPI(DocData);

                if (DocKeys != null)
                    _DocProcessingInstructions.DocIdKeys = DocKeys;

                if (DocStatus != null)
                    _DocProcessingInstructions.DocStatus = DocStatus;

                if (!string.IsNullOrWhiteSpace(DocTitle))
                    _DocProcessingInstructions.DocTitle = DocTitle;

                if (!string.IsNullOrWhiteSpace(DocSubmittedBy))
                    _DocProcessingInstructions.DocSubmittedBy = DocSubmittedBy;

                DocData = DocInterpreter.Instance.WritePI(DocData, _DocProcessingInstructions);
            }
            return DocData;
        }
    }
}