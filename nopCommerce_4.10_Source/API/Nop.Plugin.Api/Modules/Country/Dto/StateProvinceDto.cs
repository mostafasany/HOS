using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs.Base;

namespace Nop.Plugin.Api.Modules.Country.Dto
{
    public class StateProvinceDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the country identifier
        /// </summary>
        [JsonProperty("country_id")]
        public int CountryId { get; set; }

        /// <summary>
        ///     Gets or sets the name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the abbreviation
        /// </summary>
        [JsonProperty("abbreviation")]
        public string Abbreviation { get; set; }
    }
}