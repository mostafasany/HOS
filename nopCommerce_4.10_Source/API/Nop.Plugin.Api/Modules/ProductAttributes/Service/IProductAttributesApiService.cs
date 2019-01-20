using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.Constants;

namespace Nop.Plugin.Api.Modules.ProductsAttributes.Service
{
    public interface IProductAttributesApiService
    {
        ProductAttribute GetById(int id);

        IList<ProductAttribute> GetProductAttributes(int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId);

        int GetProductAttributesCount();
    }
}