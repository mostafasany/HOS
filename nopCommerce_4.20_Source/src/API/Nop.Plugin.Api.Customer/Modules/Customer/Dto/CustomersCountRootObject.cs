using Newtonsoft.Json;

namespace Nop.Plugin.Api.Customer.Modules.Customer.Dto
{
    public class CustomersCountRootObject
    {
        [JsonProperty("count")] public int Count { get; set; }
    }
}