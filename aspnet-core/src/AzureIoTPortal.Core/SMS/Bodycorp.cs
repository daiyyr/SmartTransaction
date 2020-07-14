using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace AzureIoTPortal.SMS
{
    public class Bodycorp
    {
        
        [Key]
        public int bodycorp_id { get; set; }



        public string bodycorp_code { get; set; }
        public string bodycorp_name { get; set; }
        public int? bodycorp_account_id { get; set; }
        public DateTime? bodycorp_begin_date { get; set; }


        //public virtual ICollection<comm_master> commMasters { get; set; }

        public Bodycorp()
        {
            //this.commMasters = new HashSet<comm_master>();
        }
    }
}
