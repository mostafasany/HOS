using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Common.Attributes;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.Controllers;
using Nop.Plugin.Api.Common.DTOs.Errors;
using Nop.Plugin.Api.Common.JSON.ActionResults;
using Nop.Plugin.Api.Common.JSON.Serializers;
using Nop.Plugin.Api.Content.Modules.Manufacturer.Dto;
using Nop.Plugin.Api.Content.Modules.Manufacturer.Model;
using Nop.Plugin.Api.Content.Modules.Manufacturer.Service;
using Nop.Plugin.Api.Content.Modules.Manufacturer.Translator;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Content.Modules.Manufacturer
{
    public class ManufacturersController : BaseApiController
    {
        private readonly IManufacturerTranslator _dtoHelper;
        private readonly IManufacturerApiService _manufacturerApiService;

        public ManufacturersController(IManufacturerApiService manufacturerApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            IAclService aclService,
            ICustomerService customerService,
            IManufacturerTranslator dtoHelper) : base(jsonFieldsSerializer, aclService, customerService,
            storeMappingService, storeService, discountService, customerActivityService,
            localizationService, pictureService)
        {
            _manufacturerApiService = manufacturerApiService;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        ///     Retrieve manufacturer by specified id
        /// </summary>
        /// <param name="id">Id of the manufacturer</param>
        /// <param name="fields">Fields from the manufacturer you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/manufacturers/{id}")]
        [ProducesResponseType(typeof(ManufacturersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetManufacturerById(int id, string fields = "")
        {
            if (id <= 0) return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            var manufacturer = _manufacturerApiService.GetManufacturerById(id);

            if (manufacturer == null) return Error(HttpStatusCode.NotFound, "manufacturer", "manufacturer not found");

            var manufacturerDto = _dtoHelper.ToDto(manufacturer);

            var manufacturersRootObject = new ManufacturersRootObject();

            manufacturersRootObject.Manufacturers.Add(manufacturerDto);

            var json = JsonFieldsSerializer.Serialize(manufacturersRootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a list of all manufacturers
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/manufacturers")]
        [ProducesResponseType(typeof(ManufacturersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetManufacturers(ManufacturersParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");

            var allManufacturers = _manufacturerApiService.GetManufacturers(parameters.Ids, parameters.Limit,
                    parameters.Page, parameters.SinceId,
                    parameters.PublishedStatus)
                .Where(c => StoreMappingService.Authorize(c));

            IList<ManufacturerDto> manufacturersAsDto = allManufacturers.Select(manufacturer =>
                _dtoHelper.ToDto(manufacturer)).ToList();

            var manufacturersRootObject = new ManufacturersRootObject {Manufacturers = manufacturersAsDto};

            var json = JsonFieldsSerializer.Serialize(manufacturersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Receive a count of all Manufacturers
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/manufacturers/count")]
        [ProducesResponseType(typeof(ManufacturersCountRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetManufacturersCount(ManufacturersParametersModel parameters)
        {
            var allManufacturersCount = _manufacturerApiService.GetManufacturersCount(parameters.PublishedStatus);

            var manufacturersRootObject = new ManufacturersCountRootObject {Count = allManufacturersCount};

            return Ok(manufacturersRootObject);
        }
    }
}