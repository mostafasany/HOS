using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Customer.Modules.Order.Dto.OrderItems;
using Nop.Plugin.Api.Customer.Modules.Order.Dto.Orders;

namespace Nop.Plugin.Api.Customer.Modules.Order.Translator
{
    public interface IOrderTranslator
    {
        OrderDto ToOrderDto(Core.Domain.Orders.Order order);

        OrderItemDto ToOrderItemDto(OrderItem orderItem);
    }
}