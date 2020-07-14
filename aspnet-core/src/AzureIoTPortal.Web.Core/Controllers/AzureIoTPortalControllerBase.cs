using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Abp.Reflection.Extensions;
using Abp.Runtime.Session;
using AzureIoTPortal.Configuration;
using AzureIoTPortal.Data;
using AzureIoTPortal.Users;
using AzureIoTPortal.Users.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AzureIoTPortal.Controllers
{
    public abstract class AzureIoTPortalControllerBase: AbpController
    {
        protected AzureIoTPortalControllerBase()
        {
            LocalizationSourceName = AzureIoTPortalConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
