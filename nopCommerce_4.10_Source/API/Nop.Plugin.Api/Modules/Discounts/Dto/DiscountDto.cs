using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Base;

namespace Nop.Plugin.Api.DTOs.Discounts
{
    [JsonObject(Title = "discounts")]
    public class DiscountDto : BaseDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("discountTypeId")]
        public int DiscountTypeId { get; set; }

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

        [JsonProperty("dfiscountLimitationId")]
        public int DiscountLimitationId { get; set; }

    }
}