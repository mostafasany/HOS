using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Customer.Modules.Customer.Dto;

namespace Nop.Plugin.Api.Customer.Modules.Customer.Translator
{
    public static class CustomerDtoMappings
    {
        public static CustomerForShoppingCartItemDto ToCustomerForShoppingCartItemDto(this Core.Domain.Customers.Customer customer) => customer.MapTo<Core.Domain.Customers.Customer, CustomerForShoppingCartItemDto>();

        public static CustomerDto ToDto(this Core.Domain.Customers.Customer customer) => customer.MapTo<Core.Domain.Customers.Customer, CustomerDto>();

        public static OrderCustomerDto ToOrderCustomerDto(this Core.Domain.Customers.Customer customer) => customer.MapTo<Core.Domain.Customers.Customer, OrderCustomerDto>();

        public static OrderCustomerDto ToOrderCustomerDto(this CustomerDto customerDto) => customerDto.MapTo<CustomerDto, OrderCustomerDto>();
    }
}