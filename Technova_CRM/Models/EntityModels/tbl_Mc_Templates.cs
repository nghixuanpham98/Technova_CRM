namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Mc_Templates
    {
        public Guid ID { get; set; }

        [StringLength(250)]
        public string Slug { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public string Code { get; set; }

        [StringLength(250)]
        public string Subject { get; set; }

        [StringLength(250)]
        public string FromEmail { get; set; }

        [StringLength(250)]
        public string FromName { get; set; }

        public string Text { get; set; }

        [StringLength(250)]
        public string PublishName { get; set; }

        public string PublishCode { get; set; }

        [StringLength(250)]
        public string PublishSubject { get; set; }

        [StringLength(250)]
        public string PublishFromEmail { get; set; }

        [StringLength(250)]
        public string PublishFromName { get; set; }

        public string PublishText { get; set; }

        public DateTime? PublishedAt { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool? IsBrokenTemplate { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
