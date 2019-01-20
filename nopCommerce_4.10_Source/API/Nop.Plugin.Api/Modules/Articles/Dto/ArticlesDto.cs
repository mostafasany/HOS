using System;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs.Base;
using Nop.Plugin.Api.Modules.Pictures.Dto;

namespace Nop.Plugin.Api.Modules.Articles.Dto
{
    [JsonObject(Title = "articles")]
    public class ArticlesDto : BaseDto
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("se_name")]
        public string SeName { get; set; }

        [JsonProperty("parent_id")]
        public int ParentId { get; set; }

        [JsonProperty("metaTitle")]
        public string MetaTitle { get; set; }

        [JsonProperty("metaDescription")]
        public string MetaDescription { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("allowComments")]
        public bool AllowComments { get; set; }

        [JsonProperty("commentCount")]
        public int CommentCount { get; set; }

        [JsonProperty("createdOnUtc")]
        public DateTime CreatedOnUtc { get; set; }

        [JsonProperty("updatedOnUtc")]
        public DateTime UpdatedOnUtc { get; set; }

        [JsonProperty("image")]
        public ImageDto Image { get; set; }
    }
}