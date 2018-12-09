using Nop.Plugin.Api.AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTOs.Products;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ProductDtoMappings
    {
        public static ProductDto ToDto(this Product product)
        {
            var productDto= product.MapTo<Product, ProductDto>();
            productDto.FullDescription = product.FullDescription;
            return productDto;
        }

        public static ProductAttributeValueDto ToDto(this ProductAttributeValue productAttributeValue)
        {
            var productAttributeValueDto = productAttributeValue.MapTo<ProductAttributeValue, ProductAttributeValueDto>();
            productAttributeValueDto.PriceAdjustmentUsePercentage = productAttributeValue.PriceAdjustmentUsePercentage;
            return productAttributeValueDto;
        }
    }
}