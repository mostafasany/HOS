using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Nop.Plugin.Api.Common.Authorization.Requirements;

namespace Nop.Plugin.Api.Common.Authorization.Policies
{
    public class ActiveClientAuthorizationPolicy : AuthorizationHandler<ActiveClientRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ActiveClientRequirement requirement)
        {
            if (requirement.IsClientActive())
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }
}