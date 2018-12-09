using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTOs.Products;

namespace Nop.Plugin.Api.DTOs.ShoppingCarts
{
    /// <summary>
    ///     Represents base nopCommerce entity model
    /// </summary>
    public class BaseNopEntityModel : BaseNopModel
    {
        /// <summary>
        ///     Gets or sets model identifier
        /// </summary>
        public virtual int Id { get; set; }
    }

    /// <summary>
    ///     Represents base nopCommerce model
    /// </summary>
    public class BaseNopModel
    {
        #region Ctor

        /// <summary>
        ///     Ctor
        /// </summary>
        public BaseNopModel()
        {
            CustomProperties = new Dictionary<string, object>();
            PostInitialize();
        }

        #endregion

        #region Properties

        //MVC is suppressing further validation if the IFormCollection is passed to a controller method. That's why we add it to the model
        //public IFormCollection Form { get; set; }

        /// <summary>
        ///     Gets or sets property to store any custom values for models
        /// </summary>
        public Dictionary<string, object> CustomProperties { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Perform additional actions for binding the model
        /// </summary>
        /// <param name="bindingContext">Model binding context</param>
        /// <remarks>Developers can override this method in custom partial classes in order to add some custom model binding</remarks>
        /// <summary>
        ///     Perform additional actions for the model initialization
        /// </summary>
        /// <remarks>
        ///     Developers can override this method in custom partial classes in order to add some custom initialization code
        ///     to constructors
        /// </remarks>
        protected virtual void PostInitialize()
        {
        }

        #endregion
    }

    public class ShoppingCartModel : BaseNopModel
    {
        public ShoppingCartModel()
        {
            Items = new List<ShoppingCartItemModel>();
            Warnings = new List<string>();
            DiscountBox = new DiscountBoxModel();
            GiftCardBox = new GiftCardBoxModel();
            CheckoutAttributes = new List<CheckoutAttributeModel>();
            OrderReviewData = new OrderReviewDataModel();

            ButtonPaymentMethodViewComponentNames = new List<string>();
        }

        public bool OnePageCheckoutEnabled { get; set; }

        public bool ShowSku { get; set; }
        public bool ShowProductImages { get; set; }
        public bool IsEditable { get; set; }
        public IList<ShoppingCartItemModel> Items { get; set; }

        public IList<CheckoutAttributeModel> CheckoutAttributes { get; set; }

        public IList<string> Warnings { get; set; }
        public string MinOrderSubtotalWarning { get; set; }
        public bool DisplayTaxShippingInfo { get; set; }
        public bool TermsOfServiceOnShoppingCartPage { get; set; }
        public bool TermsOfServiceOnOrderConfirmPage { get; set; }
        public bool TermsOfServicePopup { get; set; }
        public DiscountBoxModel DiscountBox { get; set; }
        public GiftCardBoxModel GiftCardBox { get; set; }
        public OrderReviewDataModel OrderReviewData { get; set; }

        public IList<string> ButtonPaymentMethodViewComponentNames { get; set; }

        public bool HideCheckoutButton { get; set; }
        public bool ShowVendorName { get; set; }

        public decimal SubTotalDiscount { get; set; }

        public decimal SubTotal { get; set; }

        #region Nested Classes

        public class ShoppingCartItemModel : BaseNopEntityModel
        {
            public ShoppingCartItemModel()
            {
                Picture = new PictureModel();
                AllowedQuantities = new List<SelectListItem>();
                Warnings = new List<string>();
            }

            public string Sku { get; set; }

            public string VendorName { get; set; }

            public decimal OriginalPrice { get; set; }

            public bool HasDiscount =>UnitPriceNumber< OriginalPrice;

            public PictureModel Picture { get; set; }

            public ProductDto Product { get; set; }
            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string ProductSeName { get; set; }

            public string UnitPrice { get; set; }
            public decimal UnitPriceNumber { get; set; }

            public string SubTotal { get; set; }

            public decimal SubTotalNumber { get; set; }

            public string Discount { get; set; }
            public decimal DiscountlNumber { get; set; }
            public int? MaximumDiscountedQty { get; set; }

            public int Quantity { get; set; }
            public List<SelectListItem> AllowedQuantities { get; set; }

            public string AttributeInfo { get; set; }

            public string RecurringInfo { get; set; }

            public string RentalInfo { get; set; }

            public bool AllowItemEditing { get; set; }

            public bool DisableRemoval { get; set; }

            public IList<string> Warnings { get; set; }
        }

        public class CheckoutAttributeModel : BaseNopEntityModel
        {
            public CheckoutAttributeModel()
            {
                AllowedFileExtensions = new List<string>();
                Values = new List<CheckoutAttributeValueModel>();
            }

            public string Name { get; set; }

            public string DefaultValue { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            ///     Selected day value for datepicker
            /// </summary>
            public int? SelectedDay { get; set; }

            /// <summary>
            ///     Selected month value for datepicker
            /// </summary>
            public int? SelectedMonth { get; set; }

            /// <summary>
            ///     Selected year value for datepicker
            /// </summary>
            public int? SelectedYear { get; set; }

            /// <summary>
            ///     Allowed file extensions for customer uploaded files
            /// </summary>
            public IList<string> AllowedFileExtensions { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<CheckoutAttributeValueModel> Values { get; set; }
        }

        public class CheckoutAttributeValueModel : BaseNopEntityModel
        {
            public string Name { get; set; }

            public string ColorSquaresRgb { get; set; }

            public string PriceAdjustment { get; set; }

            public bool IsPreSelected { get; set; }
        }

        public class DiscountBoxModel : BaseNopModel
        {
            public DiscountBoxModel()
            {
                AppliedDiscountsWithCodes = new List<DiscountInfoModel>();
                Messages = new List<string>();
            }

            public List<DiscountInfoModel> AppliedDiscountsWithCodes { get; set; }
            public bool Display { get; set; }
            public List<string> Messages { get; set; }
            public bool IsApplied { get; set; }

            public class DiscountInfoModel : BaseNopEntityModel
            {
                public string CouponCode { get; set; }
            }
        }

        public class GiftCardBoxModel : BaseNopModel
        {
            public bool Display { get; set; }
            public string Message { get; set; }
            public bool IsApplied { get; set; }
        }

        public class OrderReviewDataModel : BaseNopModel
        {
            public OrderReviewDataModel()
            {
                BillingAddress = new AddressModel();
                ShippingAddress = new AddressModel();
                PickupAddress = new AddressModel();
                CustomValues = new Dictionary<string, object>();
            }

            public bool Display { get; set; }

            public AddressModel BillingAddress { get; set; }

            public bool IsShippable { get; set; }
            public AddressModel ShippingAddress { get; set; }
            public bool SelectedPickUpInStore { get; set; }
            public AddressModel PickupAddress { get; set; }
            public string ShippingMethod { get; set; }

            public string PaymentMethod { get; set; }

            public Dictionary<string, object> CustomValues { get; set; }
        }

        #endregion
    }

    public class AddressModel : BaseNopEntityModel
    {
        //public AddressModel()
        //{
        //    AvailableCountries = new List<SelectListItem>();
        //    AvailableStates = new List<SelectListItem>();
        //    CustomAddressAttributes = new List<AddressAttributeModel>();
        //}

        //[NopResourceDisplayName("Address.Fields.FirstName")]
        //public string FirstName { get; set; }
        //[NopResourceDisplayName("Address.Fields.LastName")]
        //public string LastName { get; set; }
        //[DataType(DataType.EmailAddress)]
        //[NopResourceDisplayName("Address.Fields.Email")]
        //public string Email { get; set; }


        //public bool CompanyEnabled { get; set; }
        //public bool CompanyRequired { get; set; }
        //[NopResourceDisplayName("Address.Fields.Company")]
        //public string Company { get; set; }

        //public bool CountryEnabled { get; set; }
        //[NopResourceDisplayName("Address.Fields.Country")]
        //public int? CountryId { get; set; }
        //[NopResourceDisplayName("Address.Fields.Country")]
        //public string CountryName { get; set; }

        //public bool StateProvinceEnabled { get; set; }
        //[NopResourceDisplayName("Address.Fields.StateProvince")]
        //public int? StateProvinceId { get; set; }
        //[NopResourceDisplayName("Address.Fields.StateProvince")]
        //public string StateProvinceName { get; set; }

        //public bool CountyEnabled { get; set; }
        //public bool CountyRequired { get; set; }
        //[NopResourceDisplayName("Address.Fields.County")]
        //public string County { get; set; }

        //public bool CityEnabled { get; set; }
        //public bool CityRequired { get; set; }
        //[NopResourceDisplayName("Address.Fields.City")]
        //public string City { get; set; }

        //public bool StreetAddressEnabled { get; set; }
        //public bool StreetAddressRequired { get; set; }
        //[NopResourceDisplayName("Address.Fields.Address1")]
        //public string Address1 { get; set; }

        //public bool StreetAddress2Enabled { get; set; }
        //public bool StreetAddress2Required { get; set; }
        //[NopResourceDisplayName("Address.Fields.Address2")]
        //public string Address2 { get; set; }

        //public bool ZipPostalCodeEnabled { get; set; }
        //public bool ZipPostalCodeRequired { get; set; }
        //[NopResourceDisplayName("Address.Fields.ZipPostalCode")]
        //public string ZipPostalCode { get; set; }

        //public bool PhoneEnabled { get; set; }
        //public bool PhoneRequired { get; set; }
        //[DataType(DataType.PhoneNumber)]
        //[NopResourceDisplayName("Address.Fields.PhoneNumber")]
        //public string PhoneNumber { get; set; }

        //public bool FaxEnabled { get; set; }
        //public bool FaxRequired { get; set; }
        //[NopResourceDisplayName("Address.Fields.FaxNumber")]
        //public string FaxNumber { get; set; }

        //public IList<SelectListItem> AvailableCountries { get; set; }
        //public IList<SelectListItem> AvailableStates { get; set; }

        //public string FormattedCustomAddressAttributes { get; set; }
        //public IList<AddressAttributeModel> CustomAddressAttributes { get; set; }
    }

    public class PictureModel : BaseNopModel
    {
        public string ImageUrl { get; set; }

        public string ThumbImageUrl { get; set; }

        public string FullSizeImageUrl { get; set; }

        public string Title { get; set; }

        public string AlternateText { get; set; }
    }
}