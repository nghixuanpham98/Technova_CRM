namespace Technova_CRM.Models.CustomModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Campaigns
    {
        #region -- tbl_Mc_Campaigns --

        public string McID { get; set; }

        public int? WebID { get; set; }

        public string ParentID { get; set; }

        public string Type { get; set; }

        public DateTime? CreateTime { get; set; }

        public string ArchiveUrl { get; set; }

        public string LongArchiveUrl { get; set; }

        public string Status { get; set; }

        public int? EmailsSent { get; set; }

        public DateTime? SendTime { get; set; }

        public string ContentType { get; set; }

        public bool? NeedsBlockRefresh { get; set; }

        public bool? Resendable { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignSettings --

        public string SubjectLine { get; set; }

        public string PreviewText { get; set; }

        public string STitle { get; set; }

        public string FromName { get; set; }

        public string ReplyTo { get; set; }

        public bool? UseConversation { get; set; }

        public string ToName { get; set; }

        public string FolderID { get; set; }

        public bool? Authenticate { get; set; }

        public bool? AutoFooter { get; set; }

        public bool? InlineCSS { get; set; }

        public bool? AutoTweet { get; set; }

        public bool? FBComments { get; set; }

        public bool? Timewarp { get; set; }

        public int? TemplateID { get; set; }

        public bool? DragAndDrop { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignSocialCard --

        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public string SCTitle { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignTracking --

        public bool? Opens { get; set; }

        public bool? HtmlClicks { get; set; }

        public bool? TextClicks { get; set; }

        public bool? GoalTracking { get; set; }

        public bool? Ecomm360 { get; set; }

        public string GoogleAnalytics { get; set; }

        public string Clicktale { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignT_Capsule --

        public bool? CapsuleNotes { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignT_Salesforce --

        public bool? Campaign { get; set; }

        public bool? SalesforceNotes { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignVariateSettings --

        public string WinningCombinationID { get; set; }

        public string WinningCampaignID { get; set; }

        public string WinnerCriteria { get; set; }

        public int? CVWaitTime { get; set; }

        public int? TestSize { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignV_Combinations --

        public string CombinationID { get; set; }

        public int? CVSubjectLine { get; set; }

        public int? CVCSendTime { get; set; }

        public int? CVCFromName { get; set; }

        public int? CVReplyTo { get; set; }

        public int? ContentDescription { get; set; }

        public int? Recipients { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignV_Contents --

        public string Content { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignV_FromNames --

        public string CVFFromName { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignV_ReplyToAddresses --

        public string ReplyToAddress { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignV_SendTimes --

        public DateTime? CVSSendTime { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignV_SubjectLines --

        public string CVSSubjectLine { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignABSplitOptions --

        public string SplitTest { get; set; }

        public string PickWinner { get; set; }

        public string WaitUnits { get; set; }

        public int? CABSplitOptionWaitTime { get; set; }

        public int? SplitSize { get; set; }

        public string FromNameA { get; set; }

        public string FromNameB { get; set; }

        public string ReplyEmailA { get; set; }

        public string ReplyEmailB { get; set; }

        public string SubjectA { get; set; }

        public string SubjectB { get; set; }

        public DateTime? SendTimeA { get; set; }

        public DateTime? SendTimeB { get; set; }

        public string SendTimeWinner { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignDeliveryStatus --

        public bool? Enabled { get; set; }

        public bool? CanCancel { get; set; }

        public string CDeliveryStatus { get; set; }

        public int? CDeliveryEmailsSent { get; set; }

        public int? EmailsCanceled { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignRecipients --

        public string ListID { get; set; }

        public bool? ListIsActive { get; set; }

        public string ListName { get; set; }

        public string SegmentText { get; set; }

        public int? RecipientCount { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignReportSummary --

        public int? CReportOpens { get; set; }

        public int? UniqueOpens { get; set; }

        public float? OpenRate { get; set; }

        public int? Clicks { get; set; }

        public int? SubscriberClicks { get; set; }

        public float? ClickRate { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignRS_Ecommerce --

        public int? TotalOrders { get; set; }

        public decimal? TotalSpent { get; set; }

        public decimal? TotalRevenue { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignRSSOptions --

        public string FeedUrl { get; set; }

        public string Frequency { get; set; }

        public DateTime? LastSent { get; set; }

        public bool? ConstrainRssImg { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignRSS_Schedule --

        public int? Hour { get; set; }

        public string WeeklySendDay { get; set; }

        public int? MonthlySendDate { get; set; }

        #endregion
    }
}
