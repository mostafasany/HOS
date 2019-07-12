using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs.Base;

namespace Nop.Plugin.Api.Content.Modules.Topic.Dto
{
    [JsonObject(Title = "topic")]
    public class TopicDto : BaseDto
    {
        /// <summary>
        ///     Gets or sets the first name
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the last name
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("se_name")] public string SeName { get; set; }
    }
}