namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Mc_AudienceContacts
    {
        public Guid ID { get; set; }

        public Guid? AudienceID { get; set; }

        [StringLength(250)]
        public string Company { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        [StringLength(250)]
        public string City { get; set; }

        [StringLength(250)]
        public string State { get; set; }

        [StringLength(50)]
        public string Zip { get; set; }

        [StringLength(250)]
        public string Country { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}
