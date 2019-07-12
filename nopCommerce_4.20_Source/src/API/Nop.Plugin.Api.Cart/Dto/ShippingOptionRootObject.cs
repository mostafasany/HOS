using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Cart.Dto
{
    public class ShippingOptionRootObject : ISerializableObject
    {
        public ShippingOptionRootObject()
        {
            ShippingOptions = new List<ShippingOptionDto>();
        }

        [JsonProperty("shipping_options")] public IList<ShippingOptionDto> ShippingOptions { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "shipping_options";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ShippingOptionDto);
        }
    }
}