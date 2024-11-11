namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_QueueCall
    {
        public Guid ID { get; set; }

        public string CallID { get; set; }

        public string Phone { get; set; }

        public DateTime? Time { get; set; }
    }
}
