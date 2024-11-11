namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CRM_Campaigns
    {
        public Guid ID { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Code { get; set; }

        [StringLength(500)]
        public string Desciption { get; set; }

        public DateTime? ProposedStart { get; set; }

        public DateTime? ProposedEnd { get; set; }

        public DateTime? ActualStart { get; set; }

        public DateTime? ActualEnd { get; set; }

        public DateTime? SyncDate { get; set; }
    }
}
