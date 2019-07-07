using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Customer.Modules.Order.Dto.Orders
{
    public class OrdersRootObject : ISerializableObject
    {
        public OrdersRootObject() => Orders = new List<OrderDto>();

        [JsonProperty("orders")]
        public IList<OrderDto> Orders { get; set; }

        public string GetPrimaryPropertyName() => "orders";

        public Type GetPrimaryPropertyType() => typeof(OrderDto);
    }
}