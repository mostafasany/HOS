﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.Common.Helpers;
using Nop.Plugin.Api.Common.Validators;

namespace Nop.Plugin.Api.Modules.Orders.Dto.OrderItems
{
    public class OrderItemDtoValidator : BaseDtoValidator<OrderItemDto>
    {
        #region Constructors

        public OrderItemDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetProductIdRule();
            SetQuantityRule();
        }

        #endregion

        #region Private Methods

        private void SetProductIdRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(o => o.ProductId, "invalid product_id", "product_id");
        }

        private void SetQuantityRule()
        {
            SetGreaterThanZeroCreateOrUpdateRule(o => o.Quantity, "invalid quanitty", "quantity");
        }

        #endregion
    }
}