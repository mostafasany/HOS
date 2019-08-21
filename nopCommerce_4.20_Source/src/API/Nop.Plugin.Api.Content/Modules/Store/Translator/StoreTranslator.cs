using System.Linq;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Api.Content.Modules.Store.Dto;
using Nop.Services.Directory;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Content.Modules.Store.Translator
{
    public class StoreTranslator : IStoreTranslator
    {
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly ILanguageService _languageService;

        public StoreTranslator(
            ILanguageService languageService,
            ICurrencyService currencyService,
            CurrencySettings currencySettings)
        {
            _languageService = languageService;
            _currencyService = currencyService;
            _currencySettings = currencySettings;
        }


        public StoreDto ToDto(Core.Domain.Stores.Store store)
        {
            var storeDto = store.ToDto();

            var primaryCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);

            if (!string.IsNullOrEmpty(primaryCurrency.DisplayLocale))
                storeDto.PrimaryCurrencyDisplayLocale = primaryCurrency.DisplayLocale;

            storeDto.LanguageIds = _languageService.GetAllLanguages(false, store.Id).Select(x => x.Id).ToList();

            return storeDto;
        }
    }
}