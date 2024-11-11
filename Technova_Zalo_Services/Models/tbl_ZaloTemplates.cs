namespace Technova_Zalo_Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ZaloTemplates
    {
        public Guid ID { get; set; }

        public int? TemplateID { get; set; }

        [StringLength(500)]
        public string TemplateName { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public int? Type { get; set; }

        public long? Timeout { get; set; }

        public string PreviewUrl { get; set; }

        public string TemplateQuality { get; set; }

        public string TemplateTag { get; set; }

        public double? Price { get; set; }

        public bool? ApplyTemplateQuota { get; set; }

        public long? CreatedTime { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
