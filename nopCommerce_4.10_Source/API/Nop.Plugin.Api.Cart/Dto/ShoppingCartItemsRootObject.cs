using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Cart.Dto
{
    public class ShoppingCartItemsRootObject : ISerializableObject
    {
        public ShoppingCartItemsRootObject() => ShoppingCartItems = new List<ShoppingCartItemDto>();

        [JsonProperty("shopping_carts")]
        public IList<ShoppingCartItemDto> ShoppingCartItems { get; set; }

        public string GetPrimaryPropertyName() => "shopping_carts";

        public Type GetPrimaryPropertyType() => typeof(ShoppingCartItemDto);
    }
}