using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technova_Mailchimp_Services.Models
{
    public class Campaigns
    {
        #region -- tbl_Mc_Campaigns --

        public Guid DistributeID { get; set; }

        public Guid TemplateID { get; set; }

        public string McID { get; set; }

        public int? WebID { get; set; }

        public string ParentID { get; set; }

        public string Type { get; set; }

        public DateTime? CreateTime { get; set; }

        public string Status { get; set; }

        public int? EmailsSent { get; set; }

        public DateTime? SendTime { get; set; }

        public string ContentType { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignSettings --

        public string SubjectLine { get; set; }

        public string PreviewText { get; set; }

        public string Title { get; set; }

        public string FromName { get; set; }

        public string ReplyTo { get; set; }

        public bool? UseConversation { get; set; }

        public string ToName { get; set; }

        #endregion

        #region -- tbl_Mc_CampaignRecipients --

        public string ListID { get; set; }

        public bool? ListIsActive { get; set; }

        public string ListName { get; set; }

        public int? RecipientCount { get; set; }

        #endregion
    }
}
