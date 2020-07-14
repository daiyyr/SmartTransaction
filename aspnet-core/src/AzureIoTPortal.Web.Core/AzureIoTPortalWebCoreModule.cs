using System;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.SignalR;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.Configuration;
using AzureIoTPortal.Authentication.JwtBearer;
using AzureIoTPortal.Configuration;
using AzureIoTPortal.EntityFrameworkCore;
using System.Reflection;

namespace AzureIoTPortal
{
    [DependsOn(
         typeof(AzureIoTPortalApplicationModule),
         typeof(AzureIoTPortalEntityFrameworkModule),
         typeof(AbpAspNetCoreModule)
        ,typeof(AbpAspNetCoreSignalRModule)
     )]
    public class AzureIoTPortalWebCoreModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public AzureIoTPortalWebCoreModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                AzureIoTPortalConsts.ConnectionStringName
            );

            //SMS DB, using Microsoft.EntityFrameworkCore.DBContext
            //usage:   SMSDbContext dbSMS = new SMSDbContext();  AzureIoT.Bodycorp bodycorp = dbSMS.Bodycorps.Where(x => x.bodycorp_id == bodycorpID).FirstOrDefault();

            //IocManager.AddConventionalRegistrar(new GPEntityFrameworkConventionalRegisterer());
            //services.AddDbContextPool<BlexzWebDb>(options => options.UseSqlServer(Configuration.GetConnectionString("BlexzWebConnection")));



            // Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            Configuration.Modules.AbpAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(AzureIoTPortalApplicationModule).GetAssembly()
                 );

            ConfigureTokenAuth();
        }

        private void ConfigureTokenAuth()
        {
            IocManager.Register<TokenAuthConfiguration>();
            var tokenAuthConfig = IocManager.Resolve<TokenAuthConfiguration>();

            tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appConfiguration["Authentication:JwtBearer:SecurityKey"]));
            tokenAuthConfig.Issuer = _appConfiguration["Authentication:JwtBearer:Issuer"];
            tokenAuthConfig.Audience = _appConfiguration["Authentication:JwtBearer:Audience"];
            tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
            tokenAuthConfig.Expiration = TimeSpan.FromDays(1);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AzureIoTPortalWebCoreModule).GetAssembly());
        }
    }
}
