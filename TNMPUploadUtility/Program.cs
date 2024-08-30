using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
using GarronT.CMS.Model.DAL;
using GarronT.CMS.Model.BO;
using System.Data;
using GarronT.CMS.UI.WebAPI;
using GarronT.CMS.Model;

namespace TNMPUploadUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Console.WriteLine("Test Succeed or not - " + DateTime.Now.ToShortTimeString());
            Console.ReadLine();*/
            //Note==> Emails comment for a testsing time  ==> UploadFile.cs
            long ProjectID = 10118;
            if (args.Length > 0)
            {
                ProjectID = Convert.ToInt64(args[0]);  // Get project id in Task Schedular Argumnet
            }
            //long ProjectID = 10085; // Static project id
            #region Upload file class
             ErrorLogFile.writeStatus("\n\nThread Started at Uploading file - " + DateTime.Now.ToShortTimeString());
             Console.WriteLine("Thread Started at - " + DateTime.Now.ToShortTimeString());
             ErrorLogFile.writeStatus("Get Project Id ==> Project Id:-" + ProjectID + " -" + DateTime.Now.ToShortTimeString());
             Console.WriteLine("Get Project Id ==> Project Id:-" + ProjectID + "");
             try
             {
                 UploadFile up = new UploadFile();
                 string pathLocalFile = up.getandSaveFile(ProjectID);
                 if (!string.IsNullOrEmpty(pathLocalFile))
                 {
                     // Validate excel execk columns and row data
                     var result = up.ValidateExcel(pathLocalFile);
                     List<string> ExcelField = result.Item1;
                     DataTable dtExcelData = result.Item2;

                     // Get Excel Mapping Field ids
                     ErrorLogFile.writeStatus("Get Excel Mapping Field ids - " + DateTime.Now.ToShortTimeString());
                     Console.WriteLine("Get Excel Mapping Field ids - " + DateTime.Now.ToShortTimeString());
                     List<string> FieldID = up.excelmMappingFieldIds(ProjectID, result.Item1);

                     // Save Excel datas
                     ErrorLogFile.writeStatus("Saving Excel Data in Database - " + DateTime.Now.ToShortTimeString());
                     Console.WriteLine("Saving Excel Data in Database - " + DateTime.Now.ToShortTimeString());
                     bool sratus = up.SaveTab6Data(ProjectID, ExcelField, FieldID, dtExcelData);
                     if (sratus)
                     {
                         ErrorLogFile.writeStatus("Data Successfully Saved in Database - " + DateTime.Now.ToShortTimeString());
                         Console.WriteLine("Data Successfully Saved in Database - " + DateTime.Now.ToShortTimeString());
                         File.Delete(@"" + pathLocalFile + "");
                     }
                 }
                 else
                 {
                     ErrorLogFile.writeStatus("File does not exist in SFTP server. - " + DateTime.Now.ToShortTimeString());
                     Console.WriteLine("File does not exist in SFTP server. - " + DateTime.Now.ToShortTimeString());
                 }
             }
             catch (Exception ex)
             {
                 ErrorLogFile.writeStatus("Error:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
                 Console.WriteLine("Error:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
             }
             ErrorLogFile.writeStatus("Thread Ended at Upload file - " + DateTime.Now.ToShortTimeString());
             Console.WriteLine("Thread Ended at - " + DateTime.Now.ToShortTimeString());
            #endregion
            //Ends uploading file code

            #region Download Export Report
           /*ErrorLogFile.writeStatus("\n\nThread Started at Download Export Report - " + DateTime.Now.ToShortTimeString());
            Console.WriteLine("Thread Started at Download Export Report - " + DateTime.Now.ToShortTimeString());
            ErrorLogFile.writeStatus("Get Project Id ==> Project Id:-" + ProjectID + " -" + DateTime.Now.ToShortTimeString());
            Console.WriteLine("Get Project Id ==> Project Id:-" + ProjectID + "");
            // All Parameter Set defualt
            try
            {
                DateTime date = new CommonFunctions().ServerDate(); // Get current date
                string fromDate = string.Empty;
                string toDate = string.Empty;
                DateTime objUtilityExecutionStartDateTime = date;
                //string fromDate = date.ToString("MMM-dd-yyyy");   //"Apr-25-2022";
                //string toDate = date.ToString("MMM-dd-yyyy");    //"Apr-25-2022";
                //fromDate = "Apr-26-2022";
                //toDate = "Apr-26-2022";
                long formTypeId = 2;   // Form type 2 set because project data, 1 for Audit data
                long projectId = ProjectID; // Project id ==> 10085
                string[] visitStatus = new string[] { "Completed" };  // Only export report ==> Completed
                int latlongRequired = 1;  // latlongRequired set=1 because 1 for not requied, Pass 2 requied
                int mediaRequired = 1;    // mediaRequired set=1 because 1 for not requied, Pass 2 requied

                CompassEntities objCE = new CompassEntities();
                var lastDateTime = objCE.TblTNMPExportReportUtilityHistories.Where(o => o.FK_ProjectId == projectId && o.Active == 1).OrderByDescending(q => q.Id).Select(o => o.ToDateTime).FirstOrDefault();
                if (lastDateTime.HasValue)
                {
                    if (projectId == 10118) // TNMP CMS
                    {
                        //date = date.AddDays(-2);
                        DateTime tempTime = (DateTime)lastDateTime;
                       fromDate = (tempTime.AddMinutes(1)).ToString("yyyy-MM-dd 23:59");
                        fromDate =Convert.ToDateTime(lastDateTime).ToString("MMM-dd-yyyy 23:59");
                     toDate = date.ToString("yyyy-MM-dd HH:mm");
                       // toDate = "2024-03-24 19:50";
                    }
                    if (projectId == 10087) // TNMP HQ
                    {
                        date = date.AddDays(-1);
                        DateTime tempTime = (DateTime)lastDateTime;
                        fromDate = (tempTime.AddMinutes(1)).ToString("yyyy-MM-dd HH:mm");
                        //fromDate = (tempTime).ToString("yyyy-MM-dd 00:00");   // becasue priviou day whole day cover 00:00 to 23:59
                        toDate = date.ToString("yyyy-MM-dd 23:59");
                        //toDate = date.ToString("yyyy-MM-dd HH:mm");
                    }

                    ErrorLogFile.writeStatus("Pass FromDateTime:- " + fromDate + " and ToDateTime:- " + toDate + " to Method  ==>" + DateTime.Now.ToShortTimeString());
                    Console.WriteLine("Pass FromDate:- " + fromDate + " and ToDate :- " + toDate + " to Method");
                }
                // Get data in dt from Databse
                DownloadExportReport objDER = new DownloadExportReport();
                DataTable dt = objDER.createExcelNew(fromDate, toDate, formTypeId, projectId, visitStatus, latlongRequired, mediaRequired);
                if (dt.Rows.Count > 0)
                {
                    string filePath = objDER.DownloadCSVExport(dt, projectId);  // Generate CSV file
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        bool status = objDER.UploadFileSFTPServer(filePath); // Upload file in SFTP server.
                        if (status)
                        {
                            ErrorLogFile.writeStatus("Export File uploaded successfully in SFTP server -" + DateTime.Now.ToShortTimeString());
                            Console.WriteLine("Export File uploaded successfully in SFTP server" + DateTime.Now.ToShortTimeString());

                            // Save TNMP Utility Export Report history
                            TblTNMPExportReportUtilityHistory obj = new TblTNMPExportReportUtilityHistory();
                            obj.FK_ProjectId = projectId;
                            obj.FromDateTime = Convert.ToDateTime(fromDate);
                            obj.ToDateTime = Convert.ToDateTime(toDate);
                            obj.UtilityExecutionStartDateTime = objUtilityExecutionStartDateTime;
                            obj.UtilityExecutionLastDateTime = new CommonFunctions().ServerDate();
                            obj.Active = 1;
                            obj.CreatedBy = 14; // Set defualt user id
                            obj.CreatedOn = new CommonFunctions().ServerDate();
                            objCE.TblTNMPExportReportUtilityHistories.Add(obj);
                            objCE.SaveChanges();
                            // end here
                            ErrorLogFile.writeStatus("Save export report history in database -" + DateTime.Now.ToShortTimeString());
                            Console.WriteLine("Save export report history in database" + DateTime.Now.ToShortTimeString());
                        }
                        else
                        {
                            ErrorLogFile.writeStatus("Export File don't uploaded -" + DateTime.Now.ToShortTimeString());
                            Console.WriteLine("Export File don't uploaded");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogFile.writeStatus("Error:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Error:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
            }
            ErrorLogFile.writeStatus("Thread Ended at Download Export Report - " + DateTime.Now.ToShortTimeString());
            Console.WriteLine("Thread Ended at Download Export Report - " + DateTime.Now.ToShortTimeString());*/
            #endregion
            // Ends ecport report code

            //Start code => SkipAccountMovedToUnAllocate
            #region SkipAccountMovedToUnAllocate
            /*ErrorLogFile.writeStatus("\n\nThread Started at Skip Account Moved To UnAllocate - " + DateTime.Now.ToShortTimeString());
            Console.WriteLine("Thread Started at Skip Account Moved To UnAllocate - " + DateTime.Now.ToShortTimeString());
            ErrorLogFile.writeStatus("Get Project Id ==> Project Id:-" + ProjectID + " -" + DateTime.Now.ToShortTimeString());
            Console.WriteLine("Get Project Id ==> Project Id:-" + ProjectID + "");
            try 
            {
                AccountStatus objAS = new AccountStatus();
                bool status = objAS.SkipAccountMovedToUnAllocate(ProjectID);
                if (status)
                {
                    ErrorLogFile.writeStatus("Successfully saved all the skipped accounts as unallocated - " + DateTime.Now.ToShortTimeString());
                    Console.WriteLine("Successfully saved all the skipped accounts as unallocated " + DateTime.Now.ToShortTimeString());
                    Console.ReadLine();
                }
                else
                {
                    ErrorLogFile.writeStatus("Do not save all the skipped accounts as unallocated - " + DateTime.Now.ToShortTimeString());
                    Console.WriteLine("Do not save all the skipped accounts as unallocated " + DateTime.Now.ToShortTimeString());
                }
            }
            catch (Exception ex)
            {
                ErrorLogFile.writeStatus("Error for SkipAccountMovedToUnAllocate:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Error for SkipAccountMovedToUnAllocate:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
            }
            ErrorLogFile.writeStatus("Thread Ended at Skip Account Moved To UnAllocate - " + DateTime.Now.ToShortTimeString());
            Console.WriteLine("Thread Ended at Skip Account Moved To UnAllocate - " + DateTime.Now.ToShortTimeString());*/
            #endregion
            //ends hare

            // Start code => Upload old files s3 bucket
            #region Upload old files s3 bucket
            /* ErrorLogFile.writeStatus("\n\nThread Started at Upload old files s3 bucket - " + DateTime.Now.ToShortTimeString());
             Console.WriteLine("Thread Started at Upload old files s3 bucket - " + DateTime.Now.ToShortTimeString());
             try
             {
                 UploadFilesS3Bucekt s3bucket = new UploadFilesS3Bucekt();
                 s3bucket.getFilesPath();
             }
             catch (Exception ex)
             {
                 ErrorLogFile.writeStatus("Error for Upload old files s3 bucket:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
                 Console.WriteLine("Error for Upload old files s3 bucket:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
             }
             ErrorLogFile.writeStatus("Thread Ended at Uploaded old files s3 bucket - " + DateTime.Now.ToShortTimeString());
             Console.WriteLine("Thread Ended at Uploaded old files s3 bucket - " + DateTime.Now.ToShortTimeString());
             #endregion
             //ends hare
             #region Upload old files s3 bucket
             DateTime twoDaysAgo = DateTime.Now.AddDays(-6);
             ErrorLogFile.writeStatus("\n\nThread Started at Upload old files s3 bucket - " + twoDaysAgo.ToShortTimeString());

             Console.WriteLine("Thread Started at Upload old files s3 bucket - " + DateTime.Now.ToShortTimeString());
             try
             {
                 thumbnail newtmb = new thumbnail();
                 newtmb.getFilesPath();
             }
             catch (Exception ex)
             {
                 ErrorLogFile.writeStatus("Error for Upload old files s3 bucket:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
                 Console.WriteLine("Error for Upload old files s3 bucket:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
             }
             ErrorLogFile.writeStatus("Thread Ended at Uploaded old files s3 bucket - " + DateTime.Now.ToShortTimeString());
             Console.WriteLine("Thread Ended at Uploaded old files s3 bucket - " + DateTime.Now.ToShortTimeString());*/
            #endregion
            //ends hare

            //#region Upload tumbnail files 
            //    ErrorLogFile.writeStatus("\n\nThread Started at Upload old files - " + DateTime.Now.ToShortTimeString());
            //     Console.WriteLine("Thread Started at Upload old files- " + DateTime.Now.ToShortTimeString());
            //     try
            //     {
            //         thumbnail tmb = new thumbnail();
            //         tmb.getFilesPath();
            //     }
            //     catch (Exception ex)
            //     {
            //         ErrorLogFile.writeStatus("Error for Upload old files:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
            //         Console.WriteLine("Error for Upload old files:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
            //     }
            //     ErrorLogFile.writeStatus("Thread Ended at Uploaded old files - " + DateTime.Now.ToShortTimeString());
            //     Console.WriteLine("Thread Ended at Uploaded old files - " + DateTime.Now.ToShortTimeString());
            //     #endregion
            //     //ends hare
            //     #region Upload old files s3 bucket
            //     DateTime twoDaysAgo = DateTime.Now.AddDays(-6);
            //     ErrorLogFile.writeStatus("\n\nThread Started at Upload old files - " + twoDaysAgo.ToShortTimeString());

            //     Console.WriteLine("Thread Started at Upload old files - " + DateTime.Now.ToShortTimeString());
            //     try
            //     {
            //         thumbnail newtmb = new thumbnail();
            //         newtmb.getFilesPath();
            //     }
            //     catch (Exception ex)
            //     {
            //         ErrorLogFile.writeStatus("Error for Upload old files:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
            //         Console.WriteLine("Error for Upload old files:- " + ex.Message.ToString() + " - " + DateTime.Now.ToShortTimeString());
            //     }
            //     ErrorLogFile.writeStatus("Thread Ended at Uploaded old files - " + DateTime.Now.ToShortTimeString());
            //     Console.WriteLine("Thread Ended at Uploaded old files - " + DateTime.Now.ToShortTimeString());
            //    #endregion
            //    //ends hare

        }
    }
}