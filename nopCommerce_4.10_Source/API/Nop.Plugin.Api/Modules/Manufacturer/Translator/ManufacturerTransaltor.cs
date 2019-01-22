using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Plugin.Api.Modules.Manufacturer.Dto;
using Nop.Services.Localization;
using Nop.Services.Seo;

namespace Nop.Plugin.Api.Modules.Manufacturer.Translator
{
    public class ManufacturerTransaltor : IManufacturerTransaltor
    {
        private readonly int _currentLangaugeId;
        private readonly ILocalizationService _localizationService;
        private readonly IUrlRecordService _urlRecordService;

        public ManufacturerTransaltor(ILocalizationService localizationService,
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

        public ManufacturerDto ConvertToDto(Core.Domain.Catalog.Manufacturer manufacturer) => new ManufacturerDto {Id = manufacturer.Id, Name = manufacturer.Name, Description = manufacturer.Description};
    }
}