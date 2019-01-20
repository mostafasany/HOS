using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.Constants;

namespace Nop.Plugin.Api.Modules.Categories.Service
{
    public interface ICategoryApiService
    {
        IList<Category> GetCategories(IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            int? productId = null, int? parenttId = null, bool? publishedStatus = null);

        int GetCategoriesCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            bool? publishedStatus = null, int? productId = null);

        Category GetCategoryById(int categoryId);
    }
}