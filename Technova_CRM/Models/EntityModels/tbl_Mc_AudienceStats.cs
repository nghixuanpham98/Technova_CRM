namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Mc_AudienceStats
    {
        public Guid ID { get; set; }

        public Guid? AudienceID { get; set; }

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

        public double? AvgSubRate { get; set; }

        public double? AvgUnsubRate { get; set; }

        public double? TargetSubRate { get; set; }

        public double? OpenRate { get; set; }

        public double? ClickRate { get; set; }

        public DateTime? LastSubDate { get; set; }

        public DateTime? LastUnsubDate { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
