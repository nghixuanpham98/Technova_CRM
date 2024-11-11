namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CRM_CampMarketingLists
    {
        public Guid ID { get; set; }

        public Guid? MarketingListID { get; set; }

        public DateTime? SyncDate { get; set; }

        public Guid? CampaignID { get; set; }
    }
}
