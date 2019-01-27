using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Common.Converters;
using Nop.Plugin.Api.Customer.Modules.Order.Dto.OrderItems;
using Nop.Plugin.Api.Modules.Order.Dto.OrderItems;
using Nop.Plugin.Api.Modules.Order.Dto.Orders;
using Nop.Plugin.Api.Modules.Order.Translator;

namespace Nop.Plugin.Api.Customer.Modules.Order.Translator
{
    public class OrderTransaltor : IOrderTransaltor
    {
        private readonly IProductAttributeConverter _productAttributeConverter;


        public OrderTransaltor(
            IProductAttributeConverter productAttributeConverter) => _productAttributeConverter = productAttributeConverter;

        public OrderDto PrepareOrderDTO(Core.Domain.Orders.Order order)
        {
            OrderDto orderDto = order.ToDto();

            orderDto.OrderItems = order.OrderItems.Select(PrepareOrderItemDTO).ToList();

            return orderDto;
        }

        public OrderItemDto PrepareOrderItemDTO(OrderItem orderItem)
        {
            OrderItemDto dto = orderItem.ToDto();
            dto.Attributes = _productAttributeConverter.Parse(orderItem.AttributesXml);
            return dto;
        }
    }
}