using AutoMapper;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Api.Cart.Dto;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Common.MappingExtensions;

namespace Nop.Plugin.Api.Cart
{
    public class ApiMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public ApiMapperConfiguration()
        {
            CreateMap<ShoppingCartItem, ShoppingCartItemDto>();
        }

        public int Order => 0;

        private static new void CreateMap<TSource, TDestination>()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<TSource, TDestination>()
                .IgnoreAllNonExisting();
        }
    }
}