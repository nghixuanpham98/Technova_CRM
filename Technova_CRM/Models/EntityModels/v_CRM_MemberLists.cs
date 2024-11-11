namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class v_CRM_MemberLists
    {
        public Guid ID { get; set; }

        [StringLength(1001)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Email { get; set; }

        [StringLength(500)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string ZaloID { get; set; }
    }
}
