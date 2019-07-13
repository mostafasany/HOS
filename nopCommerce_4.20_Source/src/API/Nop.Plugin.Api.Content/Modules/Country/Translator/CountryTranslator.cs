using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Api.Content.Modules.Country.Dto;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Content.Modules.Country.Translator
{
    public class CountryTranslator : ICountryTranslator
    {
        private readonly int _currentLanguageId;
        private readonly ILocalizationService _localizationService;

        public CountryTranslator(ILocalizationService localizationService, IHttpContextAccessor httpContextAccessor)
        {
            _localizationService = localizationService;
            var headers = httpContextAccessor.HttpContext.Request.Headers;
            if (!headers.ContainsKey("Accept-Language")) return;
            var lan = headers["Accept-Language"];
            _currentLanguageId = lan.ToString() == "en" ? 1 : 2;
        }

        public StateProvinceDto ToDto(StateProvince state)
        {
            var name = _localizationService.GetLocalized(state, x => x.Name, _currentLanguageId);
            var abbreviation = _localizationService.GetLocalized(state, x => x.Abbreviation, _currentLanguageId);
            return new StateProvinceDto
            {
                Abbreviation = abbreviation, Id = state.Id, Name = name, CountryId = state.CountryId
            };
        }
    }
}