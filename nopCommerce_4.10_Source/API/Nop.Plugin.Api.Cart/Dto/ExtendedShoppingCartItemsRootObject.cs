using System;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Modules.Cart.Dto
{
    public class ExtendedShoppingCartItemsRootObject : ISerializableObject
    {
        public ExtendedShoppingCartItemsRootObject() => ShoppingCart = new ShoppingCartModel();

        [JsonProperty("shopping_cart")]
        public ShoppingCartModel ShoppingCart { get; set; }

        public string GetPrimaryPropertyName() => "shopping_cart";

        public Type GetPrimaryPropertyType() => typeof(ExtendedShoppingCartDto);
    }
}