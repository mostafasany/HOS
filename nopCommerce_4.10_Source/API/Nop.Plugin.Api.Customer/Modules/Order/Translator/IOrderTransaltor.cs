using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Customer.Modules.Order.Dto.OrderItems;
using Nop.Plugin.Api.Modules.Order.Dto.OrderItems;
using Nop.Plugin.Api.Modules.Order.Dto.Orders;

namespace Nop.Plugin.Api.Modules.Order.Translator
{
    public interface IOrderTransaltor
    {
        OrderDto PrepareOrderDTO(Core.Domain.Orders.Order order);

        OrderItemDto PrepareOrderItemDTO(OrderItem orderItem);
    }
}