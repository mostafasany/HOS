using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Content.Modules.Manufacturer.Dto;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;

namespace Nop.Plugin.Api.Content.Modules.Manufacturer.Translator
{
    public class ManufacturerTransaltor : IManufacturerTransaltor
    {
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;

        public ManufacturerTransaltor(ILocalizationService localizationService, IPictureService pictureService,
            IUrlRecordService urlRecordService, IHttpContextAccessor httpContextAccessor)
        {
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            _pictureService = pictureService;
        }

        public ManufacturerDto ConvertToDto(Core.Domain.Catalog.Manufacturer manufacturer)
        {
            var picture = _pictureService.GetPictureById(manufacturer.PictureId);
            var imageDto = PrepareImageDto(picture);

            var manufacturerDto = new ManufacturerDto
            {
                Id = manufacturer.Id, Name = manufacturer.Name, Description = manufacturer.Description
            };

            if (imageDto != null)
                manufacturerDto.Image = imageDto;

            return manufacturerDto;
        }

        protected ImageDto PrepareImageDto(Core.Domain.Media.Picture picture)
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