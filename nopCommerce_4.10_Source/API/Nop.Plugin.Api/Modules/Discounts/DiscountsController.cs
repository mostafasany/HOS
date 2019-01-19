using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.Discounts;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.Models.DiscountsParameters;
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
    public class DiscountsController : BaseApiController
    {
        private readonly IDiscountApiService _discountApiService;
        private readonly IDTOHelper _dtoHelper;

        public DiscountsController(IDiscountApiService discountApiService,
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
            _discountApiService = discountApiService;
            _dtoHelper = dtoHelper;
        }


        /// <summary>
        ///     Receive a list of all discounts
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/discounts")]
        [ProducesResponseType(typeof(DiscountsRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetDiscounts(DiscountsParametersModel parameters)
        {
            IEnumerable<Discount> allDiscounts = _discountApiService.GetDiscounts(parameters.Ids);

            IList<DiscountDto> discountsAsDtos = allDiscounts.Select(discount =>
                _dtoHelper.PrepateDiscountDto(discount)).ToList();

            var discountsRootObject = new DiscountsRootObject
            {
                Discounts = discountsAsDtos
            };

            string json = JsonFieldsSerializer.Serialize(discountsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }
    }
}