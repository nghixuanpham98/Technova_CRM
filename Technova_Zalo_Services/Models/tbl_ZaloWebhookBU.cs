namespace Technova_Zalo_Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ZaloWebhookBU
    {
        public Guid ID { get; set; }

        [StringLength(100)]
        public string EventName { get; set; }

        [StringLength(100)]
        public string EventTime { get; set; }

        public string Content { get; set; }

        public int? Status { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? SessionID { get; set; }
    }
}
