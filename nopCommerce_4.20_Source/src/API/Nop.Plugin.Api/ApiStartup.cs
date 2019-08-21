using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Hosting;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.Data;
using Nop.Plugin.Api.Common.Helpers;
using Nop.Plugin.Api.IdentityServer;
using Nop.Plugin.Api.IdentityServer.Authorization.Policies;
using Nop.Plugin.Api.IdentityServer.Authorization.Requirements;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;

namespace Nop.Plugin.Api
{
    public class ApiStartup : INopStartup
    {
        private const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
                options.AddPolicy(MyAllowSpecificOrigins, builder =>
                {
                    builder.WithOrigins("http://localhost:4200", "http://167.71.59.237",
                            "http://167.71.59.237:4200")
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .WithExposedHeaders(".Nop.Customer");
                }));

            services.AddDbContext<ApiObjectContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServerWithLazyLoading(services);
            });


            AddRequiredConfiguration();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            AddTokenGenerationPipeline(services);

            AddAuthorizationPipeline(services);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCors(MyAllowSpecificOrigins);

            // During a clean install we should not register any middleware i.e IdentityServer as it won't be able to create its  
            // tables without a connection string and will throw an exception
            var dataSettings = DataSettingsManager.LoadSettings();
            if (!dataSettings?.IsValid ?? true)
                return;

            // This needs to be called here because in the plugin install method identity server is not yet registered.
            ApplyIdentityServerMigrations(app);

            SeedData(app);


            var rewriteOptions = new RewriteOptions()
                .AddRewrite("oauth/(.*)", "connect/$1", true)
                .AddRewrite("api/token", "connect/token", true);

            app.UseRewriter(rewriteOptions);

            app.UseMiddleware<RequestResponseMiddleware>();

            UseIdentityServer(app);

            //need to enable rewind so we can read the request body multiple times (this should eventually be refactored, but both JsonModelBinder and all of the DTO validators need to read this stream)
            app.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            });
        }

        public int Order => new AuthenticationStartup().Order + 1;


        private void AddAuthorizationPipeline(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme,
                    policy =>
                    {
                        policy.Requirements.Add(new ActiveApiPluginRequirement());
                        policy.Requirements.Add(new AuthorizationSchemeRequirement());
                        //policy.Requirements.Add(new ActiveClientRequirement());
                        //policy.Requirements.Add(new RequestFromSwaggerOptional());
                        policy.RequireAuthenticatedUser();
                    });
            });

            services.AddSingleton<IAuthorizationHandler, ActiveApiPluginAuthorizationPolicy>();
            services.AddSingleton<IAuthorizationHandler, ValidSchemeAuthorizationPolicy>();
            //services.AddSingleton<IAuthorizationHandler, ActiveClientAuthorizationPolicy>();
            //services.AddSingleton<IAuthorizationHandler, RequestsFromSwaggerAuthorizationPolicy>();
        }

        private void AddRequiredConfiguration()
        {
            // NOTE: If this code is commented the certificates will be validated.
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        private void AddTokenGenerationPipeline(IServiceCollection services)
        {
            var signingKey = CryptoHelper.CreateRsaSecurityKey();

            var dataSettings = DataSettingsManager.LoadSettings();
            if (!dataSettings?.IsValid ?? true)
                return;

            var connectionStringFromNop = dataSettings.DataConnectionString;

            var migrationsAssembly = typeof(ApiStartup).GetTypeInfo().Assembly.GetName().Name;

            var identityServerConfig = services.AddIdentityServer()
                    .AddSigningCredential(signingKey)
                    .AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = builder =>
                            builder.UseSqlServer(connectionStringFromNop,
                                sql => sql.MigrationsAssembly(migrationsAssembly));
                    })
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = builder =>
                            builder.UseSqlServer(connectionStringFromNop,
                                sql => sql.MigrationsAssembly(migrationsAssembly));
                    })
                ;

            identityServerConfig.Services.AddTransient<IResourceOwnerPasswordValidator, PasswordValidator>();
            identityServerConfig.Services.AddTransient<IProfileService, ProfileService>();
            identityServerConfig.AddExtensionGrantValidator<DelegationGrantValidator>();
        }

        private void ApplyIdentityServerMigrations(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                // the database.Migrate command will apply all pending migrations and will create the database if it is not created already.
                var persistedGrantContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                persistedGrantContext.Database.Migrate();

                var configurationContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configurationContext.Database.Migrate();
            }
        }

        private void SeedData(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var configurationContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if (configurationContext.ApiResources.Any()) return;

                // In the simple case an API has exactly one scope. But there are cases where you might want to sub-divide the functionality of an API, and give different clients access to different parts. 
                configurationContext.ApiResources.Add(new ApiResource
                {
                    Enabled = true,
                    Scopes = new List<ApiScope> {new ApiScope {Name = "nop_api", DisplayName = "nop_api"}},
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    LastAccessed = DateTime.Now,
                    Name = "nop_api"
                });

                configurationContext.SaveChanges();

                TryRunUpgradeScript(configurationContext);
            }
        }

        private void TryRunUpgradeScript(ConfigurationDbContext configurationContext)
        {
            try
            {
                // All client secrets must be hashed otherwise the identity server validation will fail.
                var allClients =
                    configurationContext.Clients.Include(client => client.ClientSecrets).ToList();
                foreach (var client in allClients)
                {
                    foreach (var clientSecret in client.ClientSecrets)
                        clientSecret.Value = clientSecret.Value.Sha256();

                    client.AccessTokenLifetime = Configurations.DefaultAccessTokenExpiration;
                    client.AbsoluteRefreshTokenLifetime = Configurations.DefaultRefreshTokenExpiration;
                }

                configurationContext.SaveChanges();
            }
            catch (Exception)
            {
                // Probably the upgrade script was already executed and we don't need to do anything.
            }
        }

        private void UseIdentityServer(IApplicationBuilder app)
        {
            app.UseMiddleware<BaseUrlMiddleware>();
            app.ConfigureCors();
            app.UseMiddleware<IdentityServerMiddleware>();
        }
    }
}