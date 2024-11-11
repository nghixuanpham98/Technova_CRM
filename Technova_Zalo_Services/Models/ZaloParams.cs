using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technova_Zalo_Services.Models
{
    public class ZaloParams
    {
        public class zalo_url
        {
            public static string mainUrl = "https://openapi.zalo.me/v2.0/oa/";
            public static string getoa = mainUrl + "getoa";
            public static string listrecentchat = mainUrl + "listrecentchat?";
            public static string conversation = mainUrl + "conversation?";
            public static string message = mainUrl + "message";
            public static string tags = mainUrl + "tag/gettagsofoa";
        }
        public class zalo_output
        {
            public class token
            {
                public string access_token { get; set; }
                public string refresh_token { get; set; }
                public int expires_in { get; set; }
            }
            public class oa_info
            {
                public int error { get; set; }
                public string message { get; set; }
                public data_detail data { get; set; }
                public class data_detail
                {
                    public string oa_id { get; set; }
                    public string name { get; set; }
                    public string description { get; set; }
                    public bool is_verified { get; set; } = false;
                    public int oa_type { get; set; } = 0;
                    public string cate_name { get; set; }
                    public int num_follower { get; set; } = 0;
                    public string avatar { get; set; }
                    public string cover { get; set; }
                    public string package_name { get; set; }
                    public string package_valid_through_date { get; set; }
                    public string package_auto_renew_date { get; set; }
                    public string linked_zca { get; set; }
                }
            }
            public class quota
            {
                public int error { get; set; }
                public string message { get; set; }
                public data_detail data { get; set; }
                public class data_detail
                {
                    public int? remainingQuotaPromotion { get; set; }
                    public int? remainingQuota { get; set; }
                    public int? dailyQuotaPromotion { get; set; }
                    public int? dailyQuota { get; set; }
                }
            }
            public class user_detail
            {
                public int error { get; set; }
                public string message { get; set; }
                public data_detail data { get; set; }
                public class data_detail
                {
                    public string user_id { get; set; }
                    public string user_id_by_app { get; set; }
                    public string display_name { get; set; }
                    public string user_alias { get; set; }
                    public bool? is_sensitive { get; set; }
                    public string user_last_interaction_date { get; set; }
                    public bool? user_is_follower { get; set; }
                    public string avatar { get; set; }
                    public tags_and_notes_info tags_and_notes_info { get; set; }
                    public shared_info shared_info { get; set; }
                }

                public class tags_and_notes_info
                {
                    public string[] notes { get; set; }
                    public string[] tag_names { get; set; }
                }
                public class shared_info
                {
                    public string address { get; set; }
                    public string city { get; set; }
                    public string district { get; set; }
                    public string phone { get; set; }
                    public string name { get; set; }
                }
            }
            public class user_list
            {
                public int error { get; set; }
                public string message { get; set; }
                public data_detail data { get; set; }
                public class data_detail
                {
                    public long total { get; set; }
                    public long count { get; set; }
                    public long offset { get; set; }
                    public List<users> users { get; set; }
                }
                public class users
                {
                    public string user_id { get; set; }
                }
            }

            public class webhook_event_message
            {
                public string msg_id { get; set; }
            }
            public class webhook_event_user_submit_info
            { 
                public string address { get; set; }
                public string phone { get; set; }
                public string city { get; set; }
                public string district { get; set; }
                public string name { get; set; }
            }
            public class webhook_event
            {
                public string oa_id { get; set; }
                public string app_id { get; set; }
                public string user_id_by_app { get; set; }
                public string event_name { get; set; }
                public string timestamp { get; set; }
                public string event_time { get; set; }
                public string source { get; set; }
                public object message { get; set; }
                public object info { get; set; }
                public string message_string { get; set; }
                public sender_data sender { get; set; }
                public follower_data follower { get; set; }
                public recipient_data recipient { get; set; }
                public class sender_data { public string id { get; set; } }
                public class recipient_data { public string id { get; set; } }
                public class follower_data { public string id { get; set; } }


            }

            public class template
            {
                public int error { get; set; }
                public string message { get; set; }
                public List<data_detail> data { get; set; }
                public class data_detail
                {
                    public int templateId { get; set; }
                    public string templateName { get; set; }
                    public int? createdTime { get; set; }
                    public string status { get; set; }
                    public string templateQuality { get; set; }
                }
            }

            public class template_detail
            {
                public int error { get; set; }
                public string message { get; set; }
                public data_detail data { get; set; }
                public class data_detail
                {
                    public int templateId { get; set; }
                    public string templateName { get; set; }
                    public string status { get; set; }
                    public List<param_detail> listParams { get; set; }
                    public long timeout { get; set; }
                    public string previewUrl { get; set; }
                    public string templateQuality { get; set; }
                    public string templateTag { get; set; }
                    public float price { get; set; }
                    public bool applyTemplateQuota { get; set; }

                }
                public class param_detail
                {
                    public string name { get; set; }
                    public bool require { get; set; }
                    public string type { get; set; }
                    public int maxLength { get; set; }
                    public int minLength { get; set; }
                    public bool acceptNull { get; set; }
                }
            }

        }

    }
}
