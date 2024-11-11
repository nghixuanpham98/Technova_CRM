namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CRM_Distribute
    {
        public Guid ID { get; set; }

        public Guid? ActivityID { get; set; }

        public Guid? TemplateID { get; set; }

        public int? Type { get; set; }

        public int? Status { get; set; }

        public DateTime? SendTime { get; set; }
    }
}
