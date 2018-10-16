using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Articles;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DTOs.Articles;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.Models.ArticlesParameters;
using Nop.Plugin.Api.Services;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ArticlesController : BaseApiController
    {
        private readonly IArticleApiService _articleApiService;
        private readonly IDTOHelper _dtoHelper;

        public ArticlesController(IArticleApiService articleApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            IAclService aclService,
            ICustomerService customerService,
            IDTOHelper dtoHelper) : base(jsonFieldsSerializer, aclService, customerService,
            storeMappingService, storeService, discountService, customerActivityService,
            localizationService, pictureService)
        {
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
        [ProducesResponseType(typeof(ArticlesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetArticleById(int id, string fields = "")
        {
            if (id <= 0) return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            FNS_Article article = _articleApiService.GetArticleById(id);

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
        [ProducesResponseType(typeof(ArticlesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetArticles(ArticlelsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit) return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue) return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");

            IEnumerable<FNS_Article> allArticles = _articleApiService.GetArticles(parameters.Ids, parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.UpdatedAtMin,
                parameters.UpdatedAtMax, parameters.Limit, parameters.Page, parameters.SinceId, parameters.CategoryId, parameters.GroupId, parameters.Keyword, parameters.Tag,
                parameters.PublishedStatus);

            IList<ArticlesDto> articlesAsDtos = allArticles.Select(article =>
                _dtoHelper.PrepateArticleDto(article)).ToList();

            var artcilesRootObject = new ArticlesRootObject
            {
                Articles = articlesAsDtos
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
        [ProducesResponseType(typeof(CategoriesCountRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetArticlesCount(ArticlesCountParametersModel parameters)
        {
            int allArticlesCount = _articleApiService.GetArticlesCount(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.UpdatedAtMin,
                parameters.UpdatedAtMax, parameters.PublishedStatus,
                parameters.CategoryId, parameters.GroupId);

            var articlesCountRootObject = new CategoriesCountRootObject
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

            var artcilesRootObject = new ArticlesGroupsRootObject()
            {
                ArticlesGroups = articlesAsDtos
            };

            string json = JsonFieldsSerializer.Serialize(artcilesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }
    }
}