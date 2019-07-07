using System;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Configuration;
using IdentityServer4.Hosting;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nop.Plugin.Api.Common.IdentityServer.Extensions;

namespace Nop.Plugin.Api.Common.IdentityServer.Endpoints
{
    public class AuthorizeResult : IEndpointResult
    {
        private ISystemClock _clock;
        private IMessageStore<ErrorMessage> _errorMessageStore;

        private IdentityServerOptions _options;
        private IUserSession _userSession;

        private const string FormPostHtml = "<form method='post' action='{uri}'>{body}</form><script>(function(){document.forms[0].submit();})();</script>";

        public AuthorizeResult(AuthorizeResponse response) => Response = response ?? throw new ArgumentNullException(nameof(response));

        internal AuthorizeResult(
            AuthorizeResponse response,
            IdentityServerOptions options,
            IUserSession userSession,
            IMessageStore<ErrorMessage> errorMessageStore,
            ISystemClock clock)
            : this(response)
        {
            _options = options;
            _userSession = userSession;
            _errorMessageStore = errorMessageStore;
            _clock = clock;
        }

        public AuthorizeResponse Response { get; }

        public async Task ExecuteAsync(HttpContext context)
        {
            Init(context);

            if (Response.IsError)
                await ProcessErrorAsync(context);
            else
                await ProcessResponseAsync(context);
        }

        protected async Task ProcessResponseAsync(HttpContext context)
        {
            if (!Response.IsError) await _userSession.AddClientIdAsync(Response.Request.ClientId);

            await RenderAuthorizeResponseAsync(context);
        }

        private void AddSecurityHeaders(HttpContext context)
        {
            string formOrigin = Response.Request.RedirectUri.GetOrigin();
            string csp = $"default-src 'none'; frame-ancestors {formOrigin}; form-action {formOrigin}; script-src 'sha256-VuNUSJ59bpCpw62HM2JG/hCyGiqoPN3NqGvNXQPU+rY='; ";

            if (!context.Response.Headers.ContainsKey("Content-Security-Policy")) context.Response.Headers.Add("Content-Security-Policy", csp);

            if (!context.Response.Headers.ContainsKey("X-Content-Security-Policy")) context.Response.Headers.Add("X-Content-Security-Policy", csp);

            var referrer_policy = "no-referrer";
            if (!context.Response.Headers.ContainsKey("Referrer-Policy")) context.Response.Headers.Add("Referrer-Policy", referrer_policy);
        }

        private string BuildRedirectUri()
        {
            string uri = Response.RedirectUri;
            string query = Response.ToNameValueCollection().ToQueryString();

            if (Response.Request.ResponseMode == OidcConstants.ResponseModes.Query)
                uri = uri.AddQueryString(query);
            else
                uri = uri.AddHashFragment(query);

            if (Response.IsError && !uri.Contains("#")) uri += "#_=_";

            return uri;
        }

        private string GetFormPostHtml()
        {
            string html = FormPostHtml;

            html = html.Replace("{uri}", Response.Request.RedirectUri);
            html = html.Replace("{body}", Response.ToNameValueCollection().ToFormPost());

            return html;
        }

        private void Init(HttpContext context)
        {
            _options = _options ?? context.RequestServices.GetRequiredService<IdentityServerOptions>();
            _userSession = _userSession ?? context.RequestServices.GetRequiredService<IUserSession>();
            _errorMessageStore = _errorMessageStore ?? context.RequestServices.GetRequiredService<IMessageStore<ErrorMessage>>();
            _clock = _clock ?? context.RequestServices.GetRequiredService<ISystemClock>();
        }

        private async Task ProcessErrorAsync(HttpContext context)
        {
            // these are the conditions where we can send a response 
            // back directly to the client, otherwise we're only showing the error UI
            bool isPromptNoneError = Response.Error == OidcConstants.AuthorizeErrors.AccountSelectionRequired ||
                                     Response.Error == OidcConstants.AuthorizeErrors.LoginRequired ||
                                     Response.Error == OidcConstants.AuthorizeErrors.ConsentRequired ||
                                     Response.Error == OidcConstants.AuthorizeErrors.InteractionRequired;

            if (Response.Error == OidcConstants.AuthorizeErrors.AccessDenied ||
                isPromptNoneError && Response.Request?.PromptMode == OidcConstants.PromptModes.None
            )
                await ProcessResponseAsync(context);
            else
                await RedirectToErrorPageAsync(context);
        }

        private async Task RedirectToErrorPageAsync(HttpContext context)
        {
            var errorModel = new ErrorMessage
            {
                RequestId = context.TraceIdentifier,
                Error = Response.Error,
                ErrorDescription = Response.ErrorDescription
            };

            var message = new Message<ErrorMessage>(errorModel, _clock.UtcNow.UtcDateTime);
            string id = await _errorMessageStore.WriteAsync(message);

            string errorUrl = _options.UserInteraction.ErrorUrl;

            string url = errorUrl.AddQueryString(_options.UserInteraction.ErrorIdParameter, id);
            context.Response.RedirectToAbsoluteUrl(url);
        }

        private async Task RenderAuthorizeResponseAsync(HttpContext context)
        {
            if (Response.Request.ResponseMode == OidcConstants.ResponseModes.Query ||
                Response.Request.ResponseMode == OidcConstants.ResponseModes.Fragment)
            {
                context.Response.SetNoCache();
                context.Response.Redirect(BuildRedirectUri());
            }
            else if (Response.Request.ResponseMode == OidcConstants.ResponseModes.FormPost)
            {
                context.Response.SetNoCache();
                AddSecurityHeaders(context);
                await context.Response.WriteHtmlAsync(GetFormPostHtml());
            }
            else
            {
                //_logger.LogError("Unsupported response mode.");
                throw new InvalidOperationException("Unsupported response mode");
            }
        }
    }
}