using System;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.ShoppingCarts
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