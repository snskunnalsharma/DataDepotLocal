using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace TNMPUploadUtility
{
    class ErrorLogFile
    {
        static string appDirectory = Environment.CurrentDirectory + "\\Log";

        public static void writeStatus(string _status)
        {
            //check if the directory with access right is available
            if (grantAccess())
            {
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

                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(message);
                tw.Close();
                tw.Dispose();

            }
            catch { }
        }
    }
}
