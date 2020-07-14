using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AzureIoTPortal.Configuration;

namespace AzureIoTPortal.Web.Host.Startup
{
    [DependsOn(
       typeof(AzureIoTPortalWebCoreModule))]
    public class AzureIoTPortalWebHostModule: AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public AzureIoTPortalWebHostModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AzureIoTPortalWebHostModule).GetAssembly());
        }
    }
}
