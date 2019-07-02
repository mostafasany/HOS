using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Product.Modules.ProductCategoryMappings.Dto
{
    public class ProductCategoryMappingsRootObject : ISerializableObject
    {
        public ProductCategoryMappingsRootObject() => ProductCategoryMappingDtos = new List<ProductCategoryMappingDto>();

        [JsonProperty("product_category_mappings")]
        public IList<ProductCategoryMappingDto> ProductCategoryMappingDtos { get; set; }

        public string GetPrimaryPropertyName() => "product_category_mappings";

        public Type GetPrimaryPropertyType() => typeof(ProductCategoryMappingDto);
    }
}