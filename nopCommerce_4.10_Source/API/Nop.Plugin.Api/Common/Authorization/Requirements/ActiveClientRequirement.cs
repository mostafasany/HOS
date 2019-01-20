using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Common.Models;
using Nop.Plugin.Api.Modules.Client.Service;

namespace Nop.Plugin.Api.Common.Authorization.Requirements
{
    public class ActiveClientRequirement : IAuthorizationRequirement
    {
        public bool IsClientActive()
        {
            if (!ClientExistsAndActive()) return false;

            return true;
        }

        private bool ClientExistsAndActive()
        {
            var httpContextAccessor = EngineContext.Current.Resolve<IHttpContextAccessor>();

            string clientId =
                httpContextAccessor.HttpContext.User.FindFirst("client_id")?.Value;

            if (clientId != null)
            {
                var clientService = EngineContext.Current.Resolve<IClientService>();
                ClientApiModel client = clientService.FindClientByClientId(clientId);

                if (client != null && client.Enabled) return true;
            }

            return false;
        }
    }
}