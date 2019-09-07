using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.DataStructures;
using Nop.Services.Catalog;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Category.Service
{
    public class CategoryApiService : ICategoryApiService
    {
        private readonly IRepository<Core.Domain.Catalog.Category> _categoryRepository;
        private readonly IRepository<ProductCategory> _productCategoryMappingRepository;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICategoryService _categoryService;

        public CategoryApiService(IRepository<Core.Domain.Catalog.Category> categoryRepository,
            IRepository<ProductCategory> productCategoryMappingRepository, ICategoryService categoryService,
            IStoreMappingService storeMappingService)
        {
            _categoryRepository = categoryRepository;
            _productCategoryMappingRepository = productCategoryMappingRepository;
            _storeMappingService = storeMappingService;
            _categoryService = categoryService;
        }

        public IEnumerable<Core.Domain.Catalog.Category> GetCategories(IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null,
            DateTime? updatedAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId,
            int? productId = null, int? parentId = null,
            bool? publishedStatus = null)
        {
            var query = GetCategoriesQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax, publishedStatus,
                productId, parentId, ids);


            if (sinceId > 0) query = query.Where(c => c.Id > sinceId);

            return new ApiList<Core.Domain.Catalog.Category>(query, page - 1, limit);
        }


        public IEnumerable<Core.Domain.Catalog.Category> GetNewCategories(IList<int> ids = null,
          DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null,
          DateTime? updatedAtMax = null,
          int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
          int sinceId = Configurations.DefaultSinceId,
          int? productId = null, int? parentId = null,
          bool? publishedStatus = null)
        {
            var query = GetNewCategoriesQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax, publishedStatus,
                productId, parentId, ids);


            if (sinceId > 0)
                query = query.Where(c => c.Id > sinceId);

            return new ApiList<Core.Domain.Catalog.Category>(query, page - 1, limit);
        }

        public Core.Domain.Catalog.Category GetCategoryById(int id)
        {
            if (id <= 0)
                return null;

            var category = _categoryRepository.Table.FirstOrDefault(cat => cat.Id == id && !cat.Deleted);

            return category;
        }


        public Core.Domain.Catalog.Category GetNewCategoryById(int id)
        {
            if (id <= 0)
                return null;

            var category = _categoryService.GetCategoryById(id);
            if(category!=null && !category.Deleted && category.Published)
            {
                return category;
            }
            return null;
        }


        public int GetCategoriesCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            bool? publishedStatus = null, int? productId = null)
        {
            var query = GetCategoriesQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax,
                publishedStatus, productId);

            return query.Count(c => _storeMappingService.Authorize(c));
        }


        private IQueryable<Core.Domain.Catalog.Category> GetCategoriesQuery(
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null,
            DateTime? updatedAtMax = null,
            bool? publishedStatus = null, int? productId = null, int? parentId = null, ICollection<int> ids = null)
        {
            var query = _categoryRepository.Table;

            if (ids != null && ids.Count > 0) query = query.Where(c => ids.Contains(c.Id));

            if (publishedStatus != null) query = query.Where(c => c.Published == publishedStatus.Value);

            query = query.Where(c => !c.Deleted);

            if (createdAtMin != null) query = query.Where(c => c.CreatedOnUtc > createdAtMin.Value);

            if (createdAtMax != null) query = query.Where(c => c.CreatedOnUtc < createdAtMax.Value);

            if (updatedAtMin != null) query = query.Where(c => c.UpdatedOnUtc > updatedAtMin.Value);

            if (updatedAtMax != null) query = query.Where(c => c.UpdatedOnUtc < updatedAtMax.Value);

            if (parentId != null) query = query.Where(c => c.ParentCategoryId == parentId);


            if (productId != null)
            {
                var categoryMappingsForProduct = from productCategoryMapping in _productCategoryMappingRepository.Table
                    where productCategoryMapping.ProductId == productId
                    select productCategoryMapping;

                query = from category in query
                    join productCategoryMapping in categoryMappingsForProduct on category.Id equals
                        productCategoryMapping.CategoryId
                    select category;
            }

            query = query.OrderByDescending(category => category.DisplayOrder);

            return query;
        }

        private IQueryable<Core.Domain.Catalog.Category> GetNewCategoriesQuery(
           DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null,
           DateTime? updatedAtMax = null,
           bool? publishedStatus = null, int? productId = null, int? parentId = null, ICollection<int> ids = null)
        {
            IQueryable<Core.Domain.Catalog.Category> query;
            if (ids != null && ids.Count > 0)
                query = _categoryService.GetCategoriesByIds(ids.ToArray<int>()).AsQueryable<Core.Domain.Catalog.Category>();

            else if (parentId.HasValue)
                query = _categoryService.GetAllCategoriesByParentCategoryId(parentId.Value).AsQueryable<Core.Domain.Catalog.Category>();


            else if (productId.HasValue)
            {
                query = _categoryService.GetAllCategories().AsQueryable<Core.Domain.Catalog.Category>();
                var categoryMappingsForProduct = _categoryService.GetProductCategoriesByProductId(productId.Value).AsQueryable<Core.Domain.Catalog.ProductCategory>();
      

                query = from category in query
                        join productCategoryMapping in categoryMappingsForProduct on category.Id equals
                            productCategoryMapping.CategoryId
                        select category;
            }
            else
            {
                 query = _categoryService.GetAllCategories().AsQueryable<Core.Domain.Catalog.Category>();
            }

                if (publishedStatus != null)
                query = query.Where(c => c.Published == publishedStatus.Value);

            query = query.Where(c => !c.Deleted);

            if (createdAtMin != null)
                query = query.Where(c => c.CreatedOnUtc > createdAtMin.Value);

            if (createdAtMax != null)
                query = query.Where(c => c.CreatedOnUtc < createdAtMax.Value);

            if (updatedAtMin != null)
                query = query.Where(c => c.UpdatedOnUtc > updatedAtMin.Value);

            if (updatedAtMax != null)
                query = query.Where(c => c.UpdatedOnUtc < updatedAtMax.Value);

            if (parentId != null)
                query = query.Where(c => c.ParentCategoryId == parentId);

            query = query.OrderByDescending(category => category.DisplayOrder);

            return query;
        }
    }
}