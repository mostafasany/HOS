using Nop.Core.Domain.Articles;
using Nop.Plugin.Api.Article.Dto;

namespace Nop.Plugin.Api.Article.Translator
{
    public interface IArticleTranslator
    {
        ArticlesDto ToDto(FNS_Article article);

        ArticleGroupDto ToGroupDto(FNS_ArticleGroup article);
    }
}