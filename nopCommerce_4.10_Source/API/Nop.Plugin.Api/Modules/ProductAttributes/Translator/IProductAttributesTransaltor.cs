using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Modules.ProductAttributes.Dto;

namespace Nop.Plugin.Api.Modules.ProductAttributes.Translator
{
    public interface IProductAttributesTransaltor
    {
        ProductAttributeDto PrepareProductAttributeDTO(ProductAttribute productAttribute);
    }
}