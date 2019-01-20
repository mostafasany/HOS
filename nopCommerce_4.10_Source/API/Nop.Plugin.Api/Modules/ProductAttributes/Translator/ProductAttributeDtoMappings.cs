using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Modules.ProductAttributes.Dto;

namespace Nop.Plugin.Api.Modules.ProductAttributes.Translator
{
    public static class ProductAttributeDtoMappings
    {
        public static ProductAttributeDto ToDto(this ProductAttribute productAttribute) => productAttribute.MapTo<ProductAttribute, ProductAttributeDto>();
    }
}