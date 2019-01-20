using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Base;

namespace Nop.Plugin.Api.DTOs.Articles
{
    [JsonObject(Title = "articleGroup")]
    public class ArticleGroupDto : BaseDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("parentGroupId")]
        public int ParentGroupId { get; set; }
    }
}