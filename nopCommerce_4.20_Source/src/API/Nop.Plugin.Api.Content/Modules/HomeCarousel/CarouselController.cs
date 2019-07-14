using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Common.Attributes;
using Nop.Plugin.Api.Common.Controllers;
using Nop.Plugin.Api.Common.DTOs.Errors;
using Nop.Plugin.Api.Common.JSON.ActionResults;
using Nop.Plugin.Api.Common.JSON.Serializers;
using Nop.Plugin.Api.Content.Modules.HomeCarousel.Dto;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Content.Modules.HomeCarousel
{
    public class CarouselController : BaseApiController
    {
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;

        public CarouselController(
            ISettingService settingService,
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
            _settingService = settingService;
            _pictureService = pictureService;
        }


        /// <summary>
        ///     Get Carousel
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/carousel")]
        [ProducesResponseType(typeof(CarouselDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetCarousel()
        {
            var carousel = new CarouselRootObject();
            carousel.Carousel.Add(GetCarouselFromSettings(1));
            carousel.Carousel.Add(GetCarouselFromSettings(2));
            carousel.Carousel.Add(GetCarouselFromSettings(3));
            carousel.Carousel.Add(GetCarouselFromSettings(4));
            carousel.Carousel.Add(GetCarouselFromSettings(5));
            var json = JsonFieldsSerializer.Serialize(carousel, string.Empty);
            return new RawJsonActionResult(json);
        }

        private CarouselDto GetCarouselFromSettings(int number)
        {
            try
            {
                var pictureKey = $"nivoslidersettings.picture{number}id";
                var topicIdKey = $"nivoslidersettings.link{number}";
                var pictureId = _settingService.GetSettingByKey<int>(pictureKey);
                var topicId = _settingService.GetSettingByKey<int>(topicIdKey);
                var url = _pictureService.GetPictureUrl(pictureId);
                return new CarouselDto {Image = url, Topic = topicId};
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}