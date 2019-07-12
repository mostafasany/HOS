using Microsoft.AspNetCore.Http;
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
            var headers = httpContextAccessor.HttpContext.Request.Headers;
            if (headers.ContainsKey("Accept-Language"))
            {
                var lan = headers["Accept-Language"];
                if (lan.ToString() == "en")
                    _currentLangaugeId = 1;
                else
                    _currentLangaugeId = 2;
            }
        }

        public StateProvinceDto ConvertToDto(StateProvince state)
        {
            var name = _localizationService.GetLocalized(state, x => x.Name, _currentLangaugeId);
            var abbreviation = _localizationService.GetLocalized(state, x => x.Abbreviation, _currentLangaugeId);
            return new StateProvinceDto
            {
                Abbreviation = abbreviation, Id = state.Id, Name = name, CountryId = state.CountryId
            };
        }
    }
}