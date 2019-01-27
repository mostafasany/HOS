using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.DataStructures;
using Nop.Plugin.Api.Common.DTOs.Product;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Modules.Product.Service
{
    public class ProductApiService : IProductApiService
    {
        private readonly IRepository<Core.Domain.Catalog.Category> _categoryRepository;
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IRepository<ProductCategory> _productCategoryMappingRepository;
        private readonly IRepository<Core.Domain.Catalog.Product> _productRepository;
        private readonly IRepository<RelatedProduct> _relatedProductRepository;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IRepository<Vendor> _vendorRepository;

        public ProductApiService(IRepository<Core.Domain.Catalog.Product> productRepository,
            IRepository<ProductCategory> productCategoryMappingRepository,
            IRepository<Vendor> vendorRepository,
            IStoreMappingService storeMappingService, IRepository<RelatedProduct> relatedProductRepository
            , IUrlRecordService urlRecordService, IRepository<Manufacturer> manufacturerRepository, IRepository<Core.Domain.Catalog.Category> categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productCategoryMappingRepository = productCategoryMappingRepository;
            _vendorRepository = vendorRepository;
            _storeMappingService = storeMappingService;
            _relatedProductRepository = relatedProductRepository;
            _urlRecordService = urlRecordService;
            _manufacturerRepository = manufacturerRepository;
        }

        public Core.Domain.Catalog.Product GetProductById(int productId)
        {
            if (productId == 0)
                return null;

            return _productRepository.Table.FirstOrDefault(product => product.Id == productId && !product.Deleted);
        }

        public Core.Domain.Catalog.Product GetProductByIdNoTracking(int productId)
        {
            if (productId == 0)
                return null;

            return _productRepository.Table.FirstOrDefault(product => product.Id == productId && !product.Deleted);
        }

        public Tuple<IList<Core.Domain.Catalog.Product>, List<ProductsFiltersDto>> GetProducts(IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            int? categoryId = null, string categorySlug = null, string vendorName = null, string manufacturerName = null, string keyword = null, bool? publishedStatus = null)
        {
            Tuple<IQueryable<Core.Domain.Catalog.Product>, List<ProductsFiltersDto>> tuple = GetProductsQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax, vendorName, manufacturerName, keyword, publishedStatus, ids, categoryId, categorySlug);

            IQueryable<Core.Domain.Catalog.Product> query = tuple.Item1;
            if (sinceId > 0) query = tuple.Item1.Where(c => c.Id > sinceId);

            return new Tuple<IList<Core.Domain.Catalog.Product>, List<ProductsFiltersDto>>(new ApiList<Core.Domain.Catalog.Product>(query, page - 1, limit), tuple.Item2);
        }

        public int GetProductsCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, bool? publishedStatus = null, string vendorName = null, string keyword = null,
            int? categoryId = null)
        {
            Tuple<IQueryable<Core.Domain.Catalog.Product>, List<ProductsFiltersDto>> tuple = GetProductsQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax, vendorName, keyword,
                publishedStatus: publishedStatus, categoryId: categoryId);

            return tuple.Item1.Count(p => _storeMappingService.Authorize(p));
        }


        public List<Core.Domain.Catalog.Product> GetRelatedProducts(int productId1, bool showHidden = false)
        {
            IQueryable<RelatedProduct> query = from rp in _relatedProductRepository.Table
                join p in _productRepository.Table on rp.ProductId2 equals p.Id
                where rp.ProductId1 == productId1 &&
                      !p.Deleted &&
                      (showHidden || p.Published)
                orderby rp.DisplayOrder, rp.Id
                select rp;
            List<RelatedProduct> relatedProducts = query.ToList();

            var products = new List<Core.Domain.Catalog.Product>();
            foreach (RelatedProduct relatedProduct in relatedProducts)
            {
                Core.Domain.Catalog.Product product = GetProductById(relatedProduct.ProductId2);
                products.Add(product);
            }

            return products;
        }

        private Tuple<IQueryable<Core.Domain.Catalog.Product>, List<ProductsFiltersDto>> GetProductsQuery(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, string vendorName = null, string manufacturerName = null, string keyword = null,
            bool? publishedStatus = null, IList<int> ids = null, int? categoryId = null, string categorySlug = null)

        {
            var filters = new List<ProductsFiltersDto>();
            IQueryable<Core.Domain.Catalog.Product> query = _productRepository.Table;

            if (ids != null && ids.Count > 0) query = query.Where(c => ids.Contains(c.Id));

            if (publishedStatus != null) query = query.Where(c => c.Published == publishedStatus.Value);

            // always return products that are not deleted!!!
            query = query.Where(c => !c.Deleted);

            if (createdAtMin != null) query = query.Where(c => c.CreatedOnUtc > createdAtMin.Value);

            if (createdAtMax != null) query = query.Where(c => c.CreatedOnUtc < createdAtMax.Value);

            if (updatedAtMin != null) query = query.Where(c => c.UpdatedOnUtc > updatedAtMin.Value);

            if (updatedAtMax != null) query = query.Where(c => c.UpdatedOnUtc < updatedAtMax.Value);
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(keyword));
                filters.Add(new ProductsFiltersDto("Keyword", keyword));
            }

            if (!string.IsNullOrEmpty(vendorName))
            {
                query = from vendor in _vendorRepository.Table
                    join product in _productRepository.Table on vendor.Id equals product.VendorId
                    where vendor.Name == vendorName && !vendor.Deleted && vendor.Active
                    select product;
                filters.Add(new ProductsFiltersDto("Vendor", vendorName));
            }

            if (!string.IsNullOrEmpty(manufacturerName))
            {
                Manufacturer manufacturerTable = _manufacturerRepository.Table.FirstOrDefault(a => a.Name.ToLower() == manufacturerName.ToLower());
                if (manufacturerTable != null && !manufacturerTable.Deleted)
                {
                    query = from product in _productRepository.Table
                        where product.ProductManufacturers.Select(a => a.ManufacturerId).Contains(manufacturerTable.Id)
                        select product;

                    filters.Add(new ProductsFiltersDto("Manufacturer", manufacturerName));
                }
            }

            if (categoryId == null && categorySlug != null)
            {
                UrlRecord urlRecord = _urlRecordService.GetBySlug(categorySlug);
                categoryId = urlRecord.EntityId;
            }

            if (categoryId != null)
            {
                IQueryable<ProductCategory> categoryMappingsForProduct = from productCategoryMapping in _productCategoryMappingRepository.Table
                    where productCategoryMapping.CategoryId == categoryId
                    select productCategoryMapping;

                query = from product in query
                    join productCategoryMapping in categoryMappingsForProduct on product.Id equals productCategoryMapping.ProductId
                    orderby productCategoryMapping.DisplayOrder descending
                    select product;

                Core.Domain.Catalog.Category category = _categoryRepository.Table.FirstOrDefault(a => a.Id == categoryId);
                filters.Add(new ProductsFiltersDto("Category", category.Name));
            }
            else
            {
                query = query.OrderByDescending(product => product.DisplayOrder);
            }


            return new Tuple<IQueryable<Core.Domain.Catalog.Product>, List<ProductsFiltersDto>>(query, filters);
        }
    }
}