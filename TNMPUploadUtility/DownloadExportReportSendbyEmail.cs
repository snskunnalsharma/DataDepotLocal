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
using OfficeOpenXml;

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
                long currentUserId = 14;  // Default user ID

                // Check if export fields are saved
                bool isFieldConfigured = exportReportDAL.checkFieldSavedForExportFormType(formTypeId, projectId, (int)currentUserId);
                if (!isFieldConfigured)
                {
                    ErrorLogFile.writeStatus("Please save the report fields first & then try...! - " + DateTime.Now.ToShortTimeString());
                    Console.WriteLine("Please save the report fields first & then try...! - " + DateTime.Now.ToShortTimeString());
                    return new DataTable();
                }

                DateTime _fromDate = Convert.ToDateTime(fromDate);
                DateTime _toDate = Convert.ToDateTime(toDate);

                List<string> columnNameList1;
                ExportReportController objERC = new ExportReportController();

                if (formTypeId == 2) // Main report
                {
                    dt = objERC.AllRecordDataTable(formTypeId, projectId, visitStatus, _fromDate, _toDate, latlongRequired, mediaRequired, out columnNameList1, currentUserId);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        // Add LATITUDE/LONGITUDE columns for project 10132
                        if (projectId == 10132 && dt.Columns.Contains("LOCATION/GPS"))
                        {
                            dt.Columns.Add("LATITUDE");
                            dt.Columns.Add("LONGITUDE");

                            foreach (DataRow row in dt.Rows)
                            {
                                string location = row["LOCATION/GPS"]?.ToString();
                                if (!string.IsNullOrEmpty(location) && location.Contains(","))
                                {
                                    var parts = location.Split(',');
                                    if (parts.Length == 2)
                                    {
                                        row["LATITUDE"] = parts[0].Trim();
                                        row["LONGITUDE"] = parts[1].Trim();
                                    }
                                }
                            }

                            // Set LATITUDE to column index 32 and LONGITUDE to 33
                            try
                            {
                                if (dt.Columns.Contains("LATITUDE"))
                                    dt.Columns["LATITUDE"].SetOrdinal(32);

                                if (dt.Columns.Contains("LONGITUDE"))
                                    dt.Columns["LONGITUDE"].SetOrdinal(33);

                                dt.AcceptChanges();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Column positioning error: " + ex.Message);
                                ErrorLogFile.writeStatus("Column positioning error: " + ex.Message + " - " + DateTime.Now.ToShortTimeString());
                            }

                            // Optional: remove LOCATION/GPS if needed
                            // dt.Columns.Remove("LOCATION/GPS");
                        }

                        // Clean up row values
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                string columnName = dt.Columns[j].ColumnName;

                                if (dt.Rows[i][j] == DBNull.Value)
                                    dt.Rows[i][j] = "-";

                                if (projectId == 10132 && columnName.Equals("STREET_NAME"))
                                {
                                    string streetVal = dt.Rows[i][j].ToString().Trim();
                                    string[] words = streetVal.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    string newStreet = string.Join(" ", words.Skip(1)); // Remove first word
                                    dt.Rows[i][j] = newStreet;
                                }
                            }
                        }

                        // Remove V_NEW_LATITUDE and V_NEW_LONGITUDE for project 10132
                        if (projectId == 10132)
                        {
                            string[] columnsToRemove = { "V_NEW_LATITUDE", "V_NEW_LONGITUDE" };
                            foreach (string colName in columnsToRemove)
                            {
                                if (dt.Columns.Contains(colName))
                                {
                                    dt.Columns.Remove(colName);
                                }
                            }
                        }

                        return dt;
                    }
                    else
                    {
                        ErrorLogFile.writeStatus("No data available for selected date range. - " + DateTime.Now.ToShortTimeString());
                        Console.WriteLine("No data available for selected date range. - " + DateTime.Now.ToShortTimeString());
                        return new DataTable();
                    }
                }
                else if (formTypeId == 1) // Audit data
                {
                    dt = objERC.AllAuditRecordDataTable(formTypeId, projectId, visitStatus, _fromDate, _toDate, latlongRequired, mediaRequired, out columnNameList1, currentUserId);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        return dt;
                    }
                    else
                    {
                        ErrorLogFile.writeStatus("No data available for selected date range. - " + DateTime.Now.ToShortTimeString());
                        Console.WriteLine("No data available for selected date range. - " + DateTime.Now.ToShortTimeString());
                        return new DataTable();
                    }
                }
                else // Fallback
                {
                    ErrorLogFile.writeStatus("No data available for selected date range. - " + DateTime.Now.ToShortTimeString());
                    Console.WriteLine("No data available for selected date range. - " + DateTime.Now.ToShortTimeString());
                    return new DataTable();
                }
            }
            catch (Exception ex)
            {
                ErrorLogFile.writeStatus("Error: " + ex.Message + " - " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Error: " + ex.Message + " - " + DateTime.Now.ToShortTimeString());
                return new DataTable();
            }
        }


        #endregion
        #region Generate csv
        public string DownloadExcelExport(DataTable dt, long projectId)
        {
            ErrorLogFile.writeStatus("Start ==> Generate Excel file - " + DateTime.Now.ToShortTimeString());
            Console.WriteLine("Start ==> Generate Excel file - " + DateTime.Now.ToShortTimeString());

            try
            {
                ExportReportController objERC = new ExportReportController();
                string fileName = string.Empty;

                // === Project-specific changes ===
                if (projectId == 10132)
                {
                    if (dt.Columns.Contains("OUT_READ_DATE") && dt.Columns.Contains("VISIT ATTENDED DATETIME"))
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row["VISIT ATTENDED DATETIME"] != DBNull.Value)
                            {
                                row["OUT_READ_DATE"] = Convert.ToDateTime(row["VISIT ATTENDED DATETIME"]).ToString("MM/dd/yyyy");
                            }
                        }
                    }

                    if (dt.Columns.Contains("IN_READ_DATE") && dt.Columns.Contains("VISIT ATTENDED DATETIME"))
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row["VISIT ATTENDED DATETIME"] != DBNull.Value)
                            {
                                row["IN_READ_DATE"] = Convert.ToDateTime(row["VISIT ATTENDED DATETIME"]).ToString("MM/dd/yyyy");
                            }
                        }
                    }

                    string[] columnsToRemove = new string[]
                    {
                "ATTENDANT NAME",
                "VISIT ATTENDED DATETIME",
                "VISIT STATUS",
                "Form Name",
                "REASON",
                "COMMENT",
                "LOCATION/GPS",
                "V_NEW_LATITUDE",
                "V_NEW_LONGITUDE"

                    };

                    foreach (string colName in columnsToRemove)
                    {
                        if (dt.Columns.Contains(colName))
                        {
                            dt.Columns.Remove(colName);
                        }
                    }

                    if (dt.Columns.Contains("BASE_SERIAL_NO") && dt.Columns.Contains("NEW_METER_NO"))
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            row["BASE_SERIAL_NO"] = row["NEW_METER_NO"];
                        }
                    }
                }

             
                dt = objERC.datatableColumnChnages(dt, projectId);

               
                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        if (row[col] == DBNull.Value || row[col] == null)
                        {
                            row[col] = "";
                        }
                    }
                }

                // ✅ Set Excel filename
                fileName = (projectId == 10132)
                    ? "meter_exchange_NTX_" + DateTime.Now.AddDays(-1).ToString("MMddyyyy") + ".xlsx"
                    : "Export_Report_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";

                // ✅ Define export folder
                string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExportReport");
                if (!Directory.Exists(baseDir))
                    Directory.CreateDirectory(baseDir);

                string filePath = Path.Combine(baseDir, fileName);

                // ✅ Generate Excel using EPPlus
                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Report");
                    ws.Cells["A1"].LoadFromDataTable(dt, true);
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();

                    File.WriteAllBytes(filePath, pck.GetAsByteArray());
                }

                ErrorLogFile.writeStatus("Excel file generated successfully - " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Excel file generated successfully - " + DateTime.Now.ToShortTimeString());

                return filePath;
            }
            catch (Exception ex)
            {
                ErrorLogFile.writeStatus("Error: " + ex.Message + " - " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Error: " + ex.Message + " - " + DateTime.Now.ToShortTimeString());
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
               // mail.To.Add("kunal.sharma@snssystem.com");
                mail.To.Add("garron@cmsmeter.com");
                mail.To.Add("support@udsinc.com");
                mail.To.Add("jason.norton@republicmeter.com");
                
                //mail.To.Add("brigette.johnson@beaumonttexas.gov");
                // Add CC and BCC
                // mail.CC.Add("jason.norton@republicmeter.com");
                //mail.Bcc.Add("kunal.sharma@snssystem.com");

                mail.Subject = "Meter Exchange Report – NTX – " + DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
                mail.Body = "Dear Sir,\n\n" +
                             "Please find the attached Export Report for the project mentioned below:\n\n" +
                             "Project Name: Newton TX 625\n" +
                             "Project ID: " + projectId;




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
