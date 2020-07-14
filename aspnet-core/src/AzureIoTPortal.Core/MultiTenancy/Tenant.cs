using Abp.MultiTenancy;
using AzureIoTPortal.Authorization.Users;

namespace AzureIoTPortal.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}
