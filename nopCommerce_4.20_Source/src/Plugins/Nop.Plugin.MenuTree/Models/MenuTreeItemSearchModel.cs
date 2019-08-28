using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.MenuTree.Models
{
    public partial class MenuTreeItemSearchModel :BaseSearchModel
    {
        public int menutreeId { get; set; }
    }
}
