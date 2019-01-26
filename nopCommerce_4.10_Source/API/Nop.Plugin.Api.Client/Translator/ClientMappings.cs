using Nop.Plugin.Api.Admin.Model;
using Nop.Plugin.Api.Common.AutoMapper;

namespace Nop.Plugin.Api.Admin.Translator
{
    public static class ClientMappings
    {
        public static ClientApiModel ToApiModel(this IdentityServer4.EntityFramework.Entities.Client client) => client.MapTo<IdentityServer4.EntityFramework.Entities.Client, ClientApiModel>();
    }
}