namespace Technova_Zalo_Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ZaloOA
    {
        [Key]
        [StringLength(100)]
        public string OAID { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool? IsVerified { get; set; }

        public int? OAType { get; set; }

        public string CateName { get; set; }

        public int? NumFollower { get; set; }

        public string Avatar { get; set; }

        public string Cover { get; set; }

        public string PackageName { get; set; }

        [StringLength(100)]
        public string PackageValidThroughDate { get; set; }

        [StringLength(100)]
        public string PackageAutoRenewDate { get; set; }

        [StringLength(100)]
        public string LinkedZca { get; set; }

        public int? RemainingQuotaPromotion { get; set; }

        public int? RemainingQuota { get; set; }

        public int? DailyQuotaPromotion { get; set; }

        public int? DailyQuota { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
