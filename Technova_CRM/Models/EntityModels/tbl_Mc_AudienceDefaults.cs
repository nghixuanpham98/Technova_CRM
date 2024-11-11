namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Mc_AudienceDefaults
    {
        public Guid ID { get; set; }

        public Guid? AudienceID { get; set; }

        [StringLength(250)]
        public string FromName { get; set; }

        [StringLength(250)]
        public string FromEmail { get; set; }

        public string Subject { get; set; }

        [StringLength(50)]
        public string Language { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
