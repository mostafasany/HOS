using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Api.IdentityServer.Authorization.Requirements
{
    public class AuthorizationSchemeRequirement : IAuthorizationRequirement
    {
        public bool IsValid(IHeaderDictionary requestHeaders)
        {
            return requestHeaders != null &&
                   requestHeaders.ContainsKey("Authorization") &&
                   requestHeaders["Authorization"].ToString().Contains(JwtBearerDefaults.AuthenticationScheme);
        }
    }
}