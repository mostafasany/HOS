using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Modules.Articles.Dto
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