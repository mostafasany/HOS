using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Plugin.Api.Modules.Language.Dto;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Modules.Language.Translator
{
    public class LanguageTransaltor : ILanguageTransaltor
    {
        private readonly int _currentLangaugeId;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;

        public LanguageTransaltor(ILocalizationService localizationService, IStoreMappingService storeMappingService, IStoreService storeService,
            IUrlRecordService urlRecordService, IHttpContextAccessor httpContextAccessor)
        {
            _storeService = storeService;
            _storeMappingService = storeMappingService;
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

        public LanguageDto PrepateLanguageDto(Core.Domain.Localization.Language language)
        {
            LanguageDto languageDto = language.ToDto();

            languageDto.StoreIds = _storeMappingService.GetStoreMappings(language).Select(mapping => mapping.StoreId)
                .ToList();

            if (languageDto.StoreIds.Count == 0) languageDto.StoreIds = _storeService.GetAllStores().Select(s => s.Id).ToList();

            return languageDto;
        }
    }
}