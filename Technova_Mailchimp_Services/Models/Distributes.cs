using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technova_Mailchimp_Services.Models
{
    public class Distributes
    {
        public Guid DistID { get; set; }

        public string ListID { get; set; }

        public string Subject { get; set; }

        public Guid TemplateID { get; set; }

        public string TemplateContent { get; set; }

        public DateTime? SendTime { get; set; }
    }
}
