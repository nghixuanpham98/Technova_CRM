namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ConfigWrapupCall
    {
        public Guid ID { get; set; }

        public int? Status { get; set; }

        public int? Second { get; set; }
    }
}
