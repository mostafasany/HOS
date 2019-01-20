using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Modules.Product.Dto;

namespace Nop.Plugin.Api.Modules.Product.Translator
{
    public static class ProductDtoMappings
    {
        public static ProductDto ToDto(this Core.Domain.Catalog.Product product)
        {
            ProductDto productDto = product.MapTo<Core.Domain.Catalog.Product, ProductDto>();
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