using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Topics
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