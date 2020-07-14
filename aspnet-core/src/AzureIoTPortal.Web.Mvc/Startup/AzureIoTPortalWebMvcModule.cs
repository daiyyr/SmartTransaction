using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AzureIoTPortal.Configuration;
using Abp.Threading.BackgroundWorkers;
using AzureIoTPortal.Web.BackgroundWorker;
using Abp.Localization;

namespace AzureIoTPortal.Web.Startup
{
    [DependsOn(typeof(AzureIoTPortalWebCoreModule))]
    public class AzureIoTPortalWebMvcModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public AzureIoTPortalWebMvcModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.Navigation.Providers.Add<AzureIoTPortalNavigationProvider>();

           
           
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AzureIoTPortalWebMvcModule).GetAssembly());
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            workManager.Add(IocManager.Resolve<BackgroundWork>());
        }
    }
}
