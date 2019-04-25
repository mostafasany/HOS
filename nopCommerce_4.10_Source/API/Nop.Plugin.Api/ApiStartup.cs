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
using Nop.Plugin.Api.Common.Authorization.Policies;
using Nop.Plugin.Api.Common.Authorization.Requirements;
using Nop.Plugin.Api.Common.Constants;
using Nop.Plugin.Api.Common.Data;
using Nop.Plugin.Api.Common.Helpers;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace Nop.Plugin.Api
{
    public class ApiStartup : INopStartup
    {
        private const string ObjectContextName = "nop_object_context_web_api";
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        // TODO: extract all methods into extensions.
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

            AddBindingRedirectsFallbacks();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            AddTokenGenerationPipeline(services);

            AddAuthorizationPipeline(services);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<RequestResponseMiddleware>();

            app.UseCors(MyAllowSpecificOrigins);

            // During a clean install we should not register any middlewares i.e IdentityServer as it won't be able to create its  
            // tables without a connection string and will throw an exception
            DataSettings dataSettings = DataSettingsManager.LoadSettings();
            if (!dataSettings?.IsValid ?? true)
                return;
            //https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.1&tabs=visual-studio%2Cvisual-studio-xml
            // The default route templates for the Swagger docs and swagger - ui are "swagger/docs/{apiVersion}" and "swagger/ui/index#/{assetPath}" respectively.
            //app.UseSwagger();
            //app.UseSwaggerUI(options =>
            //    {
            //        //var currentAssembly = Assembly.GetAssembly(this.GetType());
            //        //var currentAssemblyName = currentAssembly.GetName().Name;

            //        //Needeed for removing the "Try It Out" button from the post and put methods.
            //        //http://stackoverflow.com/questions/36772032/swagger-5-2-3-supportedsubmitmethods-removed/36780806#36780806

            //        //options.InjectOnCompleteJavaScript($"{currentAssemblyName}.Scripts.swaggerPostPutTryItOutButtonsRemoval.js");

            //        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            //    }
            //);

            // This needs to be called here because in the plugin install method identity server is not yet registered.
            ApplyIdentityServerMigrations(app);

            SeedData(app);


            RewriteOptions rewriteOptions = new RewriteOptions()
                .AddRewrite("oauth/(.*)", "connect/$1", true)
                .AddRewrite("api/token", "connect/token", true);

            app.UseRewriter(rewriteOptions);

            //app.UseMiddleware<IdentityServerScopeParameterMiddleware>();

            ////uncomment only if the client is an angular application that directly calls the oauth endpoint
            //app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            UseIdentityServer(app);

            //need to enable rewind so we can read the request body multiple times (this should eventually be refactored, but both JsonModelBinder and all of the DTO validators need to read this stream)
            app.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            });
        }

        public int Order => new AuthenticationStartup().Order + 1;

        public void AddBindingRedirectsFallbacks()
        {
            // If no binding redirects are present in the config file then this will perform the binding redirect
            RedirectAssembly("Microsoft.AspNetCore.DataProtection.Abstractions", new Version(2, 0, 0, 0), "adb9793829ddae60");
        }

        /// <summary>
        ///     Adds an AssemblyResolve handler to redirect all attempts to load a specific assembly name to the specified
        ///     version.
        /// </summary>
        public static void RedirectAssembly(string shortName, Version targetVersion, string publicKeyToken)
        {
            ResolveEventHandler handler = null;

            handler = (sender, args) =>
            {
                // Use latest strong name & version when trying to load SDK assemblies
                var requestedAssembly = new AssemblyName(args.Name);
                if (requestedAssembly.Name != shortName)
                    return null;

                requestedAssembly.Version = targetVersion;
                requestedAssembly.SetPublicKeyToken(new AssemblyName("x, PublicKeyToken=" + publicKeyToken).GetPublicKeyToken());
                requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

                AppDomain.CurrentDomain.AssemblyResolve -= handler;

                return Assembly.Load(requestedAssembly);
            };
            AppDomain.CurrentDomain.AssemblyResolve += handler;
        }

        private void AddAuthorizationPipeline(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme,
                    policy =>
                    {
                        policy.Requirements.Add(new ActiveApiPluginRequirement());
                        policy.Requirements.Add(new AuthorizationSchemeRequirement());
                        policy.Requirements.Add(new ActiveClientRequirement());
                        policy.Requirements.Add(new RequestFromSwaggerOptional());
                        policy.RequireAuthenticatedUser();
                    });
            });

            services.AddSingleton<IAuthorizationHandler, ActiveApiPluginAuthorizationPolicy>();
            services.AddSingleton<IAuthorizationHandler, ValidSchemeAuthorizationPolicy>();
            services.AddSingleton<IAuthorizationHandler, ActiveClientAuthorizationPolicy>();
            services.AddSingleton<IAuthorizationHandler, RequestsFromSwaggerAuthorizationPolicy>();
        }

        private void AddRequiredConfiguration()
        {
            var configManagerHelper = new NopConfigManagerHelper();

            // some of third party libaries that we use for WebHooks and Swagger use older versions
            // of certain assemblies so we need to redirect them to the once that nopCommerce uses
            //TODO: Upgrade 4.10 check this!
            //configManagerHelper.AddBindingRedirects();

            // required by the WebHooks support
            //TODO: Upgrade 4.10 check this!
            //configManagerHelper.AddConnectionString();           

            // This is required only in development.
            // It it is required only when you want to send a web hook to an https address with an invalid SSL certificate. (self-signed)
            // The code marks all certificates as valid.
            // We may want to extract this as a setting in the future.

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
                });
            //.AddAuthorizeInteractionResponseGenerator<NopApiAuthorizeInteractionResponseGenerator>()
            //.AddEndpoint<AuthorizeCallbackEndpoint>("Authorize", "/oauth/authorize/callback")
            //.AddEndpoint<AuthorizeEndpoint>("Authorize", "/oauth/authorize")
            //.AddEndpoint<TokenEndpoint>("Token", "/oauth/token");

            identityServerConfig.Services.AddTransient<IResourceOwnerPasswordValidator, PasswordValidator>();
            identityServerConfig.Services.AddTransient<IProfileService, ProfileService>();
            //identityServerConfig.Services.AddAuthentication().AddFacebook("Facebook", options =>
            //{
            //    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            //    options.AppId = "349528745853190";
            //    options.AppSecret = "2883a27c2bb44630641e7c4bb1117147";
            //});
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

        private string LoadUpgradeScript()
        {
            var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();
            string path = fileProvider.MapPath("~/Plugins/Nop.Plugin.Api/upgrade_script.sql");
            string script = File.ReadAllText(path);

            return script;
        }

        private void SeedData(IApplicationBuilder app)
        {
            using (IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var configurationContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if (!configurationContext.ApiResources.Any())
                {
                    // In the simple case an API has exactly one scope. But there are cases where you might want to sub-divide the functionality of an API, and give different clients access to different parts. 
                    configurationContext.ApiResources.Add(new ApiResource
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
                // If there are no api resources we can assume that this is the first start after the upgrade and run the upgrade script.
                string upgradeScript = LoadUpgradeScript();
                configurationContext.Database.ExecuteSqlCommand(upgradeScript);

                // All client secrets must be hashed otherwise the identity server validation will fail.
                List<Client> allClients =
                    configurationContext.Clients.Include(client => client.ClientSecrets).ToList();
                foreach (Client client in allClients)
                {
                    foreach (ClientSecret clientSecret in client.ClientSecrets) clientSecret.Value = clientSecret.Value.Sha256();

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
            // The code below is a copy of app.UseIdentityServer();
            // but the nopCommerce AuthenticationMiddleware is added by nopCommmerce and
            // it has a try catch for the non-configured properly external authentication providers i.e Facebook
            // So there is no need to call UseAuthentication again and thus not being able to catch exceptions thrown by Facebook

            //app.Validate();
            app.UseMiddleware<BaseUrlMiddleware>();
            app.ConfigureCors();
            //app.UseAuthentication();
            app.UseMiddleware<IdentityServerMiddleware>();
        }
    }
}