using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Api.Cart.Factory;
using Nop.Plugin.Api.Cart.Service;
using Nop.Plugin.Api.Cart.Translator;
using Nop.Plugin.Api.Common.Factories;

namespace Nop.Plugin.Api.Cart
{
    public class DependencyRegister : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<Cartransaltor>().As<ICartTransaltor>().InstancePerLifetimeScope();
            builder.RegisterType<DiscountTransaltor>().As<IDiscountTransaltor>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartItemApiService>().As<IShoppingCartItemApiService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartItemFactory>().As<IFactory<ShoppingCartItem>>().InstancePerLifetimeScope();
        }

        public virtual int Order => short.MaxValue;
    }
}