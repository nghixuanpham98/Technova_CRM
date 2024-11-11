using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Technova_CRM.Models.CustomModels;
using Technova_CRM.Models.EntityModels;

namespace Technova_CRM.DAO
{
    public class DAO_Common
    {
        #region -- Connect DB (Async) --

        public static async Task<DataTable> ExecuteQueryAsync(string query, List<SqlParameter> parameters = null
            , CommandType commandType = CommandType.Text)
        {
            DataTable resultTable = new DataTable();

            using (var connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectDB"]))
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
                }
            }

            return resultTable;
        }

        public static async Task<int> ExecuteNonQueryAsync(string query, List<SqlParameter> parameters = null
            , CommandType commandType = CommandType.Text)
        {
            int rowsAffected = 0;

            using (var connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectDB"]))
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
                }
            }

            return rowsAffected;
        }

        public static async Task<object> ExecuteScalarAsync(string query, List<SqlParameter> parameters = null
            , CommandType commandType = CommandType.Text)
        {
            object result = null;

            using (var connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectDB"]))
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
                }
            }

            return result;
        }

        #endregion

        #region -- Get URL Async --

        public static async Task<string> GetURLAsync()
        {
            try
            {
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
                return "";
            }
        }

        #endregion

        #region -- Get API Key (Async) --

        public static async Task<string> GetApiKeyAsync()
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
                return "";
            }
        }

        #endregion

        #region -- Get Mandrillapp API Key (Async) --

        public static async Task<string> GetMandrillApiKeyAsync()
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
                return "";
            }
        }

        #endregion

        #region -- Get Subscriber Hash --

        public static string GetSubscriberHash(string email)
        {
            // Convert email to lowercase as required by Mailchimp
            var lowercaseEmail = email.ToLower();

            // Compute the MD5 hash of the email
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(lowercaseEmail);
                var hashBytes = md5.ComputeHash(inputBytes);

                // Convert hash bytes to a hexadecimal string
                var sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        #endregion
    }
}