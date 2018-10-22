using System;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.ShoppingCarts
{
    public class ExtendedShoppingCartItemsRootObject : ISerializableObject
    {
        public ExtendedShoppingCartItemsRootObject()
        {
            ShoppingCart = new ExtendedShoppingCartDto();
        }

        [JsonProperty("shopping_cart")]
        public ExtendedShoppingCartDto ShoppingCart { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "shopping_cart";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof (ExtendedShoppingCartDto);
        }
    }
}