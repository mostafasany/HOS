using Nop.Plugin.Api.Modules.Product.Dto;

namespace Nop.Plugin.Api.Modules.Product.Translator
{
    public interface IProductTransaltor
    {
        ProductDto PrepareProductDTO(Core.Domain.Catalog.Product product);
    }
}