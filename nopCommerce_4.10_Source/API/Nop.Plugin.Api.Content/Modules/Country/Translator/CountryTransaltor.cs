using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Api.Content.Modules.Country.Dto;
using Nop.Services.Localization;
using Nop.Services.Seo;

namespace Nop.Plugin.Api.Content.Modules.Country.Translator
{
    public class CountryTransaltor : ICountryTransaltor
    {
        private readonly int _currentLangaugeId;
        private readonly ILocalizationService _localizationService;
        private readonly IUrlRecordService _urlRecordService;

        public CountryTransaltor(ILocalizationService localizationService,
            IUrlRecordService urlRecordService, IHttpContextAccessor httpContextAccessor)
        {
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            IHeaderDictionary headers = httpContextAccessor.HttpContext.Request.Headers;
            if (headers.ContainsKey("Accept-Language"))
            {
                StringValues lan = headers["Accept-Language"];
                if (lan.ToString() == "en")
                    _currentLangaugeId = 1;
                else
                    _currentLangaugeId = 2;
            }
        }

        public StateProvinceDto ConvertToDto(StateProvince state) => new StateProvinceDto {Abbreviation = state.Abbreviation, Id = state.Id, Name = state.Name, CountryId = state.CountryId};
    }
}