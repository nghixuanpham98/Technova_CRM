namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Mc_CampaignSettings
    {
        public Guid ID { get; set; }

        public Guid? CampaignID { get; set; }

        [StringLength(250)]
        public string SubjectLine { get; set; }

        public string PreviewText { get; set; }

        [StringLength(250)]
        public string Title { get; set; }

        [StringLength(250)]
        public string FromName { get; set; }

        [StringLength(250)]
        public string ReplyTo { get; set; }

        public bool? UseConversation { get; set; }

        [StringLength(250)]
        public string ToName { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
