using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs.Base;

namespace Nop.Plugin.Api.Modules.HomeCarsoul.Dto
{
    public class CarsoulDto : BaseDto
    {
        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("topicId")]
        public int Topic { get; set; }
    }
}