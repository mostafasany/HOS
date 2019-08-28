using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.MenuTree.Data;
using Nop.Plugin.MenuTree.Services;
using Nop.Web.Areas.Admin.Validators;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.MenuTree.Models.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_MenuTree_view_tracker";
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
      
            builder.RegisterType<MenuTreeService>().As<IMenuTreeService>().InstancePerLifetimeScope();

            builder.RegisterType<MenuTreeValidator>().AsSelf().InstancePerLifetimeScope();

           
            builder.RegisterPluginDataContext<MenuTreeObjectContext>("nop_object_context_MenuTree_view_tracker");

           builder.RegisterType<EfRepository<Domain.MenuTree>>().As<IRepository<Domain.MenuTree>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_MenuTree_view_tracker"))
               .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Domain.MenuTreeItem>>().As<IRepository<Domain.MenuTreeItem>>()
                      .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_MenuTree_view_tracker"))
                      .InstancePerLifetimeScope();

        }

        public int Order
        {
            get { return 1; }
        }

    }
}
