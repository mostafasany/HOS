using System;
using System.Collections.Generic;
using Nop.Plugin.Api.Common.Constants;

namespace Nop.Plugin.Api.Category.Service
{
    public interface ICategoryApiService
    {
        IEnumerable<Core.Domain.Catalog.Category> GetCategories(IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null,
            DateTime? updatedAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId,
            int? productId = null, int? parentId = null, bool? publishedStatus = null);

        IEnumerable<Core.Domain.Catalog.Category> GetNewCategories(IList<int> ids = null,
           DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null,
           DateTime? updatedAtMax = null,
           int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
           int sinceId = Configurations.DefaultSinceId,
           int? productId = null, int? parentId = null, bool? publishedStatus = null);

        int GetCategoriesCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            bool? publishedStatus = null, int? productId = null);

        Core.Domain.Catalog.Category GetCategoryById(int categoryId);

        Core.Domain.Catalog.Category GetNewCategoryById(int categoryId);
    }
}