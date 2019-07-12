using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Content.Modules.Language.Dto
{
    public class LanguagesRootObject : ISerializableObject
    {
        public LanguagesRootObject()
        {
            Languages = new List<LanguageDto>();
        }

        [JsonProperty("languages")] public IList<LanguageDto> Languages { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "languages";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(LanguageDto);
        }
    }
}