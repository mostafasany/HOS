using System.Linq;
using Nop.Plugin.Api.Content.Modules.Language.Dto;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Content.Modules.Language.Translator
{
    public class LanguageTranslator : ILanguageTranslator
    { private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;

        public LanguageTranslator(IStoreMappingService storeMappingService,
            IStoreService storeService)
        {
            _storeService = storeService;
            _storeMappingService = storeMappingService;
        }

        public LanguageDto ToDto(Core.Domain.Localization.Language language)
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