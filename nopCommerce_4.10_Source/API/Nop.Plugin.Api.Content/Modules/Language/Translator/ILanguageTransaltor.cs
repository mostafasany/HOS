using Nop.Plugin.Api.Modules.Language.Dto;

namespace Nop.Plugin.Api.Modules.Language.Translator
{
    public interface ILanguageTransaltor
    {
        LanguageDto PrepateLanguageDto(Core.Domain.Localization.Language language);
    }
}