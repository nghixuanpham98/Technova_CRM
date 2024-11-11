namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Mc_Members
    {
        public Guid ID { get; set; }

        [StringLength(50)]
        public string McID { get; set; }

        [StringLength(50)]
        public string ListID { get; set; }

        [StringLength(250)]
        public string EmailAddress { get; set; }

        [StringLength(50)]
        public string UniqueEmailID { get; set; }

        [StringLength(50)]
        public string ContactID { get; set; }

        [StringLength(250)]
        public string FullName { get; set; }

        public int? WebID { get; set; }

        [StringLength(50)]
        public string EmailType { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [StringLength(250)]
        public string UnsubscribeReason { get; set; }

        public bool? ConsentsToOneToOneMessaging { get; set; }

        [StringLength(50)]
        public string IpSignup { get; set; }

        [StringLength(50)]
        public string TimestampSignup { get; set; }

        [StringLength(50)]
        public string IpOpt { get; set; }

        public DateTime? TimestampOpt { get; set; }

        public int? MemberRating { get; set; }

        public DateTime? LastChanged { get; set; }

        [StringLength(50)]
        public string Language { get; set; }

        public bool? VIP { get; set; }

        [StringLength(250)]
        public string EmailClient { get; set; }

        [StringLength(50)]
        public string Source { get; set; }

        public int? TagsCount { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
