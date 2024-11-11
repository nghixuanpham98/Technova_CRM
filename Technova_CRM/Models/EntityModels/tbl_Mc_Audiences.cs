namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Mc_Audiences
    {
        public Guid ID { get; set; }

        [StringLength(50)]
        public string McID { get; set; }

        public int? WebID { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public string PermissionReminder { get; set; }

        public bool? UseArchiveBar { get; set; }

        [StringLength(50)]
        public string NotifyOnSubscribe { get; set; }

        [StringLength(50)]
        public string NotifyOnUnsubscribe { get; set; }

        public DateTime? DateCreated { get; set; }

        public int? ListRating { get; set; }

        public bool? EmailTypeOption { get; set; }

        public string SubscribeUrlShort { get; set; }

        public string SubscribeUrlLong { get; set; }

        public string BeamerAddress { get; set; }

        [StringLength(50)]
        public string Visibility { get; set; }

        public bool? DoubleOptin { get; set; }

        public bool? HasWelcome { get; set; }

        public bool? MarketingPermissions { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
