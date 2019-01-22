using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Modules.Customer.Dto;
using Nop.Plugin.Api.Modules.Customer.Service;
using Nop.Plugin.Api.Modules.Customer.Translator;
using Nop.Plugin.Api.Modules.Order.Dto.OrderItems;
using Nop.Plugin.Api.Modules.Order.Dto.Orders;
using Nop.Plugin.Api.Modules.ProductAttributes.Service;
using Nop.Services.Security;

namespace Nop.Plugin.Api.Modules.Order.Translator
{
    public class OrderTransaltor : IOrderTransaltor
    {
        private readonly IAclService _aclService;

        private readonly int _currentLangaugeId;
        private readonly ICustomerApiService _customerApiService;
        private readonly IProductAttributeConverter _productAttributeConverter;


        public OrderTransaltor(IAclService aclService,
            ICustomerApiService customerApiService,
            IProductAttributeConverter productAttributeConverter, IHttpContextAccessor httpContextAccessor)
        {
            _aclService = aclService;
            _customerApiService = customerApiService;
            _productAttributeConverter = productAttributeConverter;
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

            CustomerDto customerDto = _customerApiService.GetCustomerById(order.Customer.Id);

            if (customerDto != null) orderDto.Customer = customerDto.ToOrderCustomerDto();

            return orderDto;
        }

        public OrderItemDto PrepareOrderItemDTO(OrderItem orderItem)
        {
            OrderItemDto dto = orderItem.ToDto();
            //dto.Product = PrepareProductDTO(orderItem.Product);
            dto.Attributes = _productAttributeConverter.Parse(orderItem.AttributesXml);
            return dto;
        }
    }
}