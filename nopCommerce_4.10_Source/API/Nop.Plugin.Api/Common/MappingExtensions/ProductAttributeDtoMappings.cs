using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Modules.ProductsAttributes.Dto;

namespace Nop.Plugin.Api.Common.MappingExtensions
{
    public static class ProductAttributeDtoMappings
    {
        public static ProductAttributeDto ToDto(this ProductAttribute productAttribute)
        {
            return productAttribute.MapTo<ProductAttribute, ProductAttributeDto>();
        }
    }
}
