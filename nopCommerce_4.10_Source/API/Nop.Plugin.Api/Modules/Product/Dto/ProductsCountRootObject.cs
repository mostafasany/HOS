using Newtonsoft.Json;

namespace Nop.Plugin.Api.Modules.Products.Dto
{
    public class ProductsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}