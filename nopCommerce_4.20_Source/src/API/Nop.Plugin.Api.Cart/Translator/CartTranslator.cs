using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Cart.Dto;
using Nop.Plugin.Api.Common.Converters;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Common.DTOs.Product;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Cart.Translator
{
    public class CartTranslator : ICartTranslator
    {
        private readonly IAclService _aclService;
        private readonly int _currentLanguageId;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;

        public CartTranslator(IProductService productService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPictureService pictureService,
            IProductAttributeService productAttributeService,
            IProductAttributeConverter productAttributeConverter,
            ILocalizationService localizationService,
            IUrlRecordService urlRecordService,
            IProductTagService productTagService, IHttpContextAccessor httpContextAccessor)
        {
            _productService = productService;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _pictureService = pictureService;
            _productAttributeService = productAttributeService;
            _productAttributeConverter = productAttributeConverter;
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            _productTagService = productTagService;
            var headers = httpContextAccessor.HttpContext.Request.Headers;
            if (!headers.ContainsKey("Accept-Language")) return;
            var lan = headers["Accept-Language"];
            _currentLanguageId = lan.ToString() == "en" ? 1 : 2;
        }

        public ShippingOptionDto PrepareShippingOptionItemDto(ShippingOption shippingOption)
        {
            var options = new ShippingOptionDto
            {
                Description = shippingOption.Description,
                Name = shippingOption.Name,
                Rate = shippingOption.Rate,
                ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName
            };
            try
            {
                var fromToDays = shippingOption.Description.Split(':');
                if (fromToDays.Any())
                {
                    options.FromDays = int.Parse(fromToDays.First());
                    options.ToDays = int.Parse(fromToDays.Last());
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return options;
        }


        public ShoppingCartItemDto PrepareShoppingCartItemDto(ShoppingCartItem shoppingCartItem)
        {
            var dto = shoppingCartItem.ToDto();
            dto.ProductDto = PrepareProductDto(shoppingCartItem.Product);
            dto.Attributes = _productAttributeConverter.Parse(shoppingCartItem.AttributesXml);
            return dto;
        }


        public ProductDto PrepareProductDto(Product product)
        {
            var productDto = product.ToDto();

            productDto.Name = _localizationService.GetLocalized(product, x => x.Name, _currentLanguageId);
            productDto.ShortDescription =
                _localizationService.GetLocalized(product, x => x.ShortDescription, _currentLanguageId);
            productDto.FullDescription =
                _localizationService.GetLocalized(product, x => x.FullDescription, _currentLanguageId);

            PrepareProductImages(product.ProductPictures, productDto);
            PrepareProductAttributes(product.ProductAttributeMappings, productDto);
            PrepareProductAttributesCombination(product.ProductAttributeCombinations, productDto);
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

            return productDto;
        }

        private ProductSpecificationAttributeDto PrepareProductSpecificationAttributeDto(
            ProductSpecificationAttribute productSpecificationAttribute)
        {
            return productSpecificationAttribute.ToDto();
        }


        private void PrepareProductSpecificationAttributes(
            IEnumerable<ProductSpecificationAttribute> productSpecificationAttributes, ProductDto productDto)
        {
            if (productDto.ProductSpecificationAttributes == null)
                productDto.ProductSpecificationAttributes = new List<ProductSpecificationAttributeDto>();

            foreach (var productSpecificationAttribute in productSpecificationAttributes)
            {
                var productSpecificationAttributeDto =
                    PrepareProductSpecificationAttributeDto(productSpecificationAttribute);

                if (productSpecificationAttributeDto != null)
                    productDto.ProductSpecificationAttributes.Add(productSpecificationAttributeDto);
            }
        }

        private ImageDto PrepareImageDto(Picture picture)
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

        private ProductAttributeCombinationDto PrepareProductAttributeCombinationDto(
            ProductAttributeCombination productAttributeMapping)
        {
            ProductAttributeCombinationDto productAttributeMappingDto = null;

            if (productAttributeMapping != null)
                productAttributeMappingDto = new ProductAttributeCombinationDto
                {
                    PictureId = productAttributeMapping.PictureId,
                    AllowOutOfStockOrders = productAttributeMapping.AllowOutOfStockOrders,
                    ProductId = productAttributeMapping.ProductId,
                    AttributesXml = productAttributeMapping.AttributesXml,
                    Gtin = productAttributeMapping.Gtin,
                    ManufacturerPartNumber = productAttributeMapping.ManufacturerPartNumber,
                    NotifyAdminForQuantityBelow = productAttributeMapping.NotifyAdminForQuantityBelow,
                    OverriddenPrice = productAttributeMapping.OverriddenPrice,
                    Sku = productAttributeMapping.Sku,
                    StockQuantity = productAttributeMapping.StockQuantity
                };

            return productAttributeMappingDto;
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
                    TextPrompt =
                        _localizationService.GetLocalized(productAttributeMapping, x => x.TextPrompt,
                            _currentLanguageId),
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
            if (productDto.ProductAttributeMappings == null)
                productDto.ProductAttributeMappings = new List<ProductAttributeMappingDto>();

            foreach (var productAttributeMapping in productAttributeMappings)
            {
                var productAttributeMappingDto =
                    PrepareProductAttributeMappingDto(productAttributeMapping);

                if (productAttributeMappingDto != null)
                    productDto.ProductAttributeMappings.Add(productAttributeMappingDto);
            }
        }


        private void PrepareProductAttributesCombination(
            IEnumerable<ProductAttributeCombination> productAttributeCombinations,
            ProductDto productDto)
        {
            productDto.ProductAttributesCombinations = new List<ProductAttributeCombinationDto>();

            foreach (var productAttributeCombination in productAttributeCombinations)
            {
                var productAttributeCombinationDto =
                    PrepareProductAttributeCombinationDto(productAttributeCombination);
                var attributes = _productAttributeConverter.Parse(productAttributeCombinationDto.AttributesXml);
                var attributeValue = attributes.FirstOrDefault(a => a.Id == 26);
                if (attributeValue != null)
                    productAttributeCombinationDto.ProductAttributId = int.Parse(attributeValue.Value);
                productDto.ProductAttributesCombinations.Add(productAttributeCombinationDto);
            }
        }

        private ProductAttributeValueDto PrepareProductAttributeValueDto(ProductAttributeValue productAttributeValue,
            Product product)
        {
            ProductAttributeValueDto productAttributeValueDto = null;

            if (productAttributeValue == null) return productAttributeValueDto;
            productAttributeValueDto = productAttributeValue.ToDto();

            productAttributeValueDto.Name =
                _localizationService.GetLocalized(productAttributeValue, x => x.Name, _currentLanguageId);
            if (productAttributeValue.ImageSquaresPictureId > 0)
            {
                var imageSquaresPicture =
                    _pictureService.GetPictureById(productAttributeValue.ImageSquaresPictureId);
                var imageDto = PrepareImageDto(imageSquaresPicture);
                productAttributeValueDto.ImageSquaresImage = imageDto;
            }

            if (productAttributeValue.PictureId <= 0) return productAttributeValueDto;
            // make sure that the picture is mapped to the product
            // This is needed since if you delete the product picture mapping from the nopCommerce administration the
            // then the attribute value is not updated and it will point to a picture that has been deleted
            var productPicture =
                product.ProductPictures.FirstOrDefault(pp => pp.PictureId == productAttributeValue.PictureId);
            if (productPicture == null) return productAttributeValueDto;
            productAttributeValueDto.ProductPictureId = productPicture.Id;
            productAttributeValueDto.PictureId = productPicture.PictureId;

            return productAttributeValueDto;
        }

        private void PrepareProductImages(IEnumerable<ProductPicture> productPictures, ProductDto productDto)
        {
            if (productDto.Images == null) productDto.Images = new List<ImageMappingDto>();

            productPictures = productPictures.OrderBy(a => a.DisplayOrder);
            // Here we prepare the resulted dto image.
            foreach (var productPicture in productPictures)
            {
                var imageDto = PrepareImageDto(productPicture.Picture);

                if (imageDto == null) continue;
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