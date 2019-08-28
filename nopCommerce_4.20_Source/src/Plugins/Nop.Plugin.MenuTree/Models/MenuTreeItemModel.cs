using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.MenuTree.Models
{
    public partial class MenuTreeItemModel : BaseNopEntityModel
    {
        public int MenuTreeId { get; set; }

        public int ItemId { get; set; }

        [NopResourceDisplayName("Item")]
        public string Name { get; set; }

        [NopResourceDisplayName("Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Display Order")]
        public int DisplayOrder { get; set; }

    }
}
