using Newtonsoft.Json;

namespace Nop.Plugin.Api.Modules.Product.Dto
{
    public class ProductsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}