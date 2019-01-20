using Nop.Core.Domain.Localization;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Modules.Languages.Dto;

namespace Nop.Plugin.Api.Common.MappingExtensions
{
    public static class LanguageDtoMappings
    {
        public static LanguageDto ToDto(this Language language)
        {
            return language.MapTo<Language, LanguageDto>();
        }
    }
}
