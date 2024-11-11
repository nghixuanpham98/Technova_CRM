namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Records
    {
        public Guid ID { get; set; }

        public string Base64 { get; set; }

        public string CallID { get; set; }
    }
}
