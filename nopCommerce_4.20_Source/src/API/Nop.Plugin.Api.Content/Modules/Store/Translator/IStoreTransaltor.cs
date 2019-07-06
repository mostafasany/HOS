using Nop.Plugin.Api.Content.Modules.Store.Dto;

namespace Nop.Plugin.Api.Content.Modules.Store.Translator
{
    public interface IStoreTransaltor
    {
        StoreDto PrepareStoreDTO(Core.Domain.Stores.Store store);
    }
}