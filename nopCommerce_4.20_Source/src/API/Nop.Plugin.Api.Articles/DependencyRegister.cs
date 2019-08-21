using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Api.Article.Service;
using Nop.Plugin.Api.Article.Translator;

namespace Nop.Plugin.Api.Article
{
    public class DependencyRegister : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<ArticleApiService>().As<IArticleApiService>().InstancePerLifetimeScope();
            builder.RegisterType<ArticleTranslator>().As<IArticleTranslator>().InstancePerLifetimeScope();
        }

        public virtual int Order => short.MaxValue;
    }
}