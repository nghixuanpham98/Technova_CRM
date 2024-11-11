using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Technova_CRM.Models.EntityModels;

namespace Technova_CRM.Models.CustomModels
{
    public class ZaloParams
    {
        public class user_info
        {
            public Guid? user_id{ get; set; }
            public string user_name { get; set; }
            public bool status { get; set; } = false;

        }
        public class zalo_input
        {
            public string tableName { get; set; }
            public string tableRows { get; set; }
            public string user_id { get; set; }
            public string last_message_id { get; set; }
            public string react_message_id { get; set; }
            public string attachment_id { get; set; }
            public string message_id { get; set; }
            public string react_icon { get; set; }
            public string text { get; set; }
            public string phone { get; set; }
            public string tag_name { get; set; }
            public string token { get; set; }
            public string refresh_token { get; set; }
            public string access_token { get; set; }
            public List<string> list_user_id { get; set; }
            public HttpPostedFile file { get; set; }

            //for send request share info
            public string title { get; set; }
            public string subtitle { get; set; }
            public string image_url { get; set; }
            public int? template_id { get; set; }
            public List<tbl_ZaloTemplateParamLink> ListParamLink { get; set; }
            public List<tbl_ZaloTemplateParams> ListParams { get; set; }

             
            public template_data template { get; set; }
            public class template_data
            {
                public string key { get; set; }
                public string value { get; set; }
            }

        }

        public class zalo_output
        {
            public class token
            {
                public string access_token { get; set; }
                public string refresh_token { get; set; }
                public int expires_in { get; set; }
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
            public class send_zns
            {
                public int error { get; set; }
                public string message { get; set; }
                public data_detail data { get; set; }
                public class data_detail
                {
                    public string sent_time { get; set; }
                    public string sending_mode { get; set; }
                    public quota.data_detail quota { get; set; }
                    public string msg_id { get; set; }
                }
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
            public class tag_list
            {
                public int error { get; set; }
                public string message { get; set; }
                public List<string> data { get; set; }
            }

            public class message_by_user_id_by_app
            {
                public string user_id { get; set; }
                public conversation data { get; set; }
            }
            public class conversation
            {
                public int error { get; set; }
                public string message { get; set; }
                public List<data_detail> data { get; set; }
                public class data_detail
                {
                    public int? src { get; set; }
                    public long? time { get; set; }
                    public string sent_time { get; set; }
                    public string from_id { get; set; }
                    public string from_display_name { get; set; }
                    public string from_avatar { get; set; }
                    public string to_id { get; set; }
                    public string to_display_name { get; set; }
                    public string to_avatar { get; set; }
                    public string message_id { get; set; }
                    public string type { get; set; }
                    public string message { get; set; }
                }
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


            public class sticker
            {
                public string id { get; set; }
                public string src { get; set; }
            }
            public class upload
            {
                public int error { get; set; }
                public string message { get; set; }
                public data_detail data { get; set; }
                public class data_detail
                {
                    public string token { get; set; }
                    public string attachment_id { get; set; }
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
                }

                public class tags_and_notes_info
                {
                    public string[] notes { get; set; }
                    public string[] tag_names { get; set; }
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

        }

    }
}