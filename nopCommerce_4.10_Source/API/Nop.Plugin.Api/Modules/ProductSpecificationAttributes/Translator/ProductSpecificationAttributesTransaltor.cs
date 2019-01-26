using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Modules.SpecificationAttributes.Dto;
using Nop.Plugin.Api.Modules.SpecificationAttributes.Translator;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.Api.Modules.ProductSpecificationAttributes.Translator
{
    public class ProductSpecificationAttributesTransaltor : IProductSpecificationAttributesTransaltor
    {
        private readonly int _currentLangaugeId;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;

        public ProductSpecificationAttributesTransaltor(ILocalizationService localizationService,
            IUrlRecordService urlRecordService, IHttpContextAccessor httpContextAccessor, IPictureService pictureService)
        {
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            _pictureService = pictureService;
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


        public ProductSpecificationAttributeDto PrepareProductSpecificationAttributeDto(ProductSpecificationAttribute productSpecificationAttribute) => productSpecificationAttribute.ToDto();
    }
}