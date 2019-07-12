using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Common.DTOs.Errors
{
    public class ErrorObject
    {
        [JsonProperty("cause")] public string Cause { get; set; }

        [JsonProperty("details")] public List<string> Details { get; set; }
    }
}