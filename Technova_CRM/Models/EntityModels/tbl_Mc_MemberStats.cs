namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Mc_MemberStats
    {
        public Guid ID { get; set; }

        public Guid? MemberID { get; set; }

        public double? AvgOpenRate { get; set; }

        public double? AvgClickRate { get; set; }

        public decimal? TotalRevenue { get; set; }

        public int? NumberOfOrders { get; set; }

        [StringLength(50)]
        public string CurrencyCode { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
