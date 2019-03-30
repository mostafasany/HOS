using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Common.DTOs.Product;

namespace Nop.Plugin.Api.Product.Modules.ProductSpecificationAttributes.Translator
{
    public class ProductSpecificationAttributesTransaltor : IProductSpecificationAttributesTransaltor
    {
        public ProductSpecificationAttributeDto PrepareProductSpecificationAttributeDto(ProductSpecificationAttribute productSpecificationAttribute) => productSpecificationAttribute.ToDto();
    }
}