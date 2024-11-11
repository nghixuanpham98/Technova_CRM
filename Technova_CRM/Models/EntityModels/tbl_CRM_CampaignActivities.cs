namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CRM_CampaignActivities
    {
        public Guid ID { get; set; }

        [StringLength(500)]
        public string Subject { get; set; }

        public Guid? CampaignID { get; set; }

        public int? Channel { get; set; }

        [StringLength(500)]
        public string Desciption { get; set; }

        public DateTime? ScheduledStart { get; set; }

        public DateTime? ScheduledEnd { get; set; }

        public DateTime? ActualStart { get; set; }

        public DateTime? ActualEnd { get; set; }

        public int? Status { get; set; }

        public DateTime? SyncDate { get; set; }
    }
}
