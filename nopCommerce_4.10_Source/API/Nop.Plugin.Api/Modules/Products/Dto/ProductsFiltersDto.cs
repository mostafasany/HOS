using System;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Products
{
    public class ProductsFiltersDto : ISerializableObject
    {
        public ProductsFiltersDto(string name, string value)
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