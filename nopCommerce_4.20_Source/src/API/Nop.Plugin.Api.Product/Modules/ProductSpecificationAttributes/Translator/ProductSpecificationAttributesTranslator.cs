using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Common.DTOs.Product;

namespace Nop.Plugin.Api.Product.Modules.ProductSpecificationAttributes.Translator
{
    public class ProductSpecificationAttributesTranslator : IProductSpecificationAttributesTranslator
    {
        public ProductSpecificationAttributeDto PrepareProductSpecificationAttributeDto(
            ProductSpecificationAttribute productSpecificationAttribute)
        {
            return productSpecificationAttribute.ToDto();
        }
    }
}