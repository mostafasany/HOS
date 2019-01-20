using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.Attributes;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.Controllers;
using Nop.Plugin.Api.Common.Delta;
using Nop.Plugin.Api.Common.DTOs.Errors;
using Nop.Plugin.Api.Common.JSON.ActionResults;
using Nop.Plugin.Api.Common.JSON.Serializers;
using Nop.Plugin.Api.Common.ModelBinders;
using Nop.Plugin.Api.Modules.Category.Service;
using Nop.Plugin.Api.Modules.Product.Service;
using Nop.Plugin.Api.Modules.ProductCategoryMappings.Dto;
using Nop.Plugin.Api.Modules.ProductCategoryMappings.Model;
using Nop.Plugin.Api.Modules.ProductCategoryMappings.Service;
using Nop.Plugin.Api.Modules.ProductCategoryMappings.Translator;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Modules.ProductCategoryMappings
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductCategoryMappingsController : BaseApiController
    {
        private readonly ICategoryApiService _categoryApiService;
        private readonly ICategoryService _categoryService;
        private readonly IProductApiService _productApiService;
        private readonly IProductCategoryMappingsApiService _productCategoryMappingsService;

        public ProductCategoryMappingsController(IProductCategoryMappingsApiService productCategoryMappingsService,
            ICategoryService categoryService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ICategoryApiService categoryApiService,
            IProductApiService productApiService,
            IPictureService pictureService)
            : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _productCategoryMappingsService = productCategoryMappingsService;
            _categoryService = categoryService;
            _categoryApiService = categoryApiService;
            _productApiService = productApiService;
        }

        [HttpPost]
        [Route("/api/product_category_mappings")]
        [ProducesResponseType(typeof(ProductCategoryMappingsRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult CreateProductCategoryMapping([ModelBinder(typeof(JsonModelBinder<ProductCategoryMappingDto>))]
            Delta<ProductCategoryMappingDto> productCategoryDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid) return Error();

            Core.Domain.Catalog.Category category = _categoryApiService.GetCategoryById(productCategoryDelta.Dto.CategoryId.Value);
            if (category == null) return Error(HttpStatusCode.NotFound, "category_id", "not found");

            Core.Domain.Catalog.Product product = _productApiService.GetProductById(productCategoryDelta.Dto.ProductId.Value);
            if (product == null) return Error(HttpStatusCode.NotFound, "product_id", "not found");

            int mappingsCount = _productCategoryMappingsService.GetMappingsCount(product.Id, category.Id);

            if (mappingsCount > 0) return Error(HttpStatusCode.BadRequest, "product_category_mapping", "already exist");

            var newProductCategory = new ProductCategory();
            productCategoryDelta.Merge(newProductCategory);

            //inserting new category
            _categoryService.InsertProductCategory(newProductCategory);

            // Preparing the result dto of the new product category mapping
            ProductCategoryMappingDto newProductCategoryMappingDto = newProductCategory.ToDto();

            var productCategoryMappingsRootObject = new ProductCategoryMappingsRootObject();

            productCategoryMappingsRootObject.ProductCategoryMappingDtos.Add(newProductCategoryMappingDto);

            string json = JsonFieldsSerializer.Serialize(productCategoryMappingsRootObject, string.Empty);

            //activity log 
            CustomerActivityService.InsertActivity("AddNewProductCategoryMapping", LocalizationService.GetResource("ActivityLog.AddNewProductCategoryMapping"), newProductCategory);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/product_category_mappings/{id}")]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult DeleteProductCategoryMapping(int id)
        {
            if (id <= 0) return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            ProductCategory productCategory = _categoryService.GetProductCategoryById(id);

            if (productCategory == null) return Error(HttpStatusCode.NotFound, "product_category_mapping", "not found");

            _categoryService.DeleteProductCategory(productCategory);

            //activity log 
            CustomerActivityService.InsertActivity("DeleteProductCategoryMapping", LocalizationService.GetResource("ActivityLog.DeleteProductCategoryMapping"), productCategory);

            return new RawJsonActionResult("{}");
        }

        /// <summary>
        ///     Retrieve Product-Category mappings by spcified id
        /// </summary>
        /// ///
        /// <param name="id">Id of the Product-Category mapping</param>
        /// <param name="fields">Fields from the Product-Category mapping you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/product_category_mappings/{id}")]
        [ProducesResponseType(typeof(ProductCategoryMappingsRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetMappingById(int id, string fields = "")
        {
            if (id <= 0) return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            ProductCategory mapping = _productCategoryMappingsService.GetById(id);

            if (mapping == null) return Error(HttpStatusCode.NotFound, "product_category_mapping", "not found");

            var productCategoryMappingsRootObject = new ProductCategoryMappingsRootObject();
            productCategoryMappingsRootObject.ProductCategoryMappingDtos.Add(mapping.ToDto());

            string json = JsonFieldsSerializer.Serialize(productCategoryMappingsRootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a list of all Product-Category mappings
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/product_category_mappings")]
        [ProducesResponseType(typeof(ProductCategoryMappingsRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetMappings(ProductCategoryMappingsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit) return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue) return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");

            IList<ProductCategoryMappingDto> mappingsAsDtos =
                _productCategoryMappingsService.GetMappings(parameters.ProductId,
                    parameters.CategoryId,
                    parameters.Limit,
                    parameters.Page,
                    parameters.SinceId).Select(x => x.ToDto()).ToList();

            var productCategoryMappingRootObject = new ProductCategoryMappingsRootObject
            {
                ProductCategoryMappingDtos = mappingsAsDtos
            };

            string json = JsonFieldsSerializer.Serialize(productCategoryMappingRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a count of all Product-Category mappings
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/product_category_mappings/count")]
        [ProducesResponseType(typeof(ProductCategoryMappingsCountRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetMappingsCount(ProductCategoryMappingsCountParametersModel parameters)
        {
            if (parameters.ProductId < 0) return Error(HttpStatusCode.BadRequest, "product_id", "invalid product_id");

            if (parameters.CategoryId < 0) return Error(HttpStatusCode.BadRequest, "category_id", "invalid category_id");

            int mappingsCount = _productCategoryMappingsService.GetMappingsCount(parameters.ProductId,
                parameters.CategoryId);

            var productCategoryMappingsCountRootObject = new ProductCategoryMappingsCountRootObject
            {
                Count = mappingsCount
            };

            return Ok(productCategoryMappingsCountRootObject);
        }

        [HttpPut]
        [Route("/api/product_category_mappings/{id}")]
        [ProducesResponseType(typeof(ProductCategoryMappingsRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        public IActionResult UpdateProductCategoryMapping([ModelBinder(typeof(JsonModelBinder<ProductCategoryMappingDto>))]
            Delta<ProductCategoryMappingDto> productCategoryDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid) return Error();

            if (productCategoryDelta.Dto.CategoryId.HasValue)
            {
                Core.Domain.Catalog.Category category = _categoryApiService.GetCategoryById(productCategoryDelta.Dto.CategoryId.Value);
                if (category == null) return Error(HttpStatusCode.NotFound, "category_id", "not found");
            }

            if (productCategoryDelta.Dto.ProductId.HasValue)
            {
                Core.Domain.Catalog.Product product = _productApiService.GetProductById(productCategoryDelta.Dto.ProductId.Value);
                if (product == null) return Error(HttpStatusCode.NotFound, "product_id", "not found");
            }

            // We do not need to validate the category id, because this will happen in the model binder using the dto validator.
            int updateProductCategoryId = productCategoryDelta.Dto.Id;

            ProductCategory productCategoryEntityToUpdate = _categoryService.GetProductCategoryById(updateProductCategoryId);

            if (productCategoryEntityToUpdate == null) return Error(HttpStatusCode.NotFound, "product_category_mapping", "not found");

            productCategoryDelta.Merge(productCategoryEntityToUpdate);

            _categoryService.UpdateProductCategory(productCategoryEntityToUpdate);

            //activity log
            CustomerActivityService.InsertActivity("UpdateProdutCategoryMapping",
                LocalizationService.GetResource("ActivityLog.UpdateProdutCategoryMapping"), productCategoryEntityToUpdate);

            ProductCategoryMappingDto updatedProductCategoryDto = productCategoryEntityToUpdate.ToDto();

            var productCategoriesRootObject = new ProductCategoryMappingsRootObject();

            productCategoriesRootObject.ProductCategoryMappingDtos.Add(updatedProductCategoryDto);

            string json = JsonFieldsSerializer.Serialize(productCategoriesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }
    }
}