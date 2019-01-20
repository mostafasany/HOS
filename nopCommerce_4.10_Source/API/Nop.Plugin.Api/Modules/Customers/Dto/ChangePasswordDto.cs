﻿using Newtonsoft.Json;

namespace Nop.Plugin.Api.Modules.Customers.Dto
{
    [JsonObject(Title = "changepassword")]
    public class ChangePasswordDto
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("oldPassword")]
        public string OldPassword { get; set; }


        [JsonProperty("newPassword")]
        public string NewPassword { get; set; }
    }
}