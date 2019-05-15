using System;
using System.Collections.Generic;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.DTOs.Product;

namespace Nop.Plugin.Api.Product.Modules.Product.Service
{
    public interface IProductApiService
    {
        Core.Domain.Catalog.Product GetProductById(int productId);

        Core.Domain.Catalog.Product GetProductByIdNoTracking(int productId);

        Tuple<IList<Core.Domain.Catalog.Product>, List<ProductsFiltersDto>> GetProducts(IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            int? categoryId = null, string categorySlug = null, string vendorName = null, string manufacturerName = null, string keyword = null, bool? publishedStatus = null);

        int GetProductsCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, bool? publishedStatus = null,
            string vendorName = null, string keyword = null, int? categoryId = null);

        List<Core.Domain.Catalog.Product> GetRelatedProducts(int productId1, bool showHidden = false);

        bool RateProduct(int productId, int customerId,
              int rating, int storeId, string reviewText, string title);
    }
}