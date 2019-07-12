using System.Collections.Generic;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Api.Customer.Modules.Order.Service
{
    public interface IOrderItemApiService
    {
        int GetOrderItemsCount(Core.Domain.Orders.Order order);
        IList<OrderItem> GetOrderItemsForOrder(Core.Domain.Orders.Order order, int limit, int page, int sinceId);
    }
}