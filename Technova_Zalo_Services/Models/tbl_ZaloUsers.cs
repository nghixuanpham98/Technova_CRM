namespace Technova_Zalo_Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_ZaloUsers
    {
        public Guid ID { get; set; }

        [StringLength(100)]
        public string UserId { get; set; }

        [StringLength(100)]
        public string UserIdByApp { get; set; }

        [StringLength(500)]
        public string Avatar { get; set; }

        [StringLength(500)]
        public string DisplayName { get; set; }

        [StringLength(500)]
        public string Alias { get; set; }

        public bool? IsSensitive { get; set; }

        [StringLength(100)]
        public string LastInteractionDate { get; set; }

        public DateTime? LastInteractionOn { get; set; }

        public bool? IsFollower { get; set; }

        public string Notes { get; set; }

        public string Tags { get; set; }

        [StringLength(100)]
        public string LastMsgId { get; set; }

        public DateTime? LastRequestInfoOn { get; set; }

        [StringLength(100)]
        public string Phone { get; set; }

        public string Address { get; set; }

        [StringLength(500)]
        public string District { get; set; }

        [StringLength(500)]
        public string City { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
