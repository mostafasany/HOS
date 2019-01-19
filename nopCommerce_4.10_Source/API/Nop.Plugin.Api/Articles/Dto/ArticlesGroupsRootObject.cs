using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Articles
{
    public class ArticlesGroupsRootObject : ISerializableObject
    {
        public ArticlesGroupsRootObject() => ArticlesGroups = new List<ArticleGroupDto>();

        [JsonProperty("articlesGroups")]
        public IList<ArticleGroupDto> ArticlesGroups { get; set; }

        public string GetPrimaryPropertyName() => "articlesGroups";

        public Type GetPrimaryPropertyType() => typeof(ArticleGroupDto);
    }
}