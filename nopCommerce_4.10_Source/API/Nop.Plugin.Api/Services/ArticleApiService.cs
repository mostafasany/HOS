﻿using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Articles;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DataStructures;

namespace Nop.Plugin.Api.Services
{
    public class ArticleApiService : IArticleApiService
    {
        private readonly IRepository<FNS_Article> _articleRepository;
        private readonly IRepository<FNS_ArticleGroup> _articleGroupRepository;
        private readonly IRepository<FNS_ArticleGroup_Mapping> _articleGroupMappingRepository;
        private readonly IRepository<FNS_ArticleCategory> _articleCategoryRepository;

        public ArticleApiService(IRepository<FNS_Article> articleRepository, IRepository<FNS_ArticleGroup> articleGroupRepository,
            IRepository<FNS_ArticleGroup_Mapping> articleGroupMappingRepository, IRepository<FNS_ArticleCategory> articleCategoryRepository)
        {
            _articleRepository = articleRepository;
            _articleGroupRepository = articleGroupRepository;
            _articleGroupMappingRepository = articleGroupMappingRepository;
            _articleCategoryRepository = articleCategoryRepository;
        }

        public IList<FNS_Article> GetArticles(IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            int? categoryId = null, int? groupId = null, string keyword = null, string tag = null, bool? publishedStatus = null)
        {
            IQueryable<FNS_Article> query = GetArticlesQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax,
                publishedStatus, categoryId: categoryId, groupId: groupId,tag:tag,keyword:keyword);

            if (sinceId > 0) query = query.Where(c => c.Id > sinceId);

            return new ApiList<FNS_Article>(query, page - 1, limit);
        }

        public FNS_Article GetArticleById(int id)
        {
            if (id <= 0)
                return null;

            FNS_Article category = _articleRepository.Table.FirstOrDefault(cat => cat.Id == id);

            return category;
        }

        public int GetArticlesCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, bool? publishedStatus = null,
            int? categoryId = null, int? groupId = null)
        {
            IQueryable<FNS_Article> query = GetArticlesQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax,
                publishedStatus, categoryId: categoryId, groupId: groupId);

            return query.Count();
        }

        public IList<FNS_ArticleGroup> GetArticlesGroups()
        {
            IQueryable<FNS_ArticleGroup> query = _articleGroupRepository.Table;
            return query.ToArray();

        }

        private IQueryable<FNS_Article> GetArticlesQuery(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            bool? publishedStatus = null, IList<int> ids = null, int? groupId = null, int? categoryId = null, string keyword = null, string tag = null)

        {
            IQueryable<FNS_Article> query = _articleRepository.Table;

            if (ids != null && ids.Count > 0) query = query.Where(c => ids.Contains(c.Id));

            if (publishedStatus != null) query = query.Where(c => c.Published == publishedStatus.Value);

            // always return products that are not deleted!!!
            query = query.Where(c => !c.Deleted);

            if (createdAtMin != null) query = query.Where(c => c.CreatedOnUtc > createdAtMin.Value);

            if (createdAtMax != null) query = query.Where(c => c.CreatedOnUtc < createdAtMax.Value);

            if (updatedAtMin != null) query = query.Where(c => c.UpdatedOnUtc > updatedAtMin.Value);

            if (updatedAtMax != null) query = query.Where(c => c.UpdatedOnUtc < updatedAtMax.Value);

            if (groupId != null)
            {
                var categoryMappingsForProduct = from productCategoryMapping in _articleGroupMappingRepository.Table
                                                 where productCategoryMapping.GroupId == groupId
                                                 select productCategoryMapping;

                query = from product in query
                        join productCategoryMapping in categoryMappingsForProduct on product.Id equals productCategoryMapping.ArticleId
                        select product;
            }

            if (categoryId != null)
            {
                var categoryMappingsForArticles = from productCategoryMapping in _articleCategoryRepository.Table
                    where productCategoryMapping.CategoryId == categoryId
                                                 select productCategoryMapping;

                query = from article in query
                    join productCategoryMapping in categoryMappingsForArticles on article.Id equals productCategoryMapping.ArticleId
                    select article;
            }

            if (tag != null)
            {
                query = query.Where(c => c.Tags.Contains(tag));
            }

            if (keyword != null)
            {
                keyword = keyword.ToLower();
                query = query.Where(c => c.Title.Contains(keyword));
            }



            query = query.OrderBy(product => product.Id);

            return query;
        }
    }
}