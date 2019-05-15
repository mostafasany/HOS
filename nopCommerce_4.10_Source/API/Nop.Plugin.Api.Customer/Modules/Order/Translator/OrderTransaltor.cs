using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Common.Converters;
using Nop.Plugin.Api.Customer.Modules.Order.Dto.OrderItems;
using Nop.Plugin.Api.Customer.Modules.Order.Dto.Orders;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Customer.Modules.Order.Translator
{
    public class OrderTransaltor : IOrderTransaltor
    {
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly ILocalizationService _localizationService;
        private readonly int _currentLangaugeId;

        public OrderTransaltor(ILocalizationService localizationService, IHttpContextAccessor httpContextAccessor,
            IProductAttributeConverter productAttributeConverter)
        {
            _productAttributeConverter = productAttributeConverter;
            _localizationService = localizationService;
            IHeaderDictionary headers = httpContextAccessor.HttpContext.Request.Headers;
            if (headers.ContainsKey("Accept-Language"))
            {
                StringValues lan = headers["Accept-Language"];
                if (lan.ToString() == "en")
                    _currentLangaugeId = 1;
                else
                    _currentLangaugeId = 2;
            }
        }

        public OrderDto PrepareOrderDTO(Core.Domain.Orders.Order order)
        {
            OrderDto orderDto = order.ToDto();
            orderDto.OrderItems = order.OrderItems.Select(PrepareOrderItemDTO).ToList();

            return orderDto;
        }

        public OrderItemDto PrepareOrderItemDTO(OrderItem orderItem)
        {
            OrderItemDto dto = orderItem.ToDto();
            dto.Product.Name = _localizationService.GetLocalized(orderItem.Product, x => x.Name, _currentLangaugeId);
            dto.Attributes = _productAttributeConverter.Parse(orderItem.AttributesXml);
            return dto;
        }
    }
}