using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Api.Common.Helpers;
using Nop.Plugin.Api.Customer.Helpers;

namespace Nop.Plugin.Api.Customer.Modules
{
    public class DependencyRegister : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<CustomerRolesHelper>().As<ICustomerRolesHelper>().InstancePerLifetimeScope();
        }

        public virtual int Order => short.MaxValue;
    }
}