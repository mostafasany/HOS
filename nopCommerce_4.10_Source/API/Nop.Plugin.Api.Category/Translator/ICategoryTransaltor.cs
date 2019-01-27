using Nop.Plugin.Api.Category.Dto;

namespace Nop.Plugin.Api.Category.Translator
{
    public interface ICategoryTransaltor
    {
        CategoryDto PrepareCategoryDTO(Core.Domain.Catalog.Category category);
    }
}