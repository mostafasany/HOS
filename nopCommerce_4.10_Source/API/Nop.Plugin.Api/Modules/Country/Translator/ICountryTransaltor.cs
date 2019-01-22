using Nop.Core.Domain.Directory;
using Nop.Plugin.Api.Modules.Country.Dto;

namespace Nop.Plugin.Api.Modules.Country.Translator
{
    public interface ICountryTransaltor
    {
        StateProvinceDto ConvertToDto(StateProvince state);
    }
}