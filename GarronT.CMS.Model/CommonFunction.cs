using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.IO;

using System.Net;


using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;


namespace GarronT.CMS.Model
{
    public class CommonFunctions
    {


        CompassEntities db = new CompassEntities();

        public static int GetLoggedUserID(string userName)
        {

            var userid = new CompassEntities().tblUsers.Where(x => x.UserName == userName).Select(x => x.UserID).FirstOrDefault();
            return int.Parse(userid.ToString());
        }
        public string GetFileContents(string FullPath)
        {
            WebClient webClient = new WebClient();
            return webClient.DownloadString(FullPath);
        }
        //get server date
        public DateTime ServerDate()
        {

            DateTime CurrentTime = new DateTime();


            var dbdate = db.Database.SqlQuery<DateTime>("select getdate()").FirstOrDefault<DateTime>();


            CurrentTime = Convert.ToDateTime(dbdate);

            return CurrentTime;

        }


        public static string strActiveRecordExists = "Record already exists in active record.";
        public static string strDeactiveRecordExists = "Record already exists in deactivated record.";
        public static string strDefaultAdd = "Unable to add record.";
        public static string strRecordActivated = "Record activated.";
        public static string strRecordDeactivated = "Record deactivated.";
        public static string strRecordDeleted = "Record deleted.";
        public static string strRecordDeactivatingError = "Unable to deactivate record.";
        public static string strRecordActivatingError = "Unable to Activate record.";
        public static string strRecordCreated = "Record created.";
        public static string strRecordUpdated = "Record updated.";
        public static string strRecordDeactivatingExist = "This record can not be deactivated. It is already in use.";

        public static string SMS = "SMS";
        public static string Email = "Email";
        public static string Music = "Music";
        public static string CourtesySMS = "Courtesy SMS";
        public static string PreRecordedFile = "Pre Recorded File";
        public static string Operator = "Operator";

        public static string CompanyAdmin = "Company Admin";
        public static string CompanyAgent = "Agent";
        public static string SuperAdmin = "Super Admin";
        public static string strPlanExpired = "Base Plan already Expired";


        public static long? CompanyID;
        public static long? UserID;
        public static string filepath;

        //convert list to datatable
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }


        public static DataTable LINQToDataTable<T>(IEnumerable<T> collection)
        {

            DataTable dt = new DataTable();

            Type t = typeof(T);

            PropertyInfo[] pia = t.GetProperties();

            //Create the columns in the DataTable

            foreach (PropertyInfo pi in pia)
            {

                Type propType = pi.PropertyType;

                if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {

                    propType = pi.PropertyType.GetGenericArguments()[0];

                    dt.Columns.Add(pi.Name, propType);

                }

                else
                {

                    dt.Columns.Add(pi.Name, pi.PropertyType);

                }

            }

            //Populate the table

            foreach (object item in collection)
            {

                DataRow dr = dt.NewRow();

                dr.BeginEdit();

                foreach (PropertyInfo pi in pia)
                {

                    if (pi.GetValue(item, null) == null)
                    {

                        dr[pi.Name] = DBNull.Value;

                    }

                    else
                    {

                        dr[pi.Name] = pi.GetValue(item, null);

                    }

                }

                dr.EndEdit();

                dt.Rows.Add(dr);

            }

            return dt;

        }


        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }


        public static string Encryptdata(string password)
        {
            string strmsg = string.Empty;
            byte[] encode = new byte[password.Length];
            encode = Encoding.UTF8.GetBytes(password);
            strmsg = Convert.ToBase64String(encode);
            return strmsg;
        }

        public static string Decryptdata(string encryptpwd)
        {
            string decryptpwd = string.Empty;
            UTF8Encoding encodepwd = new UTF8Encoding();
            Decoder Decode = encodepwd.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encryptpwd);
            int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            decryptpwd = new String(decoded_char);
            return decryptpwd;
        }

        public string GeneratePassword()
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            string allowedNonAlphaNum = "!@#$%&*?";
            Random rd = new Random();

            if (1 > 6 || 6 <= 0 || 1 < 0)
                throw new ArgumentOutOfRangeException();

            char[] pass = new char[6];
            int[] pos = new int[6];
            int i = 0, j = 0, temp = 0;
            bool flag = false;

            //Random the position values of the pos array for the string Pass
            while (i < 6 - 1)
            {
                j = 0;
                flag = false;
                temp = rd.Next(0, 6);
                for (j = 0; j < 6; j++)
                    if (temp == pos[j])
                    {
                        flag = true;
                        j = 6;
                    }

                if (!flag)
                {
                    pos[i] = temp;
                    i++;
                }
            }

            //Random the AlphaNumericChars
            for (i = 0; i < 6 - 1; i++)
                pass[i] = allowedChars[rd.Next(0, allowedChars.Length)];

            //Random the NonAlphaNumericChars
            for (i = 6 - 1; i < 6; i++)
                pass[i] = allowedNonAlphaNum[rd.Next(0, allowedNonAlphaNum.Length)];

            //Set the sorted array values by the pos array for the rigth posistion
            char[] sorted = new char[6];
            for (i = 0; i < 6; i++)
                sorted[i] = pass[pos[i]];

            string Pass = new String(sorted);

            return Pass;
        }


        #region Create Short Url

        //private const string key = "AIzaSyAlDLwyn_uVOVOYmClToYk9h-jLSUZ9j8Y";
        //private string key = ConfigurationManager.AppSettings["ShortURLKey"].ToString();

        public string urlShorter(string url)
        {
            string key = ConfigurationManager.AppSettings["ShortURLKey"].ToString();
            string finalURL = "";
            string post = "{\"longUrl\": \"" + url + "\"}";
            string shortUrl = url;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url?key=" + key);
            try
            {
                request.ServicePoint.Expect100Continue = false;
                request.Method = "POST";
                request.ContentLength = post.Length;
                request.ContentType = "application/json";
                request.Headers.Add("Cache-Control", "no-cache");
                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] postBuffer = Encoding.ASCII.GetBytes(post);
                    requestStream.Write(postBuffer, 0, postBuffer.Length);
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader responseReader = new StreamReader(responseStream))
                        {
                            string json = responseReader.ReadToEnd();
                            var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(json);
                            finalURL = Convert.ToString(dict["id"]);// Regex.Match(json, @"""id"": ?""(?.+)""").Groups["id"].Value;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // if Google's URL Shortener is down...
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
            return finalURL;
        }

        #endregion

    }

    public static class StringExtension
    {
        public static string TrimmedOrDefault(this string str)
        {
            if (string.IsNullOrEmpty(str)) //or if (string.IsNullOrWhitespace(str))
            {
                return str = null;
            }
            else
            {
                return str.Trim();
            }
        }

    }



    public class UserDetails
    {

        public long GetUserId(string currentUserId)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            // string currentUserId = User.Identity.GetUserId();
            var currentUser = objCompassEntities.tblUsers.FirstOrDefault(x => x.aspnet_UserId == new Guid(currentUserId));
            long userId = currentUser.UserID;

            objCompassEntities.Dispose();
            return userId;
        }


    }



    public partial class CompassEntities : DbContext
    {
        public CompassEntities(string ConnectionString)
            : base(ConnectionString)
        {
            this.SetCommandTimeOut(300);
        }

        public void SetCommandTimeOut(int Timeout)
        {
            var objectContext = (this as IObjectContextAdapter).ObjectContext;
            objectContext.CommandTimeout = Timeout;
        }
    }
}
