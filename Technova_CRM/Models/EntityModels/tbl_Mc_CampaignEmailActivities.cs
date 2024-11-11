namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Mc_CampaignEmailActivities
    {
        public Guid ID { get; set; }

        public Guid? EmailID { get; set; }

        [StringLength(250)]
        public string Action { get; set; }

        public DateTime? Timestamp { get; set; }

        [StringLength(50)]
        public string Ip { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
