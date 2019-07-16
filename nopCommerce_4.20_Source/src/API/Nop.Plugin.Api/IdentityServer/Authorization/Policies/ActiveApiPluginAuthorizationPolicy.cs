using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Nop.Plugin.Api.IdentityServer.Authorization.Requirements;

namespace Nop.Plugin.Api.IdentityServer.Authorization.Policies
{
    public class ActiveApiPluginAuthorizationPolicy : AuthorizationHandler<ActiveApiPluginRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ActiveApiPluginRequirement requirement)
        {
            if (requirement.IsActive())
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }
}