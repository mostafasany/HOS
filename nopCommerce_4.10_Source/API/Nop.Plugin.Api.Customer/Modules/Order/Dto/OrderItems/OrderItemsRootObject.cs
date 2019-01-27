using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Customer.Modules.Order.Dto.OrderItems
{
    public class OrderItemsRootObject : ISerializableObject
    {
        public OrderItemsRootObject() => OrderItems = new List<OrderItemDto>();

        [JsonProperty("order_items")]
        public IList<OrderItemDto> OrderItems { get; set; }

        public string GetPrimaryPropertyName() => "order_items";

        public Type GetPrimaryPropertyType() => typeof(OrderItemDto);
    }
}