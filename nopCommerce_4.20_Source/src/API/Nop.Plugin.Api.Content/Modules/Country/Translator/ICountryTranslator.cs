using Nop.Core.Domain.Directory;
using Nop.Plugin.Api.Content.Modules.Country.Dto;

namespace Nop.Plugin.Api.Content.Modules.Country.Translator
{
    public interface ICountryTranslator
    {
        StateProvinceDto ToDto(StateProvince state);
    }
}