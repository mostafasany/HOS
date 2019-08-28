using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.MenuTree
{
    public partial class RouteProvider : IRouteProvider
    {
        public int Priority => -1;

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("MenuTree.Create", "Plugins/MenuTree/Create",
             new { controller = "MenuTree", action = "Create" });

            routeBuilder.MapRoute("Plugin.MenuTree.List", "Admin/MenuTree/List/",
               new { controller = "MenuTree", action = "List" });

            routeBuilder.MapRoute("Plugin.MenuTree.Create", "Admin/MenuTree/Create/",
               new { controller = "MenuTree", action = "Create" });

            routeBuilder.MapRoute("Plugin.MenuTree.Edit", "Admin/MenuTree/Edit/",
                new { controller = "MenuTree", action = "Edit" });

            routeBuilder.MapRoute("Plugin.MenuTree.Delete", "Admin/MenuTree/Delete/",
                new { controller = "MenuTree", action = "Delete" });

        }
    }
}
