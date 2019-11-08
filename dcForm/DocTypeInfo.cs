using System;
using System.Runtime.Serialization;

namespace dCForm
{
    [DataContract(Namespace = "http://schemas.progablab.com/2014/12/Serialization")]
    [Serializable]
    public class DocTypeInfo
    {
        [DataMember(EmitDefaultValue = false)]
        public string DocTypeVer { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string DocTypeName { get; set; }

        /// <summary>
        ///     a per-DocTypeName basis description that is supplied by the specific DocDataInterpreter
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Description { get; set; }

        /// <summary>
        ///     If there are signature lines in the form
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public bool IsSignable { get; set; }
    }  
}