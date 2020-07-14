using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using AzureIoTPortal.Authorization.Roles;
using AzureIoTPortal.Authorization.Users;
using AzureIoTPortal.MultiTenancy;
using AzureIoTPortal.AzureIoT;
using AzureIoTPortal.SystemConfig;
namespace AzureIoTPortal.EntityFrameworkCore
{
    public class AzureIoTPortalDbContext : AbpZeroDbContext<Tenant, Role, User, AzureIoTPortalDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public DbSet<Device> Devices { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<AzureIoTPortal.SystemConfig.SystemConfig> SystemConfigs { get; set; }
        public AzureIoTPortalDbContext(DbContextOptions<AzureIoTPortalDbContext> options)
            : base(options)
        {
        }

    }
}
