using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Modules.SpecificationAttributes.Dto;

namespace Nop.Plugin.Api.Modules.ProductSpecificationAttributes.Translator
{
    public interface IProductSpecificationAttributesTransaltor
    {
        ProductSpecificationAttributeDto PrepareProductSpecificationAttributeDto(ProductSpecificationAttribute productSpecificationAttribute);
    }
}