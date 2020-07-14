using System.Threading.Tasks;
using Abp.Application.Services;
using AzureIoTPortal.Sessions.Dto;

namespace AzureIoTPortal.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
