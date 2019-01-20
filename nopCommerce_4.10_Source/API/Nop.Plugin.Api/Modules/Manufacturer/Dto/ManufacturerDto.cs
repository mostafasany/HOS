using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs.Base;

namespace Nop.Plugin.Api.Modules.Manufacturers.Dto
{
    [JsonObject(Title = "manufacturer")]
    public class ManufacturerDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the first name
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the last name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}