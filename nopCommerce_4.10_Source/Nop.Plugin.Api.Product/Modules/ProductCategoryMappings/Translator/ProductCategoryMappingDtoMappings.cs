﻿using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Modules.ProductCategoryMappings.Dto;

namespace Nop.Plugin.Api.Modules.ProductCategoryMappings.Translator
{
    public static class ProductCategoryMappingDtoMappings
    {
        public static ProductCategoryMappingDto ToDto(this ProductCategory mapping) => mapping.MapTo<ProductCategory, ProductCategoryMappingDto>();
    }
}