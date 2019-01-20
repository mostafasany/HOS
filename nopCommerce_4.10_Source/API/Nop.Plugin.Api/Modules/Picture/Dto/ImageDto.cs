using Newtonsoft.Json;
using Nop.Plugin.Api.Common.Attributes;

namespace Nop.Plugin.Api.Modules.Pictures.Dto
{
    [ImageValidation]
    public class ImageDto
    {
        [JsonProperty("src")]
        public string Src { get; set; }

        [JsonProperty("attachment")]
        public string Attachment { get; set; }

        [JsonIgnore]
        public byte[] Binary { get; set; }

        [JsonIgnore]
        public string MimeType { get; set; }
    }
}