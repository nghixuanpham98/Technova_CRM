namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class v_getListMail
    {
        public Guid ID { get; set; }

        public Guid? DistributeID { get; set; }

        [StringLength(500)]
        public string Subject { get; set; }

        public string Content { get; set; }

        [StringLength(250)]
        public string ToEmail { get; set; }

        public int? MkList_TargetedAt { get; set; }

        public Guid? Mem_ID { get; set; }

        public Guid? ActivityID { get; set; }

        public int? Status { get; set; }
    }
}
