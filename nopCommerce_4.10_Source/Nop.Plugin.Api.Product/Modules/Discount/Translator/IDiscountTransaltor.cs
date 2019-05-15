using Nop.Plugin.Api.Product.Modules.Discount.Dto;

namespace Nop.Plugin.Api.Product.Modules.Discount.Translator
{
    public interface IDiscountTransaltor
    {
        DiscountDto PrepateDiscountDto(Core.Domain.Discounts.Discount discount);
    }
}