using System;
using System.Collections.Generic;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Modules.Customer.Dto;

namespace Nop.Plugin.Api.Modules.Customer.Service
{
    public interface ICustomerApiService
    {
        CustomerDto GetCustomerById(int id, bool showDeleted = false);

        Core.Domain.Customers.Customer GetCustomerEntityById(int id);
        int GetCustomersCount();

        IList<CustomerDto> GetCustomersDtos(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId);

        Dictionary<string, string> GetFirstAndLastNameByCustomerId(int customerId);

        IList<CustomerDto> Search(string query = "", string order = Configurations.DefaultOrder,
            int page = Configurations.DefaultPageValue, int limit = Configurations.DefaultLimit);
    }
}