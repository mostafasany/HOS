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
            var order = new Core.Domain.Orders.Order
            {
                CreatedOnUtc = DateTime.UtcNow,
                OrderGuid = new Guid(),
                PaymentStatus = PaymentStatus.Pending,
                ShippingStatus = ShippingStatus.NotYetShipped,
                OrderStatus = OrderStatus.Pending
            };


            return order;
        }
    }
}