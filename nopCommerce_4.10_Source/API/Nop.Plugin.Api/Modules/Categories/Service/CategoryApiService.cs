using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.DataStructures;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Modules.Categories.Service
{
    public class CategoryApiService : ICategoryApiService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<ProductCategory> _productCategoryMappingRepository;
        private readonly IStoreMappingService _storeMappingService;

        public CategoryApiService(IRepository<Category> categoryRepository,
            IRepository<ProductCategory> productCategoryMappingRepository,
            IStoreMappingService storeMappingService)
        {
            _categoryRepository = categoryRepository;
            _productCategoryMappingRepository = productCategoryMappingRepository;
            _storeMappingService = storeMappingService;
        }

        public IList<Category> GetCategories(IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            int? productId = null, int? parenttId = null,
            bool? publishedStatus = null)
        {
            IQueryable<Category> query = GetCategoriesQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax, publishedStatus, productId, parenttId, ids);


            if (sinceId > 0) query = query.Where(c => c.Id > sinceId);

            return new ApiList<Category>(query, page - 1, limit);
        }

        public Category GetCategoryById(int id)
        {
            if (id <= 0)
                return null;

            Category category = _categoryRepository.Table.FirstOrDefault(cat => cat.Id == id && !cat.Deleted);

            return category;
        }

        public int GetCategoriesCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            bool? publishedStatus = null, int? productId = null)
        {
            IQueryable<Category> query = GetCategoriesQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax,
                publishedStatus, productId);

            return query.Count(c => _storeMappingService.Authorize(c));
        }

        private IQueryable<Category> GetCategoriesQuery(
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            bool? publishedStatus = null, int? productId = null, int? parenttId = null, IList<int> ids = null)
        {
            IQueryable<Category> query = _categoryRepository.Table;

            if (ids != null && ids.Count > 0) query = query.Where(c => ids.Contains(c.Id));

            if (publishedStatus != null) query = query.Where(c => c.Published == publishedStatus.Value);

            query = query.Where(c => !c.Deleted);

            if (createdAtMin != null) query = query.Where(c => c.CreatedOnUtc > createdAtMin.Value);

            if (createdAtMax != null) query = query.Where(c => c.CreatedOnUtc < createdAtMax.Value);

            if (updatedAtMin != null) query = query.Where(c => c.UpdatedOnUtc > updatedAtMin.Value);

            if (updatedAtMax != null) query = query.Where(c => c.UpdatedOnUtc < updatedAtMax.Value);

            if (parenttId != null) query = query.Where(c => c.ParentCategoryId == parenttId);


            if (productId != null)
            {
                IQueryable<ProductCategory> categoryMappingsForProduct = from productCategoryMapping in _productCategoryMappingRepository.Table
                    where productCategoryMapping.ProductId == productId
                    select productCategoryMapping;

                query = from category in query
                    join productCategoryMapping in categoryMappingsForProduct on category.Id equals productCategoryMapping.CategoryId
                    select category;
            }

            query = query.OrderByDescending(category => category.DisplayOrder);

            return query;
        }
    }
}