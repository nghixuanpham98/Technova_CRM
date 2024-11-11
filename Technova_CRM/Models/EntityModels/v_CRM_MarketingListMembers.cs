namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class v_CRM_MarketingListMembers
    {
        public Guid? Cp_ID { get; set; }

        [StringLength(500)]
        public string Cp_Name { get; set; }

        [StringLength(500)]
        public string Cp_Code { get; set; }

        [StringLength(500)]
        public string Cp_Desciption { get; set; }

        public DateTime? Cp_ProposedStart { get; set; }

        public DateTime? Cp_ProposedEnd { get; set; }

        public DateTime? Cp_ActualStart { get; set; }

        public DateTime? Cp_ActualEnd { get; set; }

        public Guid? MkList_ID { get; set; }

        [StringLength(500)]
        public string MkList_Name { get; set; }

        public int? MkList_TargetedAt { get; set; }

        [StringLength(500)]
        public string MkList_TargetedAtText { get; set; }

        [StringLength(500)]
        public string MkList_Description { get; set; }

        public Guid? Mem_ID { get; set; }

        [StringLength(1001)]
        public string Mem_Name { get; set; }

        [StringLength(500)]
        public string Mem_Email { get; set; }

        [StringLength(500)]
        public string Mem_Phone { get; set; }

        public Guid? Atv_ID { get; set; }

        [StringLength(500)]
        public string Atv_Subject { get; set; }

        public int? Atv_Channel { get; set; }

        [StringLength(500)]
        public string Atv_ChannelText { get; set; }

        [StringLength(500)]
        public string Atv_Desciption { get; set; }

        public DateTime? Atv_ScheduledStart { get; set; }

        public DateTime? Atv_ScheduledEnd { get; set; }

        public DateTime? Atv_ActualStart { get; set; }

        public DateTime? Atv_ActualEnd { get; set; }

        [Key]
        public Guid Dtb_ID { get; set; }

        public int? Dtb_Type { get; set; }

        [StringLength(500)]
        public string Dtb_TypeText { get; set; }

        public int? Dtb_Status { get; set; }

        [StringLength(500)]
        public string Dtb_StatusText { get; set; }

        public Guid? TemplateID { get; set; }
    }
}
