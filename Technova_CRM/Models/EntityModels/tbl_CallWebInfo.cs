namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CallWebInfo
    {
        public Guid ID { get; set; }

        public Guid? GuestID { get; set; }

        [StringLength(500)]
        public string Location { get; set; }

        [StringLength(500)]
        public string IP { get; set; }

        [StringLength(500)]
        public string Browser { get; set; }

        public string CallID { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}
