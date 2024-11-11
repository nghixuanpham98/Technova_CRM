namespace Technova_Zalo_Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ZaloSettings
    {
        public int ID { get; set; }

        [StringLength(500)]
        public string Text { get; set; }

        public string Value { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
