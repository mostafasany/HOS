using Nop.Core.Domain.Discounts;
using Nop.Plugin.Api.Cart.Dto;

namespace Nop.Plugin.Api.Cart.Translator
{
    public interface IDiscountTranslator
    {
        DiscountDto ToDiscountDto(Discount discount);
    }
}