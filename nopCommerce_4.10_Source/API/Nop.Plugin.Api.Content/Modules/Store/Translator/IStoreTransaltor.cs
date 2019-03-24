using Nop.Plugin.Api.Modules.Store.Dto;

namespace Nop.Plugin.Api.Modules.Store.Translator
{
    public interface IStoreTransaltor
    {
        StoreDto PrepareStoreDTO(Core.Domain.Stores.Store store);
    }
}