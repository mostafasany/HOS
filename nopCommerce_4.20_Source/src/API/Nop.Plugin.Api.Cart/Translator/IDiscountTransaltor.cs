using Nop.Plugin.Api.Cart.Modules.Discount.Dto;

namespace Nop.Plugin.Api.Cart.Modules.Discount.Translator
{
    public interface IDiscountTransaltor
    {
        DiscountDto PrepateDiscountDto(Core.Domain.Discounts.Discount discount);
    }
}