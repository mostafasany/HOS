using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
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
using Microsoft.IdentityModel.Tokens;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Authorization.Policies;
using Nop.Plugin.Api.Authorization.Requirements;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.Data;
using Nop.Plugin.Api.Common.Helpers;
using Nop.Plugin.Api.IdentityServer;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Api.Infrastructure
{
    public class ApiStartup : INopStartup
    {
        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
                options.AddPolicy(MyAllowSpecificOrigins, builder =>
                {
                    builder.WithOrigins("http://localhost:4200", "https://hosweb-dev.azurewebsites.net",
                            "https://hosweb.azurewebsites.net")
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .WithExposedHeaders(".Nop.Customer")
                        .AllowAnyOrigin();
                }));

            services.AddDbContext<ApiObjectContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServerWithLazyLoading(services);
            });


            AddRequiredConfiguration();

           // AddBindingRedirectsFallbacks();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            AddTokenGenerationPipeline(services);

            AddAuthorizationPipeline(services);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCors(MyAllowSpecificOrigins);

            // During a clean install we should not register any middlewares i.e IdentityServer as it won't be able to create its  
            // tables without a connection string and will throw an exception
            DataSettings dataSettings = DataSettingsManager.LoadSettings();
            if (!dataSettings?.IsValid ?? true)
                return;

            // This needs to be called here because in the plugin install method identity server is not yet registered.
            ApplyIdentityServerMigrations(app);

            SeedData(app);


            RewriteOptions rewriteOptions = new RewriteOptions()
                .AddRewrite("oauth/(.*)", "connect/$1", true)
                .AddRewrite("api/token", "connect/token", true);

            app.UseRewriter(rewriteOptions);


       
            UseIdentityServer(app);

            //need to enable rewind so we can read the request body multiple times (this should eventually be refactored, but both JsonModelBinder and all of the DTO validators need to read this stream)
            app.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            });
        }

        public int Order => new AuthenticationStartup().Order + 1;

        //public void AddBindingRedirectsFallbacks()
        //{
        //    // If no binding redirects are present in the config file then this will perform the binding redirect
        //    RedirectAssembly("Microsoft.AspNetCore.DataProtection.Abstractions", new Version(2, 0, 0, 0), "adb9793829ddae60");
        //}

        ///// <summary>
        /////     Adds an AssemblyResolve handler to redirect all attempts to load a specific assembly name to the specified
        /////     version.
        ///// </summary>
        //public static void RedirectAssembly(string shortName, Version targetVersion, string publicKeyToken)
        //{
        //    ResolveEventHandler handler = null;

        //    handler = (sender, args) =>
        //    {
        //        // Use latest strong name & version when trying to load SDK assemblies
        //        var requestedAssembly = new AssemblyName(args.Name);
        //        if (requestedAssembly.Name != shortName)
        //            return null;

        //        requestedAssembly.Version = targetVersion;
        //        requestedAssembly.SetPublicKeyToken(new AssemblyName("x, PublicKeyToken=" + publicKeyToken).GetPublicKeyToken());
        //        requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

        //        AppDomain.CurrentDomain.AssemblyResolve -= handler;

        //        return Assembly.Load(requestedAssembly);
        //    };
        //    AppDomain.CurrentDomain.AssemblyResolve += handler;
        //}

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
            RsaSecurityKey signingKey = CryptoHelper.CreateRsaSecurityKey();

            DataSettings dataSettings = DataSettingsManager.LoadSettings();
            if (!dataSettings?.IsValid ?? true)
                return;

            string connectionStringFromNop = dataSettings.DataConnectionString;

            string migrationsAssembly = typeof(ApiStartup).GetTypeInfo().Assembly.GetName().Name;

            IIdentityServerBuilder identityServerConfig = services.AddIdentityServer()
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
            using (IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
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
            using (IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var configurationContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if (!configurationContext.ApiResources.Any())
                {
                    // In the simple case an API has exactly one scope. But there are cases where you might want to sub-divide the functionality of an API, and give different clients access to different parts. 
                    configurationContext.ApiResources.Add(new IdentityServer4.EntityFramework.Entities.ApiResource
                    {
                        Enabled = true,
                        Scopes = new List<ApiScope>
                        {
                            new ApiScope
                            {
                                Name = "nop_api",
                                DisplayName = "nop_api"
                            }
                        },
                        Created=DateTime.Now,
                        Updated=DateTime.Now,
                        LastAccessed=DateTime.Now,
                
                        Name = "nop_api"
                    });

                    configurationContext.SaveChanges();

                    TryRunUpgradeScript(configurationContext);
                }
            }
        }

        private void TryRunUpgradeScript(ConfigurationDbContext configurationContext)
        {
            try
            {
                // All client secrets must be hashed otherwise the identity server validation will fail.
                List<IdentityServer4.EntityFramework.Entities.Client> allClients =
                    configurationContext.Clients.Include(client => client.ClientSecrets).ToList();
                foreach (IdentityServer4.EntityFramework.Entities.Client client in allClients)
                {
                    foreach (ClientSecret clientSecret in client.ClientSecrets)
                        clientSecret.Value = clientSecret.Value.Sha256();

                    client.AccessTokenLifetime = Configurations.DefaultAccessTokenExpiration;
                    client.AbsoluteRefreshTokenLifetime = Configurations.DefaultRefreshTokenExpiration;
                }

                configurationContext.SaveChanges();
            }
            catch (Exception ex)
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