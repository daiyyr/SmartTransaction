using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureIoTPortal.AzureIoT
{
    public class Event : Entity<int>, IHasCreationTime
    {
        public DateTime CreationTime { get; set; }
        public DateTime? iot_event_time { get; set; }
        public string data { get; set; }
        public long queue_number { get; set; }
        public int deviceId { get; set; }
    }
}
