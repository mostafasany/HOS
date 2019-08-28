using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.MenuTree.Data;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.MenuTree.Models.Infrastructure
{
    public class PluginDbStartup : INopStartup
    {
        public int Order => 11;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MenuTreeObjectContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServerWithLazyLoading(services);
            });
        }
    }
}
