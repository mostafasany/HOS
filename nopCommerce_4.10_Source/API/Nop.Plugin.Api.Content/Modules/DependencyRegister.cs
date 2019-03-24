using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Api.Content.Modules.Country.Service;
using Nop.Plugin.Api.Content.Modules.Country.Translator;
using Nop.Plugin.Api.Content.Modules.Language.Translator;
using Nop.Plugin.Api.Content.Modules.Manufacturer.Service;
using Nop.Plugin.Api.Content.Modules.Manufacturer.Translator;
using Nop.Plugin.Api.Content.Modules.Topic.Service;
using Nop.Plugin.Api.Content.Modules.Topic.Translator;
using Nop.Plugin.Api.Modules.Store.Translator;

namespace Nop.Plugin.Api.Content.Modules
{
    public class DependencyRegister : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<ManufacturerApiService>().As<IManufacturerApiService>().InstancePerLifetimeScope();
            builder.RegisterType<TopicApiService>().As<ITopicApiService>().InstancePerLifetimeScope();
            builder.RegisterType<StateProvinceApiService>().As<IStateProvinceApiService>().InstancePerLifetimeScope();
            builder.RegisterType<CountryTransaltor>().As<ICountryTransaltor>().InstancePerLifetimeScope();
            builder.RegisterType<TopicTransaltor>().As<ITopicTransaltor>().InstancePerLifetimeScope();
            builder.RegisterType<ManufacturerTransaltor>().As<IManufacturerTransaltor>().InstancePerLifetimeScope();
            builder.RegisterType<LanguageTransaltor>().As<ILanguageTransaltor>().InstancePerLifetimeScope();
            builder.RegisterType<StoreTransaltor>().As<IStoreTransaltor>().InstancePerLifetimeScope();
        }

        public virtual int Order => short.MaxValue;
    }
}