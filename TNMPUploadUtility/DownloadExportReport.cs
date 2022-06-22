using GarronT.CMS.Model;
using GarronT.CMS.Model.DAL;
using GarronT.CMS.UI.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using System.IO;

namespace TNMPUploadUtility
{
    class DownloadExportReport
    {
        #region get Excel CVS data in DataTable
        public DataTable createExcelNew(string fromDate, string toDate, long formTypeId, long projectId, string[] visitStatus, int latlongRequired, int mediaRequired)
        {
            DataTable dt = new DataTable();
            try
            {
                ExportReportDAL exportReportDAL = new ExportReportDAL();
                //long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                long currentUserId = 14;  // Defualt pass user id
                var checkIfFieldSavedBeforExport = exportReportDAL.checkFieldSavedForExportFormType(formTypeId, projectId, int.Parse(currentUserId.ToString()));
                if (!checkIfFieldSavedBeforExport)
                {
                    ErrorLogFile.writeStatus("Please save the report fields first & then try...! - " + DateTime.Now.ToShortTimeString());
                    Console.WriteLine("Please save the report fields first & then try...! - " + DateTime.Now.ToShortTimeString());
                    return dt = new DataTable();
                }

                DateTime _fromDate = Convert.ToDateTime(fromDate);
                DateTime _toDate = Convert.ToDateTime(toDate);
                List<string> columnNameList1;
                #region All
                ExportReportController objERC = new ExportReportController();
                if (formTypeId == 2)
                {
                    dt = objERC.AllRecordDataTable(formTypeId, projectId, visitStatus, _fromDate, _toDate, latlongRequired, mediaRequired, out columnNameList1, currentUserId);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        return dt;
                    }
                    else
                    {
                        ErrorLogFile.writeStatus("No data avilable for selected date range. - " + DateTime.Now.ToShortTimeString());
                        Console.WriteLine("No data avilable for selected date range. - " + DateTime.Now.ToShortTimeString());
                        return dt = new DataTable();
                    }
                }
                else if (formTypeId == 1)
                {
                    dt = objERC.AllAuditRecordDataTable(formTypeId, projectId, visitStatus, _fromDate, _toDate, latlongRequired, mediaRequired, out columnNameList1, currentUserId);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        return dt;
                    }
                    else
                    {
                        ErrorLogFile.writeStatus("No data avilable for selected date range. - " + DateTime.Now.ToShortTimeString());
                        Console.WriteLine("No data avilable for selected date range. - " + DateTime.Now.ToShortTimeString());
                        return dt = new DataTable();
                    }
                }
                else
                {
                    ErrorLogFile.writeStatus("No data avilable for selected date range. - " + DateTime.Now.ToShortTimeString());
                    Console.WriteLine("No data avilable for selected date range. - " + DateTime.Now.ToShortTimeString());
                    return dt = new DataTable();
                }
                #endregion
            }
            catch(Exception ex)
            {
                ErrorLogFile.writeStatus("Error:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Error:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
                return dt = new DataTable();
            }
        }
        #endregion

        #region Generate csv
        public string DownloadCSVExport(DataTable dt,long projectId)
        {
            ErrorLogFile.writeStatus("Start ==> Generate CSV file - " + DateTime.Now.ToShortTimeString());
            Console.WriteLine("Start ==> Generate CSV file - " + DateTime.Now.ToShortTimeString());
            try
            {
                ExportReportController objERC = new ExportReportController();
                string fileName = string.Empty;
                if (projectId == 10085)
                {
                    fileName = "meter_exchange_CMS_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                }
                else if (projectId == 10087)
                {
                    fileName = "meter_exchange_TNMP_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                }
                else
                {
                    fileName = "meter_exchange_CMS_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                }

                StringBuilder sb = new StringBuilder();
                dt = objERC.datatableColumnChnages(dt);     // Remove column not requied for TNMP project ==> Electric project
                int totalCount = dt.Columns.Count;
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    if ((totalCount - 1) == k)
                    {
                        sb.Append(dt.Columns[k].ColumnName);    // Remove last comma(,) in row
                    }
                    else
                    {
                        sb.Append(dt.Columns[k].ColumnName + ',');
                    }
                }
                sb.Append("\r\n");
                totalCount = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    totalCount = dt.Columns.Count;
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        if ((totalCount - 1) == k)
                        {
                            sb.Append(dt.Rows[i][k].ToString());    // Remove last comma(,) in row
                        }
                        else
                        {
                            sb.Append(dt.Rows[i][k].ToString() + ',');
                        }

                    }
                    sb.Append("\r\n");
                }

                string baseDir = System.AppDomain.CurrentDomain.BaseDirectory + "ExportReport";
                if (!System.IO.Directory.Exists(@"" + baseDir + ""))
                {
                    System.IO.Directory.CreateDirectory(@"" + baseDir + "");
                }
                string filePath = baseDir + "\\" + fileName;
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, false))
                {
                    sw.Write(sb.ToString());
                    sw.Close();
                }
                ErrorLogFile.writeStatus("CSV file generated successfully done - " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("CSV file generated successfully done - " + DateTime.Now.ToShortTimeString());
                return filePath;
            }
            catch (Exception ex)
            {
                ErrorLogFile.writeStatus("Error:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Error:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
                return "";
            }

        }
        #endregion

        #region Excport report file Upload in SFTP server
        public bool UploadFileSFTPServer(string filePath)
        {
            bool status = false;
            try
            {
                ErrorLogFile.writeStatus("Connect SFTP server for Export Report - " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Connect SFTP server for Export Report");
                string host = ConfigurationManager.AppSettings["sftpurl"].ToString();
                string username = ConfigurationManager.AppSettings["sftpuid"].ToString();
                string password = ConfigurationManager.AppSettings["sftppwd"].ToString();
                using (SftpClient sftp = new SftpClient(host, username, password))
                {
                    try
                    {
                        // Path to file on SFTP server
                        //string remoteDirectory = "/home/tnmp/pre_prod/";
                        string remoteDirectory = ConfigurationManager.AppSettings["sftOutputFilePath"].ToString();
                        sftp.Connect();
                        sftp.ChangeDirectory(remoteDirectory);
                        using (FileStream fs = new FileStream(filePath, FileMode.Open))
                        {
                            sftp.BufferSize = 4 * 1024;
                            sftp.UploadFile(fs, Path.GetFileName(filePath));
                            fs.Close();
                        }
                        sftp.Disconnect();
                        File.Delete(@"" + filePath + "");
                        status = true;
                    }
                    catch (Exception ex)
                    {
                        ErrorLogFile.writeStatus("An exception has been caught " + ex.Message.ToString() + DateTime.Now.ToShortTimeString());
                        Console.WriteLine("An exception has been caught " + ex.Message.ToString());
                        status = false;
                    }
                }
                return status;
            }
            catch (Exception ex)
            {
                ErrorLogFile.writeStatus("Error:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Error:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
                return status;
            }
        }
        #endregion
    }
}
