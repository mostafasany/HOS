using Nop.Plugin.Api.Category.Dto;
using Nop.Plugin.Api.Modules.Category.Dto;

namespace Nop.Plugin.Api.Modules.Category.Translator
{
    public interface ICategoryTransaltor
    {
        CategoryDto PrepareCategoryDTO(Core.Domain.Catalog.Category category);
    }
}