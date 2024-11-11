namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class v_CRM_Distribute
    {
        [Key]
        public Guid DistID { get; set; }

        public int? DistType { get; set; }

        [StringLength(500)]
        public string DistTypeText { get; set; }

        public int? DistStatus { get; set; }

        [StringLength(500)]
        public string DistStatusText { get; set; }

        public DateTime? SendTime { get; set; }

        public Guid? TemplateID { get; set; }

        [StringLength(250)]
        public string TemplateName { get; set; }

        public string TemplateContent { get; set; }

        public Guid? ActivityID { get; set; }

        [StringLength(500)]
        public string Subject { get; set; }

        public int? ActStatus { get; set; }

        public int? Channel { get; set; }

        [StringLength(500)]
        public string ChannelText { get; set; }

        [StringLength(500)]
        public string Desciption { get; set; }

        public DateTime? ScheduledStart { get; set; }

        public DateTime? ScheduledEnd { get; set; }

        public DateTime? ActualStart { get; set; }

        public DateTime? ActualEnd { get; set; }
    }
}
