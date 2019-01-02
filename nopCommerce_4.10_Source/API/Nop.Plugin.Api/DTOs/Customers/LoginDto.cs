using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Customers
{
    [JsonObject(Title = "login")]
    public class LoginDto
    {
        [JsonProperty("userNameOrEmail")]
        public string UserNameOrEmail { get; set; }


        [JsonProperty("password")]
        public string Password { get; set; }
    }
}