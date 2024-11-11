namespace Technova_Zalo_Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ZaloZNSTransactions
    {
        public Guid ID { get; set; }

        public int? TemplateID { get; set; }

        [StringLength(100)]
        public string Phone { get; set; }

        public string Input { get; set; }

        public string Output { get; set; }

        public DateTime? SentOn { get; set; }
    }
}
