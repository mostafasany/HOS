using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Modules.Products.Dto;

namespace Nop.Plugin.Api.Modules.Products.Service
{
    public interface IProductApiService
    {
        Product GetProductById(int productId);

        Product GetProductByIdNoTracking(int productId);

        Tuple<IList<Product>, List<ProductsFiltersDto>> GetProducts(IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            int? categoryId = null, string categorySlug = null, string vendorName = null, string manufacturerName = null, string keyword = null, bool? publishedStatus = null);

        int GetProductsCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, bool? publishedStatus = null,
            string vendorName = null, string keyword = null, int? categoryId = null);

        List<Product> GetRelatedProducts(int productId1, bool showHidden = false);
    }
}