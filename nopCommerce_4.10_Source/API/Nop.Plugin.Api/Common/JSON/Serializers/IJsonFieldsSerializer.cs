using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Common.JSON.Serializers
{
    public interface IJsonFieldsSerializer
    {
        string Serialize(ISerializableObject objectToSerialize, string fields);
    }
}
