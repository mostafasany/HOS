using System;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Common.Factories;

namespace Nop.Plugin.Api.Customer.Modules.Order.Factory
{
    public class OrderFactory : IFactory<Core.Domain.Orders.Order>
    {
        public Core.Domain.Orders.Order Initialize()
        {
            var order = new Core.Domain.Orders.Order();

            order.CreatedOnUtc = DateTime.UtcNow;
            order.OrderGuid = new Guid();
            order.PaymentStatus = PaymentStatus.Pending;
            order.ShippingStatus = ShippingStatus.NotYetShipped;
            order.OrderStatus = OrderStatus.Pending;

            return order;
        }
    }
}