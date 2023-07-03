using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using System.Data;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using GarronT.CMS.Model.DAL;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model;
using GarronT.CMS.UI.WebAPI;
using System.Configuration;

namespace TNMPUploadUtility
{
    class UploadFile
    {
        public string getandSaveFile(long ProjectID)
        {
            try
            {
                #region Connect SFTP server
                ErrorLogFile.writeStatus("Connect SFTP server - " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Connect SFTP server");
                /*string host = @"sftp.datadepotonline.com";
                string username = "tnmp";
                string password = @"03DDO@Tnmp@0622";*/

                string host = ConfigurationManager.AppSettings["sftpurl"].ToString();
                string username = ConfigurationManager.AppSettings["sftpuid"].ToString();
                string password = ConfigurationManager.AppSettings["sftppwd"].ToString();

                string pathLocalFile = string.Empty;
                using (SftpClient sftp = new SftpClient(host, username, password))
                {
                    try
                    {
                        // Path to file on SFTP server
                        //string remoteDirectory = "/home/tnmp/prod/";
                        string remoteDirectory = ConfigurationManager.AppSettings["remoteDir"].ToString();
                        sftp.Connect();
                        ErrorLogFile.writeStatus("Connected SFTP server - " + DateTime.Now.ToShortTimeString());
                        Console.WriteLine("Connected SFTP server");
                        var files = sftp.ListDirectory(remoteDirectory);
                        if (files != null)
                        {
                            DateTime date = new CommonFunctions().ServerDate(); // Get current date
                            date = date.AddDays(-1);        //Add -1 days because upload file coming in SFTP server yesterday
                            string tempFileName = string.Empty;
                            if (ProjectID == 10108)     // For This Project ==> TNMP Texas V10
                            {
                                tempFileName = "TNMP_CMS_RouteFile_" + date.ToString("yyyyMMdd") + ".csv";
                            }
                            else if (ProjectID==10087)  // For TNMP HQ Project ==> Texas New Mexico Power HQ
                            {
                                tempFileName = "TNMP_TNMP_RouteFile_" + date.ToString("yyyyMMdd") + ".csv";
                            }
                            var obj = files.Select(o => o.Name).ToList();
                            string serverFileName = string.Empty;
                            serverFileName = files.Where(o => o.Name == tempFileName).Select(o => o.Name).FirstOrDefault();
                            /*foreach (var item in obj)
                            {
                                bool fileExist = item.StartsWith("TNMP_CMS_RouteFile");
                                if (fileExist)
                                { 
                                    serverFileName = item.ToString();
                                    break;
                                }
                            }*/

                            if (!string.IsNullOrEmpty(serverFileName))
                            {
                                // Path to file on SFTP server
                                //string pathRemoteFile = "/home/tnmp/prod/" + serverFileName + "";
                                string path = ConfigurationManager.AppSettings["pathRemoteFile"].ToString();
                                string pathRemoteFile = path + serverFileName + "";
                                serverFileName = serverFileName.Split('.')[0].ToString();

                                // Path where the file should be saved once downloaded (locally)
                                string baseDir = System.AppDomain.CurrentDomain.BaseDirectory + "UploadData";
                                if (!System.IO.Directory.Exists(@"" + baseDir + ""))
                                {
                                    System.IO.Directory.CreateDirectory(@"" + baseDir + "");
                                }
                                pathLocalFile = baseDir + "\\"+ serverFileName + "_" + DateTime.Now.ToString("MMM-dd-yyy_hh-mm-ss") + ".csv";

                                Console.WriteLine("Downloading {0}", pathRemoteFile);
                                using (Stream fileStream = File.OpenWrite(pathLocalFile))
                                {
                                    sftp.DownloadFile(pathRemoteFile, fileStream);
                                    ErrorLogFile.writeStatus("File Successfully Save in Server Folder - " + DateTime.Now.ToShortTimeString());
                                    Console.WriteLine("File Successfully Save in Server Folder");

                                    string movingFilePath = ConfigurationManager.AppSettings["movingfilepathRemoteFile"].ToString();
                                    sftp.RenameFile(pathRemoteFile, movingFilePath + serverFileName + ".csv");
                                    //sftp.RenameFile(pathRemoteFile, "/home/tnmp/prod/old-files/" + serverFileName + ".csv");
                                    ErrorLogFile.writeStatus("Moved file another directory -" + DateTime.Now.ToShortTimeString());
                                    Console.WriteLine("Moved file another directory");
                                    fileStream.Close();
                                }
                            }
                        }
                        sftp.Disconnect();
                        ErrorLogFile.writeStatus("Disconnect SFTP server -" + DateTime.Now.ToShortTimeString());
                        Console.WriteLine("Disconnect SFTP server");
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine("An exception has been caught " + e.ToString());
                        ErrorLogFile.writeStatus("An exception has been caught " + e.ToString() + DateTime.Now.ToShortTimeString());
                        Console.WriteLine("An exception has been caught " + e.ToString());
                    }
                }
                return pathLocalFile;
                #endregion
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                ErrorLogFile.writeStatus("Error:- " + ex.Message.ToString() + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Error:- " + ex.Message.ToString());
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newFilePath"></param>
        /// <param name="List1"></param>
        /// <returns></returns>
        public Tuple<List<string>, DataTable> ValidateExcel(string newFilePath)
        {
            ErrorLogFile.writeStatus("Validate Excel - " + DateTime.Now.ToShortTimeString());
            Console.WriteLine("Validate Excel....");
            List<string> objList1 = new List<string>();
            //TempData["ExcelData"] = null;
            try
            {
                string[] fileTypeArray = newFilePath.Split('.');
                string fileType = fileTypeArray[1];
                System.Data.DataTable dt = new System.Data.DataTable();
                //List1 = null;
                FileInfo existingFile = new FileInfo(newFilePath);
                if (isExcel(existingFile))
                {
                    #region getExcel data
                    using (ExcelPackage xlPackage = new ExcelPackage(existingFile))
                    {
                        ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];
                        dt = new DataTable();
                        //adding columns  
                        int iCol = 1;
                        int iRow = 1;
                        bool CanRead = true;
                        while (CanRead)
                        {
                            if (worksheet.Cells[iRow, iCol].Value != null)
                            {
                                if (worksheet.Cells[iRow, iCol].Value != DBNull.Value && !string.IsNullOrEmpty(Convert.ToString(worksheet.Cells[iRow, iCol].Value)))
                                {
                                    dt.Columns.Add(Convert.ToString(worksheet.Cells[iRow, iCol].Value));
                                    objList1.Add(Convert.ToString(worksheet.Cells[iRow, iCol].Value));
                                    iCol++;
                                }
                                else
                                {
                                    CanRead = false;
                                }
                            }
                            else
                            {
                                CanRead = false;
                            }
                        }
                        //adding rows  
                        iRow = 2;
                        bool canRowRead = true;
                        while (canRowRead)
                        {
                            DataRow dr = dt.NewRow();
                            bool rowVal = true;
                            int colCount = 1;
                            while (colCount <= iCol)
                            {
                                if (worksheet.Cells[iRow, colCount].Value != DBNull.Value && !string.IsNullOrEmpty(Convert.ToString(worksheet.Cells[iRow, colCount].Value)))
                                {
                                    try
                                    {
                                        dr[colCount - 1] = worksheet.Cells[iRow, colCount].Value;
                                    }
                                    catch
                                    {
                                    }
                                    rowVal = false;
                                }
                                colCount++;
                            }

                            if (rowVal)
                            {
                                canRowRead = false;
                            }
                            else
                            {
                                dt.Rows.Add(dr);
                                iRow++;
                            }
                        }
                        /*TempData["ExcelData"] = dt;
                        TempData.Keep("ExcelData");*/
                    }
                    #endregion
                }
                else
                {
                    #region getCSV data
                    //csv file
                    string Fulltext;
                    using (StreamReader sr = new StreamReader(newFilePath))
                    {
                        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                        dt = new DataTable();
                        while (!sr.EndOfStream)
                        {
                            Fulltext = sr.ReadToEnd().ToString(); //read full file text  
                            string[] rows = Fulltext.Split('\n'); //split full file text into rows  

                            for (int i = 0; i < rows.Count(); i++)
                            { 
                                string[] rowValues = CSVParser.Split(rows[i]);
                                if (!string.IsNullOrEmpty(rowValues[0]))
                                {
                                    if (i == 0)
                                    {
                                        for (int j = 0; j < rowValues.Count(); j++)
                                        {
                                            string rvalue = rowValues[j].TrimStart(' ', '"');
                                            rvalue = rvalue.TrimEnd('"');
                                            rvalue = rvalue.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
                                            rvalue = rvalue.ToString().TrimEnd();
                                            dt.Columns.Add(rvalue); //add headers 
                                            objList1.Add(rvalue);
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            DataRow dr = dt.NewRow();
                                            for (int k = 0; k < rowValues.Count(); k++)
                                            {
                                                string rvalue = rowValues[k].ToString().TrimStart(' ', '"');
                                                rvalue = rvalue.TrimEnd('"');
                                                rvalue = rvalue.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
                                                rvalue = rvalue.ToString().TrimEnd();
                                                dr[k] = rvalue;
                                            }
                                            dt.Rows.Add(dr); //add other rows  
                                        }
                                        catch (Exception ex)
                                        {
                                            string obj = ex.Message.ToString();
                                            ErrorLogFile.writeStatus("Error at File Uploading - " + DateTime.Now.ToString() + " -" + ex.Message);
                                            Console.WriteLine("Error at File Uploading - " + DateTime.Now.ToString() + " -" + ex.Message);
                                        }

                                    }
                                }
                            }
                        }
                        /*TempData["ExcelData"] = dt;
                        TempData.Keep("ExcelData");*/
                    }
                    #endregion
                }
                var result = new Tuple<List<string>, DataTable>(objList1, dt);
                ErrorLogFile.writeStatus("Successfully validate excel file - "+ DateTime.Now.ToShortTimeString());
                Console.WriteLine("Successfully validate excel file");
                return result;
            }
            catch (Exception ex)
            {
                ErrorLogFile.writeStatus("Error - " + ex.Message.ToString()+" - " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Error - " + ex.Message.ToString());
                throw ex;
            }
        }

        static bool isExcel(FileInfo wbook)
        {
            bool isOpened = true;

            try
            {
                ExcelPackage xlPackage = new ExcelPackage(wbook);
            }
            catch (Exception)
            {
                isOpened = false;
            }
            return isOpened;
        }

        #region Get excel mappimg field ids
        public List<string> excelmMappingFieldIds(long ProjectID, List<string> ExcelField)
        {
            List<string> FieldId = new List<string>();
            try
            {
                string returnMessage = "";
                List<ExcelSheetBO> objExcelMapping = new ProjectMasterDAL().GetOldExcelMappingdata(Convert.ToInt32(ProjectID), out returnMessage);
                //List<string> tempdata = ExcelField;
                List<VW_GETTab6DropdownData> objData = new List<VW_GETTab6DropdownData>();
                using (ProjectMasterDAL obj = new ProjectMasterDAL())
                {
                    objData = obj.GetViewData(ProjectID);
                }

                if (objExcelMapping != null && objData.Count > 0)
                {
                    foreach (var item in objExcelMapping)
                    {
                        foreach (var item1 in objData)
                        {
                            if (item.FieldID == item1.tabelId && item.FieldType == item1.FieldType)
                            {
                                FieldId.Add(Convert.ToString(item1.Id));
                            }
                        }
                    }
                }
                /*foreach (var item in tempdata)
                {
                    foreach (var item1 in objData)
                    {
                        *//*string checkMatch = item1.FieldName.ToLower().Trim() == item.ToLower().Trim() ? "selected" : "";
                        s = s + "<option value='" + item1.Id + "' " + checkMatch + ">" + item1.FieldName + "</option>";*//*
                        FieldId.Add(Convert.ToString(item1.Id));
                    }
                }*/
                return FieldId;
            }
            catch (Exception ex)
            {
                string d = ex.Message.ToString();
                return null;
            }
        }
        #endregion

        #region Save Excel data
        public bool SaveTab6Data(long ProjectID, List<string> ExcelField, List<string> FieldID, DataTable dt)
        {
            bool status = false;
            try
            {
                //DataTable dt = new DataTable();
                List<ExcelSheetBO> objobjExcelSheetBOList = new List<ExcelSheetBO>();
                if ((ExcelField != null || ExcelField.Count != 0)
                    && (FieldID != null || FieldID.Count != 0)
                    && ExcelField.Count == FieldID.Count)
                {
                    if (dt == null)
                    {
                        return status = false;
                    }
                    else
                    {
                        List<VW_GETTab6DropdownData> objData = new List<VW_GETTab6DropdownData>();
                        using (ProjectMasterDAL obj = new ProjectMasterDAL())
                        {
                            objData = obj.GetViewData(ProjectID);
                        }
                        int i = 0;
                        foreach (var item in FieldID)
                        {
                            long m = long.Parse(item);

                            var b = objData.Where(x => x.Id == m).Select(o => o).FirstOrDefault();
                            ExcelSheetBO objExcelSheetBO = new ExcelSheetBO();

                            objExcelSheetBO.FieldID = b != null ? b.tabelId : 0;
                            objExcelSheetBO.FieldType = b != null ? b.FieldType : 0;


                            objExcelSheetBO.FieldName = ExcelField[i];

                            objobjExcelSheetBOList.Add(objExcelSheetBO);

                            i++;
                        }
                        TblProjectExcelMapping objTblProjectExcelMapping = new TblProjectExcelMapping();

                        string returnMessage = "";

                        bool success = false;

                        string MSg = "";
                        //int UserId = CommonFunctions.GetLoggedUserID(User.Identity.Name);
                        int UserId = 14;  // Defualt pass user id ==> Garron
                        objTblProjectExcelMapping.CreatedBy = UserId;
                        objTblProjectExcelMapping.Active = 1;

                        DataTable dt1;
                        string fileName = "";
                        bool isLatlongCalucalted = false;

                        int newRecordsCount = 0;
                        int skipedRecordsCount = 0;
                        int removeRecordsCount = 0;
                        int updatedRecordsCount = 0;
                        string skipedAccountsNo = string.Empty;
                        string newAccountsNo = string.Empty;
                        string removeAccountsNo = string.Empty;
                        List<officeCollection> objofficeCollection = new List<officeCollection>();
                        success = new ProjectMasterDAL().CreateOrUpdateExcelMappingData(objTblProjectExcelMapping, ProjectID,
                            objobjExcelSheetBOList, UserId, dt, out returnMessage, out dt1, out isLatlongCalucalted,
                            out newRecordsCount, out newAccountsNo,
                            out skipedRecordsCount, out skipedAccountsNo,
                            out removeRecordsCount, out removeAccountsNo,
                            out updatedRecordsCount, out objofficeCollection);

                        CompassEntities objCompassEntities = new CompassEntities();
                        var objProjectdetail = objCompassEntities.tblProjects.Where(o => o.ProjectId == ProjectID).FirstOrDefault();
                        var loginUserDetail = objCompassEntities.tblUsers.Where(o => o.UserID == UserId && o.Active == 1).FirstOrDefault();

                        List<String> mailto = new List<String>();
                        mailto.Add(loginUserDetail.Email);

                        List<String> mailcc = new List<String>();
                        //mailcc.Add("garron@garront.com");
                        mailcc.Add("yogendra.rajpoot@snssystem.com");
                        mailcc.Add("kunal.sharma@snssystem.com");
                        //mailcc.Add("snssystem.kunnal@gmail.com");
                        if (ProjectID == 10108)     // for project TNMP Texas V10
                        {
                            /*mailcc.Add("awright@cmsmeter.com");       // Removed all mail id from Client side.
                            mailcc.Add("bob@cmsmeter.com");
                            mailcc.Add("sporter@cmsmeter.com");
                            mailcc.Add("jmurillo@cmsmeter.com");
                            mailcc.Add("jking@cmsmeter.com");
                            mailcc.Add("jfoster@cmsmeter.com");
                            mailcc.Add("cworkman@cmsmeter.com");
                            mailcc.Add("christopher.gerety@tnmp.com");
                            mailcc.Add("garron@cmsmeter.com");*/
                        }
                        if (ProjectID == 10087)     // for project Texas New Mexico Power HQ
                        {
                            mailcc.Add("celeste.stephenson@tnmp.com");
                            mailcc.Add("andrea.couch@tnmp.com");
                            mailcc.Add("christopher.gerety@tnmp.com");
                            mailcc.Add("darice.taipalus@pnmresources.com");
                            mailcc.Add("michael.stanley@pnmresources.com");
                            mailcc.Add("cisproduction@pnmresources.com");
                            mailcc.Add("garron@cmsmeter.com");
                            mailcc.Add("tommy.gauna@tnmp.com");
                            mailcc.Add("pauline.moore@tnmp.com");
                        }

                        string eMailBody = string.Empty;
                        if (objProjectdetail.Utilitytype == "Electric")
                        {
                            eMailBody = "Excel file uploaded successfully.<br /><br />" +
                                        "Project Name: <b>" + objProjectdetail.ProjectName + "</b><br />" +
                                        "Project Id: <b>" + ProjectID + "</b><br />" +
                                        "Total New Records: <b>" + newRecordsCount + "</b><br />" +
                                        "Total Skipped Records: <b>" + skipedRecordsCount + "</b><br />" +
                                        "Total Removed Records: <b>" + removeRecordsCount + "</b><br /><br />" +
                                        "<br /><b>Accounts Number Summary</b><br /><br />";

                            eMailBody += "<div class='container'>" +
                                        "<table class='table table-bordered' border=" + 1 + " cellpadding=" + 0 + " cellspacing=" + 0 + " width = " + 100 + "%" + ">" +
                                        "<tr bgcolor='#80b2df'>" +
                                        "<td><b><center>Office Name</center></b></td> " +
                                        "<td><b><center>New</center></b></td> " +
                                        "<td><b><center>Skipped</center></b></td>" +
                                        "<td><b><center>Removed</center></b></td>" +
                                        "</tr>";
                            var groupedOfficeNameWise = from c in objofficeCollection
                                                        group c by new { objOfcName = c.officeName } into groupby
                                                        select new
                                                        {
                                                            Value = groupby.Key,
                                                            ColumnValues = groupby
                                                        };

                            DataTable temp_dt = new DataTable(); // This table created because skppied count sorted decending order
                            temp_dt.Columns.Add("OfficeName", typeof(string));
                            temp_dt.Columns.Add("New", typeof(int));
                            temp_dt.Columns.Add("Skipped", typeof(int));
                            temp_dt.Columns.Add("Removed", typeof(int));
                            foreach (var item in groupedOfficeNameWise.ToList())
                            {
                                string ofcName = item.Value.objOfcName.ToString();
                                var newCount = item.ColumnValues.Where(o => o.type == "New").Count();
                                var skipCount = item.ColumnValues.Where(o => o.type == "Skip").Count();
                                var removeCount = item.ColumnValues.Where(o => o.type == "Remove").Count();

                                temp_dt.Rows.Add(ofcName, newCount, skipCount, removeCount);
                                /*eMailBody += "<tr>" +
                                             "<td><center>" + ofcName + "</center></td>" +
                                             "<td><center>" + newCount + "</center></td>" +
                                             "<td><center>" + skipCount + "</center></td>" +
                                             "<td><center>" + removeCount + "</center></td>" +
                                             "</tr>";*/
                            }
                            //Sorting the Table
                            DataView dv = temp_dt.DefaultView;
                            dv.Sort = "Skipped desc";
                            DataTable sorteddt = dv.ToTable();
                            foreach (DataRow item in sorteddt.Rows)
                            {
                                eMailBody += "<tr>" +
                                             "<td><center>" + item["OfficeName"].ToString() + "</center></td>" +
                                             "<td><center>" + item["New"].ToString() + "</center></td>" +
                                             "<td><center>" + item["Skipped"].ToString() + "</center></td>" +
                                             "<td><center>" + item["Removed"].ToString() + "</center></td>" +
                                             "</tr>";
                            }

                            eMailBody += "</table></div>";
                            eMailBody += "<br /><br />Thanks and Regards,<br />Team Data Depot";
                            //eMailBody += "<br /><br />Thanks and Regards,<br />Team CMS";
                        }
                        else
                        {
                            eMailBody = "Excel file uploaded successfully.<br /><br />" +
                                               "Project Name: <b>" + objProjectdetail.ProjectName + "</b><br />" +
                                               "Project Id: <b>" + ProjectID + "</b><br />" +
                                               "Total New Records: <b>" + newRecordsCount + "</b><br />" +
                                               "Total Skipped Records: <b>" + skipedRecordsCount + "</b><br />" +
                                               "<br />" + skipedAccountsNo + "" +
                                               "<br /><br />Thanks and Regards,<br />Team Data Depot";
                                               //"<br /><br />Thanks and Regards,<br />Team CMS";
                        }
                        string eMailSubject = "Project Name " + objProjectdetail.ProjectName + " Excel file uploaded";
                        new GarronT.CMS.Model.clsMail().SendMail(mailto, mailcc, eMailBody, eMailSubject);

                        CalculateLatLongController ca = new CalculateLatLongController();
                        ca.GetLatLongForProject(ProjectID);
                        ca.updateVisitIdGroupId(ProjectID);
                        /*Task.Factory.StartNew(() =>
                        {
                            CalculateLatLongController ca = new CalculateLatLongController();
                            ca.GetLatLongForProject(ProjectID);
                            ca.updateVisitIdGroupId(ProjectID);
                        });*/
                        status = true;
                    }
                }
                return status;
            }
            catch (Exception ex)
            {
                return status = false;
            }

        }
        #endregion
    }
}
