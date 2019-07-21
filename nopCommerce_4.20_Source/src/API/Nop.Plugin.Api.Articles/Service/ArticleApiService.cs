using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Articles;
using Nop.Plugin.Api.Article.Dto;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.DataStructures;

namespace Nop.Plugin.Api.Article.Service
{
    public class ArticleApiService : IArticleApiService
    {
        private readonly IRepository<FNS_ArticleCategory> _articleCategoryRepository;
        private readonly IRepository<FNS_ArticleGroup_Mapping> _articleGroupMappingRepository;
        private readonly IRepository<FNS_ArticleGroup> _articleGroupRepository;
        private readonly IRepository<FNS_Article> _articleRepository;

        public ArticleApiService(IRepository<FNS_Article> articleRepository,
            IRepository<FNS_ArticleGroup> articleGroupRepository,
            IRepository<FNS_ArticleGroup_Mapping> articleGroupMappingRepository,
            IRepository<FNS_ArticleCategory> articleCategoryRepository)
        {
            _articleRepository = articleRepository;
            _articleGroupRepository = articleGroupRepository;
            _articleGroupMappingRepository = articleGroupMappingRepository;
            _articleCategoryRepository = articleCategoryRepository;
        }

        public Tuple<IList<FNS_Article>, List<ArticlesFilterDto>> GetArticles(IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null,
            DateTime? updatedAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId,
            int? categoryId = null, int? groupId = null, string keyword = null, string tag = null,
            bool? publishedStatus = null)
        {
            var tuple = GetArticlesQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax,
                publishedStatus, categoryId: categoryId, groupId: groupId, tag: tag, keyword: keyword);


            var query = tuple.Item1;
            if (sinceId > 0) query = tuple.Item1.Where(c => c.Id > sinceId);

            return new Tuple<IList<FNS_Article>, List<ArticlesFilterDto>>(
                new ApiList<FNS_Article>(query, page - 1, limit), tuple.Item2);
        }

        public FNS_Article GetArticleById(int id)
        {
            if (id <= 0)
                return null;

            var category = _articleRepository.Table.FirstOrDefault(cat => cat.Id == id);

            return category;
        }

        public IList<FNS_Article> GetArticlesSimilarByTag(int articleId,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            if (articleId <= 0)
                return null;

            var art = _articleRepository.Table.FirstOrDefault(article => article.Id == articleId);

            var tags = art?.Tags?.Split(new[] {','});
            var query = _articleRepository.Table;
            query = query.Where(
                a => a.Id != art.Id && !a.Deleted && a.Tags != null && tags.Any(v => a.Tags.Contains(v)));
            query = query.OrderByDescending(article => article.UpdatedOnUtc);
            if (sinceId > 0) query = query.Where(c => c.Id > sinceId);

            return new ApiList<FNS_Article>(query, page - 1, limit);
        }

        public int GetArticlesCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, bool? publishedStatus = null,
            int? categoryId = null, int? groupId = null)
        {
            var tuple = GetArticlesQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax,
                publishedStatus, categoryId: categoryId, groupId: groupId);

            return tuple.Item1.Count();
        }

        public IList<FNS_ArticleGroup> GetArticlesGroups()
        {
            var query = _articleGroupRepository.Table;
            return query.ToArray();
        }

        public bool ContainsAll(string str, params string[] values)
        {
            return
                !string.IsNullOrEmpty(str) &&
                values != null &&
                values
                    .Where(x => !string.IsNullOrEmpty(x))
                    .All(str.Contains);
        }

        private Tuple<IQueryable<FNS_Article>, List<ArticlesFilterDto>> GetArticlesQuery(
            DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            bool? publishedStatus = null, IList<int> ids = null, int? groupId = null, int? categoryId = null,
            string keyword = null, string tag = null)

        {
            var filters = new List<ArticlesFilterDto>();
            var query = _articleRepository.Table;

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
                    join productCategoryMapping in categoryMappingsForProduct on product.Id equals
                        productCategoryMapping.ArticleId
                    select product;

                filters.Add(new ArticlesFilterDto("groupId", groupId.ToString()));
            }

            if (categoryId != null && categoryId > 0)
            {
                var categoryMappingsForArticles = from productCategoryMapping in _articleCategoryRepository.Table
                    where productCategoryMapping.CategoryId == categoryId
                    select productCategoryMapping;

                query = from article in query
                    join productCategoryMapping in categoryMappingsForArticles on article.Id equals
                        productCategoryMapping.ArticleId
                    select article;

                filters.Add(new ArticlesFilterDto("categoryId", categoryId.ToString()));
            }

            if (tag != null)
            {
                var tags = tag.Split(new[] {','});
                query = query.Where(a => tags.Any(v => a.Tags != null && a.Tags.Contains(v)));

                filters.Add(new ArticlesFilterDto("tags", tag));
            }

            if (keyword != null)
            {
                keyword = keyword.ToLower();
                query = query.Where(c => c.Title.Contains(keyword));

                filters.Add(new ArticlesFilterDto("keyword", keyword));
            }


            query = query.OrderByDescending(article => article.UpdatedOnUtc);

            return new Tuple<IQueryable<FNS_Article>, List<ArticlesFilterDto>>(query, filters);
        }
    }
}