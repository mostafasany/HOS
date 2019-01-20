using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Modules.ProductsAttributes.Dto
{
    public class ProductAttributesRootObjectDto : ISerializableObject
    {
        public ProductAttributesRootObjectDto() => ProductAttributes = new List<ProductAttributeDto>();

        [JsonProperty("product_attributes")]
        public IList<ProductAttributeDto> ProductAttributes { get; set; }

        public string GetPrimaryPropertyName() => "product_attributes";

        public Type GetPrimaryPropertyType() => typeof(ProductAttributeDto);
    }
}