﻿using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs.Base;

namespace Nop.Plugin.Api.Cart.Dto
{
    [JsonObject(Title = "shipping_option")]
    public class ShippingOptionDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets a shipping rate (without discounts, additional shipping charges, etc)
        /// </summary>
        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        /// <summary>
        ///     Gets or sets a shipping option name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets a shipping option description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("from_days")]
        public int FromDays { get; set; }

        [JsonProperty("to_days")]
        public int ToDays { get; set; }

        [JsonProperty("shippingRateComputationMethodSystemName")]
        public string ShippingRateComputationMethodSystemName { get; set; }
    }
}