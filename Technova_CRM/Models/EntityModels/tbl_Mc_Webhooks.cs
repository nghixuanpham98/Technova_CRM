namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Mc_Webhooks
    {
        public Guid ID { get; set; }

        [StringLength(250)]
        public string EventName { get; set; }

        [StringLength(250)]
        public string EventFrom { get; set; }

        public string RawContent { get; set; }

        public int? Status { get; set; }

        [StringLength(250)]
        public string Timestamp { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
