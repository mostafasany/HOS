using Newtonsoft.Json;

namespace Nop.Plugin.Api.Modules.Customer.Dto
{
    [JsonObject(Title = "forget")]
    public class ForgetDto
    {
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}