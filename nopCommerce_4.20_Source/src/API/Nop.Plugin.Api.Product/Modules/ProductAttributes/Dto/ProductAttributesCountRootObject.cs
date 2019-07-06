using Newtonsoft.Json;

namespace Nop.Plugin.Api.Product.Modules.ProductAttributes.Dto
{
    public class ProductAttributesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}