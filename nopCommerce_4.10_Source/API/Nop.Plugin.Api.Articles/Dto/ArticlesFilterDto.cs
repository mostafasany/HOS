using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;
using System;

namespace Nop.Plugin.Api.Article.Dto
{
    public class ArticlesFilterDto : ISerializableObject
    {
        public ArticlesFilterDto(string name, string value)
        {
            Name = name;
            Value = value;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        public string GetPrimaryPropertyName() => "name";

        public Type GetPrimaryPropertyType() => typeof(string);
    }
}
