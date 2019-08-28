using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Routing;
using Nop.Plugin.MenuTree.Data;
using Nop.Services.Plugins;
using Nop.Web.Framework;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.MenuTree
{
    public class MenuBasePlugin : BasePlugin, IAdminMenuPlugin
    {

        private readonly MenuTreeObjectContext _objectContext;
        public MenuBasePlugin(MenuTreeObjectContext objectContext)
        {
            _objectContext = objectContext;
        }
        public override void Install()
        {
            _objectContext.Install();


            base.Install();
        }

        public override void Uninstall()
        {
            _objectContext.Uninstall();

            base.Uninstall();
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
    }
}
