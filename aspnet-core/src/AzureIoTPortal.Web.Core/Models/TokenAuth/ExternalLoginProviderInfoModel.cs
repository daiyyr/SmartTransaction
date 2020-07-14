using Abp.AutoMapper;
using AzureIoTPortal.Authentication.External;

namespace AzureIoTPortal.Models.TokenAuth
{
    [AutoMapFrom(typeof(ExternalLoginProviderInfo))]
    public class ExternalLoginProviderInfoModel
    {
        public string Name { get; set; }

        public string ClientId { get; set; }
    }
}
