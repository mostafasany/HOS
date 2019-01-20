﻿using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.Constants;

namespace Nop.Plugin.Api.Modules.ProductsCategoryMappings.Service
{
    public interface IProductCategoryMappingsApiService
    {
        ProductCategory GetById(int id);

        IList<ProductCategory> GetMappings(int? productId = null, int? categoryId = null, int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId);

        int GetMappingsCount(int? productId = null, int? categoryId = null);
    }
}