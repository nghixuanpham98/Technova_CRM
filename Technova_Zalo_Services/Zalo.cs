using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using RestSharp;
using Technova_Zalo_Services.Models;

namespace Technova_Zalo_Services
{
    public partial class Zalo : ServiceBase
    {
        public Zalo()
        {
            InitializeComponent();
        }

        Timer timerForAccessToken = new Timer();
        Timer timerForScanWebhook = new Timer();
        Timer timerForScanFollowers = new Timer();

        protected override void OnStart(string[] args)
        {
            WriteLog("SERVICE STARTED ================================================================================");

            FunctionsForFirstRun();

            timerForScanWebhook = new System.Timers.Timer(1000);  // every 01 seconds
            timerForScanWebhook.AutoReset = true;
            timerForScanWebhook.Elapsed += new System.Timers.ElapsedEventHandler(eventScanWebhook);
            timerForScanWebhook.Start();

            timerForScanFollowers = new System.Timers.Timer(2000);  // every 01 seconds
            timerForScanFollowers.AutoReset = true;
            timerForScanFollowers.Elapsed += new System.Timers.ElapsedEventHandler(eventScanFollowers);
            timerForScanFollowers.Start();

            timerForAccessToken.Elapsed += new ElapsedEventHandler(eventGetAccessToken);
            int min = 1; //x phút
            timerForAccessToken.Interval = 1000 * 60 * min;
            timerForAccessToken.Enabled = true;
        }

        protected override void OnStop()
        {
            WriteLog("SERVICE STOPPED ================================================================================");
        }

        private void eventScanWebhook(object source, ElapsedEventArgs e)
        {
            timerForScanWebhook.Stop();
            ScanWebhook();
            timerForScanWebhook.Start();
        }
        private void eventScanFollowers(object source, ElapsedEventArgs e)
        {
            timerForScanFollowers.Stop();
            ScanFollowers();
            timerForScanFollowers.Start();
        }
        private void eventGetAccessToken(object source, ElapsedEventArgs e)
        {
            timerForAccessToken.Stop();

            var now = DateTime.Now;
            if (now.Minute == 0)
            {
                if (now.Hour == 0)
                    GetOAInfo();
                else if (now.Hour == 1)
                    CleanData();
                else if (now.Hour == 6 || now.Hour == 18)
                    GetAccessToken();

                GetUserList();
            }

            if (now.Minute % 15 == 0) /* every 15mins */
                GetZNSTemplate();

            timerForAccessToken.Start();
        }

        #region Call Zalo API
        public class zalo_api
        {
            public static object method_get(string url, ref string error)
            {

                try
                {
                    DBContext db = new DBContext();
                    string access_token = db.tbl_ZaloSettings.FirstOrDefault(x => x.Text == "access_token")?.Value;
                    var req = (HttpWebRequest)WebRequest.Create(url);
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
                            WriteLog(new { type = "info", function = "call zalo api - method.get", data = result });
                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    WriteLog(new { type = "error", function = "call zalo api - method.get", data = ex });
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
                            WriteLog(new { type = "info", function = "call zalo api - method.post", data = result });
                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    WriteLog(new { type = "error", function = "call zalo api - method.post", data = ex });
                }
                return null;
            }
        }
        #endregion

        void CleanData() /* RUN at 01AM everyday */
        {
            string function = "CleanData";
            try
            {
                DBContext db = new DBContext();

                string query = "";

                query += $" INSERT INTO [tbl_ZaloWebhookBU] SELECT * FROM [dbo].[tbl_ZaloWebhook] WHERE CreatedOn<=CAST(GETDATE() AS DATE) ";
                query += $" DELETE [dbo].[tbl_ZaloWebhook] WHERE CreatedOn<=CAST(GETDATE() AS DATE) ";
                query += $" UPDATE tbl_ZaloSessions SET EndTime=GETDATE(), Status=2 WHERE EndTime IS NULL AND Status=1  ";
                query += $"  ";
                query += $"  ";
                query += $"  ";

                db.Database.ExecuteSqlCommand(query);
                db.SaveChanges();

                WriteLog(new { type = "info", function });

            }
            catch (Exception ex)
            {
                WriteLog(new { type = "error", function, data = ex });
            }
        }
        void GetAccessToken()
        {
            try
            {
                DBContext db = new DBContext();
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
                        WriteLog(new { type = "info", function = "GetAccessToken", data = tkn });
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(new { type = "error", function = "GetAccessToken", data = ex });
            }
        }
        void GetOAInfo()
        {
            try
            {
                DBContext db = new DBContext();
                string error = "";

                var obj = zalo_api.method_get(ZaloParams.zalo_url.getoa, ref error);

                var result = JsonConvert.DeserializeObject<ZaloParams.zalo_output.oa_info>(obj.ToString());
                if (result != null && result.data != null)
                {
                    var isAdd = false;
                    var data = result.data;
                    var oa = db.tbl_ZaloOA.FirstOrDefault();
                    if (oa == null)
                    {
                        isAdd = true;
                        oa = new tbl_ZaloOA();
                        oa.CreatedOn = DateTime.Now;
                        oa.OAID = data.oa_id;
                    }
                    oa.Description = data.description;
                    oa.Name = data.name;
                    oa.IsVerified = data.is_verified;
                    oa.OAType = data.oa_type;
                    oa.CateName = data.cate_name;
                    oa.NumFollower = data.num_follower;
                    oa.Avatar = data.avatar;
                    oa.Cover = data.cover;
                    oa.PackageName = data.package_name;
                    oa.PackageValidThroughDate = data.package_valid_through_date;
                    oa.PackageAutoRenewDate = data.package_auto_renew_date;
                    oa.LinkedZca = data.linked_zca;
                    oa.ModifiedOn = DateTime.Now;

                    if (isAdd) db.tbl_ZaloOA.Add(oa);
                    db.SaveChanges();
                    WriteLog(new { type = "info", function = "GetOAInfo", data = oa });
                }
            }
            catch (Exception ex)
            {
                WriteLog(new { type = "error", function = "GetOAInfo", data = ex });
            }
        }
        void ScanFollowers()
        {
            string function = "ScanFollowers";

            try
            {
                DBContext db = new DBContext();

                var liOutput = new List<ZaloParams.zalo_output.user_detail.data_detail>();

                List<string> liUserId = new List<string>();

                var eventNameList = new List<string> { "follow", "unfollow" };

                string EventNameStr = string.Join("','", eventNameList);

                string query = $"SELECT TOP 10 * FROM tbl_ZaloWebhook  WITH (READUNCOMMITTED) WHERE Status=0 AND [EventName] IN ('{EventNameStr}') ORDER BY EventTime ";

                var eventList = db.Database.SqlQuery<tbl_ZaloWebhook>(query).ToList();

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

                            if (evEF != null && evEF.follower != null && !string.IsNullOrEmpty(evEF.follower.id))
                            {
                                liUserId.Add(evEF.follower.id);
                            }
                        }
                    }
                    db.SaveChanges();
                }

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
                            userInfo.LastInteractionOn = ConvertZaloDateStringToDB(userInfo.LastInteractionDate);
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

                            #region tạo lead
                            //isAdd = false;
                            //if (userInfo.IsFollower == true)
                            //{
                            //    var lead = db.tbl_CRM_Leads.FirstOrDefault(x => x.ZaloID == userInfo.UserId);
                            //    if (lead == null)
                            //    {
                            //        lead = new tbl_CRM_Leads();
                            //        lead.ID = Guid.NewGuid();
                            //        lead.ZaloID = userInfo.UserId;
                            //        isAdd = true;
                            //    }
                            //    lead.Topic = $"{userInfo.DisplayName} - User follow OA";
                            //    lead.FullName = userInfo.DisplayName;
                            //    lead.MobilePhone = !string.IsNullOrEmpty(userInfo.Phone) && userInfo.Phone.StartsWith("84") ? "0" + userInfo.Phone.Remove(0, 2) : userInfo.Phone;
                            //    if (isAdd) db.tbl_CRM_Leads.Add(lead);
                            //}
                            #endregion

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

        void ScanWebhook()
        {
            ScanWebhookChat();
            ScanSession();
            ScanWebhookUserShareInfo();
        }

        void ScanWebhookChat()
        {

            string function = "ScanWebhookChat";
            try
            {
                DBContext db = new DBContext();

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

                            var addSession = false;

                            if (evEF != null)
                            {
                                userId = evEF.sender.id == oa_info.OAID.ToLower() ? evEF.recipient.id : evEF.sender.id;

                                string message_string = JsonConvert.SerializeObject(evEF.message);
                                var msgObj = JsonConvert.DeserializeObject<ZaloParams.zalo_output.webhook_event_message>(message_string);
                                if (msgObj != null)
                                {
                                    lastMsgId = msgObj.msg_id;
                                }

                                var userName = "";
                                var userInfo = db.tbl_ZaloUsers.FirstOrDefault(x => x.UserId == userId);
                                if (userInfo != null)
                                {
                                    userInfo.LastMsgId = lastMsgId;
                                    userName = userInfo.DisplayName;
                                }
                                //kiểm tra tin nhắn có session đang mở ko, nếu có thì dùng session đó ko thì tạo mới 

                                var session = db.tbl_ZaloSessions.FirstOrDefault(x => x.UserId == userId && x.EndTime == null);
                                if (session == null)
                                {
                                    addSession = true;
                                    session = new tbl_ZaloSessions();
                                    session.Status = 0; // 0.waiting 1.opening 2.closed

                                    session.ID = Guid.NewGuid();
                                    session.StartTime = DateTime.Now;
                                    session.UserId = userId;
                                    session.Name = $"{userName} - {session.StartTime.Value.ToString("yyyy-MM-dd HH:mm:ss")}";
                                }
                                if (session != null
                                    && !session.ResponseTime.HasValue
                                    && evEF.sender.id == oa_info.OAID.ToLower())
                                {
                                    session.Status = 1; // 0.waiting 1.opening 2.closed
                                    session.ResponseTime = DateTime.Now;
                                }

                                if (addSession) db.tbl_ZaloSessions.Add(session);

                                var timestamp = long.Parse(evEF.timestamp);
                                var MsgTime = ConvertNumberToDateTime(timestamp);

                                var msg = new tbl_ZaloMessages();
                                msg.ID = Guid.NewGuid();
                                msg.FromId = evEF.sender.id;
                                msg.ToId = evEF.recipient.id;
                                msg.MsgId = lastMsgId;
                                msg.MsgTime = MsgTime;
                                msg.Message = message_string;
                                msg.SessionID = session.ID;
                                var evName = evEF.event_name.Split('_');
                                msg.Type = evName[evName.Count() - 1];
                                liMsg.Add(msg);

                            }
                        }
                    }

                    if (liMsg.Count() > 0) db.tbl_ZaloMessages.AddRange(liMsg);

                    db.SaveChanges();

                    WriteLog(new { type = "info", function, data = liMsg });
                }
            }
            catch (Exception ex)
            {
                WriteLog(new { type = "error", function, data = ex });
            }
        }

        void ScanSession()
        {

            string function = "ScanSession";
            try
            {
                DBContext db = new DBContext();
                //tìm xem có session nào chưa có owner không 

                var liSession = db.tbl_ZaloSessions.Where(x => x.Status == 0).OrderBy(x => x.StartTime).ToList(); /*waiting*/

                if (liSession != null && liSession.Count() > 0)
                {
                    foreach (var session in liSession)
                    {
                        //chọn 01 user đang online và số lượng tương tác ít nhất trong danh sách

                        string queryUserOnline = $" SELECT TOP(1) ID FROM v_Zalo_CRMUserStatus  WITH (READUNCOMMITTED) WHERE Status=1 ORDER BY NumberOfInteractions ";

                        var userOnline = db.Database.SqlQuery<Guid>(queryUserOnline).FirstOrDefault();
                        if (userOnline != null && userOnline != Guid.Empty)
                        {
                            //gắn user đó vào session đang waiting
                            session.OwnerId = userOnline;
                            session.ModifiedBy = userOnline;
                            session.Status = 1; /*opening*/
                            db.SaveChanges();

                            WriteLog(new { type = "info", function, data = session });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(new { type = "error", function, data = ex });
            }
        }



        void UpdateSession()
        {
            string function = "UpdateSession";
            try
            {






                WriteLog(new { type = "info", function, });

            }
            catch (Exception ex)
            {

                WriteLog(new { type = "error", function, data = ex });
            }
        }

        void ScanWebhookUserShareInfo()
        {

            string function = "ScanWebhookUserShareInfo";
            try
            {
                DBContext db = new DBContext();

                var eventNameList = new List<string> {
                    "user_submit_info"
                };

                string EventNameStr = string.Join("','", eventNameList);

                string query = $"SELECT TOP 10 * FROM tbl_ZaloWebhook  WITH (READUNCOMMITTED) WHERE Status=0 AND [EventName] IN ('{EventNameStr}') ORDER BY EventTime ";

                var eventList = db.Database.SqlQuery<tbl_ZaloWebhook>(query).ToList();

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


                            if (evEF != null)
                            {
                                var userId = evEF.sender.id == oa_info.OAID.ToLower() ? evEF.recipient.id : evEF.sender.id;

                                string info_string = JsonConvert.SerializeObject(evEF.info);

                                var infoObj = JsonConvert.DeserializeObject<ZaloParams.zalo_output.webhook_event_user_submit_info>(info_string);

                                var userInfo = db.tbl_ZaloUsers.FirstOrDefault(x => x.UserId == userId);
                                if (userInfo != null && infoObj != null)
                                {
                                    userInfo.Address = infoObj.address;
                                    userInfo.Phone = infoObj.phone;
                                    userInfo.City = infoObj.city;
                                    userInfo.District = infoObj.district;
                                    userInfo.Alias = infoObj.name;
                                    userInfo.ModifiedOn = DateTime.Now;
                                }
                                db.SaveChanges();
                                WriteLog(new { type = "info", function, data = userInfo });
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
        void GetUserList()
        {
            string function = "GetUserList";
            try
            {
                DBContext db = new DBContext();
                //string url = "https://openapi.zalo.me/v3.0/oa/user/getlist?data={\"offset\":0,\"count\":50}";
                //string url = "https://openapi.zalo.me/v3.0/oa/user/getlist?data={\"offset\":0,\"count\":50,\"last_interaction_period\":\"TODAY\",\"is_follower\":\"true\"}";
                string url = "https://openapi.zalo.me/v3.0/oa/user/getlist?data={\"offset\":0,\"count\":50,\"is_follower\":\"true\"}";

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
                        userInfo.LastInteractionOn = ConvertZaloDateStringToDB(userInfo.LastInteractionDate);
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
                        if (item.shared_info != null)
                        {
                            userInfo.Address = item.shared_info.address;
                            userInfo.Phone = item.shared_info.phone;
                            userInfo.City = item.shared_info.city;
                            userInfo.District = item.shared_info.district;
                            userInfo.Alias = item.shared_info.name;
                        }
                        userInfo.Notes = notes;
                        userInfo.Tags = tag_names;

                        if (isAdd) db.tbl_ZaloUsers.Add(userInfo);

                        db.SaveChanges();
                        WriteLog(new { type = "info", function, data = userInfo });
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(new { type = "error", function, data = ex });
            }
        }
        void GetZNSTemplate()
        {
            string function = "GetZNSTemplate";
            try
            {
                DBContext db = new DBContext();

                //lấy danh sách các template, ở tất cả các status
                var url = "https://business.openapi.zalo.me/template/all?offset=0&limit=100&status=1";

                string error = "";

                var obj = zalo_api.method_get(url, ref error);

                var str = JsonConvert.SerializeObject(obj);

                var templates = JsonConvert.DeserializeObject<ZaloParams.zalo_output.template>(str);

                var liTempId = new List<int>();
                var liTempIdRemove = new List<int>();

                if (templates != null && templates.data != null && templates.data.Count() > 0)
                {
                    liTempId = templates.data.Where(x => x.status == "ENABLE").Select(x => x.templateId).ToList();

                    if (liTempId.Count() > 0)
                    {
                        for (int i = 0; i < liTempId.Count(); i++)
                        {
                            var tempId = liTempId[i];
                            //cập nhật lại thông tin STATUS của template này trước -> vì nếu bị REJECT thì sẽ không gọi được api chi tiết
                            var tmpForUpdate = db.tbl_ZaloTemplates.FirstOrDefault(x => x.TemplateID == tempId);
                            if (tmpForUpdate != null)
                            {
                                tmpForUpdate.Status = templates.data.FirstOrDefault(x => x.templateId == tempId)?.status;
                                db.SaveChanges();
                            }

                            db = new DBContext();
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

                                WriteLog(new { type = "info", function, data = tmp });


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

                                        WriteLog(new { type = "info", function, data = param });
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
            }
            catch (Exception ex)
            {
                WriteLog(new { type = "error", function, data = ex });
            }
        }

        DateTime ConvertZaloDateStringToDB(string day)
        {
            string function = "ConvertZaloDateStringToDB";
            DateTime output = new DateTime();
            try
            {
                var strArr = day.Split('/');
                if (strArr.Count() == 3)
                {

                    var dd = int.Parse(strArr[0]);
                    var mm = int.Parse(strArr[1]);
                    var yy = int.Parse(strArr[2]);
                    output = new DateTime(yy, mm, dd);
                }
            }
            catch (Exception ex)
            {
                WriteLog(new { type = "error", function, data = ex });

            }
            return output;
        }

        void GetQuotaInfo()
        {
            string function = "GetQuotaInfo";
            try
            {
                DBContext db = new DBContext();
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
                                //oaInfo.RemainingQuotaPromotion = quotaInfo.data.remainingQuotaPromotion;
                                //oaInfo.RemainingQuota = quotaInfo.data.remainingQuota;
                                //oaInfo.DailyQuotaPromotion = quotaInfo.data.dailyQuotaPromotion;
                                //oaInfo.DailyQuota = quotaInfo.data.dailyQuota;
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
        void FunctionsForFirstRun()
        {
            string function = "FunctionsForFirstRun";
            try
            {
                if (Configs.Default.IsFirstRun)
                {
                    WriteLog(new { type = "info", function });
                    GetAccessToken();
                    GetOAInfo();
                    GetUserList();
                    GetZNSTemplate();
                }
            }
            catch (Exception ex)
            {
                WriteLog(new { type = "error", function, data = ex });
            }
        }
        public static void WriteLog(object Message, bool isException = false)
        {
            try
            {
                string folderLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");

                if (!Directory.Exists(folderLog)) Directory.CreateDirectory(folderLog);

                string path = Path.Combine(folderLog, DateTime.Now.ToString("yyyy_MM_dd") + ".txt");

                if (isException) path = Path.Combine(folderLog, DateTime.Now.ToString("yyyy_MM_dd") + "_exception.txt");

                if (!System.IO.File.Exists(path)) System.IO.File.Create(path).Close();

                TextWriter tw = new StreamWriter(path, true);

                tw.WriteLine($"\n\n{DateTime.Now.ToString("HH:mm:ss")}\t{JsonConvert.SerializeObject(Message)}\n");

                tw.Close();
            }
            catch { }
        }
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

    }
}