using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Plugin.Api.Product.Modules.Discount.Dto;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.Api.Product.Modules.Discount.Translator
{
    public class DiscountTransaltor : IDiscountTransaltor
    {
        private readonly int _currentLangaugeId;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;

        public DiscountTransaltor(ILocalizationService localizationService,
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


        public DiscountDto PrepateDiscountDto(Core.Domain.Discounts.Discount discount) => new DiscountDto
        {
            CouponCode = discount.CouponCode,
            Name = discount.Name,
            Id = discount.Id,
            DiscountLimitationId = discount.DiscountLimitationId,
            DiscountPercentage = discount.DiscountPercentage,
            DiscountTypeId = discount.DiscountTypeId,
            IsCumulative = discount.IsCumulative,
            LimitationTimes = discount.LimitationTimes,
            MaximumDiscountAmount = discount.MaximumDiscountAmount,
            MaximumDiscountedQuantity = discount.MaximumDiscountedQuantity,
            RequiresCouponCode = discount.RequiresCouponCode,
            UsePercentage = discount.UsePercentage
        };
    }
}