using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using dCForm.Util;
using Newtonsoft.Json;

namespace dCForm
{
    /// <summary>
    ///     Writes a Lucene document store database string field value that acts as a unique among
    ///     all other documents (of any DocType) persisted to the primary repository of this solution
    /// </summary>
    [DataContract(Namespace = "http://schemas.progablab.com/2014/12/Serialization")]
    [Serializable]
    public class DocTerm : BaseAutoIdent, IDocTerm
    {
        private string _docTypeName;

        [XmlIgnore]
        [ScriptIgnore]
        [NotMapped]
        [DataMember]
        public virtual string DocId {
            get;
            set;
        }

        [NotMapped]
        [XmlIgnore]
        [DataMember]
        public Dictionary<string, string> DocIdKeys {
            get {
                return DocKey.DocIdToKeys(DocId);
            }
            set {
                DocId = DocKey.DocIdFromKeys(value);
            }
        }

        /// <summary>
        ///     The reflected GetType().Name of this object
        /// </summary>
        [XmlIgnore]
        [NotMapped]
        [DataMember]
        public virtual string DocTypeName {
            get {
                return (_docTypeName ?? (_docTypeName = GetType().Name));
            }
            set {
                _docTypeName = value;
            }
        }

        public string AsTermTxt()
        {
            return JsonConvert.SerializeObject(new
            {
                //TODO:sort dictionary before serializing
                DocTypeName,
                DocKeys = DocIdKeys
            });
        }


    }
}