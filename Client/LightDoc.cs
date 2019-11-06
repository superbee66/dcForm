using System;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using dCForm.Client.Util;

//TODO:Move to DCF_Relay
namespace dCForm.Client
{
    using InfoPathBroker = dCForm;
    /// <summary>
    ///     Summary & links about persisted documents. LightDoc(s) & BaseDoc(s) are many-to-one
    ///     A different LighDoc is created for every submission of the BaseDoc. Normal
    ///     queries to the system should only yield the latest LighDoc for there BaseDocs
    /// </summary>
    [DataContract(Namespace = "http://schemas.progablab.com/2014/12/Serialization")]
    [Serializable]
    public class LightDoc : IDocLogistics, IDocIdentifiers, IComparable<LightDoc>
    {
        private static readonly JavaScriptSerializer Jsonizer = new JavaScriptSerializer();

        /// <summary>
        ///     Simply this object in json form
        /// </summary>
        [ScriptIgnore]
        public string DataKeyField { get { return Serialize.Json.Serialize(this); } }

        /// <summary>
        ///     Should be the same if this LightDoc represents the first submission for the BaseDoc
        /// </summary>
        [DataMember]
        public DateTime DocSubmitDate { get; set; }

        [DataMember]
        public string DocTitle { get; set; }

        [DataMember]
        public string DocTypeName { get; set; }

        [IgnoreDataMember]
        [ScriptIgnore]
        public long LogSequenceNumber {
            get { return DocSubmitDate.ToFileTimeUtc(); }
            set { DocSubmitDate = DateTime.FromFileTimeUtc(value); }
        }

        public int CompareTo(LightDoc Other)
        {
            int i = 0;

            if (i == 0)
                i = Other.DocSubmitDate.CompareTo(DocSubmitDate);

            if (i == 0)
                i = Other.DocStatus.HasValue.CompareTo(DocStatus.HasValue);

            if (i == 0)
                i = (DocStatus != null && DocStatus.Value).CompareTo(Other.DocStatus != null && Other.DocStatus.Value);

            if (i == 0)
                i = String.Compare(DocTitle, Other.DocTitle, StringComparison.OrdinalIgnoreCase);
            return i;
        }

        [DataMember]
        public string DocId { get; set; }

        /// <summary>
        ///     Reflects the current status of the persisted BaseDoc
        ///     null = the document has not been persisted yet, it has only been rendered for the user to open
        ///     0 =
        /// </summary>
        [DataMember]
        public bool? DocStatus { get; set; }

        [DataMember]
        public string DocSrc { get; set; }

        /// <summary>
        ///     uses a serialize-deserialize technique to construct a new LightDoc
        /// </summary>
        /// <param name="o"></param>
        /// <returns>property filled LightDoc</returns>
        public static LightDoc FromObject(object o)
        {
            //TODO:Make this FromObject conversion method type safe & "round-trippable" by restructuring architecture
            return (LightDoc)Serialize.Json.Deserialize(
                Serialize.Json.Serialize(o),
                typeof(LightDoc));
        }

        /// <summary>
        ///     self inflate from json (DataKeyField) string
        /// </summary>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static LightDoc Parse(string Json) { return Serialize.Json.Deserialize<LightDoc>(Json); }
    }
}