using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DTOs.ShoppingCarts;

namespace Nop.Plugin.Api.Services
{
    public interface IShoppingCartItemApiService
    {
        List<ShoppingCartItem> GetShoppingCartItems(int? customerId = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
                                                    DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, int limit = Configurations.DefaultLimit, 
                                                    int page = Configurations.DefaultPageValue);

        ShoppingCartItem GetShoppingCartItem(int id);

        ShoppingCartModel PrepareShoppingCartModel(ShoppingCartModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true,
            bool validateCheckoutAttributes = false,
            bool prepareAndDisplayOrderReviewData = false);
    }
}