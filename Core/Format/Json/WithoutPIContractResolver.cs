using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace dCForm.Core.Format.Json
{
    class WithoutPIContractResolver : WithPIContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (!IgnoredProperties.Contains(member.Name))
                property.Ignored = DocProcessingInstructionProperties.Contains(property.PropertyName);

            property.ShouldSerialize = instance => !property.Ignored;

            return property;
        }
    }
}