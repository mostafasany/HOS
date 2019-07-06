using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Media;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Content.Modules.Manufacturer.Dto;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.Api.Content.Modules.Manufacturer.Translator
{
    public class ManufacturerTransaltor : IManufacturerTransaltor
    {
        private readonly int _currentLangaugeId;
        private readonly ILocalizationService _localizationService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;

        public ManufacturerTransaltor(ILocalizationService localizationService, IPictureService pictureService,
            IUrlRecordService urlRecordService, IHttpContextAccessor httpContextAccessor)
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

        public ManufacturerDto ConvertToDto(Core.Domain.Catalog.Manufacturer manufacturer)
        {
            Picture picture = _pictureService.GetPictureById(manufacturer.PictureId);
            ImageDto imageDto = PrepareImageDto(picture);

            var manufacturerDto = new ManufacturerDto
            {
                Id = manufacturer.Id,
                Name = manufacturer.Name,
                Description = manufacturer.Description
            };

            if (imageDto != null)
                manufacturerDto.Image = imageDto;

            return manufacturerDto;
        }

        protected ImageDto PrepareImageDto(Picture picture)
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