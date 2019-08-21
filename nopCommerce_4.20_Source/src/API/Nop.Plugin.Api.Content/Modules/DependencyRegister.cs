using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Api.Content.Modules.Country.Service;
using Nop.Plugin.Api.Content.Modules.Country.Translator;
using Nop.Plugin.Api.Content.Modules.Language.Translator;
using Nop.Plugin.Api.Content.Modules.Manufacturer.Service;
using Nop.Plugin.Api.Content.Modules.Manufacturer.Translator;
using Nop.Plugin.Api.Content.Modules.Store.Translator;
using Nop.Plugin.Api.Content.Modules.Topic.Service;
using Nop.Plugin.Api.Content.Modules.Topic.Translator;

namespace Nop.Plugin.Api.Content.Modules
{
    public class DependencyRegister : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<ManufacturerApiService>().As<IManufacturerApiService>().InstancePerLifetimeScope();
            builder.RegisterType<TopicApiService>().As<ITopicApiService>().InstancePerLifetimeScope();
            builder.RegisterType<StateProvinceApiService>().As<IStateProvinceApiService>().InstancePerLifetimeScope();
            builder.RegisterType<CountryTranslator>().As<ICountryTranslator>().InstancePerLifetimeScope();
            builder.RegisterType<TopicTranslator>().As<ITopicTranslator>().InstancePerLifetimeScope();
            builder.RegisterType<ManufacturerTranslator>().As<IManufacturerTranslator>().InstancePerLifetimeScope();
            builder.RegisterType<LanguageTranslator>().As<ILanguageTranslator>().InstancePerLifetimeScope();
            builder.RegisterType<StoreTranslator>().As<IStoreTranslator>().InstancePerLifetimeScope();
        }

        public virtual int Order => short.MaxValue;
    }
}