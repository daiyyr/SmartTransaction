using System.Collections.Generic;
using AzureIoTPortal.Roles.Dto;
using AzureIoTPortal.Users.Dto;

namespace AzureIoTPortal.Web.Models.Users
{
    public class UserListViewModel
    {
        public IReadOnlyList<UserDto> Users { get; set; }

        public IReadOnlyList<RoleDto> Roles { get; set; }
    }
}
