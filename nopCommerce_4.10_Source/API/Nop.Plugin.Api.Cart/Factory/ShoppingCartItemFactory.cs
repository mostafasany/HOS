using System;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Common.Factories;

namespace Nop.Plugin.Api.Cart.Factory
{
    public class ShoppingCartItemFactory : IFactory<ShoppingCartItem>
    {
        public ShoppingCartItem Initialize()
        {
            var newShoppingCartItem = new ShoppingCartItem();

            newShoppingCartItem.CreatedOnUtc = DateTime.UtcNow;
            newShoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;

            return newShoppingCartItem;
        }
    }
}