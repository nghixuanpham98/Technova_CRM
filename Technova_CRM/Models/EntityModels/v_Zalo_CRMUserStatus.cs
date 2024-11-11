namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class v_Zalo_CRMUserStatus
    {
        public Guid? ID { get; set; }

        [StringLength(500)]
        public string UserName { get; set; }

        public DateTime? LastStatusOn { get; set; }

        public bool? Status { get; set; }

        [StringLength(500)]
        public string StatusText { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NumberOfInteractions { get; set; }
    }
}
