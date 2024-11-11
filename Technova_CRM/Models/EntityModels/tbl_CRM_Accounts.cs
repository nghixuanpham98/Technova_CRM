namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CRM_Accounts
    {
        public Guid ID { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Phone { get; set; }

        [StringLength(500)]
        public string Fax { get; set; }

        [StringLength(500)]
        public string Website { get; set; }

        public Guid? ParentID { get; set; }

        public DateTime? SyncDate { get; set; }

        [StringLength(500)]
        public string Email { get; set; }
    }
}
