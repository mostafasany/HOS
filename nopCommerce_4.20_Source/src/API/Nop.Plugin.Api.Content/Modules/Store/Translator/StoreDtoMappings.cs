﻿using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Content.Modules.Store.Dto;

namespace Nop.Plugin.Api.Content.Modules.Store.Translator
{
    public static class StoreDtoMappings
    {
        public static StoreDto ToDto(this Core.Domain.Stores.Store store)
        {
            return store.MapTo<Core.Domain.Stores.Store, StoreDto>();
        }
    }
}