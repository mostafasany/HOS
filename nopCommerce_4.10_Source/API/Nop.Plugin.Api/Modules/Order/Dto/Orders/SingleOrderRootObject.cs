using Newtonsoft.Json;

namespace Nop.Plugin.Api.Modules.Orders.Dto.Orders
{
    public class SingleOrderRootObject
    {
        [JsonProperty("order")]
        public OrderDto Order { get; set; }
    }
}