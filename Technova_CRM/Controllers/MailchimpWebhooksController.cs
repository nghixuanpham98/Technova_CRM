using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using Technova_CRM.DAO;

namespace Technova_CRM.Controllers
{
    public class MailchimpWebhooksController : ApiController
    {
        #region -- Configuration --

        private class WebhooksCommon
        {
            public static Dictionary<string, string> ParseUrlEncodedString(string encodedString)
            {
                string[] keyValuePairs = encodedString.Split('&');
                Dictionary<string, string> data = new Dictionary<string, string>();

                foreach (string pair in keyValuePairs)
                {
                    string[] parts = pair.Split('=');

                    if (parts.Length == 2)
                    {
                        string key = Uri.UnescapeDataString(parts[0]);
                        string value = Uri.UnescapeDataString(parts[1]);
                        data[key] = value;
                    }
                }

                return data;
            }

            public static JObject HandleSubscribe(Dictionary<string, string> data)
            {
                try
                {
                    string type, id, list_id, email, email_type, ip_opt, ip_signup, merges_email, merges_fname, merges_lname;

                    // Fetching the values from the dictionary
                    data.TryGetValue("type", out type);
                    data.TryGetValue("data[id]", out id);
                    data.TryGetValue("data[list_id]", out list_id);
                    data.TryGetValue("data[email]", out email);
                    data.TryGetValue("data[email_type]", out email_type);
                    data.TryGetValue("data[ip_opt]", out ip_opt);
                    data.TryGetValue("data[ip_signup]", out ip_signup);
                    data.TryGetValue("data[merges][EMAIL]", out merges_email);
                    data.TryGetValue("data[merges][FNAME]", out merges_fname);
                    data.TryGetValue("data[merges][LNAME]", out merges_lname);

                    JObject jsonObject = new JObject(
                        new JProperty("type", type), // This is where 'type' is being used
                        new JProperty("fired_at", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")),
                        new JProperty("data",
                            new JObject(
                                new JProperty("id", id),
                                new JProperty("list_id", list_id),
                                new JProperty("email", email),
                                new JProperty("email_type", email_type),
                                new JProperty("ip_opt", ip_opt),
                                new JProperty("ip_signup", ip_signup),
                                new JProperty("merges",
                                    new JObject(
                                        new JProperty("EMAIL", merges_email),
                                        new JProperty("FNAME", merges_fname),
                                        new JProperty("LNAME", merges_lname),
                                        new JProperty("INTERESTS", "")
                                    ))
                            ))
                    );

                    return jsonObject;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        private class WebhookContent
        {
            public string EventName { get; set; }
            public string Timestamp { get; set; }
            public string RawContent { get; set; }
        }

        #endregion

        #region -- Method --

        #region -- Webhooks mailchimp --

        [HttpGet]
        [HttpPost]
        [Route("api/mailchimp/webhooks")]
        public async Task<HttpResponseMessage> GetWebhooksMailchimp()
        {
            try
            {
                if (Request.Method == HttpMethod.Get)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    var content = await Request.Content.ReadAsStringAsync();

                    Dictionary<string, string> data = WebhooksCommon.ParseUrlEncodedString(content);

                    data.TryGetValue("type", out string type);

                    if (type == "campaign")
                    {
                        data.TryGetValue("data[id]", out string id);
                        data.TryGetValue("data[status]", out string status);

                        string updateQuery = @"
                            UPDATE [tbl_Mc_Campaigns] 
                            SET Status = @Status, SendTime = @SendTime, ModifiedOn = @ModifiedOn
                            WHERE McID = @McID";

                        List<SqlParameter> updateParams = new List<SqlParameter>
                        {
                            new SqlParameter("@McID", id),
                            new SqlParameter("@Status", status),
                            new SqlParameter("@SendTime", DateTime.Now),
                            new SqlParameter("@ModifiedOn", DateTime.Now)
                        };

                        // Execute the update query asynchronously
                        await DAO_Common.ExecuteNonQueryAsync(updateQuery, updateParams);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        #endregion

        #region -- Webhooks mandrill --

        [HttpGet]
        [HttpPost]
        [Route("api/mandrill/webhooks")]
        public async Task<HttpResponseMessage> GetWebhooksMandrillAsync()
        {
            try
            {
                if (Request.Method == HttpMethod.Get)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    //var signature = Request.Headers.GetValues("X-Mandrill-Signature").FirstOrDefault();
                    var content = await Request.Content.ReadAsStringAsync();

                    Dictionary<string, string> data = WebhooksCommon.ParseUrlEncodedString(content);

                    string mandrill_events;
                    data.TryGetValue("mandrill_events", out mandrill_events);

                    var arr = (JArray)JsonConvert.DeserializeObject(mandrill_events);

                    if (arr != null && arr.Count > 0)
                    {
                        foreach (var item in arr)
                        {
                            var rsEvent = "";
                            var eventName = item["event"]?.Value<string>();

                            if (eventName != null && !string.IsNullOrEmpty(eventName))
                            {
                                rsEvent = eventName;
                            }
                            else
                            {
                                var type = item["type"]?.Value<string>();
                                var action = item["action"]?.Value<string>();

                                rsEvent = "allowlist_" + type + "_" + action;
                            }

                            var timestamp = item["ts"]?.Value<string>();

                            WebhookContent webhook = new WebhookContent();

                            webhook.EventName = rsEvent;
                            webhook.Timestamp = timestamp;
                            webhook.RawContent = item.ToString();

                            await CreateNewWebhookToDBAsync(webhook);
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        #endregion

        #region -- Handle --

        private async Task CreateNewWebhookToDBAsync(WebhookContent content)
        {
            try
            {
                // Insert query for creating a new template in the database
                var newID = Guid.NewGuid();

                string insertQuery = @"
                    INSERT INTO tbl_Mc_Webhooks 
                    (ID, EventName, EventFrom, RawContent, Status, Timestamp, CreatedOn, ModifiedOn) 
                    VALUES 
                    (@ID, @EventName, @EventFrom, @RawContent, @Status, @Timestamp, @CreatedOn, @ModifiedOn)";

                List<SqlParameter> insertParams = new List<SqlParameter>
                {
                    new SqlParameter("@ID", newID),
                    new SqlParameter("@EventName", content.EventName ?? (object)DBNull.Value),
                    new SqlParameter("@EventFrom", "Mandrill"),
                    new SqlParameter("@RawContent", content.RawContent ?? (object)DBNull.Value),
                    new SqlParameter("@Status", "0"),
                    new SqlParameter("@Timestamp", content.Timestamp ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedOn", DateTime.Now),
                    new SqlParameter("@ModifiedOn", DateTime.Now)
                };

                // Execute the insert query asynchronously
                await DAO_Common.ExecuteNonQueryAsync(insertQuery, insertParams);
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #endregion
    }
}
