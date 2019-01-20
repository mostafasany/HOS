using Newtonsoft.Json;

namespace Nop.Plugin.Api.Modules.Order.Dto.Orders
{
    public class OrdersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}