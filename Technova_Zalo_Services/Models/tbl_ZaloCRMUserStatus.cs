namespace Technova_Zalo_Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ZaloCRMUserStatus
    {
        public Guid ID { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }

        public bool? Status { get; set; }

        public DateTime? LastStatusOn { get; set; }
    }
}
