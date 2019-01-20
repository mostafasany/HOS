using Newtonsoft.Json;

namespace Nop.Plugin.Api.Modules.Customers.Dto
{
    [JsonObject(Title = "forget")]
    public class ForgetDto
    {
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}