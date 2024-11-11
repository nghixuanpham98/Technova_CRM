namespace Technova_CRM.Models.CustomModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CampaignsFilterModel
    {
        public string Fields { get; set; }
        public string ExcludeFields { get; set; }
        public int? Count { get; set; } = 10;
        public int? Offset { get; set; } = 0;
        public string Type { get; set; }
        public string Status { get; set; }
        public string BeforeSendTime { get; set; }
        public string SinceSendTime { get; set; }
        public string BeforeCreateTime { get; set; }
        public string SinceCreateTime { get; set; }
        public string ListID { get; set; }
        public string FolderID { get; set; }
        public string MemberID { get; set; }
        public string SortField { get; set; }
        public string SortDir { get; set; }
    }
}
