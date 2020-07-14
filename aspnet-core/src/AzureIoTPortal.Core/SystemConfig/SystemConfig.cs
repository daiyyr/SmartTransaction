using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureIoTPortal.SystemConfig
{
    public class SystemConfig : Entity<int>
    {
        public string system_code { get; set; }
        public string system_value { get; set; }
        public string system_description { get; set; }
    }
}
