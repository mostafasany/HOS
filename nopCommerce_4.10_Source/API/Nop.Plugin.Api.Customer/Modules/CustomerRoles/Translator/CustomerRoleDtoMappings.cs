using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Customer.Modules.CustomerRoles.Dto;

namespace Nop.Plugin.Api.Customer.Modules.CustomerRoles.Translator
{
    public static class CustomerRoleDtoMappings
    {
        public static CustomerRoleDto ToDto(this CustomerRole customerRole) => customerRole.MapTo<CustomerRole, CustomerRoleDto>();
    }
}