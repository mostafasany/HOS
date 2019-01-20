using Newtonsoft.Json;

namespace Nop.Plugin.Api.Modules.ProductAttributes.Dto
{
    public class ProductAttributesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}