using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Modules.Orders.Dto.Orders;

namespace Nop.Plugin.Api.Common.MappingExtensions
{
    public static class OrderDtoMappings
    {
        public static OrderDto ToDto(this Order order) => order.MapTo<Order, OrderDto>();
    }
}