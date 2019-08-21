using Newtonsoft.Json;

namespace Nop.Plugin.Api.Content.Modules.Manufacturer.Dto
{
    public class ManufacturersCountRootObject
    {
        [JsonProperty("count")] public int Count { get; set; }
    }
}