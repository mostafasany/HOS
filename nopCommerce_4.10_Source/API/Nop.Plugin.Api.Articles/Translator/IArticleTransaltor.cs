using Nop.Core.Domain.Articles;
using Nop.Plugin.Api.Article.Dto;

namespace Nop.Plugin.Api.Article.Translator
{
    public interface IArticleTransaltor
    {
        ArticlesDto PrepateArticleDto(Core.Domain.Articles.Article article);

        ArticleGroupDto PrepateArticleGroupDto(FNS_ArticleGroup article);
    }
}