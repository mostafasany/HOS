﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.Api.Common.Attributes;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.Controllers;
using Nop.Plugin.Api.Common.Delta;
using Nop.Plugin.Api.Common.DTOs.Errors;
using Nop.Plugin.Api.Common.Factories;
using Nop.Plugin.Api.Common.JSON.ActionResults;
using Nop.Plugin.Api.Common.JSON.Serializers;
using Nop.Plugin.Api.Common.ModelBinders;
using Nop.Plugin.Api.Modules.Category.Dto;
using Nop.Plugin.Api.Modules.Category.Model;
using Nop.Plugin.Api.Modules.Category.Service;
using Nop.Plugin.Api.Modules.Category.Translator;
using Nop.Plugin.Api.Modules.Picture.Dto;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Modules.Category
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CategoriesController : BaseApiController
    {
        private readonly ICategoryApiService _categoryApiService;
        private readonly ICategoryService _categoryService;
        private readonly ICategoryTransaltor _dtoHelper;
        private readonly IFactory<Core.Domain.Catalog.Category> _factory;
        private readonly IUrlRecordService _urlRecordService;

        public CategoriesController(ICategoryApiService categoryApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            ICategoryService categoryService,
            IUrlRecordService urlRecordService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            IAclService aclService,
            ICustomerService customerService,
            IFactory<Core.Domain.Catalog.Category> factory,
            ICategoryTransaltor dtoHelper) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _categoryApiService = categoryApiService;
            _categoryService = categoryService;
            _urlRecordService = urlRecordService;
            _factory = factory;
            _dtoHelper = dtoHelper;
        }

        [HttpPost]
        [Route("/api/categories")]
        [ProducesResponseType(typeof(CategoriesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public IActionResult CreateCategory([ModelBinder(typeof(JsonModelBinder<CategoryDto>))]
            Delta<CategoryDto> categoryDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid) return Error();

            //If the validation has passed the categoryDelta object won't be null for sure so we don't need to check for this.

            Core.Domain.Media.Picture insertedPicture = null;

            // We need to insert the picture before the category so we can obtain the picture id and map it to the category.
            if (categoryDelta.Dto.Image != null && categoryDelta.Dto.Image.Binary != null) insertedPicture = PictureService.InsertPicture(categoryDelta.Dto.Image.Binary, categoryDelta.Dto.Image.MimeType, string.Empty);

            // Inserting the new category
            Core.Domain.Catalog.Category category = _factory.Initialize();
            categoryDelta.Merge(category);

            if (insertedPicture != null) category.PictureId = insertedPicture.Id;

            _categoryService.InsertCategory(category);


            UpdateAclRoles(category, categoryDelta.Dto.RoleIds);

            UpdateDiscounts(category, categoryDelta.Dto.DiscountIds);

            UpdateStoreMappings(category, categoryDelta.Dto.StoreIds);

            //search engine name
            if (categoryDelta.Dto.SeName != null)
            {
                string seName = _urlRecordService.ValidateSeName(category, categoryDelta.Dto.SeName, category.Name, true);
                _urlRecordService.SaveSlug(category, seName, 0);
            }

            CustomerActivityService.InsertActivity("AddNewCategory",
                LocalizationService.GetResource("ActivityLog.AddNewCategory"), category);

            // Preparing the result dto of the new category
            CategoryDto newCategoryDto = _dtoHelper.PrepareCategoryDTO(category);

            var categoriesRootObject = new CategoriesRootObject();

            categoriesRootObject.Categories.Add(newCategoryDto);

            string json = JsonFieldsSerializer.Serialize(categoriesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [Route("/api/categories/{id}")]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult DeleteCategory(int id)
        {
            if (id <= 0) return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            Core.Domain.Catalog.Category categoryToDelete = _categoryApiService.GetCategoryById(id);

            if (categoryToDelete == null) return Error(HttpStatusCode.NotFound, "category", "category not found");

            _categoryService.DeleteCategory(categoryToDelete);

            //activity log
            CustomerActivityService.InsertActivity("DeleteCategory", LocalizationService.GetResource("ActivityLog.DeleteCategory"), categoryToDelete);

            return new RawJsonActionResult("{}");
        }

        /// <summary>
        ///     Receive a list of all Categories
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/categories")]
        [ProducesResponseType(typeof(CategoriesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetCategories(CategoriesParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit) return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue) return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");

            IEnumerable<Core.Domain.Catalog.Category> allCategories = _categoryApiService.GetCategories(parameters.Ids, parameters.CreatedAtMin, parameters.CreatedAtMax,
                    parameters.UpdatedAtMin, parameters.UpdatedAtMax,
                    parameters.Limit, parameters.Page, parameters.SinceId,
                    parameters.ProductId, parameters.ParentId, parameters.PublishedStatus)
                .Where(c => StoreMappingService.Authorize(c));

            IList<CategoryDto> categoriesAsDtos = allCategories.Select(category =>
            {
                return _dtoHelper.PrepareCategoryDTO(category);
            }).ToList();

            var categoriesRootObject = new CategoriesRootObject
            {
                Categories = categoriesAsDtos
            };

            string json = JsonFieldsSerializer.Serialize(categoriesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a count of all Categories
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/categories/count")]
        [ProducesResponseType(typeof(CategoriesCountRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetCategoriesCount(CategoriesCountParametersModel parameters)
        {
            int allCategoriesCount = _categoryApiService.GetCategoriesCount(parameters.CreatedAtMin, parameters.CreatedAtMax,
                parameters.UpdatedAtMin, parameters.UpdatedAtMax,
                parameters.PublishedStatus, parameters.ProductId);

            var categoriesCountRootObject = new CategoriesCountRootObject
            {
                Count = allCategoriesCount
            };

            return Ok(categoriesCountRootObject);
        }

        /// <summary>
        ///     Retrieve category by spcified id
        /// </summary>
        /// <param name="id">Id of the category</param>
        /// <param name="fields">Fields from the category you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/categories/{id}")]
        [ProducesResponseType(typeof(CategoriesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetCategoryById(int id, string fields = "")
        {
            if (id <= 0) return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            Core.Domain.Catalog.Category category = _categoryApiService.GetCategoryById(id);

            if (category == null) return Error(HttpStatusCode.NotFound, "category", "category not found");

            CategoryDto categoryDto = _dtoHelper.PrepareCategoryDTO(category);

            var categoriesRootObject = new CategoriesRootObject();

            categoriesRootObject.Categories.Add(categoryDto);

            string json = JsonFieldsSerializer.Serialize(categoriesRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/categories/{id}")]
        [ProducesResponseType(typeof(CategoriesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        public IActionResult UpdateCategory(
            [ModelBinder(typeof(JsonModelBinder<CategoryDto>))]
            Delta<CategoryDto> categoryDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid) return Error();

            Core.Domain.Catalog.Category category = _categoryApiService.GetCategoryById(categoryDelta.Dto.Id);

            if (category == null) return Error(HttpStatusCode.NotFound, "category", "category not found");

            categoryDelta.Merge(category);

            category.UpdatedOnUtc = DateTime.UtcNow;

            _categoryService.UpdateCategory(category);

            UpdatePicture(category, categoryDelta.Dto.Image);

            UpdateAclRoles(category, categoryDelta.Dto.RoleIds);

            UpdateDiscounts(category, categoryDelta.Dto.DiscountIds);

            UpdateStoreMappings(category, categoryDelta.Dto.StoreIds);

            //search engine name
            if (categoryDelta.Dto.SeName != null)
            {
                string seName = _urlRecordService.ValidateSeName(category, categoryDelta.Dto.SeName, category.Name, true);
                _urlRecordService.SaveSlug(category, seName, 0);
            }

            _categoryService.UpdateCategory(category);

            CustomerActivityService.InsertActivity("UpdateCategory",
                LocalizationService.GetResource("ActivityLog.UpdateCategory"), category);

            CategoryDto categoryDto = _dtoHelper.PrepareCategoryDTO(category);

            var categoriesRootObject = new CategoriesRootObject();

            categoriesRootObject.Categories.Add(categoryDto);

            string json = JsonFieldsSerializer.Serialize(categoriesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        private void UpdateDiscounts(Core.Domain.Catalog.Category category, List<int> passedDiscountIds)
        {
            if (passedDiscountIds == null)
                return;

            IList<Core.Domain.Discounts.Discount> allDiscounts = DiscountService.GetAllDiscounts(DiscountType.AssignedToCategories, showHidden: true);
            foreach (Core.Domain.Discounts.Discount discount in allDiscounts)
                if (passedDiscountIds.Contains(discount.Id))
                {
                    //new discount
                    if (category.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                        category.AppliedDiscounts.Add(discount);
                }
                else
                {
                    //remove discount
                    if (category.AppliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                        category.AppliedDiscounts.Remove(discount);
                }

            _categoryService.UpdateCategory(category);
        }

        private void UpdatePicture(Core.Domain.Catalog.Category categoryEntityToUpdate, ImageDto imageDto)
        {
            // no image specified then do nothing
            if (imageDto == null)
                return;

            Core.Domain.Media.Picture updatedPicture;
            Core.Domain.Media.Picture currentCategoryPicture = PictureService.GetPictureById(categoryEntityToUpdate.PictureId);

            // when there is a picture set for the category
            if (currentCategoryPicture != null)
            {
                PictureService.DeletePicture(currentCategoryPicture);

                // When the image attachment is null or empty.
                if (imageDto.Binary == null)
                {
                    categoryEntityToUpdate.PictureId = 0;
                }
                else
                {
                    updatedPicture = PictureService.InsertPicture(imageDto.Binary, imageDto.MimeType, string.Empty);
                    categoryEntityToUpdate.PictureId = updatedPicture.Id;
                }
            }
            // when there isn't a picture set for the category
            else
            {
                if (imageDto.Binary != null)
                {
                    updatedPicture = PictureService.InsertPicture(imageDto.Binary, imageDto.MimeType, string.Empty);
                    categoryEntityToUpdate.PictureId = updatedPicture.Id;
                }
            }
        }
    }
}