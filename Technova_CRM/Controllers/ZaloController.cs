using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using Technova_CRM.Models.EntityModels;
using Technova_CRM.Models.CustomModels;
using System.Data;
using RestSharp;
using System.Xml;
using System.ComponentModel.DataAnnotations;

namespace Technova_CRM.Controllers
{
    public class ZaloController : ApiController
    {
        DBContext db = new DBContext();
        public class zalo_url
        {
            public static string mainUrl = "https://openapi.zalo.me/v2.0/oa/";
            public static string getoa = mainUrl + "getoa";
            public static string listrecentchat = mainUrl + "listrecentchat?";
            public static string conversation = mainUrl + "conversation?";
            public static string message = mainUrl + "message";
            public static string tags = mainUrl + "tag/gettagsofoa";
        }

        public class zalo_api
        {
            public static object method_get(string url, ref string error)
            {

                try
                {
                    DBContext db = new DBContext();
                    string access_token = db.tbl_ZaloSettings.FirstOrDefault(x => x.Text == "access_token")?.Value;
                    var req = (HttpWebRequest)WebRequest.Create(url);
                    //req.Headers.Set("access_token", File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "zalo_access_token.txt")));
                    req.Headers.Set("access_token", access_token);
                    req.Method = "GET";

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11
                                                  | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    string output = "";
                    using (var res = req.GetResponse())
                    {
                        using (var reader = new StreamReader(res.GetResponseStream()))
                        {
                            output = reader.ReadToEnd();
                        }
                    }
                    if (!string.IsNullOrEmpty(output))
                    {
                        var result = JsonConvert.DeserializeObject<object>(output);

                        if (result != null)
                        {
                            WriteLog(new { type = "info", function = "call zalo api (get)", data = result });
                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    WriteLog(new { type = "error", function = "call zalo api (get)", data = ex });
                }
                return null;
            }

            public static object method_post(string url, object obj, ref string error)
            {
                try
                {
                    DBContext db = new DBContext();
                    string access_token = db.tbl_ZaloSettings.FirstOrDefault(x => x.Text == "access_token")?.Value;
                    string input = JsonConvert.SerializeObject(obj);
                    var req = (HttpWebRequest)WebRequest.Create(url);
                    //req.Headers.Set("access_token", File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "zalo_access_token.txt")));
                    req.Headers.Set("access_token", access_token);
                    req.Method = "POST";
                    req.ContentType = "application/json";

                    if (!string.IsNullOrEmpty(input))
                    {
                        using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                        {
                            streamWriter.Write(input);
                        }
                    }

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11
                                                  | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                    string output = "";
                    using (var res = req.GetResponse())
                    {
                        using (var reader = new StreamReader(res.GetResponseStream()))
                        {
                            output = reader.ReadToEnd();
                        }
                    }
                    if (!string.IsNullOrEmpty(output))
                    {
                        var result = JsonConvert.DeserializeObject<object>(output);

                        if (result != null)
                        {
                            WriteLog(new { type = "info", function = "call zalo api (post)", data = result });
                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    WriteLog(new { type = "error", function = "call zalo api (post)", data = ex });
                }
                return null;
            }

            public static ZaloParams.zalo_output.upload method_upload(HttpPostedFile file_upload, ref string error, bool isImage = false)
            {
                try
                {
                    DBContext db = new DBContext();
                    string access_token = db.tbl_ZaloSettings.FirstOrDefault(x => x.Text == "access_token")?.Value;

                    string folder = HttpContext.Current.Server.MapPath("~/File");

                    if (!System.IO.Directory.Exists(folder)) System.IO.Directory.CreateDirectory(folder);

                    var file_url = Path.Combine(folder, Guid.NewGuid() + file_upload.FileName);

                    if (File.Exists(file_url)) File.Delete(file_url);

                    file_upload.SaveAs(file_url);

                    byte[] file_base = File.ReadAllBytes(file_url);

                    System.IO.File.WriteAllBytes(file_url, file_base);

                    string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                    byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                    var wrCreate = isImage == true
                        ? "https://openapi.zalo.me/v2.0/oa/upload/image" /*upload ảnh /gif output giống image*/
                        : "https://openapi.zalo.me/v2.0/oa/upload/file";

                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(wrCreate);

                    req.Headers.Set("access_token", access_token);
                    req.ContentType = "multipart/form-data; boundary=" + boundary;
                    req.Method = "POST";

                    Stream rs = req.GetRequestStream();

                    rs.Write(boundarybytes, 0, boundarybytes.Length);

                    string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                    string header = string.Format(headerTemplate, "file", file_url, file_upload.ContentType);
                    byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                    rs.Write(headerbytes, 0, headerbytes.Length);

                    FileStream fileStream = new FileStream(file_url, FileMode.Open, FileAccess.Read);
                    byte[] buffer = new byte[4096];
                    int bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        rs.Write(buffer, 0, bytesRead);
                    }
                    fileStream.Close();

                    byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                    rs.Write(trailer, 0, trailer.Length);
                    rs.Close();

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11
                                                   | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                    string output = "";
                    using (var res = req.GetResponse())
                    {
                        using (var reader = new StreamReader(res.GetResponseStream()))
                        {
                            output = reader.ReadToEnd();
                        }
                    }
                    if (!string.IsNullOrEmpty(output))
                    {
                        var result = JsonConvert.DeserializeObject<ZaloParams.zalo_output.upload>(output);

                        if (result != null)
                        {
                            WriteLog(result);

                            if (File.Exists(file_url)) File.Delete(file_url);

                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
                return null;
            }
        }

        public static void WriteLog(object Message, bool isException = false)
        {
            try
            {
                string folderLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogZalo");

                if (!Directory.Exists(folderLog)) Directory.CreateDirectory(folderLog);

                string path = Path.Combine(folderLog, DateTime.Now.ToString("yyyy_MM_dd") + ".txt");

                if (isException) path = Path.Combine(folderLog, DateTime.Now.ToString("yyyy_MM_dd") + "_exception.txt");

                if (!System.IO.File.Exists(path)) System.IO.File.Create(path).Close();

                TextWriter tw = new StreamWriter(path, true);

                tw.WriteLine($"\n{DateTime.Now.ToString("HH:mm:ss")}\t{JsonConvert.SerializeObject(Message)}");

                tw.Close();
            }
            catch { }
        }


        #region Webhook 

        [HttpPost]
        [Route("api/zalo/webhook")]
        public IHttpActionResult webhook()
        {
            try
            {
                var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
                bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                var bodyText = bodyStream.ReadToEnd();

                var output = JsonConvert.DeserializeObject<ZaloParams.zalo_output.webhook_event>(bodyText);

                string connectionString = ConfigurationManager.ConnectionStrings["DBContext"].ConnectionString;
                SqlConnection connection = new SqlConnection(@connectionString);
                string query = $"INSERT INTO tbl_ZaloWebhook(ID,EventName, EventTime, Content, Status, CreatedOn)";
                query += $" VALUES(NEWID(),'{output.event_name}','{output.timestamp}',N'{bodyText}',0,GETDATE())";

                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                }
                finally
                {
                    connection.Close();
                }

                return Json(new { code = 200 });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/zalo/oa_info")]
        public IHttpActionResult oa_info()
        {
            try
            {
                //get_quota_info();
                var data = db.tbl_ZaloOA.FirstOrDefault();
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        void insert_user_detail(List<string> liUserId)
        {
            string function = "get_user_detail_by_id";
            try
            {
                var liOutput = new List<ZaloParams.zalo_output.user_detail.data_detail>();

                if (liUserId.Count > 0)
                {
                    for (int i = 0; i < liUserId.Count; i++)
                    {
                        string error = "";

                        string url = "https://openapi.zalo.me/v3.0/oa/user/detail?data={\"user_id\":\"" + liUserId[i] + "\"}";

                        var obj2 = zalo_api.method_get(url, ref error);

                        var str2 = JsonConvert.SerializeObject(obj2);

                        liOutput.Add(JsonConvert.DeserializeObject<ZaloParams.zalo_output.user_detail>(str2).data);

                        if (!string.IsNullOrEmpty(error)) WriteLog(new { type = "error", function, data = error });
                    }

                    if (liOutput != null && liOutput.Count() > 0)
                    {
                        foreach (var item in liOutput)
                        {
                            var isAdd = false;
                            var userInfo = db.tbl_ZaloUsers.FirstOrDefault(x => x.UserId == item.user_id);
                            if (userInfo == null)
                            {
                                isAdd = true;
                                userInfo = new tbl_ZaloUsers();
                                userInfo.ID = Guid.NewGuid();
                                userInfo.UserId = item.user_id;
                                userInfo.UserIdByApp = item.user_id_by_app;
                                userInfo.CreatedOn = DateTime.Now;
                            }

                            userInfo.Avatar = item.avatar;
                            userInfo.DisplayName = item.display_name;
                            userInfo.Alias = item.user_alias;
                            userInfo.IsSensitive = item.is_sensitive;
                            userInfo.LastInteractionDate = item.user_last_interaction_date;
                            userInfo.IsFollower = item.user_is_follower;
                            userInfo.ModifiedOn = DateTime.Now;

                            string notes = "", tag_names = "";
                            if (item.tags_and_notes_info != null)
                            {
                                if (item.tags_and_notes_info.notes != null && item.tags_and_notes_info.notes.Count() > 0)
                                {
                                    notes = string.Join(", ", item.tags_and_notes_info.notes);
                                }
                                if (item.tags_and_notes_info.tag_names != null && item.tags_and_notes_info.tag_names.Count() > 0)
                                {
                                    tag_names = string.Join(", ", item.tags_and_notes_info.tag_names);
                                }
                            }
                            userInfo.Notes = notes;
                            userInfo.Tags = tag_names;

                            if (isAdd) db.tbl_ZaloUsers.Add(userInfo);

                            db.SaveChanges();

                            WriteLog(new { type = "info", function, data = userInfo });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(new { type = "error", function, data = ex });
            }
        }

        void get_user_list()
        {
            try
            {
                string url = "https://openapi.zalo.me/v3.0/oa/user/getlist?data={\"offset\":0,\"count\":50}";
                string url1 = "https://openapi.zalo.me/v3.0/oa/user/getlist?data={\"offset\":0,\"count\":50,\"is_follower\":\"true\"}";

                string error = "";

                var obj = zalo_api.method_get(url, ref error);

                var liUserId = new List<string>();
                var liDetail = new List<ZaloParams.zalo_output.user_detail>();
                var liOutput = new List<ZaloParams.zalo_output.user_detail.data_detail>();

                if (obj != null)
                {
                    var str = JsonConvert.SerializeObject(obj);
                    if (!string.IsNullOrEmpty(str))
                    {
                        var rs = JsonConvert.DeserializeObject<ZaloParams.zalo_output.user_list>(str);

                        if (rs != null && rs.data != null && rs.data.users.Count > 0)
                        {
                            for (int i = 0; i < rs.data.users.Count; i++)
                            {
                                liUserId.Add(rs.data.users[i].user_id);
                            }
                        }
                    }

                    if (liUserId.Count > 0)
                    {
                        for (int i = 0; i < liUserId.Count; i++)
                        {
                            url = "https://openapi.zalo.me/v3.0/oa/user/detail?data={\"user_id\":\"" + liUserId[i] + "\"}";

                            var obj2 = zalo_api.method_get(url, ref error);

                            var str2 = JsonConvert.SerializeObject(obj2);

                            liOutput.Add(JsonConvert.DeserializeObject<ZaloParams.zalo_output.user_detail>(str2).data);
                        }
                    }
                }

                if (liOutput != null && liOutput.Count() > 0)
                {
                    foreach (var item in liOutput)
                    {
                        var isAdd = false;
                        var userInfo = db.tbl_ZaloUsers.FirstOrDefault(x => x.UserId == item.user_id);
                        if (userInfo == null)
                        {
                            isAdd = true;
                            userInfo = new tbl_ZaloUsers();
                            userInfo.ID = Guid.NewGuid();
                            userInfo.UserId = item.user_id;
                            userInfo.UserIdByApp = item.user_id_by_app;
                            userInfo.CreatedOn = DateTime.Now;
                        }

                        userInfo.Avatar = item.avatar;
                        userInfo.DisplayName = item.display_name;
                        userInfo.Alias = item.user_alias;
                        userInfo.IsSensitive = item.is_sensitive;
                        userInfo.LastInteractionDate = item.user_last_interaction_date;
                        userInfo.IsFollower = item.user_is_follower;
                        userInfo.ModifiedOn = DateTime.Now;

                        string notes = "", tag_names = "";
                        if (item.tags_and_notes_info != null)
                        {
                            if (item.tags_and_notes_info.notes != null && item.tags_and_notes_info.notes.Count() > 0)
                            {
                                notes = string.Join(", ", item.tags_and_notes_info.notes);
                            }
                            if (item.tags_and_notes_info.tag_names != null && item.tags_and_notes_info.tag_names.Count() > 0)
                            {
                                tag_names = string.Join(", ", item.tags_and_notes_info.tag_names);
                            }
                        }
                        userInfo.Notes = notes;
                        userInfo.Tags = tag_names;

                        if (isAdd) db.tbl_ZaloUsers.Add(userInfo);

                        db.SaveChanges();
                    }

                }
            }
            catch (Exception ex)
            {
            }
        }

        void get_quota_info()
        {
            string function = "get_quota_info";
            try
            {
                string error = "";

                var obj = zalo_api.method_get("https://business.openapi.zalo.me/message/quota", ref error);

                if (obj != null)
                {
                    var str = JsonConvert.SerializeObject(obj);
                    if (!string.IsNullOrEmpty(str))
                    {
                        var quotaInfo = JsonConvert.DeserializeObject<ZaloParams.zalo_output.quota>(str);

                        if (quotaInfo != null && quotaInfo.data != null)
                        {
                            var oaInfo = db.tbl_ZaloOA.FirstOrDefault();
                            if (oaInfo != null)
                            {
                                oaInfo.RemainingQuotaPromotion = quotaInfo.data.remainingQuotaPromotion;
                                oaInfo.RemainingQuota = quotaInfo.data.remainingQuota;
                                oaInfo.DailyQuotaPromotion = quotaInfo.data.dailyQuotaPromotion;
                                oaInfo.DailyQuota = quotaInfo.data.dailyQuota;
                                db.SaveChanges();
                                WriteLog(new { type = "info", function, quotaInfo.data });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(new { type = "error", function, data = ex });
            }
        }

        void get_access_token()
        {
            try
            {
                var settings = db.tbl_ZaloSettings.ToList();
                var app_id = settings.FirstOrDefault(x => x.Text == "app_id")?.Value;
                var secret_key = settings.FirstOrDefault(x => x.Text == "secret_key")?.Value;
                var refresh_token = settings.FirstOrDefault(x => x.Text == "refresh_token")?.Value;

                var client = new RestClient("https://oauth.zaloapp.com/v4/oa/access_token");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("secret_key", secret_key);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                request.AddParameter("app_id", app_id);
                request.AddParameter("grant_type", "refresh_token");
                request.AddParameter("refresh_token", refresh_token);

                IRestResponse response = client.Execute(request);
                if (!string.IsNullOrEmpty(response.Content))
                {
                    var tkn = JsonConvert.DeserializeObject<ZaloParams.zalo_output.token>(response.Content);
                    if (tkn != null && !string.IsNullOrEmpty(tkn.access_token))
                    {
                        foreach (var item in settings)
                        {
                            if (item.Text == "access_token") item.Value = tkn.access_token;
                            if (item.Text == "refresh_token") item.Value = tkn.refresh_token;
                            if (item.Text == "expires_in") item.Value = DateTime.Now.AddSeconds(tkn.expires_in).ToString("yyyy-MM-dd HH:mm:ss");
                            item.ModifiedOn = DateTime.Now;
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        void get_all_message_by_user_id()
        {
            try
            {
                //get message detail by user id 

                var liUserId = db.tbl_ZaloUsers.Select(x => x.UserIdByApp).ToList();

                if (liUserId != null && liUserId.Count() > 0)
                {
                    for (int i = 0; i < liUserId.Count(); i++)
                    {
                        var user_id = liUserId[i];

                        string error = "";

                        string data = "data={\"offset\":0,\"user_id\":" + user_id + ",\"count\":10}";

                        var obj = zalo_api.method_get(zalo_url.conversation + data, ref error);

                        var result = JsonConvert.DeserializeObject<ZaloParams.zalo_output.conversation>(obj.ToString());

                        var liMsg = new List<tbl_ZaloMessages>();

                        if (result != null && result.data != null && result.data.Count() > 0)
                        {
                            if (result.data.Count() == 10)
                            {
                                for (int j = 0; j <= 5; j++)
                                {
                                    var offset = j * 10;

                                    data = "data={\"offset\":" + offset + ",\"user_id\":" + user_id + ",\"count\":10}";

                                    obj = zalo_api.method_get(zalo_url.conversation + data, ref error);

                                    result = JsonConvert.DeserializeObject<ZaloParams.zalo_output.conversation>(obj.ToString());

                                    foreach (var item in result.data)
                                    {
                                        var time = ConvertNumberToDateTime(item.time.Value);
                                        string _mess = JsonConvert.SerializeObject(item);
                                        liMsg.Add(new tbl_ZaloMessages
                                        {
                                            ID = Guid.NewGuid(),
                                            MsgId = item.message_id,
                                            Type = item.type,
                                            FromId = item.from_id,
                                            ToId = item.to_id,
                                            MsgTime = time,
                                            Message = _mess
                                        });
                                        WriteLog(new { type = "info", function = $"get_all_message_by_user_id", data = new { user_id, item.message_id } });
                                    }
                                }
                            }
                            else
                            {
                                foreach (var item in result.data)
                                {
                                    var time = ConvertNumberToDateTime(item.time.Value);
                                    string _mess = JsonConvert.SerializeObject(item);
                                    liMsg.Add(new tbl_ZaloMessages
                                    {
                                        ID = Guid.NewGuid(),
                                        MsgId = item.message_id,
                                        Type = item.type,
                                        FromId = item.from_id,
                                        ToId = item.to_id,
                                        MsgTime = time,
                                        Message = _mess
                                    });
                                    WriteLog(new { type = "info", function = $"get_all_message_by_user_id", data = new { user_id, item.message_id } });
                                }
                            }
                        }

                        if (liMsg.Count > 0) db.tbl_ZaloMessages.AddRange(liMsg);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(new { type = "error", data = ex });
            }
        }

        [HttpGet]
        [Route("api/zalo/for_1st_run")]
        public IHttpActionResult for_1st_run()
        {
            try
            {
                //get_access_token();

                //get_user_list();

                get_all_message_by_user_id();

                return Json(new { });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }

        public static string CreateSession = "select lower(replace(convert(nvarchar(max), newid()),'-',''))";
        public static DateTime ConvertNumberToDateTime(long input)
        {
            try
            {
                long beginTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
                var time = new DateTime(beginTicks + input * 10000, DateTimeKind.Utc);
                time = time.AddHours(7);
                return time;
            }
            catch { return new DateTime(); }
        }

        [HttpGet]
        [Route("api/zalo/webhook_scan_chat")]
        public IHttpActionResult webhook_scan_chat()
        {
            try
            {
                db = new DBContext();

                var eventNameList = new List<string> {

                    "user_send_location" /* 1 Sự kiện người dùng gửi vị trí*/
                    , "user_send_image" /*  2 Sự kiện người dùng gửi tin nhắn hình ảnh*/
                    , "user_send_link" /*   3 Sự kiện người dùng gửi tin nhắn liên kết*/
                    , "user_send_text" /*   4 Sự kiện người dùng gửi tin nhắn text*/
                    , "user_send_sticker" /*5 Sự kiện người dùng gửi tin nhắn sticker*/	
                    , "user_send_gif" /*    6 Sự kiện người dùng gửi tin nhắn gif*/
                    , "user_send_audio" /*  7 Sự kiện người dùng gửi tin nhắn voice*/
                    , "user_send_video" /*  8 Sự kiện người dùng gửi tin nhắn video*/
                    , "user_send_file" /*   9 Sự kiện người dùng gửi tin nhắn đính kèm file*/

                    , "user_send_business_card" /*  10 Sự kiện người dùng gửi danh thiếp*/
                    , "user_send_group_text" /*     11 Sự kiện người dùng gửi tin nhắn văn bản tới nhóm*/
                    , "user_send_group_image" /*    12 Sự kiện người dùng gửi tin nhắn hình ảnh tới nhóm*/
                    , "user_send_group_link" /*     13 Sự kiện người dùng gửi tin nhắn liên kết tới nhóm*/
                    , "user_send_group_audio" /*    14 Sự kiện người dùng gửi tin nhắn âm thanh tới nhóm*/
                    , "user_send_group_location" /* 15 Sự kiện người dùng gửi tin nhắn vị trí tới nhóm*/
                    , "user_send_group_video" /*    16 Sự kiện người dùng gửi tin nhắn video tới nhóm*/
                    , "user_send_group_business_card" /*17 Sự kiện người dùng gửi tin nhắn danh thiếp tới nhóm*/
                    , "user_send_group_sticker" /*  18 Sự kiện người dùng gửi tin nhắn sticker tới nhóm*/
                    , "user_send_group_gif" /*      19 Sự kiện người dùng gửi tin nhắn gif tới nhóm*/
                    , "user_send_group_file" /*     20 Sự kiện người dùng gửi tin nhắn file tới nhóm*/

                    , "oa_send_text" /*     1 Sự kiện Official Account gửi tin nhắn text */
                    , "oa_send_image" /*    2 Sự kiện Official Account gửi tin nhắn hình ảnh */
                    , "oa_send_list" /*     3 Sự kiện Official Account gửi tin nhắn tương tác */
                    , "oa_send_gif" /*      4 Sự kiện Official Account gửi tin nhắn gif  */
                    , "oa_send_file" /*     5 Sự kiện Official Account gửi tin nhắn đính kèm file*/
                    , "oa_send_sticker" /*  6 Sự kiện Official Account gửi tin nhắn sticker */
                    //, "oa_send_consent" /*  7 Sự kiện OA gửi yêu cầu thực hiện cuộc gọi đến người dùng / yêu cầu đã hết hạn*/

                    , "anonymous_send_text" /*      1 Sự kiện người dùng ẩn danh gửi tin nhắn text*/
                    , "anonymous_send_image" /*     2 Sự kiện người dùng ẩn danh gửi tin nhắn hình ảnh*/
                    , "anonymous_send_file" /*      3 Sự kiện người dùng ẩn danh gửi tin nhắn file    */
                    , "anonymous_send_sticker" /*   4 Sự kiện người dùng ẩn danh gửi tin nhắn sticker*/
                    , "oa_send_anonymous_text" /*   5 Sự kiện Official Account gửi tin nhắn text đến người dùng ẩn danh */
                    , "oa_send_anonymous_image" /*  6 Sự kiện Official Account gửi tin nhắn hình ảnh đến người dùng ẩn danh  */
                    , "oa_send_anonymous_file" /*   7 Sự kiện Official Account gửi tin nhắn file đến người dùng ẩn danh */
                    , "oa_send_anonymous_sticker" /*8 Sự kiện Official Account gửi tin nhắn sticker đến người dùng ẩn danh */
                     
                };

                string EventNameStr = string.Join("','", eventNameList);

                string query = $"SELECT TOP 10 * FROM tbl_ZaloWebhook  WITH (READUNCOMMITTED) WHERE Status=0 AND [EventName] IN ('{EventNameStr}') ORDER BY EventTime ";

                var eventList = db.Database.SqlQuery<tbl_ZaloWebhook>(query).ToList();

                var liMsg = new List<tbl_ZaloMessages>();
                var liUserId = new List<string>();

                if (eventList != null && eventList.Count() > 0)
                {
                    var oa_info = db.tbl_ZaloOA.FirstOrDefault();

                    foreach (var ev in eventList)
                    {
                        //update lại status cho webhook đó
                        var evInfo = db.tbl_ZaloWebhook.FirstOrDefault(x => x.ID == ev.ID);
                        if (evInfo != null)
                        {
                            evInfo.Status = 1;
                            evInfo.ModifiedOn = DateTime.Now;
                        }

                        //lưu nội dung vào các bảng tương ứng
                        if (!string.IsNullOrEmpty(ev.Content))
                        {
                            var evEF = JsonConvert.DeserializeObject<ZaloParams.zalo_output.webhook_event>(ev.Content);

                            string userId = "", lastMsgId = "";

                            if (evEF != null)
                            {
                                //lấy từ [bảng chi tiết] để update ngược lại cho [bảng chính]
                                userId = evEF.sender.id == oa_info.OAID.ToLower() ? evEF.recipient.id : evEF.sender.id;

                                string message_string = JsonConvert.SerializeObject(evEF.message);
                                var msgObj = JsonConvert.DeserializeObject<ZaloParams.zalo_output.webhook_event_message>(message_string);
                                if (msgObj != null)
                                {
                                    lastMsgId = msgObj.msg_id;
                                }
                                var timestamp = long.Parse(evEF.timestamp);
                                var MsgTime = ConvertNumberToDateTime(timestamp);

                                var msg = new tbl_ZaloMessages();
                                msg.ID = Guid.NewGuid();
                                msg.FromId = evEF.sender.id;
                                msg.ToId = evEF.recipient.id;
                                msg.MsgId = lastMsgId;
                                msg.MsgTime = MsgTime;
                                msg.Message = message_string;
                                var evName = evEF.event_name.Split('_');
                                msg.Type = evName[evName.Count() - 1];
                                liMsg.Add(msg);

                                var userInfo = db.tbl_ZaloUsers.FirstOrDefault(x => x.UserId == userId);
                                if (userInfo != null)
                                {
                                    userInfo.LastMsgId = lastMsgId;
                                }
                            }
                        }
                    }

                    if (liMsg.Count() > 0) db.tbl_ZaloMessages.AddRange(liMsg);

                    db.SaveChanges();

                    return Json(new { msg = "Update successfully" });
                }
                return Json(new { msg = "No data to update" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("api/zalo/webhook_list")]
        public IHttpActionResult webhook_list()
        {
            try
            {
                db = new DBContext();
                var data = new DataTable();
                string query = $"SELECT TOP(50) * FROM tbl_ZaloWebhook  WITH (READUNCOMMITTED) ORDER BY CreatedOn DESC ";
                (new SqlDataAdapter(query, db.Database.Connection.ConnectionString)).Fill(data);
                if (data != null)
                {
                    var columns = get_columns_name_by_data_table(data);
                    return Json(new { data, columns });
                }
                return Json(new { error = "Please try again!" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("api/zalo/user_list")]
        public IHttpActionResult user_list()
        {
            try
            {
                db = new DBContext();
                var data = new DataTable();
                string query = $"SELECT * FROM tbl_ZaloUsers  WITH (READUNCOMMITTED) ORDER BY DisplayName ";
                (new SqlDataAdapter(query, db.Database.Connection.ConnectionString)).Fill(data);
                if (data != null)
                {
                    var columns = get_columns_name_by_data_table(data);
                    return Json(new { data, columns });
                }
                return Json(new { error = "Please try again!" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


        [HttpGet]
        [Route("api/zalo/conversation")]
        public IHttpActionResult conversation(Guid? userid = default)
        {
            try
            {
                db = new DBContext();
                var data = new DataTable();
                string query = $"SELECT * FROM v_Zalo_Conversations  WITH (READUNCOMMITTED) WHERE OwnerId='{userid}' ORDER BY msgtime desc ";
                (new SqlDataAdapter(query, db.Database.Connection.ConnectionString)).Fill(data);
                if (data != null)
                {
                    return Json(new { data });
                }
                return Json(new { error = "Please try again!" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public static void WriteLogHook(object Message)
        {
            try
            {
                string folderLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");

                if (!Directory.Exists(folderLog)) Directory.CreateDirectory(folderLog);

                string path = Path.Combine(folderLog, DateTime.Now.ToString("yyyy_MM_dd") + "_hook.txt");

                if (!System.IO.File.Exists(path)) System.IO.File.Create(path).Close();

                TextWriter tw = new StreamWriter(path, true);

                tw.WriteLine($"\n{DateTime.Now.ToString("HH:mm:ss")}\t{JsonConvert.SerializeObject(Message)}");

                tw.Close();
            }
            catch { }
        }

        #endregion



        //lệnh tạo lead khi có follow OA / thao tác này nằm ở phần services


        #region [   DONE    ]


        #region [   Offical Account    ]

        public List<string> get_columns_name_by_data_table(DataTable data)
        {

            if (data != null)
            {
                var columns = new List<string>();

                foreach (DataColumn column in data.Columns)
                {
                    columns.Add(column.ColumnName);
                }

                return columns;
            }
            return null;
        }



        [HttpPost]
        [Route("api/zalo/query_table")]
        public IHttpActionResult query_table(ZaloParams.zalo_input input)
        {
            try
            {
                if (input != null && !string.IsNullOrEmpty(input.tableName))
                {
                    db = new DBContext();
                    var data = new DataTable();
                    var query = $"SELECT TOP({input.tableRows}) * FROM {input.tableName}  WITH (READUNCOMMITTED) ";
                    if (input.tableName == "tbl_ZaloWebhook")
                        query = $"{query} ORDER BY CreatedOn DESC ";
                    if (input.tableName == "tbl_ZaloMessages")
                        query = $"{query} ORDER BY MsgTime DESC ";

                    (new SqlDataAdapter(query, db.Database.Connection.ConnectionString)).Fill(data);
                    if (data != null)
                    {
                        var columns = get_columns_name_by_data_table(data);
                        return Json(new { data, columns });
                    }
                    return Json(new { error = "Please try again!" });
                }
                return Json(new { });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Route("api/zalo/user_status")]
        public IHttpActionResult user_status(ZaloParams.user_info info)
        {
            try
            {
                if (info != null && info.user_id.HasValue)
                {
                    db = new DBContext();

                    var isAdd = false;
                    var userCRM = db.tbl_CRM_Users.FirstOrDefault(x => x.ID == info.user_id.Value);
                    string userName = userCRM != null ? userCRM.UserName : "";
                    var stt = db.tbl_ZaloCRMUserStatus.FirstOrDefault(x => x.UserName == userName);
                    if (stt == null)
                    {
                        isAdd = true;
                        stt = new tbl_ZaloCRMUserStatus();
                        stt.ID = Guid.NewGuid();
                        stt.UserName = userName;
                    }
                    if (info.status != stt.Status)
                        stt.Status = info.status;

                    stt.LastStatusOn = DateTime.Now;

                    if (isAdd) db.tbl_ZaloCRMUserStatus.Add(stt);
                    db.SaveChanges();

                    return Json(new { stt });
                }
                return Json(new { error = "Please try again!" });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }


        #endregion

        #region [   Chat    ]  
        [HttpGet]
        [Route("api/zalo/chat_list")]/*DONE|lay_danh_sach_cac_tin_nhan_gan_nhat*/
        public IHttpActionResult chat_list()
        {
            try
            {
                string data = "data={\"offset\":0,\"count\":10}";

                string error = "";

                var obj = zalo_api.method_get(zalo_url.listrecentchat + data, ref error);

                var result = JsonConvert.DeserializeObject<ZaloParams.zalo_output.conversation>(obj.ToString());

                return Json(result);
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }





        [HttpGet]
        [Route("api/zalo/chat_detail")]
        public IHttpActionResult chat_detail(string user_id)
        {
            try
            {
                //thông tin lấy lần đầu tiên
                var userInfo = db.Database.SqlQuery<tbl_ZaloUsers>($"SELECT TOP(1) * FROM tbl_ZaloUsers WHERE UserId='{user_id}' OR UserIdByApp='{user_id}'").FirstOrDefault();

                var lastMsgInfo = db.Database.SqlQuery<tbl_ZaloMessages>($"SELECT TOP(1) * FROM tbl_ZaloMessages WHERE FromId='{user_id}' OR ToId='{user_id}' ORDER BY MsgTime DESC").FirstOrDefault();

                if (lastMsgInfo != null && userInfo != null)
                {
                    userInfo.LastMsgId = lastMsgInfo.MsgId;
                    db.SaveChanges();
                }

                db = new DBContext();
                var oaInfo = db.Database.SqlQuery<tbl_ZaloOA>("SELECT TOP(1) * FROM tbl_ZaloOA").FirstOrDefault();


                var sessionList = db.v_Zalo_Sessions.Where(x => x.UserId == user_id).OrderByDescending(x => x.StartTime).ToList();

                string liSessionId = string.Join("','", sessionList.Select(x => x.ID));

                var data = db.Database.SqlQuery<tbl_ZaloMessages>($"SELECT TOP(100) * FROM tbl_ZaloMessages WHERE SessionID IN('{liSessionId}')  ORDER BY MsgTime ").ToList();
                //var data = db.Database.SqlQuery<tbl_ZaloMessages>($"SELECT TOP(100) * FROM tbl_ZaloMessages WHERE SessionID IN('{liSessionId}')  ORDER BY MsgTime DESC").ToList();

                //var data = db.Database.SqlQuery<tbl_ZaloMessages>($"SELECT TOP(100) * FROM tbl_ZaloMessages WHERE FromId='{user_id}' OR ToId='{user_id}' ORDER BY MsgTime DESC").ToList();

                userInfo = db.Database.SqlQuery<tbl_ZaloUsers>($"SELECT TOP(1) * FROM tbl_ZaloUsers WHERE UserId='{user_id}' OR UserIdByApp='{user_id}'").FirstOrDefault();

                return Json(new { data, oaInfo, userInfo, sessionList });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }


        //đang focus vào khung chat thì hứng hook dưới sql
        [HttpPost]
        [Route("api/zalo/chat_detail_recall")]
        public IHttpActionResult chat_detail_recall(ZaloParams.zalo_input input)
        {
            try
            {
                var userInfo = db.Database.SqlQuery<tbl_ZaloUsers>($"SELECT TOP(1) * FROM tbl_ZaloUsers WHERE UserId='{input.user_id}' OR UserIdByApp='{input.user_id}'").FirstOrDefault();

                if (userInfo != null && userInfo.LastMsgId != input.last_message_id)
                {
                    var lastMsgInfo = db.tbl_ZaloMessages.FirstOrDefault(x => x.MsgId == input.last_message_id);

                    if (lastMsgInfo != null)
                    {
                        //tìm những msg có thời gian lớn hơn last message
                        var newMsgInfo = db.tbl_ZaloMessages.Where(x => x.MsgTime > lastMsgInfo.MsgTime).OrderBy(x => x.Message).ToList();

                        WriteLog(new { func = "chat_detail_recall", data = newMsgInfo });

                        if (newMsgInfo != null && newMsgInfo.Count() > 0)
                        {
                            var oaInfo = db.Database.SqlQuery<tbl_ZaloOA>("SELECT TOP(1) * FROM tbl_ZaloOA").FirstOrDefault();

                            var data = db.Database.SqlQuery<tbl_ZaloMessages>($"SELECT TOP(100) * FROM tbl_ZaloMessages WHERE FromId='{input.user_id}' OR ToId='{input.user_id}' ORDER BY MsgTime DESC").ToList();

                            return Json(new { data = newMsgInfo, oaInfo, userInfo });
                        }
                    }
                }
                return Json(new { });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }


        [HttpPost]
        [Route("api/zalo/chat_text")]/*DONE|gui_tin_nhan_text|gui_tin_tu_van_dang_van_ban*/
        public IHttpActionResult chat_text(ZaloParams.zalo_input input)
        {
            try
            {
                var objInput = new
                {
                    recipient = new { input.user_id },
                    message = new { input.text }
                };

                string error = "";

                var obj = zalo_api.method_post("https://openapi.zalo.me/v3.0/oa/message/cs", objInput, ref error);

                return Json(obj);
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Route("api/zalo/chat_sticker")]/*DONE|gui_tin_tu_van_kem_sticker|gui_thong_bao_kem_sticker*/
        public IHttpActionResult chat_sticker(ZaloParams.zalo_input input)
        {
            try
            {
                var objInput = new
                {
                    recipient = new { input.user_id },
                    message = new
                    {
                        attachment = new
                        {
                            payload = new
                            {
                                elements = new[] { new { media_type = "sticker", input.attachment_id } },
                                template_type = "media"
                            },
                            type = "template"
                        }
                    }
                };

                string error = "";

                var obj = zalo_api.method_post("https://openapi.zalo.me/v3.0/oa/message/cs", objInput, ref error);

                return Json(obj);
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("api/zalo/sticker")]
        public IHttpActionResult sticker()
        {
            try
            {
                DirectoryInfo folder = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "File", "Zalo", "sticker"));

                FileInfo[] Files = folder.GetFiles(); //Getting Text files
                string str = "";
                var stickers = new List<ZaloParams.zalo_output.sticker>();
                foreach (FileInfo file in Files)
                {
                    str = str + ", " + file.Name;

                    byte[] imageArray = System.IO.File.ReadAllBytes(file.FullName);
                    string base64String = Convert.ToBase64String(imageArray);

                    stickers.Add(new ZaloParams.zalo_output.sticker { id = Path.GetFileNameWithoutExtension(file.Name), src = "data:image/png;base64," + base64String });
                }
                return Json(stickers);
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Route("api/zalo/chat_image")]/*gui_tin_nhan_text_dinh_kem_anh*/
        public IHttpActionResult chat_image(ZaloParams.zalo_input input)
        {
            try
            {
                var objInput = new
                {
                    recipient = new { input.user_id },
                    message = new
                    {
                        attachment = new
                        {
                            payload = new
                            {
                                elements = new[] { new { media_type = "image", attachment_id = "attachment_id" } },
                                template_type = "media"
                            },
                            type = "template"
                        }
                    }
                };

                string error = "";

                var obj = zalo_api.method_post("https://openapi.zalo.me/v3.0/oa/message/cs", objInput, ref error);

                return Json(obj);
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Route("api/zalo/chat_reply")]/*gui_tin_nhan_text_phan_hoi_nguoi_dung*/
        public IHttpActionResult chat_reply(ZaloParams.zalo_input input)
        {
            try
            {
                var objInput = new
                {
                    recipient = new { input.user_id },
                    message = new { input.text, input.react_message_id }
                };

                string error = "";

                var obj = zalo_api.method_post("https://openapi.zalo.me/v3.0/oa/message/cs", objInput, ref error);

                return Json(obj);

            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Route("api/zalo/chat_react")] //tha_bieu_tuong_cam_xuc_vao_tin_nhan
        public IHttpActionResult chat_react(ZaloParams.zalo_input input)
        {
            try
            {
                var objInput = new
                {
                    recipient = new { input.user_id },
                    sender_action = new { input.react_message_id, input.react_icon }
                };

                string error = "";

                var obj = zalo_api.method_post(zalo_url.message, objInput, ref error);

                return Json(obj);
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }
        #endregion

        #region [   Cuộc gọi    ]
        [HttpPost]
        [Route("api/zalo/call_send_request")]/*gui_thong_bao_theo_mau_yeu_cau_quyen_thuc_hien_cuoc_goi_den_nguoi_dung*/
        public IHttpActionResult call_send_request(ZaloParams.zalo_input input)
        {
            try
            {
                var objInput = new
                {
                    reason_code = 101,
                    call_type = "audio",
                    input.phone,
                };

                string error = "";

                var obj = zalo_api.method_post("https://openapi.zalo.me/v2.0/oa/call/requestconsent", objInput, ref error);

                return Json(obj);

            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("api/zalo/call_check_request")]/*kiem_tra_nguoi_dung_chap_nhan_quyen_thuc_hien_cuoc_goi_tu_oa*/
        public IHttpActionResult call_check_request(string phone)
        {
            try
            {
                var url = "https://openapi.zalo.me/v2.0/oa/call/checkconsent?data={\"phone\":\"" + phone + "\"}";

                string error = "";

                var obj = zalo_api.method_get(url, ref error);

                return Json(obj);
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }
        #endregion

        #region [   Nhãn    ] 
        //[HttpGet]
        //[Route("api/zalo/tag_list")]/*lay_danh_sach_nhan*/
        //public IHttpActionResult tag_list()
        //{
        //    try
        //    {
        //        string error = "";

        //        var obj = zalo_api.method_get(zalo_url.tags, ref error);

        //        var result = JsonConvert.DeserializeObject<ZaloParams.zalo_output.tag_list>(obj.ToString());

        //        return Json(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}
        //[HttpPost]
        //[Route("api/zalo/tag_add")]/*gan_nhan_nguoi_dung*/
        //public IHttpActionResult tag_add(ZaloParams.zalo_input input)
        //{
        //    try
        //    {

        //        var objInput = new { input.user_id, input.tag_name };

        //        string error = "";

        //        var obj = zalo_api.method_post("https://openapi.zalo.me/v2.0/oa/tag/tagfollower", objInput, ref error);

        //        return Json(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Route("api/zalo/tag_remove")]/*go_nhan_khoi_nguoi_dung*/
        //public IHttpActionResult tag_remove(ZaloParams.zalo_input input)
        //{
        //    try
        //    {
        //        var objInput = new { input.user_id, input.tag_name };

        //        string error = "";

        //        var obj = zalo_api.method_post("https://openapi.zalo.me/v2.0/oa/tag/rmfollowerfromtag", objInput, ref error);

        //        return Json(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Route("api/zalo/tag_delete")]/*xoa_nhan*/
        //public IHttpActionResult tag_delete(ZaloParams.zalo_input input)
        //{
        //    try
        //    {
        //        var objInput = new { input.tag_name };

        //        string error = "";

        //        var obj = zalo_api.method_post("https://openapi.zalo.me/v2.0/oa/tag/rmtag", objInput, ref error);

        //        return Json(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}
        #endregion



        #endregion




        /////////////////////////////////////////////////////////////////////////////////////


        [HttpPost]
        [Route("api/zalo/chat_file")]
        public IHttpActionResult chat_file()
        {
            try
            {
                string error = "";
                var objInput = new
                {
                    recipient = new { user_id = "2381526250622754802" },
                    message = new
                    {
                        attachment = new
                        {
                            payload = new { token = "-wluLVbGcXdi_kHDirl5IAwqvKxm4PuOj-NbJFyGcKgzwE5Bg4hFJkkakXMZNSz3uA-m5APSoaJlgh4JgZIREus6uo-U2-muWApmCi4Nt2F9eSKGu5R-GgVOvcQyNivalV6rHgvykb6mnQTRk6cUIh_RadkuVgzxZuZGAguy-mVogEOIq2Jj1VUHuLw_8C9S-uZARhzazcdvveuyya2S1jR7w03WGlKGyyZr5kzlqppDrCeouoIoPgVHe3Bk8jirgOAWDUuctcIdWl4ni7lOPA2FepApBTbZC34tSS5DO-8UmKO" },
                            type = "file"
                        }
                    }
                };

                var obj = zalo_api.method_post("https://openapi.zalo.me/v3.0/oa/message/cs", objInput, ref error);

                return Json(obj);
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Route("api/zalo/chat_file1")]
        public IHttpActionResult chat_file1(ZaloParams.zalo_input input)
        {
            try
            {
                string error = "";

                if (input != null && input.file != null)
                {
                    var file_output = zalo_api.method_upload(input.file, ref error);

                    if (!string.IsNullOrEmpty(error)) return Json(new { error = error });

                    if (file_output != null && file_output.data != null
                       && !string.IsNullOrEmpty(file_output.data.token))
                    {
                        var objInput = new
                        {
                            recipient = new { user_id = "2381526250622754802" },
                            message = new
                            {
                                attachment = new
                                {
                                    payload = new { token = file_output.data.token },
                                    //payload = new { token = "-wluLVbGcXdi_kHDirl5IAwqvKxm4PuOj-NbJFyGcKgzwE5Bg4hFJkkakXMZNSz3uA-m5APSoaJlgh4JgZIREus6uo-U2-muWApmCi4Nt2F9eSKGu5R-GgVOvcQyNivalV6rHgvykb6mnQTRk6cUIh_RadkuVgzxZuZGAguy-mVogEOIq2Jj1VUHuLw_8C9S-uZARhzazcdvveuyya2S1jR7w03WGlKGyyZr5kzlqppDrCeouoIoPgVHe3Bk8jirgOAWDUuctcIdWl4ni7lOPA2FepApBTbZC34tSS5DO-8UmKO" },
                                    type = "file"
                                }
                            }
                        };

                        var obj = zalo_api.method_post("https://openapi.zalo.me/v3.0/oa/message/cs", objInput, ref error);

                        return Json(obj);
                    }
                }
                else
                {
                    error = "You have not selected a file for upload!";
                }
                return Json(new { error = error });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }


        [HttpPost]
        [Route("api/zalo/chat_upload_image")]
        public IHttpActionResult chat_upload_image(ZaloParams.zalo_input input)
        {
            try
            {
                string error = "";

                if (input != null && input.file != null)
                {
                    var file_output = zalo_api.method_upload(input.file, ref error);

                    if (!string.IsNullOrEmpty(error)) return Json(new { error = error });

                    if (file_output != null && file_output.data != null
                       && !string.IsNullOrEmpty(file_output.data.token))
                    {
                        var objInput = new
                        {
                            recipient = new { user_id = "2381526250622754802" },
                            message = new
                            {
                                attachment = new
                                {
                                    payload = new { token = file_output.data.token },
                                    //payload = new { token = "-wluLVbGcXdi_kHDirl5IAwqvKxm4PuOj-NbJFyGcKgzwE5Bg4hFJkkakXMZNSz3uA-m5APSoaJlgh4JgZIREus6uo-U2-muWApmCi4Nt2F9eSKGu5R-GgVOvcQyNivalV6rHgvykb6mnQTRk6cUIh_RadkuVgzxZuZGAguy-mVogEOIq2Jj1VUHuLw_8C9S-uZARhzazcdvveuyya2S1jR7w03WGlKGyyZr5kzlqppDrCeouoIoPgVHe3Bk8jirgOAWDUuctcIdWl4ni7lOPA2FepApBTbZC34tSS5DO-8UmKO" },
                                    type = "file"
                                }
                            }
                        };

                        var obj = zalo_api.method_post("https://openapi.zalo.me/v3.0/oa/message/cs", objInput, ref error);

                        return Json(obj);
                    }
                }
                else
                {
                    error = "You have not selected a file for upload!";
                }
                return Json(new { error = error });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }


        [HttpGet]
        [Route("api/zalo/upload_file")]
        public IHttpActionResult upload_file(/*ZaloParams zalo*/)
        {
            string output = "";

            try
            {
                //var httpPostedFile = HttpContext.Current.Request.Files[0];

                string file_url = @"D:\\Records\\file.pdf";
                file_url = @"D:\\Records\\Logo_dương bản_ngang.png";

                string file_type = "application/pdf";
                file_type = "image/jpeg";
                //file_type = "image/png";
                byte[] file_base = File.ReadAllBytes(file_url);
                string file_name = Path.GetFileName(file_url);

                //string contentType = "";
                //new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);

                bool isImage = false;
                isImage = true;

                string folder = HttpContext.Current.Server.MapPath("~/File");

                if (!System.IO.Directory.Exists(folder))
                    System.IO.Directory.CreateDirectory(folder);

                var FilePath = Path.Combine(folder, Guid.NewGuid() + file_name);

                System.IO.File.WriteAllBytes(FilePath, file_base /*Convert.FromBase64String(file_base)*/);

                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");


                var wrCreate = isImage == true
                    ? "https://openapi.zalo.me/v2.0/oa/upload/image" /*upload ảnh /gif output giống image*/
                    : "https://openapi.zalo.me/v2.0/oa/upload/file";
                //wrCreate = "https://business.openapi.zalo.me/upload/image";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(wrCreate);

                DBContext db = new DBContext();
                string access_token = db.tbl_ZaloSettings.FirstOrDefault(x => x.Text == "access_token")?.Value;

                request.Headers.Set("access_token", access_token);
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.Method = "POST";

                Stream rs = request.GetRequestStream();

                rs.Write(boundarybytes, 0, boundarybytes.Length);

                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, "file", FilePath, file_type);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);

                FileStream fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();

                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
                rs.Close();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11
                                               | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                using (var response1 = request.GetResponse())
                {
                    using (var reader = new StreamReader(response1.GetResponseStream()))
                    {
                        output = reader.ReadToEnd();
                    }
                }
                WriteLog(output);
                return Json(output);
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { Error = ex.Message, Error_Type = "Exception" });
            }

        }


        [HttpGet]
        [Route("api/zalo/zns_template")]
        public IHttpActionResult zns_template()
        {
            try
            {

                //lấy danh sách các template, ở tất cả các status

                var url = "https://business.openapi.zalo.me/template/all?offset=0&limit=100&status=1";

                string error = "";

                var obj = zalo_api.method_get(url, ref error);

                var str = JsonConvert.SerializeObject(obj);

                var templates = JsonConvert.DeserializeObject<ZaloParams.zalo_output.template>(str);

                var liTempId = new List<int>();

                if (templates != null && templates.data != null && templates.data.Count() > 0)
                {
                    liTempId = templates.data.Select(x => x.templateId).ToList();

                    if (liTempId.Count() > 0)
                    {
                        for (int i = 0; i < liTempId.Count(); i++)
                        {
                            var tempId = liTempId[i];

                            url = $"https://business.openapi.zalo.me/template/info?template_id={tempId}";

                            error = "";

                            obj = zalo_api.method_get(url, ref error);

                            str = JsonConvert.SerializeObject(obj);

                            var tempDetail = JsonConvert.DeserializeObject<ZaloParams.zalo_output.template_detail>(str);

                            if (tempDetail != null && tempDetail.data != null)
                            {
                                var tmpData = tempDetail.data;

                                //cập nhật lại thông tin template chi tiết
                                var templateId = tmpData.templateId;
                                var isAdd = false;
                                var tmp = db.tbl_ZaloTemplates.FirstOrDefault(x => x.TemplateID == templateId);
                                if (tmp == null)
                                {
                                    isAdd = true;
                                    tmp = new tbl_ZaloTemplates();
                                    tmp.ID = Guid.NewGuid();
                                    tmp.TemplateID = tmpData.templateId;
                                    tmp.CreatedOn = DateTime.Now;
                                }
                                tmp.TemplateName = tmpData.templateName;
                                tmp.Status = tmpData.status;
                                tmp.Timeout = tmpData.timeout;
                                tmp.PreviewUrl = tmpData.previewUrl;
                                tmp.TemplateQuality = tmpData.templateQuality;
                                tmp.TemplateTag = tmpData.templateTag;
                                tmp.Price = tmpData.price;
                                tmp.ApplyTemplateQuota = tmpData.applyTemplateQuota;
                                tmp.ModifiedOn = DateTime.Now;

                                if (isAdd) db.tbl_ZaloTemplates.Add(tmp);

                                db.SaveChanges();

                                db = new DBContext();
                                var liParams = new List<string>();

                                //cập nhật thông tin params truyền vào (add/remove/...)
                                if (tmpData.listParams != null && tmpData.listParams.Count() > 0)
                                {
                                    foreach (var pr in tmpData.listParams)
                                    {
                                        liParams.Add(pr.name);
                                        var isAddPr = false;
                                        var param = db.tbl_ZaloTemplateParams.FirstOrDefault(x => x.TemplateID == templateId && x.Name == pr.name);

                                        if (param == null)
                                        {
                                            isAddPr = true;
                                            param = new tbl_ZaloTemplateParams();
                                            param.ID = Guid.NewGuid();
                                            param.TemplateID = templateId;
                                            param.Name = pr.name;
                                            param.CreatedOn = DateTime.Now;
                                        }

                                        param.Require = pr.require;
                                        param.Type = pr.type;
                                        param.MaxLength = pr.maxLength;
                                        param.MinLength = pr.minLength;
                                        param.AcceptNull = pr.acceptNull;

                                        param.ModifiedOn = DateTime.Now;
                                        if (isAddPr) db.tbl_ZaloTemplateParams.Add(param);
                                        db.SaveChanges();
                                    }

                                    //nếu params không nằm trong danh sách được cập nhật mới thì remove params cũ 
                                    if (liParams.Count() > 0)
                                    {
                                        var liParamsRemove = db.tbl_ZaloTemplateParams.Where(x => x.TemplateID == templateId && !liParams.Contains(x.Name)).ToList();
                                        if (liParamsRemove != null && liParamsRemove.Count() > 0)
                                        {
                                            db.tbl_ZaloTemplateParams.RemoveRange(liParamsRemove);
                                        }
                                    }

                                    db.SaveChanges();

                                }
                            }
                        }
                    }
                }

                //1   Sự kiện OA gửi tin nhắn template oa_send_template
                //2   Sự kiện người dùng phản hồi template đánh giá dịch vụ user_feedback
                //3   Thông báo thay đổi về loại nội dung ZNS có thể gửi  change_oa_template_tags
                //4   Thông báo thay đổi về chất lượng gửi của mẫu tin ZNS    change_template_quality
                //5   Thông báo thay đổi quota mẫu ZNS rủi ro change_template_quota
                //6   Sự kiện thay đổi trạng thái Template ZNS    change_template_status


                return Json(new { msg = "Update successfully" });

            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }


        [HttpGet]
        [Route("api/zalo/zns_params_list")]
        public IHttpActionResult zns_params_list()
        {
            try
            {
                db = new DBContext();
                var data = new DataTable();
                string query = $"SELECT * FROM tbl_ZaloTemplateParamLink  WITH (READUNCOMMITTED) ORDER BY EntityName, FieldName ";
                (new SqlDataAdapter(query, db.Database.Connection.ConnectionString)).Fill(data);
                if (data != null)
                {
                    var columns = get_columns_name_by_data_table(data);
                    return Json(new { data, columns });
                }

                return Json(new { error = "Please try again!" });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }


        //danh sách hiển thị tất cả các tin nhắn (chia theo từng ngày session) - theo zaloid
        //thêm function khi nhận được event truyền zaloid thì focus vào nội dung chat của  zaloid đó
        //



        [HttpGet]
        [Route("api/zalo/session_list")]
        public IHttpActionResult session_list(string zaloid = "", Guid? userid = default)
        {
            try
            {
                db = new DBContext();
                var data = new DataTable();
                string query = $"select * from v_Zalo_Sessions  WITH (READUNCOMMITTED)  order by StartTime desc ";
                if (!string.IsNullOrEmpty(zaloid))
                {
                    query = $"select * from v_Zalo_Sessions  WITH (READUNCOMMITTED) WHERE UserId='{zaloid}'  order by StartTime desc ";
                }
                if (userid.HasValue && userid.Value != Guid.Empty)
                {
                    query = $"select * from v_Zalo_Sessions  WITH (READUNCOMMITTED) WHERE OwnerId='{userid}'  order by StartTime desc ";
                }

                (new SqlDataAdapter(query, db.Database.Connection.ConnectionString)).Fill(data);
                if (data != null)
                {
                    //var columns = get_columns_name_by_data_table(data);
                    return Json(new { data });
                }

                return Json(new { error = "Please try again!" });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("api/zalo/session_close")]
        public IHttpActionResult session_close(string zaloid = "")
        {
            try
            {
                db = new DBContext();
                //tìm session đang status=1 với zaloid này 
                var sessionInfo = db.tbl_ZaloSessions.FirstOrDefault(x => x.UserId == zaloid && x.Status == 1);
                if (sessionInfo != null)
                {
                    sessionInfo.Status = 2;
                    sessionInfo.EndTime = DateTime.Now;
                    db.SaveChanges();

                }
                return Json(new { msg = "Update successfully" });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }



        [HttpGet]
        [Route("api/zalo/chat_detail_by_session")]
        public IHttpActionResult chat_detail_by_session(Guid? session_id = default, string zaloid = "")
        {
            try
            {
                if (session_id.HasValue)
                {
                    db = new DBContext();
                    var oaInfo = db.Database.SqlQuery<tbl_ZaloOA>("SELECT TOP(1) * FROM tbl_ZaloOA").FirstOrDefault();
                    var sessionInfo = db.v_Zalo_Sessions.FirstOrDefault(x => x.ID == session_id);
                    var data = db.Database.SqlQuery<tbl_ZaloMessages>($"SELECT TOP(100) * FROM tbl_ZaloMessages WHERE SessionID='{session_id}'  ORDER BY MsgTime DESC").ToList();
                    var ddd = db.tbl_ZaloMessages.Where(x => x.SessionID == session_id).ToList();
                    var userInfo = db.Database.SqlQuery<tbl_ZaloUsers>($"SELECT TOP(1) * FROM tbl_ZaloUsers WHERE UserId='{sessionInfo.UserId}' OR UserIdByApp='{sessionInfo.UserId}'").FirstOrDefault();

                    return Json(new { data, oaInfo, userInfo, sessionInfo });
                }

                return Json(new { });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }


        [HttpGet]
        [Route("api/zalo/chat_detail_by_zaloid")]
        public IHttpActionResult chat_detail_by_zaloid(string zaloid = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(zaloid))
                {

                    db = new DBContext();

                    var oaInfo = db.Database.SqlQuery<tbl_ZaloOA>("SELECT TOP(1) * FROM tbl_ZaloOA").FirstOrDefault();

                    var sessionList = db.v_Zalo_Sessions.Where(x => x.UserId == zaloid).OrderByDescending(x => x.StartTime).ToList();

                    string liSessionId = string.Join("','", sessionList.Select(x => x.ID));

                    var data = db.Database.SqlQuery<tbl_ZaloMessages>($"SELECT TOP(100) * FROM tbl_ZaloMessages WHERE SessionID IN('{liSessionId}')  ORDER BY MsgTime").ToList();

                    var userInfo = db.Database.SqlQuery<tbl_ZaloUsers>($"SELECT TOP(1) * FROM tbl_ZaloUsers WHERE UserId='{zaloid}' OR UserIdByApp='{zaloid}'").FirstOrDefault();

                    return Json(new { data, oaInfo, userInfo, sessionList });
                }

                return Json(new { });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }


        [HttpPost]
        [Route("api/zalo/zns_params_save")]
        public IHttpActionResult zns_params_save(ZaloParams.zalo_input input)
        {
            try
            {
                if (input != null && input.ListParamLink != null && input.ListParamLink.Count > 0)
                {
                    foreach (var item in input.ListParamLink)
                    {
                        if (!string.IsNullOrEmpty(item.EntityName) && !string.IsNullOrEmpty(item.FieldName))
                        {
                            var link = db.tbl_ZaloTemplateParamLink.FirstOrDefault(x => x.EntityName == item.EntityName && x.FieldName == item.FieldName);
                            if (link == null)
                            {
                                link = new tbl_ZaloTemplateParamLink();
                                link.ID = Guid.NewGuid();
                                link.EntityName = item.EntityName;
                                link.FieldName = item.FieldName;
                                db.tbl_ZaloTemplateParamLink.Add(link);
                                db.SaveChanges();
                            }
                        }
                    }
                    return Json(new { msg = "Update successfully" });
                }
                return Json(new { error = "Please try again!" });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }


        [HttpPost]
        [Route("api/zalo/zns_params_delete")]
        public IHttpActionResult zns_params_delete(ZaloParams.zalo_input input)
        {
            try
            {
                if (input != null && input.ListParamLink != null && input.ListParamLink.Count > 0)
                {
                    foreach (var item in input.ListParamLink)
                    {
                        if (!string.IsNullOrEmpty(item.EntityName) && !string.IsNullOrEmpty(item.FieldName))
                        {
                            var link = db.tbl_ZaloTemplateParamLink.FirstOrDefault(x => x.EntityName == item.EntityName && x.FieldName == item.FieldName);
                            if (link != null)
                            {
                                db.tbl_ZaloTemplateParamLink.Remove(link);
                                db.SaveChanges();
                            }
                        }
                    }
                    return Json(new { msg = "Delete successfully" });
                }
                return Json(new { error = "Please try again!" });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("api/zalo/zns_template_list")]
        public IHttpActionResult zns_template_list()
        {
            try
            {
                db = new DBContext();
                var data = new DataTable();
                var dataParamsLink = new DataTable();
                string query = $"SELECT TemplateID [ID], TemplateName[Tên], Status[Trạng thái] FROM tbl_ZaloTemplates  WITH (READUNCOMMITTED) ORDER BY TemplateName ";
                (new SqlDataAdapter(query, db.Database.Connection.ConnectionString)).Fill(data);


                //params link list

                (new SqlDataAdapter($"SELECT * FROM tbl_ZaloTemplateParamLink ORDER BY EntityName, FieldName"
                    , db.Database.Connection.ConnectionString)).Fill(dataParamsLink);

                if (data != null)
                {
                    var columns = get_columns_name_by_data_table(data);
                    return Json(new { data, columns, dataParamsLink });
                }

                return Json(new { error = "Please try again!" });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("api/zalo/zns_template_detail")]
        public IHttpActionResult zns_template_detail(long TemplateID)
        {
            try
            {
                db = new DBContext();
                var dataParams = new DataTable();
                var dataDetail = new DataTable();

                var queryDetail = $"SELECT TOP(1)  [TemplateID], [TemplateName], [Status], [Timeout], [PreviewUrl], [TemplateQuality], [TemplateTag], [Price], [ApplyTemplateQuota]  FROM [tbl_ZaloTemplates] WITH (READUNCOMMITTED) WHERE TemplateID='{TemplateID}' ";
                (new SqlDataAdapter(queryDetail, db.Database.Connection.ConnectionString)).Fill(dataDetail);

                if (dataDetail != null && dataDetail.Rows.Count == 1)
                {
                    var columnsDetail = get_columns_name_by_data_table(dataDetail);

                    var query = $"SELECT * FROM [tbl_ZaloTemplateParams] WITH (READUNCOMMITTED) WHERE TemplateID='{TemplateID}' ORDER BY Name";
                    (new SqlDataAdapter(query, db.Database.Connection.ConnectionString)).Fill(dataParams);


                    return Json(new { dataDetail, columnsDetail, dataParams });
                }

                return Json(new { error = "Please try again! " + TemplateID });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        [Route("api/zalo/zns_template_save")]
        public IHttpActionResult zns_template_save(ZaloParams.zalo_input input)
        {
            try
            {
                if (input != null && input.ListParams != null && input.ListParams.Count > 0)
                {
                    foreach (var item in input.ListParams)
                    {

                        var link = db.tbl_ZaloTemplateParamLink.FirstOrDefault(x => x.EntityName == item.LinkWithEntity && x.FieldName == item.LinkWithField);
                        if (link != null)
                        {
                            var pr = db.tbl_ZaloTemplateParams.FirstOrDefault(x => x.ID == item.ID);
                            if (pr != null)
                            {
                                pr.LinkWithEntity = link.EntityName;
                                pr.LinkWithField = link.FieldName;
                            }
                        }
                        db.SaveChanges();
                    }
                    return Json(new { msg = "Update successfully" });
                }
                return Json(new { error = "Please try again!" });

            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }


        [HttpPost]
        [Route("api/zalo/zns_template_send")]
        public IHttpActionResult zns_template_send(ZaloParams.zalo_input input)
        {
            try
            {
                var tmpInfo = db.tbl_ZaloTemplates.FirstOrDefault(x => x.TemplateID == input.template_id);
                if (tmpInfo != null)
                {
                    var prInfo = db.tbl_ZaloTemplateParams.Where(x => x.TemplateID == input.template_id).ToList();

                    if (prInfo != null && prInfo.Count() > 0)
                    {
                        var template = new List<ZaloParams.zalo_input.template_data>();

                        List<string> objStringArr = new List<string>();

                        foreach (var item in prInfo)
                        {
                            var prlink = item.LinkWithEntity;

                            string tableName = $"[tbl_CRM_{item.LinkWithEntity}s]";

                            string phone = "03480100101";

                            string query = $"SELECT TOP(1) {item.LinkWithField} FROM {tableName} WHERE MobilePhone='{phone}'";

                            string value = db.Database.SqlQuery<string>(query).FirstOrDefault();

                            template.Add(new ZaloParams.zalo_input.template_data
                            {
                                key = item.Name,
                                value = value,
                            });
                        }

                        for (int i = 0; i < template.Count(); i++)
                        {
                            objStringArr.Add("\"" + template[i].key + "\":\"" + template[i].value + "\"");
                        }

                        string objString = "{" + string.Join(",", objStringArr) + "}";

                        //tạo ra 1 raw data để update result

                        var convertJS = JsonConvert.DeserializeObject<object>(objString);

                        var inputPr = new
                        {
                            phone = "84932032276",
                            //phone = "84348010010",
                            input.template_id,
                            template_data = convertJS,
                            tracking_id = Guid.NewGuid().ToString().Replace("-", "")
                        };

                        string error = "";

                        var obj = zalo_api.method_post("https://business.openapi.zalo.me/message/template", inputPr, ref error);

                        if (!string.IsNullOrEmpty(error)) return Json(new { error });

                        string outputStr = JsonConvert.SerializeObject(obj);

                        var output = JsonConvert.DeserializeObject<ZaloParams.zalo_output.send_zns>(outputStr);

                        if (output != null && output.data != null && output.data.quota != null)
                        {
                            var oaInfo = db.tbl_ZaloOA.FirstOrDefault();
                            if (oaInfo != null)
                            {
                                oaInfo.RemainingQuotaPromotion = output.data.quota.remainingQuotaPromotion;
                                oaInfo.RemainingQuota = output.data.quota.remainingQuota;
                                oaInfo.DailyQuotaPromotion = output.data.quota.dailyQuotaPromotion;
                                oaInfo.DailyQuota = output.data.quota.dailyQuota;
                                db.SaveChanges();
                            }
                        }
                        return Json(output);
                    }
                }

                #region MyRegion

                //{
                //    "phone": "84932032276",
                //    "template_id": "245881",
                //    "template_data": {
                //        "order_code": "1994", 
                //        "customer_name": "Lý Ngọc Phụng",
                //        "payment_status": "Pending" 
                //     },
                //    "tracking_id":"242487FF0275434ABF6E706B07EDAFEA"
                //}
                #endregion

                return Json(new { input });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }


        [HttpPost]
        [Route("api/zalo/request_share_info")]
        public IHttpActionResult request_share_info(ZaloParams.zalo_input input)
        {
            try
            {
                if (input != null && !string.IsNullOrEmpty(input.user_id))
                {
                    DBContext db = new DBContext();

                    var setting = db.tbl_ZaloSettingShareInfo.ToList();

                    string title = setting.FirstOrDefault(x => x.Text == "title")?.Value;
                    string subtitle = setting.FirstOrDefault(x => x.Text == "subtitle")?.Value;
                    string image_url = setting.FirstOrDefault(x => x.Text == "image_url")?.Value;

                    var pr = new
                    {
                        recipient = new { input.user_id },
                        message = new
                        {
                            attachment = new
                            {
                                type = "template",
                                payload = new
                                {
                                    template_type = "request_user_info",
                                    elements = new[] { new { title, subtitle, image_url } },
                                }
                            },
                        }
                    };

                    string error = "";

                    var obj = zalo_api.method_post("https://openapi.zalo.me/v3.0/oa/message/cs", pr, ref error);

                    if (!string.IsNullOrEmpty(error)) return Json(new { error });

                    var userInfo = db.tbl_ZaloUsers.FirstOrDefault(x => x.UserId == input.user_id);
                    if (userInfo != null)
                    {
                        userInfo.LastRequestInfoOn = DateTime.Now;
                        db.SaveChanges();
                    }
                    return Json(obj);
                }
                return Json(new { error = "Please try again!" });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }


        [HttpGet]
        [Route("api/zalo/contact_list")]
        public IHttpActionResult contact_list()
        {
            try
            {

                var data = db.tbl_ZaloUsers.OrderByDescending(x => x.LastInteractionOn).ToList();

                return Json(new { data });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }




        #region Workflow





        #endregion









        #region ẩn danh

        [HttpPost]
        [Route("api/zalo/send_msg_to_anonymous")]
        public IHttpActionResult send_msg_to_anonymous()
        {
            try
            {
                //https://openapi.zalo.me/v2.0/oa/message
                //{
                //  "recipient": {
                //    "conversation_id": "conversation_id",
                //    "anonymous_id": "anonymous_id"
                //  },
                //  "message": {
                //    "text": "hello, world!"
                //  }
                //}

                return Json(new { });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }


        #endregion

        #region tin tư vấn

        //[HttpPost]
        //[Route("api/zalo/gui_tin_tu_van_dinh_kem_hinh_anh")]
        //public IHttpActionResult gui_tin_tu_van_dinh_kem_hinh_anh()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v3.0/oa/message/cs
        //        //{
        //        //  "recipient": {
        //        //    "user_id": "user_id"
        //        //  },
        //        //  "message": {
        //        //    "attachment": {
        //        //      "payload": {
        //        //        "elements": [
        //        //          {
        //        //            "media_type": "image",
        //        //            "url": "https://stc-developers.zdn.vn/images/bg_1.jpg"
        //        //          }
        //        //        ],
        //        //        "template_type": "media"
        //        //      },
        //        //      "type": "template"
        //        //    },
        //        //    "text": "Zalo đạt 100 triệu người dùng"
        //        //  }
        //        //}

        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Route("api/zalo/gui_tin_tu_van_theo_mau_yeu_cau_thong_tin_nguoi_dung")]
        //public IHttpActionResult gui_tin_tu_van_theo_mau_yeu_cau_thong_tin_nguoi_dung()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v3.0/oa/message/cs
        //        //{
        //        //  "recipient": {
        //        //    "user_id": "user_id"
        //        //  },
        //        //  "message": {
        //        //    "attachment": {
        //        //      "payload": {
        //        //        "elements": [
        //        //          {
        //        //            "image_url": "https://developers.zalo.me/web/static/zalo.png",
        //        //            "subtitle": "Đang yêu cầu thông tin từ bạn",
        //        //            "title": "OA chatbot (Testing)"
        //        //          }
        //        //        ],
        //        //        "template_type": "request_user_info"
        //        //      },
        //        //      "type": "template"
        //        //    }
        //        //  }
        //        //}

        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Route("api/zalo/gui_tin_tu_van_dinh_kem_file")]
        //public IHttpActionResult gui_tin_tu_van_dinh_kem_file()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v3.0/oa/message/cs
        //        //{
        //        //  "recipient": {
        //        //    "user_id": "user_id"
        //        //  },
        //        //  "message": {
        //        //    "attachment": {
        //        //      "payload": {
        //        //        "token": "token"
        //        //      },
        //        //      "type": "file"
        //        //    }
        //        //  }
        //        //}

        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Route("api/zalo/gui_tin_tu_van_trich_dan")]
        //public IHttpActionResult gui_tin_tu_van_trich_dan()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v3.0/oa/message/cs
        //        //{
        //        //  "recipient": {
        //        //    "user_id": "186729651760683225"
        //        //  },
        //        //  "message": {
        //        //    "react_message_id": "quote_message_id",
        //        //    "text": "Chào bạn, Shop có địa chỉ là 182 Lê Đại Hành, P15, Q10, HCM"
        //        //  }
        //        //} 
        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Route("api/zalo/gui_tin_tu_van_kem_sticker")]
        //public IHttpActionResult gui_tin_tu_van_kem_sticker()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v3.0/oa/message/cs
        //        //{
        //        //  "recipient": {
        //        //    "user_id": "2468458835296117922"
        //        //  },
        //        //  "message": {
        //        //    "attachment": {
        //        //      "payload": {
        //        //        "elements": [
        //        //          {
        //        //            "media_type": "sticker",
        //        //            "attachment_id": "c3f934a408e1e1bfb8f0"
        //        //          }
        //        //        ],
        //        //        "template_type": "media"
        //        //      },
        //        //      "type": "template"
        //        //    }
        //        //  }
        //        //} 
        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Route("api/zalo/gui_tin_giao_dich")]
        //public IHttpActionResult gui_tin_giao_dich()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v3.0/oa/message/transaction
        //        //{
        //        //  "recipient": {
        //        //    "user_id": "2468458835296117922"
        //        //  },
        //        //  "message": {
        //        //    "attachment": {
        //        //      "payload": {
        //        //        "buttons": [
        //        //          {
        //        //            "payload": {
        //        //              "url": "https://oa.zalo.me/home"
        //        //            },
        //        //            "image_icon": "",
        //        //            "title": "Kiểm tra lộ trình - default icon",
        //        //            "type": "oa.open.url"
        //        //          },
        //        //          {
        //        //            "payload": "kiểm tra giỏ hàng",
        //        //            "image_icon": "wZ753VDsR4xWEC89zNTsNkGZr1xsPs19vZF22VHtTbxZ8zG9g24u3FXjZrQvQNH2wMl1MhbwT5_oOvX5_szXLB8tZq--TY0Dhp61JRfsAWglCej8ltmg3xC_rqsWAdjRkctG5lXzAGVlQe9BhZ9mJcSYVIDsc7MoPMnQ",
        //        //            "title": "Xem lại giỏ hàng",
        //        //            "type": "oa.query.show"
        //        //          },
        //        //          {
        //        //            "payload": {
        //        //              "phone_code": "84123456789"
        //        //            },
        //        //            "image_icon": "gNf2KPUOTG-ZSqLJaPTl6QTcKqIIXtaEfNP5Kv2NRncWPbDJpC4XIxie20pTYMq5gYv60DsQRHYn9XyVcuzu4_5o21NQbZbCxd087DcJFq7bTmeUq9qwGVie2ahEpZuLg2KDJfJ0Q12c85jAczqtKcSYVGJJ1cZMYtKR",
        //        //            "title": "Liên hệ tổng đài",
        //        //            "type": "oa.open.phone"
        //        //          }
        //        //        ],
        //        //        "elements": [
        //        //          {
        //        //            "attachment_id": "a-JJEvLdkcEPxTOwb6gYTfhwm26VSBHjaE3MDfrWedgLyC0smJRiA8w-csdGVg1cdxZLPT1je7k4i8nwbdYrSCJact3NOVGltEUQTjDayIhTvf1zqsR-Ai3aboRERgjvm-cI8iqv-NoIxi0cdNBoE6SYVJooM6xKTBft",
        //        //            "type": "banner"
        //        //          },
        //        //          {
        //        //            "type": "header",
        //        //            "align": "left",
        //        //            "content": "Trạng thái đơn hàng"
        //        //          },
        //        //          {
        //        //            "type": "text",
        //        //            "align": "left",
        //        //            "content": "• Cảm ơn bạn đã mua hàng tại cửa hàng.<br>• Thông tin đơn hàng của bạn như sau:"
        //        //          },
        //        //          {
        //        //            "type": "table",
        //        //            "content": [
        //        //              {
        //        //                "value": "F-01332973223",
        //        //                "key": "Mã khách hàng"
        //        //              },
        //        //              {
        //        //                "style": "yellow",
        //        //                "value": "Đang giao",
        //        //                "key": "Trạng thái"
        //        //              },
        //        //              {
        //        //                "value": "250,000đ",
        //        //                "key": "Giá tiền"
        //        //              }
        //        //            ]
        //        //          },
        //        //          {
        //        //            "type": "text",
        //        //            "align": "center",
        //        //            "content": "Lưu ý điện thoại. Xin cảm ơn!"
        //        //          }
        //        //        ],
        //        //        "template_type": "transaction_order",
        //        //        "language": "VI"
        //        //      },
        //        //      "type": "template"
        //        //    }
        //        //  }
        //        //}
        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Route("api/zalo/gui_tin_truyen_thong_broadcast")]
        //public IHttpActionResult gui_tin_truyen_thong_broadcast()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v2.0/oa/message
        //        //            {
        //        //  "recipient": {
        //        //    "target": "target"
        //        //  },
        //        //  "message": {
        //        //    "attachment": {
        //        //      "payload": {
        //        //        "elements": [
        //        //          {
        //        //            "media_type": "article",
        //        //            "attachment_id": "bd5ea46bb32e5a0033f"
        //        //          }
        //        //        ],
        //        //        "template_type": "media"
        //        //      },
        //        //      "type": "template"
        //        //    }
        //        //  }
        //        //}

        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Route("api/zalo/gui_tin_truyen_thong_ca_nhan")]
        //public IHttpActionResult gui_tin_truyen_thong_ca_nhan()
        //{
        //    try
        //    {

        //        //https://openapi.zalo.me/v3.0/oa/message/promotion

        //        //                {
        //        //  "recipient": {
        //        //    "user_id": "2468458835296117922"
        //        //  },
        //        //  "message": {
        //        //    "attachment": {
        //        //      "payload": {
        //        //        "buttons": [
        //        //          {
        //        //            "payload": {
        //        //              "url": "https://oa.zalo.me/home"
        //        //            },
        //        //            "image_icon": "",
        //        //            "title": "Tham khảo chương trình",
        //        //            "type": "oa.open.url"
        //        //          },
        //        //          {
        //        //            "payload": "#tuvan",
        //        //            "image_icon": "aeqg9SYn3nIUYYeWohGI1fYRF3V9f0GHceig8Ckq4WQVcpmWb-9SL8JLPt-6gX0QbTCfSuQv40UEst1imAm53CwFPsQ1jq9MsOnlQe6rIrZOYcrlWBTAKy_UQsV9vnfGozCuOvFfIbN5rcXddFKM4sSYVM0D50I9eWy3",
        //        //            "title": "Liên hệ chăm sóc viên",
        //        //            "type": "oa.query.hide"
        //        //          }
        //        //        ],
        //        //        "elements": [
        //        //          {
        //        //            "attachment_id": "aERC3A0iYGgQxim8fYIK6fxzsXkaFfq7ZFRB3RCyZH6RyziRis3RNydebK3iSPCJX_cJ3k1nW1EQufjN_pUL1f6Ypq3rTef5nxp6H_HnXKFDiyD5y762HS-baqRpQe5FdA376lTfq1sRyPr8ypd74ecbaLyA-tGmuJ-97W",
        //        //            "type": "banner"
        //        //          },
        //        //          {
        //        //            "type": "header",
        //        //            "align": "left",
        //        //            "content": "Ưu đãi thành viên Platinum"
        //        //          },
        //        //          {
        //        //            "type": "text",
        //        //            "align": "left",
        //        //            "content": "Ưu đãi dành riêng cho khách hàng Nguyen Van A hạng thẻ Platinum<br>Voucher trị giá 150$"
        //        //          },
        //        //          {
        //        //            "type": "table",
        //        //            "content": [
        //        //              {
        //        //                "value": "VC09279222",
        //        //                "key": "Voucher"
        //        //              },
        //        //              {
        //        //                "value": "30/12/2023",
        //        //                "key": "Hạn sử dụng"
        //        //              }
        //        //            ]
        //        //          },
        //        //          {
        //        //            "type": "text",
        //        //            "align": "center",
        //        //            "content": "Áp dụng tất cả cửa hàng trên toàn quốc"
        //        //          }
        //        //        ],
        //        //        "template_type": "promotion"
        //        //      },
        //        //      "type": "template"
        //        //    }
        //        //  }
        //        //}
        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Route("api/zalo/kiem_tra_han_muc_tu_van_mien_phi")]
        //public IHttpActionResult kiem_tra_han_muc_tu_van_mien_phi()
        //{
        //    try
        //    {

        //        ///https://openapi.zalo.me/v2.0/oa/quota/message
        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Route("api/zalo/kiem_tra_tin_tu_van_trong_khung_48h")]
        //public IHttpActionResult kiem_tra_tin_tu_van_trong_khung_48h()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v2.0/oa/quota/message
        //        //{
        //        //  "message_id": "message_id"
        //        //}

        //        //                {
        //        //  "error": -240,
        //        //  "message": "This API has been shut down, please switch to V3. See instructions here https://go.zalo.me/upgrade-api-06_2024"
        //        //}

        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpGet]
        //[Route("api/zalo/lay_thong_tin_tin_nhan_trong_mot_hoi_thoai")]
        //public IHttpActionResult lay_thong_tin_tin_nhan_trong_mot_hoi_thoai()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v2.0/oa/conversation?
        //        //data={"offset":0,"user_id":2512523625412515,"count":5}

        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Route("api/zalo/kiem_tra_han_muc_tin_nhan")]
        //public IHttpActionResult kiem_tra_han_muc_tin_nhan()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v3.0/oa/quota/message
        //        //{
        //        //  "user_id": "186729651760683225"
        //        //}
        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        #endregion

        #region GROUP CHAT | để sau
        //[HttpGet]
        //[Route("api/zalo/quota_group")] /*hạn mức của OA => để lấy asset_id*/
        //public IHttpActionResult quota_group() /*kiem_tra_han_muc_nhom*/
        //{
        //    try
        //    {
        //        //không tạo nhóm được cho chưa có asset_id
        //        string error = "";
        //        var objInput = new
        //        {
        //            quota_owner = "OA",
        //            product_type = "gmf10",
        //            quota_type = "sub_quota"

        //        };

        //        var obj = zalo_api.method_post("https://openapi.zalo.me/v3.0/oa/quota/group", objInput, ref error);

        //        return Json(obj);e
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}


        //[HttpPost]
        //[Route("api/zalo/group_create")] /*tao_nhom_moi*/
        //public IHttpActionResult group_create()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v3.0/oa/group/creategroupwithoa

        //        //{
        //        //  "group_name": "Tư vấn nha khoa",
        //        //  "member_user_ids": [
        //        //    "member_user_ids"
        //        //  ],
        //        //  "asset_id": "326e977e4d3da463fd2c",
        //        //  "group_description": "Group tư vấn nha khoa cho các nhân viên"
        //        //}
        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpGet]
        //[Route("api/zalo/group_info")] /*lay_thong_tin_nhom*/
        //public IHttpActionResult group_info()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v3.0/oa/group/getgroup?group_id=group_id 
        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Route("api/zalo/group_update")] /*cap_nhat_nhom*/
        //public IHttpActionResult group_update()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v3.0/oa/group/updateinfo

        //        //{
        //        //  "group_id": "f414c8f76fa586fbdfb4",
        //        //  "group_name": "Tư vấn nha khoa nhân viên",
        //        //  "group_avatar": "https://genk.mediacdn.vn/139269124445442048/2022/10/25/4311-1663948766-1666658898382-1666658898610182205404.png",
        //        //  "group_description": "Group tư vấn nha khoa cho các nhân viên",
        //        //  "lock_send_msg": true,
        //        //  "join_appr": true,
        //        //  "enable_msg_history": false,
        //        //  "enable_link_join": false
        //        //}

        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Route("api/zalo/group_invite")]/*moi_nguoi_quan_tam_vao_nhom*/
        //public IHttpActionResult group_invite()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v3.0/oa/group/invite 


        //        //{
        //        //  "group_id": "group_id",
        //        //  "member_user_ids": [
        //        //    "member_user_ids"
        //        //  ]
        //        //}
        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpGet]
        //[Route("api/zalo/group_member_pending")]/*lay_danh_sach_thanh_vien_cho_duyet*/
        //public IHttpActionResult group_member_pending()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v3.0/oa/group/listpendinginvite?group_id=group_id&offset=0&count=5
        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Route("api/zalo/group_member_accept")]/*dong_y_duyet_thanh_vien_moi_vao_nhom*/
        //public IHttpActionResult group_member_accept()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v3.0/oa/group/acceptpendinginvite

        //        //                {
        //        //  "group_id": "group_id",
        //        //  "member_user_ids": [
        //        //    "member_user_ids"
        //        //  ]
        //        //}
        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[Route("api/zalo/group_member_reject")]/*tu_choi_duyet_thanh_vien_moi_vao_nhom*/
        //public IHttpActionResult group_member_reject()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v3.0/oa/group/rejectpendinginvite

        //        //                {
        //        //  "group_id": "group_id",
        //        //  "member_user_ids": [
        //        //    "member_user_ids"
        //        //  ]
        //        //}
        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}


        //[HttpGet]
        //[Route("api/zalo/group_chat_list")]/*lay_danh_sach_tin_nhan_gan_nhat_cua_nhom*/
        //public IHttpActionResult group_chat_list()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v3.0/oa/group/listrecentchat?offset=0&count=5

        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpGet]
        //[Route("api/zalo/group_chat_detail")]/*lay_thong_tin_tin_nhan_trong_mot_hoi_thoai_nhom*/
        //public IHttpActionResult group_chat_detail()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v3.0/oa/group/conversation?group_id=group_id&offset=0&count=5

        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}



        //[HttpOptions]
        //[Route("api/zalo/cap_nhat_dich_vu_cho_nhom_chat")] /*để sau*/
        //public IHttpActionResult cap_nhat_dich_vu_cho_nhom_chat()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v3.0/oa/group/updateasset

        //        //{
        //        //  "group_id": "513c4f117a479319ca56",
        //        //  "asset_id": "asset_id"
        //        //}
        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        #endregion

        #region KHÔNG HỖ TRỢ NỮAAAAAAAAAAAAAAAAAA

        //[HttpPost]
        //[Route("api/zalo/gui_tin_nhan_dinh_kem_list_element")]
        //public IHttpActionResult gui_tin_nhan_dinh_kem_list_element()
        //{
        //    try
        //    {
        //        //{
        //        //  "recipient": {
        //        //    "user_id": "user_id"
        //        //  },
        //        //  "message": {
        //        //    "attachment": {
        //        //      "type": "template",
        //        //      "payload": {
        //        //        "template_type": "list",
        //        //        "elements": [
        //        //          {
        //        //            "title": "Official Account API",
        //        //            "subtitle": "There Is No Limit To What You Can Accomplish Using Zalo",
        //        //            "image_url": "https://stc-developers.zdn.vn/images/bg_1.jpg",
        //        //            "default_action": {
        //        //              "type": "oa.open.url",
        //        //              "url": "https://developers.zalo.me/docs/api/official-account-api-147"
        //        //            }
        //        //          },
        //        //          {
        //        //            "title": "Article API",
        //        //            "image_url": "https://stc-zaloprofile.zdn.vn/pc/v1/images/zalo_sharelogo.png",
        //        //            "default_action": {
        //        //              "type": "oa.open.url",
        //        //              "url": "https://developers.zalo.me/docs/api/article-api-151"
        //        //            }
        //        //          },
        //        //          {
        //        //            "title": "Social API",
        //        //            "image_url": "https://stc-zaloprofile.zdn.vn/pc/v1/images/zalo_sharelogo.png",
        //        //            "default_action": {
        //        //              "type": "oa.open.url",
        //        //              "url": "https://developers.zalo.me/docs/api/social-api-4"
        //        //            }
        //        //          },
        //        //          {
        //        //            "title": "Shop API",
        //        //            "image_url": "https://stc-zaloprofile.zdn.vn/pc/v1/images/zalo_sharelogo.png",
        //        //            "default_action": {
        //        //              "type": "oa.open.url",
        //        //              "url": "https://developers.zalo.me/docs/api/shop-api-124"
        //        //            }
        //        //          }
        //        //        ]
        //        //      }
        //        //    }
        //        //  }
        //        //}
        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}


        //[HttpPost]
        //[Route("api/zalo/gui_tin_nhan_theo_mau_yeu_cau_thong_tin_tu_nguoi_dung")]
        //public IHttpActionResult gui_tin_nhan_theo_mau_yeu_cau_thong_tin_tu_nguoi_dung()
        //{
        //    try
        //    { 
        //        //{
        //        //  "recipient": {
        //        //    "user_id": "user_id"
        //        //  },
        //        //  "message": {
        //        //    "attachment": {
        //        //      "payload": {
        //        //        "elements": [
        //        //          {
        //        //            "image_url": "https://developers.zalo.me/web/static/zalo.png",
        //        //            "subtitle": "Đang yêu cầu thông tin từ bạn",
        //        //            "title": "OA chatbot (Testing)"
        //        //          }
        //        //        ],
        //        //        "template_type": "request_user_info"
        //        //      },
        //        //      "type": "template"
        //        //    },
        //        //    "text": "hello, world!"
        //        //  }
        //        //}

        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //} 
        #endregion

        #region để sau 
        [HttpPost]
        [Route("api/zalo/follower_update")]/*cap_nhat_thong_tin_nguoi_quan_tam*/
        public IHttpActionResult follower_update()
        {
            try
            {
                //https://openapi.zalo.me/v2.0/oa/updatefollowerinfo


                //{
                //  "address": "ho chi minh",
                //  "user_id": "user_id",
                //  "phone": "012345678",
                //  "name": "name",
                //  "district_id": "district_id",
                //  "city_id": "city_id"
                //}


                return Json(new { });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }


        [HttpPost]
        [Route("api/zalo/follower_delete")]/*xoa_thong_tin_nguoi_quan_tam*/
        public IHttpActionResult follower_delete()
        {
            try
            {
                //https://openapi.zalo.me/v2.0/oa/deletefollowerinfo

                //            {
                //  "user_id": "2512523625412515"
                //}    

                return Json(new { });
            }
            catch (Exception ex)
            {
                WriteLog(ex, true);
                return Json(new { error = ex.Message });
            }
        }


        //[HttpPost]
        //[Route("api/zalo/broadcast_bai_viet")]
        //public IHttpActionResult broadcast_bai_viet()
        //{
        //    try
        //    {
        //        //{
        //        //  "recipient": {
        //        //    "target": "target"
        //        //  },
        //        //  "message": {
        //        //    "attachment": {
        //        //      "payload": {
        //        //        "elements": [
        //        //          {
        //        //            "media_type": "article",
        //        //            "attachment_id": "bd5ea46bb32e5a0033f"
        //        //          }
        //        //        ],
        //        //        "template_type": "media"
        //        //      },
        //        //      "type": "template"
        //        //    }
        //        //  }
        //        //}


        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        //[HttpGet]
        //[Route("api/zalo/lay_thong_tin_form")]
        //public IHttpActionResult lay_thong_tin_form()
        //{
        //    try
        //    {
        //        //https://openapi.zalo.me/v2.0/oa/form/get?
        //        //offset=0&form_id=123456&limit=200&to_time=1623689999&from_time=1621137100

        //        return Json(new { });
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(ex, true);
        //        return Json(new { error = ex.Message });
        //    }
        //}

        #endregion

    }
}