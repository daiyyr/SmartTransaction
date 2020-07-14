using Microsoft.Extensions.Configuration;
using Castle.MicroKernel.Registration;
using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AzureIoTPortal.Configuration;
using AzureIoTPortal.EntityFrameworkCore;
using AzureIoTPortal.Migrator.DependencyInjection;

namespace AzureIoTPortal.Migrator
{
    [DependsOn(typeof(AzureIoTPortalEntityFrameworkModule))]
    public class AzureIoTPortalMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public AzureIoTPortalMigratorModule(AzureIoTPortalEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(AzureIoTPortalMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                AzureIoTPortalConsts.ConnectionStringName
            );

            //SMS DB, using Microsoft.EntityFrameworkCore.DBContext
            IocManager.AddConventionalRegistrar(new GPEntityFrameworkConventionalRegisterer());

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(
                typeof(IEventBus), 
                () => IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AzureIoTPortalMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
