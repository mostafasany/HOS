using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Api.Common.Factories;
using Nop.Plugin.Api.Customer.Helpers;
using Nop.Plugin.Api.Customer.Modules.Customer.Factory;
using Nop.Plugin.Api.Customer.Modules.Customer.Service;
using Nop.Plugin.Api.Customer.Modules.NewsLetterSubscription.Service;
using Nop.Plugin.Api.Customer.Modules.Order.Factory;
using Nop.Plugin.Api.Customer.Modules.Order.Service;
using Nop.Plugin.Api.Customer.Modules.Order.Translator;

namespace Nop.Plugin.Api.Customer.Modules
{
    public class DependencyRegister : IDependencyRegistrar
    {
        public virtual int Order => short.MaxValue;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<CustomerRolesHelper>().As<ICustomerRolesHelper>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerApiService>().As<ICustomerApiService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderApiService>().As<IOrderApiService>().InstancePerLifetimeScope();
            builder.RegisterType<NewsLetterSubscriptionApiService>().As<INewsLetterSubscriptionApiService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<OrderItemApiService>().As<IOrderItemApiService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerFactory>().As<IFactory<Core.Domain.Customers.Customer>>()
                .InstancePerLifetimeScope();
            builder.RegisterType<OrderFactory>().As<IFactory<Core.Domain.Orders.Order>>().InstancePerLifetimeScope();
            builder.RegisterType<OrderTransaltor>().As<IOrderTransaltor>().InstancePerLifetimeScope();
        }
    }
}