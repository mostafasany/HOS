using System;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.Factories;
using Nop.Services.Catalog;

namespace Nop.Plugin.Api.Category.Factory
{
    public class CategoryFactory : IFactory<Core.Domain.Catalog.Category>
    {
        private readonly CatalogSettings _catalogSettings;
        private readonly ICategoryTemplateService _categoryTemplateService;

        public CategoryFactory(ICategoryTemplateService categoryTemplateService, CatalogSettings catalogSettings)
        {
            _categoryTemplateService = categoryTemplateService;
            _catalogSettings = catalogSettings;
        }

        public Core.Domain.Catalog.Category Initialize()
        {
            // TODO: cache the default entity.
            var defaultCategory = new Core.Domain.Catalog.Category();

            // Set the first template as the default one.
            CategoryTemplate firstTemplate = _categoryTemplateService.GetAllCategoryTemplates().FirstOrDefault();

            if (firstTemplate != null) defaultCategory.CategoryTemplateId = firstTemplate.Id;

            //default values
            defaultCategory.PageSize = _catalogSettings.DefaultCategoryPageSize;
            defaultCategory.PageSizeOptions = _catalogSettings.DefaultCategoryPageSizeOptions;
            defaultCategory.Published = true;
            defaultCategory.IncludeInTopMenu = true;
            defaultCategory.AllowCustomersToSelectPageSize = true;

            defaultCategory.CreatedOnUtc = DateTime.UtcNow;
            defaultCategory.UpdatedOnUtc = DateTime.UtcNow;

            return defaultCategory;
        }
    }
}