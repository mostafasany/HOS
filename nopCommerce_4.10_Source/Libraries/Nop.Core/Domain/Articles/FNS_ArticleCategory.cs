
using Nop.Core.Domain.Catalog;

namespace Nop.Core.Domain.Articles
{
   public class FNS_ArticleCategory : BaseEntity
    {
        public int ArticleId { get; set; }

        public int CategoryId { get; set; }

       public virtual Category Category { get; set; }

        public virtual Article Article { get; set; }
    }
}
