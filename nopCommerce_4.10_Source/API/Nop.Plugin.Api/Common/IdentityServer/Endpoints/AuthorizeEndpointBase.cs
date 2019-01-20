using System.Collections.Specialized;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Events;
using IdentityServer4.Hosting;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Api.Common.IdentityServer.Endpoints
{
    public abstract class AuthorizeEndpointBase : IEndpointHandler
    {
        private readonly IAuthorizeResponseGenerator _authorizeResponseGenerator;
        private readonly IEventService _events;
        private readonly IAuthorizeInteractionResponseGenerator _interactionGenerator;
        private readonly IAuthorizeRequestValidator _validator;

        protected AuthorizeEndpointBase(IEventService events, IUserSession userSession, IAuthorizeRequestValidator validator, IAuthorizeResponseGenerator authorizeResponseGenerator, IAuthorizeInteractionResponseGenerator interactionGenerator)
        {
            _events = events;
            UserSession = userSession;
            _validator = validator;
            _authorizeResponseGenerator = authorizeResponseGenerator;
            _interactionGenerator = interactionGenerator;
        }

        protected IUserSession UserSession { get; }

        public abstract Task<IEndpointResult> ProcessAsync(HttpContext context);

        protected async Task<IEndpointResult> CreateErrorResultAsync(
            string logMessage,
            ValidatedAuthorizeRequest request = null,
            string error = OidcConstants.AuthorizeErrors.ServerError,
            string errorDescription = null)
        {
            await RaiseFailureEventAsync(request, error, errorDescription);

            return new AuthorizeResult(new AuthorizeResponse
            {
                Request = request,
                Error = error,
                ErrorDescription = errorDescription
            });
        }

        protected async Task<IEndpointResult> CreateErrorResultAsync(
            ValidatedAuthorizeRequest request = null,
            string error = OidcConstants.AuthorizeErrors.ServerError,
            string errorDescription = null)
        {
            await RaiseFailureEventAsync(request, error, errorDescription);

            return new AuthorizeResult(new AuthorizeResponse
            {
                Request = request,
                Error = error,
                ErrorDescription = errorDescription
            });
        }

        protected async Task<IEndpointResult> ProcessAuthorizeRequestAsync(NameValueCollection parameters, ClaimsPrincipal user, ConsentResponse consent)
        {
            // validate request
            AuthorizeRequestValidationResult result = await _validator.ValidateAsync(parameters, user);
            if (result.IsError)
                return await CreateErrorResultAsync(
                    "Request validation failed",
                    result.ValidatedRequest,
                    result.Error,
                    result.ErrorDescription);

            ValidatedAuthorizeRequest request = result.ValidatedRequest;

            // determine user interaction
            InteractionResponse interactionResult = await _interactionGenerator.ProcessInteractionAsync(request, consent);
            if (interactionResult.IsError) return await CreateErrorResultAsync("Interaction generator error", request, interactionResult.Error);
            if (interactionResult.IsLogin) return new LoginPageResult(request);
            if (interactionResult.IsConsent) return new ConsentPageResult(request);
            if (interactionResult.IsRedirect) return new CustomRedirectResult(request, interactionResult.RedirectUrl);

            AuthorizeResponse response = await _authorizeResponseGenerator.CreateResponseAsync(request);

            await RaiseResponseEventAsync(response);

            return new AuthorizeResult(response);
        }

        protected Task RaiseFailureEventAsync(ValidatedAuthorizeRequest request, string error, string errorDescription) => _events.RaiseAsync(new TokenIssuedFailureEvent(request, error, errorDescription));

        protected Task RaiseResponseEventAsync(AuthorizeResponse response)
        {
            if (!response.IsError)
                return _events.RaiseAsync(new TokenIssuedSuccessEvent(response));
            return RaiseFailureEventAsync(response.Request, response.Error, response.ErrorDescription);
        }
    }
}