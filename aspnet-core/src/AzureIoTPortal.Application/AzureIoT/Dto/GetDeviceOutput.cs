using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureIoTPortal.AzureIoT.Dto
{
    public class GetDeviceOutput
    {
        public List<DeviceDto> Devcies { get; set; }
    }
}
