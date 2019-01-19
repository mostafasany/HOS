using System;
using System.Collections.Generic;
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

    public class ProductsRootObjectDto : ISerializableObject
    {
        public ProductsRootObjectDto()
        {
            Products = new List<ProductDto>();
            Filters = new List<ProductsFiltersDto>();
        }

        [JsonProperty("products")]
        public IList<ProductDto> Products { get; set; }

        [JsonProperty("filters")]
        public List<ProductsFiltersDto> Filters { get; set; }

        public string GetPrimaryPropertyName() => "products";

        public Type GetPrimaryPropertyType() => typeof(ProductDto);
    }
}