namespace Technova_CRM.Models.CustomModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class CampaignContents
    {
        public string CampaignID { get; set; }
        public Guid? TemplateID { get; set; }
    }
}
