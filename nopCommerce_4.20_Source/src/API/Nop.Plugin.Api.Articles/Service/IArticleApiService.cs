using System;
using System.Collections.Generic;
using Nop.Core.Domain.Articles;
using Nop.Plugin.Api.Article.Dto;
using Nop.Plugin.Api.Common.Constants;

namespace Nop.Plugin.Api.Article.Service
{
    public interface IArticleApiService
    {
        FNS_Article GetArticleById(int articleId);

        Tuple<IList<FNS_Article>, List<ArticlesFilterDto>> GetArticles(IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null,
            DateTime? updatedAtMax = null,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId,
            int? categoryId = null, int? groupId = null, string keyword = null, string tag = null,
            bool? publishedStatus = null);

        int GetArticlesCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, bool? publishedStatus = null,
            int? categoryId = null, int? groupId = null);

        IList<FNS_ArticleGroup> GetArticlesGroups();

        IList<FNS_Article> GetArticlesSimilarByTag(int articleId,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId);
    }
}