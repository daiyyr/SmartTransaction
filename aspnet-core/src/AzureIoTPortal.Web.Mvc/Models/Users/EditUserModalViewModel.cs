using System.Collections.Generic;
using System.Linq;
using AzureIoTPortal.Roles.Dto;
using AzureIoTPortal.Users.Dto;

namespace AzureIoTPortal.Web.Models.Users
{
    public class EditUserModalViewModel
    {
        public UserDto User { get; set; }

        public IReadOnlyList<RoleDto> Roles { get; set; }

        public bool UserIsInRole(RoleDto role)
        {
            return User.RoleNames != null && User.RoleNames.Any(r => r == role.NormalizedName);
        }
    }
}
