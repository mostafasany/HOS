using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Api.Category.Dto;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Common.MappingExtensions;

namespace Nop.Plugin.Api.Category
{
    public class ApiMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public ApiMapperConfiguration()
        {
            CreateMap<Core.Domain.Catalog.Category, CategoryDto>();
            CreateMap<CategoryDto, Core.Domain.Catalog.Category>();
        }

        public int Order => 0;


        private static new void CreateMap<TSource, TDestination>()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<TSource, TDestination>()
                .IgnoreAllNonExisting();
        }
    }
}