using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Castle.Facilities.Logging;
using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using AzureIoTPortal.Authentication.JwtBearer;
using AzureIoTPortal.Configuration;
using AzureIoTPortal.Identity;
using AzureIoTPortal.Web.Resources;
using Abp.AspNetCore.SignalR.Hubs;
using AzureIoTPortal.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AzureIoTPortal.Web.Startup
{
    public class Startup
    {
        private readonly IConfigurationRoot _appConfiguration;

        public Startup(IHostingEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // MVC
            services.AddMvc(
                options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute())
            );

            //SMS DB, using Microsoft.EntityFrameworkCore.DBContext
            //services.AddEntityFrameworkMySql();
            //services.AddDbContextPool<SMSDbContext>((serviceProvider, optionsBuilder) =>
            //{
            //    optionsBuilder.UseMySql(_appConfiguration.GetConnectionString("SMS"));
            //    optionsBuilder.UseInternalServiceProvider(serviceProvider);
            //});

            //services.AddDbContextPool<SMSDbContext>(options => options.UseMySql(_appConfiguration.GetConnectionString("SMS")));

            IdentityRegistrar.Register(services);


            AuthConfigurer.Configure(services, _appConfiguration);

            services.AddScoped<IWebResourceManager, WebResourceManager>();

            services.AddSignalR();

            // Configure Abp and Dependency Injection
            return services.AddAbp<AzureIoTPortalWebMvcModule>(
                // Configure Log4Net logging
                options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                )
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAbp(); // Initializes ABP framework.

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseJwtTokenMiddleware();

            app.UseSignalR(routes =>
            {
                routes.MapHub<AbpCommonHub>("/signalr");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "defaultWithArea",
                    template: "{area}/{controller=IoT}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=IoT}/{action=Index}/{id?}");
            });
        }
    }
}
