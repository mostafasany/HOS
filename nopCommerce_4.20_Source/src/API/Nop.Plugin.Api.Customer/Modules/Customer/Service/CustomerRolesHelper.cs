using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;

namespace Nop.Plugin.Api.Customer.Modules.Customer.Service
{
    public class CustomerRolesHelper : ICustomerRolesHelper
    {
        private const string CustomerRolesAllKey = "Nop.customerrole.all-{0}";
        private readonly ICacheManager _cacheManager;

        private readonly ICustomerService _customerService;

        public CustomerRolesHelper(ICustomerService customerService, ICacheManager cacheManager)
        {
            _customerService = customerService;
            _cacheManager = cacheManager;
        }

        public IList<CustomerRole> GetValidCustomerRoles(List<int> roleIds)
        {
            // This is needed because the caching mess up the entity framework context
            // and when you try to send something TO the database it throws an exception.
            //TODO:4.2 Migration
            //_cacheManager.RemoveByPattern(CustomerRolesAllKey);
            _cacheManager.Remove(CustomerRolesAllKey);

            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);

            return allCustomerRoles.Where(customerRole => roleIds != null && roleIds.Contains(customerRole.Id))
                .ToList();
        }

        public bool IsInGuestsRole(IList<CustomerRole> customerRoles)
        {
            return customerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.GuestsRoleName) != null;
        }

        public bool IsInRegisteredRole(IList<CustomerRole> customerRoles)
        {
            return customerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.RegisteredRoleName) != null;
        }
    }
}