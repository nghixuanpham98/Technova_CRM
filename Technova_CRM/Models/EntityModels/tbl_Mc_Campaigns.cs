namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Mc_Campaigns
    {
        public Guid ID { get; set; }

        public Guid? DistributeID { get; set; }

        public Guid? TemplateID { get; set; }

        [StringLength(50)]
        public string McID { get; set; }

        public int? WebID { get; set; }

        [StringLength(50)]
        public string ParentID { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        public DateTime? CreateTime { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public int? EmailsSent { get; set; }

        public DateTime? SendTime { get; set; }

        [StringLength(50)]
        public string ContentType { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
