﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Common.DTOs.Product;

namespace Nop.Plugin.Api.Product.Modules.SpecificationAttributes.Dto
{
    public class SpecificationAttributesRootObjectDto : ISerializableObject
    {
        public SpecificationAttributesRootObjectDto() => SpecificationAttributes = new List<SpecificationAttributeDto>();

        [JsonProperty("specification_attributes")]
        public IList<SpecificationAttributeDto> SpecificationAttributes { get; set; }

        public string GetPrimaryPropertyName() => "specification_attributes";

        public Type GetPrimaryPropertyType() => typeof(SpecificationAttributeDto);
    }
}