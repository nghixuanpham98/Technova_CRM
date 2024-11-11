using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Technova_Zalo_Services.Models
{
    public partial class DBContext : DbContext
    {
        public DBContext()
            : base("name=DBContext")
        {
        }
        public virtual DbSet<tbl_ZaloCRMUserStatus> tbl_ZaloCRMUserStatus { get; set; }
        public virtual DbSet<tbl_ZaloMessages> tbl_ZaloMessages { get; set; }
        public virtual DbSet<tbl_ZaloOA> tbl_ZaloOA { get; set; }
        public virtual DbSet<tbl_ZaloSessions> tbl_ZaloSessions { get; set; }
        public virtual DbSet<tbl_ZaloSettings> tbl_ZaloSettings { get; set; }
        public virtual DbSet<tbl_ZaloSettingShareInfo> tbl_ZaloSettingShareInfo { get; set; }
        public virtual DbSet<tbl_ZaloTemplateParamLink> tbl_ZaloTemplateParamLink { get; set; }
        public virtual DbSet<tbl_ZaloTemplateParams> tbl_ZaloTemplateParams { get; set; }
        public virtual DbSet<tbl_ZaloTemplates> tbl_ZaloTemplates { get; set; }
        public virtual DbSet<tbl_ZaloUsers> tbl_ZaloUsers { get; set; }
        public virtual DbSet<tbl_ZaloWebhook> tbl_ZaloWebhook { get; set; }
        public virtual DbSet<tbl_ZaloZNSTransactions> tbl_ZaloZNSTransactions { get; set; }
        public virtual DbSet<tbl_ZaloMessagesBU> tbl_ZaloMessagesBU { get; set; }
        public virtual DbSet<tbl_ZaloWebhookBU> tbl_ZaloWebhookBU { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
