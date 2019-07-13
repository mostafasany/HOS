using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Api.Cart.Service;
using Nop.Plugin.Api.Cart.Translator;

namespace Nop.Plugin.Api.Cart
{
    public class DependencyRegister : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<CartTranslator>().As<ICartTranslator>().InstancePerLifetimeScope();
            builder.RegisterType<DiscountTranslator>().As<IDiscountTranslator>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartItemApiService>().As<IShoppingCartItemApiService>()
                .InstancePerLifetimeScope();
        }

        public virtual int Order => short.MaxValue;
    }
}