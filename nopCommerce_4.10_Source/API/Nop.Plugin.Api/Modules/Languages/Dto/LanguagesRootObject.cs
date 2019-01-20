using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Modules.Languages.Dto
{
    public class LanguagesRootObject : ISerializableObject
    {
        public LanguagesRootObject() => Languages = new List<LanguageDto>();

        [JsonProperty("languages")]
        public IList<LanguageDto> Languages { get; set; }

        public string GetPrimaryPropertyName() => "languages";

        public Type GetPrimaryPropertyType() => typeof(LanguageDto);
    }
}