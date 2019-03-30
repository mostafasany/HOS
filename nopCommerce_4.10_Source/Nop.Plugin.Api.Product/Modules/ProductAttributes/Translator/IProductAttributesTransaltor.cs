using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Product.Modules.ProductAttributes.Dto;

namespace Nop.Plugin.Api.Product.Modules.ProductAttributes.Translator
{
    public interface IProductAttributesTransaltor
    {
        ProductAttributeDto PrepareProductAttributeDTO(ProductAttribute productAttribute);
    }
}