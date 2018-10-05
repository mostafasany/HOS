using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Base;

namespace Nop.Plugin.Api.DTOs.Topics
{
    [JsonObject(Title = "topic")]
    public class TopicDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the first name
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the last name
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; }
    }
}