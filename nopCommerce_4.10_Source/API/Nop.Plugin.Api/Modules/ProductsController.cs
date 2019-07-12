using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Plugin.Api.Category.Dto;
using Nop.Plugin.Api.Category.Service;
using Nop.Plugin.Api.Category.Translator;
using Nop.Plugin.Api.Common.Attributes;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.Controllers;
using Nop.Plugin.Api.Common.Delta;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Common.DTOs.Errors;
using Nop.Plugin.Api.Common.DTOs.Product;
using Nop.Plugin.Api.Common.Factories;
using Nop.Plugin.Api.Common.JSON.ActionResults;
using Nop.Plugin.Api.Common.JSON.Serializers;
using Nop.Plugin.Api.Common.ModelBinders;
using Nop.Plugin.Api.Content.Modules.Manufacturer.Translator;
using Nop.Plugin.Api.Product.Modules.Product.Dto;
using Nop.Plugin.Api.Product.Modules.Product.Model;
using Nop.Plugin.Api.Product.Modules.Product.Service;
using Nop.Plugin.Api.Product.Modules.Product.Translator;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Modules
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductsController : BaseApiController
    {
        private readonly IProductTransaltor _dtoHelper;
        private readonly IFactory<Core.Domain.Catalog.Product> _factory;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductApiService _productApiService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ICategoryApiService _categoryApiService;
        private readonly ICategoryTransaltor _categoryTransaltor;
        private readonly IManufacturerTransaltor _manufacturerTransaltor;
        private readonly IWorkContext _workContext;

        public ProductsController(IProductApiService productApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IProductService productService,
            IUrlRecordService urlRecordService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IFactory<Core.Domain.Catalog.Product> factory,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            ICustomerService customerService,
            ICategoryTransaltor categoryTransaltor,
            ICategoryApiService categoryApiService,
            IDiscountService discountService,
            IPictureService pictureService,
            IManufacturerService manufacturerService,
            IProductTagService productTagService,
            IWorkContext workContext,
            IManufacturerTransaltor manufacturerTransaltor,
            IProductAttributeService productAttributeService,
            IProductTransaltor dtoHelper) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _productApiService = productApiService;
            _factory = factory;
            _manufacturerService = manufacturerService;
            _productTagService = productTagService;
            _urlRecordService = urlRecordService;
            _productService = productService;
            _productAttributeService = productAttributeService;
            _dtoHelper = dtoHelper;
            _categoryApiService = categoryApiService;
            _categoryTransaltor = categoryTransaltor;
            _workContext = workContext;
            _manufacturerTransaltor = manufacturerTransaltor;
        }

        [HttpPost]
        [Route("/api/products")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult CreateProduct([ModelBinder(typeof(JsonModelBinder<ProductDto>))]
            Delta<ProductDto> productDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid) return Error();

            // Inserting the new product
            Core.Domain.Catalog.Product product = _factory.Initialize();
            productDelta.Merge(product);

            _productService.InsertProduct(product);

            UpdateProductPictures(product, productDelta.Dto.Images);

            UpdateProductTags(product, productDelta.Dto.Tags);

            UpdateProductManufacturers(product, productDelta.Dto.ManufacturerIds);

            UpdateAssociatedProducts(product, productDelta.Dto.AssociatedProductIds);

            //search engine name
            string seName = _urlRecordService.ValidateSeName(product, productDelta.Dto.SeName, product.Name, true);
            _urlRecordService.SaveSlug(product, seName, 0);

            UpdateAclRoles(product, productDelta.Dto.RoleIds);

            UpdateDiscountMappings(product, productDelta.Dto.DiscountIds);

            UpdateStoreMappings(product, productDelta.Dto.StoreIds);

            _productService.UpdateProduct(product);

            CustomerActivityService.InsertActivity("AddNewProduct",
                LocalizationService.GetResource("ActivityLog.AddNewProduct"), product);

            // Preparing the result dto of the new product
            ProductDto productDto = _dtoHelper.PrepareProductDTO(product);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            string json = JsonFieldsSerializer.Serialize(productsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/products/{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult DeleteProduct(int id)
        {
            if (id <= 0) return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            Core.Domain.Catalog.Product product = _productApiService.GetProductById(id);

            if (product == null) return Error(HttpStatusCode.NotFound, "product", "not found");

            _productService.DeleteProduct(product);

            //activity log
            CustomerActivityService.InsertActivity("DeleteProduct",
                string.Format(LocalizationService.GetResource("ActivityLog.DeleteProduct"), product.Name), product);

            return new RawJsonActionResult("{}");
        }

        /// <summary>
        ///     Retrieve product by spcified id
        /// </summary>
        /// <param name="id">Id of the product</param>
        /// <param name="fields">Fields from the product you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/products/{id}")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProductById(int id, string fields = "")
        {
            if (id <= 0) return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            Core.Domain.Catalog.Product product = _productApiService.GetProductById(id);

            if (product == null) return Error(HttpStatusCode.NotFound, "product", "not found");

            ProductDto productDto = _dtoHelper.PrepareProductDTO(product);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            string json = JsonFieldsSerializer.Serialize(productsRootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a list of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/products")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProducts(ProductsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit) return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue) return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");

            Tuple<IList<Core.Domain.Catalog.Product>, List<ProductsFiltersDto>> tuple = _productApiService.GetProducts(parameters.Ids, parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.UpdatedAtMin,
                parameters.UpdatedAtMax, parameters.Limit, parameters.Page, parameters.SinceId, parameters.CategoryId, parameters.CategorySlug,
                parameters.VendorName, parameters.ManufacturerName,parameters.ManufacturerId, parameters.Keyword, parameters.PublishedStatus);
            IEnumerable<Core.Domain.Catalog.Product> allProducts = tuple.Item1.Where(p => StoreMappingService.Authorize(p));

            IList<ProductDto> productsAsDtos = allProducts.Select(product => _dtoHelper.PrepareProductDTO(product)).ToList();

            var title = "";
            if (parameters.CategoryId.HasValue)
            {
                Core.Domain.Catalog.Category category = _categoryApiService.GetCategoryById(parameters.CategoryId.Value);
                CategoryDto dto = _categoryTransaltor.PrepareCategoryDTO(category);
                title = dto.Name;
            }
            else if (parameters.ManufacturerId.HasValue)
            {
                var manufacturer = _manufacturerService.GetManufacturerById(parameters.ManufacturerId.Value);
                var dto = _manufacturerTransaltor.ConvertToDto(manufacturer);
                title = dto.Name;
            }

            var productsRootObject = new ProductsRootObjectDto
            {
                Products = productsAsDtos,
                Filters = tuple.Item2,
                HeaderTitle = title
            };

            string json = JsonFieldsSerializer.Serialize(productsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a count of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/products/count")]
        [ProducesResponseType(typeof(ProductsCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProductsCount(ProductsCountParametersModel parameters)
        {
            int allProductsCount = _productApiService.GetProductsCount(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.UpdatedAtMin,
                parameters.UpdatedAtMax, parameters.PublishedStatus, parameters.VendorName, parameters.Keyword,
                parameters.CategoryId);

            var productsCountRootObject = new ProductsCountRootObject
            {
                Count = allProductsCount
            };

            return Ok(productsCountRootObject);
        }

        /// <summary>
        ///     Receive a list of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/products/{id}/related")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetRelatedProducts(int id, string fields = "")
        {
            IEnumerable<Core.Domain.Catalog.Product> allProducts = _productApiService.GetRelatedProducts(id)
                .Where(p => StoreMappingService.Authorize(p));

            IList<ProductDto> productsAsDtos = allProducts.Select(product => _dtoHelper.PrepareProductDTO(product)).ToList();

            var productsRootObject = new ProductsRootObjectDto
            {
                Products = productsAsDtos
            };

            string json = JsonFieldsSerializer.Serialize(productsRootObject, fields);

            return new RawJsonActionResult(json);
        }


        /// <summary>
        ///     Receive a list of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/products/{customerId}/productReview")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetReviewedProducts(int customerId,bool approved, int pageIndex, int pageSize)
        {
            IEnumerable<ProductReview> allProducts = _productService.GetAllProductReviews(customerId, approved, pageIndex: pageIndex, pageSize: pageSize);

            IList<ProductReviewDto> productsAsDtos = allProducts.Select(product => _dtoHelper.PrepareProductReviewDTO(product)).ToList();

            var productsRootObject = new ProductsReviewRootObjectDto
            {
                ProductsReview = productsAsDtos
            };

            string json = JsonFieldsSerializer.Serialize(productsRootObject, "");

            return new RawJsonActionResult(json);
        }



        /// <summary>
        ///     Receive a list of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpDelete]
        [Route("/api/products/{customerId}/productReview/{id}")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult DeleteReviewedProducts(int id)
        {
            try
            {
                var review = _productService.GetProductReviewById(id);
                _productService.DeleteProductReview(review);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }           
        }

        [HttpPut]
        [Route("/api/products/{id}")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public IActionResult UpdateProduct([ModelBinder(typeof(JsonModelBinder<ProductDto>))]
            Delta<ProductDto> productDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid) return Error();

            Core.Domain.Catalog.Product product = _productApiService.GetProductById(productDelta.Dto.Id);

            if (product == null) return Error(HttpStatusCode.NotFound, "product", "not found");

            productDelta.Merge(product);

            product.UpdatedOnUtc = DateTime.UtcNow;
            _productService.UpdateProduct(product);

            UpdateProductAttributes(product, productDelta);

            UpdateProductPictures(product, productDelta.Dto.Images);

            UpdateProductTags(product, productDelta.Dto.Tags);

            UpdateProductManufacturers(product, productDelta.Dto.ManufacturerIds);

            UpdateAssociatedProducts(product, productDelta.Dto.AssociatedProductIds);

            // Update the SeName if specified
            if (productDelta.Dto.SeName != null)
            {
                string seName = _urlRecordService.ValidateSeName(product, productDelta.Dto.SeName, product.Name, true);
                _urlRecordService.SaveSlug(product, seName, 0);
            }

            UpdateDiscountMappings(product, productDelta.Dto.DiscountIds);

            UpdateStoreMappings(product, productDelta.Dto.StoreIds);

            UpdateAclRoles(product, productDelta.Dto.RoleIds);

            _productService.UpdateProduct(product);

            CustomerActivityService.InsertActivity("UpdateProduct",
                LocalizationService.GetResource("ActivityLog.UpdateProduct"), product);

            // Preparing the result dto of the new product
            ProductDto productDto = _dtoHelper.PrepareProductDTO(product);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            string json = JsonFieldsSerializer.Serialize(productsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }


        [HttpPut]
        [Route("/api/products/{id}/rate")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        public IActionResult RateProduct(int id, [ModelBinder(typeof(JsonModelBinder<ProductsRatingParametersModel>))]
            Delta<ProductsRatingParametersModel> productDelta)
        {
            var parameters = productDelta.Dto;
            var success = _productApiService.RateProduct(id, _workContext.CurrentCustomer.Id, parameters.Rating, parameters.StoreId,
                parameters.ReviewText, parameters.Title);
            if (success)
            {
                return Ok();
            }
            return BadRequest();
        }

        private void UpdateAssociatedProducts(Core.Domain.Catalog.Product product, List<int> passedAssociatedProductIds)
        {
            // If no associated products specified then there is nothing to map 
            if (passedAssociatedProductIds == null)
                return;

            IEnumerable<Core.Domain.Catalog.Product> noLongerAssociatedProducts =
                _productService.GetAssociatedProducts(product.Id, showHidden: true)
                    .Where(p => !passedAssociatedProductIds.Contains(p.Id));

            // update all products that are no longer associated with our product
            foreach (Core.Domain.Catalog.Product noLongerAssocuatedProduct in noLongerAssociatedProducts)
            {
                noLongerAssocuatedProduct.ParentGroupedProductId = 0;
                _productService.UpdateProduct(noLongerAssocuatedProduct);
            }

            IList<Core.Domain.Catalog.Product> newAssociatedProducts = _productService.GetProductsByIds(passedAssociatedProductIds.ToArray());
            foreach (Core.Domain.Catalog.Product newAssociatedProduct in newAssociatedProducts)
            {
                newAssociatedProduct.ParentGroupedProductId = product.Id;
                _productService.UpdateProduct(newAssociatedProduct);
            }
        }

        private void UpdateDiscountMappings(Core.Domain.Catalog.Product product, List<int> passedDiscountIds)
        {
            if (passedDiscountIds == null)
                return;

            IList<Discount> allDiscounts = DiscountService.GetAllDiscounts(DiscountType.AssignedToSkus, showHidden: true);

            foreach (Discount discount in allDiscounts)
                if (passedDiscountIds.Contains(discount.Id))
                {
                    //new discount
                    if (product.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                        product.AppliedDiscounts.Add(discount);
                }
                else
                {
                    //remove discount
                    if (product.AppliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                        product.AppliedDiscounts.Remove(discount);
                }

            _productService.UpdateProduct(product);
            _productService.UpdateHasDiscountsApplied(product);
        }

        private void UpdateProductAttributes(Core.Domain.Catalog.Product entityToUpdate, Delta<ProductDto> productDtoDelta)
        {
            // If no product attribute mappings are specified means we don't have to update anything
            if (productDtoDelta.Dto.ProductAttributeMappings == null)
                return;

            // delete unused product attribute mappings
            IEnumerable<int> toBeUpdatedIds = productDtoDelta.Dto.ProductAttributeMappings.Where(y => y.Id != 0).Select(x => x.Id);

            List<ProductAttributeMapping> unusedProductAttributeMappings = entityToUpdate.ProductAttributeMappings.Where(x => !toBeUpdatedIds.Contains(x.Id)).ToList();

            foreach (ProductAttributeMapping unusedProductAttributeMapping in unusedProductAttributeMappings) _productAttributeService.DeleteProductAttributeMapping(unusedProductAttributeMapping);

            foreach (ProductAttributeMappingDto productAttributeMappingDto in productDtoDelta.Dto.ProductAttributeMappings)
                if (productAttributeMappingDto.Id > 0)
                {
                    // update existing product attribute mapping
                    ProductAttributeMapping productAttributeMappingToUpdate = entityToUpdate.ProductAttributeMappings.FirstOrDefault(x => x.Id == productAttributeMappingDto.Id);
                    if (productAttributeMappingToUpdate != null)
                    {
                        productDtoDelta.Merge(productAttributeMappingDto, productAttributeMappingToUpdate, false);

                        _productAttributeService.UpdateProductAttributeMapping(productAttributeMappingToUpdate);

                        UpdateProductAttributeValues(productAttributeMappingDto, productDtoDelta);
                    }
                }
                else
                {
                    var newProductAttributeMapping = new ProductAttributeMapping { ProductId = entityToUpdate.Id };

                    productDtoDelta.Merge(productAttributeMappingDto, newProductAttributeMapping);

                    // add new product attribute
                    _productAttributeService.InsertProductAttributeMapping(newProductAttributeMapping);
                }
        }

        private void UpdateProductAttributeValues(ProductAttributeMappingDto productAttributeMappingDto, Delta<ProductDto> productDtoDelta)
        {
            // If no product attribute values are specified means we don't have to update anything
            if (productAttributeMappingDto.ProductAttributeValues == null)
                return;

            // delete unused product attribute values
            IEnumerable<int> toBeUpdatedIds = productAttributeMappingDto.ProductAttributeValues.Where(y => y.Id != 0).Select(x => x.Id);

            List<ProductAttributeValue> unusedProductAttributeValues =
                _productAttributeService.GetProductAttributeValues(productAttributeMappingDto.Id).Where(x => !toBeUpdatedIds.Contains(x.Id)).ToList();

            foreach (ProductAttributeValue unusedProductAttributeValue in unusedProductAttributeValues) _productAttributeService.DeleteProductAttributeValue(unusedProductAttributeValue);

            foreach (ProductAttributeValueDto productAttributeValueDto in productAttributeMappingDto.ProductAttributeValues)
                if (productAttributeValueDto.Id > 0)
                {
                    // update existing product attribute mapping
                    ProductAttributeValue productAttributeValueToUpdate =
                        _productAttributeService.GetProductAttributeValueById(productAttributeValueDto.Id);
                    if (productAttributeValueToUpdate != null)
                    {
                        productDtoDelta.Merge(productAttributeValueDto, productAttributeValueToUpdate, false);

                        _productAttributeService.UpdateProductAttributeValue(productAttributeValueToUpdate);
                    }
                }
                else
                {
                    var newProductAttributeValue = new ProductAttributeValue();
                    productDtoDelta.Merge(productAttributeValueDto, newProductAttributeValue);

                    newProductAttributeValue.ProductAttributeMappingId = productAttributeMappingDto.Id;
                    // add new product attribute value
                    _productAttributeService.InsertProductAttributeValue(newProductAttributeValue);
                }
        }

        private void UpdateProductManufacturers(Core.Domain.Catalog.Product product, List<int> passedManufacturerIds)
        {
            // If no manufacturers specified then there is nothing to map 
            if (passedManufacturerIds == null)
                return;

            List<ProductManufacturer> unusedProductManufacturers = product.ProductManufacturers.Where(x => !passedManufacturerIds.Contains(x.ManufacturerId)).ToList();

            // remove all manufacturers that are not passed
            foreach (ProductManufacturer unusedProductManufacturer in unusedProductManufacturers) _manufacturerService.DeleteProductManufacturer(unusedProductManufacturer);

            foreach (int passedManufacturerId in passedManufacturerIds)
                // not part of existing manufacturers so we will create a new one
                if (product.ProductManufacturers.All(x => x.ManufacturerId != passedManufacturerId))
                {
                    // if manufacturer does not exist we simply ignore it, otherwise add it to the product
                    Manufacturer manufacturer = _manufacturerService.GetManufacturerById(passedManufacturerId);
                    if (manufacturer != null)
                        _manufacturerService.InsertProductManufacturer(new ProductManufacturer { ProductId = product.Id, ManufacturerId = manufacturer.Id });
                }
        }

        private void UpdateProductPictures(Core.Domain.Catalog.Product entityToUpdate, List<ImageMappingDto> setPictures)
        {
            // If no pictures are specified means we don't have to update anything
            if (setPictures == null)
                return;

            // delete unused product pictures
            List<ProductPicture> unusedProductPictures = entityToUpdate.ProductPictures.Where(x => setPictures.All(y => y.Id != x.Id)).ToList();
            foreach (ProductPicture unusedProductPicture in unusedProductPictures)
            {
                Picture picture = PictureService.GetPictureById(unusedProductPicture.PictureId);
                if (picture == null)
                    throw new ArgumentException("No picture found with the specified id");
                PictureService.DeletePicture(picture);
            }

            foreach (ImageMappingDto imageDto in setPictures)
                if (imageDto.Id > 0)
                {
                    // update existing product picture
                    ProductPicture productPictureToUpdate = entityToUpdate.ProductPictures.FirstOrDefault(x => x.Id == imageDto.Id);
                    if (productPictureToUpdate != null && imageDto.Position > 0)
                    {
                        productPictureToUpdate.DisplayOrder = imageDto.Position;
                        _productService.UpdateProductPicture(productPictureToUpdate);
                    }
                }
                else
                {
                    // add new product picture
                    Picture newPicture = PictureService.InsertPicture(imageDto.Binary, imageDto.MimeType, string.Empty);
                    _productService.InsertProductPicture(new ProductPicture
                    {
                        PictureId = newPicture.Id,
                        ProductId = entityToUpdate.Id,
                        DisplayOrder = imageDto.Position
                    });
                }
        }

        private void UpdateProductTags(Core.Domain.Catalog.Product product, IReadOnlyCollection<string> productTags)
        {
            if (productTags == null)
                return;

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //Copied from UpdateProductTags method of ProductTagService
            //product tags
            IList<ProductTag> existingProductTags = _productTagService.GetAllProductTagsByProductId(product.Id);
            var productTagsToRemove = new List<ProductTag>();
            foreach (ProductTag existingProductTag in existingProductTags)
            {
                var found = false;
                foreach (string newProductTag in productTags)
                {
                    if (!existingProductTag.Name.Equals(newProductTag, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    found = true;
                    break;
                }

                if (!found) productTagsToRemove.Add(existingProductTag);
            }

            foreach (ProductTag productTag in productTagsToRemove)
            {
                //product.ProductTags.Remove(productTag);
                product.ProductProductTagMappings
                    .Remove(product.ProductProductTagMappings.FirstOrDefault(mapping => mapping.ProductTagId == productTag.Id));
                _productService.UpdateProduct(product);
            }

            foreach (string productTagName in productTags)
            {
                ProductTag productTag;
                ProductTag productTag2 = _productTagService.GetProductTagByName(productTagName);
                if (productTag2 == null)
                {
                    //add new product tag
                    productTag = new ProductTag
                    {
                        Name = productTagName
                    };
                    _productTagService.InsertProductTag(productTag);
                }
                else
                {
                    productTag = productTag2;
                }

                if (!_productService.ProductTagExists(product, productTag.Id))
                {
                    product.ProductProductTagMappings.Add(new ProductProductTagMapping { ProductTag = productTag });
                    _productService.UpdateProduct(product);
                }

                string seName = _urlRecordService.ValidateSeName(productTag, string.Empty, productTag.Name, true);
                _urlRecordService.SaveSlug(productTag, seName, 0);
            }
        }
    }
}