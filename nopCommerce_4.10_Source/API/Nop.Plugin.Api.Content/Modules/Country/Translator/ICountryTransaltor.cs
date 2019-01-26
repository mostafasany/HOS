using Nop.Core.Domain.Directory;
using Nop.Plugin.Api.Content.Modules.Country.Dto;

namespace Nop.Plugin.Api.Content.Modules.Country.Translator
{
    public interface ICountryTransaltor
    {
        StateProvinceDto ConvertToDto(StateProvince state);
    }
}