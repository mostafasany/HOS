using Newtonsoft.Json;

namespace Nop.Plugin.Api.Modules.SpecificationAttributes.Dto
{
    public class ProductSpecificationAttributesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}