using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Content.Modules.Topic.Dto
{
    public class TopicsRootObject : ISerializableObject
    {
        public TopicsRootObject() => Topics = new List<TopicDto>();

        [JsonProperty("topics")]
        public IList<TopicDto> Topics { get; set; }

        public string GetPrimaryPropertyName() => "topics";

        public Type GetPrimaryPropertyType() => typeof(TopicDto);
    }
}