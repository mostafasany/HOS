namespace Nop.Core.Domain.Articles
{
    public class FNS_ArticleCategory : BaseEntity
    {
        public int ArticleId { get; set; }

        public int CategoryId { get; set; }

        public virtual Core.Domain.Catalog.Category Category { get; set; }

        public virtual FNS_Article FnsArticle { get; set; }
    }
}