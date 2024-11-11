namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CRM_OptionSetValue
    {
        public int ID { get; set; }

        public long? OptionSetID { get; set; }

        public long? Value { get; set; }

        [StringLength(500)]
        public string Text { get; set; }

        public string Description { get; set; }
    }
}
