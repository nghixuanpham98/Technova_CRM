namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CRM_MarketingListMembers
    {
        public Guid ID { get; set; }

        public Guid? MarketingListID { get; set; }

        public Guid? MemberID { get; set; }

        public DateTime? SyncDate { get; set; }
    }
}
