using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.MenuTree.Models
{
    public partial class AddItemToMenuTreeModel : BaseNopModel
    {
        public AddItemToMenuTreeModel()
        {
            SelectedItemIds = new List<int>();
        }

        public int MenuTreeId { get; set; }

        public IList<int> SelectedItemIds { get; set; }
    }
}
