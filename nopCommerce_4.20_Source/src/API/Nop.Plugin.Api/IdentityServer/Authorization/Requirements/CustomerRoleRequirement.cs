using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Common.Constants;
using Nop.Services.Customers;

namespace Nop.Plugin.Api.IdentityServer.Authorization.Requirements
{
    public class CustomerRoleRequirement : IAuthorizationRequirement
    {
        public bool IsCustomerInRole()
        {
            try
            {
                var httpContextAccessor = EngineContext.Current.Resolve<IHttpContextAccessor>();

                var customerIdClaim =
                    httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(m =>
                        m.Type == ClaimTypes.NameIdentifier);

                if (customerIdClaim != null && Guid.TryParse(customerIdClaim.Value, out var customerGuid))
                {
                    var customerService = EngineContext.Current.Resolve<ICustomerService>();

                    var customer = customerService.GetCustomerByGuid(customerGuid);

                    if (customer != null) return IsInApiRole(customer.CustomerRoles);
                }
            }
            catch
            {
                // best effort
            }

            return false;
        }

        private static bool IsInApiRole(IEnumerable<CustomerRole> customerRoles)
        {
            return customerRoles.FirstOrDefault(cr => cr.SystemName == Constants.Roles.ApiRoleSystemName) != null;
        }

        private static bool IsInAdminRole(IEnumerable<CustomerRole> customerRoles)
        {
            return customerRoles.FirstOrDefault(cr =>
                       cr.SystemName == Constants.Roles.AdminRoleName
                       || cr.SystemName == Constants.Roles.ForumModeratorsRoleName
                       || cr.SystemName == Constants.Roles.RegisteredRoleName
                       || cr.SystemName == Constants.Roles.GuestsRoleName
                   ) != null;
        }

        private static bool IsInRegisterRole(IEnumerable<CustomerRole> customerRoles)
        {
            return customerRoles.FirstOrDefault(cr =>
                       cr.SystemName == Constants.Roles.RegisteredRoleName
                       || cr.SystemName == Constants.Roles.GuestsRoleName
                   ) != null;
        }

        private static bool IsInForumModeratorRole(IEnumerable<CustomerRole> customerRoles)
        {
            return customerRoles.FirstOrDefault(cr =>
                       cr.SystemName == Constants.Roles.ForumModeratorsRoleName
                       || cr.SystemName == Constants.Roles.GuestsRoleName
                   ) != null;
        }

        private static bool IsInVendorRole(IEnumerable<CustomerRole> customerRoles)
        {
            return customerRoles.FirstOrDefault(cr =>
                       cr.SystemName == Constants.Roles.VendorsRoleName
                       || cr.SystemName == Constants.Roles.GuestsRoleName
                   ) != null;
        }

        private static bool IsInGuestRole(IEnumerable<CustomerRole> customerRoles)
        {
            return customerRoles.FirstOrDefault(cr =>
                       cr.SystemName == Constants.Roles.GuestsRoleName
                   ) != null;
        }
    }
}