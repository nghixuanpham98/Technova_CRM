namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Mc_CampaignEmails
    {
        public Guid ID { get; set; }

        public Guid? DistributeID { get; set; }

        [StringLength(250)]
        public string McEmailID { get; set; }

        [StringLength(250)]
        public string McCampaignID { get; set; }

        [StringLength(250)]
        public string McListID { get; set; }

        [StringLength(500)]
        public string Subject { get; set; }

        public string Content { get; set; }

        [StringLength(250)]
        public string ToEmail { get; set; }

        public int? TargetedAt { get; set; }

        public int? Status { get; set; }

        public bool? IsSync { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
