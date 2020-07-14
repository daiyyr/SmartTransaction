using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AzureIoTPortal.Authorization.Users;
using AzureIoTPortal.Roles.Dto;
using AzureIoTPortal.Users.Dto;

namespace AzureIoTPortal.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);

        Task<UserDto> GetCurrentUserSMSInfo(string connIOT, string connSMS);

    }
}
