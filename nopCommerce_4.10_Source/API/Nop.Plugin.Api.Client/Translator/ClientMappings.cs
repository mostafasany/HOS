using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Modules.Client.Model;

namespace Nop.Plugin.Api.Modules.Client.Translator
{
    public static class ClientMappings
    {
        public static ClientApiModel ToApiModel(this IdentityServer4.EntityFramework.Entities.Client client) => client.MapTo<IdentityServer4.EntityFramework.Entities.Client, ClientApiModel>();
    }
}