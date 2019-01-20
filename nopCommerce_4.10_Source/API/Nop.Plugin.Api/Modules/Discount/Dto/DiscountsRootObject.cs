using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Modules.Discounts.Dto
{
    public class DiscountsRootObject : ISerializableObject
    {
        public DiscountsRootObject() => Discounts = new List<DiscountDto>();

        [JsonProperty("discounts")]
        public IList<DiscountDto> Discounts { get; set; }

        public string GetPrimaryPropertyName() => "discounts";

        public Type GetPrimaryPropertyType() => typeof(DiscountDto);
    }
}