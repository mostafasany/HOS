﻿using System;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Base;
using Nop.Plugin.Api.DTOs.Images;

namespace Nop.Plugin.Api.DTOs.Articles
{
    [JsonObject(Title = "articles")]
    public class ArticlesDto : BaseDto
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

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


        [JsonProperty("image")]
        public ImageDto Image { get; set; }



    }
}