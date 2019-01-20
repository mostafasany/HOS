using Newtonsoft.Json;

namespace Nop.Plugin.Api.Modules.Customer.Dto
{
    public class CustomersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}