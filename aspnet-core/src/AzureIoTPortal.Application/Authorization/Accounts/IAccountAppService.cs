using System.Threading.Tasks;
using Abp.Application.Services;
using AzureIoTPortal.Authorization.Accounts.Dto;

namespace AzureIoTPortal.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
