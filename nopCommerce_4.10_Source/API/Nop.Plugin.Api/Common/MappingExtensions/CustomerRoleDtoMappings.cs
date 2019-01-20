using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Modules.CustomerRoles.Dto;

namespace Nop.Plugin.Api.Common.MappingExtensions
{
    public static class CustomerRoleDtoMappings
    {
        public static CustomerRoleDto ToDto(this CustomerRole customerRole) => customerRole.MapTo<CustomerRole, CustomerRoleDto>();
    }
}