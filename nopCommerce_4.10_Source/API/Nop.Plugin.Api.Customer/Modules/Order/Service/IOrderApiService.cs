using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Common.Constants;

namespace Nop.Plugin.Api.Customer.Modules.Order.Service
{
    public interface IOrderApiService
    {
        Core.Domain.Orders.Order GetOrderById(int orderId);

        IList<Core.Domain.Orders.Order> GetOrders(IList<int> ids = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId, OrderStatus? status = null, PaymentStatus? paymentStatus = null,
            ShippingStatus? shippingStatus = null, int? customerId = null, int? storeId = null);

        IList<Core.Domain.Orders.Order> GetOrdersByCustomerId(IList<int> ids = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            OrderStatus? status = null, PaymentStatus? paymentStatus = null, ShippingStatus? shippingStatus = null, int? customerId = null,
            int? storeId = null);

        int GetOrdersCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null, OrderStatus? status = null,
            PaymentStatus? paymentStatus = null, ShippingStatus? shippingStatus = null,
            int? customerId = null, int? storeId = null);
    }
}