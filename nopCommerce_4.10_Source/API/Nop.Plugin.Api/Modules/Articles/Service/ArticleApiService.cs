using System;
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
        private readonly IRepository<FNS_ArticleCategory> _articleCategoryRepository;
        private readonly IRepository<FNS_ArticleGroup_Mapping> _articleGroupMappingRepository;
        private readonly IRepository<FNS_ArticleGroup> _articleGroupRepository;
        private readonly IRepository<Article> _articleRepository;

        public ArticleApiService(IRepository<Article> articleRepository, IRepository<FNS_ArticleGroup> articleGroupRepository,
            IRepository<FNS_ArticleGroup_Mapping> articleGroupMappingRepository, IRepository<FNS_ArticleCategory> articleCategoryRepository)
        {
            _articleRepository = articleRepository;
            _articleGroupRepository = articleGroupRepository;
            _articleGroupMappingRepository = articleGroupMappingRepository;
            _articleCategoryRepository = articleCategoryRepository;
        }

        public IList<Article> GetArticles(IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            int? categoryId = null, int? groupId = null, string keyword = null, string tag = null, bool? publishedStatus = null)
        {
            IQueryable<Article> query = GetArticlesQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax,
                publishedStatus, categoryId: categoryId, groupId: groupId, tag: tag, keyword: keyword);

            if (sinceId > 0) query = query.Where(c => c.Id > sinceId);

            return new ApiList<Article>(query, page - 1, limit);
        }

        public Article GetArticleById(int id)
        {
            if (id <= 0)
                return null;

            Article category = _articleRepository.Table.FirstOrDefault(cat => cat.Id == id);

            return category;
        }

        public IList<Article> GetArticlesSimilarByTag(int articleId, int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId)
        {
            if (articleId <= 0)
                return null;

            Article art = _articleRepository.Table.FirstOrDefault(article => article.Id == articleId);
            string[] tags = art?.Tags.Split(new[] {','});
            IQueryable<Article> query = _articleRepository.Table;
            //query= query.Where(a => a.Tags!=null &&  a.Tags.Split(new[] { ',' })
            //    .Intersect(tags)
            //    .Any());
            query = query.Where(a => a.Id != art.Id && tags.Any(v => a.Tags.Contains(v)));
            // always return products that are not deleted!!!
            query = query.Where(c => !c.Deleted);
            query = query.OrderByDescending(article => article.UpdatedOnUtc);
            if (sinceId > 0) query = query.Where(c => c.Id > sinceId);

            return new ApiList<Article>(query, page - 1, limit);
        }

        public int GetArticlesCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, bool? publishedStatus = null,
            int? categoryId = null, int? groupId = null)
        {
            IQueryable<Article> query = GetArticlesQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax,
                publishedStatus, categoryId: categoryId, groupId: groupId);

            return query.Count();
        }

        public IList<FNS_ArticleGroup> GetArticlesGroups()
        {
            IQueryable<FNS_ArticleGroup> query = _articleGroupRepository.Table;
            return query.ToArray();
        }

        public bool ContainsAll(string str, params string[] values)
        {
            return
                !string.IsNullOrEmpty(str) &&
                values != null &&
                values
                    .Where(x => !string.IsNullOrEmpty(x))
                    .All(v => str.Contains(v));
        }

        private IQueryable<Article> GetArticlesQuery(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            bool? publishedStatus = null, IList<int> ids = null, int? groupId = null, int? categoryId = null, string keyword = null, string tag = null)

        {
            IQueryable<Article> query = _articleRepository.Table;

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
                IQueryable<FNS_ArticleGroup_Mapping> categoryMappingsForProduct = from productCategoryMapping in _articleGroupMappingRepository.Table
                    where productCategoryMapping.GroupId == groupId
                    select productCategoryMapping;

                query = from product in query
                    join productCategoryMapping in categoryMappingsForProduct on product.Id equals productCategoryMapping.ArticleId
                    select product;
            }

            if (categoryId != null && categoryId > 0)
            {
                IQueryable<FNS_ArticleCategory> categoryMappingsForArticles = from productCategoryMapping in _articleCategoryRepository.Table
                    where productCategoryMapping.CategoryId == categoryId
                    select productCategoryMapping;

                query = from article in query
                    join productCategoryMapping in categoryMappingsForArticles on article.Id equals productCategoryMapping.ArticleId
                    select article;
            }

            if (tag != null) query = query.Where(c => c.Tags.Contains(tag));

            if (keyword != null)
            {
                keyword = keyword.ToLower();
                query = query.Where(c => c.Title.Contains(keyword));
            }


            query = query.OrderByDescending(article => article.UpdatedOnUtc);

            return query;
        }
    }
}