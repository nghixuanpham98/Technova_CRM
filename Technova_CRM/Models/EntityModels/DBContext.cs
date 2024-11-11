using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Technova_CRM.Models.EntityModels
{
    public partial class DBContext : DbContext
    {
        public DBContext()
            : base("name=DBContext")
        {
        }

        #region -- Table --
        public virtual DbSet<tbl_Agents> tbl_Agents { get; set; }
        public virtual DbSet<tbl_AuditAgentSocket> tbl_AuditAgentSocket { get; set; }
        public virtual DbSet<tbl_CallHistory> tbl_CallHistory { get; set; }
        public virtual DbSet<tbl_CallWebInfo> tbl_CallWebInfo { get; set; }
        public virtual DbSet<tbl_Channel> tbl_Channel { get; set; }
        public virtual DbSet<tbl_ConfigCallToWeb> tbl_ConfigCallToWeb { get; set; }
        public virtual DbSet<tbl_ConfigWrapupCall> tbl_ConfigWrapupCall { get; set; }
        public virtual DbSet<tbl_CRM_Accounts> tbl_CRM_Accounts { get; set; }
        public virtual DbSet<tbl_CRM_CampaignActivitiesItem> tbl_CRM_CampaignActivitiesItem { get; set; }
        public virtual DbSet<tbl_CRM_CampMarketingLists> tbl_CRM_CampMarketingLists { get; set; }
        public virtual DbSet<tbl_CRM_Contacts> tbl_CRM_Contacts { get; set; }
        public virtual DbSet<tbl_CRM_Distribute> tbl_CRM_Distribute { get; set; }
        public virtual DbSet<tbl_CRM_Leads> tbl_CRM_Leads { get; set; }
        public virtual DbSet<tbl_CRM_OptionSet> tbl_CRM_OptionSet { get; set; }
        public virtual DbSet<tbl_CRM_OptionSetValue> tbl_CRM_OptionSetValue { get; set; }
        public virtual DbSet<tbl_Mc_AudienceContacts> tbl_Mc_AudienceContacts { get; set; }
        public virtual DbSet<tbl_Mc_AudienceDefaults> tbl_Mc_AudienceDefaults { get; set; }
        public virtual DbSet<tbl_Mc_Audiences> tbl_Mc_Audiences { get; set; }
        public virtual DbSet<tbl_Mc_AudienceStats> tbl_Mc_AudienceStats { get; set; }
        public virtual DbSet<tbl_Mc_CampaignEmailActivities> tbl_Mc_CampaignEmailActivities { get; set; }
        public virtual DbSet<tbl_Mc_CampaignEmails> tbl_Mc_CampaignEmails { get; set; }
        public virtual DbSet<tbl_Mc_CampaignRecipients> tbl_Mc_CampaignRecipients { get; set; }
        public virtual DbSet<tbl_Mc_Campaigns> tbl_Mc_Campaigns { get; set; }
        public virtual DbSet<tbl_Mc_CampaignSettings> tbl_Mc_CampaignSettings { get; set; }
        public virtual DbSet<tbl_Mc_Configs> tbl_Mc_Configs { get; set; }
        public virtual DbSet<tbl_Mc_Templates> tbl_Mc_Templates { get; set; }
        public virtual DbSet<tbl_Mc_Webhooks> tbl_Mc_Webhooks { get; set; }
        public virtual DbSet<tbl_QueueAgent> tbl_QueueAgent { get; set; }
        public virtual DbSet<tbl_QueueAgentGuest> tbl_QueueAgentGuest { get; set; }
        public virtual DbSet<tbl_QueueCall> tbl_QueueCall { get; set; }
        public virtual DbSet<tbl_QueueCallGuest> tbl_QueueCallGuest { get; set; }
        public virtual DbSet<tbl_Records> tbl_Records { get; set; }
        public virtual DbSet<tbl_Users> tbl_Users { get; set; }
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
        public virtual DbSet<tbl_CRM_CampaignActivities> tbl_CRM_CampaignActivities { get; set; }
        public virtual DbSet<tbl_CRM_Campaigns> tbl_CRM_Campaigns { get; set; }
        public virtual DbSet<tbl_CRM_MarketingListMembers> tbl_CRM_MarketingListMembers { get; set; }
        public virtual DbSet<tbl_CRM_MarketingLists> tbl_CRM_MarketingLists { get; set; }
        public virtual DbSet<tbl_CRM_Users> tbl_CRM_Users { get; set; }
        public virtual DbSet<tbl_ZaloMessagesBU> tbl_ZaloMessagesBU { get; set; }
        public virtual DbSet<tbl_ZaloWebhookBU> tbl_ZaloWebhookBU { get; set; }

        #endregion

        #region -- View --

        public virtual DbSet<v_CRM_Distribute> v_CRM_Distribute { get; set; }
        public virtual DbSet<v_CRM_MarketingListMembers> v_CRM_MarketingListMembers { get; set; }
        public virtual DbSet<v_CRM_MemberLists> v_CRM_MemberLists { get; set; }
        public virtual DbSet<v_Mc_Campaigns> v_Mc_Campaigns { get; set; }
        public virtual DbSet<v_UserByLastCall> v_UserByLastCall { get; set; }
        public virtual DbSet<v_Zalo_CRMUserStatus> v_Zalo_CRMUserStatus { get; set; }
        public virtual DbSet<v_Zalo_Sessions> v_Zalo_Sessions { get; set; }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
