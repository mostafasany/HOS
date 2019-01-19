using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Customers
{
    [JsonObject(Title = "forget")]
    public class ForgetDto
    {
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}