namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class v_CRM_MarketingLists
    {
        [Key]
        public Guid DistID { get; set; }

        public Guid? ActivityID { get; set; }

        public Guid? MarketingListID { get; set; }

        [StringLength(500)]
        public string MarketingListName { get; set; }

        public int? TargetedAt { get; set; }

        [StringLength(500)]
        public string Desciption { get; set; }
    }
}
