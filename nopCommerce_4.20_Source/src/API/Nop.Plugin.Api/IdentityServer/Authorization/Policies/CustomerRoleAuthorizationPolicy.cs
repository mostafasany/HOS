using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Nop.Plugin.Api.IdentityServer.Authorization.Requirements;

namespace Nop.Plugin.Api.IdentityServer.Authorization.Policies
{
    public class CustomerRoleAuthorizationPolicy : AuthorizationHandler<CustomerRoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            CustomerRoleRequirement requirement)
        {
            if (requirement.IsCustomerInRole())
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }
}