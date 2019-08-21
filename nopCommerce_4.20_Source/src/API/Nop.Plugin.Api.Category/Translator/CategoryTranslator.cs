using System.Linq;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Media;
using Nop.Plugin.Api.Category.Dto;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Category.Translator
{
    public class CategoryTranslator : ICategoryTranslator
    {
        private readonly IAclService _aclService;
        private readonly int _currentLanguageId;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;

        public CategoryTranslator(
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IUrlRecordService urlRecordService, IHttpContextAccessor httpContextAccessor)
        {
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _pictureService = pictureService;
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            var headers = httpContextAccessor.HttpContext.Request.Headers;
            if (!headers.ContainsKey("Accept-Language")) return;
            var lan = headers["Accept-Language"];
            _currentLanguageId = lan.ToString() == "en" ? 1 : 2;
        }


        public CategoryDto PrepareCategoryDto(Core.Domain.Catalog.Category category)
        {
            var categoryDto = category.ToDto();
            categoryDto.Name = _localizationService.GetLocalized(category, x => x.Name, _currentLanguageId);
            categoryDto.Description =
                _localizationService.GetLocalized(category, x => x.Description, _currentLanguageId);

            var picture = _pictureService.GetPictureById(category.PictureId);
            var imageDto = PrepareImageDto(picture);

            if (imageDto != null) categoryDto.Image = imageDto;

            categoryDto.SeName = _urlRecordService.GetSeName(category);
            categoryDto.DiscountIds = category.AppliedDiscounts.Select(discount => discount.Id).ToList();
            categoryDto.RoleIds = _aclService.GetAclRecords(category).Select(acl => acl.CustomerRoleId).ToList();
            categoryDto.StoreIds = _storeMappingService.GetStoreMappings(category).Select(mapping => mapping.StoreId)
                .ToList();

            return categoryDto;
        }

        private ImageDto PrepareImageDto(Picture picture)
        {
            ImageDto image = null;

            if (picture != null)
                image = new ImageDto
                {
                    //Attachment = Convert.ToBase64String(picture.PictureBinary),
                    Src = _pictureService.GetPictureUrl(picture)
                };

            return image;
        }
    }
}