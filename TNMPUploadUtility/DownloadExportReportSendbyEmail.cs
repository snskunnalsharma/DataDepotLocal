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
using System.Net;
using System.Net.Mail;


namespace TNMPUploadUtility
{
    class DownloadExportReportSendbyEmail
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
                        if (projectId == 10122)
                        {
                            var colname = new List<string> { "ACCOUNT", "OLD METER NUMBER", "NEW METER RADIO" };

                            // Remove unwanted columns
                            foreach (var item in dt.Columns.Cast<DataColumn>().ToList())
                            {
                                if (!colname.Contains(item.ColumnName))
                                {
                                    dt.Columns.Remove(item.ColumnName);
                                }
                            }

                            // Remove rows with null, blank, or hyphen in "NEW METER NUMBER" column
                            var rowsToRemove = dt.AsEnumerable()
                                .Where(row => string.IsNullOrWhiteSpace(row["NEW METER RADIO"].ToString())
                                              || row["NEW METER RADIO"].ToString() == "-")
                                .ToList();

                            foreach (var row in rowsToRemove)
                            {
                                dt.Rows.Remove(row);
                            }
                        }
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
            catch (Exception ex)
            {
                ErrorLogFile.writeStatus("Error:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Error:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
                return dt = new DataTable();
            }
        }
        #endregion
        #region Generate csv
        public string DownloadCSVExport(DataTable dt, long projectId)
        {
            ErrorLogFile.writeStatus("Start ==> Generate CSV file - " + DateTime.Now.ToShortTimeString());
            Console.WriteLine("Start ==> Generate CSV file - " + DateTime.Now.ToShortTimeString());
            try
            {
                ExportReportController objERC = new ExportReportController();
                string fileName = string.Empty;
                if (projectId == 10122)
                {
                    fileName = "New_ERT_Installed_on _" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + ".csv";
                }
                
                else
                {
                    fileName = "Export_Report_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                }

                StringBuilder sb = new StringBuilder();
                dt = objERC.datatableColumnChnages(dt, projectId);     // Remove column not requied for TNMP project ==> Electric project
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
        public void SendEmailWithAttachment(long projectId, String filePath)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com"); // Update SMTP server

                mail.From = new MailAddress("datadepotonline@gmail.com");
              //mail.To.Add("kunal.sharma@snssystem.com");
                mail.To.Add("jeff.downs@beaumonttexas.gov");
                mail.To.Add("brigette.johnson@beaumonttexas.gov");
                // Add CC and BCC
                 mail.CC.Add("jason.norton@republicmeter.com");
                mail.Bcc.Add("garron@cmsmeter.com");

                mail.Subject = "New ERT Installed on " + DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
                mail.Body = "Dear Sir,\n" +
             "Please find the attached Export Report file.\n" +
             "Project Name: Beaumont\n" +
             "Project ID: " + projectId ;




                Attachment attachment = new Attachment(filePath);
                mail.Attachments.Add(attachment);

                SmtpServer.Port = 587; // Update with the port
                SmtpServer.Credentials = new NetworkCredential("datadepotonline@gmail.com", "wgln xqsd iwge okuu");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                ErrorLogFile.writeStatus("Email sent successfully - " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Email sent successfully - " + DateTime.Now.ToShortTimeString());
            }
            catch (Exception ex)
            {
                ErrorLogFile.writeStatus("Error in sending email: " + ex.Message + " - " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Error in sending email: " + ex.Message + " - " + DateTime.Now.ToShortTimeString());
            }
        }
    }
}
