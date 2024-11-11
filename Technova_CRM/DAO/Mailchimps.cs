using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Technova_CRM.DAO
{
    public class Mailchimps
    {
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
    }
}