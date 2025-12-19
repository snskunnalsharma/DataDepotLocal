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
    class ExportReportByEmail_2
    {
        #region get Excel CVS data in DataTable
        public DataTable createExcelNew(string fromDate, string toDate, long formTypeId, long projectId, string[] visitStatus, int latlongRequired, int mediaRequired)
        {
            DataTable dt = new DataTable();

            try
            {
                ExportReportDAL exportReportDAL = new ExportReportDAL();
                long currentUserId = 274;  // Default user ID

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
                        // Add LATITUDE/LONGITUDE columns for project 10133
                        if (projectId == 10133)
                        {
                            // ===============================
                            // 1️⃣ Keep only required source columns
                            // ===============================
                            string[] requiredColumns =
                            {
        "CYCLE-ROUTE",
        "ACCOUNT",
        "STREET",
        "OLD METER NUMBER",
        "OLD METER FINAL READ",
        "NEW METER NUMBER",
        "NEW METER READING",
        "NEW REGISTER READ",
        "OLD FINAL REGISTER READ",
        "OLD METER RADIO NUMBER",
        "NEW METER RADIO",
        "LOCATION/GPS"
    };

                            for (int i = dt.Columns.Count - 1; i >= 0; i--)
                            {
                                if (!requiredColumns.Contains(dt.Columns[i].ColumnName))
                                {
                                    dt.Columns.RemoveAt(i);
                                }
                            }

                            // ===============================
                            // 2️⃣ Remove rows where BOTH
                            // OLD FINAL REGISTER READ AND
                            // NEW METER NUMBER are blank
                            // ===============================
                            for (int i = dt.Rows.Count - 1; i >= 0; i--)
                            {
                                var oldFinalRegisterReadVal = dt.Rows[i]["OLD FINAL REGISTER READ"];
                                var newMeterNumberVal = dt.Rows[i]["NEW METER NUMBER"];

                                bool isOldFinalBlank =
                                    oldFinalRegisterReadVal == DBNull.Value ||
                                    string.IsNullOrWhiteSpace(oldFinalRegisterReadVal.ToString());

                                bool isNewMeterBlank =
                                    newMeterNumberVal == DBNull.Value ||
                                    string.IsNullOrWhiteSpace(newMeterNumberVal.ToString());

                                // ❌ Remove row ONLY if BOTH are blank
                                if (isOldFinalBlank && isNewMeterBlank)
                                {
                                    dt.Rows.RemoveAt(i);
                                }
                            }

                            // ===============================
                            // 3️⃣ Create final mapped DataTable
                            // ===============================
                            DataTable finalDt = new DataTable();

                            finalDt.Columns.Add("Cycle & Route");
                            finalDt.Columns.Add("Location");
                            finalDt.Columns.Add("Address");
                            finalDt.Columns.Add("Removed Meter");
                            finalDt.Columns.Add("Remove Read");
                            finalDt.Columns.Add("New Meter");
                            finalDt.Columns.Add("Set Read");
                            finalDt.Columns.Add("NEW REGISTER READ");
                            finalDt.Columns.Add("OLD FINAL REGISTER READ");
                            finalDt.Columns.Add("OLD ERT");
                            finalDt.Columns.Add("NEW ERT");
                            finalDt.Columns.Add("Latitude & Longitude");

                            // ===============================
                            // 4️⃣ Populate final DataTable
                            // ===============================
                            foreach (DataRow row in dt.Rows)
                            {
                                string gpsValue = "";
                                if (dt.Columns.Contains("LOCATION/GPS") && row["LOCATION/GPS"] != DBNull.Value)
                                {
                                    gpsValue = row["LOCATION/GPS"].ToString().Trim();
                                }

                                // -------------------------------
                                // New Meter
                                // -------------------------------
                                string newMeter = row["NEW METER NUMBER"] == DBNull.Value
                                                    ? ""
                                                    : row["NEW METER NUMBER"].ToString().Trim();

                                // -------------------------------
                                // Remove Read → only if New Meter exists
                                // -------------------------------
                                string removeRead = "";
                                if (!string.IsNullOrWhiteSpace(newMeter))
                                {
                                    removeRead = row["OLD METER FINAL READ"] == DBNull.Value
                                                    ? ""
                                                    : row["OLD METER FINAL READ"].ToString().Trim();
                                }

                                // -------------------------------
                                // Set Read → only if New Meter exists
                                // -------------------------------
                                string setRead = "";
                                if (!string.IsNullOrWhiteSpace(newMeter))
                                {
                                    setRead = row["NEW METER READING"] == DBNull.Value
                                                ? ""
                                                : row["NEW METER READING"].ToString().Trim();
                                }

                                // -------------------------------
                                // OLD FINAL REGISTER READ
                                // -------------------------------
                                string oldFinalRegisterRead = row["OLD FINAL REGISTER READ"] == DBNull.Value
                                                                ? ""
                                                                : row["OLD FINAL REGISTER READ"].ToString().Trim();

                                // -------------------------------
                                // NEW REGISTER READ
                                // Rule: blank if OLD FINAL REGISTER READ is blank
                                // -------------------------------
                                string newRegisterRead = "";
                                if (!string.IsNullOrWhiteSpace(oldFinalRegisterRead))
                                {
                                    newRegisterRead = row["NEW REGISTER READ"] == DBNull.Value
                                                        ? ""
                                                        : row["NEW REGISTER READ"].ToString().Trim();
                                }

                                finalDt.Rows.Add(
                                    row["CYCLE-ROUTE"],              // Cycle & Route
                                    row["ACCOUNT"],                  // Location
                                    row["STREET"],                   // Address
                                    row["OLD METER NUMBER"],         // Removed Meter
                                    removeRead,                      // Remove Read
                                    newMeter,                        // New Meter
                                    setRead,                         // Set Read
                                    newRegisterRead,                 // NEW REGISTER READ
                                    oldFinalRegisterRead,            // OLD FINAL REGISTER READ
                                    row["OLD METER RADIO NUMBER"],   // OLD ERT
                                    row["NEW METER RADIO"],          // NEW ERT
                                    gpsValue                          // GPS
                                );
                            }

                            finalDt.AcceptChanges();

                            // ===============================
                            // 5️⃣ Replace original table
                            // ===============================
                            dt = finalDt;
                        }






                        /*if (projectId == 10133 && dt.Columns.Contains("LOCATION/GPS"))
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
                        }*/

                        // Clean up row values
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                string columnName = dt.Columns[j].ColumnName;

                                if (dt.Rows[i][j] == DBNull.Value)
                                    dt.Rows[i][j] = "-";

                                /*if (projectId == 10133 && columnName.Equals("STREET_NAME"))
                                {
                                    string streetVal = dt.Rows[i][j].ToString().Trim();
                                    string[] words = streetVal.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    string newStreet = string.Join(" ", words.Skip(1)); // Remove first word
                                    dt.Rows[i][j] = newStreet;
                                }*/
                            }
                        }

                        // Remove V_NEW_LATITUDE and V_NEW_LONGITUDE for project 10133
                        /*if (projectId == 10133)
                        {
                            string[] columnsToRemove = { "V_NEW_LATITUDE", "V_NEW_LONGITUDE" };
                            foreach (string colName in columnsToRemove)
                            {
                                if (dt.Columns.Contains(colName))
                                {
                                    dt.Columns.Remove(colName);
                                }
                            }
                        }*/

                        return dt;
                    }
                    else
                    {
                        string noDataMsg = "No data available for selected date range. - " + DateTime.Now.ToShortTimeString();
                        ErrorLogFile.writeStatus(noDataMsg);
                        Console.WriteLine(noDataMsg);
                        SendFailureEmail(projectId, fromDate, toDate);
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
        public string DownloadCSVExport(DataTable dt, long projectId)
        {
            ErrorLogFile.writeStatus("Start ==> Generate CSV file - " + DateTime.Now.ToShortTimeString());
            Console.WriteLine("Start ==> Generate CSV file - " + DateTime.Now.ToShortTimeString());

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

                // Replace nulls/DBNULL with empty string
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

                // ✅ Set CSV filename
                fileName = (projectId == 10133)
                    ? "New_Reg-Meter_Installed_on _" + DateTime.Now.AddDays(-1).ToString("MMddyyyy") + ".csv"
                    : "New_Reg-Meter_Installed_on _" + DateTime.Now.ToString("yyyyMMdd") + ".csv";

                // ✅ Define export folder
                string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExportReport");
                if (!Directory.Exists(baseDir))
                    Directory.CreateDirectory(baseDir);

                string filePath = Path.Combine(baseDir, fileName);

                //  Write CSV
                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    // Write headers
                    IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(col => "\"" + col.ColumnName + "\"");
                    writer.WriteLine(string.Join(",", columnNames));

                    // Write rows
                    foreach (DataRow row in dt.Rows)
                    {
                        IEnumerable<string> fields = row.ItemArray.Select(field =>
                            "\"" + field.ToString().Replace("\"", "\"\"") + "\"");
                        writer.WriteLine(string.Join(",", fields));
                    }
                }

                ErrorLogFile.writeStatus("CSV file generated successfully - " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("CSV file generated successfully - " + DateTime.Now.ToShortTimeString());

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
        public void SendEmailWithAttachment(long projectId, string filePath)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("datadepotonline@gmail.com");
                //mail.To.Add("kunal.sharma@snssystem.com");
                //mail.To.Add("snssystem.pammi@gmail.com");
                 mail.To.Add("mike.harris@beaumonttexas.gov");
                 mail.To.Add("howard.dennings@beaumonttexas.gov");
                 mail.To.Add("ashley.hoyt@beaumonttexas.gov");
                 mail.To.Add("brigette.johnson@BeaumontTexas.gov");
                // Add CC and BCC
                 mail.CC.Add("garron@cmsmeter.com");
                 mail.CC.Add("jason.norton@republicmeter.com");
                 mail.CC.Add("juston.hargraves@beaumonttexas.gov");
                 mail.CC.Add("kelli.reindel@beaumonttexas.gov");
                 mail.Bcc.Add("kunal.sharma@snssystem.com");

                mail.Subject = "BTX – Daily Meter & Register ERT Replacement Report ("
               + DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy") + ")";

                mail.Body = "Hi Team,\n\n" +
                            "Please find the attached export report for the project listed below.\n\n" +
                            "This report includes all meter and register ERT replacements completed on the last business day, "
                            + DateTime.Now.AddDays(-1).ToString("MMMM dd, yyyy") + ".\n\n" +
                            "If you have any questions or need additional information, please let me know.\n\n" +
                            "Thank you,";

                // "Project ID: " + projectId;

                Attachment attachment = new Attachment(filePath);
                mail.Attachments.Add(attachment);

                SmtpServer.Port = 587;
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

                // Send fallback email
                try
                {
                    MailMessage fallbackMail = new MailMessage();
                    SmtpClient fallbackSmtp = new SmtpClient("smtp.gmail.com");

                    fallbackMail.From = new MailAddress("datadepotonline@gmail.com");
                    fallbackMail.To.Add("kunal.sharma@snssystem.com"); // Notify yourself/admin
                    fallbackMail.To.Add("yogendra.rajpoot@snssystem.com");

                    fallbackMail.Subject = "ALERT: Meter Without Meter Exchange Report Email Not Sent";
                    fallbackMail.Body = "Dear Team,\n\nThe export report email for Project ID: " + projectId +
                                        " was not sent due to an error.\n\nPlease check the system logs for more details.\n\nError:\n" + ex.Message;

                    fallbackSmtp.Port = 587;
                    fallbackSmtp.Credentials = new NetworkCredential("datadepotonline@gmail.com", "wgln xqsd iwge okuu");
                    fallbackSmtp.EnableSsl = true;

                    fallbackSmtp.Send(fallbackMail);

                    ErrorLogFile.writeStatus("Fallback email (failure alert) sent - " + DateTime.Now.ToShortTimeString());
                    Console.WriteLine("Fallback email sent - " + DateTime.Now.ToShortTimeString());
                }
                catch (Exception fallbackEx)
                {
                    ErrorLogFile.writeStatus("Failed to send fallback email: " + fallbackEx.Message);
                    Console.WriteLine("Failed to send fallback email: " + fallbackEx.Message);
                }
            }
        }
        private void SendFailureEmail(long projectId, string fromDate, string toDate)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("datadepotonline@gmail.com");
                mail.To.Add("kunal.sharma@snssystem.com");
                mail.To.Add("yogendra.rajpoot@snssystem.com");

                mail.Subject = "ALERT: No Data in Without Meter Export Report – Project ID: " + projectId;
                mail.Body = "Dear Team,\n\n" +
                            $"The export report from {fromDate} to {toDate} for Project ID: {projectId} did not return any data.\n\n" +
                            "Please verify the system or data entries.";

                smtpServer.Port = 587;
                smtpServer.Credentials = new NetworkCredential("datadepotonline@gmail.com", "wgln xqsd iwge okuu");
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);

                ErrorLogFile.writeStatus("Failure email sent - " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Failure email sent - " + DateTime.Now.ToShortTimeString());
            }
            catch (Exception ex)
            {
                ErrorLogFile.writeStatus("Error sending failure email: " + ex.Message);
                Console.WriteLine("Error sending failure email: " + ex.Message);
            }
        }

    }
}
