namespace Technova_CRM.Models.EntityModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_Agents
    {
        public Guid ID { get; set; }

        public Guid? SocketID { get; set; }

        public int? SocketStatus { get; set; }

        [StringLength(500)]
        public string AgentNumber { get; set; }

        public Guid? Channel { get; set; }
    }
}
