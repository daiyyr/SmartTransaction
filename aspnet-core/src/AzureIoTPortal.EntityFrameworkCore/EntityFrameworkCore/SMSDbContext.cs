using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using AzureIoTPortal.Authorization.Roles;
using AzureIoTPortal.Authorization.Users;
using AzureIoTPortal.MultiTenancy;
using AzureIoTPortal.SystemConfig;
using AzureIoTPortal.SMS;

namespace AzureIoTPortal.EntityFrameworkCore
{
    public class SMSDbContext : DbContext
    {
        public SMSDbContext(DbContextOptions<SMSDbContext> options)
            : base(options)
        {
        }




        //entities
        public DbSet<Bodycorp> Bodycorps { get; set; }

        /*
        public DbSet<chart_master> chart_masters { get; set; }
        public DbSet<gl_transactions> gl_transactions { get; set; }
        public DbSet<invoice_master> invoice_masters { get; set; }
        public DbSet<cinvoice> cinvoices { get; set; }
        public DbSet<receipt> receipts { get; set; }
        public DbSet<cpayment> cpayments { get; set; }
        public DbSet<unit_master> units { get; set; }
        public DbSet<debtor_master> debtors { get; set; }
        public DbSet<contact_master> contacts { get; set; }
        public DbSet<comm_master> comms { get; set; }
        public DbSet<contact_type> contact_types { get; set; }
        public DbSet<comm_type> comm_types { get; set; }
        */



    }
}
