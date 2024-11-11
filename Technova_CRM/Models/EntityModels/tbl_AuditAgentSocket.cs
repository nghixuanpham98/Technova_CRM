namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_AuditAgentSocket
    {
        public Guid ID { get; set; }

        [StringLength(500)]
        public string Agent { get; set; }

        public Guid? SocketID { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? Time { get; set; }
    }
}
