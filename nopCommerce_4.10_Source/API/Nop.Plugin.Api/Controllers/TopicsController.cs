﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Topics;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.DTOs.Topics;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.Models.CategoriesParameters;
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
    public class TopicsController : BaseApiController
    {
        private readonly IDTOHelper _dtoHelper;
        private readonly ITopicApiService _topicApiService;

        public TopicsController(ITopicApiService topicApiService,
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
            _topicApiService = topicApiService;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        ///     Retrieve topic by spcified id
        /// </summary>
        /// <param name="id">Id of the topic</param>
        /// <param name="fields">Fields from the topic you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/topics/{id}")]
        [ProducesResponseType(typeof(TopicsRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetTopicById(int id, string fields = "")
        {
            if (id <= 0) return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            Topic topic = _topicApiService.GetTopicById(id);

            if (topic == null) return Error(HttpStatusCode.NotFound, "topic", "topic not found");

            TopicDto topicDto = _dtoHelper.PrepateTopicDto(topic);

            var topicsRootObject = new TopicsRootObject();

            topicsRootObject.Topics.Add(topicDto);

            string json = JsonFieldsSerializer.Serialize(topicsRootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a list of all Topics
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/topics")]
        [ProducesResponseType(typeof(TopicsRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetTopics(CategoriesParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit) return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue) return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");

            IEnumerable<Topic> allTopics = _topicApiService.GetTopics(parameters.Ids, parameters.Limit, parameters.Page, parameters.SinceId,
                    parameters.PublishedStatus)
                .Where(c => StoreMappingService.Authorize(c));

            IList<TopicDto> topicsAsDtos = allTopics.Select(topic =>
                _dtoHelper.PrepateTopicDto(topic)).ToList();

            var topicsRootObject = new TopicsRootObject
            {
                Topics = topicsAsDtos
            };

            string json = JsonFieldsSerializer.Serialize(topicsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a count of all Topics
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/topics/count")]
        [ProducesResponseType(typeof(CategoriesCountRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetTopicsCount(CategoriesCountParametersModel parameters)
        {
            int allTopicCount = _topicApiService.GetTopicsCount(parameters.PublishedStatus);

            var topicsCountRootObject = new CategoriesCountRootObject
            {
                Count = allTopicCount
            };

            return Ok(topicsCountRootObject);
        }
    }
}