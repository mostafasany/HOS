using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Modules.Orders.Dto.OrderItems;

namespace Nop.Plugin.Api.Common.MappingExtensions
{
    public static class OrderItemDtoMappings
    {
        public static OrderItemDto ToDto(this OrderItem orderItem) => orderItem.MapTo<OrderItem, OrderItemDto>();
    }
}