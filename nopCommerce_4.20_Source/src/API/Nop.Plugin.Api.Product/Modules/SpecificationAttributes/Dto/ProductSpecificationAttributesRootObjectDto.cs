using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Common.DTOs.Product;

namespace Nop.Plugin.Api.Product.Modules.SpecificationAttributes.Dto
{
    public class ProductSpecificationAttributesRootObjectDto : ISerializableObject
    {
        public ProductSpecificationAttributesRootObjectDto()
        {
            ProductSpecificationAttributes = new List<ProductSpecificationAttributeDto>();
        }

        [JsonProperty("product_specification_attributes")]
        public IList<ProductSpecificationAttributeDto> ProductSpecificationAttributes { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "product_specification_attributes";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ProductSpecificationAttributeDto);
        }
    }
}