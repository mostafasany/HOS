using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;

namespace Nop.Plugin.MenuTree.Domain
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
}
