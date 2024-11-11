using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technova_Mailchimp_Services.Models
{
    public class Reports
    {
        public Guid? DistID { get; set; }

        public Guid? CampaignID { get; set; }

        public Guid? BounceID { get; set; }

        public int? HardBounces { get; set; }

        public int? SoftBounces { get; set; }

        public int? SyntaxErrors { get; set; }

        public Guid? ForwardID { get; set; }

        public int? ForwardsCount { get; set; }

        public int? ForwardsOpens { get; set; }

        public Guid? OpenID { get; set; }

        public int? OpensTotal { get; set; }

        public int? UniqueOpens { get; set; }

        public decimal? OpenRate { get; set; }

        public DateTime? LastOpen { get; set; }

        public Guid? ClickID { get; set; }

        public int? ClicksTotal { get; set; }

        public int? UniqueClicks { get; set; }

        public int? UniqueSubscriberClicks { get; set; }

        public decimal? ClickRate { get; set; }

        public DateTime? LastClick { get; set; }
    }
}
