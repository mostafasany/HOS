using Nop.Plugin.Api.Content.Modules.Manufacturer.Dto;

namespace Nop.Plugin.Api.Content.Modules.Manufacturer.Translator
{
    public interface IManufacturerTranslator
    {
        ManufacturerDto ToDto(Core.Domain.Catalog.Manufacturer topic);
    }
}