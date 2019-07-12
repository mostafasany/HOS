using System.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.Content.Modules.Language.Dto;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Content.Modules.Language.Translator
{
    public class LanguageTransaltor : ILanguageTransaltor
    {
        private readonly ILocalizationService _localizationService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;

        public LanguageTransaltor(ILocalizationService localizationService, IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService, IHttpContextAccessor httpContextAccessor)
        {
            _storeService = storeService;
            _storeMappingService = storeMappingService;
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
        }

        public LanguageDto PrepateLanguageDto(Core.Domain.Localization.Language language)
        {
            var languageDto = language.ToDto();

            languageDto.StoreIds = _storeMappingService.GetStoreMappings(language).Select(mapping => mapping.StoreId)
                .ToList();

            if (languageDto.StoreIds.Count == 0)
                languageDto.StoreIds = _storeService.GetAllStores().Select(s => s.Id).ToList();

            return languageDto;
        }
    }
}