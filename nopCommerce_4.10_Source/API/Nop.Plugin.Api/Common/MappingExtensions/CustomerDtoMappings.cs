using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Modules.Customers.Dto;

namespace Nop.Plugin.Api.Common.MappingExtensions
{
    public static class CustomerDtoMappings
    {
        public static CustomerDto ToDto(this Customer customer)
        {
            return customer.MapTo<Customer, CustomerDto>();
        }

        public static OrderCustomerDto ToOrderCustomerDto(this Customer customer)
        {
            return customer.MapTo<Customer, OrderCustomerDto>();
        }

        public static OrderCustomerDto ToOrderCustomerDto(this CustomerDto customerDto)
        {
            return customerDto.MapTo<CustomerDto, OrderCustomerDto>();
        }

        public static CustomerForShoppingCartItemDto ToCustomerForShoppingCartItemDto(this Customer customer)
        {
            return customer.MapTo<Customer, CustomerForShoppingCartItemDto>();
        }
    }
}