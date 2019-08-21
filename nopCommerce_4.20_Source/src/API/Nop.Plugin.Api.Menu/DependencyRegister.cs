using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Api.Article.Service;
using Nop.Plugin.Api.Article.Translator;
using Nop.Plugin.Api.Menu.Service;

namespace Nop.Plugin.Api.Menu
{
    public class DependencyRegister : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<MenuApiService>().As<IMenuApiService>().InstancePerLifetimeScope();
        }

        public virtual int Order => short.MaxValue;
    }
}