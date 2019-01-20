using Newtonsoft.Json;
using Nop.Plugin.Api.Common.Attributes;

namespace Nop.Plugin.Api.Modules.Pictures.Dto
{
    [ImageValidation]
    public class ImageMappingDto : ImageDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("pictureId")]
        public int PictureId { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }
    }
}