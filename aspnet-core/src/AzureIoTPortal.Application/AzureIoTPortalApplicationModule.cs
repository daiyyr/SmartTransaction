using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AzureIoTPortal.Authorization;
using Castle.MicroKernel.Registration;
using System.Reflection;

namespace AzureIoTPortal
{
    [DependsOn(
        typeof(AzureIoTPortalCoreModule),
        typeof(AbpAutoMapperModule))]
    public class AzureIoTPortalApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<AzureIoTPortalAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(AzureIoTPortalApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddProfiles(thisAssembly)
            );
        }
    }
}
