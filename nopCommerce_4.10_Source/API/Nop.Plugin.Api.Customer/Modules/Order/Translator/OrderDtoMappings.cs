using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Customer.Modules.Order.Dto.Orders;

namespace Nop.Plugin.Api.Customer.Modules.Order.Translator
{
    public static class OrderDtoMappings
    {
        public static OrderDto ToDto(this Core.Domain.Orders.Order order) => order.MapTo<Core.Domain.Orders.Order, OrderDto>();
    }
}