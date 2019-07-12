using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Articles;
using Nop.Plugin.Api.Article.Dto;
using Nop.Plugin.Api.Article.Model;
using Nop.Plugin.Api.Article.Service;
using Nop.Plugin.Api.Article.Translator;
using Nop.Plugin.Api.Category.Dto;
using Nop.Plugin.Api.Category.Service;
using Nop.Plugin.Api.Category.Translator;
using Nop.Plugin.Api.Common.Attributes;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.Controllers;
using Nop.Plugin.Api.Common.DTOs.Errors;
using Nop.Plugin.Api.Common.JSON.ActionResults;
using Nop.Plugin.Api.Common.JSON.Serializers;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Modules
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ArticlesController : BaseApiController
    {
        private readonly IArticleApiService _articleApiService;
        private readonly ICategoryApiService _categoryApiService;
        private readonly ICategoryTransaltor _categoryTransaltor;
        private readonly IArticleTransaltor _dtoHelper;

        public ArticlesController(IArticleApiService articleApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            IAclService aclService,
            ICategoryTransaltor categoryTransaltor,
            ICategoryApiService categoryApiService,
            ICustomerService customerService,
            IArticleTransaltor dtoHelper) : base(jsonFieldsSerializer, aclService, customerService,
            storeMappingService, storeService, discountService, customerActivityService,
            localizationService, pictureService)
        {
            _categoryTransaltor = categoryTransaltor;
            _categoryApiService = categoryApiService;
            _articleApiService = articleApiService;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        ///     Retrieve article by spcified id
        /// </summary>
        /// <param name="id">Id of the article</param>
        /// <param name="fields">Fields from the article you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/articles/{id}")]
        [ProducesResponseType(typeof(ArticlesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetArticleById(int id, string fields = "")
        {
            if (id <= 0) return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            Core.Domain.Articles.Article article = _articleApiService.GetArticleById(id);

            if (article == null) return Error(HttpStatusCode.NotFound, "article", "article not found");

            ArticlesDto articleDto = _dtoHelper.PrepateArticleDto(article);

            var articlesRootObject = new ArticlesRootObject();

            articlesRootObject.Articles.Add(articleDto);

            string json = JsonFieldsSerializer.Serialize(articlesRootObject, fields);

            return new RawJsonActionResult(json);
        }


        /// <summary>
        ///     Receive a list of all Articles
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/articles")]
        [ProducesResponseType(typeof(ArticlesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetArticles(ArticlelsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit) return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue) return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");

            Tuple<IList<Core.Domain.Articles.Article>, List<ArticlesFilterDto>> tuple = _articleApiService.GetArticles(parameters.Ids, parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.UpdatedAtMin,
                parameters.UpdatedAtMax, parameters.Limit, parameters.Page, parameters.SinceId, parameters.CategoryId, parameters.GroupId, parameters.Keyword, parameters.Tag,
                parameters.PublishedStatus);

            var allArticles = tuple.Item1;

            var title = "";
            if (parameters.CategoryId.HasValue)
            {
                Core.Domain.Catalog.Category category = _categoryApiService.GetCategoryById(parameters.CategoryId.Value);
                CategoryDto dto = _categoryTransaltor.PrepareCategoryDTO(category);
                title = dto.Name;
            }
            else if (!string.IsNullOrEmpty(parameters.Tag))
            {
                title = parameters.Tag;
            }

            IList<ArticlesDto> articlesAsDtos = allArticles.Select(article =>
                _dtoHelper.PrepateArticleDto(article)).ToList();

            var artcilesRootObject = new ArticlesRootObject
            {
                Articles = articlesAsDtos,
                HeaderTitle = title,
                Filters = tuple.Item2,
            };

            string json = JsonFieldsSerializer.Serialize(artcilesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a count of all Articles
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/articles/count")]
        [ProducesResponseType(typeof(ArticlesCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetArticlesCount(ArticlesCountParametersModel parameters)
        {
            int allArticlesCount = _articleApiService.GetArticlesCount(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.UpdatedAtMin,
                parameters.UpdatedAtMax, parameters.PublishedStatus,
                parameters.CategoryId, parameters.GroupId);

            var articlesCountRootObject = new ArticlesCountRootObject
            {
                Count = allArticlesCount
            };

            return Ok(articlesCountRootObject);
        }


        /// <summary>
        ///     Receive a list of all Articles
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/articles/groups")]
        [ProducesResponseType(typeof(ArticlesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetArticlesGroups()
        {
            IEnumerable<FNS_ArticleGroup> allArticles = _articleApiService.GetArticlesGroups();

            IList<ArticleGroupDto> articlesAsDtos = allArticles.Select(article =>
                _dtoHelper.PrepateArticleGroupDto(article)).ToList();

            var artcilesRootObject = new ArticlesGroupsRootObject
            {
                ArticlesGroups = articlesAsDtos
            };

            string json = JsonFieldsSerializer.Serialize(artcilesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }


        /// <summary>
        ///     Receive a list of all Articles
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/articles/{id}/recommended")]
        [ProducesResponseType(typeof(ArticlesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetRecommendedArticles(int id, ArticlelsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit) return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue) return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");

            IEnumerable<Core.Domain.Articles.Article> allArticles = _articleApiService.GetArticlesSimilarByTag(id, parameters.Limit, parameters.Page, parameters.SinceId);

            IList<ArticlesDto> articlesAsDtos = allArticles.Select(article =>
                _dtoHelper.PrepateArticleDto(article)).ToList();

            var artcilesRootObject = new ArticlesRootObject
            {
                Articles = articlesAsDtos
            };

            string json = JsonFieldsSerializer.Serialize(artcilesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }
    }
}