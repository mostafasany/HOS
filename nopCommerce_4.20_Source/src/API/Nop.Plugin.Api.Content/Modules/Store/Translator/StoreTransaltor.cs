using System.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Api.Content.Modules.Store.Dto;
using Nop.Services.Directory;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Content.Modules.Store.Translator
{
    public class StoreTransaltor : IStoreTransaltor
    {
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly ILanguageService _languageService;

        public StoreTransaltor(
            ILanguageService languageService,
            ICurrencyService currencyService,
            CurrencySettings currencySettings, IHttpContextAccessor httpContextAccessor)
        {
            _languageService = languageService;
            _currencyService = currencyService;
            _currencySettings = currencySettings;
        }


        public StoreDto PrepareStoreDTO(Core.Domain.Stores.Store store)
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