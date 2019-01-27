using System;
using System.Collections.Generic;
using Nop.Plugin.Api.Common.Constants;

namespace Nop.Plugin.Api.Category.Service
{
    public interface ICategoryApiService
    {
        IList<Core.Domain.Catalog.Category> GetCategories(IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            int? productId = null, int? parenttId = null, bool? publishedStatus = null);

        int GetCategoriesCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            bool? publishedStatus = null, int? productId = null);

        Core.Domain.Catalog.Category GetCategoryById(int categoryId);
    }
}