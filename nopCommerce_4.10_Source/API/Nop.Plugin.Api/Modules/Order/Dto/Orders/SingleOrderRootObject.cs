using Newtonsoft.Json;

namespace Nop.Plugin.Api.Modules.Order.Dto.Orders
{
    public class SingleOrderRootObject
    {
        [JsonProperty("order")]
        public OrderDto Order { get; set; }
    }
}