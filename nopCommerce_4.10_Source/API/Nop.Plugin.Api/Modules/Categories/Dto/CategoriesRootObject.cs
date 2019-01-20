using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Modules.Categories.Dto
{
    public class CategoriesRootObject : ISerializableObject
    {
        public CategoriesRootObject() => Categories = new List<CategoryDto>();

        [JsonProperty("categories")]
        public IList<CategoryDto> Categories { get; set; }

        public string GetPrimaryPropertyName() => "categories";

        public Type GetPrimaryPropertyType() => typeof(CategoryDto);
    }
}