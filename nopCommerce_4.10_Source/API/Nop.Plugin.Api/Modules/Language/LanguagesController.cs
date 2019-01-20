using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Localization;
using Nop.Plugin.Api.Common.Attributes;
using Nop.Plugin.Api.Common.Controllers;
using Nop.Plugin.Api.Common.DTOs.Errors;
using Nop.Plugin.Api.Common.Helpers;
using Nop.Plugin.Api.Common.JSON.ActionResults;
using Nop.Plugin.Api.Common.JSON.Serializers;
using Nop.Plugin.Api.Modules.Languages.Dto;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Modules.Languages
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LanguagesController : BaseApiController
    {
        private readonly IDTOHelper _dtoHelper;
        private readonly ILanguageService _languageService;

        public LanguagesController(IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            ILanguageService languageService,
            IDTOHelper dtoHelper)
            : base(jsonFieldsSerializer,
                aclService,
                customerService,
                storeMappingService,
                storeService,
                discountService,
                customerActivityService,
                localizationService,
                pictureService)
        {
            _languageService = languageService;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        ///     Retrieve all languages
        /// </summary>
        /// <param name="fields">Fields from the language you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/languages")]
        [ProducesResponseType(typeof(LanguagesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetAllLanguages(string fields = "")
        {
            IList<Language> allLanguages = _languageService.GetAllLanguages();

            IList<LanguageDto> languagesAsDto = allLanguages.Select(language => _dtoHelper.PrepateLanguageDto(language)).ToList();

            var languagesRootObject = new LanguagesRootObject
            {
                Languages = languagesAsDto
            };

            string json = JsonFieldsSerializer.Serialize(languagesRootObject, fields);

            return new RawJsonActionResult(json);
        }
    }
}