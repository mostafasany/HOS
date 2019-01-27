using IdentityServer4.EntityFramework.Entities;
using Nop.Plugin.Api.Admin.Model;
using Nop.Plugin.Api.Common.AutoMapper;

namespace Nop.Plugin.Api.Admin.Translator
{
    public static class ClientMappings
    {
        public static ClientApiModel ToApiModel(this Client client) => client.MapTo<Client, ClientApiModel>();
    }
}