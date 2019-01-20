using System.Collections.Generic;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Api.Modules.Orders.Service
{
    public interface IOrderItemApiService
    {
        int GetOrderItemsCount(Order order);
        IList<OrderItem> GetOrderItemsForOrder(Order order, int limit, int page, int sinceId);
    }
}