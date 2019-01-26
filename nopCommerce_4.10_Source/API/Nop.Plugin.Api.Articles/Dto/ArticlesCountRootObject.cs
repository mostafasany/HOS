using Newtonsoft.Json;

namespace Nop.Plugin.Api.Article.Dto
{
    public class ArticlesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}