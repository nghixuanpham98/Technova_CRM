namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Mc_CampaignRecipients
    {
        public Guid ID { get; set; }

        public Guid? CampaignID { get; set; }

        [StringLength(50)]
        public string ListID { get; set; }

        public bool? ListIsActive { get; set; }

        [StringLength(250)]
        public string ListName { get; set; }

        public int? RecipientCount { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
