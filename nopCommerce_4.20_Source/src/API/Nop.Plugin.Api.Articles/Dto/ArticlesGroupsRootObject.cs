using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Article.Dto
{
    public class ArticlesGroupsRootObject : ISerializableObject
    {
        public ArticlesGroupsRootObject()
        {
            ArticlesGroups = new List<ArticleGroupDto>();
        }

        [JsonProperty("articlesGroups")] public IList<ArticleGroupDto> ArticlesGroups { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "articlesGroups";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ArticleGroupDto);
        }
    }
}