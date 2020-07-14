using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AzureIoTPortal.MultiTenancy.Dto;

namespace AzureIoTPortal.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

