using System.Threading.Tasks;
using AzureIoTPortal.Configuration.Dto;

namespace AzureIoTPortal.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
