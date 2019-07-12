using Newtonsoft.Json;

namespace Nop.Plugin.Api.Product.Modules.Product.Dto
{
    public class ProductsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}