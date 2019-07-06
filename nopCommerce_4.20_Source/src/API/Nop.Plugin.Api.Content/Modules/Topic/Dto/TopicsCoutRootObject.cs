using Newtonsoft.Json;

namespace Nop.Plugin.Api.Content.Modules.Topic.Dto
{
    public class TopicsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
