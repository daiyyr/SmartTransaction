using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using AzureIoTPortal.Configuration.Dto;

namespace AzureIoTPortal.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : AzureIoTPortalAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
