using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Api.Category.Translator;
using Nop.Plugin.Api.Common.Factories;
using Nop.Plugin.Api.Modules.Category.Factory;
using Nop.Plugin.Api.Modules.Category.Service;
using Nop.Plugin.Api.Modules.Category.Translator;

namespace Nop.Plugin.Api.Article
{
    //using Nop.Plugin.Api.WebHooks;

    public class DependencyRegister : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<CategoryApiService>().As<ICategoryApiService>().InstancePerLifetimeScope();

            builder.RegisterType<CategoryFactory>().As<IFactory<Core.Domain.Catalog.Category>>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryTransaltor>().As<ICategoryTransaltor>().InstancePerLifetimeScope();


        }

        public virtual int Order => short.MaxValue;
    }
}