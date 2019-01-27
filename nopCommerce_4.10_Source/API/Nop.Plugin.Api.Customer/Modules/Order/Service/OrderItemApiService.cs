using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Common.DataStructures;

namespace Nop.Plugin.Api.Customer.Modules.Order.Service
{
    public class OrderItemApiService : IOrderItemApiService
    {
        public IList<OrderItem> GetOrderItemsForOrder(Core.Domain.Orders.Order order, int limit, int page, int sinceId)
        {
            IQueryable<OrderItem> orderItems = order.OrderItems.AsQueryable();

            return new ApiList<OrderItem>(orderItems, page - 1, limit);
        }

        public int GetOrderItemsCount(Core.Domain.Orders.Order order)
        {
            int orderItemsCount = order.OrderItems.Count();

            return orderItemsCount;
        }
    }
}