using Newtonsoft.Json;

namespace Nop.Plugin.Api.Modules.Category.Dto
{
    public class CategoriesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}