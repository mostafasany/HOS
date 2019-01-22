using Nop.Plugin.Api.Modules.Manufacturer.Dto;

namespace Nop.Plugin.Api.Modules.Manufacturer.Translator
{
    public interface IManufacturerTransaltor
    {
        ManufacturerDto ConvertToDto(Core.Domain.Catalog.Manufacturer topic);
    }
}