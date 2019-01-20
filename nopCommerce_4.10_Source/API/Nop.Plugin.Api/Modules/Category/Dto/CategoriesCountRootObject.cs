using Newtonsoft.Json;

namespace Nop.Plugin.Api.Modules.Categories.Dto
{
    public class CategoriesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}