using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs.Base;
using System;

namespace Nop.Plugin.Api.Product.Modules.Discount.Dto
{
    [JsonObject(Title = "discounts")]
    public class DiscountDto : BaseDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("discountTypeId")]
        public int DiscountTypeId { get; set; }

        [JsonProperty("discountAmount")]
        public decimal DiscountAmount { get; set; }

        [JsonProperty("usePercentage")]
        public bool UsePercentage { get; set; }

        [JsonProperty("discountPercentage")]
        public decimal DiscountPercentage { get; set; }

        [JsonProperty("maximumDiscountAmount")]
        public decimal? MaximumDiscountAmount { get; set; }

        [JsonProperty("requiresCouponCode")]
        public bool RequiresCouponCode { get; set; }

        [JsonProperty("couponCode")]
        public string CouponCode { get; set; }

        [JsonProperty("isCumulative")]
        public bool IsCumulative { get; set; }

        [JsonProperty("maximumDiscountedQuantity")]
        public int? MaximumDiscountedQuantity { get; set; }

        [JsonProperty("limitationTimes")]
        public int LimitationTimes { get; set; }

        [JsonProperty("discountLimitationId")]
        public int DiscountLimitationId { get; set; }

        [JsonProperty("startDateUtc")]
        public DateTime? StartDateUtc { get; set; }

        [JsonProperty("endDateUtc")]
        public DateTime? EndDateUtc { get; set; }

    }
}