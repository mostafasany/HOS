using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Nop.Plugin.Api.Common.Authorization.Requirements;

namespace Nop.Plugin.Api.Common.Authorization.Policies
{
    public class RequestsFromSwaggerAuthorizationPolicy : AuthorizationHandler<RequestFromSwaggerOptional>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RequestFromSwaggerOptional requirement)
        {
            if (requirement.AllowRequestsFromSwagger())
            {
                if (requirement.IsRequestFromSwagger(context.Resource as string))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            else
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}