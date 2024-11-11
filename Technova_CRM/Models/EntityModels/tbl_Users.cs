namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Users
    {
        public Guid ID { get; set; }

        [Required]
        [StringLength(250)]
        public string UserName { get; set; }

        [StringLength(50)]
        public string DisplayName { get; set; }

        public Guid? AgentID { get; set; }

        public Guid? CRMID { get; set; }
    }
}
