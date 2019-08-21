using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs.Base;

namespace Nop.Plugin.Api.Common.DTOs
{
    [JsonObject(Title = "attribute")]
    public class ProductItemAttributeDto : BaseDto
    {
        [JsonProperty("value")] public string Value { get; set; }
    }
}