using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureIoTPortal.AzureIoT.Dto
{
    [AutoMapFrom(typeof(Device))]
    public class DeviceDto : EntityDto
    {
        public string iot_id { get; set; }
        public string primary_key { get; set; }
        public string conneciton_string { get; set; }
        public string connection_state { get; set; }
        public DateTime? last_conection_state_update_time { get; set; }
        public DateTime? last_activity_time { get; set; }
        public string state { get; set; }
        public DateTime CreationTime { get; set; }

        public List<Event> events { get; set; }
    }
}
