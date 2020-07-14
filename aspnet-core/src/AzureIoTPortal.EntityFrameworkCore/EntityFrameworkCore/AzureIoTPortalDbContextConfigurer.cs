using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace AzureIoTPortal.EntityFrameworkCore
{
    public static class AzureIoTPortalDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AzureIoTPortalDbContext> builder, string connectionString)
        {
            builder.UseMySql(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<AzureIoTPortalDbContext> builder, DbConnection connection)
        {
            builder.UseMySql(connection);
        }
    }
}
