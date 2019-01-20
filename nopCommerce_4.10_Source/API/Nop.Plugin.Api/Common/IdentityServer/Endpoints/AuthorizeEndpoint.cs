using System.Collections.Specialized;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Api.Common.IdentityServer.Endpoints
{
    public class AuthorizeEndpoint : AuthorizeEndpointBase
    {
        public AuthorizeEndpoint(
            IEventService events,
            IAuthorizeRequestValidator validator,
            IAuthorizeInteractionResponseGenerator interactionGenerator,
            IAuthorizeResponseGenerator authorizeResponseGenerator,
            IUserSession userSession)
            : base(events, userSession, validator, authorizeResponseGenerator, interactionGenerator)
        {
        }

        public override async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            NameValueCollection values;

            if (context.Request.Method == "GET")
            {
                values = context.Request.Query.AsNameValueCollection();
            }
            else if (context.Request.Method == "POST")
            {
                if (!context.Request.HasFormContentType) return new StatusCodeResult(HttpStatusCode.UnsupportedMediaType);

                values = context.Request.Form.AsNameValueCollection();
            }
            else
            {
                return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
            }

            ClaimsPrincipal user = await UserSession.GetUserAsync();
            IEndpointResult result = await ProcessAuthorizeRequestAsync(values, user, null);

            return result;
        }
    }
}