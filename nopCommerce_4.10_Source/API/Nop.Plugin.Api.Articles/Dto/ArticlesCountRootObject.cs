using Newtonsoft.Json;

namespace Nop.Plugin.Api.Articles.Dto
{
    public class ArticlesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}