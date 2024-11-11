using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Technova_Mailchimp_Services.Models;

namespace Technova_Mailchimp_Services
{
    public partial class Mailchimp : ServiceBase
    {
        #region *** Configuration ***

        public Mailchimp()
        {
            InitializeComponent();
        }

        #region -- On Start --

        protected override void OnStart(string[] args)
        {
            Common.WriteLogs("Service Mailchimp is started");

            // Calling the async method from a non-async context
            Task.Run(() => GetAudienceFromMailchimp());

            Task.Run(() => GetTemplatesFromMailchimp());

            Task.Run(() => GetCampaignsInDB());

            #region -- Config schedule --

            // Templates
            Timer timerTemplateAsync = new Timer();

            timerTemplateAsync.Elapsed += new ElapsedEventHandler(OnElapsedTimeTemplateAsync);
            timerTemplateAsync.Interval = Configs.Default.TimeTemplateAsync; //number in miliseconds  
            timerTemplateAsync.Enabled = true;

            // Data
            Timer timerScanData = new Timer();

            timerScanData.Elapsed += new ElapsedEventHandler(OnElapsedTimeScanData);
            timerScanData.Interval = Configs.Default.TimeScanData; //number in miliseconds  
            timerScanData.Enabled = true;

            // Email Activities
            Timer timerScanEmailActivities = new Timer();

            timerScanEmailActivities.Elapsed += new ElapsedEventHandler(OnElapsedTimeScanEmailActivities);
            timerScanEmailActivities.Interval = Configs.Default.TimeScanEmailActivities; //number in miliseconds  
            timerScanEmailActivities.Enabled = true;

            #endregion
        }

        #endregion

        #region -- On Stop --

        protected override void OnStop()
        {
            Common.WriteLogs("Service is stopped");
        }

        #endregion

        #endregion

        #region *** Functions ***

        #region -- Schedule --

        private void OnElapsedTimeTemplateAsync(object source, ElapsedEventArgs e)
        {
            Task.Run(() => GetTemplatesFromMailchimp());
        }

        private void OnElapsedTimeScanData(object source, ElapsedEventArgs e)
        {
            Task.Run(() => GetDataToMailchimp());
        }

        private void OnElapsedTimeScanEmailActivities(object source, ElapsedEventArgs e)
        {
            Task.Run(() => GetCampaignsInDB());
        }

        #endregion

        #region -- Audiences --

        private async Task GetAudienceFromMailchimp()
        {
            try
            {
                var offset = 0;
                var count = 1000;
                bool moreData = true;

                while (moreData)
                {
                    var response = await Common
                        .ExecuteHttpRequestWithLogging(client => client.GetAsync($"lists?count={count}&offset={offset}"));

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        JObject jsonResponse = JObject.Parse(content);
                        var audiences = jsonResponse["lists"];

                        foreach (var item in audiences)
                        {
                            Audiences audience = ParseAudience(item);

                            await CheckAudienceExistAsync(audience);
                        }

                        moreData = audiences.Count() == count;
                        offset += count;
                    }
                    else
                    {
                        moreData = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Error in GetAudienceFromMailchimp: " + ex.Message);
            }
        }

        private Audiences ParseAudience(JToken item)
        {
            var audience = new Audiences
            {
                McID = item["id"]?.Value<string>(),
                WebID = item["web_id"]?.Value<int>(),
                Name = item["name"]?.Value<string>(),
                PermissionReminder = item["permission_reminder"]?.Value<string>(),
                UseArchiveBar = item["use_archive_bar"]?.Value<bool>(),
                NotifyOnSubscribe = item["notify_on_subscribe"]?.Value<string>(),
                NotifyOnUnsubscribe = item["notify_on_unsubscribe"]?.Value<string>(),
                DateCreated = DateTime.TryParse(item["date_created"]?.ToString(), out var dateCreated) ? dateCreated : (DateTime?)null,
                ListRating = item["list_rating"]?.Value<int>(),
                EmailTypeOption = item["email_type_option"]?.Value<bool>(),
                SubscribeUrlShort = item["subscribe_url_short"]?.Value<string>(),
                SubscribeUrlLong = item["subscribe_url_long"]?.Value<string>(),
                BeamerAddress = item["beamer_address"]?.Value<string>(),
                Visibility = item["visibility"]?.Value<string>(),
                DoubleOptin = item["double_optin"]?.Value<bool>(),
                HasWelcome = item["has_welcome"]?.Value<bool>(),
                MarketingPermissions = item["marketing_permissions"]?.Value<bool>(),
                Company = item["contact"]?["company"]?.Value<string>(),
                Address1 = item["contact"]?["address1"]?.Value<string>(),
                Address2 = item["contact"]?["address2"]?.Value<string>(),
                City = item["contact"]?["city"]?.Value<string>(),
                State = item["contact"]?["state"]?.Value<string>(),
                Zip = item["contact"]?["zip"]?.Value<string>(),
                Country = item["contact"]?["country"]?.Value<string>(),
                Phone = item["contact"]?["phone"]?.Value<string>(),
                FromName = item["campaign_defaults"]?["from_name"]?.Value<string>(),
                FromEmail = item["campaign_defaults"]?["from_email"]?.Value<string>(),
                Subject = item["campaign_defaults"]?["subject"]?.Value<string>(),
                Language = item["campaign_defaults"]?["language"]?.Value<string>(),
                MemberCount = item["stats"]?["member_count"]?.Value<int>(),
                TotalContacts = item["stats"]?["total_contacts"]?.Value<int>(),
                UnsubscribeCount = item["stats"]?["unsubscribe_count"]?.Value<int>(),
                CleanedCount = item["stats"]?["cleaned_count"]?.Value<int>(),
                MemberCountSinceSend = item["stats"]?["member_count_since_send"]?.Value<int>(),
                UnsubscribeCountSinceSend = item["stats"]?["unsubscribe_count_since_send"]?.Value<int>(),
                CleanedCountSinceSend = item["stats"]?["cleaned_count_since_send"]?.Value<int>(),
                CampaignCount = item["stats"]?["campaign_count"]?.Value<int>(),
                CampaignLastSent = DateTime.TryParse(item["stats"]?["campaign_last_sent"]?.ToString(), out var campaignLastSent) ? campaignLastSent : (DateTime?)null,
                MergeFieldCount = item["stats"]?["merge_field_count"]?.Value<int>(),
                AvgSubRate = item["stats"]?["avg_sub_rate"]?.Value<float>(),
                AvgUnsubRate = item["stats"]?["avg_unsub_rate"]?.Value<float>(),
                TargetSubRate = item["stats"]?["target_sub_rate"]?.Value<float>(),
                OpenRate = item["stats"]?["open_rate"]?.Value<float>(),
                ClickRate = item["stats"]?["click_rate"]?.Value<float>(),
                LastSubDate = DateTime.TryParse(item["stats"]?["last_sub_date"]?.ToString(), out var lastSubDate) ? lastSubDate : (DateTime?)null,
                LastUnsubDate = DateTime.TryParse(item["stats"]?["last_unsub_date"]?.ToString(), out var lastUnsubDate) ? lastUnsubDate : (DateTime?)null
            };

            return audience;
        }

        private async Task CheckAudienceExistAsync(Audiences audience)
        {
            try
            {
                // Query to check if McID already exists in tbl_Mc_Audiences
                string checkQuery = "SELECT ID FROM tbl_Mc_Audiences WHERE McID = @McID";

                List<SqlParameter> checkParams = new List<SqlParameter>
                {
                    new SqlParameter("@McID", audience.McID)
                };

                // Execute the query asynchronously to fetch the McID if it exists
                var result = await Common.ExecuteQueryAsync(checkQuery, checkParams);

                // If the audience exists (result has rows)
                if (result != null && result.Rows.Count > 0)
                {
                    string existingID = result.Rows[0]["ID"].ToString();

                    var convID = new Guid(existingID);

                    // Update the existing audience
                    await UpdateAudienceToDBAsync(audience, convID);
                }
                else
                {
                    // If the audience does not exist, create a new one
                    await CreateNewAudienceToDBAsync(audience);
                }
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function CheckAudienceExistAsync Error: " + ex.Message);
            }
        }

        private async Task CreateNewAudienceToDBAsync(Audiences audience)
        {
            try
            {
                // Begin inserting data into multiple tables

                #region -- Insert into tbl_Mc_Audiences --

                var newAID = Guid.NewGuid();

                string insertAudiencesQuery = @"
                    INSERT INTO tbl_Mc_Audiences
                    (
                        ID, McID, WebID, Name, PermissionReminder, UseArchiveBar, NotifyOnSubscribe, NotifyOnUnsubscribe, 
                        DateCreated, ListRating, EmailTypeOption, SubscribeUrlShort, SubscribeUrlLong, BeamerAddress, Visibility, 
                        DoubleOptin, HasWelcome, MarketingPermissions, CreatedOn, ModifiedOn
                    )
                    VALUES
                    (
                        @ID, @McID, @WebID, @Name, @PermissionReminder, @UseArchiveBar, @NotifyOnSubscribe, @NotifyOnUnsubscribe, 
                        @DateCreated, @ListRating, @EmailTypeOption, @SubscribeUrlShort, @SubscribeUrlLong, @BeamerAddress, @Visibility, 
                        @DoubleOptin, @HasWelcome, @MarketingPermissions, @CreatedOn, @ModifiedOn
                    )";

                List<SqlParameter> insertAudiencesParams = new List<SqlParameter>
                {
                    new SqlParameter("@ID", newAID),
                    new SqlParameter("@McID", audience.McID ?? (object)DBNull.Value),
                    new SqlParameter("@WebID", audience.WebID ?? (object)DBNull.Value),
                    new SqlParameter("@Name", audience.Name ?? (object)DBNull.Value),
                    new SqlParameter("@PermissionReminder", audience.PermissionReminder ?? (object)DBNull.Value),
                    new SqlParameter("@UseArchiveBar", audience.UseArchiveBar ?? (object)DBNull.Value),
                    new SqlParameter("@NotifyOnSubscribe", audience.NotifyOnSubscribe ?? (object)DBNull.Value),
                    new SqlParameter("@NotifyOnUnsubscribe", audience.NotifyOnUnsubscribe ?? (object)DBNull.Value),
                    new SqlParameter("@DateCreated", audience.DateCreated ?? (object)DBNull.Value),
                    new SqlParameter("@ListRating", audience.ListRating ?? (object)DBNull.Value),
                    new SqlParameter("@EmailTypeOption", audience.EmailTypeOption ?? (object)DBNull.Value),
                    new SqlParameter("@SubscribeUrlShort", audience.SubscribeUrlShort ?? (object)DBNull.Value),
                    new SqlParameter("@SubscribeUrlLong", audience.SubscribeUrlLong ?? (object)DBNull.Value),
                    new SqlParameter("@BeamerAddress", audience.BeamerAddress ?? (object)DBNull.Value),
                    new SqlParameter("@Visibility", audience.Visibility ?? (object)DBNull.Value),
                    new SqlParameter("@DoubleOptin", audience.DoubleOptin ?? (object)DBNull.Value),
                    new SqlParameter("@HasWelcome", audience.HasWelcome ?? (object)DBNull.Value),
                    new SqlParameter("@MarketingPermissions", audience.MarketingPermissions ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedOn", DateTime.Now),
                    new SqlParameter("@ModifiedOn", DateTime.Now)
                };

                await Common.ExecuteNonQueryAsync(insertAudiencesQuery, insertAudiencesParams);

                #endregion

                #region -- Insert into tbl_Mc_AudienceContacts --

                var newAContactID = Guid.NewGuid();

                string insertContactsQuery = @"
                    INSERT INTO tbl_Mc_AudienceContacts
                    (
                        ID, AudienceID, Company, Address1, Address2, City, State, Zip, Country, Phone, CreatedOn, ModifiedOn
                    )
                    VALUES
                    (
                        @ID, @AudienceID, @Company, @Address1, @Address2, @City, @State, @Zip, @Country, @Phone, @CreatedOn, @ModifiedOn
                    )";

                List<SqlParameter> insertContactsParams = new List<SqlParameter>
                {
                    new SqlParameter("@ID", newAContactID),
                    new SqlParameter("@AudienceID", newAID),
                    new SqlParameter("@Company", audience.Company ?? (object)DBNull.Value),
                    new SqlParameter("@Address1", audience.Address1 ?? (object)DBNull.Value),
                    new SqlParameter("@Address2", audience.Address2 ?? (object)DBNull.Value),
                    new SqlParameter("@City", audience.City ?? (object)DBNull.Value),
                    new SqlParameter("@State", audience.State ?? (object)DBNull.Value),
                    new SqlParameter("@Zip", audience.Zip ?? (object)DBNull.Value),
                    new SqlParameter("@Country", audience.Country ?? (object)DBNull.Value),
                    new SqlParameter("@Phone", audience.Phone ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedOn", DateTime.Now),
                    new SqlParameter("@ModifiedOn", DateTime.Now)
                };

                await Common.ExecuteNonQueryAsync(insertContactsQuery, insertContactsParams);

                #endregion

                #region -- Insert into tbl_Mc_AudienceDefaults --

                var newADefaultID = Guid.NewGuid();

                string insertDefaultsQuery = @"
                    INSERT INTO tbl_Mc_AudienceDefaults
                    (
                        ID, AudienceID, FromName, FromEmail, Subject, Language, CreatedOn, ModifiedOn
                    )
                    VALUES
                    (
                        @ID, @AudienceID, @FromName, @FromEmail, @Subject, @Language, @CreatedOn, @ModifiedOn
                    )";

                List<SqlParameter> insertDefaultsParams = new List<SqlParameter>
                {
                    new SqlParameter("@ID", newADefaultID),
                    new SqlParameter("@AudienceID", newAID),
                    new SqlParameter("@FromName", audience.FromName ?? (object)DBNull.Value),
                    new SqlParameter("@FromEmail", audience.FromEmail ?? (object)DBNull.Value),
                    new SqlParameter("@Subject", audience.Subject ?? (object)DBNull.Value),
                    new SqlParameter("@Language", audience.Language ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedOn", DateTime.Now),
                    new SqlParameter("@ModifiedOn", DateTime.Now)
                };

                await Common.ExecuteNonQueryAsync(insertDefaultsQuery, insertDefaultsParams);

                #endregion

                #region -- Insert into tbl_Mc_AudienceStats --

                var newAStatID = Guid.NewGuid();

                string insertStatsQuery = @"
                    INSERT INTO tbl_Mc_AudienceStats
                    (
                        ID, AudienceID, MemberCount, TotalContacts, UnsubscribeCount, CleanedCount, MemberCountSinceSend, 
                        UnsubscribeCountSinceSend, CleanedCountSinceSend, CampaignCount, CampaignLastSent, MergeFieldCount, 
                        AvgSubRate, AvgUnsubRate, TargetSubRate, OpenRate, ClickRate, LastSubDate, LastUnsubDate, CreatedOn, ModifiedOn
                    )
                    VALUES
                    (
                        @ID, @AudienceID, @MemberCount, @TotalContacts, @UnsubscribeCount, @CleanedCount, @MemberCountSinceSend, 
                        @UnsubscribeCountSinceSend, @CleanedCountSinceSend, @CampaignCount, @CampaignLastSent, @MergeFieldCount, 
                        @AvgSubRate, @AvgUnsubRate, @TargetSubRate, @OpenRate, @ClickRate, @LastSubDate, @LastUnsubDate, @CreatedOn, @ModifiedOn
                    )";

                List<SqlParameter> insertStatsParams = new List<SqlParameter>
                {
                    new SqlParameter("@ID", newAStatID),
                    new SqlParameter("@AudienceID", newAID),
                    new SqlParameter("@MemberCount", audience.MemberCount ?? (object)DBNull.Value),
                    new SqlParameter("@TotalContacts", audience.TotalContacts ?? (object)DBNull.Value),
                    new SqlParameter("@UnsubscribeCount", audience.UnsubscribeCount ?? (object)DBNull.Value),
                    new SqlParameter("@CleanedCount", audience.CleanedCount ?? (object)DBNull.Value),
                    new SqlParameter("@MemberCountSinceSend", audience.MemberCountSinceSend ?? (object)DBNull.Value),
                    new SqlParameter("@UnsubscribeCountSinceSend", audience.UnsubscribeCountSinceSend ?? (object)DBNull.Value),
                    new SqlParameter("@CleanedCountSinceSend", audience.CleanedCountSinceSend ?? (object)DBNull.Value),
                    new SqlParameter("@CampaignCount", audience.CampaignCount ?? (object)DBNull.Value),
                    new SqlParameter("@CampaignLastSent", audience.CampaignLastSent ?? (object)DBNull.Value),
                    new SqlParameter("@MergeFieldCount", audience.MergeFieldCount ?? (object)DBNull.Value),
                    new SqlParameter("@AvgSubRate", audience.AvgSubRate ?? (object)DBNull.Value),
                    new SqlParameter("@AvgUnsubRate", audience.AvgUnsubRate ?? (object)DBNull.Value),
                    new SqlParameter("@TargetSubRate", audience.TargetSubRate ?? (object)DBNull.Value),
                    new SqlParameter("@OpenRate", audience.OpenRate ?? (object)DBNull.Value),
                    new SqlParameter("@ClickRate", audience.ClickRate ?? (object)DBNull.Value),
                    new SqlParameter("@LastSubDate", audience.LastSubDate ?? (object)DBNull.Value),
                    new SqlParameter("@LastUnsubDate", audience.LastUnsubDate ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedOn", DateTime.Now),
                    new SqlParameter("@ModifiedOn", DateTime.Now)
                };

                await Common.ExecuteNonQueryAsync(insertStatsQuery, insertStatsParams);

                #endregion

                // Begin get data in db to Mailchimp
                await GetDataToMailchimp();
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function CreateNewAudienceToDBAsync Error: " + ex.Message);
            }
        }

        private async Task UpdateAudienceToDBAsync(Audiences audience, Guid existingID)
        {
            try
            {
                // Begin updating data into multiple tables

                #region -- Update into tbl_Mc_Audiences --

                string updateAudiencesQuery = @"
                    UPDATE tbl_Mc_Audiences
                    SET McID = @McID, WebID = @WebID, Name = @Name, PermissionReminder = @PermissionReminder,
                        UseArchiveBar = @UseArchiveBar, NotifyOnSubscribe = @NotifyOnSubscribe, NotifyOnUnsubscribe = @NotifyOnUnsubscribe,
                        DateCreated = @DateCreated, ListRating = @ListRating, EmailTypeOption = @EmailTypeOption,
                        SubscribeUrlShort = @SubscribeUrlShort, SubscribeUrlLong = @SubscribeUrlLong, BeamerAddress = @BeamerAddress,
                        Visibility = @Visibility, DoubleOptin = @DoubleOptin, HasWelcome = @HasWelcome,
                        MarketingPermissions = @MarketingPermissions, ModifiedOn = @ModifiedOn
                    WHERE ID = @ID";

                List<SqlParameter> updateAudiencesParams = new List<SqlParameter>
                {
                    new SqlParameter("@ID", existingID),
                    new SqlParameter("@McID", audience.McID ?? (object)DBNull.Value),
                    new SqlParameter("@WebID", audience.WebID ?? (object)DBNull.Value),
                    new SqlParameter("@Name", audience.Name ?? (object)DBNull.Value),
                    new SqlParameter("@PermissionReminder", audience.PermissionReminder ?? (object)DBNull.Value),
                    new SqlParameter("@UseArchiveBar", audience.UseArchiveBar ?? (object)DBNull.Value),
                    new SqlParameter("@NotifyOnSubscribe", audience.NotifyOnSubscribe ?? (object)DBNull.Value),
                    new SqlParameter("@NotifyOnUnsubscribe", audience.NotifyOnUnsubscribe ?? (object)DBNull.Value),
                    new SqlParameter("@DateCreated", audience.DateCreated ?? (object)DBNull.Value),
                    new SqlParameter("@ListRating", audience.ListRating ?? (object)DBNull.Value),
                    new SqlParameter("@EmailTypeOption", audience.EmailTypeOption ?? (object)DBNull.Value),
                    new SqlParameter("@SubscribeUrlShort", audience.SubscribeUrlShort ?? (object)DBNull.Value),
                    new SqlParameter("@SubscribeUrlLong", audience.SubscribeUrlLong ?? (object)DBNull.Value),
                    new SqlParameter("@BeamerAddress", audience.BeamerAddress ?? (object)DBNull.Value),
                    new SqlParameter("@Visibility", audience.Visibility ?? (object)DBNull.Value),
                    new SqlParameter("@DoubleOptin", audience.DoubleOptin ?? (object)DBNull.Value),
                    new SqlParameter("@HasWelcome", audience.HasWelcome ?? (object)DBNull.Value),
                    new SqlParameter("@MarketingPermissions", audience.MarketingPermissions ?? (object)DBNull.Value),
                    new SqlParameter("@ModifiedOn", DateTime.Now)
                };

                await Common.ExecuteNonQueryAsync(updateAudiencesQuery, updateAudiencesParams);

                #endregion

                #region -- Update into tbl_Mc_AudienceContacts --

                string updateContactsQuery = @"
                    UPDATE tbl_Mc_AudienceContacts
                    SET Company = @Company, Address1 = @Address1, Address2 = @Address2,
                        City = @City, State = @State, Zip = @Zip, Country = @Country, Phone = @Phone, ModifiedOn = @ModifiedOn
                    WHERE AudienceID = @AudienceID";

                List<SqlParameter> updateContactsParams = new List<SqlParameter>
                {
                    new SqlParameter("@AudienceID", existingID),
                    new SqlParameter("@Company", audience.Company ?? (object)DBNull.Value),
                    new SqlParameter("@Address1", audience.Address1 ?? (object)DBNull.Value),
                    new SqlParameter("@Address2", audience.Address2 ?? (object)DBNull.Value),
                    new SqlParameter("@City", audience.City ?? (object)DBNull.Value),
                    new SqlParameter("@State", audience.State ?? (object)DBNull.Value),
                    new SqlParameter("@Zip", audience.Zip ?? (object)DBNull.Value),
                    new SqlParameter("@Country", audience.Country ?? (object)DBNull.Value),
                    new SqlParameter("@Phone", audience.Phone ?? (object)DBNull.Value),
                    new SqlParameter("@ModifiedOn", DateTime.Now)
                };

                await Common.ExecuteNonQueryAsync(updateContactsQuery, updateContactsParams);

                #endregion

                #region -- Update into tbl_Mc_AudienceDefaults --

                string updateDefaultsQuery = @"
                    UPDATE tbl_Mc_AudienceDefaults
                    SET FromName = @FromName, FromEmail = @FromEmail,
                        Subject = @Subject, Language = @Language, ModifiedOn = @ModifiedOn
                    WHERE AudienceID = @AudienceID";

                List<SqlParameter> updateDefaultsParams = new List<SqlParameter>
                {
                    new SqlParameter("@AudienceID", existingID),
                    new SqlParameter("@FromName", audience.FromName ?? (object)DBNull.Value),
                    new SqlParameter("@FromEmail", audience.FromEmail ?? (object)DBNull.Value),
                    new SqlParameter("@Subject", audience.Subject ?? (object)DBNull.Value),
                    new SqlParameter("@Language", audience.Language ?? (object)DBNull.Value),
                    new SqlParameter("@ModifiedOn", DateTime.Now)
                };

                await Common.ExecuteNonQueryAsync(updateDefaultsQuery, updateDefaultsParams);

                #endregion

                #region -- Update into tbl_Mc_AudienceStats --

                string updateStatsQuery = @"
                    UPDATE tbl_Mc_AudienceStats
                    SET MemberCount = @MemberCount, TotalContacts = @TotalContacts, UnsubscribeCount = @UnsubscribeCount,
                        CleanedCount = @CleanedCount, MemberCountSinceSend = @MemberCountSinceSend,
                        UnsubscribeCountSinceSend = @UnsubscribeCountSinceSend, CleanedCountSinceSend = @CleanedCountSinceSend,
                        CampaignCount = @CampaignCount, CampaignLastSent = @CampaignLastSent, MergeFieldCount = @MergeFieldCount,
                        AvgSubRate = @AvgSubRate, AvgUnsubRate = @AvgUnsubRate, TargetSubRate = @TargetSubRate, OpenRate = @OpenRate,
                        ClickRate = @ClickRate, LastSubDate = @LastSubDate, LastUnsubDate = @LastUnsubDate, ModifiedOn = @ModifiedOn
                    WHERE AudienceID = @AudienceID";

                List<SqlParameter> updateStatsParams = new List<SqlParameter>
                {
                    new SqlParameter("@AudienceID", existingID),
                    new SqlParameter("@MemberCount", audience.MemberCount ?? (object)DBNull.Value),
                    new SqlParameter("@TotalContacts", audience.TotalContacts ?? (object)DBNull.Value),
                    new SqlParameter("@UnsubscribeCount", audience.UnsubscribeCount ?? (object)DBNull.Value),
                    new SqlParameter("@CleanedCount", audience.CleanedCount ?? (object)DBNull.Value),
                    new SqlParameter("@MemberCountSinceSend", audience.MemberCountSinceSend ?? (object)DBNull.Value),
                    new SqlParameter("@UnsubscribeCountSinceSend", audience.UnsubscribeCountSinceSend ?? (object)DBNull.Value),
                    new SqlParameter("@CleanedCountSinceSend", audience.CleanedCountSinceSend ?? (object)DBNull.Value),
                    new SqlParameter("@CampaignCount", audience.CampaignCount ?? (object)DBNull.Value),
                    new SqlParameter("@CampaignLastSent", audience.CampaignLastSent ?? (object)DBNull.Value),
                    new SqlParameter("@MergeFieldCount", audience.MergeFieldCount ?? (object)DBNull.Value),
                    new SqlParameter("@AvgSubRate", audience.AvgSubRate ?? (object)DBNull.Value),
                    new SqlParameter("@AvgUnsubRate", audience.AvgUnsubRate ?? (object)DBNull.Value),
                    new SqlParameter("@TargetSubRate", audience.TargetSubRate ?? (object)DBNull.Value),
                    new SqlParameter("@OpenRate", audience.OpenRate ?? (object)DBNull.Value),
                    new SqlParameter("@ClickRate", audience.ClickRate ?? (object)DBNull.Value),
                    new SqlParameter("@LastSubDate", audience.LastSubDate ?? (object)DBNull.Value),
                    new SqlParameter("@LastUnsubDate", audience.LastUnsubDate ?? (object)DBNull.Value),
                    new SqlParameter("@ModifiedOn", DateTime.Now)
                };

                await Common.ExecuteNonQueryAsync(updateStatsQuery, updateStatsParams);

                #endregion
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function UpdateAudienceToDBAsync Error: " + ex.Message);
            }
        }

        #endregion

        #region -- Data DB --

        #region -- Get data in db to Mailchimp --

        private async Task GetDataToMailchimp()
        {
            try
            {
                string checkDist = "SELECT DistID, Subject, TemplateID, TemplateContent, SendTime " +
                    "FROM [v_CRM_Distribute] " +
                    "WHERE DistTypeText = 'Mailchimp' AND DistStatusText = 'Pending'";

                var resultDist = await Common.ExecuteQueryAsync(checkDist);

                if (resultDist != null && resultDist.Rows.Count > 0)
                {
                    foreach (DataRow row in resultDist.Rows)
                    {
                        string checkAudience = "SELECT TOP 1 ID, McID FROM [tbl_Mc_Audiences]";

                        var resultAudience = await Common.ExecuteQueryAsync(checkAudience);

                        if (resultAudience != null && resultAudience.Rows.Count > 0)
                        {
                            var distribute = new Distributes
                            {
                                DistID = Guid.TryParse(row["DistID"]?.ToString(), out var distID) ? distID : Guid.Empty,
                                ListID = resultAudience.Rows[0]["McID"]?.ToString(),
                                Subject = row["Subject"]?.ToString(),
                                TemplateID = Guid.TryParse(row["TemplateID"]?.ToString(), out var templateID) ? templateID : Guid.Empty,
                                TemplateContent = row["TemplateContent"]?.ToString(),
                                SendTime = DateTime.TryParse(row["SendTime"]?.ToString(), out var sendTime) ? sendTime : (DateTime?)null
                            };

                            await CheckDuplicateDataBeforeCreateMember(distribute);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function GetDataToMailchimp Error: " + ex.Message);
            }
        }

        #endregion

        #region -- Update status complete to db --

        private async Task UpdateStatusToDB(Guid distID)
        {
            try
            {
                Common.WriteLogs(distID.ToString());

                string updateQuery = @"
                    UPDATE tbl_CRM_Distribute 
                    SET Status = @Status
                    WHERE ID = @ID";

                List<SqlParameter> updateParams = new List<SqlParameter>
                {
                    new SqlParameter("@ID", distID),
                    new SqlParameter("@Status", "1"),
                };

                // Execute the update query asynchronously
                await Common.ExecuteNonQueryAsync(updateQuery, updateParams);
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function UpdateStatusToDB Error: " + ex.Message);
            }
        }

        #endregion

        #endregion

        #region -- Members --

        private async Task CheckDuplicateDataBeforeCreateMember(Distributes distribute) {
            try
            {
                string checkDuplicate = @"
                    WITH UniqueEmails AS (
                        SELECT 
                            Dtb_ID, 
                            Atv_Subject, 
                            TemplateContent, 
                            Mem_Name, 
                            Mem_Email,
                            ROW_NUMBER() OVER (PARTITION BY Mem_Email ORDER BY Dtb_ID) AS RowNum
                        FROM 
                            [v_CRM_MarketingListMembers]
                        WHERE 
                            Dtb_ID = @Dtb_ID 
                            AND Dtb_Status = 0
                    )
                    SELECT 
                        Dtb_ID, 
                        Atv_Subject, 
                        TemplateContent, 
                        Mem_Name, 
                        Mem_Email
                    FROM 
                        UniqueEmails
                    WHERE 
                        RowNum > 1;";

                List<SqlParameter> checkParams = new List<SqlParameter>
                {
                    new SqlParameter("@Dtb_ID", distribute.DistID)
                };

                var result = await Common.ExecuteQueryAsync(checkDuplicate, checkParams);

                if (result != null && result.Rows.Count > 0)
                {
                    foreach (DataRow row in result.Rows)
                    {
                        var distributeID = Guid.TryParse(row["Dtb_ID"]?.ToString(), out var tempDistributeID) 
                            ? tempDistributeID : Guid.Empty;
                        var subject = row["Atv_Subject"]?.ToString();
                        var content = row["TemplateContent"]?.ToString();
                        var toEmail = row["Mem_Email"]?.ToString();

                        var newID = Guid.NewGuid();

                        string insertEmailQuery = @"
                            INSERT INTO tbl_Mc_CampaignEmails (ID, DistributeID, Subject, Content, ToEmail, Status, CreatedOn, ModifiedOn)
                            VALUES (@ID, @DistributeID, @Subject, @Content, @ToEmail, @Status, @CreatedOn, @ModifiedOn)";

                        List<SqlParameter> emailParams = new List<SqlParameter>
                        {
                            new SqlParameter("@ID", newID),
                            new SqlParameter("@DistributeID", distributeID),
                            new SqlParameter("@Subject", subject ?? (object)DBNull.Value),
                            new SqlParameter("@Content", content ?? (object)DBNull.Value),
                            new SqlParameter("@ToEmail", toEmail ?? (object)DBNull.Value),
                            new SqlParameter("@Status", "0"),
                            new SqlParameter("@CreatedOn", DateTime.Now),
                            new SqlParameter("@ModifiedOn", DateTime.Now)
                        };

                        await Common.ExecuteNonQueryAsync(insertEmailQuery, emailParams);
                    }
                }

                await GetDataToCreateMember(distribute);
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function CheckDuplicateDataBeforeCreateMember Error: " + ex.Message);
            }
        }

        private async Task GetDataToCreateMember(Distributes distribute)
        {
            try
            {
                // List to store emails
                List<string> emailList = new List<string>();
                // List to keep track of all member creation tasks
                List<Task> memberTasks = new List<Task>();

                string checkMem = @"
                    WITH UniqueEmails AS (
                        SELECT 
                            Dtb_ID, 
                            Mem_Name, 
                            Mem_Email,
                            ROW_NUMBER() OVER (PARTITION BY Mem_Email ORDER BY Dtb_ID) AS RowNum
                        FROM 
                            [v_CRM_MarketingListMembers]
                        WHERE 
                            Dtb_ID = @Dtb_ID 
                            AND Dtb_Status = 0
                    )
                    SELECT 
                        Dtb_ID, 
                        Mem_Name, 
                        Mem_Email
                    FROM 
                        UniqueEmails
                    WHERE 
                        RowNum = 1;";

                List<SqlParameter> checkParams = new List<SqlParameter>
                {
                    new SqlParameter("@Dtb_ID", distribute.DistID)
                };

                var resultMem = await Common.ExecuteQueryAsync(checkMem, checkParams);

                if (resultMem != null && resultMem.Rows.Count > 0)
                {
                    foreach (DataRow row in resultMem.Rows)
                    {
                        var name = row["Mem_Name"]?.ToString();
                        var email = row["Mem_Email"]?.ToString();

                        if (!string.IsNullOrEmpty(email))
                        {
                            emailList.Add(email); // Add email to the list

                            // Add the task to create a member to the list of tasks
                            memberTasks.Add(CreateMailchimpMember(distribute.ListID, email, name));
                        }
                    }

                    // Wait for all member creation tasks to complete
                    await Task.WhenAll(memberTasks);

                    // Now that all emails are added and all members are created, create the segment
                    await CreateMailchimpSegment(distribute, emailList.ToArray());
                }
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function GetDataToCreateMember Error: " + ex.Message);
            }
        }

        private async Task CreateMailchimpMember(string listId, string email, string name)
        {
            try
            {
                // Set up the request URL
                var subscriberHash = await Common.ComputeMD5HashAsync(email.ToLower());
                string url = $"lists/{listId}/members/{subscriberHash}?skip_merge_validation=true";

                // Create the payload for the member data
                var memberPayload = new
                {
                    email_address = email,
                    status = "subscribed",
                    merge_fields = new { LNAME = name }
                };

                // Serialize the payload
                var content = new StringContent(JsonConvert.SerializeObject(memberPayload), Encoding.UTF8, "application/json");

                // Use ExecuteHttpRequestWithLogging to handle the HTTP PUT request
                var response = await Common.ExecuteHttpRequestWithLogging(client => client.PutAsync(url, content));

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    Common.WriteLogs("CreateMailchimpMember error: " + errorContent);
                }
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function CreateMailchimpMember Error: " + ex.Message);
            }
        }

        #endregion

        #region -- Segments --

        private async Task CreateMailchimpSegment(Distributes distribute, string[] emails)
        {
            try
            {
                // Define the URL endpoint for creating a segment
                string url = $"lists/{distribute.ListID}/segments";

                // Construct the payload with the segment details
                var segmentPayload = new
                {
                    name = $"{distribute.DistID} - {distribute.Subject}",
                    static_segment = emails
                };

                // Serialize the payload
                var content = new StringContent(JsonConvert.SerializeObject(segmentPayload), Encoding.UTF8, "application/json");

                // Use ExecuteHttpRequestWithLogging to send the POST request
                var response = await Common.ExecuteHttpRequestWithLogging(client => client.PostAsync(url, content));

                // Handle the response
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var segmentResponse = JsonConvert.DeserializeObject<JObject>(result);

                    // Get the segment ID from the response
                    var segmentId = segmentResponse["id"]?.Value<int>();

                    // Proceed to create a campaign with the new segment ID
                    await CreateMailchimpCampaign(distribute, segmentId);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    Common.WriteLogs("Function CreateMailchimpSegment Error: " + errorContent);
                }
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function CreateMailchimpSegment Error: " + ex.Message);
            }
        }

        #endregion

        #region -- Campaigns --

        #region -- Get data to create campaign --

        private async Task CreateMailchimpCampaign(Distributes distribute, int? segmentId)
        {
            try
            {
                // Define the URL endpoint for creating a campaign
                string url = "campaigns";

                // Construct the payload for creating the campaign
                var campaignPayload = new
                {
                    type = "regular",
                    recipients = new
                    {
                        list_id = distribute.ListID,
                        segment_opts = new { saved_segment_id = segmentId }
                    },
                    settings = new
                    {
                        subject_line = distribute.Subject,
                        from_name = Configs.Default.FromName,
                        reply_to = Configs.Default.FromEmail,
                        use_conversation = true
                    },
                    tracking = new
                    {
                        opens = true,
                        html_clicks = true,
                        text_clicks = true,
                        ecomm360 = true
                    }
                };

                // Serialize the payload
                var content = new StringContent(JsonConvert.SerializeObject(campaignPayload), Encoding.UTF8, "application/json");

                // Use ExecuteHttpRequestWithLogging to send the POST request
                var response = await Common.ExecuteHttpRequestWithLogging(client => client.PostAsync(url, content));

                // Handle the response
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var campaignRS = JsonConvert.DeserializeObject<JObject>(result);

                    // Parse the campaign data from Mailchimp response
                    var campaign = ParseCampaign(campaignRS);

                    // Set additional campaign details
                    campaign.DistributeID = distribute.DistID;
                    campaign.TemplateID = distribute.TemplateID;

                    // Set content for the campaign (call SetContentForCampaign method here)
                    await SetContentForCampaign(campaign.McID, distribute);

                    // Insert the campaign into the database
                    await CreateNewCampaignToDBAsync(campaign);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    Common.WriteLogs("Function CreateMailchimpCampaign Error: " + errorContent);
                }
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function CreateMailchimpCampaign Error: " + ex.Message);
            }
        }

        private Campaigns ParseCampaign(JToken campaignRS)
        {
            var campaign = new Campaigns
            {
                // Campaign Basic Info
                McID = campaignRS["id"]?.Value<string>(),
                WebID = campaignRS["web_id"]?.Value<int>(),
                ParentID = campaignRS["parent_campaign_id"]?.Value<string>(),
                Type = campaignRS["type"]?.Value<string>(),
                CreateTime = DateTime.TryParse(campaignRS["create_time"]?.ToString(), out var createTime) ? createTime : (DateTime?)null,
                Status = campaignRS["status"]?.Value<string>(),
                EmailsSent = campaignRS["emails_sent"]?.Value<int>(),
                SendTime = DateTime.TryParse(campaignRS["send_time"]?.ToString(), out var sendTime) ? sendTime : (DateTime?)null,
                ContentType = campaignRS["content_type"]?.Value<string>(),

                // Campaign Settings (tbl_Mc_CampaignSettings)
                SubjectLine = campaignRS["settings"]?["subject_line"]?.Value<string>(),
                PreviewText = campaignRS["settings"]?["preview_text"]?.Value<string>(),
                Title = campaignRS["settings"]?["title"]?.Value<string>(),
                FromName = campaignRS["settings"]?["from_name"]?.Value<string>(),
                ReplyTo = campaignRS["settings"]?["reply_to"]?.Value<string>(),
                UseConversation = campaignRS["settings"]?["use_conversation"]?.Value<bool>(),
                ToName = campaignRS["settings"]?["to_name"]?.Value<string>(),

                // Campaign Recipients (tbl_Mc_CampaignRecipients)
                ListID = campaignRS["recipients"]?["list_id"]?.Value<string>(),
                ListIsActive = campaignRS["recipients"]?["list_is_active"]?.Value<bool>(),
                ListName = campaignRS["recipients"]?["list_name"]?.Value<string>(),
                RecipientCount = campaignRS["recipients"]?["recipient_count"]?.Value<int>()
            };

            return campaign;
        }

        private async Task CreateNewCampaignToDBAsync(Campaigns campaign)
        {
            try
            {
                // Begin inserting data into multiple tables

                #region -- Insert into tbl_Mc_Campaigns --

                var newCID = Guid.NewGuid();

                string insertCampaignQuery = @"
                    INSERT INTO tbl_Mc_Campaigns (ID, DistributeID, TemplateID, McID, WebID, ParentID, Type, CreateTime, 
                    Status, EmailsSent, SendTime, ContentType, CreatedOn, ModifiedOn)
                    VALUES (@ID, @DistributeID, @TemplateID, @McID, @WebID, @ParentID, @Type, @CreateTime, 
                    @Status, @EmailsSent, @SendTime, @ContentType, @CreatedOn, @ModifiedOn)";

                List<SqlParameter> campaignParams = new List<SqlParameter>
                {
                    new SqlParameter("@ID", newCID),
                    new SqlParameter("@DistributeID", campaign.DistributeID),
                    new SqlParameter("@TemplateID", campaign.TemplateID),
                    new SqlParameter("@McID", campaign.McID ?? (object)DBNull.Value),
                    new SqlParameter("@WebID", campaign.WebID ??(object) DBNull.Value),
                    new SqlParameter("@ParentID", campaign.ParentID ?? (object)DBNull.Value),
                    new SqlParameter("@Type", campaign.Type ?? (object)DBNull.Value),
                    new SqlParameter("@CreateTime", campaign.CreateTime ?? (object)DBNull.Value),
                    new SqlParameter("@Status", campaign.Status ?? (object)DBNull.Value),
                    new SqlParameter("@EmailsSent", campaign.EmailsSent ?? (object)DBNull.Value),
                    new SqlParameter("@SendTime", campaign.SendTime ?? (object)DBNull.Value),
                    new SqlParameter("@ContentType", campaign.ContentType ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedOn", DateTime.Now),
                    new SqlParameter("@ModifiedOn", DateTime.Now)
                };

                await Common.ExecuteNonQueryAsync(insertCampaignQuery, campaignParams);

                #endregion

                #region -- Insert into tbl_Mc_CampaignSettings --

                var newCSettingID = Guid.NewGuid();

                string insertSettingsQuery = @"
                    INSERT INTO tbl_Mc_CampaignSettings (ID, CampaignID, SubjectLine, PreviewText, Title, FromName, 
                    ReplyTo, UseConversation, ToName, CreatedOn, ModifiedOn)
                    VALUES (@ID, @CampaignID, @SubjectLine, @PreviewText, @Title, @FromName, 
                    @ReplyTo, @UseConversation, @ToName, @CreatedOn, @ModifiedOn)";

                List<SqlParameter> settingsParams = new List<SqlParameter>
                {
                    new SqlParameter("@ID", newCSettingID),
                    new SqlParameter("@CampaignID", newCID),
                    new SqlParameter("@SubjectLine", campaign.SubjectLine ?? (object)DBNull.Value),
                    new SqlParameter("@PreviewText", campaign.PreviewText ?? (object)DBNull.Value),
                    new SqlParameter("@Title", campaign.Title ?? (object)DBNull.Value),
                    new SqlParameter("@FromName", campaign.FromName ?? (object)DBNull.Value),
                    new SqlParameter("@ReplyTo", campaign.ReplyTo ?? (object)DBNull.Value),
                    new SqlParameter("@UseConversation", campaign.UseConversation ?? (object)DBNull.Value),
                    new SqlParameter("@ToName", campaign.ToName ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedOn", DateTime.Now),
                    new SqlParameter("@ModifiedOn", DateTime.Now)
                };

                await Common.ExecuteNonQueryAsync(insertSettingsQuery, settingsParams);

                #endregion

                #region -- Insert into tbl_Mc_CampaignRecipients --

                var newCRecipientID = Guid.NewGuid();

                string insertRecipientsQuery = @"
                    INSERT INTO tbl_Mc_CampaignRecipients (ID, CampaignID, ListID, ListIsActive, ListName, 
                    RecipientCount, CreatedOn, ModifiedOn)
                    VALUES (@ID, @CampaignID, @ListID, @ListIsActive, @ListName, @RecipientCount, 
                    @CreatedOn, @ModifiedOn)";

                List<SqlParameter> recipientsParams = new List<SqlParameter>
                {
                    new SqlParameter("@ID", newCRecipientID),
                    new SqlParameter("@CampaignID", newCID),
                    new SqlParameter("@ListID", campaign.ListID ?? (object)DBNull.Value),
                    new SqlParameter("@ListIsActive", campaign.ListIsActive ?? (object)DBNull.Value),
                    new SqlParameter("@ListName", campaign.ListName ?? (object)DBNull.Value),
                    new SqlParameter("@RecipientCount", campaign.RecipientCount ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedOn", DateTime.Now),
                    new SqlParameter("@ModifiedOn", DateTime.Now)
                };

                await Common.ExecuteNonQueryAsync(insertRecipientsQuery, recipientsParams);

                #endregion
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function CreateNewCampaignToDBAsync Error: " + ex.Message);
            }
        }

        #endregion

        #region -- Set content for campaign --

        private async Task SetContentForCampaign(string campaignID, Distributes distribute)
        {
            try
            {
                // Define the URL endpoint for setting campaign content
                string url = $"campaigns/{campaignID}/content";

                // Construct the payload for updating the campaign content
                var contentPayload = new
                {
                    html = distribute.TemplateContent,
                };

                // Serialize the payload
                var content = new StringContent(JsonConvert.SerializeObject(contentPayload), Encoding.UTF8, "application/json");

                // Use ExecuteHttpRequestWithLogging to send the PUT request
                var response = await Common.ExecuteHttpRequestWithLogging(client => client.PutAsync(url, content));

                // Handle the response
                if (response.IsSuccessStatusCode)
                {
                    var sendTime = distribute.SendTime;

                    if (sendTime <= DateTime.Today)
                    {
                        // Send the campaign immediately
                        await SendCampaign(campaignID);
                    }
                    else
                    {
                        // Schedule the campaign for later
                        await ScheduleCampaign(campaignID, sendTime);
                    }

                    // Update the status in the database
                    await UpdateStatusToDB(distribute.DistID);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    Common.WriteLogs("Function SetContentForCampaign Error: " + errorContent);
                }
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function SetContentForCampaign Error: " + ex.Message);
            }
        }

        #endregion

        #region -- Send campaign --

        private async Task SendCampaign(string campaignID)
        {
            try
            {
                // Define the URL endpoint for sending the campaign
                string url = $"campaigns/{campaignID}/actions/send";

                // Use ExecuteHttpRequestWithLogging to send the POST request
                var response = await Common.ExecuteHttpRequestWithLogging(client => client.PostAsync(url, null));

                // Handle the response
                if (response.IsSuccessStatusCode)
                {
                    Common.WriteLogs("Send campaign: " + campaignID + " successfully");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Common.WriteLogs("Function SendCampaign Error: " + errorContent);
                }
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function SendCampaign Error: " + ex.Message);
            }
        }

        #endregion

        #region -- Schedule for campaign --

        private async Task ScheduleCampaign(string campaignID, DateTime? scheduleTime)
        {
            try
            {
                // If scheduleTime is not provided, set the time to 8 AM today
                DateTime finalScheduleTime = (scheduleTime ?? DateTime.Today).AddHours(8);

                // Convert to ISO 8601 format string
                string scheduleTimeString = finalScheduleTime.ToString("o");

                // Define the URL endpoint for scheduling the campaign
                string url = $"campaigns/{campaignID}/actions/schedule";

                // Construct the payload for scheduling the campaign
                var schedulePayload = new
                {
                    schedule_time = scheduleTimeString, // ISO 8601 formatted date/time
                    timewarp = true
                };

                // Serialize the payload
                var content = new StringContent(JsonConvert.SerializeObject(schedulePayload), Encoding.UTF8, "application/json");

                // Use ExecuteHttpRequestWithLogging to send the POST request
                var response = await Common.ExecuteHttpRequestWithLogging(client => client.PostAsync(url, content));

                // Handle the response
                if (response.IsSuccessStatusCode)
                {
                    Common.WriteLogs("Schedule campaign: " + campaignID + " at " + finalScheduleTime + " successfully");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    Common.WriteLogs("Function ScheduleCampaign Error: " + errorContent);
                }
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function ScheduleCampaign Error: " + ex.Message);
            }
        }

        #endregion

        #endregion

        #region -- Templates --

        #region -- Get all templates from Mailchimp --

        private async Task GetTemplatesFromMailchimp()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11
                                      | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

                // Skip validation of SSL/TLS certificate
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(Common.GetURLMandrill());

                    // Prepare the POST request body as JSON
                    var postData = new
                    {
                        key = await Common.GetMandrillAPIKeyAsync()
                    };

                    var content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");

                    // Send the POST request to the API
                    HttpResponseMessage response = await client.PostAsync("templates/list", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        JArray jsonResponse = JArray.Parse(responseBody);

                        // Process each template in the response
                        foreach (var template in jsonResponse)
                        {
                            var slug = template["slug"]?.Value<string>();
                            var name = template["name"]?.Value<string>();

                            var labels = template["labels"] as JArray;
                            if (labels != null)
                            {
                                foreach (var label in labels)
                                {

                                }
                            }

                            var code = template["code"]?.Value<string>();
                            var subject = template["subject"]?.Value<string>();
                            var from_email = template["from_email"]?.Value<string>();
                            var from_name = template["from_name"]?.Value<string>();
                            var text = template["text"]?.Value<string>();
                            var publish_name = template["publish_name"]?.Value<string>();
                            var publish_code = template["publish_code"]?.Value<string>();
                            var publish_subject = template["publish_subject"]?.Value<string>();
                            var publish_from_email = template["publish_from_email"]?.Value<string>();
                            var publish_from_name = template["publish_from_name"]?.Value<string>();
                            var publish_text = template["publish_text"]?.Value<string>();

                            var published_at_string = template["published_at"]?.Value<string>();
                            DateTime? published_at = null;

                            if (!string.IsNullOrEmpty(published_at_string) &&
                                DateTime.TryParse(published_at_string, out DateTime parsedPublishedAt))
                            {
                                published_at = parsedPublishedAt;
                            }

                            var created_at_string = template["created_at"]?.Value<string>();
                            DateTime? created_at = null;

                            if (!string.IsNullOrEmpty(created_at_string) &&
                                DateTime.TryParse(created_at_string, out DateTime parsedCreatedAt))
                            {
                                created_at = parsedCreatedAt;
                            }

                            var updated_at_string = template["updated_at"]?.Value<string>();
                            DateTime? updated_at = null;

                            if (!string.IsNullOrEmpty(updated_at_string) &&
                                DateTime.TryParse(updated_at_string, out DateTime parsedUpdatedAt))
                            {
                                updated_at = parsedUpdatedAt;
                            }

                            var is_broken_template = template["is_broken_template"]?.Value<bool>();

                            // New

                            Templates item = new Templates();

                            item.Slug = slug;
                            item.Name = name;
                            item.Code = code;
                            item.Subject = subject;
                            item.FromEmail = from_email;
                            item.FromName = from_name;
                            item.Text = text;
                            item.PublishName = publish_name;
                            item.PublishCode = publish_code;
                            item.PublishSubject = publish_subject;
                            item.PublishFromEmail = publish_from_email;
                            item.PublishFromName = publish_from_name;
                            item.PublishText = publish_text;
                            item.PublishedAt = published_at;
                            item.CreatedAt = created_at;
                            item.UpdatedAt = updated_at;
                            item.IsBrokenTemplate = is_broken_template;

                            await CheckTemplateExistAsync(item);
                        }
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();

                        Common.WriteLogs("Function CreateMailchimpSegment Error: " + errorContent);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function GetTemplatesFromMailchimp Error: " + ex.Message);
            }
        }

        #endregion

        #region -- Async templates into db --

        private async Task CheckTemplateExistAsync(Templates template)
        {
            try
            {
                // Query to check if McID already exists in tbl_Mc_Templates
                string checkQuery = "SELECT ID FROM tbl_Mc_Templates WHERE Slug = @Slug";

                List<SqlParameter> checkParams = new List<SqlParameter>
                {
                    new SqlParameter("@Slug", template.Slug)
                };

                // Execute the query asynchronously to fetch the McID if it exists
                var result = await Common.ExecuteQueryAsync(checkQuery, checkParams);

                // If the template exists (result has rows)
                if (result != null && result.Rows.Count > 0)
                {
                    var existingID = Guid.TryParse(result.Rows[0]["ID"]?.ToString(), out var tempID) ? tempID : Guid.Empty;

                    // Update the existing template
                    await UpdateTemplateToDBAsync(template, existingID);
                }
                else
                {
                    // If the template does not exist, create a new one
                    await CreateNewTemplateToDBAsync(template);
                }
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function CheckTemplateExistAsync Error: " + ex.Message);
            }
        }

        private async Task CreateNewTemplateToDBAsync(Templates template)
        {
            try
            {
                // Insert query for creating a new template in the database
                var newID = Guid.NewGuid();

                string insertQuery = @"
                    INSERT INTO tbl_Mc_Templates 
                    (ID, Slug, Name, Code, Subject, FromEmail, FromName, Text, PublishName, PublishCode, 
                    PublishSubject, PublishFromEmail, PublishFromName, PublishText, PublishedAt, 
                    CreatedAt, UpdatedAt, IsBrokenTemplate, CreatedOn, ModifiedOn) 
                    VALUES 
                    (@ID, @Slug, @Name, @Code, @Subject, @FromEmail, @FromName, @Text, @PublishName, @PublishCode, 
                    @PublishSubject, @PublishFromEmail, @PublishFromName, @PublishText, @PublishedAt, 
                    @CreatedAt, @UpdatedAt, @IsBrokenTemplate, @CreatedOn, @ModifiedOn)";

                List<SqlParameter> insertParams = new List<SqlParameter>
                {
                    new SqlParameter("@ID", newID),
                    new SqlParameter("@Slug", template.Slug ?? (object)DBNull.Value),
                    new SqlParameter("@Name", template.Name ?? (object)DBNull.Value),
                    new SqlParameter("@Code", template.Code ?? (object)DBNull.Value),
                    new SqlParameter("@Subject", template.Subject ?? (object)DBNull.Value),
                    new SqlParameter("@FromEmail", template.FromEmail ?? (object)DBNull.Value),
                    new SqlParameter("@FromName", template.FromName ?? (object)DBNull.Value),
                    new SqlParameter("@Text", template.Text ?? (object)DBNull.Value),
                    new SqlParameter("@PublishName", template.PublishName ?? (object)DBNull.Value),
                    new SqlParameter("@PublishCode", template.PublishCode ?? (object)DBNull.Value),
                    new SqlParameter("@PublishSubject", template.PublishSubject ?? (object)DBNull.Value),
                    new SqlParameter("@PublishFromEmail", template.PublishFromEmail ?? (object)DBNull.Value),
                    new SqlParameter("@PublishFromName", template.PublishFromName ?? (object)DBNull.Value),
                    new SqlParameter("@PublishText", template.PublishText ?? (object)DBNull.Value),
                    new SqlParameter("@PublishedAt", template.PublishedAt ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedAt", template.CreatedAt ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedAt", template.UpdatedAt ?? (object)DBNull.Value),
                    new SqlParameter("@IsBrokenTemplate", template.IsBrokenTemplate ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedOn", DateTime.Now),
                    new SqlParameter("@ModifiedOn", DateTime.Now)
                };

                // Execute the insert query asynchronously
                await Common.ExecuteNonQueryAsync(insertQuery, insertParams);
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function CreateNewTemplateToDBAsync Error: " + ex.Message);
            }
        }

        private async Task UpdateTemplateToDBAsync(Templates template, Guid existingID)
        {
            try
            {
                // Update query for updating an existing template
                string updateQuery = @"
                    UPDATE tbl_Mc_Templates 
                    SET Name = @Name, Code = @Code, Subject = @Subject, FromEmail = @FromEmail, FromName = @FromName, Text = @Text, 
                        PublishName = @PublishName, PublishCode = @PublishCode, PublishSubject = @PublishSubject, PublishFromEmail = @PublishFromEmail, 
                        PublishFromName = @PublishFromName, PublishText = @PublishText, PublishedAt = @PublishedAt, CreatedAt = @CreatedAt, UpdatedAt = @UpdatedAt, 
                        IsBrokenTemplate = @IsBrokenTemplate, ModifiedOn = @ModifiedOn
                    WHERE ID = @ID";

                List<SqlParameter> updateParams = new List<SqlParameter>
                {
                    new SqlParameter("@ID", existingID),
                    new SqlParameter("@Name", template.Name ?? (object)DBNull.Value),
                    new SqlParameter("@Code", template.Code ?? (object)DBNull.Value),
                    new SqlParameter("@Subject", template.Subject ?? (object)DBNull.Value),
                    new SqlParameter("@FromEmail", template.FromEmail ?? (object)DBNull.Value),
                    new SqlParameter("@FromName", template.FromName ?? (object)DBNull.Value),
                    new SqlParameter("@Text", template.Text ?? (object)DBNull.Value),
                    new SqlParameter("@PublishName", template.PublishName ?? (object)DBNull.Value),
                    new SqlParameter("@PublishCode", template.PublishCode ?? (object)DBNull.Value),
                    new SqlParameter("@PublishSubject", template.PublishSubject ?? (object)DBNull.Value),
                    new SqlParameter("@PublishFromEmail", template.PublishFromEmail ?? (object)DBNull.Value),
                    new SqlParameter("@PublishFromName", template.PublishFromName ?? (object)DBNull.Value),
                    new SqlParameter("@PublishText", template.PublishText ?? (object)DBNull.Value),
                    new SqlParameter("@PublishedAt", template.PublishedAt ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedAt", template.CreatedAt ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedAt", template.UpdatedAt ?? (object)DBNull.Value),
                    new SqlParameter("@IsBrokenTemplate", template.IsBrokenTemplate ?? (object)DBNull.Value),
                    new SqlParameter("@ModifiedOn", DateTime.Now)
                };

                // Execute the update query asynchronously
                await Common.ExecuteNonQueryAsync(updateQuery, updateParams);
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function UpdateTemplateToDBAsync Error: " + ex.Message);
            }
        }

        #endregion

        #endregion

        #region -- Emails --

        private async Task GetCampaignsInDB()
        {
            try
            {
                string checkQuery = "SELECT TOP 10 McID, DistributeID, ActSubject, TemplateContent " +
                    "FROM [v_Mc_Campaigns] " +
                    "ORDER BY SendTime desc";

                var result = await Common.ExecuteQueryAsync(checkQuery);

                if (result != null && result.Rows.Count > 0)
                {
                    foreach (DataRow row in result.Rows)
                    {
                        CampaignEmails email = ParseCampaignRow(row);

                        await GetEmailFromMailchimp(email);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function GetAllCampaignsInDB Error: " + ex.Message);
            }
        }

        private CampaignEmails ParseCampaignRow(DataRow row)
        {
            var newEmail = new CampaignEmails
            {
                DistributeID = Guid.TryParse(row["DistributeID"]?.ToString(), out var tempDistributeID) ? tempDistributeID : Guid.Empty,
                McCampaignID = row["McID"]?.ToString(),
                Subject = row["ActSubject"]?.ToString(),
                Content = row["TemplateContent"]?.ToString()
            };

            return newEmail;
        }

        private async Task GetEmailFromMailchimp(CampaignEmails email)
        {
            try
            {
                var offset = 0;
                var count = 1000; // Retrieve up to 1000 emails per request
                bool moreData = true;

                while (moreData)
                {
                    var url = $"reports/{email.McCampaignID}/email-activity?count={count}&offset={offset}";

                    var response = await Common.ExecuteHttpRequestWithLogging(client => client.GetAsync(url));

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        JObject jsonResponse = JObject.Parse(content);
                        var emails = jsonResponse["emails"];

                        foreach (var item in emails)
                        {
                            var list_id = item["list_id"]?.Value<string>();
                            var email_id = item["email_id"]?.Value<string>();
                            var email_address = item["email_address"]?.Value<string>();

                            email.McEmailID = email_id;
                            email.McListID = list_id;
                            email.ToEmail = email_address;

                            var emailID = await CreateNewEmailToDBAsync(email);

                            if (emailID != Guid.Empty)
                            {
                                foreach (var activity in item["activity"] as JArray)
                                {
                                    var timestamp = DateTime.TryParse(activity["timestamp"]?.Value<string>(), out var parsedTimestamp)
                                        ? parsedTimestamp : (DateTime?)null;

                                    var newActivity = new CampaignEmailActivities
                                    {
                                        EmailID = emailID,
                                        Action = activity["action"]?.Value<string>(),
                                        Timestamp = timestamp,
                                        Ip = activity["ip"]?.Value<string>(),
                                        ClickUrl = activity["url"]?.Value<string>()
                                    };

                                    await CreateNewEmailActivitiesToDBAsync(newActivity);
                                }
                            }
                        }
  
                        moreData = emails.Count() == count;
                        offset += count; // Move to the next batch of emails
                    }
                    else
                    {
                        moreData = false; // Stop if there's an error
                    }
                }
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function GetEmailFromMailchimp Error: " + ex.Message);
            }
        }

        private async Task<Guid> CreateNewEmailToDBAsync(CampaignEmails email)
        {
            try
            {
                string checkQuery = "SELECT ID FROM tbl_Mc_CampaignEmails " +
                    "WHERE McEmailID = @McEmailID AND McCampaignID = @McCampaignID";

                List<SqlParameter> checkParams = new List<SqlParameter>
                {
                    new SqlParameter("@McEmailID", email.McEmailID),
                    new SqlParameter("@McCampaignID", email.McCampaignID)
                };

                var result = await Common.ExecuteQueryAsync(checkQuery, checkParams);

                if (result != null && result.Rows.Count > 0)
                {
                    var existingID = Guid.TryParse(result.Rows[0]["ID"]?.ToString(), out var tempID) ? tempID : Guid.Empty;

                    return existingID;
                }
                else
                {
                    var newID = Guid.NewGuid();

                    string insertEmailQuery = @"
                        INSERT INTO tbl_Mc_CampaignEmails (ID, DistributeID, McEmailID, McCampaignID, McListID, 
                        Subject, Content, ToEmail, Status, CreatedOn, ModifiedOn)
                        VALUES (@ID, @DistributeID, @McEmailID, @McCampaignID, @McListID, @Subject, @Content, 
                        @ToEmail, @Status, @CreatedOn, @ModifiedOn)";

                    List<SqlParameter> emailParams = new List<SqlParameter>
                    {
                        new SqlParameter("@ID", newID),
                        new SqlParameter("@DistributeID", email.DistributeID),
                        new SqlParameter("@McEmailID", email.McEmailID ?? (object)DBNull.Value),
                        new SqlParameter("@McCampaignID", email.McCampaignID ?? (object)DBNull.Value),
                        new SqlParameter("@McListID", email.McListID ?? (object)DBNull.Value),
                        new SqlParameter("@Subject", email.Subject ?? (object)DBNull.Value),
                        new SqlParameter("@Content", email.Content ?? (object)DBNull.Value),
                        new SqlParameter("@ToEmail", email.ToEmail ?? (object)DBNull.Value),
                        new SqlParameter("@Status", "1"),
                        new SqlParameter("@CreatedOn", DateTime.Now),
                        new SqlParameter("@ModifiedOn", DateTime.Now)
                    };

                    await Common.ExecuteNonQueryAsync(insertEmailQuery, emailParams);

                    return newID;
                }
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function CreateNewEmailToDBAsync Error: " + ex.Message);

                return Guid.Empty;
            }
        }

        private async Task CreateNewEmailActivitiesToDBAsync(CampaignEmailActivities emailAct)
        {
            try
            {
                // Query to check for an existing identical action
                string checkQuery = @"
                    SELECT ID FROM [tbl_Mc_CampaignEmailActivities]
                    WHERE EmailID = @EmailID
                    AND Action = @Action
                    AND Timestamp = @Timestamp
                    AND Ip = @Ip
                    AND (@ClickUrl IS NULL OR ClickUrl = @ClickUrl)";

                // Check parameters, with handling for possible null ClickUrl
                List<SqlParameter> checkParams = new List<SqlParameter>
                {
                    new SqlParameter("@EmailID", emailAct.EmailID),
                    new SqlParameter("@Action", emailAct.Action),
                    new SqlParameter("@Timestamp", emailAct.Timestamp),
                    new SqlParameter("@Ip", emailAct.Ip),
                    new SqlParameter("@ClickUrl", emailAct.ClickUrl ?? (object)DBNull.Value)
                };

                var result = await Common.ExecuteQueryAsync(checkQuery, checkParams);

                // Insert if no duplicate entry is found
                if (result == null || result.Rows.Count == 0)
                {
                    var newID = Guid.NewGuid();

                    string insertEmailActQuery = @"
                        INSERT INTO tbl_Mc_CampaignEmailActivities (ID, EmailID, Action, ClickUrl, Ip, Timestamp, CreatedOn, ModifiedOn)
                        VALUES (@ID, @EmailID, @Action, @ClickUrl, @Ip, @Timestamp, @CreatedOn, @ModifiedOn)";

                    List<SqlParameter> emailActParams = new List<SqlParameter>
                    {
                        new SqlParameter("@ID", newID),
                        new SqlParameter("@EmailID", emailAct.EmailID),
                        new SqlParameter("@Action", emailAct.Action ?? (object)DBNull.Value),
                        new SqlParameter("@Timestamp", emailAct.Timestamp ?? (object)DBNull.Value),
                        new SqlParameter("@ClickUrl", emailAct.ClickUrl ?? (object)DBNull.Value),
                        new SqlParameter("@Ip", emailAct.Ip ?? (object)DBNull.Value),
                        new SqlParameter("@CreatedOn", DateTime.UtcNow),
                        new SqlParameter("@ModifiedOn", DateTime.UtcNow)
                    };

                    await Common.ExecuteNonQueryAsync(insertEmailActQuery, emailActParams);
                }
            }
            catch (Exception ex)
            {
                Common.WriteLogs("Function CreateNewEmailActivitiesToDBAsync Error: " + ex.Message);
            }
        }

        #endregion

        #endregion
    }
}
