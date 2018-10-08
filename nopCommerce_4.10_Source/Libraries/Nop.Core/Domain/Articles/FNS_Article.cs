using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.Articles
{
   public class FNS_Article : BaseEntity
    {
        private ICollection<FNS_ArticleGroup> _articleGroups;
        public string Title { get; set; }

        public string Body { get; set; }

        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public int PictureId { get; set; }

        public string Tags { get; set; }

        public bool AllowComments { get; set; }

        public int CommentCount { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime UpdatedOnUtc { get; set; }

        public bool Published { get; set; }

        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the collection of ProductCategory
        /// </summary>
        public virtual ICollection<FNS_ArticleGroup> ArticleGroups
        {
            get => _articleGroups ?? (_articleGroups = new List<FNS_ArticleGroup>());
            protected set => _articleGroups = value;
        }

    }
}
