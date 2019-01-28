﻿using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Modules.Cart.Dto;

namespace Nop.Plugin.Api.Modules.Cart.Service
{
    public interface IShoppingCartItemApiService
    {
        ShoppingCartItem GetShoppingCartItem(int id);

        List<ShoppingCartItem> GetShoppingCartItems(int? customerId = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue);

        ShoppingCartModel PrepareShoppingCartModel(ShoppingCartModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true,
            bool validateCheckoutAttributes = false,
            bool prepareAndDisplayOrderReviewData = false);
    }
}