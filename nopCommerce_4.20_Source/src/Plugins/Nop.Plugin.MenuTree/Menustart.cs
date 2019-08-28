using Nop.Services.Plugins;
using System;
using Nop.Web.Framework.Menu;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using System.Linq;
using Nop.Plugin.MenuTree.Data;

namespace Nop.Plugin.MenuTree
{
    public  class Menustart : BasePlugin , IAdminMenuPlugin
    {
       private readonly MenuTreeObjectContext _objectContext;
        public Menustart(MenuTreeObjectContext objectContext)
        {
            _objectContext = objectContext;
        }
        public override void Install()
        {
            _objectContext.Install();


            base.Install();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                SystemName = "MenuTree",
                Title = "MenuTree",
                IconClass = "fa fa-dot-circle-o",
                Visible = true,
                ControllerName = "MenuTree",
                ActionName = "List",
                RouteValues = new RouteValueDictionary() { { "area", AreaNames.Admin } },
            };

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "ParentMenuItemSystemName");
            if (pluginNode != null)
                pluginNode.ChildNodes.Add(menuItem);
            else
                rootNode.ChildNodes.Add(menuItem);
        }

        public override void Uninstall()
        {
            _objectContext.Uninstall();

            base.Uninstall();
        }
    }
}
