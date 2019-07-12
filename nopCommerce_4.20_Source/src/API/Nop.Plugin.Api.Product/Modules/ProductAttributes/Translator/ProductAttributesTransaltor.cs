using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Product.Modules.ProductAttributes.Dto;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.Api.Product.Modules.ProductAttributes.Translator
{
    public class ProductAttributesTransaltor : IProductAttributesTransaltor
    {
        private readonly int _currentLangaugeId;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;

        public ProductAttributesTransaltor(ILocalizationService localizationService,
            IUrlRecordService urlRecordService, IHttpContextAccessor httpContextAccessor,
            IPictureService pictureService)
        {
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            _pictureService = pictureService;
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


        public ProductAttributeDto PrepareProductAttributeDTO(ProductAttribute productAttribute)
        {
            var attribute = productAttribute.ToDto();
            attribute.Name = _localizationService.GetLocalized(productAttribute, x => x.Name, _currentLangaugeId);
            attribute.Description =
                _localizationService.GetLocalized(productAttribute, x => x.Description, _currentLangaugeId);
            return attribute;
        }
    }
}