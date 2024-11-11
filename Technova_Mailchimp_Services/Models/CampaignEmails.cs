using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technova_Mailchimp_Services.Models
{
    public class CampaignEmails
    {
        public Guid? DistributeID { get; set; }

        public string McEmailID { get; set; }

        public string McCampaignID { get; set; }

        public string McListID { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public string ToEmail { get; set; }
    }

    public class CampaignEmailActivities
    {
        public Guid EmailID { get; set; }

        public string Action { get; set; }

        public DateTime? Timestamp { get; set; }

        public string ClickUrl { get; set; }

        public string Ip { get; set; }
    }
}
