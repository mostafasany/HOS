using System;

namespace Nop.Core.Domain.Articles
{
   public class FNS_ArticleGroup : BaseEntity
    {
        public string Name { get; set; }

        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public int ParentGroupId { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime UpdatedOnUtc { get; set; }

        public bool Published { get; set; }

        public bool Deleted { get; set; }

      

    }
}
