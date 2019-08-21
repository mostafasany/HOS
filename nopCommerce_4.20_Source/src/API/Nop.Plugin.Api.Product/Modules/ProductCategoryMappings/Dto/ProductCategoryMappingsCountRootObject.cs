using Newtonsoft.Json;

namespace Nop.Plugin.Api.Product.Modules.ProductCategoryMappings.Dto
{
    public class ProductCategoryMappingsCountRootObject
    {
        [JsonProperty("count")] public int Count { get; set; }
    }
}