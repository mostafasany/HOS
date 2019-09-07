using System;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;

namespace Nop.Core.Domain.Custom
{
    public partial class MenuTree : BaseEntity , ISlugSupported , ILocalizedEntity
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public string EntityName { get; set; }
        public string DisplayIn { get; set; }
        public bool Deleted { get; set; }
        public bool Published { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public int DisplayOrder { get; set; }
        public bool ShowOnHomePage { get; set; }
    }

    public partial class MenuTreeItem : BaseEntity
    {
        public int MenuTreeId { get; set; }
        public int ItemId { get; set; }
        public bool Deleted { get; set; }
        public bool Published { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public virtual MenuTree menuTree { get; set; }
    }
}
