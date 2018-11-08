using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.ShoppingCarts
{
    public class ShippingOptionRootObject : ISerializableObject
    {
        public ShippingOptionRootObject()
        {
            ShippingOptions = new List<ShippingOptionDto>();
        }

        [JsonProperty("shipping_options")]
        public IList<ShippingOptionDto> ShippingOptions { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "shipping_options";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof (ShippingOptionDto);
        }
    }
}