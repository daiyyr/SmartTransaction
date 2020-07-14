using Abp.AutoMapper;
using AzureIoTPortal.Sessions.Dto;

namespace AzureIoTPortal.Web.Views.Shared.Components.TenantChange
{
    [AutoMapFrom(typeof(GetCurrentLoginInformationsOutput))]
    public class TenantChangeViewModel
    {
        public TenantLoginInfoDto Tenant { get; set; }
    }
}
