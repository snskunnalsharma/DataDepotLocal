using System;
using System.IO;
using System.Security.AccessControl;

namespace TNMPUploadUtility
{
    class ErrorLogFile
    {
        static string appDirectory = System.AppDomain.CurrentDomain.BaseDirectory + "Log";
        // Keep only last N days of log files to save disk space
        const int LogRetentionDays = 7;
        static DateTime lastCleanupDate = DateTime.MinValue;

        public static void writeStatus(string _status)
        {
            if (grantAccess())
            {
                CleanupOldLogsIfNeeded();
                writeFile(_status);
            }
        }

        private static bool grantAccess()
        {
            bool isSucessed = false;
            try
            {
                if (!Directory.Exists(appDirectory))
                {
                    Directory.CreateDirectory(appDirectory);
                }

                DirectoryInfo dInfo = new DirectoryInfo(appDirectory);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();
                dSecurity.AddAccessRule(new FileSystemAccessRule("everyone", FileSystemRights.FullControl,
                                                                 InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                                                                 PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);
                isSucessed = true;
            }
            catch { }
            return isSucessed;
        }

        private static void writeFile(string message)
        {
            try
            {
                string path = appDirectory + "\\" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt";

                if (!File.Exists(path))
                {
                    File.Create(path).Dispose();
                }

                using (TextWriter tw = new StreamWriter(path, true))
                {
                    tw.WriteLine(message);
                }
            }
            catch { }
        }

        // Delete log files older than LogRetentionDays (once per day)
        private static void CleanupOldLogsIfNeeded()
        {
            try
            {
                DateTime today = DateTime.Today;
                if (lastCleanupDate == today)
                {
                    return;
                }
                lastCleanupDate = today;

                if (!Directory.Exists(appDirectory))
                {
                    return;
                }

                DateTime cutoff = today.AddDays(-LogRetentionDays);
                foreach (string file in Directory.GetFiles(appDirectory, "*.txt"))
                {
                    try
                    {
                        FileInfo fi = new FileInfo(file);
                        if (fi.LastWriteTime.Date < cutoff)
                        {
                            fi.Delete();
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}
