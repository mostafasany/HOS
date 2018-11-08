using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Media;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.Serializers;
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
    public class PicturesController : BaseApiController
    {
        private readonly IPictureService _pictureService;

        public PicturesController(IDiscountApiService discountApiService,
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
            localizationService, pictureService) => _pictureService = pictureService;


        /// <summary>
        ///     Receive a list of all discounts
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/pictures/{id}")]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetPictures(int id)
        {
            Picture picture = _pictureService.GetPictureById(id);
            string url = _pictureService.GetPictureUrl(picture);
            return Ok(url);
        }
    }
}