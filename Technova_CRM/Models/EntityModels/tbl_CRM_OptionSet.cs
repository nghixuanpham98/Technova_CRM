namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CRM_OptionSet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long OptionSetID { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
