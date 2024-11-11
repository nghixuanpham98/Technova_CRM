using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;

namespace Technova_Mailchimp_Services
{
    public class Common
    {
        #region -- Connect DB (Async) --

        public static async Task<DataTable> ExecuteQueryAsync(string query, List<SqlParameter> parameters = null
            , CommandType commandType = CommandType.Text)
        {
            DataTable resultTable = new DataTable();

            using (var connection = new SqlConnection(Configs.Default.IVG_Connection))
            {
                try
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = commandType;

                        // Add parameters if any
                        if (parameters != null && parameters.Count > 0)
                        {
                            command.Parameters.AddRange(parameters.ToArray());
                        }

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            await Task.Run(() => adapter.Fill(resultTable));
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLogs("ExecuteQueryAsync Error: " + ex.Message);
                }
            }

            return resultTable;
        }

        public static async Task<int> ExecuteNonQueryAsync(string query, List<SqlParameter> parameters = null
            , CommandType commandType = CommandType.Text)
        {
            int rowsAffected = 0;

            using (var connection = new SqlConnection(Configs.Default.IVG_Connection))
            {
                try
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = commandType;

                        // Add parameters if any
                        if (parameters != null && parameters.Count > 0)
                        {
                            command.Parameters.AddRange(parameters.ToArray());
                        }

                        rowsAffected = await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    WriteLogs("ExecuteNonQueryAsync Error: " + ex.Message);
                }
            }

            return rowsAffected;
        }

        public static async Task<object> ExecuteScalarAsync(string query, List<SqlParameter> parameters = null
            , CommandType commandType = CommandType.Text)
        {
            object result = null;

            using (var connection = new SqlConnection(Configs.Default.IVG_Connection))
            {
                try
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.CommandType = commandType;

                        // Add parameters if any
                        if (parameters != null && parameters.Count > 0)
                        {
                            command.Parameters.AddRange(parameters.ToArray());
                        }

                        result = await command.ExecuteScalarAsync();
                    }
                }
                catch (Exception ex)
                {
                    WriteLogs("ExecuteScalarAsync Error: " + ex.Message);
                }
            }

            return result;
        }

        #endregion

        #region -- Execute HttpRequest With Logging --

        public static async Task<HttpResponseMessage> ExecuteHttpRequestWithLogging(Func<HttpClient, Task<HttpResponseMessage>> requestFunc)
        {
            try
            {
                using (var client = SetupHttpClient(await GetAPIKeyAsync(), await GetURLAsync()))
                {
                    var response = await requestFunc(client);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();

                        WriteLogs($"HTTP request error: {response.ReasonPhrase}. Details: {errorContent}");
                    }

                    return response;
                }
            }
            catch (Exception ex)
            {
                WriteLogs($"HTTP request failed. Exception: {ex.Message}");

                throw;
            }
        }

        #endregion

        #region -- Setup HttpClient --

        public static HttpClient SetupHttpClient(string apiKey, string baseUrl)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            var client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes("anystring:" + apiKey)));

            return client;
        }

        #endregion

        #region -- Get URL (Async) --

        public static async Task<string> GetURLAsync()
        {
            try
            {
                // Define the query and any necessary parameters
                string query = "SELECT [Value] FROM [tbl_Mc_Configs] WHERE Name = @name";

                var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@name", "dc")
                    };

                // Execute the query asynchronously using ExecuteQueryAsync
                var data = await ExecuteQueryAsync(query, parameters);

                var rs = data != null && data.Rows.Count == 1 ? data.Rows[0][0].ToString() : "";

                var mailchimpUrl = "https://" + rs + ".api.mailchimp.com/3.0/";

                return mailchimpUrl;
            }
            catch (Exception ex)
            {
                WriteLogs("Function GetDCAsync Error: " + ex.Message);
                return "";
            }
        }

        #endregion

        #region -- Get API Key (Async) --

        public static async Task<string> GetAPIKeyAsync()
        {
            try
            {
                string query = "SELECT [Value] FROM [tbl_Mc_Configs] WHERE Name = @name";
                var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@name", "apikey")
                    };

                // Execute the query asynchronously using ExecuteQueryAsync
                var data = await ExecuteQueryAsync(query, parameters);

                return data != null && data.Rows.Count == 1 ? data.Rows[0][0].ToString() : "";
            }
            catch (Exception ex)
            {
                WriteLogs("Function GetAPIKeyAsyncAsync Error: " + ex.Message);
                return "";
            }
        }

        #endregion

        #region -- Get Max Contact (Async) --

        public static async Task<string> GetMaxContactAsync()
        {
            try
            {
                string query = "SELECT [Value] FROM [tbl_Mc_Configs] WHERE Name = @name";
                var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@name", "max-contact")
                    };

                // Execute the query asynchronously using ExecuteQueryAsync
                var data = await ExecuteQueryAsync(query, parameters);

                return data != null && data.Rows.Count == 1 ? data.Rows[0][0].ToString() : "";
            }
            catch (Exception ex)
            {
                WriteLogs("Function GetMaxContactAsync Error: " + ex.Message);
                return "";
            }
        }

        #endregion

        #region -- Get URL Mandrill (Async) --

        public static string GetURLMandrill()
        {
            try
            {
                return "https://mandrillapp.com/api/1.0/";
            }
            catch (Exception ex)
            {
                WriteLogs($"Function GetURLMandrill Error: {ex.Message}");
                return string.Empty;
            }
        }

        #endregion

        #region -- Get Mandrill API Key (Async) --

        public static async Task<string> GetMandrillAPIKeyAsync()
        {
            try
            {
                string query = "SELECT [Value] FROM [tbl_Mc_Configs] WHERE Name = @name";
                var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@name", "md-apikey")
                    };

                // Execute the query asynchronously using ExecuteQueryAsync
                var data = await ExecuteQueryAsync(query, parameters);

                return data != null && data.Rows.Count == 1 ? data.Rows[0][0].ToString() : "";
            }
            catch (Exception ex)
            {
                WriteLogs("Function GetMandrillAPIKeyAsync Error: " + ex.Message);
                return "";
            }
        }

        #endregion

        #region -- Write Logs --

        public static void WriteLogs(string msg)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "Mailchimps");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filepath = Path.Combine(path, "Mailchimp_" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt");

            lock (typeof(File))
            {
                using (StreamWriter sw = new StreamWriter(filepath, true)) // Append text if file exists
                {
                    sw.WriteLine($"{DateTime.Now} - {msg}");
                }
            }
        }

        #endregion

        #region -- MD5 Hash --

        public static async Task<string> ComputeMD5HashAsync(string input)
        {
            return await Task.Run(() =>
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    // Convert the byte array to hexadecimal string
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hashBytes)
                    {
                        sb.Append(b.ToString("x2"));
                    }

                    return sb.ToString();
                }
            });
        }

        #endregion
    }
}
