namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CallHistory
    {
        public Guid ID { get; set; }

        public DateTime? StartTime { get; set; }

        [StringLength(50)]
        public string CallingNumber { get; set; }

        [StringLength(50)]
        public string CalledNumber { get; set; }

        public DateTime? ConnectTime { get; set; }

        public DateTime? DisconnectTime { get; set; }

        public int? Direction { get; set; }

        public int? Status { get; set; }

        public string CallID { get; set; }
    }
}
