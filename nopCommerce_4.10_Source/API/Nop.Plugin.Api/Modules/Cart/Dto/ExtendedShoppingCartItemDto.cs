using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Base;

namespace Nop.Plugin.Api.DTOs.ShoppingCarts
{
    [JsonObject(Title = "shopping_cart_item")]
    public class ExtendedShoppingCartItemDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the total
        /// </summary>
        [JsonProperty("shopping_cart_item")]
        public ShoppingCartItemDto ShoppingCartItem { get; set; }

        /// <summary>
        ///     Gets or sets the total
        /// </summary>
        [JsonProperty("total")]
        public decimal? Total { get; set; }

        /// <summary>
        ///     Gets or sets the price
        /// </summary>
        [JsonProperty("price")]
        public decimal? Price { get; set; }

        /// <summary>
        ///     Gets or sets the extra info
        /// </summary>
        [JsonProperty("extra_info")]
        public string ExtraInfo { get; set; }

        /// <summary>
        ///     Gets or sets the total
        /// </summary>
        [JsonProperty("discount")]
        public decimal? Discount { get; set; }

        /// <summary>
        ///     Gets or sets the discount applied
        /// </summary>
        [JsonProperty("discount_applied")]
        public bool DiscountApplied { get; set; }
    }
}