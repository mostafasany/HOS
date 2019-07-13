using Nop.Plugin.Api.Content.Modules.Language.Dto;

namespace Nop.Plugin.Api.Content.Modules.Language.Translator
{
    public interface ILanguageTranslator
    {
        LanguageDto ToDto(Core.Domain.Localization.Language language);
    }
}