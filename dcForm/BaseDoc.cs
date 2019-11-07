using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml.Serialization;
using dCForm.Client;
using dCForm.Client.Util;
using dCForm.Storage.Sql;
using dCForm.Format;

namespace dCForm
{
    /// <summary>
    ///     Summary description for baseInfoPathXmlRequestBody. It is serializable for JavaScriptSerialization.
    ///     The chain of inherited classes (DocProcessingInstructions, DocTerm, BaseAutoIdent) each support
    ///     serialization to individual (different) mediums.
    /// </summary>
    [ServiceKnownType("DocTypes", typeof(DocExchange))]
    [DataContract(Namespace = "http://schemas.progablab.com/2014/12/Serialization")]
    [Serializable]
    public class BaseDoc : DocProcessingInstructions, IBaseDoc
    {
        public BaseDoc() : base() { }

        private static readonly string[] FormNonFillablePropertyNames =
        {
            "DocChecksum", "DocStatus", "DocId", "DocTitleFormat", "DocSrc", "DocDropbox"
        };

        private static readonly InfoController _InfoController = new InfoController();
        private static readonly TextInfo _TextInfo = new CultureInfo("en-US", false).TextInfo;
        private static readonly Dictionary<string, string> DocTypeNameDescription = new Dictionary<string, string>();

        /// <summary>
        ///     User may want to title there own document?
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        [DataMember]
        public override string DocTitle {
            get {
                return base.DocTitle ?? DocTypeName.ToUpper();
            }
            set {
                base.DocTitle = value;
            }
        }

        /// <summary>
        ///     Returns similar results are TryContractSerialize() with XML processing instructions
        /// </summary>
        /// <param name="href">
        ///     The value of this attribute MUST be set to the Uniform Resource Locator (URL) of the form template
        ///     upon which this form file is based.
        /// </param>
        /// <returns>Xml document suitable for Microsoft InfoPath to read</returns>
        public string GetDocData(string href = null)
        {
            this.href = href;
            return DocInterpreter.Instance.Write(this);
        }

        public LightDoc ToLightDoc(string DocSrc = null)
        {
            return new LightDoc
            {
                DocId = DocId,
                DocSrc = DocSrc ?? Nav.ToUrl(this),
                DocStatus = DocStatus,
                DocTitle = DocTitle,
                DocTypeName = DocTypeName
            };
        }

        #region GetForm*Properties*

        /// <summary>
        ///     Further filters GetFormObjectNavProperties stripping DocId
        /// </summary>
        /// <param name="filled"></param>
        /// <returns></returns>
        public PropertyInfo[] GetFormFillableProperties(bool filled = false)
        {
            //TODO:Need to detect interface implementation safely
            return GetFormObjectNavProperties(filled).Where(m => !FormNonFillablePropertyNames.Contains(m.Name)).ToArray();
        }


        /// <summary>
        ///     Further filters GetFormObjectMappedProperties stripping IgnoreDataMember,XmlIgnoreAttribute,ScriptIgnoreAttribute &
        ///     NotMapped properties
        /// </summary>
        /// <param name="filled">when true, ensures the properties have been explicitly set</param>
        /// <returns></returns>
        public PropertyInfo[] GetFormObjectNavProperties(bool filled = false)
        {
            PropertyInfo[] p = CacheMan.Cache(() => GetFormObjectMappedProperties(false)
                                                        .Where(m =>
                                                               m.DeclaringType != typeof(BaseDoc) &&
                                                               m.DeclaringType != typeof(BaseAutoIdent)
                                                        )
                                                        .ToArray(),
                false,
                "GetFormObjectNavProperties",
                GetType().FullName,
                "GetFormObjectNavProperties");


            return p.Where(m => !filled || !this.IsDefaultValue(m)).ToArray();
        }

        /// <summary>
        ///     serialize-able, settable properties
        /// </summary>
        /// <param name="filled">when true, ensures the properties have been explicitly set</param>
        /// <returns></returns>
        public PropertyInfo[] GetFormObjectMappedProperties(bool filled = false)
        {
            PropertyInfo[] p = CacheMan.Cache(() =>
                                              {
                                                  return GetType().GetProperties(
                                                      BindingFlags.IgnoreCase
                                                      | BindingFlags.Public
                                                      | BindingFlags.Instance
                                                      | BindingFlags.SetProperty).Where(m =>
                                                                                        m.CanWrite
                                                                                        && m.PropertyType.IsPublic
                                                                                        && m.PropertyType.IsSerializable
                                                                                        && !m.PropertyType.IsArray
                                                                                        && !ExpressionParser.GetNonNullableType(m.PropertyType).IsAbstract
                                                                                        && !ExpressionParser.GetNonNullableType(m.PropertyType).IsGenericType).ToArray();
                                              },
                false,
                GetType().FullName,
                "GetFormObjectMappedProperties");

            return p.Where(m => !filled || !this.IsDefaultValue(m)).ToArray();
        }

        #endregion GetForm*Properties*

        #region InfoPath Document XML Black List Properties

        [XmlIgnore]
        [DataMember]
        public override sealed bool? DocStatus {
            get {
                return base.DocStatus;
            }
            set {
                base.DocStatus = value;
            }
        }

        [XmlIgnore]
        [DataMember]
        public override sealed int DocChecksum {
            get {
                return base.DocChecksum;
            }
            set {
                base.DocChecksum = value;
            }
        }

        /// <summary>
        ///     Ensure this is not written (XmlSerializer) to the final InfoPath document XML content
        /// </summary>
        [XmlIgnore]
        [DataMember(EmitDefaultValue = false)]
        [NotMapped]
        public override sealed string DocId {
            get {
                return base.DocId;
            }
            set {
                base.DocId = value;
            }
        }

        [IgnoreDataMember]
        [NonSerialized]
        private List<DocKey> docKey = new List<DocKey>();

        [XmlIgnore]
        [IgnoreDataMember]
        public virtual List<DocKey> DocKey { get { return docKey; } set { docKey = value; } }

        #endregion InfoPath Document XML Black List Properties
    }
}