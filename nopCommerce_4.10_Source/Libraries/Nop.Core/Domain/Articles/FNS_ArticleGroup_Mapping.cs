
namespace Nop.Core.Domain.Articles
{
   public class FNS_ArticleGroup_Mapping : BaseEntity
    {
        public int ArticleId { get; set; }

        public int GroupId { get; set; }

       public virtual FNS_ArticleGroup Group { get; set; }

        public virtual FNS_Article Article { get; set; }
    }
}
