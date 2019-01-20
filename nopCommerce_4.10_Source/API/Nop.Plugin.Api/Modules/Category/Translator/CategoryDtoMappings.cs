using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.AutoMapper;
using Nop.Plugin.Api.Modules.Categories.Dto;

namespace Nop.Plugin.Api.Common.MappingExtensions
{
    public static class CategoryDtoMappings
    {
        public static CategoryDto ToDto(this Category category) => category.MapTo<Category, CategoryDto>();

        public static Category ToEntity(this CategoryDto categoryDto) => categoryDto.MapTo<CategoryDto, Category>();
    }
}