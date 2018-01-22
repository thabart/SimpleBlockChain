using EheathBlockChain.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleIdentityServer.DataAccess.SqlServer;
using SimpleIdentityServer.Host;
using System;

namespace EheathBlockChain
{
    public class Startup
    {
        private readonly IdentityServerOptions _options;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            _options = new IdentityServerOptions
            {
                 Logging = new LoggingOptions
                 {
                     ElasticsearchOptions = new ElasticsearchOptions
                     {
                        IsEnabled = false
                     },
                     FileLogOptions = new FileLogOptions
                     {
                         IsEnabled = false
                     }
                 },
                 DataSource = new DataSourceOptions
                 {
                      OpenIdDataSourceType = DataSourceTypes.InMemory,
                      IsOpenIdDataMigrated = false,
                      EvtStoreDataSourceType = DataSourceTypes.InMemory,
                      IsEvtStoreDataMigrated = false
                 },
                 IsDeveloperModeEnabled = false,
                 Authenticate = new AuthenticateOptions
                 {
                     CookieName =   Constants.CookieName
                 },
            };
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSimpleIdentityServer(_options);
            services.AddMvc();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var simpleIdentityServerContext = serviceScope.ServiceProvider.GetService<SimpleIdentityServerContext>();
                try
                {
                    simpleIdentityServerContext.Database.EnsureCreated();
                }
                catch (Exception) { }
                simpleIdentityServerContext.EnsureSeedData();
            }

            app.UseStaticFiles();
            app.UseSimpleIdentityServer(_options, loggerFactory);
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
