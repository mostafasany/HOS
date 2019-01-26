using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Content.Modules.Language.Dto;

namespace Nop.Plugin.Api.Content.Modules.Language.Translator
{
    public static class LanguageDtoMappings
    {
        public static LanguageDto ToDto(this Core.Domain.Localization.Language language) => language.MapTo<Core.Domain.Localization.Language, LanguageDto>();
    }
}