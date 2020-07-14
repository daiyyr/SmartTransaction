using Abp.AspNetCore.Mvc.ViewComponents;

namespace AzureIoTPortal.Web.Views
{
    public abstract class AzureIoTPortalViewComponent : AbpViewComponent
    {
        protected AzureIoTPortalViewComponent()
        {
            LocalizationSourceName = AzureIoTPortalConsts.LocalizationSourceName;
        }
    }
}
