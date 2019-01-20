using IdentityServer4.EntityFramework.Entities;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Common.Models;

namespace Nop.Plugin.Api.Common.MappingExtensions
{
    public static class ClientMappings
    {
        public static ClientApiModel ToApiModel(this Client client) => client.MapTo<Client, ClientApiModel>();
    }
}