namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CRM_Leads
    {
        public Guid ID { get; set; }

        [StringLength(500)]
        public string Topic { get; set; }

        [StringLength(500)]
        public string FullName { get; set; }

        [StringLength(500)]
        public string JobTitle { get; set; }

        [StringLength(500)]
        public string BusinessPhone { get; set; }

        [StringLength(500)]
        public string MobilePhone { get; set; }

        [StringLength(500)]
        public string Email { get; set; }

        [StringLength(500)]
        public string Company { get; set; }

        [StringLength(100)]
        public string ZaloID { get; set; }

        public DateTime? SyncDate { get; set; }
    }
}
