using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace dCForm.Format.Json
{
    /// <summary>
    ///     controls order of property json serialization placing ProcessingInstruction declared deigned properties at the top
    ///     of the output, maintaining order as they are defined in the actual ProcessingInstruction POCO
    /// </summary>
    internal class WithPIContractResolver : DefaultContractResolver
    {
        /// <summary>
        ///     DocProcessingInstructionProperties are inserted at the top of the document before any other properties. This is
        ///     simply for constancy
        /// </summary>
        protected static readonly List<string> DocProcessingInstructionProperties = typeof (DocProcessingInstructions)
            .GetProperties()
            .Where(p =>
                   p.DeclaringType == typeof (DocProcessingInstructions)
                   ||
                   p.DeclaringType == typeof (DocTerm)
                   ||
                   p.DeclaringType == typeof (BaseAutoIdent)
            )
            .Select(p => p.Name).ToList();

        /// <summary>
        ///     These properties should never serialize as they don't need to be exposed to the outside world
        /// </summary>
        protected static readonly List<string> IgnoredProperties = new List<string>
        {
            "DocSrc", "DocIdKeys", "Id", "name", "FormTitleFormat", "DocKey"
        };

        /// <summary>
        ///     set order & would should & should not read/serialize
        /// </summary>
        /// <param name="member"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            int PocoPropertyOrdinal = DocProcessingInstructionProperties.IndexOf(property.PropertyName);
            if (PocoPropertyOrdinal != -1)
                property.Order = PocoPropertyOrdinal;

            property.Ignored = IgnoredProperties.Contains(member.Name);
            property.ShouldSerialize = instance => !property.Ignored;
            property.DefaultValueHandling = DefaultValueHandling.Include;
            property.PropertyName = member.Name;

            return property;
        }
    }
}