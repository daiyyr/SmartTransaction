using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using AzureIoTPortal.AzureIoT;
using AzureIoTPortal.AzureIoT.Dto;
namespace AzureIoTPortal.DtoMapping
{
    public class DeviceDtoMapping :IDtoMapping
    {
        public void CreateMapping(IMapperConfigurationExpression mapperConfig)
        {
            mapperConfig.CreateMap<Device, DeviceDto>();
            mapperConfig.CreateMap<DeviceDto, Device>();
        }
    }
}
