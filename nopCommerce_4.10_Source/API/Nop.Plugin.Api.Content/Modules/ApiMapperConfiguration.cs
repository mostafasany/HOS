using AutoMapper;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Common.MappingExtensions;
using Nop.Plugin.Api.Content.Modules.Language.Dto;
using Nop.Plugin.Api.Modules.Store.Dto;

namespace Nop.Plugin.Api.Content.Modules
{
    public class ApiMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public ApiMapperConfiguration()
        {
            CreateMap<Store, StoreDto>();

            CreateMap<Core.Domain.Localization.Language, LanguageDto>();
        }

        public int Order => 0;

        private new static void CreateMap<TSource, TDestination>()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<TSource, TDestination>()
                .IgnoreAllNonExisting();
        }
    }
}