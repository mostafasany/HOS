using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.DataStructures;

namespace Nop.Plugin.Api.Modules.ProductAttributes.Service
{
    public class ProductAttributesApiService : IProductAttributesApiService
    {
        private readonly IRepository<ProductAttribute> _productAttributesRepository;

        public ProductAttributesApiService(IRepository<ProductAttribute> productAttributesRepository) => _productAttributesRepository = productAttributesRepository;

        public IList<ProductAttribute> GetProductAttributes(int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId)
        {
            IQueryable<ProductAttribute> query = GetProductAttributesQuery(sinceId);

            return new ApiList<ProductAttribute>(query, page - 1, limit);
        }

        public int GetProductAttributesCount() => GetProductAttributesQuery().Count();

        ProductAttribute IProductAttributesApiService.GetById(int id)
        {
            if (id <= 0)
                return null;

            return _productAttributesRepository.GetById(id);
        }

        private IQueryable<ProductAttribute> GetProductAttributesQuery(int sinceId = Configurations.DefaultSinceId)
        {
            IQueryable<ProductAttribute> query = _productAttributesRepository.Table;

            if (sinceId > 0) query = query.Where(productAttribute => productAttribute.Id > sinceId);

            query = query.OrderByDescending(productAttribute => productAttribute.Id);

            return query;
        }
    }
}