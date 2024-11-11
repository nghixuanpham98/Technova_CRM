namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class v_Mc_Campaigns
    {
        [Key]
        public Guid CampaignID { get; set; }

        [StringLength(50)]
        public string McID { get; set; }

        public int? WebID { get; set; }

        [StringLength(50)]
        public string ParentID { get; set; }

        [StringLength(50)]
        public string CampaignType { get; set; }

        public DateTime? CreateTime { get; set; }

        [StringLength(50)]
        public string CampaignStatus { get; set; }

        public int? EmailsSent { get; set; }

        public DateTime? SendTime { get; set; }

        [StringLength(50)]
        public string ContentType { get; set; }

        public Guid? DistributeID { get; set; }

        public int? DistType { get; set; }

        [StringLength(500)]
        public string DistTypeText { get; set; }

        public int? DistStatus { get; set; }

        [StringLength(500)]
        public string DistStatusText { get; set; }

        public DateTime? DistSendTime { get; set; }

        public Guid? TemplateID { get; set; }

        [StringLength(250)]
        public string TemplateName { get; set; }

        public string TemplateContent { get; set; }

        public Guid? ActID { get; set; }

        [StringLength(500)]
        public string ActSubject { get; set; }

        public int? ActStatus { get; set; }

        public int? Channel { get; set; }

        [StringLength(500)]
        public string ChannelText { get; set; }

        [StringLength(500)]
        public string Desciption { get; set; }
    }
}
