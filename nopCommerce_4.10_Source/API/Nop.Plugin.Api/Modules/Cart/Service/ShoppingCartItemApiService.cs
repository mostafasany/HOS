using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Plugin.Api.Helpers;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Tax;
using Nop.Services.Vendors;

namespace Nop.Plugin.Api.Services
{
    public class ShoppingCartItemApiService : IShoppingCartItemApiService
    {
        private readonly CatalogSettings _catalogSettings;
        private readonly CommonSettings _commonSettings;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly MediaSettings _mediaSettings;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly OrderSettings _orderSettings;
        private readonly IPaymentService _paymentService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductService _productService;
        private readonly ShippingSettings _shippingSettings;
        private readonly IRepository<ShoppingCartItem> _shoppingCartItemsRepository;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorService _vendorService;
        private readonly VendorSettings _vendorSettings;
        private readonly IWorkContext _workContext;
        private readonly IDTOHelper _dtoHelper;
        public ShoppingCartItemApiService(IRepository<ShoppingCartItem> shoppingCartItemsRepository, IStoreContext storeContext,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDiscountService discountService,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService,
            IPaymentService paymentService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IProductService productService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            ShippingSettings shippingSettings,
            ShoppingCartSettings shoppingCartSettings,
            VendorSettings vendorSettings,
            IDTOHelper dtoHelper,
            IProductAttributeConverter productAttributeConverter)
        {
            _dtoHelper = dtoHelper;
            _shoppingCartItemsRepository = shoppingCartItemsRepository;
            _storeContext = storeContext;
            _catalogSettings = catalogSettings;
            _commonSettings = commonSettings;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _discountService = discountService;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _orderProcessingService = orderProcessingService;
            _paymentService = paymentService;
            _priceCalculationService = priceCalculationService;
            _priceFormatter = priceFormatter;
            _productAttributeFormatter = productAttributeFormatter;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _taxService = taxService;
            _urlRecordService = urlRecordService;
            _vendorService = vendorService;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _orderSettings = orderSettings;
            _shippingSettings = shippingSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _vendorSettings = vendorSettings;
            _productAttributeConverter = productAttributeConverter;
        }

        public List<ShoppingCartItem> GetShoppingCartItems(int? customerId = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue)
        {
            IQueryable<ShoppingCartItem> query = GetShoppingCartItemsQuery(customerId, createdAtMin, createdAtMax,
                updatedAtMin, updatedAtMax);

            return new ApiList<ShoppingCartItem>(query, page - 1, limit);
        }

        public ShoppingCartItem GetShoppingCartItem(int id) => _shoppingCartItemsRepository.GetById(id);

        private IQueryable<ShoppingCartItem> GetShoppingCartItemsQuery(int? customerId = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null)
        {
            IQueryable<ShoppingCartItem> query = _shoppingCartItemsRepository.Table;

            if (customerId != null) query = query.Where(shoppingCartItem => shoppingCartItem.CustomerId == customerId);

            if (createdAtMin != null) query = query.Where(c => c.CreatedOnUtc > createdAtMin.Value);

            if (createdAtMax != null) query = query.Where(c => c.CreatedOnUtc < createdAtMax.Value);

            if (updatedAtMin != null) query = query.Where(c => c.UpdatedOnUtc > updatedAtMin.Value);

            if (updatedAtMax != null) query = query.Where(c => c.UpdatedOnUtc < updatedAtMax.Value);

            // items for the current store only
            int currentStoreId = _storeContext.CurrentStore.Id;
            query = query.Where(c => c.StoreId == currentStoreId);

            query = query.OrderByDescending(shoppingCartItem => shoppingCartItem.CreatedOnUtc);

            return query;
        }

        private PictureModel PrepareCartItemPictureModel(ShoppingCartItem sci, int pictureSize, bool showDefaultPicture, string productName) => new PictureModel();

        /// <summary>
        ///     Prepare the order review data model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <returns>Order review data model</returns>
        private ShoppingCartModel.OrderReviewDataModel PrepareOrderReviewDataModel(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var model = new ShoppingCartModel.OrderReviewDataModel
            {
                Display = true
            };

            //billing info
            Address billingAddress = _workContext.CurrentCustomer.BillingAddress;
            if (billingAddress != null)
            {
                //Comment
                //_addressModelFactory.PrepareAddressModel(model.BillingAddress,
                //        address: billingAddress,
                //        excludeProperties: false,
                //        addressSettings: _addressSettings);
            }

            //shipping info
            if (_shoppingCartService.ShoppingCartRequiresShipping(cart))
            {
                model.IsShippable = true;

                var pickupPoint = _genericAttributeService.GetAttribute<PickupPoint>(_workContext.CurrentCustomer,
                    NopCustomerDefaults.SelectedPickupPointAttribute, _storeContext.CurrentStore.Id);
                model.SelectedPickUpInStore = _shippingSettings.AllowPickUpInStore && pickupPoint != null;
                if (!model.SelectedPickUpInStore)
                {
                    if (_workContext.CurrentCustomer.ShippingAddress != null)
                    {
                        //Comment
                        //_addressModelFactory.PrepareAddressModel(model.ShippingAddress,
                        //    address: _workContext.CurrentCustomer.ShippingAddress,
                        //    excludeProperties: false,
                        //    addressSettings: _addressSettings);
                    }
                }
                else
                {
                    Country country = _countryService.GetCountryByTwoLetterIsoCode(pickupPoint.CountryCode);
                    StateProvince state = _stateProvinceService.GetStateProvinceByAbbreviation(pickupPoint.StateAbbreviation, country?.Id);

                    model.PickupAddress = new AddressModel();
                }

                //selected shipping method
                var shippingOption = _genericAttributeService.GetAttribute<ShippingOption>(_workContext.CurrentCustomer,
                    NopCustomerDefaults.SelectedShippingOptionAttribute, _storeContext.CurrentStore.Id);
                if (shippingOption != null)
                    model.ShippingMethod = shippingOption.Name;
            }

            //payment info
            var selectedPaymentMethodSystemName = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer, NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.CurrentStore.Id);
            IPaymentMethod paymentMethod = _paymentService.LoadPaymentMethodBySystemName(selectedPaymentMethodSystemName);
            model.PaymentMethod = paymentMethod != null
                ? _localizationService.GetLocalizedFriendlyName(paymentMethod, _workContext.WorkingLanguage.Id)
                : "";

            //custom values
            var processPaymentRequest = _httpContextAccessor.HttpContext?.Session?.Get<ProcessPaymentRequest>("OrderPaymentInfo");
            if (processPaymentRequest != null)
                model.CustomValues = processPaymentRequest.CustomValues;

            return model;
        }


        /// <summary>
        ///     Prepare the shopping cart item model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="sci">Shopping cart item</param>
        /// <returns>Shopping cart item model</returns>
        private ShoppingCartModel.ShoppingCartItemModel PrepareShoppingCartItemModel(IList<ShoppingCartItem> cart, ShoppingCartItem sci, IList<Vendor> vendors)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (sci == null)
                throw new ArgumentNullException(nameof(sci));
            var cartItemModel = new ShoppingCartModel.ShoppingCartItemModel
            {
                Id = sci.Id,
                Sku = _productService.FormatSku(sci.Product, sci.AttributesXml),
                VendorName = vendors.FirstOrDefault(v => v.Id == sci.Product.VendorId)?.Name ?? string.Empty,
                Product = _dtoHelper.PrepareProductDTO(sci.Product),
                ProductId = sci.Product.Id,
                ProductName = _localizationService.GetLocalized(sci.Product, x => x.Name),
                ProductSeName = _urlRecordService.GetSeName(sci.Product),
                Quantity = sci.Quantity,
                AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml)
            };
            var productAttributes = _productAttributeConverter.Parse(sci.AttributesXml);
            decimal originalPrice =0;
            if (cartItemModel.Product.Price.HasValue)
            {
                originalPrice = cartItemModel.Product.Price.Value;
                if (productAttributes != null && productAttributes.Any())
                {
                    var firstAttribute = productAttributes.FirstOrDefault();
                    ProductAttributeMappingDto productAttributeMappingDto = cartItemModel.Product.ProductAttributeMappings.FirstOrDefault(a => firstAttribute != null && a.Id == firstAttribute.Id);
                    ProductAttributeValueDto productAttributeValueDto = productAttributeMappingDto?.ProductAttributeValues.FirstOrDefault(a => firstAttribute != null && a.Id == int.Parse(firstAttribute.Value));
                    if (productAttributeValueDto?.PriceAdjustment != null)
                    {
                        var adjustedAmount = productAttributeValueDto.PriceAdjustment.Value;
                        if (!productAttributeValueDto.PriceAdjustmentUsePercentage)
                        {
                            originalPrice += adjustedAmount;
                        }
                        else
                        {
                            var adjustedDiscountAmount = (originalPrice * adjustedAmount) / 100;
                            originalPrice += adjustedDiscountAmount;
                        }
                    }
                }
            }

            cartItemModel.OriginalPrice= originalPrice;


            //allow editing?
            //1. setting enabled?
            //2. simple product?
            //3. has attribute or gift card?
            //4. visible individually?
            cartItemModel.AllowItemEditing = _shoppingCartSettings.AllowCartItemEditing &&
                                             sci.Product.ProductType == ProductType.SimpleProduct &&
                                             (!string.IsNullOrEmpty(cartItemModel.AttributeInfo) ||
                                              sci.Product.IsGiftCard) &&
                                             sci.Product.VisibleIndividually;

            //disable removal?
            //1. do other items require this one?
            cartItemModel.DisableRemoval = cart.Any(item => item.Product.RequireOtherProducts && _productService.ParseRequiredProductIds(item.Product).Contains(sci.ProductId));

            //allowed quantities
            int[] allowedQuantities = _productService.ParseAllowedQuantities(sci.Product);
            foreach (int qty in allowedQuantities)
                cartItemModel.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = sci.Quantity == qty
                });

            //recurring info
            if (sci.Product.IsRecurring)
                cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"),
                    sci.Product.RecurringCycleLength, _localizationService.GetLocalizedEnum(sci.Product.RecurringCyclePeriod));

            //rental info
            if (sci.Product.IsRental)
            {
                string rentalStartDate = sci.RentalStartDateUtc.HasValue
                    ? _productService.FormatRentalDate(sci.Product, sci.RentalStartDateUtc.Value)
                    : "";
                string rentalEndDate = sci.RentalEndDateUtc.HasValue
                    ? _productService.FormatRentalDate(sci.Product, sci.RentalEndDateUtc.Value)
                    : "";
                cartItemModel.RentalInfo =
                    string.Format(_localizationService.GetResource("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
            }

            //unit prices
            if (sci.Product.CallForPrice &&
                //also check whether the current user is impersonated
                (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
            {
                cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                cartItemModel.UnitPriceNumber = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out decimal _);
            }
            else
            {
                decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out decimal _);
                decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                cartItemModel.UnitPriceNumber = shoppingCartUnitPriceWithDiscount;
            }

            //subtotal, discount
            if (sci.Product.CallForPrice &&
                //also check whether the current user is impersonated
                (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
            {
                cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
            }
            else
            {
                //sub total
                decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci, true, out decimal shoppingCartItemDiscountBase, out List<DiscountForCaching> _, out int? maximumDiscountQty), out decimal taxRate);
                decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);
                cartItemModel.SubTotalNumber = shoppingCartItemSubTotalWithDiscount;
                cartItemModel.MaximumDiscountedQty = maximumDiscountQty;

                //display an applied discount amount
                if (shoppingCartItemDiscountBase > decimal.Zero)
                {
                    shoppingCartItemDiscountBase = _taxService.GetProductPrice(sci.Product, shoppingCartItemDiscountBase, out taxRate);
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, _workContext.WorkingCurrency);
                        cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                        cartItemModel.DiscountlNumber = shoppingCartItemDiscount;
                    }
                }
            }

            //picture
            if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
                cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                    _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);

            //item warnings
            IList<string> itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
                _workContext.CurrentCustomer,
                sci.ShoppingCartType,
                sci.Product,
                sci.StoreId,
                sci.AttributesXml,
                sci.CustomerEnteredPrice,
                sci.RentalStartDateUtc,
                sci.RentalEndDateUtc,
                sci.Quantity,
                false,
                sci.Id);
            foreach (string warning in itemWarnings)
                cartItemModel.Warnings.Add(warning);

            return cartItemModel;
        }


        /// <summary>
        ///     Prepare the shopping cart model
        /// </summary>
        /// <param name="model">Shopping cart model</param>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <param name="validateCheckoutAttributes">Whether to validate checkout attributes</param>
        /// <param name="prepareAndDisplayOrderReviewData">Whether to prepare and display order review data</param>
        /// <returns>Shopping cart model</returns>
        public ShoppingCartModel PrepareShoppingCartModel(ShoppingCartModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true,
            bool validateCheckoutAttributes = false,
            bool prepareAndDisplayOrderReviewData = false)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //simple properties
            model.OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled;
            if (!cart.Any())
                return model;
            model.IsEditable = isEditable;
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnShoppingCart;
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
            model.ShowVendorName = _vendorSettings.ShowVendorOnOrderDetailsPage;
            var checkoutAttributesXml = _genericAttributeService.GetAttribute<string>(_workContext.CurrentCustomer,
                NopCustomerDefaults.CheckoutAttributes, _storeContext.CurrentStore.Id);
            bool minOrderSubtotalAmountOk = _orderProcessingService.ValidateMinOrderSubtotalAmount(cart);
            if (!minOrderSubtotalAmountOk)
            {
                decimal minOrderSubtotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderSubtotalAmount, _workContext.WorkingCurrency);
                model.MinOrderSubtotalWarning = string.Format(_localizationService.GetResource("Checkout.MinOrderSubtotalAmount"), _priceFormatter.FormatPrice(minOrderSubtotalAmount, true, false));
            }

            model.TermsOfServiceOnShoppingCartPage = _orderSettings.TermsOfServiceOnShoppingCartPage;
            model.TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage;
            model.TermsOfServicePopup = _commonSettings.PopupForTermsOfServiceLinks;
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoShoppingCart;

            //discount and gift card boxes
            model.DiscountBox.Display = _shoppingCartSettings.ShowDiscountBox;
            string[] discountCouponCodes = _customerService.ParseAppliedDiscountCouponCodes(_workContext.CurrentCustomer);
            foreach (string couponCode in discountCouponCodes)
            {
                DiscountForCaching discount = _discountService.GetAllDiscountsForCaching(couponCode: couponCode)
                    .FirstOrDefault(d => d.RequiresCouponCode && _discountService.ValidateDiscount(d, _workContext.CurrentCustomer).IsValid);

                if (discount != null)
                    model.DiscountBox.AppliedDiscountsWithCodes.Add(new ShoppingCartModel.DiscountBoxModel.DiscountInfoModel
                    {
                        Id = discount.Id,
                        CouponCode = discount.CouponCode
                    });
            }

            model.GiftCardBox.Display = _shoppingCartSettings.ShowGiftCardBox;

            //cart warnings
            IList<string> cartWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, checkoutAttributesXml, validateCheckoutAttributes);
            foreach (string warning in cartWarnings)
                model.Warnings.Add(warning);

            //checkout attributes
            //model.CheckoutAttributes = PrepareCheckoutAttributeModels(cart);

            IList<Vendor> vendors = _vendorSettings.ShowVendorOnOrderDetailsPage ? _vendorService.GetVendorsByIds(cart.Select(item => item.Product.VendorId).ToArray()) : new List<Vendor>();

            //cart items
            foreach (ShoppingCartItem sci in cart)
            {
                ShoppingCartModel.ShoppingCartItemModel cartItemModel = PrepareShoppingCartItemModel(cart, sci, vendors);
                model.Items.Add(cartItemModel);
            }

            //payment methods
            //all payment methods (do not filter by country here as it could be not specified yet)
            List<IPaymentMethod> paymentMethods = _paymentService
                .LoadActivePaymentMethods(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id)
                .Where(pm => !pm.HidePaymentMethod(cart))
                .ToList();
            //payment methods displayed during checkout (not with "Button" type)
            List<IPaymentMethod> nonButtonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType != PaymentMethodType.Button)
                .ToList();
            //"button" payment methods(*displayed on the shopping cart page)
            List<IPaymentMethod> buttonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Button)
                .ToList();
            foreach (IPaymentMethod pm in buttonPaymentMethods)
            {
                if (_shoppingCartService.ShoppingCartIsRecurring(cart) && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                string viewComponentName = pm.GetPublicViewComponentName();
                model.ButtonPaymentMethodViewComponentNames.Add(viewComponentName);
            }

            //hide "Checkout" button if we have only "Button" payment methods
            model.HideCheckoutButton = !nonButtonPaymentMethods.Any() && model.ButtonPaymentMethodViewComponentNames.Any();

            //order review data
            if (prepareAndDisplayOrderReviewData) model.OrderReviewData = PrepareOrderReviewDataModel(cart);

            return model;
        }
    }
}