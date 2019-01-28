using AutoMapper;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Common.MappingExtensions;
using Nop.Plugin.Api.Modules.Cart.Dto;

namespace Nop.Plugin.Api
{
    public class ApiMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public ApiMapperConfiguration()
        {
            CreateMap<ShoppingCartItem, ShoppingCartItemDto>();
        }

        public int Order => 0;

        private new static void CreateMap<TSource, TDestination>()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<TSource, TDestination>()
                .IgnoreAllNonExisting();
        }
    }
}