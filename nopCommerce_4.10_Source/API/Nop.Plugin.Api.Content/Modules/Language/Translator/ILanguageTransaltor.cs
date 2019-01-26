using Nop.Plugin.Api.Content.Modules.Language.Dto;

namespace Nop.Plugin.Api.Content.Modules.Language.Translator
{
    public interface ILanguageTransaltor
    {
        LanguageDto PrepateLanguageDto(Core.Domain.Localization.Language language);
    }
}