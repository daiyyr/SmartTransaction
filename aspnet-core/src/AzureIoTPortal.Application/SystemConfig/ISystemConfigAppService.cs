using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;


namespace AzureIoTPortal.SystemConfig
{
    public interface ISystemConfigAppService : IApplicationService
    {
        string GetConfig(string systemCode);
    }
}
