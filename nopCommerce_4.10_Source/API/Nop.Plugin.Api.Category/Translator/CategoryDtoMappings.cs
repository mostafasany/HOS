using Nop.Plugin.Api.Category.Dto;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Modules.Category.Dto;

namespace Nop.Plugin.Api.Modules.Category.Translator
{
    public static class CategoryDtoMappings
    {
        public static CategoryDto ToDto(this Core.Domain.Catalog.Category category) => category.MapTo<Core.Domain.Catalog.Category, CategoryDto>();

        public static Core.Domain.Catalog.Category ToEntity(this CategoryDto categoryDto) => categoryDto.MapTo<CategoryDto, Core.Domain.Catalog.Category>();
    }
}