using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.ModelBinders;

namespace Nop.Plugin.Api.Customer.Modules.Order.Model
{
    [ModelBinder(typeof(ParametersModelBinder<OrderItemsParametersModel>))]
    public class OrderItemsParametersModel
    {
        public OrderItemsParametersModel()
        {
            Limit = Configurations.DefaultLimit;
            Page = Configurations.DefaultPageValue;
            SinceId = 0;
            Fields = string.Empty;
        }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("since_id")]
        public int SinceId { get; set; }

        [JsonProperty("fields")]
        public string Fields { get; set; }
    }
}