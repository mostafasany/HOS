using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Api.Admin.Service;

namespace Nop.Plugin.Api.Admin
{
    public class DependencyRegister : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<ClientService>().As<IClientService>().InstancePerLifetimeScope();
        }

        public virtual int Order => short.MaxValue;
    }
}