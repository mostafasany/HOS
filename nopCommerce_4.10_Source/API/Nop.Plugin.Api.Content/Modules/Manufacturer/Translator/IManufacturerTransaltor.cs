using Nop.Plugin.Api.Content.Modules.Manufacturer.Dto;

namespace Nop.Plugin.Api.Content.Modules.Manufacturer.Translator
{
    public interface IManufacturerTransaltor
    {
        ManufacturerDto ConvertToDto(Core.Domain.Catalog.Manufacturer topic);
    }
}