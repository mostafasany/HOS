using Newtonsoft.Json;

namespace Nop.Plugin.Api.Modules.Customers.Dto
{
    public class CustomersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}