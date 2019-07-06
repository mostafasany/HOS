using Nop.Plugin.Api.Product.Modules.Discount.Dto;


namespace Nop.Plugin.Api.Product.Modules.Discount.Translator
{
    public class DiscountTransaltor : IDiscountTransaltor
    {
        public DiscountDto PrepateDiscountDto(Core.Domain.Discounts.Discount discount) => new DiscountDto
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
            DiscountAmount=discount.DiscountAmount,
            EndDateUtc=discount.EndDateUtc,
            StartDateUtc=discount.StartDateUtc,
        };
    }
}