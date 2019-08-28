using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;

namespace Nop.Plugin.MenuTree.Domain
{
    public partial  class MenuTreeItem : BaseEntity
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
