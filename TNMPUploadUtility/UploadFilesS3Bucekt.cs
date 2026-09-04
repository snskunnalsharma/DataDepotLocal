
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using GarronT.CMS.Model;
using Newtonsoft.Json;
using System.Net;
using RestSharp;
using System.IO;

namespace TNMPUploadUtility
{
    internal class UploadFilesS3Bucekt
    {
        private const string PathFilter = "MobileUploadData";
        private const string WebUrl = @"C:\inetpub\vhosts\datadepotonline.com\httpdocs\";

        // Main entry: pending uploads first (batched). Optional HTTP validation of existing URLs.
        public void ProcessCloudUrls()
        {
            int batchSize = GetConfigInt("CloudUploadBatchSize", 5000);
            bool validateExisting = GetConfigBool("ValidateExistingCloudUrls", false);

            ErrorLogFile.writeStatus("ProcessCloudUrls started batchSize=" + batchSize +
                " validateExisting=" + validateExisting + " - " + DateTime.Now.ToShortTimeString());
            Console.WriteLine("ProcessCloudUrls batchSize=" + batchSize + " validateExisting=" + validateExisting);

            UploadPendingCloudUrls(batchSize);

            if (validateExisting)
            {
                ValidateAndFixBrokenCloudUrlsBatch(batchSize);
            }
            else
            {
                ErrorLogFile.writeStatus("Skipped existing URL HTTP validation (ValidateExistingCloudUrls=false)");
                Console.WriteLine("Skipped existing URL HTTP validation.");
            }
        }
        public void getFilesPath()
        {
            try
            {
                ErrorLogFile.writeStatus("Getting files path from DB Table" + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Getting files path from DB Table");
                CompassEntities compassEntities = new CompassEntities();

                // Restored on 24 Aug 2026 - original filter by CreatedOn year range
                DateTime startDate = new DateTime(2026, 1, 1);
                DateTime endDate = new DateTime(2027, 1, 1);

                var filesPath = compassEntities.TblProjectFieldDataAttachments
                    .Where(o =>
                        o.strFilePath != null &&
                        (string.IsNullOrEmpty(o.strCloudUrl)) &&
                        o.CreatedOn != null &&
                        o.CreatedOn >= startDate &&
                        o.CreatedOn < endDate
                    )
                    .ToList();

                // Removed on 24 Aug 2026 - previous 10122 empty-only filter (not enough for broken cloud links)
                //var filesPath = compassEntities.TblProjectFieldDataAttachments
                //    .Where(o =>
                //        o.strFilePath != null &&
                //        string.IsNullOrEmpty(o.strCloudUrl) &&
                //        o.strFilePath.Contains(@"MobileUploadData\10122\")
                //    )
                //    .ToList();

                //var filesPath = compassEntities.TblProjectFieldDataAttachments.Where(o => o.CreatedOn.Value.Year == 2023).ToList();
                string _webUrl = "C:\\inetpub\\vhosts\\datadepotonline.com\\httpdocs\\"; // Server Image defualt file path
               // string _webUrl = "C:\\inetpub\\vhosts\\datadepotonline.com\\ts2.datadepotonline.com\\"; // Server Image defualt file path

                //string _webUrl = "D:\\DataDepot\\GarronT.CMS.UI\\"; //Local path for development purpose.
                ErrorLogFile.writeStatus("Number of old files:- " + filesPath.Count() + " : " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Number of old files:- " + filesPath.Count() + " : " + DateTime.Now.ToShortTimeString());
                int count = 0;
                int failCount = 0;
                foreach (var objfilespath in filesPath)
                {
                    try
                    {
                        string oldFilePath = string.Empty;
                        if (!string.IsNullOrEmpty(objfilespath.strFilePath))
                        {
                            oldFilePath = ResolveLocalFilePath(objfilespath.strFilePath, _webUrl);
                            Console.WriteLine("Oldfile" + oldFilePath);

                            string cluodFilePath = UploadFileS3Bucket(oldFilePath);
  
                            if (!string.IsNullOrEmpty(cluodFilePath))
                            {
                                objfilespath.strCloudUrl = cluodFilePath;
                                compassEntities.SaveChanges();
                                cluodFilePath = null;
                                count++;
                                if (count % 100 == 0)
                                {
                                    ErrorLogFile.writeStatus("getFilesPath progress uploaded=" + count);
                                }
                            }
                            else
                            {
                                failCount++;
                                ErrorLogFile.writeStatus("getFilesPath upload failed ID=" + objfilespath.ID + " path=" + oldFilePath);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        ErrorLogFile.writeStatus("getFilesPath have caught an exception " + ex.Message.ToString() + DateTime.Now.ToShortTimeString());
                    }
                }
                ErrorLogFile.writeStatus("Number of uploaded files => " + count + " failed=" + failCount + " : " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Number of uploaded files => " + count + " failed=" + failCount + " : " + DateTime.Now.ToShortTimeString());
            }
            catch (Exception ex)
            {
                ErrorLogFile.writeStatus("getFilesPath have caught an exception " + ex.Message.ToString() + DateTime.Now.ToShortTimeString());
                //Console.WriteLine("getFilesPath have caught an exception " + ex.Message.ToString());
            }
        }

        // Phase 1 (priority): upload rows with empty strCloudUrl — batched per run
        private void UploadPendingCloudUrls(int batchSize)
        {
            CompassEntities compassEntities = new CompassEntities();

            var pendingQuery = compassEntities.TblProjectFieldDataAttachments
                .Where(o =>
                    o.Active == 1 &&
                    o.strFilePath != null &&
                    o.strFilePath.Contains(PathFilter) &&
                    (o.strCloudUrl == null || o.strCloudUrl == "")
                );

            int totalPending = pendingQuery.Count();
            var batch = pendingQuery
                .OrderBy(o => o.ID)
                .Take(batchSize)
                .ToList();

            ErrorLogFile.writeStatus("Pending cloud URLs total=" + totalPending +
                " processingThisRun=" + batch.Count + " - " + DateTime.Now.ToShortTimeString());
            Console.WriteLine("Pending total=" + totalPending + " processing this run=" + batch.Count);

            int uploaded = 0;
            int missing = 0;
            int failed = 0;

            foreach (var item in batch)
            {
                try
                {
                    string localPath = ResolveLocalFilePath(item.strFilePath, WebUrl).Replace('/', '\\');
                    if (!File.Exists(localPath))
                    {
                        missing++;
                        continue;
                    }

                    string newCloudUrl = UploadFileS3Bucket(localPath);
                    if (!string.IsNullOrEmpty(newCloudUrl))
                    {
                        item.strCloudUrl = newCloudUrl;
                        compassEntities.SaveChanges();
                        uploaded++;
                        if (uploaded % 100 == 0)
                        {
                            ErrorLogFile.writeStatus("Pending upload progress uploaded=" + uploaded +
                                " missing=" + missing + " failed=" + failed);
                            Console.WriteLine("Uploaded " + uploaded + " / " + batch.Count);
                        }
                    }
                    else
                    {
                        failed++;
                        ErrorLogFile.writeStatus("Pending upload failed ID=" + item.ID);
                    }
                }
                catch (Exception ex)
                {
                    failed++;
                    ErrorLogFile.writeStatus("Pending upload exception ID=" + item.ID + " " + ex.Message);
                }
            }

            int remaining = Math.Max(0, totalPending - uploaded);
            string summary = "Pending upload done. totalPending=" + totalPending +
                " processed=" + batch.Count + " uploaded=" + uploaded +
                " missing=" + missing + " failed=" + failed +
                " approxRemaining=" + remaining + " - " + DateTime.Now.ToShortTimeString();
            ErrorLogFile.writeStatus(summary);
            Console.WriteLine(summary);
        }

        // Phase 2 (optional): validate existing cloud URLs and re-upload broken ones — batched per run
        private void ValidateAndFixBrokenCloudUrlsBatch(int batchSize)
        {
            CompassEntities compassEntities = new CompassEntities();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var withUrlQuery = compassEntities.TblProjectFieldDataAttachments
                .Where(o =>
                    o.Active == 1 &&
                    o.strFilePath != null &&
                    o.strFilePath.Contains(PathFilter) &&
                    o.strCloudUrl != null &&
                    o.strCloudUrl != ""
                );

            int totalWithUrl = withUrlQuery.Count();
            var batch = withUrlQuery
                .OrderBy(o => o.ID)
                .Take(batchSize)
                .ToList();

            ErrorLogFile.writeStatus("URL validation total=" + totalWithUrl +
                " checkingThisRun=" + batch.Count + " - " + DateTime.Now.ToShortTimeString());
            Console.WriteLine("URL validation total=" + totalWithUrl + " checking this run=" + batch.Count);

            int checkedCount = 0;
            int okCount = 0;
            int brokenCount = 0;
            int reuploadedCount = 0;
            int missingCount = 0;
            int failedCount = 0;

            foreach (var item in batch)
            {
                try
                {
                    checkedCount++;
                    string cloudUrl = (item.strCloudUrl ?? "").Trim();

                    if (IsCloudUrlReachable(cloudUrl))
                    {
                        okCount++;
                        if (checkedCount % 500 == 0)
                        {
                            ErrorLogFile.writeStatus("Validation progress checked=" + checkedCount +
                                " ok=" + okCount + " broken=" + brokenCount + " reuploaded=" + reuploadedCount);
                            Console.WriteLine("Checked " + checkedCount + " ok=" + okCount + " broken=" + brokenCount);
                        }
                        continue;
                    }

                    brokenCount++;
                    string localPath = ResolveLocalFilePath(item.strFilePath, WebUrl).Replace('/', '\\');
                    if (!File.Exists(localPath))
                    {
                        missingCount++;
                        ErrorLogFile.writeStatus("Broken URL, local missing ID=" + item.ID);
                        continue;
                    }

                    string newCloudUrl = UploadFileS3Bucket(localPath);
                    if (!string.IsNullOrEmpty(newCloudUrl))
                    {
                        item.strCloudUrl = newCloudUrl;
                        compassEntities.SaveChanges();
                        reuploadedCount++;
                    }
                    else
                    {
                        failedCount++;
                        ErrorLogFile.writeStatus("Broken URL re-upload failed ID=" + item.ID);
                    }
                }
                catch (Exception ex)
                {
                    failedCount++;
                    ErrorLogFile.writeStatus("Validation item exception ID=" + item.ID + " " + ex.Message);
                }
            }

            string summary = "URL validation done. totalWithUrl=" + totalWithUrl +
                " checked=" + checkedCount + " ok=" + okCount + " broken=" + brokenCount +
                " reuploaded=" + reuploadedCount + " missing=" + missingCount +
                " failed=" + failedCount + " - " + DateTime.Now.ToShortTimeString();
            ErrorLogFile.writeStatus(summary);
            Console.WriteLine(summary);
        }

        // Kept for backward compatibility — delegates to ProcessCloudUrls
        public void ValidateAndFixBrokenCloudUrls()
        {
            ProcessCloudUrls();
        }

        private static int GetConfigInt(string key, int defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            int parsed;
            return int.TryParse(value, out parsed) && parsed > 0 ? parsed : defaultValue;
        }

        private static bool GetConfigBool(string key, bool defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }
            return value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                   value.Equals("1", StringComparison.OrdinalIgnoreCase) ||
                   value.Equals("yes", StringComparison.OrdinalIgnoreCase);
        }

        // Added on 24 Aug 2026 - shared local path resolve
        private static string ResolveLocalFilePath(string dbFilePath, string webUrl)
        {
            string oldFilePath = dbFilePath ?? string.Empty;
            if (oldFilePath.Contains("..\\..\\"))
            {
                oldFilePath = oldFilePath.Replace("..\\..\\", webUrl);
            }
            if (oldFilePath.Contains("~"))
            {
                oldFilePath = oldFilePath.Replace("~\\", webUrl);
            }
            oldFilePath = oldFilePath.Replace('\\', '/').Trim();
            return oldFilePath;
        }

        // Added on 24 Aug 2026 - HTTP check whether cloud URL still serves the file
        private static bool IsCloudUrlReachable(string cloudUrl)
        {
            if (string.IsNullOrWhiteSpace(cloudUrl))
            {
                return false;
            }
            if (!cloudUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (cloudUrl.IndexOf("failed", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return false;
            }

            try
            {
                if (TryHttpStatus(cloudUrl, "HEAD", out int headStatus))
                {
                    if (headStatus >= 200 && headStatus < 400)
                    {
                        return true;
                    }
                    // Some hosts reject HEAD; fall through to GET
                    if (headStatus != 405 && headStatus != 403 && headStatus < 500)
                    {
                        return false;
                    }
                }

                if (TryHttpStatus(cloudUrl, "GET", out int getStatus))
                {
                    return getStatus >= 200 && getStatus < 400;
                }
            }
            catch (Exception ex)
            {
                ErrorLogFile.writeStatus("IsCloudUrlReachable exception: " + ex.Message + " URL=" + cloudUrl);
            }

            return false;
        }

        // Added on 24 Aug 2026
        private static bool TryHttpStatus(string url, string method, out int statusCode)
        {
            statusCode = 0;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.Timeout = 15000;
            request.ReadWriteTimeout = 15000;
            request.AllowAutoRedirect = true;
            request.UserAgent = "TNMPUploadUtility-CloudUrlCheck/1.0";

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    statusCode = (int)response.StatusCode;
                    return true;
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse errorResponse = ex.Response as HttpWebResponse;
                if (errorResponse != null)
                {
                    statusCode = (int)errorResponse.StatusCode;
                    errorResponse.Close();
                    return true;
                }
                return false;
            }
        }

        public static string UploadFileS3Bucket(string filePath)
        {
            try
            {
                string path = @"" + filePath + "";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.DefaultConnectionLimit = 9999;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                var client = new RestClient("https://dddailyapi.mytiny.us/api/v1/");
                var request = new RestRequest("file-upload", Method.POST);
                request.AddParameter("token", "cCTIlSLsjvinXp9ye4UsBh");
                request.AddParameter("type", "public");
                request.AddFile("attachments", path);

                IRestResponse response = client.Execute(request);
                string jsonResponse = response.Content ?? string.Empty;

                // Check for upload failure — log only failures (not every API response)
                if (response.ResponseStatus != ResponseStatus.Completed || jsonResponse.Contains("failed to upload"))
                {
                    ErrorLogFile.writeStatus("Upload failed: " + Path.GetFileName(filePath) + " status=" + response.ResponseStatus);
                    return null;
                }

                var jsonData = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                var imgUrl = jsonData?.data?.ToString();

                if (string.IsNullOrWhiteSpace(imgUrl) || imgUrl.Contains("failed to upload"))
                {
                    ErrorLogFile.writeStatus("Invalid/empty image URL: " + Path.GetFileName(filePath));
                    return null;
                }

                return imgUrl;
            }
            catch (Exception ex)
            {
                ErrorLogFile.writeStatus("UploadFileS3Bucket exception: " + ex.Message + " file=" + Path.GetFileName(filePath));
                return null;
            }
        }


    }
}
