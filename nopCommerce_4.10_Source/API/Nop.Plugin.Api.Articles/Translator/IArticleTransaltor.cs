using Nop.Core.Domain.Articles;
using Nop.Plugin.Api.Articles.Dto;

namespace Nop.Plugin.Api.Articles.Translator
{
    public interface IArticleTransaltor
    {
        ArticlesDto PrepateArticleDto(Core.Domain.Articles.Article article);

        ArticleGroupDto PrepateArticleGroupDto(FNS_ArticleGroup article);
    }
}