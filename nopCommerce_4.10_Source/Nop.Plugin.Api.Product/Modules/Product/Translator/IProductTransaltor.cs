using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.DTOs.Product;
using Nop.Plugin.Api.Product.Modules.Product.Dto;

namespace Nop.Plugin.Api.Product.Modules.Product.Translator
{
    public interface IProductTransaltor
    {
        ProductDto PrepareProductDTO(Core.Domain.Catalog.Product product);
        ProductReviewDto PrepareProductReviewDTO(ProductReview productReview);
    }
}