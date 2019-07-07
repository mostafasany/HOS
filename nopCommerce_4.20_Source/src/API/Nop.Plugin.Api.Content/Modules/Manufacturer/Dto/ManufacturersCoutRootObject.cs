using Newtonsoft.Json;

namespace Nop.Plugin.Api.Content.Modules.Manufacturer.Dto
{
    public class ManufacturersCoutRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
