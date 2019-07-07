using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Common.Attributes;
using Nop.Plugin.Api.Common.Controllers;
using Nop.Plugin.Api.Common.DTOs.Errors;
using Nop.Plugin.Api.Common.JSON.ActionResults;
using Nop.Plugin.Api.Common.JSON.Serializers;
using Nop.Plugin.Api.HomeCarsoul.Dto;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Modules
{
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
            ICustomerService customerService) : base(jsonFieldsSerializer, aclService, customerService,
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
            carsoul.Carsoul.Add(new CarsoulDto {Image = "assets/images/banner2.jpg", Topic = 17});
            carsoul.Carsoul.Add(new CarsoulDto {Image = "assets/images/banner3.jpg", Topic = 18});
            carsoul.Carsoul.Add(new CarsoulDto {Image = "assets/images/banner2.jpg", Topic = 19});
            carsoul.Carsoul.Add(new CarsoulDto {Image = "assets/images/banner2.jpg", Topic = 20});
            carsoul.Carsoul.Add(new CarsoulDto {Image = "assets/images/banner1.jpg", Topic = 21});
            string json = JsonFieldsSerializer.Serialize(carsoul, string.Empty);
            return new RawJsonActionResult(json);
        }
    }
}