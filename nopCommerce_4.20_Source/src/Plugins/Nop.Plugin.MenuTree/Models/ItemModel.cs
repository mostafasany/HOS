using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.MenuTree.Models
{
    public class ItemModel : BaseNopModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public bool Published { get; set; }
    }
}
