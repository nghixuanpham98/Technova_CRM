namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CRM_MarketingLists
    {
        public Guid ID { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public int? TargetedAt { get; set; }

        [StringLength(500)]
        public string Desciption { get; set; }

        public DateTime? SyncDate { get; set; }
    }
}
