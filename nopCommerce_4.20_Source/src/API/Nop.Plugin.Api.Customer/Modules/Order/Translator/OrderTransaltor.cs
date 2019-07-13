using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Common.Converters;
using Nop.Plugin.Api.Customer.Modules.Order.Dto.OrderItems;
using Nop.Plugin.Api.Customer.Modules.Order.Dto.Orders;
using Nop.Services.Localization;
using Nop.Services.Shipping;

namespace Nop.Plugin.Api.Customer.Modules.Order.Translator
{
    public class OrderTransaltor : IOrderTransaltor
    {
        private readonly int _currentLangaugeId;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IShippingService _shippingService;
        private readonly IWorkContext _workContext;
        private readonly IList<ShippingMethod> ShippingsMethods;

        public OrderTransaltor(ILocalizationService localizationService, IHttpContextAccessor httpContextAccessor,
            IProductAttributeConverter productAttributeConverter, IWorkContext workContext,
            IShippingService shippingService)
        {
            _productAttributeConverter = productAttributeConverter;
            _localizationService = localizationService;
            _workContext = workContext;
            _shippingService = shippingService;
            ShippingsMethods = _shippingService.GetAllShippingMethods();
            var headers = httpContextAccessor.HttpContext.Request.Headers;
            if (headers.ContainsKey("Accept-Language"))
            {
                var lan = headers["Accept-Language"];
                if (lan.ToString() == "en")
                    _currentLangaugeId = 1;
                else
                    _currentLangaugeId = 2;
            }
        }

        public OrderDto PrepareOrderDTO(Core.Domain.Orders.Order order)
        {
            var orderDto = order.ToDto();
            orderDto.OrderItems = order.OrderItems.Select(PrepareOrderItemDTO).ToList();
            try
            {
                var orderShippingMethod = ShippingsMethods.FirstOrDefault(a => a.Name == order.ShippingMethod);
                if (orderShippingMethod != null)
                {
                    var splittedDaysRange = orderShippingMethod.Description.Split(':');
                    var startDay = splittedDaysRange.First();
                    var endtDay = splittedDaysRange.Last();
                    orderDto.ExpectedDeliveryDateFrom = orderDto.CreatedOnUtc.Value.AddDays(int.Parse(startDay));
                    orderDto.ExpectedDeliveryDateTo = orderDto.CreatedOnUtc.Value.AddDays(int.Parse(endtDay));
                }
            }
            catch (Exception)
            {
            }

            return orderDto;
        }

        public OrderItemDto PrepareOrderItemDTO(OrderItem orderItem)
        {
            var dto = orderItem.ToDto();
            dto.Product.Name = _localizationService.GetLocalized(orderItem.Product, x => x.Name, _currentLangaugeId);
            dto.Attributes = _productAttributeConverter.Parse(orderItem.AttributesXml);
            return dto;
        }
    }
}