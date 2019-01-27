using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Common.MappingExtensions;
using Nop.Plugin.Api.Customer.Modules.CustomerRoles.Dto;
using Nop.Plugin.Api.Customer.Modules.Order.Dto.OrderItems;
using Nop.Plugin.Api.Customer.Modules.Order.Dto.Orders;
using Nop.Plugin.Api.Customer.Modules.Order.Translator;
using Nop.Plugin.Api.Modules.Customer.Dto;
using Nop.Plugin.Api.Modules.NewsLetterSubscription.Dto;

namespace Nop.Plugin.Api.Customer.Modules
{
    public class ApiMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public ApiMapperConfiguration()
        {
            CreateMap<CustomerRole, CustomerRoleDto>();

            CreateCustomerToDTOMap();
            CreateCustomerToOrderCustomerDTOMap();
            CreateCustomerDTOToOrderCustomerDTOMap();
            CreateCustomerForShoppingCartItemMapFromCustomer();

            CreateMap<OrderItem, OrderItemDto>();
            CreateOrderEntityToOrderDtoMap();

            CreateMap<NewsLetterSubscriptionDto, NewsLetterSubscription>();
            CreateMap<NewsLetterSubscription, NewsLetterSubscriptionDto>();
        }

        public int Order => 0;


        private void CreateCustomerDTOToOrderCustomerDTOMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<CustomerDto, OrderCustomerDto>()
                .IgnoreAllNonExisting();
        }

        private void CreateCustomerForShoppingCartItemMapFromCustomer()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression
                .CreateMap<Core.Domain.Customers.Customer, CustomerForShoppingCartItemDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Id, y => y.MapFrom(src => src.Id))
                .ForMember(x => x.BillingAddress,
                    y => y.MapFrom(src => src.BillingAddress.GetWithDefault(x => x, new Address()).ToDto()))
                .ForMember(x => x.ShippingAddress,
                    y => y.MapFrom(src => src.ShippingAddress.GetWithDefault(x => x, new Address()).ToDto()))
                .ForMember(x => x.Addresses,
                    y => y.MapFrom(src =>
                        src.Addresses.GetWithDefault(x => x, new List<Address>()).Select(address => address.ToDto())));
        }

        private void CreateCustomerToDTOMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Core.Domain.Customers.Customer, CustomerDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Id, y => y.MapFrom(src => src.Id))
                .ForMember(x => x.BillingAddress,
                    y => y.MapFrom(src => src.BillingAddress.GetWithDefault(x => x, new Address()).ToDto()))
                .ForMember(x => x.ShippingAddress,
                    y => y.MapFrom(src => src.ShippingAddress.GetWithDefault(x => x, new Address()).ToDto()))
                .ForMember(x => x.Addresses,
                    y =>
                        y.MapFrom(
                            src =>
                                src.Addresses.GetWithDefault(x => x, new List<Address>())
                                    .Select(address => address.ToDto())))
                .ForMember(x => x.RoleIds, y => y.MapFrom(src => src.CustomerRoles.Select(z => z.Id)));
        }

        private void CreateCustomerToOrderCustomerDTOMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Core.Domain.Customers.Customer, OrderCustomerDto>()
                .IgnoreAllNonExisting();
        }

        private new static void CreateMap<TSource, TDestination>()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<TSource, TDestination>()
                .IgnoreAllNonExisting();
        }

        private void CreateOrderEntityToOrderDtoMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Core.Domain.Orders.Order, OrderDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Id, y => y.MapFrom(src => src.Id))
                .ForMember(x => x.OrderItems, y => y.MapFrom(src => src.OrderItems.Select(x => x.ToDto())));
        }
    }
}