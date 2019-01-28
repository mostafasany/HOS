using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.Api.Common.ModelBinders;

namespace Nop.Plugin.Api.Modules.Cart.Model
{
    [ModelBinder(typeof(ParametersModelBinder<ShoppingCartItemsShipmentEstimationModel>))]
    public class ShoppingCartItemsShipmentEstimationModel
    {
        [JsonProperty("country_id")]
        public int CountryId { get; set; }

        [JsonProperty("state_province_id")]
        public int StateProvinceId { get; set; }

        [JsonProperty("zip_postal_code")]
        public string ZipPostalCode { get; set; }

        [JsonProperty("address1")]
        public string Address1 { get; set; }

        [JsonProperty("address2")]
        public string Address2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }
    }
}