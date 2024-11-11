namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CRM_Users
    {
        public Guid ID { get; set; }

        [StringLength(500)]
        public string UserName { get; set; }

        [StringLength(500)]
        public string FirstName { get; set; }

        [StringLength(500)]
        public string LastName { get; set; }

        [StringLength(500)]
        public string Nickname { get; set; }

        [StringLength(500)]
        public string Title { get; set; }

        [StringLength(500)]
        public string PrimaryEmail { get; set; }

        [StringLength(500)]
        public string MobilePhone { get; set; }

        [StringLength(500)]
        public string MainPhone { get; set; }

        public DateTime? SyncDate { get; set; }
    }
}
