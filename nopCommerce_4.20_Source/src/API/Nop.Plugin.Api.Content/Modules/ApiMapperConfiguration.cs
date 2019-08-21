using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Common.MappingExtensions;
using Nop.Plugin.Api.Content.Modules.Language.Dto;
using Nop.Plugin.Api.Content.Modules.Store.Dto;

namespace Nop.Plugin.Api.Content.Modules
{
    public class ApiMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public ApiMapperConfiguration()
        {
            CreateMap<Core.Domain.Stores.Store, StoreDto>();

            CreateMap<Core.Domain.Localization.Language, LanguageDto>();
        }

        public int Order => 0;

        private static new void CreateMap<TSource, TDestination>()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<TSource, TDestination>()
                .IgnoreAllNonExisting();
        }
    }
}