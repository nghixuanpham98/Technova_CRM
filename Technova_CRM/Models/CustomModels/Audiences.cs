namespace Technova_CRM.Models.CustomModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class Audiences
    {
        #region -- tbl_Mc_Audiences --

        public string McID { get; set; }

        public int? WebID { get; set; }

        public string Name { get; set; }

        public string PermissionReminder { get; set; }

        public bool? UseArchiveBar { get; set; }

        public string NotifyOnSubscribe { get; set; }

        public string NotifyOnUnsubscribe { get; set; }

        public DateTime? DateCreated { get; set; }

        public int? ListRating { get; set; }

        public bool? EmailTypeOption { get; set; }

        public string SubscribeUrlShort { get; set; }

        public string SubscribeUrlLong { get; set; }

        public string BeamerAddress { get; set; }

        public string Visibility { get; set; }

        public bool? DoubleOptin { get; set; }

        public bool? HasWelcome { get; set; }

        public bool? MarketingPermissions { get; set; }

        #endregion

        #region -- tbl_Mc_AudienceContacts --

        public string Company { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Country { get; set; }

        public string Phone { get; set; }

        #endregion

        #region -- tbl_Mc_AudienceDefaults --

        public string FromName { get; set; }

        public string FromEmail { get; set; }

        public string Subject { get; set; }

        public string Language { get; set; }

        #endregion

        #region -- tbl_Mc_AudienceStats --

        public int? MemberCount { get; set; }

        public int? TotalContacts { get; set; }

        public int? UnsubscribeCount { get; set; }

        public int? CleanedCount { get; set; }

        public int? MemberCountSinceSend { get; set; }

        public int? UnsubscribeCountSinceSend { get; set; }

        public int? CleanedCountSinceSend { get; set; }

        public int? CampaignCount { get; set; }

        public DateTime? CampaignLastSent { get; set; }

        public int? MergeFieldCount { get; set; }

        public float? AvgSubRate { get; set; }

        public float? AvgUnsubRate { get; set; }

        public float? TargetSubRate { get; set; }

        public float? OpenRate { get; set; }

        public float? ClickRate { get; set; }

        public DateTime? LastSubDate { get; set; }

        public DateTime? LastUnsubDate { get; set; }

        #endregion
        public List<Audiences_Member> Members { get; set; }
    }

    public class Audiences_Member
    {
        public string ListID { get; set; }
        public string EmailAddress { get; set; }
        public string EmailType { get; set; }
        public string Status { get; set; }
        public string Language { get; set; }
        public bool? Vip { get; set; }
    }
}
