﻿using System.Collections.Generic;
using Autofac;
using Microsoft.AspNetCore.Http;
using Nop.Core.Configuration;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Api.Common.Converters;
using Nop.Plugin.Api.Common.Factories;
using Nop.Plugin.Api.Common.Helpers;
using Nop.Plugin.Api.Common.JSON.Serializers;
using Nop.Plugin.Api.Common.Maps;
using Nop.Plugin.Api.Common.ModelBinders;
using Nop.Plugin.Api.Common.Validators;
using Nop.Plugin.Api.Modules.Menu.Service;

namespace Nop.Plugin.Api.Common
{
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
            builder.RegisterType<MenuApiService>().As<IMenuApiService>().InstancePerLifetimeScope();

            builder.RegisterType<MappingHelper>().As<IMappingHelper>().InstancePerLifetimeScope();

            builder.RegisterType<JsonHelper>().As<IJsonHelper>().InstancePerLifetimeScope();

            builder.RegisterType<NopConfigManagerHelper>().As<IConfigManagerHelper>().InstancePerLifetimeScope();

            //TODO: Upgrade 4.1. Check this!
            //builder.RegisterType<NopWebHooksLogger>().As<Microsoft.AspNet.WebHooks.Diagnostics.ILogger>().InstancePerLifetimeScope();

            builder.RegisterType<JsonFieldsSerializer>().As<IJsonFieldsSerializer>().InstancePerLifetimeScope();

            builder.RegisterType<FieldsValidator>().As<IFieldsValidator>().InstancePerLifetimeScope();

            //TODO: Upgrade 4.1. Check this!
            //builder.RegisterType<WebHookService>().As<IWebHookService>().SingleInstance();

            builder.RegisterType<ObjectConverter>().As<IObjectConverter>().InstancePerLifetimeScope();

            builder.RegisterType<ApiTypeConverter>().As<IApiTypeConverter>().InstancePerLifetimeScope();

            builder.RegisterType<AddressFactory>().As<IFactory<Address>>().InstancePerLifetimeScope();

            builder.RegisterType<JsonPropertyMapper>().As<IJsonPropertyMapper>().InstancePerLifetimeScope();

            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();

            builder.RegisterType<Dictionary<string, object>>().SingleInstance();
        }
    }
}