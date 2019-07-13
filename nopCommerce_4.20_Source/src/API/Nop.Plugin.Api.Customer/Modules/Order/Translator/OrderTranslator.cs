using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Common.Converters;
using Nop.Plugin.Api.Customer.Modules.Order.Dto.OrderItems;
using Nop.Plugin.Api.Customer.Modules.Order.Dto.Orders;
using Nop.Services.Localization;
using Nop.Services.Shipping;

namespace Nop.Plugin.Api.Customer.Modules.Order.Translator
{
    public class OrderTranslator : IOrderTranslator
    {
        private readonly int _currentLanguageId;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IList<ShippingMethod> _shippingMethods;

        public OrderTranslator(ILocalizationService localizationService, IHttpContextAccessor httpContextAccessor,
            IProductAttributeConverter productAttributeConverter,
            IShippingService shippingService)
        {
            _productAttributeConverter = productAttributeConverter;
            _localizationService = localizationService;
            _shippingMethods = shippingService.GetAllShippingMethods();
            var headers = httpContextAccessor.HttpContext.Request.Headers;
            if (!headers.ContainsKey("Accept-Language")) return;
            var lan = headers["Accept-Language"];
            _currentLanguageId = lan.ToString() == "en" ? 1 : 2;
        }

        public OrderDto ToOrderDto(Core.Domain.Orders.Order order)
        {
            var orderDto = order.ToDto();
            orderDto.OrderItems = order.OrderItems.Select(ToOrderItemDto).ToList();
            try
            {
                var orderShippingMethod = _shippingMethods.FirstOrDefault(a => a.Name == order.ShippingMethod);
                if (orderShippingMethod != null)
                {
                    var dayRange = orderShippingMethod.Description.Split(':');
                    var startDay = dayRange.First();
                    var endDay = dayRange.Last();
                    if (orderDto.CreatedOnUtc != null)
                    {
                        orderDto.ExpectedDeliveryDateFrom = orderDto.CreatedOnUtc.Value.AddDays(int.Parse(startDay));
                        orderDto.ExpectedDeliveryDateTo = orderDto.CreatedOnUtc.Value.AddDays(int.Parse(endDay));
                    }
                }
            }
            catch (Exception)
            {
            }

            return orderDto;
        }

        public OrderItemDto ToOrderItemDto(OrderItem orderItem)
        {
            var dto = orderItem.ToDto();
            dto.Product.Name = _localizationService.GetLocalized(orderItem.Product, x => x.Name, _currentLanguageId);
            dto.Attributes = _productAttributeConverter.Parse(orderItem.AttributesXml);
            return dto;
        }
    }
}