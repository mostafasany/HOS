using System.Collections.Generic;
using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Api.Customer.Modules.Customer.Service
{
    public interface ICustomerRolesHelper
    {
        IList<CustomerRole> GetValidCustomerRoles(List<int> roleIds);
        bool IsInGuestsRole(IList<CustomerRole> customerRoles);
        bool IsInRegisteredRole(IList<CustomerRole> customerRoles);
    }
}