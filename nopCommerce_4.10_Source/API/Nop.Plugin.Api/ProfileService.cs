using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace Nop.Plugin.Api
{
    public class ProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var claims = new List<Claim>();
            string id = context.Subject.Claims.FirstOrDefault(a => a.Type == "id")?.Value;
            if (!string.IsNullOrEmpty(id))
                claims.Add(new Claim("id", id));
            context.IssuedClaims = claims;
            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.FromResult(0);
        }
    }
}