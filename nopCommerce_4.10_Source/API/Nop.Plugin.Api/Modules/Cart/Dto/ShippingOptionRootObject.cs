using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Modules.Cart.Dto
{
    public class ShippingOptionRootObject : ISerializableObject
    {
        public ShippingOptionRootObject() => ShippingOptions = new List<ShippingOptionDto>();

        [JsonProperty("shipping_options")]
        public IList<ShippingOptionDto> ShippingOptions { get; set; }

        public string GetPrimaryPropertyName() => "shipping_options";

        public Type GetPrimaryPropertyType() => typeof(ShippingOptionDto);
    }
}