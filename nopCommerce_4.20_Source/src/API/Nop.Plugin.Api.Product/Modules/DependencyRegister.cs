using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Api.Common.Converters;
using Nop.Plugin.Api.Common.Factories;
using Nop.Plugin.Api.Product.Modules.Discount.Service;
using Nop.Plugin.Api.Product.Modules.Discount.Translator;
using Nop.Plugin.Api.Product.Modules.Product.Factory;
using Nop.Plugin.Api.Product.Modules.Product.Service;
using Nop.Plugin.Api.Product.Modules.Product.Translator;
using Nop.Plugin.Api.Product.Modules.ProductAttributes.Service;
using Nop.Plugin.Api.Product.Modules.ProductAttributes.Translator;
using Nop.Plugin.Api.Product.Modules.ProductCategoryMappings.Service;
using Nop.Plugin.Api.Product.Modules.ProductSpecificationAttributes.Translator;
using Nop.Plugin.Api.Product.Modules.SpecificationAttributes.Service;
using Nop.Plugin.Api.Product.Modules.SpecificationAttributes.Translator;

namespace Nop.Plugin.Api.Product.Modules
{
    public class DependencyRegister : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            RegisterPluginServices(builder);
        }

        public virtual int Order => short.MaxValue;

        private void RegisterPluginServices(ContainerBuilder builder)
        {
            builder.RegisterType<ProductApiService>().As<IProductApiService>().InstancePerLifetimeScope();

            builder.RegisterType<ProductCategoryMappingsApiService>().As<IProductCategoryMappingsApiService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ProductAttributesApiService>().As<IProductAttributesApiService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ProductAttributeConverter>().As<IProductAttributeConverter>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SpecificationAttributesApiService>().As<ISpecificationAttributeApiService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DiscountApiService>().As<IDiscountApiService>().InstancePerLifetimeScope();

            builder.RegisterType<ProductFactory>().As<IFactory<Core.Domain.Catalog.Product>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ProductTransaltor>().As<IProductTransaltor>().InstancePerLifetimeScope();

            builder.RegisterType<ProductSpecificationAttributesTransaltor>()
                .As<IProductSpecificationAttributesTransaltor>().InstancePerLifetimeScope();

            builder.RegisterType<SpecificationAttributesTransaltor>().As<ISpecificationAttributesTransaltor>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ProductAttributesTransaltor>().As<IProductAttributesTransaltor>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DiscountTransaltor>().As<IDiscountTransaltor>().InstancePerLifetimeScope();
        }
    }
}