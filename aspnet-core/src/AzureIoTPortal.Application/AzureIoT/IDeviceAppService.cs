using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;
using AzureIoTPortal.AzureIoT.Dto;

namespace AzureIoTPortal.AzureIoT
{
    public interface IDeviceAppService :IApplicationService
    {
        List<Device> GetDevices();

        List<Device> GetDevicesWithLastEvent(int nCount, bool get_events = true);
    }
}
