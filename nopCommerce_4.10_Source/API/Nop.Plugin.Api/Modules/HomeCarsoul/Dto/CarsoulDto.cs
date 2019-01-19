using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Base;

namespace Nop.Plugin.Api.DTOs.Carsoul
{
    public class CarsoulDto : BaseDto
    {
        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("topicId")]
        public int Topic { get; set; }
    }
}