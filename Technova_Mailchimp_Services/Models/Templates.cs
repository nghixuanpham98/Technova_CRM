using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technova_Mailchimp_Services.Models
{
    public class Templates
    {
        public string Slug { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Subject { get; set; }

        public string FromEmail { get; set; }

        public string FromName { get; set; }

        public string Text { get; set; }

        public string PublishName { get; set; }

        public string PublishCode { get; set; }

        public string PublishSubject { get; set; }

        public string PublishFromEmail { get; set; }

        public string PublishFromName { get; set; }

        public string PublishText { get; set; }

        public DateTime? PublishedAt { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool? IsBrokenTemplate { get; set; }
    }
}
