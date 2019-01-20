using System.Collections.Generic;
using Autofac;
using Microsoft.AspNetCore.Http;
using Nop.Core.Configuration;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Api.Common.Converters;
using Nop.Plugin.Api.Common.Factories;
using Nop.Plugin.Api.Common.Helpers;
using Nop.Plugin.Api.Common.JSON.Serializers;
using Nop.Plugin.Api.Common.Maps;
using Nop.Plugin.Api.Common.ModelBinders;
using Nop.Plugin.Api.Common.Validators;
using Nop.Plugin.Api.Modules.Article.Service;
using Nop.Plugin.Api.Modules.Cart.Factory;
using Nop.Plugin.Api.Modules.Cart.Service;
using Nop.Plugin.Api.Modules.Category.Factory;
using Nop.Plugin.Api.Modules.Category.Service;
using Nop.Plugin.Api.Modules.Client.Service;
using Nop.Plugin.Api.Modules.Country.Service;
using Nop.Plugin.Api.Modules.Customer.Factory;
using Nop.Plugin.Api.Modules.Customer.Service;
using Nop.Plugin.Api.Modules.Discount.Service;
using Nop.Plugin.Api.Modules.Manufacturer.Service;
using Nop.Plugin.Api.Modules.Menu.Service;
using Nop.Plugin.Api.Modules.NewsLetterSubscription.Service;
using Nop.Plugin.Api.Modules.Order.Factory;
using Nop.Plugin.Api.Modules.Order.Service;
using Nop.Plugin.Api.Modules.Product.Factory;
using Nop.Plugin.Api.Modules.Product.Service;
using Nop.Plugin.Api.Modules.ProductAttributes.Service;
using Nop.Plugin.Api.Modules.ProductCategoryMappings.Service;
using Nop.Plugin.Api.Modules.SpecificationAttributes.Service;
using Nop.Plugin.Api.Modules.Topic.Service;

namespace Nop.Plugin.Api.Common.Infrastructure
{
    //using Nop.Plugin.Api.WebHooks;

    public class DependencyRegister : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            RegisterPluginServices(builder);

            RegisterModelBinders(builder);
        }

        public virtual int Order => short.MaxValue;

        private void RegisterModelBinders(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(ParametersModelBinder<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(JsonModelBinder<>)).InstancePerLifetimeScope();
        }

        private void RegisterPluginServices(ContainerBuilder builder)
        {
            builder.RegisterType<ClientService>().As<IClientService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerApiService>().As<ICustomerApiService>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryApiService>().As<ICategoryApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductApiService>().As<IProductApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductCategoryMappingsApiService>().As<IProductCategoryMappingsApiService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderApiService>().As<IOrderApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartItemApiService>().As<IShoppingCartItemApiService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderItemApiService>().As<IOrderItemApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductAttributesApiService>().As<IProductAttributesApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductAttributeConverter>().As<IProductAttributeConverter>().InstancePerLifetimeScope();
            builder.RegisterType<SpecificationAttributesApiService>().As<ISpecificationAttributeApiService>().InstancePerLifetimeScope();
            builder.RegisterType<NewsLetterSubscriptionApiService>().As<INewsLetterSubscriptionApiService>().InstancePerLifetimeScope();
            builder.RegisterType<TopicApiService>().As<ITopicApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ManufacturerApiService>().As<IManufacturerApiService>().InstancePerLifetimeScope();
            builder.RegisterType<MenuApiService>().As<IMenuApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ArticleApiService>().As<IArticleApiService>().InstancePerLifetimeScope();
            builder.RegisterType<StateProvinceApiService>().As<IStateProvinceApiService>().InstancePerLifetimeScope();
            builder.RegisterType<DiscountApiService>().As<IDiscountApiService>().InstancePerLifetimeScope();
            builder.RegisterType<MappingHelper>().As<IMappingHelper>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerRolesHelper>().As<ICustomerRolesHelper>().InstancePerLifetimeScope();
            builder.RegisterType<JsonHelper>().As<IJsonHelper>().InstancePerLifetimeScope();
            builder.RegisterType<DTOHelper>().As<IDTOHelper>().InstancePerLifetimeScope();
            builder.RegisterType<NopConfigManagerHelper>().As<IConfigManagerHelper>().InstancePerLifetimeScope();

            //TODO: Upgrade 4.1. Check this!
            //builder.RegisterType<NopWebHooksLogger>().As<Microsoft.AspNet.WebHooks.Diagnostics.ILogger>().InstancePerLifetimeScope();

            builder.RegisterType<JsonFieldsSerializer>().As<IJsonFieldsSerializer>().InstancePerLifetimeScope();

            builder.RegisterType<FieldsValidator>().As<IFieldsValidator>().InstancePerLifetimeScope();

            //TODO: Upgrade 4.1. Check this!
            //builder.RegisterType<WebHookService>().As<IWebHookService>().SingleInstance();

            builder.RegisterType<ObjectConverter>().As<IObjectConverter>().InstancePerLifetimeScope();
            builder.RegisterType<ApiTypeConverter>().As<IApiTypeConverter>().InstancePerLifetimeScope();

            builder.RegisterType<CategoryFactory>().As<IFactory<Category>>().InstancePerLifetimeScope();
            builder.RegisterType<ProductFactory>().As<IFactory<Product>>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerFactory>().As<IFactory<Customer>>().InstancePerLifetimeScope();
            builder.RegisterType<AddressFactory>().As<IFactory<Address>>().InstancePerLifetimeScope();
            builder.RegisterType<OrderFactory>().As<IFactory<Order>>().InstancePerLifetimeScope();
            builder.RegisterType<ShoppingCartItemFactory>().As<IFactory<ShoppingCartItem>>().InstancePerLifetimeScope();

            builder.RegisterType<JsonPropertyMapper>().As<IJsonPropertyMapper>().InstancePerLifetimeScope();

            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();

            builder.RegisterType<Dictionary<string, object>>().SingleInstance();
        }
    }
}