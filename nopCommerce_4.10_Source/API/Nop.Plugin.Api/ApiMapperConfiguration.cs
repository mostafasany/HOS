using System.Linq;
using System.Net;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Api.Admin.Model;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Common.Domain;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Common.DTOs.Product;
using Nop.Plugin.Api.Common.MappingExtensions;
using Nop.Plugin.Api.Common.Models;
using Nop.Plugin.Api.Content.Modules.Language.Dto;
using Nop.Plugin.Api.Modules.Cart.Dto;
using Nop.Plugin.Api.Modules.Customer.Translator;
using Nop.Plugin.Api.Modules.Product.Translator;
using Nop.Plugin.Api.Modules.ProductAttributes.Dto;
using Nop.Plugin.Api.Modules.ProductCategoryMappings.Dto;
using Nop.Plugin.Api.Modules.Store.Dto;

namespace Nop.Plugin.Api
{
    public class ApiMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public ApiMapperConfiguration()
        {
            CreateMap<ApiSettings, ConfigurationModel>();
            CreateMap<ConfigurationModel, ApiSettings>();

            CreateMap<Store, StoreDto>();

            CreateMap<ProductCategory, ProductCategoryMappingDto>();

            CreateMap<Language, LanguageDto>();

            CreateClientToClientApiModelMap();

            CreateAddressMap();
            CreateAddressDtoToEntityMap();
            CreateShoppingCartItemMap();

            CreateProductMap();

            CreateMap<ProductAttributeValue, ProductAttributeValueDto>();

            CreateMap<ProductAttribute, ProductAttributeDto>();

            CreateMap<ProductSpecificationAttribute, ProductSpecificationAttributeDto>();

            CreateMap<SpecificationAttribute, SpecificationAttributeDto>();
            CreateMap<SpecificationAttributeOption, SpecificationAttributeOptionDto>();
        }

        public int Order => 0;

        private void CreateAddressDtoToEntityMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<AddressDto, Address>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Id, y => y.MapFrom(src => src.Id));
        }

        private void CreateAddressMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Address, AddressDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.Id, y => y.MapFrom(src => src.Id))
                .ForMember(x => x.CountryName,
                    y => y.MapFrom(src => src.Country.GetWithDefault(x => x, new Country()).Name))
                .ForMember(x => x.StateProvinceName,
                    y => y.MapFrom(src => src.StateProvince.GetWithDefault(x => x, new StateProvince()).Name));
        }

        private static void CreateClientToClientApiModelMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Client, ClientApiModel>()
                .ForMember(x => x.ClientSecret, y => y.MapFrom(src => src.ClientSecrets.FirstOrDefault().Description))
                .ForMember(x => x.RedirectUrl, y => y.MapFrom(src => src.RedirectUris.FirstOrDefault().RedirectUri))
                .ForMember(x => x.AccessTokenLifetime, y => y.MapFrom(src => src.AccessTokenLifetime))
                .ForMember(x => x.RefreshTokenLifetime, y => y.MapFrom(src => src.AbsoluteRefreshTokenLifetime));
        }


        private new static void CreateMap<TSource, TDestination>()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<TSource, TDestination>()
                .IgnoreAllNonExisting();
        }


        private void CreateProductMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Product, ProductDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.ProductAttributeMappings, y => y.Ignore())
                .ForMember(x => x.FullDescription, y => y.MapFrom(src => WebUtility.HtmlEncode(src.FullDescription)))
                .ForMember(x => x.Tags,
                    y => y.MapFrom(src => src.ProductProductTagMappings.Select(x => x.ProductTag.Name)));
        }

        private void CreateShoppingCartItemMap()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<ShoppingCartItem, ShoppingCartItemDto>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.CustomerDto,
                    y => y.MapFrom(src =>
                        src.Customer.GetWithDefault(x => x, new Core.Domain.Customers.Customer()).ToCustomerForShoppingCartItemDto()))
                .ForMember(x => x.ProductDto,
                    y => y.MapFrom(src => src.Product.GetWithDefault(x => x, new Product()).ToDto()));
        }
    }
}