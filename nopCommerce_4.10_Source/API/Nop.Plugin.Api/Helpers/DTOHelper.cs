using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Articles;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Topics;
using Nop.Plugin.Api.DTOs.Articles;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.DTOs.Countries;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.DTOs.Discounts;
using Nop.Plugin.Api.DTOs.Images;
using Nop.Plugin.Api.DTOs.Languages;
using Nop.Plugin.Api.DTOs.Manufacturers;
using Nop.Plugin.Api.DTOs.OrderItems;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.DTOs.ProductAttributes;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Plugin.Api.DTOs.SpecificationAttributes;
using Nop.Plugin.Api.DTOs.Stores;
using Nop.Plugin.Api.DTOs.Topics;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Helpers
{
    public class DTOHelper : IDTOHelper
    {
        private readonly IAclService _aclService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly ICustomerApiService _customerApiService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly int _currentLangaugeId;

        public DTOHelper(IProductService productService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPictureService pictureService,
            IProductAttributeService productAttributeService,
            ICustomerApiService customerApiService,
            IProductAttributeConverter productAttributeConverter,
            ILanguageService languageService,
            ICurrencyService currencyService,
            CurrencySettings currencySettings,
            IStoreService storeService,
            ILocalizationService localizationService,
            IUrlRecordService urlRecordService,
            IProductTagService productTagService, IHttpContextAccessor httpContextAccessor)
        {
            _productService = productService;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _pictureService = pictureService;
            _productAttributeService = productAttributeService;
            _customerApiService = customerApiService;
            _productAttributeConverter = productAttributeConverter;
            _languageService = languageService;
            _currencyService = currencyService;
            _currencySettings = currencySettings;
            _storeService = storeService;
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            _productTagService = productTagService;
            var headers = httpContextAccessor.HttpContext.Request.Headers;
            if (headers.ContainsKey("Accept-Language"))
            {
                var lan = headers["Accept-Language"];
                if (lan.ToString() == "en")
                {
                    _currentLangaugeId = 1;
                }
                else
                {
                    _currentLangaugeId = 2;
                }
            }
        }

        public ProductDto PrepareProductDTO(Product product)
        {
            ProductDto productDto = product.ToDto();

            productDto.Name = _localizationService.GetLocalized(product, x => x.Name, _currentLangaugeId);
            productDto.ShortDescription = _localizationService.GetLocalized(product, x => x.ShortDescription, _currentLangaugeId);
            productDto.FullDescription = _localizationService.GetLocalized(product, x => x.FullDescription, _currentLangaugeId);

            PrepareProductImages(product.ProductPictures, productDto);
            PrepareProductAttributes(product.ProductAttributeMappings, productDto);
            PrepareProductSpecificationAttributes(product.ProductSpecificationAttributes, productDto);

            productDto.SeName = _urlRecordService.GetSeName(product);
            productDto.DiscountIds = product.AppliedDiscounts.Select(discount => discount.Id).ToList();
            productDto.ManufacturerIds = product.ProductManufacturers.Select(pm => pm.ManufacturerId).ToList();
            productDto.RoleIds = _aclService.GetAclRecords(product).Select(acl => acl.CustomerRoleId).ToList();
            productDto.StoreIds = _storeMappingService.GetStoreMappings(product).Select(mapping => mapping.StoreId)
                .ToList();
            productDto.Tags = _productTagService.GetAllProductTagsByProductId(product.Id).Select(tag => tag.Name)
                .ToList();

            productDto.AssociatedProductIds =
                _productService.GetAssociatedProducts(product.Id, showHidden: true)
                    .Select(associatedProduct => associatedProduct.Id)
                    .ToList();

            //IList<Language> allLanguages = _languageService.GetAllLanguages();

            //productDto.LocalizedNames = new List<LocalizedNameDto>();

            //foreach (Language language in allLanguages)
            //{
            //    var localizedNameDto = new LocalizedNameDto
            //    {
            //        LanguageId = language.Id,
            //        LocalizedName = _localizationService.GetLocalized(product, x => x.Name, language.Id)
            //    };

            //    productDto.LocalizedNames.Add(localizedNameDto);
            //}


            // productDto.LocalizedNames.FirstOrDefault(a => a.LanguageId == _currentLangaugeId)?.LocalizedName;
            return productDto;
        }

        public CategoryDto PrepareCategoryDTO(Category category)
        {
            CategoryDto categoryDto = category.ToDto();
            categoryDto.Name = _localizationService.GetLocalized(category, x => x.Name, _currentLangaugeId);
            categoryDto.Description = _localizationService.GetLocalized(category, x => x.Description, _currentLangaugeId);

            Picture picture = _pictureService.GetPictureById(category.PictureId);
            ImageDto imageDto = PrepareImageDto(picture);

            if (imageDto != null) categoryDto.Image = imageDto;

            categoryDto.SeName = _urlRecordService.GetSeName(category);
            categoryDto.DiscountIds = category.AppliedDiscounts.Select(discount => discount.Id).ToList();
            categoryDto.RoleIds = _aclService.GetAclRecords(category).Select(acl => acl.CustomerRoleId).ToList();
            categoryDto.StoreIds = _storeMappingService.GetStoreMappings(category).Select(mapping => mapping.StoreId)
                .ToList();

            IList<Language> allLanguages = _languageService.GetAllLanguages();

            categoryDto.LocalizedNames = new List<LocalizedNameDto>();

            foreach (Language language in allLanguages)
            {
                var localizedNameDto = new LocalizedNameDto
                {
                    LanguageId = language.Id,
                    LocalizedName = _localizationService.GetLocalized(category, x => x.Name, language.Id)
                };

                categoryDto.LocalizedNames.Add(localizedNameDto);
            }

            return categoryDto;
        }

        public OrderDto PrepareOrderDTO(Order order)
        {
            OrderDto orderDto = order.ToDto();

            orderDto.OrderItems = order.OrderItems.Select(PrepareOrderItemDTO).ToList();

            CustomerDto customerDto = _customerApiService.GetCustomerById(order.Customer.Id);

            if (customerDto != null) orderDto.Customer = customerDto.ToOrderCustomerDto();

            return orderDto;
        }

        public ShoppingCartItemDto PrepareShoppingCartItemDTO(ShoppingCartItem shoppingCartItem)
        {
            ShoppingCartItemDto dto = shoppingCartItem.ToDto();
            dto.ProductDto = PrepareProductDTO(shoppingCartItem.Product);
            dto.CustomerDto = shoppingCartItem.Customer.ToCustomerForShoppingCartItemDto();
            dto.Attributes = _productAttributeConverter.Parse(shoppingCartItem.AttributesXml);
            return dto;
        }

        public OrderItemDto PrepareOrderItemDTO(OrderItem orderItem)
        {
            OrderItemDto dto = orderItem.ToDto();
            dto.Product = PrepareProductDTO(orderItem.Product);
            dto.Attributes = _productAttributeConverter.Parse(orderItem.AttributesXml);
            return dto;
        }

        public StoreDto PrepareStoreDTO(Store store)
        {
            StoreDto storeDto = store.ToDto();

            Currency primaryCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);

            if (!string.IsNullOrEmpty(primaryCurrency.DisplayLocale)) storeDto.PrimaryCurrencyDisplayLocale = primaryCurrency.DisplayLocale;

            storeDto.LanguageIds = _languageService.GetAllLanguages(false, store.Id).Select(x => x.Id).ToList();

            return storeDto;
        }

        public LanguageDto PrepateLanguageDto(Language language)
        {
            LanguageDto languageDto = language.ToDto();

            languageDto.StoreIds = _storeMappingService.GetStoreMappings(language).Select(mapping => mapping.StoreId)
                .ToList();

            if (languageDto.StoreIds.Count == 0) languageDto.StoreIds = _storeService.GetAllStores().Select(s => s.Id).ToList();

            return languageDto;
        }

        public ProductAttributeDto PrepareProductAttributeDTO(ProductAttribute productAttribute)
        {
            var attribute = productAttribute.ToDto();
            attribute.Name = _localizationService.GetLocalized(productAttribute, x => x.Name, _currentLangaugeId);
            attribute.Description = _localizationService.GetLocalized(productAttribute, x => x.Description, _currentLangaugeId);
            return attribute;
        }

        public ProductSpecificationAttributeDto PrepareProductSpecificationAttributeDto(ProductSpecificationAttribute productSpecificationAttribute) => productSpecificationAttribute.ToDto();

        public SpecificationAttributeDto PrepareSpecificationAttributeDto(SpecificationAttribute specificationAttribute) => specificationAttribute.ToDto();

        public TopicDto PrepateTopicDto(Topic topic)
        {
            string seName = _urlRecordService.GetSeName(topic);
            var body = _localizationService.GetLocalized(topic, x => x.Body, _currentLangaugeId);
            var title = _localizationService.GetLocalized(topic, x => x.Title, _currentLangaugeId);
            return new TopicDto { Id = topic.Id, Body = body, Title = title, SeName = seName };
        }

        public ManufacturerDto PrepateManufacturerDto(Manufacturer manufacturer) => new ManufacturerDto { Id = manufacturer.Id, Name = manufacturer.Name, Description = manufacturer.Description };

        public ArticlesDto PrepateArticleDto(Article article)
        {
            Picture picture = _pictureService.GetPictureById(article.PictureId);
            ImageDto imageDto = PrepareImageDto(picture);


            var articleDto = new ArticlesDto
            {
                Id = article.Id,
                Body = article.Body,
                Title = article.Title,
                AllowComments = article.AllowComments,
                CommentCount = article.CommentCount,
                CreatedOnUtc = article.CreatedOnUtc,
                UpdatedOnUtc = article.UpdatedOnUtc,
                MetaDescription = article.MetaDescription,
                MetaTitle = article.MetaTitle,
                Tags = article.Tags
            };
            articleDto.Title = _localizationService.GetLocalized(article, x => x.Title, _currentLangaugeId);
            articleDto.Body = _localizationService.GetLocalized(article, x => x.Body, _currentLangaugeId);
            if (imageDto != null) articleDto.Image = imageDto;

            return articleDto;
        }

        public ArticleGroupDto PrepateArticleGroupDto(FNS_ArticleGroup articleGroup) => new ArticleGroupDto { Id = articleGroup.Id, Name = articleGroup.Name, ParentGroupId = articleGroup.ParentGroupId };

        public DiscountDto PrepateDiscountDto(Discount discount) => new DiscountDto
        {
            CouponCode = discount.CouponCode,
            Name = discount.Name,
            Id = discount.Id,
            DiscountLimitationId = discount.DiscountLimitationId,
            DiscountPercentage = discount.DiscountPercentage,
            DiscountTypeId = discount.DiscountTypeId,
            IsCumulative = discount.IsCumulative,
            LimitationTimes = discount.LimitationTimes,
            MaximumDiscountAmount = discount.MaximumDiscountAmount,
            MaximumDiscountedQuantity = discount.MaximumDiscountedQuantity,
            RequiresCouponCode = discount.RequiresCouponCode,
            UsePercentage = discount.UsePercentage
        };

        public ExtendedShoppingCartDto PrepareExtendedShoppingCartItemDto(IEnumerable<ShoppingCartItem> shoppingCartItems)
        {
            var cart = new ExtendedShoppingCartDto { ShoppingCartItems = new List<ExtendedShoppingCartItemDto>() };
            foreach (ShoppingCartItem shoppingCartItem in shoppingCartItems)
            {
                decimal total = 0;
                ShoppingCartItemDto dto = shoppingCartItem.ToDto();
                dto.ProductDto = PrepareProductDTO(shoppingCartItem.Product);
                dto.CustomerDto = shoppingCartItem.Customer.ToCustomerForShoppingCartItemDto();
                dto.Attributes = _productAttributeConverter.Parse(shoppingCartItem.AttributesXml);
                if (dto.ProductDto.Price.HasValue)
                {
                    total = dto.ProductDto.Price.Value * (dto.Quantity ?? 1);
                    DateTime now = DateTime.Now;
                    IEnumerable<Discount> productsDiscounts = shoppingCartItem.Product.AppliedDiscounts.Where(discount => now > discount.StartDateUtc && now < discount.EndDateUtc && discount.DiscountType == DiscountType.AssignedToSkus).ToArray();
                    //IEnumerable<Discount> categoriesDiscounts = shoppingCartItem.Product.AppliedDiscounts.Where(discount => now > discount.StartDateUtc && now < discount.EndDateUtc && discount.DiscountType == DiscountType.AssignedToCategories).ToArray();
                    //IEnumerable<Discount> manufacturerssDiscounts = shoppingCartItem.Product.AppliedDiscounts.Where(discount => now > discount.StartDateUtc && now < discount.EndDateUtc &&  discount.DiscountType == DiscountType.AssignedToManufacturers).ToArray();
                    foreach (Discount discount in productsDiscounts)
                    {
                        if (discount.UsePercentage)
                            total = total * (discount.DiscountPercentage * discount.MaximumDiscountedQuantity ?? 1) / 100;
                        else
                            total -= discount.DiscountAmount * discount.MaximumDiscountedQuantity ?? 1;
                        break;
                    }
                }

                var extendedShoppingCartItem = new ExtendedShoppingCartItemDto
                {
                    Id = dto.Id,
                    ShoppingCartItem = dto,
                    Price = dto.ProductDto.Price,
                    Total = total,
                    DiscountApplied = dto.ProductDto.Price > total,
                    Discount = dto.ProductDto.Price > total ? dto.ProductDto.Price - total : 0,
                    ExtraInfo = dto.ProductDto.Price > total ? "You saved " + (dto.ProductDto.Price - total) : ""
                };

                cart.ShoppingCartItems.Add(extendedShoppingCartItem);
            }
            // IEnumerable<Discount> subTotalDiscount = shoppingCartItem.Product.AppliedDiscounts.Where(discount => now > discount.StartDateUtc && now < discount.EndDateUtc && discount.DiscountType == DiscountType.AssignedToSkus).ToArray();

            cart.SubTotal = cart.ShoppingCartItems.Sum(a => a.Total);
            cart.SubTotalDiscount = cart.ShoppingCartItems.Sum(a => a.Discount);
            cart.Total = cart.SubTotal - cart.SubTotalDiscount;
            cart.TotalDiscount = 0;
            cart.Shipping = 0;
            cart.Tax = 0;
            return cart;
        }

        public StateProvinceDto PrepateProvinceStateDto(StateProvince state) => new StateProvinceDto { Abbreviation = state.Abbreviation, Id = state.Id, Name = state.Name, CountryId = state.CountryId };

        public void PrepareProductSpecificationAttributes(IEnumerable<ProductSpecificationAttribute> productSpecificationAttributes, ProductDto productDto)
        {
            if (productDto.ProductSpecificationAttributes == null)
                productDto.ProductSpecificationAttributes = new List<ProductSpecificationAttributeDto>();

            foreach (ProductSpecificationAttribute productSpecificationAttribute in productSpecificationAttributes)
            {
                ProductSpecificationAttributeDto productSpecificationAttributeDto = PrepareProductSpecificationAttributeDto(productSpecificationAttribute);

                if (productSpecificationAttributeDto != null) productDto.ProductSpecificationAttributes.Add(productSpecificationAttributeDto);
            }
        }


        public ShippingOptionDto PrepareShippingOptionItemDTO(ShippingOption shippingOption)
        {
            return new ShippingOptionDto
            {
                Description = shippingOption.Description,
                Name = shippingOption.Name,
                Rate = shippingOption.Rate,
                ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName
            };
        }
        protected ImageDto PrepareImageDto(Picture picture)
        {
            ImageDto image = null;

            if (picture != null)
                image = new ImageDto
                {
                    //Attachment = Convert.ToBase64String(picture.PictureBinary),
                    Src = _pictureService.GetPictureUrl(picture)
                };

            return image;
        }

        private ProductAttributeMappingDto PrepareProductAttributeMappingDto(
            ProductAttributeMapping productAttributeMapping)
        {
            ProductAttributeMappingDto productAttributeMappingDto = null;

            if (productAttributeMapping != null)
                productAttributeMappingDto = new ProductAttributeMappingDto
                {
                    Id = productAttributeMapping.Id,
                    ProductAttributeId = productAttributeMapping.ProductAttributeId,
                    ProductAttributeName = _productAttributeService
                        .GetProductAttributeById(productAttributeMapping.ProductAttributeId).Name,
                    TextPrompt = _localizationService.GetLocalized(productAttributeMapping, x => x.TextPrompt, _currentLangaugeId),
                    DefaultValue = productAttributeMapping.DefaultValue,
                    AttributeControlTypeId = productAttributeMapping.AttributeControlTypeId,
                    DisplayOrder = productAttributeMapping.DisplayOrder,
                    IsRequired = productAttributeMapping.IsRequired,
                    ProductAttributeValues = productAttributeMapping.ProductAttributeValues
                        .Select(x => PrepareProductAttributeValueDto(x, productAttributeMapping.Product)).ToList()
                };

            return productAttributeMappingDto;
        }

        private void PrepareProductAttributes(IEnumerable<ProductAttributeMapping> productAttributeMappings,
            ProductDto productDto)
        {
            if (productDto.ProductAttributeMappings == null) productDto.ProductAttributeMappings = new List<ProductAttributeMappingDto>();

            foreach (ProductAttributeMapping productAttributeMapping in productAttributeMappings)
            {
                ProductAttributeMappingDto productAttributeMappingDto =
                    PrepareProductAttributeMappingDto(productAttributeMapping);

                if (productAttributeMappingDto != null) productDto.ProductAttributeMappings.Add(productAttributeMappingDto);
            }
        }

        private ProductAttributeValueDto PrepareProductAttributeValueDto(ProductAttributeValue productAttributeValue,
            Product product)
        {
            ProductAttributeValueDto productAttributeValueDto = null;

            if (productAttributeValue != null)
            {
                productAttributeValueDto = productAttributeValue.ToDto();
 
                productAttributeValueDto.Name = _localizationService.GetLocalized(productAttributeValue, x => x.Name, _currentLangaugeId);
                if (productAttributeValue.ImageSquaresPictureId > 0)
                {
                    Picture imageSquaresPicture =
                        _pictureService.GetPictureById(productAttributeValue.ImageSquaresPictureId);
                    ImageDto imageDto = PrepareImageDto(imageSquaresPicture);
                    productAttributeValueDto.ImageSquaresImage = imageDto;
                }

                if (productAttributeValue.PictureId > 0)
                {
                    // make sure that the picture is mapped to the product
                    // This is needed since if you delete the product picture mapping from the nopCommerce administrationthe
                    // then the attribute value is not updated and it will point to a picture that has been deleted
                    ProductPicture productPicture =
                        product.ProductPictures.FirstOrDefault(pp => pp.PictureId == productAttributeValue.PictureId);
                    if (productPicture != null)
                    {
                        productAttributeValueDto.ProductPictureId = productPicture.Id;
                        productAttributeValueDto.PictureId = productPicture.PictureId;
                    }
                }
            }

            return productAttributeValueDto;
        }

        private void PrepareProductImages(IEnumerable<ProductPicture> productPictures, ProductDto productDto)
        {
            if (productDto.Images == null) productDto.Images = new List<ImageMappingDto>();

            productPictures = productPictures.OrderBy(a => a.DisplayOrder);
            // Here we prepare the resulted dto image.
            foreach (ProductPicture productPicture in productPictures)
            {
                ImageDto imageDto = PrepareImageDto(productPicture.Picture);

                if (imageDto != null)
                {
                    var productImageDto = new ImageMappingDto
                    {
                        Id = productPicture.Id,
                        PictureId = productPicture.PictureId,
                        Position = productPicture.DisplayOrder,
                        Src = imageDto.Src,
                        Attachment = imageDto.Attachment
                    };

                    productDto.Images.Add(productImageDto);
                }
            }
        }
    }
}