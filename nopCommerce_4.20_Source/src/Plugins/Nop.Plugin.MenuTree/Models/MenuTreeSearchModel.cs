using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.MenuTree.Models
{
    public class MenuTreeSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Search MenuTree Name")]
        public string SearchMenuTreeName { get; set; }
    }
}
