using Newtonsoft.Json;

namespace Nop.Plugin.Api.Modules.Orders.Dto.OrderItems
{
    public class OrderItemsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}