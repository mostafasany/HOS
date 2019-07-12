using System;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Customer.Modules.Customer.Dto
{
    [JsonObject(Title = "information")]
    public class InformationDto : ISerializableObject
    {
        [JsonProperty("message")] public string Message { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "message";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(string);
        }
    }
}