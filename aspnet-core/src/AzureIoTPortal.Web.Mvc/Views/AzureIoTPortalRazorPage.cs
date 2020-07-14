using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;

namespace AzureIoTPortal.Web.Views
{
    public abstract class AzureIoTPortalRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected AzureIoTPortalRazorPage()
        {
            LocalizationSourceName = AzureIoTPortalConsts.LocalizationSourceName;
        }
    }
}
