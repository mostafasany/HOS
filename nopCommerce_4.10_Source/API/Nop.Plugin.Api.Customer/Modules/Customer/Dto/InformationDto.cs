using System;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.DTOs;

namespace Nop.Plugin.Api.Customer.Modules.Customer.Dto
{
    [JsonObject(Title = "information")]
    public class InformationDto: ISerializableObject
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        public string GetPrimaryPropertyName() => "message";

        public Type GetPrimaryPropertyType() => typeof(string);
    }
}