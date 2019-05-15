using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Common.DTOs.Product;

namespace Nop.Plugin.Api.Product.Modules.Product.Dto
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

        [JsonProperty("header_title")]
        public string HeaderTitle { get; set; }

        public string GetPrimaryPropertyName() => "products";

        public Type GetPrimaryPropertyType() => typeof(ProductDto);
    }
}