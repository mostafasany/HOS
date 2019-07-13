using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Content.Modules.Manufacturer.Dto;
using Nop.Services.Media;

namespace Nop.Plugin.Api.Content.Modules.Manufacturer.Translator
{
    public class ManufacturerTranslator : IManufacturerTranslator
    {
        private readonly IPictureService _pictureService;

        public ManufacturerTranslator(IPictureService pictureService)
        {
            _pictureService = pictureService;
        }

        public ManufacturerDto ToDto(Core.Domain.Catalog.Manufacturer manufacturer)
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

        private ImageDto PrepareImageDto(Core.Domain.Media.Picture picture)
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