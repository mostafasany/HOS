using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Cart.Dto;
using Nop.Plugin.Api.Cart.Model;
using Nop.Plugin.Api.Cart.Service;
using Nop.Plugin.Api.Cart.Translator;
using Nop.Plugin.Api.Common.Attributes;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.Controllers;
using Nop.Plugin.Api.Common.Converters;
using Nop.Plugin.Api.Common.Delta;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Common.DTOs.Errors;
using Nop.Plugin.Api.Common.DTOs.Product;
using Nop.Plugin.Api.Common.JSON.ActionResults;
using Nop.Plugin.Api.Common.JSON.Serializers;
using Nop.Plugin.Api.Common.ModelBinders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Cart
{
    public class ShoppingCartItemsController : BaseApiController
    {
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IDiscountTranslator _discountTranslator;
        private readonly ICartTranslator _dtoHelper;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IProductService _productService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartItemApiService _shoppingCartItemApiService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        public ShoppingCartItemsController(IShoppingCartItemApiService shoppingCartItemApiService,
            IShippingService shippingService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IShoppingCartService shoppingCartService,
            IProductService productService,
            IPictureService pictureService,
            IProductAttributeConverter productAttributeConverter,
            ICartTranslator dtoHelper,
            IStoreContext storeContext,
            IDiscountTranslator discountTranslator,
            IWorkContext workContext)
            : base(jsonFieldsSerializer,
                aclService,
                customerService,
                storeMappingService,
                storeService,
                discountService,
                customerActivityService,
                localizationService,
                pictureService)
        {
            _shoppingCartItemApiService = shoppingCartItemApiService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _productAttributeConverter = productAttributeConverter;
            _dtoHelper = dtoHelper;
            _storeContext = storeContext;
            _shippingService = shippingService;
            _workContext = workContext;
            _customerService = customerService;
            _discountService = discountService;
            _discountTranslator = discountTranslator;
        }

        [HttpPost]
        [Route("/api/shopping_cart_items")]
        [ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), 422)]
        public IActionResult CreateShoppingCartItem([ModelBinder(typeof(JsonModelBinder<ShoppingCartItemDto>))]
            Delta<ShoppingCartItemDto> shoppingCartItemDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid) return Error();

            var newShoppingCartItem =
                new ShoppingCartItem {CreatedOnUtc = DateTime.UtcNow, UpdatedOnUtc = DateTime.UtcNow};
            shoppingCartItemDelta.Merge(newShoppingCartItem);

            // We know that the product id and customer id will be provided because they are required by the validator.
            // TODO: validate
            var product = _productService.GetProductById(newShoppingCartItem.ProductId);

            if (product == null) return Error(HttpStatusCode.NotFound, "product", "not found");

            var customer =
                _workContext.CurrentCustomer; // CustomerService.GetCustomerById(newShoppingCartItem.CustomerId);

            if (customer == null) return Error(HttpStatusCode.NotFound, "customer", "not found");

            var shoppingCartType =
                (ShoppingCartType)Enum.Parse(typeof(ShoppingCartType), shoppingCartItemDelta.Dto.ShoppingCartType);

            if (!product.IsRental)
            {
                newShoppingCartItem.RentalStartDateUtc = null;
                newShoppingCartItem.RentalEndDateUtc = null;
            }

            var attributesXml =
                _productAttributeConverter.ConvertToXml(shoppingCartItemDelta.Dto.Attributes, product.Id);

            var currentStoreId = _storeContext.CurrentStore.Id;

            var warnings = _shoppingCartService.AddToCart(customer, product, shoppingCartType, currentStoreId,
                attributesXml, 0M,
                newShoppingCartItem.RentalStartDateUtc, newShoppingCartItem.RentalEndDateUtc,
                shoppingCartItemDelta.Dto.Quantity ?? 1);

            if (warnings.Count > 0)
            {
                foreach (var warning in warnings) ModelState.AddModelError("shopping cart item", warning);

                return Error(HttpStatusCode.BadRequest);
            }

            // the newly added shopping cart item should be the last one
            newShoppingCartItem = customer.ShoppingCartItems.LastOrDefault();

            // Preparing the result dto of the new product category mapping
            var newShoppingCartItemDto = _dtoHelper.PrepareShoppingCartItemDto(newShoppingCartItem);

            var shoppingCartsRootObject = new ShoppingCartItemsRootObject();

            shoppingCartsRootObject.ShoppingCartItems.Add(newShoppingCartItemDto);

            var json = JsonFieldsSerializer.Serialize(shoppingCartsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/shopping_cart_items/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult DeleteShoppingCartItem(int id)
        {
            if (id <= 0) return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            var shoppingCartItemForDelete = _shoppingCartItemApiService.GetShoppingCartItem(id);

            if (shoppingCartItemForDelete == null)
                return Error(HttpStatusCode.NotFound, "shopping_cart_item", "not found");

            _shoppingCartService.DeleteShoppingCartItem(shoppingCartItemForDelete);

            //activity log
            CustomerActivityService.InsertActivity("DeleteShoppingCartItem",
                LocalizationService.GetResource("ActivityLog.DeleteShoppingCartItem"), shoppingCartItemForDelete);

            return new OkResult();
        }


        /// <summary>
        ///     Receive a list of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/shopping_cart_items/crosssells")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetCorssSellsProducts(ShoppingCartItemsForCrossSellsParametersModel parameters)
        {
            var allProducts = _productService.GetCrosssellProductsByShoppingCart(
                    parameters.ProductIds?.Select(a => new ShoppingCartItem {ProductId = a}).ToList(), parameters.Limit)
                .Where(p => StoreMappingService.Authorize(p));

            IList<ProductDto> productsAsDtos =
                allProducts.Select(product => product.ToDto()).ToList();

            var productsRootObject = new ProductsRootObjectDto {Products = productsAsDtos};

            var json = JsonFieldsSerializer.Serialize(productsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }


        /// <summary>
        ///     Receive a list of all shopping cart items
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/shopping_cart_items/cart")]
        [ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetShoppingCartItemsByCookie(ShoppingCartItemsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");

            IList<ShoppingCartItem> shoppingCartItems = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(a => a.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();

            shoppingCartItems = shoppingCartItems.Where(a => a.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .ToList();
            var model = new ShoppingCartModel();
            _shoppingCartItemApiService.PrepareShoppingCartModel(model, shoppingCartItems);
            model.SubTotal = model.Items.Sum(a => a.SubTotalNumber);
            model.SubTotalDiscount = model.Items.Sum(a => a.DiscountlNumber);
            model.Total = model.SubTotal - model.SubTotalDiscount;
            var shoppingCartsRootObject = new ExtendedShoppingCartItemsRootObject {ShoppingCart = model};

            var json = JsonFieldsSerializer.Serialize(shoppingCartsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a list of all shopping cart items
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/shopping_cart_items/{customerId}/cart")]
        [ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOnlyShoppingCartItems(int customerId, ShoppingCartItemsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");

            IList<ShoppingCartItem> shoppingCartItems = _shoppingCartItemApiService.GetShoppingCartItems(customerId,
                parameters.CreatedAtMin,
                parameters.CreatedAtMax,
                parameters.UpdatedAtMin,
                parameters.UpdatedAtMax,
                parameters.Limit,
                parameters.Page);

            shoppingCartItems = shoppingCartItems.Where(a => a.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .ToList();
            var model = new ShoppingCartModel();
            _shoppingCartItemApiService.PrepareShoppingCartModel(model, shoppingCartItems);
            model.SubTotal = model.Items.Sum(a => a.SubTotalNumber);
            model.SubTotalDiscount = model.Items.Sum(a => a.DiscountlNumber);
            model.Total = model.SubTotal - model.SubTotalDiscount;
            var shoppingCartsRootObject = new ExtendedShoppingCartItemsRootObject {ShoppingCart = model};

            var json = JsonFieldsSerializer.Serialize(shoppingCartsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }


        [HttpGet]
        [Route("/api/shopping_cart_items/shipping/estimation")]
        [ProducesResponseType(typeof(ShippingOptionRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetShippingEstimation(ShoppingCartItemsShipmentEstimationModel parameters)
        {
            if (parameters.CountryId <= 0)
                return Error(HttpStatusCode.BadRequest, "country", "invalid country parameter");

            if (parameters.StateProvinceId <= 0)
                return Error(HttpStatusCode.BadRequest, "province", "invalid province parameter");

            IList<ShoppingCartItem> shoppingCartItems = _shoppingCartItemApiService.GetShoppingCartItems();

            //TODO:4.2 Migration
            //var shippingRateComputationMethods = _shippingService.LoadActiveShippingRateComputationMethods().FirstOrDefault();

            //IShippingRateComputationMethod n = _shippingService.LoadShippingRateComputationMethodBySystemName(shippingRateComputationMethods?.PluginDescriptor?.SystemName);
            //var optionsRequest = new GetShippingOptionRequest
            //{
            //    Customer = _workContext.CurrentCustomer,
            //    ShippingAddress = new Address
            //    {
            //        Address1 = "-",
            //        City = "-",
            //        ZipPostalCode = parameters.ZipPostalCode,
            //        CountryId = parameters.CountryId,
            //        StateProvinceId = parameters.StateProvinceId
            //    },
            //};
            var shippingAddress = new Address
            {
                Address1 = "-",
                City = "-",
                ZipPostalCode = parameters.ZipPostalCode,
                CountryId = parameters.CountryId,
                StateProvinceId = parameters.StateProvinceId
            };
            var packageItems = shoppingCartItems
                .Select(shoppingCart => new GetShippingOptionRequest.PackageItem(shoppingCart)).ToList();

            // optionsRequest.Items = items;

            var shippingOptionResponse = _shippingService.GetShippingOptions(shoppingCartItems, shippingAddress,
                _workContext.CurrentCustomer, storeId: _storeContext.CurrentStore.Id);
            //  GetShippingOptionResponse shippingOptionResponse = n.GetShippingOptions(optionsRequest);

            var shippingOptionDto = shippingOptionResponse.ShippingOptions
                .Select(shoppingCartItem => _dtoHelper.PrepareShippingOptionItemDto(shoppingCartItem)).ToList();

            var shippingOptionRootObject = new ShippingOptionRootObject {ShippingOptions = shippingOptionDto};

            var json = JsonFieldsSerializer.Serialize(shippingOptionRootObject, "");

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a list of all shopping cart items
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/shopping_cart_items")]
        [ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetShoppingCartItems(ShoppingCartItemsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");

            IList<ShoppingCartItem> shoppingCartItems = _shoppingCartItemApiService.GetShoppingCartItems(null,
                parameters.CreatedAtMin,
                parameters.CreatedAtMax,
                parameters.UpdatedAtMin,
                parameters.UpdatedAtMax,
                parameters.Limit,
                parameters.Page);

            var shoppingCartItemsDto = shoppingCartItems
                .Select(shoppingCartItem => _dtoHelper.PrepareShoppingCartItemDto(shoppingCartItem)).ToList();

            var shoppingCartsRootObject = new ShoppingCartItemsRootObject {ShoppingCartItems = shoppingCartItemsDto};

            var json = JsonFieldsSerializer.Serialize(shoppingCartsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a list of all shopping cart items by customer id
        /// </summary>
        /// <param name="customerId">Id of the customer whoes shopping cart items you want to get</param>
        /// <param name="parameters"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/shopping_cart_items/{customerId}")]
        [ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetShoppingCartItemsByCustomerId(int customerId,
            ShoppingCartItemsForCustomerParametersModel parameters)
        {
            if (customerId <= Configurations.DefaultCustomerId)
                return Error(HttpStatusCode.BadRequest, "customer_id", "invalid customer_id");

            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");

            IList<ShoppingCartItem> shoppingCartItems = _shoppingCartItemApiService.GetShoppingCartItems(customerId,
                parameters.CreatedAtMin,
                parameters.CreatedAtMax, parameters.UpdatedAtMin,
                parameters.UpdatedAtMax, parameters.Limit,
                parameters.Page);

            if (shoppingCartItems == null) return Error(HttpStatusCode.NotFound, "shopping_cart_item", "not found");

            var shoppingCartItemsDtos = shoppingCartItems
                .Select(shoppingCartItem => _dtoHelper.PrepareShoppingCartItemDto(shoppingCartItem))
                .ToList();

            var shoppingCartsRootObject = new ShoppingCartItemsRootObject {ShoppingCartItems = shoppingCartItemsDtos};

            var json = JsonFieldsSerializer.Serialize(shoppingCartsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }


        /// <summary>
        ///     Receive a list of all shopping cart items
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/shopping_cart_items/{customerId}/wishlist")]
        [ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetWishlistItems(int customerId, ShoppingCartItemsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");

            IList<ShoppingCartItem> shoppingCartItems = _shoppingCartItemApiService.GetShoppingCartItems(customerId,
                parameters.CreatedAtMin,
                parameters.CreatedAtMax,
                parameters.UpdatedAtMin,
                parameters.UpdatedAtMax,
                parameters.Limit,
                parameters.Page);

            shoppingCartItems = shoppingCartItems.Where(a => a.ShoppingCartType == ShoppingCartType.Wishlist).ToList();

            var shoppingCartItemsDtos = shoppingCartItems.Select(shoppingCartItem =>
            {
                return _dtoHelper.PrepareShoppingCartItemDto(shoppingCartItem);
            }).ToList();

            var shoppingCartsRootObject = new ShoppingCartItemsRootObject {ShoppingCartItems = shoppingCartItemsDtos};

            var json = JsonFieldsSerializer.Serialize(shoppingCartsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/shopping_cart_items/{id}")]
        [ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult UpdateShoppingCartItem([ModelBinder(typeof(JsonModelBinder<ShoppingCartItemDto>))]
            Delta<ShoppingCartItemDto> shoppingCartItemDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid) return Error();

            // We kno that the id will be valid integer because the validation for this happens in the validator which is executed by the model binder.
            var shoppingCartItemForUpdate =
                _shoppingCartItemApiService.GetShoppingCartItem(shoppingCartItemDelta.Dto.Id);

            if (shoppingCartItemForUpdate == null)
                return Error(HttpStatusCode.NotFound, "shopping_cart_item", "not found");

            shoppingCartItemDelta.Merge(shoppingCartItemForUpdate);

            if (!shoppingCartItemForUpdate.Product.IsRental)
            {
                shoppingCartItemForUpdate.RentalStartDateUtc = null;
                shoppingCartItemForUpdate.RentalEndDateUtc = null;
            }

            if (shoppingCartItemDelta.Dto.Attributes != null)
                shoppingCartItemForUpdate.AttributesXml =
                    _productAttributeConverter.ConvertToXml(shoppingCartItemDelta.Dto.Attributes,
                        shoppingCartItemForUpdate.Product.Id);

            // The update time is set in the service.
            var warnings = _shoppingCartService.UpdateShoppingCartItem(shoppingCartItemForUpdate.Customer,
                shoppingCartItemForUpdate.Id,
                shoppingCartItemForUpdate.AttributesXml, shoppingCartItemForUpdate.CustomerEnteredPrice,
                shoppingCartItemForUpdate.RentalStartDateUtc, shoppingCartItemForUpdate.RentalEndDateUtc,
                shoppingCartItemForUpdate.Quantity, false);

            if (warnings.Count > 0)
            {
                foreach (var warning in warnings) ModelState.AddModelError("shopping cart item", warning);

                return Error(HttpStatusCode.BadRequest);
            }

            shoppingCartItemForUpdate = _shoppingCartItemApiService.GetShoppingCartItem(shoppingCartItemForUpdate.Id);

            // Preparing the result dto of the new product category mapping
            var newShoppingCartItemDto = _dtoHelper.PrepareShoppingCartItemDto(shoppingCartItemForUpdate);

            var shoppingCartsRootObject = new ShoppingCartItemsRootObject();

            shoppingCartsRootObject.ShoppingCartItems.Add(newShoppingCartItemDto);

            var json = JsonFieldsSerializer.Serialize(shoppingCartsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/shopping_cart_items/discount")]
        [ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult ApplyDiscount(string discountCoupon)
        {
            try
            {
                _customerService.ApplyDiscountCouponCode(_workContext.CurrentCustomer, discountCoupon);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [HttpDelete]
        [Route("/api/shopping_cart_items/discount")]
        [ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult DeleteDiscount(string discountCoupon)
        {
            try
            {
                _customerService.RemoveDiscountCouponCode(_workContext.CurrentCustomer, discountCoupon);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [HttpGet]
        [Route("/api/shopping_cart_items/discount")]
        [ProducesResponseType(typeof(ShoppingCartItemsRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult GetDiscount(int discountId)
        {
            try
            {
                var discount = _discountService.GetDiscountById(discountId);
                var discountDTo = _discountTranslator.ToDiscountDto(discount);
                var discountsRootObject = new DiscountsRootObject {Discounts = new List<DiscountDto>()};
                discountsRootObject.Discounts.Add(discountDTo);
                var json = JsonFieldsSerializer.Serialize(discountsRootObject, string.Empty);
                return new RawJsonActionResult(json);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}