using System.Collections.Generic;
using AzureIoTPortal.Roles.Dto;

namespace AzureIoTPortal.Web.Models.Common
{
    public interface IPermissionsEditViewModel
    {
        List<FlatPermissionDto> Permissions { get; set; }
    }
}