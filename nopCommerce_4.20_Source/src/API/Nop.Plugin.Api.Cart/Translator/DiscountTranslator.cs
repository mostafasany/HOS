using Nop.Core.Domain.Discounts;
using Nop.Plugin.Api.Cart.Dto;

namespace Nop.Plugin.Api.Cart.Translator
{
    public class DiscountTranslator : IDiscountTranslator
    {
        public DiscountDto ToDiscountDto(Discount discount)
        {
            return new DiscountDto
            {
                CouponCode = discount.CouponCode,
                Name = discount.Name,
                Id = discount.Id,
                DiscountLimitationId = discount.DiscountLimitationId,
                DiscountPercentage = discount.DiscountPercentage,
                DiscountTypeId = discount.DiscountTypeId,
                IsCumulative = discount.IsCumulative,
                LimitationTimes = discount.LimitationTimes,
                MaximumDiscountAmount = discount.MaximumDiscountAmount,
                MaximumDiscountedQuantity = discount.MaximumDiscountedQuantity,
                RequiresCouponCode = discount.RequiresCouponCode,
                UsePercentage = discount.UsePercentage,
                DiscountAmount = discount.DiscountAmount,
                EndDateUtc = discount.EndDateUtc,
                StartDateUtc = discount.StartDateUtc
            };
        }
    }
}