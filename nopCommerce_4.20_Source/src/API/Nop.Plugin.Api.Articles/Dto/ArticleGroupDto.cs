using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs.Base;

namespace Nop.Plugin.Api.Article.Dto
{
    [JsonObject(Title = "articleGroup")]
    public class ArticleGroupDto : BaseDto
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("parentGroupId")] public int ParentGroupId { get; set; }
    }
}