using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;

namespace Nop.Plugin.Api.Customer.Helpers
{
    public class CustomerRolesHelper : ICustomerRolesHelper
    {
        private readonly ICacheManager _cacheManager;

        private readonly ICustomerService _customerService;
        private const string CUSTOMERROLES_ALL_KEY = "Nop.customerrole.all-{0}";

        public CustomerRolesHelper(ICustomerService customerService, ICacheManager cacheManager)
        {
            _customerService = customerService;
            _cacheManager = cacheManager;
        }

        public IList<CustomerRole> GetValidCustomerRoles(List<int> roleIds)
        {
            // This is needed because the caching messeup the entity framework context
            // and when you try to send something TO the database it throws an exeption.
            _cacheManager.RemoveByPattern(CUSTOMERROLES_ALL_KEY);

            IList<CustomerRole> allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            var newCustomerRoles = new List<CustomerRole>();
            foreach (CustomerRole customerRole in allCustomerRoles)
                if (roleIds != null && roleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);

            return newCustomerRoles;
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