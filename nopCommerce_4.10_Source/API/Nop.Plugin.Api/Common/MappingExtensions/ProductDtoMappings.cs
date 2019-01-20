using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Modules.Products.Dto;

namespace Nop.Plugin.Api.Common.MappingExtensions
{
    public static class ProductDtoMappings
    {
        public static ProductDto ToDto(this Product product)
        {
            ProductDto productDto = product.MapTo<Product, ProductDto>();
            productDto.FullDescription = product.FullDescription;
            return productDto;
        }

        public static ProductAttributeValueDto ToDto(this ProductAttributeValue productAttributeValue)
        {
            ProductAttributeValueDto productAttributeValueDto = productAttributeValue.MapTo<ProductAttributeValue, ProductAttributeValueDto>();
            productAttributeValueDto.PriceAdjustmentUsePercentage = productAttributeValue.PriceAdjustmentUsePercentage;
            return productAttributeValueDto;
        }
    }
}