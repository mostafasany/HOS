using System;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Base;
using Nop.Plugin.Api.Validators;

namespace Nop.Plugin.Api.DTOs.Topics
{
    [JsonObject(Title = "topic")]
    // [Validator(typeof(AddressDtoValidator))]
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