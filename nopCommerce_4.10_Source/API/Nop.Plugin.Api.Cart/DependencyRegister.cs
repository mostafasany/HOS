﻿using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Api.Cart.Translator;

namespace Nop.Plugin.Api.Cart
{
    public class DependencyRegister : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<Cartransaltor>().As<ICartTransaltor>().InstancePerLifetimeScope();
        }

        public virtual int Order => short.MaxValue;
    }
}