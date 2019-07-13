using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Product.Modules.ProductAttributes.Dto;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Product.Modules.ProductAttributes.Translator
{
    public class ProductAttributesTranslator : IProductAttributesTranslator
    {
        private readonly int _currentLanguageId;
        private readonly ILocalizationService _localizationService;

        public ProductAttributesTranslator(ILocalizationService localizationService, IHttpContextAccessor httpContextAccessor)
        {
            _localizationService = localizationService;
            var headers = httpContextAccessor.HttpContext.Request.Headers;
            if (!headers.ContainsKey("Accept-Language")) return;
            var lan = headers["Accept-Language"];
            _currentLanguageId = lan.ToString() == "en" ? 1 : 2;
        }


        public ProductAttributeDto ToDto(ProductAttribute productAttribute)
        {
            var attribute = productAttribute.ToDto();
            attribute.Name = _localizationService.GetLocalized(productAttribute, x => x.Name, _currentLanguageId);
            attribute.Description =
                _localizationService.GetLocalized(productAttribute, x => x.Description, _currentLanguageId);
            return attribute;
        }
    }
}