using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Common.DTOs.Errors
{
    public class ErrorsRootObject : ISerializableObject
    {
        //[JsonProperty("errors")]
        //public Dictionary<string, List<string>> Errors { get; set; }

        [JsonProperty("errors")]
        public List<ErrorObject> Errors { get; set; }

        public string GetPrimaryPropertyName() => "errors";

        public Type GetPrimaryPropertyType() => Errors.GetType();
    }

    public class ErrorObject
    {
        [JsonProperty("cause")]
        public string Cause { get; set; }

        [JsonProperty("details")]
        public List<string> Details { get; set; }
    }
}