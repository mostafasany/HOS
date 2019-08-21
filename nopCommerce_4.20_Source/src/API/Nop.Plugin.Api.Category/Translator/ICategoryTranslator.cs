using Nop.Plugin.Api.Category.Dto;

namespace Nop.Plugin.Api.Category.Translator
{
    public interface ICategoryTranslator
    {
        CategoryDto PrepareCategoryDto(Core.Domain.Catalog.Category category);
    }
}