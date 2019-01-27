﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.Converters;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Modules.Product.Dto;
using Nop.Plugin.Api.Modules.SpecificationAttributes.Dto;
using Nop.Plugin.Api.Modules.SpecificationAttributes.Translator;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Modules.Product.Translator
{
    public class ProductTransaltor : IProductTransaltor
    {
        private readonly IAclService _aclService;

        private readonly int _currentLangaugeId;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;

        public ProductTransaltor(IProductService productService,
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

        public ProductDto PrepareProductDTO(Core.Domain.Catalog.Product product)
        {
            ProductDto productDto = product.ToDto();

            productDto.Name = _localizationService.GetLocalized(product, x => x.Name, _currentLangaugeId);
            productDto.ShortDescription = _localizationService.GetLocalized(product, x => x.ShortDescription, _currentLangaugeId);
            productDto.FullDescription = _localizationService.GetLocalized(product, x => x.FullDescription, _currentLangaugeId);

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

        public ProductSpecificationAttributeDto PrepareProductSpecificationAttributeDto(ProductSpecificationAttribute productSpecificationAttribute) => productSpecificationAttribute.ToDto();


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

        protected ImageDto PrepareImageDto(Core.Domain.Media.Picture picture)
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


        private void PrepareProductAttributesCombination(IEnumerable<ProductAttributeCombination> productAttributeCombinations,
            ProductDto productDto)
        {
            productDto.ProductAttributesCombinations = new List<ProductAttributeCombinationDto>();

            foreach (ProductAttributeCombination productAttributeCombination in productAttributeCombinations)
            {
                ProductAttributeCombinationDto productAttributeComnatbionDto =
                    PrepareProductAttributeCombinationDto(productAttributeCombination);
                List<ProductItemAttributeDto> attributes = _productAttributeConverter.Parse(productAttributeComnatbionDto.AttributesXml);
                ProductItemAttributeDto attributeValue = attributes.FirstOrDefault(a => a.Id == 26);
                if (attributeValue != null) productAttributeComnatbionDto.ProductAttributId = int.Parse(attributeValue.Value);
                productDto.ProductAttributesCombinations.Add(productAttributeComnatbionDto);
            }
        }

        private ProductAttributeValueDto PrepareProductAttributeValueDto(ProductAttributeValue productAttributeValue,
            Core.Domain.Catalog.Product product)
        {
            ProductAttributeValueDto productAttributeValueDto = null;

            if (productAttributeValue != null)
            {
                productAttributeValueDto = productAttributeValue.ToDto();

                productAttributeValueDto.Name = _localizationService.GetLocalized(productAttributeValue, x => x.Name, _currentLangaugeId);
                if (productAttributeValue.ImageSquaresPictureId > 0)
                {
                    Core.Domain.Media.Picture imageSquaresPicture =
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