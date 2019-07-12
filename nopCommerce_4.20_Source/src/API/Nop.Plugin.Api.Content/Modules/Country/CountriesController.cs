using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Common.Attributes;
using Nop.Plugin.Api.Common.Controllers;
using Nop.Plugin.Api.Common.DTOs.Errors;
using Nop.Plugin.Api.Common.JSON.ActionResults;
using Nop.Plugin.Api.Common.JSON.Serializers;
using Nop.Plugin.Api.Content.Modules.Country.Dto;
using Nop.Plugin.Api.Content.Modules.Country.Service;
using Nop.Plugin.Api.Content.Modules.Country.Translator;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Content.Modules.Country
{
    public class CountriesController : BaseApiController
    {
        private readonly ICountryTransaltor _dtoHelper;
        private readonly IStateProvinceApiService _stateProvinceApiService;

        public CountriesController(IStateProvinceApiService stateProvinceApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            IAclService aclService,
            ICustomerService customerService,
            ICountryTransaltor dtoHelper) : base(jsonFieldsSerializer, aclService, customerService,
            storeMappingService, storeService, discountService, customerActivityService,
            localizationService, pictureService)
        {
            _stateProvinceApiService = stateProvinceApiService;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        ///     Retrieve states by spcified id
        /// </summary>
        /// <param name="id">Id of the states</param>
        /// <param name="fields">Fields from the states you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/countries/{id}/states")]
        [ProducesResponseType(typeof(StateProvinceRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetStateByCountryId(int id, string fields = "")
        {
            if (id <= 0) return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            var states = _stateProvinceApiService.GetStateProvincesByCountryId(id);

            if (states == null) return Error(HttpStatusCode.NotFound, "states", "states not found");

            IList<StateProvinceDto> statesAsDtos = states.Select(state =>
                _dtoHelper.ConvertToDto(state)).ToList();

            var statesRootObject = new StateProvinceRootObject {States = statesAsDtos};


            var json = JsonFieldsSerializer.Serialize(statesRootObject, fields);

            return new RawJsonActionResult(json);
        }
    }
}