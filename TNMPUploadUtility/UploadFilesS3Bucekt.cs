
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
        private const string PathFilter = @"MobileUploadData\10076\";
        private const string PathFilterAlt = @"MobileUploadData/10076/";
        private const string WebUrl = @"C:\inetpub\vhosts\datadepotonline.com\httpdocs\";

        private enum CloudUrlStatus
        {
            Ok,
            AccessDenied,
            NotFound,
            Broken
        }

        // Main entry: pending uploads first, then AccessDenied/broken URL re-upload.
        public void ProcessCloudUrls()
        {
            int dbPageSize = GetConfigInt("CloudUploadDbPageSize", 5000);
            bool validateExisting = GetConfigBool("ValidateExistingCloudUrls", true);

            string projectFolder = Path.Combine(WebUrl, "MobileUploadData", "10076");
            if (!Directory.Exists(projectFolder))
            {
                ErrorLogFile.writeStatus("Project folder does not exist: " + projectFolder);
                Console.WriteLine("Project folder does not exist: " + projectFolder);
                return;
            }

            Dictionary<string, string> filesByRelative;
            Dictionary<string, List<string>> filesByName;
            int diskCount = IndexLocalProjectFiles(projectFolder, out filesByRelative, out filesByName);

            ErrorLogFile.writeStatus("ProcessCloudUrls started project=10076 diskFiles=" + diskCount +
                " uploadCap=NONE validateExisting=" + validateExisting +
                " - " + DateTime.Now.ToShortTimeString());
            Console.WriteLine("ProcessCloudUrls project=10076 local files=" + diskCount +
                " upload cap=NONE validateExisting=" + validateExisting);

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.DefaultConnectionLimit = 9999;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            UploadPendingCloudUrls(filesByRelative, filesByName, diskCount);

            ErrorLogFile.writeStatus("Starting broken-link check. Browser-failing URLs will be re-uploaded and strCloudUrl replaced.");
            Console.WriteLine("Checking existing cloud URLs. Broken / Access Denied links will be re-uploaded and the old URL replaced.");
            ValidateAndFixBrokenCloudUrls(dbPageSize, filesByRelative, filesByName);
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

        // Phase 1: upload from disk first. Every local file with an empty strCloudUrl row is uploaded,
        // including MAP/CommonMAP when the file actually exists. Missing DB rows are not marked uploaded.
        private void UploadPendingCloudUrls(
            Dictionary<string, string> filesByRelative,
            Dictionary<string, List<string>> filesByName,
            int diskCount)
        {
            CompassEntities compassEntities = new CompassEntities();
            var pendingRows = EmptyCloudUrlAttachments(compassEntities)
                .Select(o => new { o.ID, o.strFilePath })
                .ToList();

            var pendingByRelative = new Dictionary<string, List<long>>(StringComparer.OrdinalIgnoreCase);
            int mapPending = 0;
            int photoPending = 0;
            foreach (var row in pendingRows)
            {
                string relative = GetRelativePathUnderProject(row.strFilePath);
                if (string.IsNullOrEmpty(relative))
                {
                    continue;
                }

                List<long> ids;
                if (!pendingByRelative.TryGetValue(relative, out ids))
                {
                    ids = new List<long>();
                    pendingByRelative[relative] = ids;
                }
                ids.Add(row.ID);

                if (IsMapPath(row.strFilePath))
                {
                    mapPending++;
                }
                else
                {
                    photoPending++;
                }
            }

            int diskMatchedPending = 0;
            foreach (string relative in filesByRelative.Keys)
            {
                if (pendingByRelative.ContainsKey(relative))
                {
                    diskMatchedPending++;
                }
            }

            ErrorLogFile.writeStatus("Disk-first upload diskFiles=" + diskCount +
                " emptyCloudUrl=" + pendingRows.Count + " photos=" + photoPending +
                " maps=" + mapPending + " diskMatchedPending=" + diskMatchedPending +
                " uploadCap=NONE - " + DateTime.Now.ToShortTimeString());
            Console.WriteLine("Local files=" + diskCount + ", empty cloud URL rows=" + pendingRows.Count +
                " (photos=" + photoPending + ", MAP=" + mapPending + "), matched on disk=" + diskMatchedPending);
            Console.WriteLine("Uploading every local file that still has an empty strCloudUrl.");

            int sample = 0;
            foreach (string relative in filesByRelative.Keys)
            {
                ErrorLogFile.writeStatus("Disk sample: " + relative);
                sample++;
                if (sample >= 8)
                {
                    break;
                }
            }

            int uploaded = 0;
            int failed = 0;
            int filesUploaded = 0;
            var uploadedIds = new HashSet<long>();

            try
            {
                foreach (var diskFile in filesByRelative)
                {
                    List<long> ids;
                    if (!pendingByRelative.TryGetValue(diskFile.Key, out ids) || ids == null || ids.Count == 0)
                    {
                        continue;
                    }

                    string localPath = diskFile.Value;
                    if (string.IsNullOrEmpty(localPath) || !File.Exists(localPath))
                    {
                        continue;
                    }

                    try
                    {
                        string newCloudUrl = UploadFileS3Bucket(localPath);
                        if (string.IsNullOrEmpty(newCloudUrl))
                        {
                            failed++;
                            ErrorLogFile.writeStatus("Disk upload failed path=" + localPath);
                            continue;
                        }

                        var items = compassEntities.TblProjectFieldDataAttachments
                            .Where(o => ids.Contains(o.ID))
                            .ToList();
                        foreach (var item in items)
                        {
                            item.strCloudUrl = newCloudUrl;
                            uploadedIds.Add(item.ID);
                            uploaded++;
                        }
                        compassEntities.SaveChanges();
                        filesUploaded++;

                        if (filesUploaded % 50 == 0)
                        {
                            ErrorLogFile.writeStatus("Disk-first progress files=" + filesUploaded +
                                " rows=" + uploaded + " failed=" + failed);
                            Console.WriteLine("Uploaded files=" + filesUploaded + " DB rows=" + uploaded);
                            compassEntities.Dispose();
                            compassEntities = new CompassEntities();
                        }
                    }
                    catch (Exception ex)
                    {
                        failed++;
                        ErrorLogFile.writeStatus("Disk upload exception path=" + localPath + " " + ex.Message);
                    }
                }
            }
            finally
            {
                compassEntities.Dispose();
            }

            compassEntities = new CompassEntities();
            try
            {
                foreach (var row in pendingRows)
                {
                    if (uploadedIds.Contains(row.ID))
                    {
                        continue;
                    }

                    string localPath = FindLocalFile(row.strFilePath, filesByRelative, filesByName);
                    if (string.IsNullOrEmpty(localPath) || !File.Exists(localPath))
                    {
                        continue;
                    }

                    try
                    {
                        string newCloudUrl = UploadFileS3Bucket(localPath);
                        if (string.IsNullOrEmpty(newCloudUrl))
                        {
                            failed++;
                            continue;
                        }

                        var item = compassEntities.TblProjectFieldDataAttachments.FirstOrDefault(o => o.ID == row.ID);
                        if (item == null)
                        {
                            continue;
                        }
                        item.strCloudUrl = newCloudUrl;
                        compassEntities.SaveChanges();
                        uploadedIds.Add(row.ID);
                        uploaded++;
                        filesUploaded++;
                    }
                    catch (Exception ex)
                    {
                        failed++;
                        ErrorLogFile.writeStatus("Fallback upload exception ID=" + row.ID + " " + ex.Message);
                    }
                }
            }
            finally
            {
                compassEntities.Dispose();
            }

            string missingListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log",
                "MissingLocalFiles_10076_" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt");
            int missingPhotos = 0;
            Directory.CreateDirectory(Path.GetDirectoryName(missingListPath));
            using (StreamWriter missingWriter = new StreamWriter(missingListPath, false))
            {
                foreach (var row in pendingRows)
                {
                    if (uploadedIds.Contains(row.ID) || IsMapPath(row.strFilePath))
                    {
                        continue;
                    }

                    missingPhotos++;
                    string resolved = ResolveLocalFilePath(row.strFilePath, WebUrl).Replace('/', '\\');
                    missingWriter.WriteLine(row.ID + "\t" + resolved);
                    if (missingPhotos <= 10 || missingPhotos % 500 == 0)
                    {
                        ErrorLogFile.writeStatus("Local photo missing ID=" + row.ID + " path=" + resolved);
                    }
                }
            }

            int remainingPhotos;
            int remainingMaps;
            using (CompassEntities remainingDb = new CompassEntities())
            {
                remainingPhotos = PendingAttachments(remainingDb).Count();
                remainingMaps = EmptyCloudUrlAttachments(remainingDb).Count() - remainingPhotos;
            }

            string summary = "Disk-first upload done. diskFiles=" + diskCount +
                " diskMatchedPending=" + diskMatchedPending +
                " filesUploaded=" + filesUploaded + " rowsUploaded=" + uploaded +
                " failed=" + failed + " missingPhotos=" + missingPhotos +
                " stillEmptyPhotos=" + remainingPhotos + " stillEmptyMAP=" + remainingMaps +
                " - " + DateTime.Now.ToShortTimeString();
            ErrorLogFile.writeStatus(summary);
            Console.WriteLine(summary);
            if (missingPhotos > 0)
            {
                ErrorLogFile.writeStatus("Missing local photos listed in " + missingListPath);
                Console.WriteLine(missingPhotos + " photos are not on disk. Missing list: " + missingListPath);
            }
        }

        private IQueryable<TblProjectFieldDataAttachment> EmptyCloudUrlAttachments(CompassEntities db)
        {
            return db.TblProjectFieldDataAttachments
                .Where(o =>
                    o.Active == 1 &&
                    o.strFilePath != null &&
                    (o.strFilePath.Contains(PathFilter) || o.strFilePath.Contains(PathFilterAlt)) &&
                    (o.strCloudUrl == null || o.strCloudUrl == "")
                );
        }

        private IQueryable<TblProjectFieldDataAttachment> PendingAttachments(CompassEntities db)
        {
            // MAP/CommonMAP img.png files are generated Google Map snapshots, not field photos.
            // They are usually not on disk, so skip them. Do not mark them uploaded.
            return EmptyCloudUrlAttachments(db)
                .Where(o =>
                    !o.strFilePath.Contains(@"\MAP\") &&
                    !o.strFilePath.Contains(@"/MAP/") &&
                    !o.strFilePath.Contains(@"\CommonMAP\") &&
                    !o.strFilePath.Contains(@"/CommonMAP/")
                );
        }

        private static int IndexLocalProjectFiles(
            string projectFolder,
            out Dictionary<string, string> filesByRelative,
            out Dictionary<string, List<string>> filesByName)
        {
            filesByRelative = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            filesByName = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            int diskCount = 0;
            string root = Path.GetFullPath(projectFolder).TrimEnd('\\') + "\\";

            foreach (string fullPath in Directory.EnumerateFiles(projectFolder, "*", SearchOption.AllDirectories))
            {
                diskCount++;
                string full = Path.GetFullPath(fullPath);
                string relative = full.StartsWith(root, StringComparison.OrdinalIgnoreCase)
                    ? NormalizeRelativePath(full.Substring(root.Length))
                    : Path.GetFileName(full);

                if (!filesByRelative.ContainsKey(relative))
                {
                    filesByRelative[relative] = full;
                }

                string name = Path.GetFileName(full);
                List<string> list;
                if (!filesByName.TryGetValue(name, out list))
                {
                    list = new List<string>();
                    filesByName[name] = list;
                }
                list.Add(full);
            }

            return diskCount;
        }

        private static string FindLocalFile(
            string dbFilePath,
            Dictionary<string, string> filesByRelative,
            Dictionary<string, List<string>> filesByName)
        {
            string relative = GetRelativePathUnderProject(dbFilePath);
            string found;
            if (!string.IsNullOrEmpty(relative) && filesByRelative.TryGetValue(relative, out found))
            {
                return found;
            }

            string resolved = ResolveLocalFilePath(dbFilePath, WebUrl).Replace('/', '\\');
            if (File.Exists(resolved))
            {
                return resolved;
            }

            string longPath = resolved.StartsWith(@"\\?\", StringComparison.Ordinal) ? resolved : @"\\?\" + resolved;
            if (resolved.Length >= 240 && File.Exists(longPath))
            {
                return resolved;
            }

            string fileName = Path.GetFileName(relative);
            if (string.IsNullOrEmpty(fileName) ||
                fileName.Equals("img.png", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            List<string> candidates;
            if (!filesByName.TryGetValue(fileName, out candidates) || candidates == null || candidates.Count == 0)
            {
                return null;
            }

            if (candidates.Count == 1)
            {
                return candidates[0];
            }

            string dirHint = Path.GetDirectoryName(relative) ?? string.Empty;
            string[] parts = dirHint.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string candidate in candidates)
            {
                bool allPartsMatch = true;
                foreach (string part in parts)
                {
                    if (candidate.IndexOf("\\" + part + "\\", StringComparison.OrdinalIgnoreCase) < 0 &&
                        !candidate.EndsWith("\\" + part, StringComparison.OrdinalIgnoreCase))
                    {
                        allPartsMatch = false;
                        break;
                    }
                }
                if (allPartsMatch)
                {
                    return candidate;
                }
            }

            return null;
        }

        private static string GetRelativePathUnderProject(string dbFilePath)
        {
            string path = NormalizeRelativePath(dbFilePath);
            string marker = @"MobileUploadData\10076\";
            int index = path.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                return NormalizeRelativePath(path.Substring(index + marker.Length));
            }
            return Path.GetFileName(path);
        }

        private static string NormalizeRelativePath(string path)
        {
            return (path ?? string.Empty)
                .Replace('/', '\\')
                .Trim()
                .TrimStart('\\');
        }

        private static bool IsMapPath(string path)
        {
            string normalized = NormalizeRelativePath(path);
            return normalized.IndexOf(@"\MAP\", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   normalized.IndexOf(@"\CommonMAP\", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   normalized.StartsWith(@"MAP\", StringComparison.OrdinalIgnoreCase) ||
                   normalized.StartsWith(@"CommonMAP\", StringComparison.OrdinalIgnoreCase);
        }

        // Phase 2: HTTP-check every existing 10076 cloud URL. AccessDenied/403 and other broken links are re-uploaded.
        private void ValidateAndFixBrokenCloudUrls(
            int fetchWindow,
            Dictionary<string, string> filesByRelative,
            Dictionary<string, List<string>> filesByName)
        {
            CompassEntities compassEntities = new CompassEntities();
            int totalWithUrl = AttachmentsWithCloudUrl(compassEntities).Count();
            ErrorLogFile.writeStatus("URL validation total=" + totalWithUrl +
                " checkingAllRemaining fetchWindow=" + fetchWindow + " - " + DateTime.Now.ToShortTimeString());
            Console.WriteLine("Checking " + totalWithUrl + " existing cloud URLs. Broken links will be re-uploaded.");

            int checkedCount = 0;
            int okCount = 0;
            int accessDeniedCount = 0;
            int notFoundCount = 0;
            int brokenCount = 0;
            int reuploadedCount = 0;
            int missingCount = 0;
            int failedCount = 0;
            long lastId = 0;
            string deniedListPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log",
                "AccessDenied_10076_" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt");

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(deniedListPath));
                using (StreamWriter deniedWriter = new StreamWriter(deniedListPath, false))
                {
                    deniedWriter.WriteLine("ID\tStatus\tOldUrl\tLocalPath\tResult");

                    while (true)
                    {
                        var batch = AttachmentsWithCloudUrl(compassEntities)
                            .Where(o => o.ID > lastId)
                            .OrderBy(o => o.ID)
                            .Take(fetchWindow)
                            .ToList();

                        if (batch.Count == 0)
                        {
                            break;
                        }

                        foreach (var item in batch)
                        {
                            lastId = item.ID;
                            checkedCount++;
                            try
                            {
                                string cloudUrl = (item.strCloudUrl ?? "").Trim();
                                CloudUrlStatus status = CheckCloudUrl(cloudUrl);
                                if (status == CloudUrlStatus.Ok)
                                {
                                    okCount++;
                                    if (checkedCount % 100 == 0)
                                    {
                                        ErrorLogFile.writeStatus("Validation progress checked=" + checkedCount +
                                            " ok=" + okCount + " accessDenied=" + accessDeniedCount +
                                            " reuploaded=" + reuploadedCount + " missing=" + missingCount);
                                        Console.WriteLine("Checked " + checkedCount + " ok=" + okCount +
                                            " denied=" + accessDeniedCount + " reuploaded=" + reuploadedCount);
                                    }
                                    continue;
                                }

                                if (status == CloudUrlStatus.AccessDenied)
                                {
                                    accessDeniedCount++;
                                }
                                else if (status == CloudUrlStatus.NotFound)
                                {
                                    notFoundCount++;
                                }
                                else
                                {
                                    brokenCount++;
                                }

                                string localPath = FindLocalFile(item.strFilePath, filesByRelative, filesByName);
                                if (string.IsNullOrEmpty(localPath) || !File.Exists(localPath))
                                {
                                    missingCount++;
                                    string resolved = ResolveLocalFilePath(item.strFilePath, WebUrl).Replace('/', '\\');
                                    deniedWriter.WriteLine(item.ID + "\t" + status + "\t" + cloudUrl + "\t" + resolved + "\tLOCAL_MISSING");
                                    if (missingCount <= 10 || missingCount % 200 == 0)
                                    {
                                        ErrorLogFile.writeStatus(status + " local missing ID=" + item.ID + " url=" + cloudUrl);
                                    }
                                    continue;
                                }

                                string newCloudUrl = UploadFileS3Bucket(localPath);
                                if (!string.IsNullOrEmpty(newCloudUrl))
                                {
                                    item.strCloudUrl = newCloudUrl;
                                    compassEntities.SaveChanges();
                                    reuploadedCount++;
                                    deniedWriter.WriteLine(item.ID + "\t" + status + "\t" + cloudUrl + "\t" + localPath + "\tREUPLOADED\t" + newCloudUrl);
                                    if (reuploadedCount % 50 == 0)
                                    {
                                        ErrorLogFile.writeStatus("Re-uploaded broken URLs=" + reuploadedCount +
                                            " accessDenied=" + accessDeniedCount);
                                    }
                                }
                                else
                                {
                                    failedCount++;
                                    deniedWriter.WriteLine(item.ID + "\t" + status + "\t" + cloudUrl + "\t" + localPath + "\tUPLOAD_FAILED");
                                    ErrorLogFile.writeStatus(status + " re-upload failed ID=" + item.ID);
                                }
                            }
                            catch (Exception ex)
                            {
                                failedCount++;
                                ErrorLogFile.writeStatus("Validation item exception ID=" + item.ID + " " + ex.Message);
                            }
                        }

                        compassEntities.Dispose();
                        compassEntities = new CompassEntities();
                    }
                }
            }
            finally
            {
                compassEntities.Dispose();
            }

            string summary = "URL validation done. totalWithUrl=" + totalWithUrl +
                " checked=" + checkedCount + " ok=" + okCount +
                " accessDenied=" + accessDeniedCount + " notFound=" + notFoundCount +
                " broken=" + brokenCount + " reuploaded=" + reuploadedCount +
                " missing=" + missingCount + " failed=" + failedCount +
                " lastId=" + lastId + " - " + DateTime.Now.ToShortTimeString();
            ErrorLogFile.writeStatus(summary);
            ErrorLogFile.writeStatus("AccessDenied/broken list: " + deniedListPath);
            Console.WriteLine(summary);
            Console.WriteLine("AccessDenied/broken list: " + deniedListPath);
        }

        private IQueryable<TblProjectFieldDataAttachment> AttachmentsWithCloudUrl(CompassEntities db)
        {
            return db.TblProjectFieldDataAttachments
                .Where(o =>
                    o.Active == 1 &&
                    o.strFilePath != null &&
                    (o.strFilePath.Contains(PathFilter) || o.strFilePath.Contains(PathFilterAlt)) &&
                    o.strCloudUrl != null &&
                    o.strCloudUrl != ""
                );
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
            if (oldFilePath.Contains("../../"))
            {
                oldFilePath = oldFilePath.Replace("../../", webUrl);
            }
            if (oldFilePath.Contains("~"))
            {
                oldFilePath = oldFilePath.Replace("~\\", webUrl);
                oldFilePath = oldFilePath.Replace("~/", webUrl);
            }
            oldFilePath = oldFilePath.Replace('\\', '/').Trim();
            return oldFilePath;
        }

        private static CloudUrlStatus CheckCloudUrl(string cloudUrl)
        {
            if (string.IsNullOrWhiteSpace(cloudUrl) ||
                !cloudUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase) ||
                cloudUrl.IndexOf("failed", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return CloudUrlStatus.Broken;
            }

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(cloudUrl);
                request.Method = "GET";
                request.Timeout = 15000;
                request.ReadWriteTimeout = 15000;
                request.AllowAutoRedirect = true;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";
                request.Accept = "*/*";

                HttpWebResponse response = null;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    response = ex.Response as HttpWebResponse;
                    if (response == null)
                    {
                        return CloudUrlStatus.Broken;
                    }
                }

                using (response)
                {
                    int status = (int)response.StatusCode;
                    string body = ReadBodyPreview(response, 4096);
                    if (IsAccessDeniedBody(body))
                    {
                        return CloudUrlStatus.AccessDenied;
                    }
                    return MapHttpStatus(status);
                }
            }
            catch (Exception ex)
            {
                ErrorLogFile.writeStatus("CheckCloudUrl exception: " + ex.Message + " URL=" + cloudUrl);
            }

            return CloudUrlStatus.Broken;
        }

        private static string ReadBodyPreview(HttpWebResponse response, int maxChars)
        {
            try
            {
                Stream stream = response.GetResponseStream();
                if (stream == null)
                {
                    return string.Empty;
                }
                using (StreamReader reader = new StreamReader(stream))
                {
                    char[] buffer = new char[maxChars];
                    int read = reader.Read(buffer, 0, buffer.Length);
                    return read > 0 ? new string(buffer, 0, read) : string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private static bool IsAccessDeniedBody(string body)
        {
            if (string.IsNullOrEmpty(body))
            {
                return false;
            }
            return body.IndexOf("AccessDenied", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   body.IndexOf("Access Denied", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   body.IndexOf("AllAccessDisabled", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static CloudUrlStatus MapHttpStatus(int status)
        {
            if (status >= 200 && status < 400)
            {
                return CloudUrlStatus.Ok;
            }
            if (status == 403)
            {
                return CloudUrlStatus.AccessDenied;
            }
            if (status == 404)
            {
                return CloudUrlStatus.NotFound;
            }
            return CloudUrlStatus.Broken;
        }

        // Added on 24 Aug 2026 - HTTP check whether cloud URL still serves the file
        private static bool IsCloudUrlReachable(string cloudUrl)
        {
            return CheckCloudUrl(cloudUrl) == CloudUrlStatus.Ok;
        }

        private static bool TryHttpStatus(string url, string method, out int statusCode)
        {
            return TryHttpStatus(url, method, 15000, false, out statusCode);
        }

        private static bool TryHttpStatus(string url, string method, int timeoutMs, bool firstByteOnly, out int statusCode)
        {
            statusCode = 0;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.Timeout = timeoutMs;
            request.ReadWriteTimeout = timeoutMs;
            request.AllowAutoRedirect = true;
            request.UserAgent = "TNMPUploadUtility-CloudUrlCheck/1.0";
            if (firstByteOnly)
            {
                request.AddRange(0, 0);
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    statusCode = (int)response.StatusCode;
                    if (firstByteOnly)
                    {
                        Stream stream = response.GetResponseStream();
                        if (stream != null)
                        {
                            stream.ReadByte();
                        }
                    }
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
