using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Api.Common.IdentityServer.Endpoints
{
    public class TokenEndpoint : IEndpointHandler
    {
        private readonly IClientSecretValidator _clientValidator;
        private readonly IEventService _events;
        private readonly ITokenRequestValidator _requestValidator;
        private readonly ITokenResponseGenerator _responseGenerator;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TokenEndpoint" /> class.
        /// </summary>
        /// <param name="clientValidator">The client validator.</param>
        /// <param name="requestValidator">The request validator.</param>
        /// <param name="responseGenerator">The response generator.</param>
        /// <param name="events">The events.</param>
        /// <param name="logger">The logger.</param>
        public TokenEndpoint(
            IClientSecretValidator clientValidator,
            ITokenRequestValidator requestValidator,
            ITokenResponseGenerator responseGenerator,
            IEventService events)
        {
            _clientValidator = clientValidator;
            _requestValidator = requestValidator;
            _responseGenerator = responseGenerator;
            _events = events;
        }

        /// <summary>
        ///     Processes the request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns></returns>
        public async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            // validate HTTP
            if (context.Request.Method != "POST" || !context.Request.HasFormContentType) return Error(OidcConstants.TokenErrors.InvalidRequest);

            return await ProcessTokenRequestAsync(context);
        }

        private TokenErrorResult Error(string error, string errorDescription = null, Dictionary<string, object> custom = null)
        {
            var response = new TokenErrorResponse
            {
                Error = error,
                ErrorDescription = errorDescription,
                Custom = custom
            };

            return new TokenErrorResult(response);
        }

        private async Task<IEndpointResult> ProcessTokenRequestAsync(HttpContext context)
        {
            // validate client
            ClientSecretValidationResult clientResult = await _clientValidator.ValidateAsync(context);

            if (clientResult.Client == null) return Error(OidcConstants.TokenErrors.InvalidClient);

            // validate request
            NameValueCollection form = (await context.Request.ReadFormAsync()).AsNameValueCollection();
            TokenRequestValidationResult requestResult = await _requestValidator.ValidateRequestAsync(form, clientResult);

            if (requestResult.IsError)
            {
                await _events.RaiseAsync(new TokenIssuedFailureEvent(requestResult));
                return Error(requestResult.Error, requestResult.ErrorDescription, requestResult.CustomResponse);
            }

            // create response
            TokenResponse response = await _responseGenerator.ProcessAsync(requestResult);

            await _events.RaiseAsync(new TokenIssuedSuccessEvent(response, requestResult));

            // return result
            return new TokenResult(response);
        }
    }
}