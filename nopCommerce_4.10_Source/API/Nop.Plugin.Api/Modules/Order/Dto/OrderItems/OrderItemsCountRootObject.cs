using Newtonsoft.Json;

namespace Nop.Plugin.Api.Modules.Order.Dto.OrderItems
{
    public class OrderItemsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}