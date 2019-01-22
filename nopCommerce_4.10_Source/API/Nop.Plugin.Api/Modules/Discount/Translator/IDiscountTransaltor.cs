using Nop.Plugin.Api.Modules.Discount.Dto;

namespace Nop.Plugin.Api.Modules.Discount.Translator
{
    public interface IDiscountTransaltor
    {
        DiscountDto PrepateDiscountDto(Core.Domain.Discounts.Discount article);
    }
}