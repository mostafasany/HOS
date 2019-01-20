using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Common.DataStructures;

namespace Nop.Plugin.Api.Modules.Orders.Service
{
    public class OrderItemApiService : IOrderItemApiService
    {
        public IList<OrderItem> GetOrderItemsForOrder(Order order, int limit, int page, int sinceId)
        {
            IQueryable<OrderItem> orderItems = order.OrderItems.AsQueryable();

            return new ApiList<OrderItem>(orderItems, page - 1, limit);
        }

        public int GetOrderItemsCount(Order order)
        {
            int orderItemsCount = order.OrderItems.Count();

            return orderItemsCount;
        }
    }
}