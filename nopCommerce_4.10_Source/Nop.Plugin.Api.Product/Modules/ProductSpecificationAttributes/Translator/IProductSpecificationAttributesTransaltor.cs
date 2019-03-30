using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.DTOs.Product;

namespace Nop.Plugin.Api.Product.Modules.ProductSpecificationAttributes.Translator
{
    public interface IProductSpecificationAttributesTransaltor
    {
        ProductSpecificationAttributeDto PrepareProductSpecificationAttributeDto(ProductSpecificationAttribute productSpecificationAttribute);
    }
}