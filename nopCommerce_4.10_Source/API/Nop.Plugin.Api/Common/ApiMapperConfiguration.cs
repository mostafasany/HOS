using AutoMapper;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Common.Domain;
using Nop.Plugin.Api.Common.DTOs;
using Nop.Plugin.Api.Common.MappingExtensions;
using Nop.Plugin.Api.Common.Models;

namespace Nop.Plugin.Api.Common
{
    public class ApiMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public ApiMapperConfiguration()
        {
            CreateMap<ApiSettings, ConfigurationModel>();

            CreateMap<ConfigurationModel, ApiSettings>();

            CreateAddressMap();

            CreateAddressDtoToEntityMap();
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

        private new static void CreateMap<TSource, TDestination>()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<TSource, TDestination>()
                .IgnoreAllNonExisting();
        }
    }
}