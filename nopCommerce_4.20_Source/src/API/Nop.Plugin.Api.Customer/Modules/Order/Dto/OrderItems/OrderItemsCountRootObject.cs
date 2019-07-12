using Newtonsoft.Json;

namespace Nop.Plugin.Api.Customer.Modules.Order.Dto.OrderItems
{
    public class OrderItemsCountRootObject
    {
        [JsonProperty("count")] public int Count { get; set; }
    }
}