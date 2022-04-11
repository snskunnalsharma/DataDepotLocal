using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;

namespace TNMPUploadUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            ErrorLogFile.writeStatus("Thread Started at -" + DateTime.Now.ToShortTimeString());

            ErrorLogFile.writeStatus("Thread Ended at -" + DateTime.Now.ToShortTimeString());
        }
    }
}
