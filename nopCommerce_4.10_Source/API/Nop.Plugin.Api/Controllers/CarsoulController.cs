﻿using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.Carsoul;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
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
    public class CarsoulController : BaseApiController
    {
        public CarsoulController(
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
        }


        /// <summary>
        ///     Get carsoul
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/carsoul")]
        [ProducesResponseType(typeof(CarsoulDto), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetCarsoul()
        {
            var carsoul = new CarsoulRootObject();
            carsoul.Carsoul.Add(new CarsoulDto {Image = "assets/images/banner2.jpg", Url = ""});
            carsoul.Carsoul.Add(new CarsoulDto {Image = "assets/images/banner3.jpg", Url = ""});
            carsoul.Carsoul.Add(new CarsoulDto {Image = "assets/images/banner2.jpg", Url = ""});
            carsoul.Carsoul.Add(new CarsoulDto {Image = "assets/images/banner2.jpg", Url = ""});
            carsoul.Carsoul.Add(new CarsoulDto {Image = "assets/images/banner1.jpg", Url = ""});
            string json = JsonFieldsSerializer.Serialize(carsoul, string.Empty);
            return new RawJsonActionResult(json);
        }
    }
}