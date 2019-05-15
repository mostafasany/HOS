using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Cart.Service;
using Nop.Plugin.Api.Common.Attributes;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.Controllers;
using Nop.Plugin.Api.Common.Converters;
using Nop.Plugin.Api.Common.Delta;
using Nop.Plugin.Api.Common.DTOs.Errors;
using Nop.Plugin.Api.Common.Factories;
using Nop.Plugin.Api.Common.JSON.ActionResults;
using Nop.Plugin.Api.Common.JSON.Serializers;
using Nop.Plugin.Api.Common.ModelBinders;
using Nop.Plugin.Api.Customer.Modules.Order.Dto.OrderItems;
using Nop.Plugin.Api.Customer.Modules.Order.Dto.Orders;
using Nop.Plugin.Api.Customer.Modules.Order.Model;
using Nop.Plugin.Api.Customer.Modules.Order.Service;
using Nop.Plugin.Api.Customer.Modules.Order.Translator;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Modules
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderTransaltor _dtoHelper;
        private readonly IFactory<Order> _factory;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IOrderApiService _orderApiService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IProductService _productService;
        private readonly IShippingService _shippingService;
        private readonly IWorkContext _workContext;
        private readonly IShoppingCartItemApiService _shoppingCartItemApiService;
        private readonly IShoppingCartService _shoppingCartService;

        private readonly IStoreContext _storeContext;

        // We resolve the order settings this way because of the tests.
        // The auto mocking does not support concreate types as dependencies. It supports only interfaces.
        private OrderSettings _orderSettings;

        public OrdersController(IOrderApiService orderApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IProductService productService,
            IFactory<Order> factory,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IShoppingCartService shoppingCartService,
            IGenericAttributeService genericAttributeService,
            IStoreContext storeContext,
            IShippingService shippingService,
            IPictureService pictureService,
            IOrderTransaltor dtoHelper,
             IWorkContext workContext,
            IProductAttributeConverter productAttributeConverter,
            IShoppingCartItemApiService shoppingCartItemApiService)
            : base(jsonFieldsSerializer, aclService, customerService, storeMappingService,
                storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _orderApiService = orderApiService;
            _factory = factory;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _shoppingCartService = shoppingCartService;
            _genericAttributeService = genericAttributeService;
            _storeContext = storeContext;
            _shippingService = shippingService;
            _dtoHelper = dtoHelper;
            _productService = productService;
            _productAttributeConverter = productAttributeConverter;
            _shoppingCartItemApiService = shoppingCartItemApiService;
            _workContext = workContext;
        }

        private OrderSettings OrderSettings => _orderSettings ?? (_orderSettings = EngineContext.Current.Resolve<OrderSettings>());

        [HttpPost]
        [Route("/api/orders")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult CreateOrder([ModelBinder(typeof(JsonModelBinder<OrderDto>))]
            Delta<OrderDto> orderDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid) return Error();

            //if (orderDelta.Dto.CustomerId == null) return Error();

            // We doesn't have to check for value because this is done by the order validator.
            //Core.Domain.Customers.Customer customer = CustomerService.GetCustomerById(orderDelta.Dto.CustomerId.Value);
            Core.Domain.Customers.Customer customer = _workContext.CurrentCustomer;

            if (customer == null) return Error(HttpStatusCode.NotFound, "customer", "not found");


            if (orderDelta.Dto.OrderItems != null)
            {
                bool shouldReturnError = AddOrderItemsToCart(orderDelta.Dto.OrderItems, customer, orderDelta.Dto.StoreId ?? _storeContext.CurrentStore.Id);
                if (shouldReturnError) return Error(HttpStatusCode.BadRequest);

                //  shippingRequired = IsShippingAddressRequired(orderDelta.Dto.OrderItems);
            }

            List<ShoppingCartItem> shoppingCartItems = _shoppingCartItemApiService.GetShoppingCartItems(customer.Id).Where(a => a.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();

            //  var orderItems = _orderApiService.GetOrdersByCustomerId(customer.Id).SelectMany(x => x.OrderItems).ToList();
            bool shippingRequired = shoppingCartItems.Any(a => a.Product.IsShipEnabled);

            if (shippingRequired)
            {
                var isValid = true;
                // var orderItemsDto = orderItems.Select(a => _dtoHelper.PrepareOrderItemDTO(a));
                isValid &= SetShippingOption(orderDelta.Dto.ShippingRateComputationMethodSystemName,
                    orderDelta.Dto.ShippingMethod,
                    orderDelta.Dto.StoreId ?? _storeContext.CurrentStore.Id,
                    customer, shoppingCartItems);

                if (!isValid) return Error(HttpStatusCode.BadRequest);
            }

            Order newOrder = _factory.Initialize();
            orderDelta.Merge(newOrder);

            customer.BillingAddress = newOrder.BillingAddress;
            customer.ShippingAddress = newOrder.ShippingAddress;

            // If the customer has something in the cart it will be added too. Should we clear the cart first? 
            newOrder.Customer = customer;

            // The default value will be the currentStore.id, but if it isn't passed in the json we need to set it by hand.
            if (!orderDelta.Dto.StoreId.HasValue) newOrder.StoreId = _storeContext.CurrentStore.Id;

            PlaceOrderResult placeOrderResult = PlaceOrder(newOrder, customer);
            if (placeOrderResult != null && placeOrderResult.Success && !string.IsNullOrEmpty(orderDelta.Dto.OrderNotes))
                placeOrderResult.PlacedOrder.OrderNotes?.Add(new OrderNote
                {
                    Note = orderDelta.Dto.OrderNotes,
                    CreatedOnUtc = DateTime.Now
                });

            if (!placeOrderResult.Success)
            {
                foreach (string error in placeOrderResult.Errors) ModelState.AddModelError("order placement", error);

                return Error(HttpStatusCode.BadRequest);
            }

            CustomerActivityService.InsertActivity("AddNewOrder",
                LocalizationService.GetResource("ActivityLog.AddNewOrder"), newOrder);

            var ordersRootObject = new OrdersRootObject();

            OrderDto placedOrderDto = _dtoHelper.PrepareOrderDTO(placeOrderResult.PlacedOrder);

            ordersRootObject.Orders.Add(placedOrderDto);

            string json = JsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/orders/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult DeleteOrder(int id)
        {
            if (id <= 0) return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            Order orderToDelete = _orderApiService.GetOrderById(id);

            if (orderToDelete == null) return Error(HttpStatusCode.NotFound, "order", "not found");

            _orderProcessingService.DeleteOrder(orderToDelete);

            //activity log
            CustomerActivityService.InsertActivity("DeleteOrder", LocalizationService.GetResource("ActivityLog.DeleteOrder"), orderToDelete);

            return new RawJsonActionResult("{}");
        }

        /// <summary>
        ///     Retrieve order by spcified id
        /// </summary>
        /// ///
        /// <param name="id">Id of the order</param>
        /// <param name="fields">Fields from the order you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders/{id}")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrderById(int id, string fields = "")
        {
            if (id <= 0) return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            Order order = _orderApiService.GetOrderById(id);

            if (order == null) return Error(HttpStatusCode.NotFound, "order", "not found");

            var ordersRootObject = new OrdersRootObject();

            OrderDto orderDto = _dtoHelper.PrepareOrderDTO(order);
            ordersRootObject.Orders.Add(orderDto);

            string json = JsonFieldsSerializer.Serialize(ordersRootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a list of all Orders
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrders(OrdersParametersModel parameters)
        {
            if (parameters.Page < Configurations.DefaultPageValue) return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");

            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit) return Error(HttpStatusCode.BadRequest, "page", "Invalid limit parameter");

            int storeId = _storeContext.CurrentStore.Id;

            IList<Order> orders = _orderApiService.GetOrders(parameters.Ids, parameters.CreatedAtMin,
                parameters.CreatedAtMax,
                parameters.Limit, parameters.Page, parameters.SinceId,
                parameters.Status, parameters.PaymentStatus, parameters.ShippingStatus,
                parameters.CustomerId, storeId);

            IList<OrderDto> ordersAsDtos = orders.Select(x => _dtoHelper.PrepareOrderDTO(x)).ToList();

            var ordersRootObject = new OrdersRootObject
            {
                Orders = ordersAsDtos
            };

            string json = JsonFieldsSerializer.Serialize(ordersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Retrieve all orders for customer
        /// </summary>
        /// <param name="customerId">Id of the customer whoes orders you want to get</param>
        /// <param name="parameters">Id of the customer whoes orders you want to get</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders/customer/{customerId}")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrdersByCustomerId(int customerId, OrdersParametersModel parameters)
        {
            IList<Order> ordersForCustomer = _orderApiService.GetOrdersByCustomerId(parameters.Ids, parameters.CreatedAtMin,
                parameters.CreatedAtMax,
                parameters.Limit, parameters.Page, parameters.SinceId,
                parameters.Status, parameters.PaymentStatus, parameters.ShippingStatus,
                customerId, null);
            IList<OrderDto> ordersAsDtos = ordersForCustomer.Select(x => _dtoHelper.PrepareOrderDTO(x)).ToList();

            var ordersRootObject = new OrdersRootObject
            {
                Orders = ordersAsDtos
            };

            return Ok(ordersRootObject);
        }

        /// <summary>
        ///     Receive a count of all Orders
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/orders/count")]
        [ProducesResponseType(typeof(OrdersCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrdersCount(OrdersCountParametersModel parameters)
        {
            int storeId = _storeContext.CurrentStore.Id;

            int ordersCount = _orderApiService.GetOrdersCount(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Status,
                parameters.PaymentStatus, parameters.ShippingStatus, parameters.CustomerId, storeId);

            var ordersCountRootObject = new OrdersCountRootObject
            {
                Count = ordersCount
            };

            return Ok(ordersCountRootObject);
        }

        [HttpPut]
        [Route("/api/orders/{id}")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult UpdateOrder([ModelBinder(typeof(JsonModelBinder<OrderDto>))]
            Delta<OrderDto> orderDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid) return Error();

            Order currentOrder = _orderApiService.GetOrderById(orderDelta.Dto.Id);

            if (currentOrder == null) return Error(HttpStatusCode.NotFound, "order", "not found");

            Core.Domain.Customers.Customer customer = currentOrder.Customer;

            bool shippingRequired = currentOrder.OrderItems.Any(item => !item.Product.IsFreeShipping);

            if (shippingRequired)
            {
                var isValid = true;

                if (!string.IsNullOrEmpty(orderDelta.Dto.ShippingRateComputationMethodSystemName) ||
                    !string.IsNullOrEmpty(orderDelta.Dto.ShippingMethod))
                {
                    int storeId = orderDelta.Dto.StoreId ?? _storeContext.CurrentStore.Id;

                    isValid &= SetShippingOption(orderDelta.Dto.ShippingRateComputationMethodSystemName ?? currentOrder.ShippingRateComputationMethodSystemName,
                        orderDelta.Dto.ShippingMethod,
                        storeId,
                        customer, BuildShoppingCartItemsFromOrderItems(currentOrder.OrderItems.ToList(), customer.Id, storeId));
                }

                if (isValid)
                    currentOrder.ShippingMethod = orderDelta.Dto.ShippingMethod;
                else
                    return Error(HttpStatusCode.BadRequest);
            }

            orderDelta.Merge(currentOrder);

            customer.BillingAddress = currentOrder.BillingAddress;
            customer.ShippingAddress = currentOrder.ShippingAddress;

            _orderService.UpdateOrder(currentOrder);

            CustomerActivityService.InsertActivity("UpdateOrder",
                LocalizationService.GetResource("ActivityLog.UpdateOrder"), currentOrder);

            var ordersRootObject = new OrdersRootObject();

            OrderDto placedOrderDto = _dtoHelper.PrepareOrderDTO(currentOrder);
            placedOrderDto.ShippingMethod = orderDelta.Dto.ShippingMethod;

            ordersRootObject.Orders.Add(placedOrderDto);

            string json = JsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        private bool AddOrderItemsToCart(ICollection<OrderItemDto> orderItems, Core.Domain.Customers.Customer customer, int storeId)
        {
            var shouldReturnError = false;

            foreach (OrderItemDto orderItem in orderItems)
                if (orderItem.ProductId != null)
                {
                    Core.Domain.Catalog.Product product = _productService.GetProductById(orderItem.ProductId.Value);

                    if (!product.IsRental)
                    {
                        orderItem.RentalStartDateUtc = null;
                        orderItem.RentalEndDateUtc = null;
                    }

                    string attributesXml = _productAttributeConverter.ConvertToXml(orderItem.Attributes.ToList(), product.Id);

                    IList<string> errors = _shoppingCartService.AddToCart(customer, product,
                        ShoppingCartType.ShoppingCart, storeId, attributesXml,
                        0M, orderItem.RentalStartDateUtc, orderItem.RentalEndDateUtc,
                        orderItem.Quantity ?? 1);

                    if (errors.Count > 0)
                    {
                        foreach (string error in errors) ModelState.AddModelError("order", error);

                        shouldReturnError = true;
                    }
                }

            return shouldReturnError;
        }

        private List<ShoppingCartItem> BuildShoppingCartItemsFromOrderItemDtos(List<OrderItemDto> orderItemDtos, int customerId, int storeId)
        {
            var shoppingCartItems = new List<ShoppingCartItem>();

            foreach (OrderItemDto orderItem in orderItemDtos)
                if (orderItem.ProductId != null)
                    shoppingCartItems.Add(new ShoppingCartItem
                    {
                        ProductId = orderItem.ProductId.Value, // required field
                        CustomerId = customerId,
                        Quantity = orderItem.Quantity ?? 1,
                        RentalStartDateUtc = orderItem.RentalStartDateUtc,
                        RentalEndDateUtc = orderItem.RentalEndDateUtc,
                        StoreId = storeId,
                        Product = _productService.GetProductById(orderItem.ProductId.Value),
                        ShoppingCartType = ShoppingCartType.ShoppingCart
                    });

            return shoppingCartItems;
        }

        private List<ShoppingCartItem> BuildShoppingCartItemsFromOrderItems(List<OrderItem> orderItems, int customerId, int storeId)
        {
            var shoppingCartItems = new List<ShoppingCartItem>();

            foreach (OrderItem orderItem in orderItems)
                shoppingCartItems.Add(new ShoppingCartItem
                {
                    ProductId = orderItem.ProductId,
                    CustomerId = customerId,
                    Quantity = orderItem.Quantity,
                    RentalStartDateUtc = orderItem.RentalStartDateUtc,
                    RentalEndDateUtc = orderItem.RentalEndDateUtc,
                    StoreId = storeId,
                    Product = orderItem.Product,
                    ShoppingCartType = ShoppingCartType.ShoppingCart
                });

            return shoppingCartItems;
        }

        private bool IsShippingAddressRequired(ICollection<OrderItemDto> orderItems)
        {
            var shippingAddressRequired = false;

            foreach (OrderItemDto orderItem in orderItems)
                if (orderItem.ProductId != null)
                {
                    Core.Domain.Catalog.Product product = _productService.GetProductById(orderItem.ProductId.Value);

                    shippingAddressRequired |= product.IsShipEnabled;
                }

            return shippingAddressRequired;
        }

        private PlaceOrderResult PlaceOrder(Order newOrder, Core.Domain.Customers.Customer customer)
        {
            var processPaymentRequest = new ProcessPaymentRequest
            {
                StoreId = newOrder.StoreId,
                CustomerId = customer.Id,
                PaymentMethodSystemName = newOrder.PaymentMethodSystemName
            };


            PlaceOrderResult placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest);

            return placeOrderResult;
        }

        private bool SetShippingOption(string shippingRateComputationMethodSystemName, string shippingOptionName, int storeId, Core.Domain.Customers.Customer customer, List<ShoppingCartItem> shoppingCartItems)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(shippingRateComputationMethodSystemName))
            {
                isValid = false;

                ModelState.AddModelError("shipping_rate_computation_method_system_name",
                    "Please provide shipping_rate_computation_method_system_name");
            }
            else if (string.IsNullOrEmpty(shippingOptionName))
            {
                isValid = false;

                ModelState.AddModelError("shipping_option_name", "Please provide shipping_option_name");
            }
            else
            {
                GetShippingOptionResponse shippingOptionResponse = _shippingService.GetShippingOptions(shoppingCartItems, customer.ShippingAddress, customer,
                    shippingRateComputationMethodSystemName, storeId);

                if (shippingOptionResponse.Success)
                {
                    List<ShippingOption> shippingOptions = shippingOptionResponse.ShippingOptions.ToList();

                    ShippingOption shippingOption = shippingOptions
                        .Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(shippingOptionName, StringComparison.InvariantCultureIgnoreCase));

                    _genericAttributeService.SaveAttribute(customer,
                        NopCustomerDefaults.SelectedShippingOptionAttribute,
                        shippingOption, storeId);
                }
                else
                {
                    isValid = false;

                    foreach (string errorMessage in shippingOptionResponse.Errors) ModelState.AddModelError("shipping_option", errorMessage);
                }
            }

            return isValid;
        }
    }
}