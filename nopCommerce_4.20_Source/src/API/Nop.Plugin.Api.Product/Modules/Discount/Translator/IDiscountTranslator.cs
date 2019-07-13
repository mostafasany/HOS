using Nop.Plugin.Api.Product.Modules.Discount.Dto;

namespace Nop.Plugin.Api.Product.Modules.Discount.Translator
{
    public interface IDiscountTranslator
    {
        DiscountDto ToDiscountDto(Core.Domain.Discounts.Discount discount);
    }
}