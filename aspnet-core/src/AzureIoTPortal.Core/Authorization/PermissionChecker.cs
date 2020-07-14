using Abp.Authorization;
using AzureIoTPortal.Authorization.Roles;
using AzureIoTPortal.Authorization.Users;

namespace AzureIoTPortal.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
