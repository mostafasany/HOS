using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Modules.SpecificationAttributes.Dto
{
    public class ProductSpecificationAttributesRootObjectDto : ISerializableObject
    {
        public ProductSpecificationAttributesRootObjectDto() => ProductSpecificationAttributes = new List<ProductSpecificationAttributeDto>();

        [JsonProperty("product_specification_attributes")]
        public IList<ProductSpecificationAttributeDto> ProductSpecificationAttributes { get; set; }

        public string GetPrimaryPropertyName() => "product_specification_attributes";

        public Type GetPrimaryPropertyType() => typeof(ProductSpecificationAttributeDto);
    }
}