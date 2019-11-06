using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace dCForm.Core
{
    /// <summary>
    ///     Writes Microsoft InfoPath specific & IPB proprietary XML processing instructions on each document
    ///     destined to be opened by the end-user.
    /// </summary>
    [DataContract(Namespace = "http://schemas.progablab.com/2014/12/Serialization")]
    [Serializable]
    public class DocProcessingInstructions : DocTerm, IDocProcessingInstructions
    {
        private string docSrc = string.Empty;
        private bool? docStatus;

        [XmlIgnore]
        [ScriptIgnore]
        [DataMember]
        public virtual int DocChecksum {
            get;
            set;
        }

        [XmlIgnore]
        [DataMember]
        public virtual bool? DocStatus {
            get {
                return docStatus;
            }
            set {
                docStatus = value;
            }
        }

        [XmlIgnore]
        [DataMember]
        public virtual string DocSubmittedBy
        {
            get;
            set;
        }

        [NotMapped]
        [XmlIgnore]
        [DataMember]
        public virtual string DocTitle {
            get;
            set;
        }

        [NotMapped]
        [XmlIgnore]
        [ScriptIgnore]
        [DataMember]
        public string href {
            get;
            set;
        }

        /// <summary>
        ///     Schema name
        /// </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        [ScriptIgnore]
        [NotMapped]
        public virtual string name {
            get;
            set;
        }

        /// <summary>
        ///     "One XML processing instruction tag named mso-infoPathSolution MUST be specified as part of the form file. This XML
        ///     processing instruction tag specifies properties, as defined by the following attributes, of this form file and the
        ///     associated form template."
        /// </summary>
        /// <summary>
        ///     Excluded from digest algorithms (form signatures); instruction properties that will retain there value from
        ///     previous     submitted infopath xml documents after they have been signed. "0.0.0.0" as the default
        ///     also tricks InfoPath into parsing are less then perfect Xml that is rendered for the     initial DocData
        /// </summary>
        [DataMember(Name = "DocRev")]
        [XmlIgnore]
        [ScriptIgnore]
        [NotMapped]
        public virtual string solutionVersion {
            get;
            set;
        }

        //TODO:Test to make sure solutionVersion { get; private set; } to { get; set; } did not break anything
    }
}