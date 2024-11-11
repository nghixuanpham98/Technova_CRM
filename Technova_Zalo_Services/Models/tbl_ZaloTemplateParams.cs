namespace Technova_Zalo_Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ZaloTemplateParams
    {
        public Guid ID { get; set; }

        public int? TemplateID { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public bool? Require { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        public int? MaxLength { get; set; }

        public int? MinLength { get; set; }

        public bool? AcceptNull { get; set; }

        public string LinkWithEntity { get; set; }

        public string LinkWithField { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
