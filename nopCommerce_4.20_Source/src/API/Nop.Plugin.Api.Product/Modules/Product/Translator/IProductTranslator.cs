using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.DTOs.Product;
using Nop.Plugin.Api.Product.Modules.Product.Dto;

namespace Nop.Plugin.Api.Product.Modules.Product.Translator
{
    public interface IProductTranslator
    {
        ProductDto ToDto(Core.Domain.Catalog.Product product);
        ProductReviewDto ToReviewDto(ProductReview productReview);
    }
}