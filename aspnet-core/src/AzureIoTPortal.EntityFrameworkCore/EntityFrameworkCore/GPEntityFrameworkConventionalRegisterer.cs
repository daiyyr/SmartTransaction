using Abp.Dependency;
using Abp.Reflection.Extensions;
using AzureIoTPortal.Configuration;
using AzureIoTPortal.EntityFrameworkCore;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureIoTPortal
{
    public class GPEntityFrameworkConventionalRegisterer : IConventionalDependencyRegistrar
    {
        private IConfigurationRoot _appConfiguration;
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            context.IocManager.IocContainer.Register(
                Classes.FromAssembly(context.Assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<SMSDbContext>()
                    .WithServiceSelf()
                    .LifestyleTransient()
                    .Configure(c => c.DynamicParameters(
                        (kernel, dynamicParams) =>
                        {
                            var connectionString = GetNameOrConnectionStringOrNull(context.IocManager);
                            if (!string.IsNullOrWhiteSpace(connectionString))
                            {
                                dynamicParams["connectionString"] = connectionString;
                            }
                        })));
        }

        private string GetNameOrConnectionStringOrNull(IIocResolver iocResolver)
        {
            _appConfiguration = AppConfigurations.Get(
                   typeof(GPEntityFrameworkConventionalRegisterer).GetAssembly().GetDirectoryPathOrNull()
               );

            return
            _appConfiguration.GetConnectionString(
                AzureIoTPortalConsts.ConnectionStringNameSMS
            );
        }
    }
}
