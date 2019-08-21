using Nop.Plugin.Api.Content.Modules.Store.Dto;

namespace Nop.Plugin.Api.Content.Modules.Store.Translator
{
    public interface IStoreTranslator
    {
        StoreDto ToDto(Core.Domain.Stores.Store store);
    }
}