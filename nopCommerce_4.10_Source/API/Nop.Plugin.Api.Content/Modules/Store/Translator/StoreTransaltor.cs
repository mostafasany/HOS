using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Api.Modules.Store.Dto;
using Nop.Services.Directory;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Modules.Store.Translator
{
    public class StoreTransaltor : IStoreTransaltor
    {
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly int _currentLangaugeId;
        private readonly ILanguageService _languageService;

        public StoreTransaltor(
            ILanguageService languageService,
            ICurrencyService currencyService,
            CurrencySettings currencySettings, IHttpContextAccessor httpContextAccessor)
        {
            _languageService = languageService;
            _currencyService = currencyService;
            _currencySettings = currencySettings;
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


        public StoreDto PrepareStoreDTO(Core.Domain.Stores.Store store)
        {
            StoreDto storeDto = store.ToDto();

            Currency primaryCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId);

            if (!string.IsNullOrEmpty(primaryCurrency.DisplayLocale)) storeDto.PrimaryCurrencyDisplayLocale = primaryCurrency.DisplayLocale;

            storeDto.LanguageIds = _languageService.GetAllLanguages(false, store.Id).Select(x => x.Id).ToList();

            return storeDto;
        }
    }
}