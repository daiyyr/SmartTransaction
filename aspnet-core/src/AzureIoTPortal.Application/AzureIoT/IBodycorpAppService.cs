using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;
using AzureIoTPortal.AzureIoT.Dto;
using AzureIoTPortal.SMS;

namespace AzureIoTPortal.AzureIoT
{
    public interface IBodycorpAppService :IApplicationService
    {
        List<Bodycorp> GetBodycorps();

    }
}
