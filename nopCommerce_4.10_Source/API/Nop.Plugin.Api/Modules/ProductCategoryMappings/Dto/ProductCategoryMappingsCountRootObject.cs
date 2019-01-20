using Newtonsoft.Json;

namespace Nop.Plugin.Api.Modules.ProductsCategoryMappings.Dto
{
    public class ProductCategoryMappingsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}