using Newtonsoft.Json;

namespace Nop.Plugin.Api.Modules.Orders.Dto.Orders
{
    public class OrdersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}