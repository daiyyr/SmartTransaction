using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureIoTPortal.AzureIoT
{
    public class Device :Entity<int>,IHasCreationTime
    {
        public string iot_id { get; set; }
        public string primary_key { get; set; }
        public string conneciton_string { get; set; }
        public string connection_state { get; set; }
        public DateTime? last_conection_state_update_time { get; set; }
        public DateTime? last_activity_time { get; set; }
        public string state { get; set; }
        public DateTime CreationTime { get; set; }
       
        public string type { get; set; }
        public int message_count { get; set; }
        public string chart_x { get; set; }
        public string chart_y { get; set; }
        public List<Event> events { get; set; }
        public Device()
        {
            CreationTime = DateTime.Now;
            message_count = 0;
        }
    }
}
