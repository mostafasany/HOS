using System;
using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Modules.Customers.Dto;

namespace Nop.Plugin.Api.Modules.Customers.Service
{
    public interface ICustomerApiService
    {
        CustomerDto GetCustomerById(int id, bool showDeleted = false);

        Customer GetCustomerEntityById(int id);
        int GetCustomersCount();

        IList<CustomerDto> GetCustomersDtos(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId);

        Dictionary<string, string> GetFirstAndLastNameByCustomerId(int customerId);

        IList<CustomerDto> Search(string query = "", string order = Configurations.DefaultOrder,
            int page = Configurations.DefaultPageValue, int limit = Configurations.DefaultLimit);
    }
}