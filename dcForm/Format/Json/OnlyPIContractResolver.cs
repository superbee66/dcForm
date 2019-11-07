using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace dCForm.Format.Json
{
    /// <summary>
    ///     controls order of property json serialization placing ProcessingInstruction declared degined properties at the top
    ///     of the output, maintaining order as they are defined in the actual ProcessingInstruction POCO
    /// </summary>
    class OnlyPiContractResolver : WithPIContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (!IgnoredProperties.Contains(member.Name))
                property.Ignored = !DocProcessingInstructionProperties.Contains(property.PropertyName);

            property.ShouldSerialize = instance => !property.Ignored;

            return property;
        }
    }
}