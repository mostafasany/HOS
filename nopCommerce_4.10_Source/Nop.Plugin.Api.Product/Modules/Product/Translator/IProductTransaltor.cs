using Nop.Plugin.Api.Common.DTOs.Product;

namespace Nop.Plugin.Api.Product.Modules.Product.Translator
{
    public interface IProductTransaltor
    {
        ProductDto PrepareProductDTO(Core.Domain.Catalog.Product product);
    }
}