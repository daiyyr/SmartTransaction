using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Domain.Repositories;
using AutoMapper;
using AzureIoTPortal.AzureIoT.Dto;

namespace AzureIoTPortal.AzureIoT
{
    public class DeviceAppService : AzureIoTPortalAppServiceBase, IDeviceAppService
    {
        private readonly IRepository<Device> _DevcieRepository;
        private readonly IRepository<Event> _EventRepository;
        public DeviceAppService(IRepository<Device> DeviceRepository,IRepository<Event> EventRepository)
        {
            _DevcieRepository = DeviceRepository;
            _EventRepository = EventRepository;
        }
        public List<Device> GetDevices()
        {
            return _DevcieRepository.GetAllIncluding(x => x.events).ToList();

            
        }

        public List<Device> GetDevicesWithLastEvent(int nCount, bool get_events = true)
        {
            var devices = _DevcieRepository.GetAllList();

            if ( 
                devices != null && 
                (devices.Count < 20000 || get_events)
                )
            {
                foreach (var device in devices)
                {
                    if (device.events == null)
                    {
                        device.events = new List<Event>();
                    }
                    device.events = _EventRepository.GetAll().Where(x => x.deviceId == device.Id).OrderByDescending(x => x.iot_event_time).Take(nCount).ToList();

                }
            }
            return devices;
        }
    }
}
