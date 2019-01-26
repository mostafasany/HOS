using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Articles.Dto
{
    public class ArticlesRootObject : ISerializableObject
    {
        public ArticlesRootObject() => Articles = new List<ArticlesDto>();

        [JsonProperty("articles")]
        public IList<ArticlesDto> Articles { get; set; }

        public string GetPrimaryPropertyName() => "articles";

        public Type GetPrimaryPropertyType() => typeof(ArticlesDto);
    }
}