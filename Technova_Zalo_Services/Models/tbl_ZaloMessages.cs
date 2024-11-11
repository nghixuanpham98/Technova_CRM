namespace Technova_Zalo_Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ZaloMessages
    {
        public Guid ID { get; set; }

        [StringLength(100)]
        public string MsgId { get; set; }

        [StringLength(100)]
        public string FromId { get; set; }

        [StringLength(100)]
        public string ToId { get; set; }

        public DateTime? MsgTime { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        public string Message { get; set; }

        public Guid? SessionID { get; set; }
    }
}
