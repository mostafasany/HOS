using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Base;
using System.Collections.Generic;

namespace Nop.Plugin.Api.DTOs.ShoppingCarts
{
    [JsonObject(Title = "shopping_cart")]
    public class ExtendedShoppingCartDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the total
        /// </summary>
        [JsonProperty("shopping_cart_items")]
        public List<ExtendedShoppingCartItemDto> ShoppingCartItems { get; set; }

        /// <summary>
        ///     Gets or sets the total
        /// </summary>
        [JsonProperty("total_discount")]
        public decimal? TotalDiscount { get; set; }


        /// <summary>
        ///     Gets or sets the total
        /// </summary>
        [JsonProperty("total")]
        public decimal? Total { get; set; }


        /// <summary>
        ///     Gets or sets the extra info
        /// </summary>
        [JsonProperty("total_extra_info")]
        public string TotalExtraInfo { get; set; }


        /// <summary>
        ///     Gets or sets the total
        /// </summary>
        [JsonProperty("sub_total_discount")]
        public decimal? SubTotalDiscount { get; set; }

        /// <summary>
        ///     Gets or sets the total
        /// </summary>
        [JsonProperty("sub_total")]
        public decimal? SubTotal { get; set; }

        /// <summary>
        ///     Gets or sets the extra info
        /// </summary>
        [JsonProperty("sub_total_extra_info")]
        public string SubTotalExtraInfo { get; set; }


        /// <summary>
        ///     Gets or sets the total
        /// </summary>
        [JsonProperty("shipping")]
        public decimal? Shipping { get; set; }


        /// <summary>
        ///     Gets or sets the total
        /// </summary>
        [JsonProperty("tax")]
        public decimal? Tax { get; set; }
    }
}