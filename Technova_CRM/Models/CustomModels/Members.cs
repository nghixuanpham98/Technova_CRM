namespace Technova_CRM.Models.CustomModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class Members
    {
        #region -- tbl_Mc_Members --

        public string McID { get; set; }

        public string ListID { get; set; }

        public string EmailAddress { get; set; }

        public string UniqueEmailID { get; set; }

        public string ContactID { get; set; }

        public string FullName { get; set; }

        public int? WebID { get; set; }

        public string EmailType { get; set; }

        public string Status { get; set; }

        public string UnsubscribeReason { get; set; }

        public bool? ConsentsToOneToOneMessaging { get; set; }

        public string IpSignup { get; set; }

        public string TimestampSignup { get; set; }

        public string IpOpt { get; set; }

        public DateTime? TimestampOpt { get; set; }

        public int? MemberRating { get; set; }

        public DateTime? LastChanged { get; set; }

        public string Language { get; set; }

        public bool? VIP { get; set; }

        public string EmailClient { get; set; }

        public string Source { get; set; }

        public int? TagsCount { get; set; }

        #endregion

        #region -- tbl_Mc_MemberStats --

        public float? AvgOpenRate { get; set; }

        public float? AvgClickRate { get; set; }

        public decimal? TotalRevenue { get; set; }

        public int? NumberOfOrders { get; set; }

        public string CurrencyCode { get; set; }

        #endregion

        #region -- tbl_Mc_MemberMergeFields --

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public string Birthday { get; set; }

        public string Gender { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Country { get; set; }

        #endregion

        #region -- tbl_Mc_MemberLastNotes --

        public int? NoteID { get; set; }

        public DateTime? NoteCreatedAt { get; set; }

        public string NoteCreatedBy { get; set; }

        public string Note { get; set; }

        #endregion
    }
}
