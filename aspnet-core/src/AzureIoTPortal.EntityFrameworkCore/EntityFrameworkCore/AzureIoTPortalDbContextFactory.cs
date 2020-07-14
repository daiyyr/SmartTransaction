using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using AzureIoTPortal.Configuration;
using AzureIoTPortal.Web;

namespace AzureIoTPortal.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class AzureIoTPortalDbContextFactory : IDesignTimeDbContextFactory<AzureIoTPortalDbContext>
    {
        public AzureIoTPortalDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AzureIoTPortalDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            AzureIoTPortalDbContextConfigurer.Configure(builder, configuration.GetConnectionString(AzureIoTPortalConsts.ConnectionStringName));

            return new AzureIoTPortalDbContext(builder.Options);
        }
    }
}
