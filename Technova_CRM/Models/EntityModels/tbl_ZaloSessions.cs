namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ZaloSessions
    {
        public Guid ID { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? ResponseTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int? Status { get; set; }

        public Guid? OwnerId { get; set; }

        [StringLength(100)]
        public string UserId { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
