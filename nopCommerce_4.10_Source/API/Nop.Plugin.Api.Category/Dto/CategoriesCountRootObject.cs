using Newtonsoft.Json;

namespace Nop.Plugin.Api.Category.Dto
{
    public class CategoriesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}