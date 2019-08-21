using Newtonsoft.Json;

namespace Nop.Plugin.Api.Common.DTOs.Base
{
    public abstract class BaseDto
    {
        [JsonProperty("id")] public int Id { get; set; }
    }
}