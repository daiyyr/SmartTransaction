using Abp.Application.Services.Dto;

namespace AzureIoTPortal.Roles.Dto
{
    public class PagedRoleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

