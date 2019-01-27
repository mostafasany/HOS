using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Api.Common.Factories;
using Nop.Plugin.Api.Customer.Helpers;
using Nop.Plugin.Api.Customer.Modules.Order.Factory;
using Nop.Plugin.Api.Customer.Modules.Order.Service;
using Nop.Plugin.Api.Customer.Modules.Order.Translator;

namespace Nop.Plugin.Api.Customer.Modules
{
    public class DependencyRegister : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<CustomerRolesHelper>().As<ICustomerRolesHelper>().InstancePerLifetimeScope();

            builder.RegisterType<OrderApiService>().As<IOrderApiService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderItemApiService>().As<IOrderItemApiService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderFactory>().As<IFactory<Core.Domain.Orders.Order>>().InstancePerLifetimeScope();
            builder.RegisterType<OrderTransaltor>().As<IOrderTransaltor>().InstancePerLifetimeScope();
        }

        public virtual int Order => short.MaxValue;
    }
}