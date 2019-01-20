using Nop.Core.Domain.Stores;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Modules.Stores.Dto;

namespace Nop.Plugin.Api.Common.MappingExtensions
{
    public static class StoreDtoMappings
    {
        public static StoreDto ToDto(this Store store)
        {
            return store.MapTo<Store, StoreDto>();
        }
    }
}
