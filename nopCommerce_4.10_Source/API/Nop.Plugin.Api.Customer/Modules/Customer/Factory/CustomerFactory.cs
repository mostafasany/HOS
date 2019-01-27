using System;
using Nop.Plugin.Api.Common.Factories;

namespace Nop.Plugin.Api.Modules.Customer.Factory
{
    public class CustomerFactory : IFactory<Core.Domain.Customers.Customer>
    {
        public Core.Domain.Customers.Customer Initialize()
        {
            var defaultCustomer = new Core.Domain.Customers.Customer
            {
                CustomerGuid = Guid.NewGuid(),
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                Active = true
            };

            return defaultCustomer;
        }
    }
}