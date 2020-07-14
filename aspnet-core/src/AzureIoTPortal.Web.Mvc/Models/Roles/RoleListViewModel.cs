using System.Collections.Generic;
using AzureIoTPortal.Roles.Dto;

namespace AzureIoTPortal.Web.Models.Roles
{
    public class RoleListViewModel
    {
        public IReadOnlyList<RoleListDto> Roles { get; set; }

        public IReadOnlyList<PermissionDto> Permissions { get; set; }
    }
}
