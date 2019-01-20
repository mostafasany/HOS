using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Modules.Products.Dto
{
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