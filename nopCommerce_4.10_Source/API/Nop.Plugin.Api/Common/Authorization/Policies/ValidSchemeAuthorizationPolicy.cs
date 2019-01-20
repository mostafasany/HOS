using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Nop.Plugin.Api.Common.Authorization.Requirements;

namespace Nop.Plugin.Api.Common.Authorization.Policies
{
    public class ValidSchemeAuthorizationPolicy : AuthorizationHandler<AuthorizationSchemeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationSchemeRequirement requirement)
        {
            var mvcContext = context.Resource as
                Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext;

            if (requirement.IsValid(mvcContext?.HttpContext.Request.Headers))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}