using System;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Cart.Dto
{
    public class ExtendedShoppingCartItemsRootObject : ISerializableObject
    {
        public ExtendedShoppingCartItemsRootObject()
        {
            ShoppingCart = new ShoppingCartModel();
        }

        [JsonProperty("shopping_cart")] public ShoppingCartModel ShoppingCart { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "shopping_cart";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ExtendedShoppingCartDto);
        }
    }
}