using System;
using System.Web;
using AutoMapper;
using System.Linq;
using System.Text;
using System.Reflection;
using GarronT.CMS.Model.BO;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using log4net;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Transactions;
using Microsoft.AspNet.Identity;
using System.Collections;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Security.AccessControl;
using System.Net;

namespace GarronT.CMS.Model.DAL
{
    public class ClockTimeReportDAL
    {
        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(ClockTimeReportDAL));
        #endregion
        public List<WeeksModel> GetWeekRangeDateCollection(List<Tuple<DateTime, DateTime>> weeksrangedate)
        {
            try
            {
                List<WeeksModel> objWeeksModel = new List<WeeksModel>();
                foreach (var item in weeksrangedate)
                {
                    objWeeksModel.Add(
                                new WeeksModel
                                {
                                    WeekRangeDate = Convert.ToString(item).Substring(1, Convert.ToString(item).Length - 2),
                                    //WeekFromDate = item.Item1,
                                    //WeekToDate = item.Item2,
                                    DisplayWeekRangeDate = "Week Date Range : " + item.Item1.ToString("dd-MMM-yyyy") + " To " + item.Item2.ToString("dd-MMM-yyyy")
                                }
                                );
                }
                return objWeeksModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<WeeksModel> GetWeekRangeDateCollection_new(List<Tuple<DateTime, DateTime, int>> weeksrangedate, DateTime projectStartDate)
        {
            try
            {
                List<WeeksModel> objWeeksModel = new List<WeeksModel>();
                var serverDateTime = new CommonFunctions().ServerDate();
                bool status = true;
                int i = 0;
                foreach (var item in weeksrangedate)
                {

                    if (projectStartDate.Year == item.Item2.Year && projectStartDate.Year != serverDateTime.Year)
                    {
                        if (projectStartDate.Date <= item.Item2.Date)
                        {
                            if (status)
                            {
                                objWeeksModel.Add(new WeeksModel
                                {
                                    WeekRangeDate = Convert.ToString(projectStartDate.Date) + ", " + Convert.ToString(item.Item2) + ", " + item.Item3,
                                    DisplayWeekRangeDate = "Week No. : " + item.Item3 + " Week Date Range : " + projectStartDate.Date.ToString("dd-MMM-yyyy") + " To " + item.Item2.ToString("dd-MMM-yyyy"),
                                    SerialNo = i++
                                });
                                status = false;
                            }
                            else
                            {
                                objWeeksModel.Add(new WeeksModel
                                {
                                    WeekRangeDate = Convert.ToString(item).Substring(1, Convert.ToString(item).Length - 2),
                                    DisplayWeekRangeDate = "Week No. : " + item.Item3 + " Week Date Range : " + item.Item1.ToString("dd-MMM-yyyy") + " To " + item.Item2.ToString("dd-MMM-yyyy"),
                                    SerialNo = i++
                                });
                            }
                        }
                    }
                    else if (serverDateTime.Year == item.Item2.Year && serverDateTime.Month >= item.Item1.Month)
                    {
                        if (serverDateTime.Year == item.Item2.Year && serverDateTime.Month >= item.Item1.Month && serverDateTime.Date >= item.Item2.Date)
                        {
                            //if (serverDateTime.Date >= item.Item2.Date)
                            //{
                            objWeeksModel.Add(new WeeksModel
                            {
                                WeekRangeDate = Convert.ToString(item).Substring(1, Convert.ToString(item).Length - 2),
                                DisplayWeekRangeDate = "Week No. : " + item.Item3 + " Week Date Range : " + item.Item1.ToString("dd-MMM-yyyy") + " To " + item.Item2.ToString("dd-MMM-yyyy"),
                                SerialNo = i++
                            });
                            /*}
                          else
                           {
                               objWeeksModel.Add(new WeeksModel
                               {
                                   //WeekRangeDate = Convert.ToString(item).Substring(1, Convert.ToString(item).Length - 2),
                                   WeekRangeDate = Convert.ToString(item.Item1) + ", " + serverDateTime.Date + ", " + item.Item3,
                                   DisplayWeekRangeDate = "Week No. : " + item.Item3 + " Week Date Range : " + item.Item1.ToString("dd-MMM-yyyy") + " To " + serverDateTime.Date.ToString("dd-MMM-yyyy"),
                                   SerialNo = i++
                               });
                               break;
                           }*/
                        }
                        else
                        {
                            objWeeksModel.Add(new WeeksModel
                            {
                                //WeekRangeDate = Convert.ToString(item).Substring(1, Convert.ToString(item).Length - 2),
                                WeekRangeDate = Convert.ToString(item.Item1) + ", " + serverDateTime.Date + ", " + item.Item3,
                                DisplayWeekRangeDate = "Week No. : " + item.Item3 + " Week Date Range : " + item.Item1.ToString("dd-MMM-yyyy") + " To " + serverDateTime.Date.ToString("dd-MMM-yyyy"),
                                SerialNo = i++
                            });
                            break;
                        }
                    }
                    else
                    {
                        objWeeksModel.Add(new WeeksModel
                        {
                            WeekRangeDate = Convert.ToString(item).Substring(1, Convert.ToString(item).Length - 2),
                            DisplayWeekRangeDate = "Week No. : " + item.Item3 + " Week Date Range : " + item.Item1.ToString("dd-MMM-yyyy") + " To " + item.Item2.ToString("dd-MMM-yyyy"),
                            SerialNo = i++
                        });
                    }
                }
                //return objWeeksModel;    
                return objWeeksModel.OrderByDescending(o=>o.SerialNo).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }                                                   
        }

        //Working harenow  currently
        public List<Model_InstalledMeterDetailPayRate> getInstalledMetersDetails(
    long _ProjectID, int installerid, string WeekRangefromDate, string WeekRangetoDate, SqlConnection con)
        {
            // Beaumont 2.1 custom logic only for Project ID 10134
            if (_ProjectID == 10134)
            {
                return GetBeaumont21InstalledMetersDetails(
                    _ProjectID,
                    installerid,
                    WeekRangefromDate,
                    WeekRangetoDate,
                    con
                );
            }

            List<Model_InstalledMeterDetailPayRate> objIMDPR = new List<Model_InstalledMeterDetailPayRate>();

            // Determine which fields to query
            string[] fieldNames;
            if (_ProjectID == 10124 || _ProjectID == 10132)
            {
                fieldNames = new string[] { "NEW METER NUMBER" };
            }
            else if (_ProjectID == 10122)
            {
                fieldNames = new string[] { "NEW METER RADIO" };
            }
            else
            {
                // For all other projects, include both fields
                fieldNames = new string[] { "WORK COMPLETED" };
            }

            DataTable dtMeterSizesData = new DataTable();

            foreach (var fieldName in fieldNames)
            {
                string sqlQuery = $@"
            SELECT ROW_NUMBER() OVER(ORDER BY (SELECT 1)) AS SerianNo,
                   dm.FieldValue AS MeterSize,
                   CONVERT(varchar, pf.visitdatetime, 101) AS CreatedOn,
                   DATENAME(weekday, pf.visitdatetime) AS DayName,
                   FORMAT(pf.visitdatetime,'HH:mm') AS Time,
                   dm.TblProjectFieldData_Id AS ProjectFieldDataId
            FROM uploadedexceldata AS up
            JOIN TblProjectFieldData AS pf ON up.id = pf.fk_uploadedexceldata_id
            JOIN tblProjectFieldDataMaster AS dm ON pf.id = dm.TblProjectFieldData_Id
            WHERE up.FKProjectId = {_ProjectID}
              AND CONVERT(DATE, pf.visitdatetime) BETWEEN CONVERT(DATE, '{WeekRangefromDate}') 
                                                      AND CONVERT(DATE, '{WeekRangetoDate}')
              AND dm.InitialFieldName = '{fieldName}'
              AND dm.CreatedBy = {installerid}
              AND pf.CreatedBy = {installerid}
              AND dm.Active = 1
              AND pf.Active = 1
            ORDER BY dm.id";

                SqlDataAdapter sdaMeterSizeData = new SqlDataAdapter(sqlQuery, con);
                DataTable tempTable = new DataTable();
                sdaMeterSizeData.Fill(tempTable);

                // Merge results for both fields
                dtMeterSizesData.Merge(tempTable);
            }

            if (dtMeterSizesData.Rows.Count > 0)
            {
                var combineData = (from ms in dtMeterSizesData.AsEnumerable()
                                   select new Model_InstalledMeterDetail
                                   {
                                       ProjectId = _ProjectID,
                                       MeterSize = "",
                                       MeterType = "",
                                       CreatedOn = ms.Field<string>("CreatedOn"),
                                       DayName = ms.Field<string>("DayName"),
                                       Time = ms.Field<string>("Time"),
                                       ProjectFieldDataId = ms.Field<long>("ProjectFieldDataId")
                                   }).ToList();

                CompassEntities objCompassEntities = new CompassEntities();

                foreach (var item in combineData)
                {
                    var appliedServices = objCompassEntities.tblProjectFieldDataMasters
                        .Where(o => o.TblProjectFieldData_Id == item.ProjectFieldDataId
                                    && o.InitialFieldName == "SERVICES"
                                    && o.Active == 1)
                        .Select(a => a.FieldValue)
                        .FirstOrDefault();

                    string[] serviceList = (!string.IsNullOrEmpty(appliedServices)) ? appliedServices.Split('|') : null;

                    if (serviceList != null)
                    {
                        foreach (var selectedService in serviceList)
                        {
                            var payData = objCompassEntities.tblMeterPays
                                .Where(o => o.Description == selectedService && o.Active == 1)
                                .FirstOrDefault();

                            if (payData != null)
                            {
                                objIMDPR.Add(new Model_InstalledMeterDetailPayRate
                                {
                                    ProjectId = _ProjectID,
                                    MeterSize = item.MeterSize,
                                    MeterType = item.MeterType,
                                    MeterDisplayName = payData.MeterDisplayName,
                                    CreatedOn = item.CreatedOn,
                                    DayName = item.DayName,
                                    Time = item.Time,
                                    Pay = Convert.ToString(payData.Pay)
                                });
                            }
                        }
                    }
                }
            }

            return objIMDPR;
        }
        //


        // Craete getting installer start or end time
        public List<Model_FinalResult> GetInstallerTotalWorkedTime(long _ProjectID, int installerid, string weekrangedate)
        {
            List<Model_FinalResult> finalResult = new List<Model_FinalResult>();
            try
            {
                

                List<string> weekDayName = new List<string>();      // First of all one static list create in week days as me requirement
                weekDayName.Add("Monday");
                weekDayName.Add("Tuesday");
                weekDayName.Add("Wednesday");
                weekDayName.Add("Thursday");
                weekDayName.Add("Friday");
                weekDayName.Add("Saturday");
                weekDayName.Add("Sunday");

                List<Get_CTR> final_CTR_List = new List<Get_CTR>();
                List<Model_UserWorkedTime> final_List = new List<Model_UserWorkedTime>();
                log.Info("from date - " + weekrangedate);
                var dateArray = weekrangedate.Split(',');
                //var WeekRangefromDate = Convert.ToDateTime(weekrangedate.Split()[0]).ToString("yyyy-MM-dd"); // From date format change as my requirement
                //var WeekRangetoDate = Convert.ToDateTime(weekrangedate.Split()[2]).ToString("yyyy-MM-dd");  // To date format change as my requirement
                var startDate = dateArray[0].Trim().Split()[0];
                var endDate = dateArray[1].Trim().Split()[0];
                log.Info("split from date - " + startDate);
                log.Info("split to date -   " + endDate);
                var WeekRangefromDate = DateTime.Parse(startDate).ToString("yyyy-MM-dd"); // From date format change as my requirement
                var WeekRangetoDate = DateTime.Parse(endDate).ToString("yyyy-MM-dd");  // To date format change as my requirement
                log.Info("from date - " + WeekRangefromDate);
                log.Info("to date -   " + WeekRangefromDate);
                CompassEntities CompassEntities = new CompassEntities();
                SqlConnection con = new SqlConnection(CompassEntities.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                con.Open();
                List<Model_InstalledMeterDetailPayRate> objInstalledMeterDetailPayRate = new List<Model_InstalledMeterDetailPayRate>();
                objInstalledMeterDetailPayRate = getInstalledMetersDetails(_ProjectID, installerid, WeekRangefromDate, WeekRangetoDate, con);
                log.Info("objInstalledMeterDetailPayRate list size - " + objInstalledMeterDetailPayRate.Count);
                // This function use to get All selected week date renage all date list in dynamic form show in html view table in first row
                final_CTR_List = getDynamicDates(Convert.ToDateTime(WeekRangefromDate), Convert.ToDateTime(WeekRangetoDate));
                log.Info("final_CTR_List  size - " + final_CTR_List.Count);
                decimal totalWorkingHoursInWeek = 0.00m;
                int totalInstalledMeterInWeek = 0;
                decimal totalPay = 0.00m;

                if (objInstalledMeterDetailPayRate != null && objInstalledMeterDetailPayRate.Count() > 0)
                {
                    // Datatable set group by week day name wise    //
                    var groupedWeekDayNameWise = from c in objInstalledMeterDetailPayRate
                                                 group c by new { placeCol = c.DayName } into groupby
                                                 select new
                                                 {
                                                     Value = groupby.Key,
                                                     ColumnValues = groupby
                                                 };
                    TimeSpan totalInstallerWorkingHours = new TimeSpan();

                    foreach (var w in weekDayName)
                    {
                        string day = w.ToString();
                        string Dates = string.Empty;
                        string StartTime = string.Empty;
                        string LunchBreak = string.Empty;
                        string EndTime = string.Empty;
                        string TotalHours = string.Empty;
                        foreach (var item in groupedWeekDayNameWise.ToList())
                        {
                            var dbDay = item.Value.placeCol.ToString();
                            if (day == dbDay)
                            {
                                //var itemArray = item.ColumnValues.Select(a => a.MeterDisplayName);
                                //var satrtRecord = itemArray.FirstOrDefault();
                                //var lastRecord = itemArray.LastOrDefault();

                                //var itemArray = item.ColumnValues.Select(a => a.MeterDisplayName);
                                var satrtRecord = item.ColumnValues.FirstOrDefault();
                                var lastRecord = item.ColumnValues.LastOrDefault();

                                TimeSpan st = TimeSpan.Parse(satrtRecord.Time.ToString()).Subtract(TimeSpan.FromMinutes(30));   // st => Start time
                                TimeSpan et = TimeSpan.Parse(lastRecord.Time.ToString());   // et => End time
                                TimeSpan th = et.Subtract(st);                            // th=> Total Hours
                                totalInstallerWorkingHours = totalInstallerWorkingHours.Add(th);

                                StartTime = Convert.ToString(st.ToString(@"hh\:mm"));
                                EndTime = Convert.ToString(et.ToString(@"hh\:mm"));
                                TotalHours = Convert.ToString(th.ToString(@"hh\:mm"));
                            }
                        }
                        final_List.Add(new Model_UserWorkedTime      // Store final display record view in html table view page
                        {
                            //Dates = "",
                            Day = day,
                            StartTime = StartTime,
                            LunchBreak = "-",
                            EndTime = EndTime,
                            TotalHours = TotalHours
                        });
                    }

                    string __Dis = string.Empty;
                    string Monday = string.Empty;
                    string Tuesday = string.Empty;
                    string Wednesday = string.Empty;
                    string Thursday = string.Empty;
                    string Friday = string.Empty;
                    string Saturday = string.Empty;

                    if (final_List != null)
                    {
                        // Fill Start time data
                        foreach (var f in final_List)
                        {
                            __Dis = "START TIME";
                            if (f.Day == "Monday")
                            { Monday = f.StartTime; }
                            if (f.Day == "Tuesday")
                            { Tuesday = f.StartTime; }
                            if (f.Day == "Wednesday")
                            { Wednesday = f.StartTime; }
                            if (f.Day == "Thursday")
                            { Thursday = f.StartTime; }
                            if (f.Day == "Friday")
                            { Friday = f.StartTime; }
                            if (f.Day == "Saturday")
                            { Saturday = f.StartTime; }
                            //TotalTime = TotalTime.Add(f.TotalWorkedTime);   // this use for total week time
                        }
                        final_CTR_List.Add(new Get_CTR      // Store final display Start Time
                        {
                            Description = __Dis,
                            Monday = Monday,
                            Tuesday = Tuesday,
                            Wednesday = Wednesday,
                            Thursday = Thursday,
                            Friday = Friday,
                            Saturday = Saturday,
                            TotalWorkInWeek = ""
                        });

                        // Fill LUNCH BREAK data
                        foreach (var f in final_List)
                        {
                            __Dis = "LUNCH BREAK";
                            if (f.Day == "Monday")
                            { Monday = f.LunchBreak; }
                            if (f.Day == "Tuesday")
                            { Tuesday = f.LunchBreak; }
                            if (f.Day == "Wednesday")
                            { Wednesday = f.LunchBreak; }
                            if (f.Day == "Thursday")
                            { Thursday = f.LunchBreak; }
                            if (f.Day == "Friday")
                            { Friday = f.LunchBreak; }
                            if (f.Day == "Saturday")
                            { Saturday = f.LunchBreak; }
                            //TotalTime = TotalTime.Add(f.TotalWorkedTime);   
                        }
                        final_CTR_List.Add(new Get_CTR      // Store final display Lunch Breck
                        {
                            Description = __Dis,
                            Monday = Monday,
                            Tuesday = Tuesday,
                            Wednesday = Wednesday,
                            Thursday = Thursday,
                            Friday = Friday,
                            Saturday = Saturday,
                            TotalWorkInWeek = ""
                        });

                        // Fill END TIME data
                        foreach (var f in final_List)
                        {
                            __Dis = "END TIME";
                            if (f.Day == "Monday")
                            { Monday = f.EndTime; }
                            if (f.Day == "Tuesday")
                            { Tuesday = f.EndTime; }
                            if (f.Day == "Wednesday")
                            { Wednesday = f.EndTime; }
                            if (f.Day == "Thursday")
                            { Thursday = f.EndTime; }
                            if (f.Day == "Friday")
                            { Friday = f.EndTime; }
                            if (f.Day == "Saturday")
                            { Saturday = f.EndTime; }
                            //TotalTime = TotalTime.Add(f.TotalWorkedTime);   // this use for total week time
                        }
                        final_CTR_List.Add(new Get_CTR      // Store final display End Time
                        {
                            Description = __Dis,
                            Monday = Monday,
                            Tuesday = Tuesday,
                            Wednesday = Wednesday,
                            Thursday = Thursday,
                            Friday = Friday,
                            Saturday = Saturday,
                            TotalWorkInWeek = ""
                        });


                        // Fill TOTAL HOURS data
                        foreach (var f in final_List)
                        {
                            __Dis = "TOTAL HOURS";
                            if (f.Day == "Monday")
                            { Monday = f.TotalHours; }
                            if (f.Day == "Tuesday")
                            { Tuesday = f.TotalHours; }
                            if (f.Day == "Wednesday")
                            { Wednesday = f.TotalHours; }
                            if (f.Day == "Thursday")
                            { Thursday = f.TotalHours; }
                            if (f.Day == "Friday")
                            { Friday = f.TotalHours; }
                            if (f.Day == "Saturday")
                            { Saturday = f.TotalHours; }
                            //TotalTime = TotalTime.Add(f.TotalWorkedTime);   
                        }
                        #region Installer Over all working hours calculation
                        int d = Convert.ToInt32(totalInstallerWorkingHours.Days.ToString());
                        int h = Convert.ToInt32(totalInstallerWorkingHours.Hours.ToString());
                        string m = totalInstallerWorkingHours.Minutes.ToString();
                        if (d > 0)  // d count days 
                        {
                            h += d * 24;    // 1 day == 24 hours
                        }
                        string hm = Convert.ToString(h) + ":" + m;  // hm => Installer Total Working Hours Minutes
                        string ph= Convert.ToString(h) + "." + m;   //Ph=> Print hours value
                        totalWorkingHoursInWeek = 0.00m;
                        totalWorkingHoursInWeek =Convert.ToDecimal(ph);
                        #endregion
                        final_CTR_List.Add(new Get_CTR      // Store final display record view in html table view page
                        {
                            Description = __Dis,
                            Monday = Monday,
                            Tuesday = Tuesday,
                            Wednesday = Wednesday,
                            Thursday = Thursday,
                            Friday = Friday,
                            Saturday = Saturday,
                            TotalWorkInWeek = hm
                        });

                        // METER INSTALL DESCRIPTION
                        final_CTR_List.Add(new Get_CTR     // Store final METER INSTALL DESCRIPTION, Pey, Total new column add
                        {
                            Description = "METER INSTALL DESCRIPTION",
                            Monday = "",
                            Tuesday = "",
                            Wednesday = "",
                            Thursday = "",
                            Friday = "",
                            Saturday = "",
                            TotalWorkInWeek = "",
                            Pay = "Pay",
                            Total = "Total"
                        });
                        //
                    }


                    DataTable dt_AllMeterSizeOrPay = new DataTable();

                    if (_ProjectID == 10134)
                    {
                        dt_AllMeterSizeOrPay = GetProjectTimeClockMeterPayOrder(_ProjectID, con);
                    }
                    else
                    {
                        SqlCommand cnn = new SqlCommand("PROC_GetMeterSizeOrder", con);
                        cnn.Parameters.AddWithValue("@projectId", _ProjectID);
                        cnn.CommandType = CommandType.StoredProcedure;

                        SqlDataAdapter sda = new SqlDataAdapter(cnn);
                        sda.Fill(dt_AllMeterSizeOrPay);
                    }


                    /*SqlDataAdapter sda_AllMeterSizeOrPay = new SqlDataAdapter("select MeterSize,Pay,MeterDisplayName from tblMeterPay where fk_project_id=" + _ProjectID + " and Active=1  order by id", con);
                    DataTable dt_AllMeterSizeOrPay = new DataTable();
                    sda_AllMeterSizeOrPay.Fill(dt_AllMeterSizeOrPay);*/
                    List<Get_CTR> CTR_MeterSizeList = new List<Get_CTR>();
                    totalInstalledMeterInWeek = 0;
                    totalPay = 0.00m;
                    CTR_MeterSizeList = InstalledMeterCalculation(weekDayName, objInstalledMeterDetailPayRate, dt_AllMeterSizeOrPay, ref totalInstalledMeterInWeek, ref totalPay);
                    final_CTR_List.AddRange(CTR_MeterSizeList);

                }
                if (totalPay > 0 && totalInstalledMeterInWeek > 0)
                {
                    log.Info("total pay  > 0 and totalInstalledMeterInWeek > 0");
                    #region get file folder path
                    string clockTimeReportFilePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["ClockTimeReportUploadPath"]);
                    GrantAccess(HttpContext.Current.Server.MapPath(clockTimeReportFilePath));
                    string newFileName = "TimeClockReport " + DateTime.Now.ToString("MMM-dd-yyy_hh-mm-ss") + ".pdf";
                    System.IO.FileStream fileStream = System.IO.File.Create(HttpContext.Current.Server.MapPath(clockTimeReportFilePath) + newFileName);
                    var filepath = clockTimeReportFilePath + newFileName;
                    string WebURLFilePath = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();
                    WebURLFilePath = WebURLFilePath + @"ClockTimeReport/" + newFileName;
                    fileStream.Dispose();
                    #endregion

                    #region add all final data in list for a display purpose
                    finalResult.Add(new Model_FinalResult
                    {
                        TotalInstalledMeterInWeek = totalInstalledMeterInWeek,
                        TotalWorkingHoursInWeek = totalWorkingHoursInWeek,
                        AvgPerHours = Math.Round(totalPay / totalWorkingHoursInWeek, 2),
                        _GetCTR = final_CTR_List,
                        filePath= WebURLFilePath
                    });
                    #endregion

                    decimal AvgPerHours = 0m;
                    AvgPerHours = Math.Round(totalPay / totalWorkingHoursInWeek, 2);
                    var pd = CompassEntities.tblProjects.Where(o => o.ProjectId == _ProjectID).FirstOrDefault(); // pd =projectdetails
                    var installer = CompassEntities.tblUsers.Where(o => o.UserID == installerid).FirstOrDefault();

                    string projectName = string.Empty;
                    if (pd.CityId > 0 && pd.StateId > 0)
                    {
                        var cityName = CompassEntities.TblCityMasters.Where(o => o.CityId == pd.CityId).FirstOrDefault();
                        var stateName = CompassEntities.tblStateMasters.Where(o => o.StateId == pd.StateId).FirstOrDefault();
                        projectName = cityName.CityName + " " + stateName.StateName;
                    }
                    else
                    {
                        projectName = pd.ProjectName;
                    }
                    string contractorName= installer.FirstName + " " + installer.LastName;
                    string contractorLLCName = string.Empty;
                    string contractorPhone =Convert.ToString(installer.MobileNo);
                    string contractorEMail = installer.Email;

                    string projectManager = string.Empty;
                    if (installer.ManagerId != null)
                    { 
                        var pm = CompassEntities.tblUsers.Where(o => o.UserID == installer.ManagerId).FirstOrDefault();
                        projectManager = pm.FirstName + " " + pm.LastName;
                    }
                    else
                    {
                        projectManager = installer.FirstName + " " + installer.LastName;
                    }

                    string weekEndDate = string.Empty;  // Getting week the ending date
                    if (Convert.ToString(Convert.ToDateTime(WeekRangetoDate).DayOfWeek) == "Sunday")
                    { weekEndDate = (Convert.ToDateTime(WeekRangetoDate).AddDays(-1)).ToString("MM/dd/yyyy"); }
                    else { weekEndDate = (Convert.ToDateTime(WeekRangetoDate)).ToString("MM/dd/yyyy"); }

                    //string[] collection = { contractorName, contractorLLCName,contractorEMail, projectName, (projectManager), Convert.ToString(totalInstalledMeterInWeek), Convert.ToString(totalWorkingHoursInWeek), ("$ " + Convert.ToString(AvgPerHours)), weekEndDate, contractorPhone };
                    string[] collection = { projectName, weekEndDate, contractorName, contractorLLCName, contractorPhone, contractorEMail, projectManager, Convert.ToString(totalInstalledMeterInWeek), Convert.ToString(totalWorkingHoursInWeek), ("$ " + Convert.ToString(AvgPerHours)) };
                    #region generate PDF
                    DataTable dt = ToDataTable(final_CTR_List);                    
                    ExportToPdf(dt, fileStream.Name, collection);
                    #endregion
                }
                else
                {
                    finalResult.Add(new Model_FinalResult
                    {
                        TotalInstalledMeterInWeek = 0,
                        TotalWorkingHoursInWeek = 0.00m,
                        AvgPerHours = 0.00m,
                        _GetCTR = final_CTR_List
                    });
                }
                
                return finalResult;
            }
            catch (Exception ex)
            {
                log.Error("Error in DAL GetInstallerTotalWorkedTime Method : ", ex.InnerException);
                return finalResult;
                //throw ex;
            }
        }
        //ends hare

        public List<Get_CTR> getDynamicDates(DateTime startDate, DateTime endDate)
        {
            #region Week Range get all Dynamic dates
            try
            {
                List<Get_CTR> objList = new List<Get_CTR>();
                List<HeaderDates> objHeaderDates = new List<HeaderDates>();
                List<DateTime> datelist = new List<DateTime>();

                while (startDate <= endDate)    // Get week range all date store in DateTime list "datelist"
                {
                    datelist.Add(startDate);
                    startDate = startDate.AddDays(1);
                }
                foreach (var _date in datelist)     // Store data in weeks all dates and Day Name
                {
                    objHeaderDates.Add(new HeaderDates
                    {
                        DayName = _date.DayOfWeek.ToString(),
                        DynamicDate = Convert.ToString(_date.ToString("MMM-dd-yyyy"))
                    });
                }

                String __dis = string.Empty;
                String monday_Date = string.Empty;
                String tuesday_Date = string.Empty;
                String wednesday_Date = string.Empty;
                String thursday_Date = string.Empty;
                String friday_Date = string.Empty;
                String saturday_Date = string.Empty;
                if (objHeaderDates != null)
                {
                    foreach (var f in objHeaderDates)
                    {
                        __dis = "DATES";
                        if (f.DayName == "Monday")
                        { monday_Date = f.DynamicDate; }
                        if (f.DayName == "Tuesday")
                        { tuesday_Date = f.DynamicDate; }
                        if (f.DayName == "Wednesday")
                        { wednesday_Date = f.DynamicDate; }
                        if (f.DayName == "Thursday")
                        { thursday_Date = f.DynamicDate; }
                        if (f.DayName == "Friday")
                        { friday_Date = f.DynamicDate; }
                        if (f.DayName == "Saturday")
                        { saturday_Date = f.DynamicDate; }
                    }
                    objList.Add(new Get_CTR             // Final CTR List First row record dynamic dates store in this list view html table
                    {
                        Description = __dis,
                        Monday = monday_Date,
                        Tuesday = tuesday_Date,
                        Wednesday = wednesday_Date,
                        Thursday = thursday_Date,
                        Friday = friday_Date,
                        Saturday = saturday_Date,
                        TotalWorkInWeek = ""
                    });
                }
                return objList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion
        }

        #region Mater pay calculation this method

        public List<Get_CTR> InstalledMeterCalculation(
            List<string> weekDayName,
            List<Model_InstalledMeterDetailPayRate> objInstalledMeterDetailPayRate,
            DataTable dt_AllMeterSizeOrPay,
            ref int totalInstalledMeterInWeek,
            ref decimal totalPay)
        {
            try
            {
                // Step 1: Deduplicate entries by MeterDisplayName + DayName to avoid double counting
                var distinctMeters = objInstalledMeterDetailPayRate
      .GroupBy(x => new { x.MeterDisplayName, x.DayName, x.Pay })
      .Select(g => new { g.Key.MeterDisplayName, g.Key.DayName, g.Key.Pay, MeterCount = g.Count() })
      .ToList();

                // Step 2: Initialize list for meter size details
                List<Model_MeterSizeDetail> MeterSizeDetail = new List<Model_MeterSizeDetail>();

                // Step 3: Build MeterSizeDetail list for each day
                foreach (var w in weekDayName)
                {
                    string Day = w;
                    foreach (var item in distinctMeters)
                    {
                        if (item.DayName == Day)
                        {
                            MeterSizeDetail.Add(new Model_MeterSizeDetail
                            {
                                Day = Day,
                                MeterDisplayName = item.MeterDisplayName,
                                MeterCount = item.MeterCount.ToString(),
                                Pay = item.Pay
                            });
                        }
                    }
                }

                // Step 4: Initialize list for final report
                List<Get_CTR> CTR_MeterSizeList = new List<Get_CTR>();
                decimal total_Pay = 0m;
                int total_meter = 0;

                // Step 5: Build row-wise totals
                foreach (DataRow msize in dt_AllMeterSizeOrPay.Rows)
                {
                    string meterDisplayName = msize["MeterDisplayName"].ToString();
                    decimal pay = Convert.ToDecimal(msize["Pay"]);

                    var dayWiseRows = MeterSizeDetail.Where(x => x.MeterDisplayName == meterDisplayName).ToList();

                    int rowTotalMeter = 0;
                    decimal rowTotalPay = 0;

                    string Monday = "0", Tuesday = "0", Wednesday = "0", Thursday = "0", Friday = "0", Saturday = "0";

                    foreach (var f in dayWiseRows)
                    {
                        int meterCount = string.IsNullOrEmpty(f.MeterCount) ? 0 : Convert.ToInt32(f.MeterCount);
                        rowTotalMeter += meterCount;
                        rowTotalPay += meterCount * pay;

                        switch (f.Day)
                        {
                            case "Monday": Monday = meterCount.ToString(); break;
                            case "Tuesday": Tuesday = meterCount.ToString(); break;
                            case "Wednesday": Wednesday = meterCount.ToString(); break;
                            case "Thursday": Thursday = meterCount.ToString(); break;
                            case "Friday": Friday = meterCount.ToString(); break;
                            case "Saturday": Saturday = meterCount.ToString(); break;
                        }
                    }

                    total_meter += rowTotalMeter;
                    total_Pay += rowTotalPay;

                    CTR_MeterSizeList.Add(new Get_CTR
                    {
                        Description = meterDisplayName,
                        Monday = Monday,
                        Tuesday = Tuesday,
                        Wednesday = Wednesday,
                        Thursday = Thursday,
                        Friday = Friday,
                        Saturday = Saturday,
                        TotalWorkInWeek = rowTotalMeter.ToString(),
                        Pay = "$ " + pay,
                        Total = "$ " + rowTotalPay.ToString("0.00")
                    });
                }

                // Step 6: Calculate bottom total row from displayed rows only
                int Mon = CTR_MeterSizeList.Sum(x => string.IsNullOrEmpty(x.Monday) ? 0 : Convert.ToInt32(x.Monday));
                int Tue = CTR_MeterSizeList.Sum(x => string.IsNullOrEmpty(x.Tuesday) ? 0 : Convert.ToInt32(x.Tuesday));
                int Wed = CTR_MeterSizeList.Sum(x => string.IsNullOrEmpty(x.Wednesday) ? 0 : Convert.ToInt32(x.Wednesday));
                int Thu = CTR_MeterSizeList.Sum(x => string.IsNullOrEmpty(x.Thursday) ? 0 : Convert.ToInt32(x.Thursday));
                int Fri = CTR_MeterSizeList.Sum(x => string.IsNullOrEmpty(x.Friday) ? 0 : Convert.ToInt32(x.Friday));
                int Sat = CTR_MeterSizeList.Sum(x => string.IsNullOrEmpty(x.Saturday) ? 0 : Convert.ToInt32(x.Saturday));

                int finalTotalMeter = Mon + Tue + Wed + Thu + Fri + Sat;

                // Step 7: Add bottom total row
                CTR_MeterSizeList.Add(new Get_CTR
                {
                    Description = "Total",
                    Monday = Mon.ToString(),
                    Tuesday = Tue.ToString(),
                    Wednesday = Wed.ToString(),
                    Thursday = Thu.ToString(),
                    Friday = Fri.ToString(),
                    Saturday = Sat.ToString(),
                    TotalWorkInWeek = finalTotalMeter.ToString(),
                    Pay = "",
                    Total = "$ " + total_Pay.ToString("0.00")
                });

                // Step 8: Update out parameters
                totalInstalledMeterInWeek = finalTotalMeter;
                totalPay = total_Pay;

                // Step 9: Return final list
                return CTR_MeterSizeList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties by using reflection   
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names  
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {

                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        public void ExportToPdf(DataTable dt, string strFilePath, string[] collection)
        {
            try
            {
                // Landscape Letter with balanced margins
                Document document = new Document(PageSize.LETTER.Rotate(), 28f, 28f, 22f, 22f);

                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(strFilePath, FileMode.Create));
                document.Open();

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 13f, BaseColor.BLACK);
                Font infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 9f, BaseColor.BLACK);
                Font infoBoldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9f, BaseColor.BLACK);
                Font tableFont = FontFactory.GetFont(FontFactory.HELVETICA, 7.2f, BaseColor.BLACK);
                Font tableBoldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7.2f, BaseColor.BLACK);
                Font summaryFont = FontFactory.GetFont(FontFactory.HELVETICA, 9f, BaseColor.BLACK);

                // =========================
                // Header title
                // =========================
                Paragraph title = new Paragraph();
                title.Alignment = Element.ALIGN_CENTER;
                title.SetLeading(0f, 1.05f);
                title.SpacingBefore = 0f;
                title.SpacingAfter = 4f;

                title.Add(new Chunk(collection[0].ToString() + " Project", titleFont));
                title.Add(new Chunk("\nContractor's Weekly Install Sheet", titleFont));

                document.Add(title);

                // =========================
                // Top info section
                // Left side: Week Ending / Project Name
                // Right side: Contractor details
                // =========================
                // =========================
                // Top contractor/project info
                // Better alignment: left info + contractor details on far right
                // =========================
                PdfPTable topInfo = new PdfPTable(3);
                topInfo.WidthPercentage = 100;
                topInfo.SetWidths(new float[] { 2.1f, 2.0f, 2.1f });
                topInfo.SpacingBefore = 0f;
                topInfo.SpacingAfter = 4f;
                topInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                // LEFT BLOCK
                PdfPTable leftInfo = new PdfPTable(1);
                leftInfo.WidthPercentage = 100;
                leftInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                leftInfo.AddCell(CreateNoBorderPdfCell("\n", infoFont, Element.ALIGN_LEFT));
                leftInfo.AddCell(CreateNoBorderPdfCell("Week Ending: " + collection[1].ToString(), infoFont, Element.ALIGN_LEFT));
                leftInfo.AddCell(CreateNoBorderPdfCell("\nProject Name: " + collection[0].ToString(), infoFont, Element.ALIGN_LEFT));

                // CENTER BLANK BLOCK
                PdfPCell centerBlank = new PdfPCell(new Phrase(""));
                centerBlank.Border = Rectangle.NO_BORDER;

                // RIGHT CONTRACTOR BLOCK
                PdfPTable rightInfo = new PdfPTable(1);
                rightInfo.WidthPercentage = 100;
                rightInfo.DefaultCell.Border = Rectangle.NO_BORDER;

                rightInfo.AddCell(CreateNoBorderPdfCell("Contractor Name: " + collection[2].ToString(), infoFont, Element.ALIGN_LEFT));
                rightInfo.AddCell(CreateNoBorderPdfCell("Contractor LLC. Name: " + collection[3].ToString(), infoFont, Element.ALIGN_LEFT));
                rightInfo.AddCell(CreateNoBorderPdfCell("Contractor Phone: " + collection[4].ToString(), infoFont, Element.ALIGN_LEFT));
                rightInfo.AddCell(CreateNoBorderPdfCell("Contractor E-Mail: " + collection[5].ToString(), infoFont, Element.ALIGN_LEFT));
                rightInfo.AddCell(CreateNoBorderPdfCell("Project Manager: " + collection[6].ToString(), infoFont, Element.ALIGN_LEFT));

                PdfPCell leftWrap = new PdfPCell(leftInfo);
                leftWrap.Border = Rectangle.NO_BORDER;
                leftWrap.Padding = 0f;
                leftWrap.VerticalAlignment = Element.ALIGN_BOTTOM;

                PdfPCell rightWrap = new PdfPCell(rightInfo);
                rightWrap.Border = Rectangle.NO_BORDER;
                rightWrap.Padding = 0f;
                rightWrap.PaddingRight = 4f;
                rightWrap.VerticalAlignment = Element.ALIGN_TOP;

                topInfo.AddCell(leftWrap);
                topInfo.AddCell(centerBlank);
                topInfo.AddCell(rightWrap);

                document.Add(topInfo);

                // =========================
                // Main table
                // =========================
                PdfPTable table = new PdfPTable(dt.Columns.Count);
                table.WidthPercentage = 100;
                table.HorizontalAlignment = Element.ALIGN_CENTER;
                table.SpacingBefore = 0f;
                table.SpacingAfter = 10f;
                table.SplitLate = false;
                table.SplitRows = true;

                float[] widths = new float[]
                {
            2.45f,  // Description
            1.08f,  // Monday
            1.08f,  // Tuesday
            1.08f,  // Wednesday
            1.08f,  // Thursday
            1.08f,  // Friday
            1.08f,  // Saturday
            0.95f,  // Total Work In Week
            0.95f,  // Pay
            1.00f   // Total
                };

                table.SetWidths(widths);

                // Header row
                foreach (DataColumn c in dt.Columns)
                {
                    string headerText;

                    if (c.ColumnName == "TotalWorkInWeek")
                    {
                        headerText = "Total Work\nIn Week";
                    }
                    else if (c.ColumnName == "Pay" || c.ColumnName == "Total")
                    {
                        headerText = "";
                    }
                    else
                    {
                        headerText = c.ColumnName;
                    }

                    PdfPCell headerCell = CreateBorderPdfCell(headerText, tableBoldFont, Element.ALIGN_CENTER);
                    headerCell.PaddingTop = 3f;
                    headerCell.PaddingBottom = 3f;
                    table.AddCell(headerCell);
                }

                // Data rows
                foreach (DataRow r in dt.Rows)
                {
                    string description = Convert.ToString(r["Description"]);

                    bool isSectionRow =
                        description == "METER INSTALL DESCRIPTION" ||
                        description == "Total";

                    for (int h = 0; h < dt.Columns.Count; h++)
                    {
                        string cellText = r[h] == null ? "" : r[h].ToString();

                        int alignment = Element.ALIGN_CENTER;

                        // Description column
                        if (h == 0)
                        {
                            alignment = Element.ALIGN_CENTER;
                        }

                        // Pay and Total columns right aligned, except title cells
                        if ((h == 8 || h == 9) && cellText != "Pay" && cellText != "Total")
                        {
                            alignment = Element.ALIGN_RIGHT;
                        }

                        Font activeFont = isSectionRow ? tableBoldFont : tableFont;

                        PdfPCell cell = CreateBorderPdfCell(cellText, activeFont, alignment);

                        if (isSectionRow)
                        {
                            cell.PaddingTop = 3f;
                            cell.PaddingBottom = 3f;
                        }

                        table.AddCell(cell);
                    }
                }

                document.Add(table);

                // =========================
                // Bottom summary + signature section
                // =========================
                // =========================
                // Bottom summary + signature section
                // Clean alignment: summary left, signature center-right, dates far right
                // =========================
                // =========================
                // Bottom summary + signatures side by side
                // =========================
                PdfPTable bottom = new PdfPTable(6);
                bottom.WidthPercentage = 100;
                bottom.SetWidths(new float[] { 1.05f, 1.05f, 1.05f, 1.75f, 1.75f, 0.95f });
                bottom.SpacingBefore = 10f;
                bottom.SpacingAfter = 0f;
                bottom.KeepTogether = true;

                string today = DateTime.Now.ToString("MM/dd/yyyy");

                // Summary 1
                bottom.AddCell(CreateNoBorderPdfCell(
                    "TOTAL WO IN\nDATA DEPOT\n" + collection[7].ToString(),
                    summaryFont,
                    Element.ALIGN_CENTER
                ));

                // Summary 2
                bottom.AddCell(CreateNoBorderPdfCell(
                    "TOTAL\nHOURS\n" + collection[8].ToString(),
                    summaryFont,
                    Element.ALIGN_CENTER
                ));

                // Summary 3
                bottom.AddCell(CreateNoBorderPdfCell(
                    "AVG PER\nHOUR\n" + collection[9].ToString(),
                    summaryFont,
                    Element.ALIGN_CENTER
                ));

                // Contractor signature column
                bottom.AddCell(CreateSingleSignatureColumnCell(
                    "Contractor Electronic Signature",
                    today,
                    summaryFont
                ));

                // Project manager signature column
                bottom.AddCell(CreateSingleSignatureColumnCell(
                    "Project Manager Electronic Signature",
                    today,
                    summaryFont
                ));

                // Optional blank/right spacer column
                bottom.AddCell(CreateNoBorderPdfCell("", summaryFont, Element.ALIGN_CENTER));

                document.Add(bottom);
               
                document.Close();
            }
            catch (Exception ex)
            {
                log.Error("Error in ExportToPdf: ", ex);
            }
        }


        /// <summary>
        /// create directory with access right.
        /// </summary>
        /// <param name="_direcotryPath"></param>
        private void GrantAccess(string _direcotryPath)
        {

            try
            {
                string folderPath = _direcotryPath;
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                DirectoryInfo dInfo = new DirectoryInfo(folderPath);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();
                dSecurity.AddAccessRule(new FileSystemAccessRule("everyone", FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);
            }

            catch (Exception ex) { }

        }
        private const int BEAUMONT_SERVICE_RESET_METER_BOX = 7;
        private const int BEAUMONT_SERVICE_METER_BOX_INSTALL_58_TO_1 = 10;
        private const int BEAUMONT_SERVICE_LINE_INSPECTION_OUTSIDE_BOX = 20;

        // TODO: Replace this ID with actual tblService.ID for REG EXCH STAND ALONE
        private const int BEAUMONT_SERVICE_REG_EXCH_STAND_ALONE = -1;

        private List<Model_InstalledMeterDetailPayRate> GetBeaumont21InstalledMetersDetails(
    long projectId,
    int installerId,
    string weekRangeFromDate,
    string weekRangeToDate,
    SqlConnection con)
        {
            List<Model_InstalledMeterDetailPayRate> result =
                new List<Model_InstalledMeterDetailPayRate>();

            Dictionary<string, TimeClockServiceConfig> serviceConfig =
                GetTimeClockServiceConfig(projectId, con);

            string sqlQuery = @"
        SELECT
            pf.Id AS ProjectFieldDataId,
            CONVERT(varchar, pf.VisitDateTime, 101) AS CreatedOn,
            DATENAME(weekday, pf.VisitDateTime) AS DayName,
            FORMAT(pf.VisitDateTime, 'HH:mm') AS [Time],
            dm.InitialFieldName,
            dm.FieldValue
        FROM uploadedexceldata up
        INNER JOIN TblProjectFieldData pf
            ON up.Id = pf.fk_uploadedexceldata_id
        INNER JOIN tblProjectFieldDataMaster dm
            ON pf.Id = dm.TblProjectFieldData_Id
        WHERE up.FKProjectId = @ProjectId
          AND CONVERT(date, pf.VisitDateTime) BETWEEN CONVERT(date, @FromDate)
                                                  AND CONVERT(date, @ToDate)
          AND pf.CreatedBy = @InstallerId
          AND pf.Active = 1
          AND dm.Active = 1
        ORDER BY
            pf.VisitDateTime,
            pf.Id,
            dm.Id;
    ";

            DataTable dt = new DataTable();

            using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
            {
                cmd.Parameters.AddWithValue("@ProjectId", projectId);
                cmd.Parameters.AddWithValue("@InstallerId", installerId);
                cmd.Parameters.AddWithValue("@FromDate", weekRangeFromDate);
                cmd.Parameters.AddWithValue("@ToDate", weekRangeToDate);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }

            if (dt.Rows.Count == 0)
            {
                return result;
            }

            var groupedRecords = dt.AsEnumerable()
                .GroupBy(r => Convert.ToInt64(r["ProjectFieldDataId"]));

            foreach (var recordGroup in groupedRecords)
            {
                DataRow firstRow = recordGroup.First();

                string createdOn = Convert.ToString(firstRow["CreatedOn"]);
                string dayName = Convert.ToString(firstRow["DayName"]);
                string time = Convert.ToString(firstRow["Time"]);

                string serviceType = "";              // SERVICE = Residential / Commercial
                string endPointReading = "";          // END POINT READING
                string fRegRead = "";                 // OLD FINAL REGISTER READ
                string fMeterRead = "";               // OLD METER FINAL READ
                string oldMeterSize = "";             // OLD METER SIZE

                // Lead Line Inspection logic fields
                string lcrInvestigation = "";         // LCR INVESTIGATION (Y/N)
                string citySidePipeType = "";         // CITY SIDE PIPE TYPE
                string customerSidePipeType = "";     // CUSTOMER SIDE PIPE TYPE
                string verified = "";                 // VERIFIED

                bool isWorkCompleted = false;

                List<int> selectedServiceIds = new List<int>();

                foreach (DataRow row in recordGroup)
                {
                    string fieldName = Convert.ToString(row["InitialFieldName"]);
                    string fieldValue = Convert.ToString(row["FieldValue"]);

                    if (string.IsNullOrWhiteSpace(fieldValue))
                    {
                        continue;
                    }

                    string normalizedFieldName = NormalizeBeaumontText(fieldName);
                    string normalizedFieldValue = NormalizeBeaumontText(fieldValue);

                    /*
                        SERVICE is only account/service type:
                        Residential / Commercial
                    */
                    if (normalizedFieldName == "SERVICE")
                    {
                        serviceType = fieldValue.Trim();
                    }

                    /*
                        SERVICES contains service IDs like:
                        20|49
                        4|20|49|50

                        So parse IDs from SERVICES.
                    */
                    if (normalizedFieldName == "SERVICES")
                    {
                        selectedServiceIds.AddRange(ParseBeaumontServiceIds(fieldValue));
                    }

                    /*
                        WORK COMPLETED should be true for completed record.
                        Existing projects also use WORK COMPLETED as completed service indicator.
                    */
                    if (normalizedFieldName == "WORK COMPLETED")
                    {
                        if (normalizedFieldValue == "TRUE"
                            || normalizedFieldValue == "YES"
                            || normalizedFieldValue == "Y"
                            || normalizedFieldValue == "1"
                            || normalizedFieldValue == "DONE")
                        {
                            isWorkCompleted = true;
                        }
                    }

                    if (normalizedFieldName == "END POINT READING")
                    {
                        if (string.IsNullOrWhiteSpace(endPointReading))
                        {
                            endPointReading = fieldValue.Trim();
                        }
                    }

                    if (normalizedFieldName == "OLD FINAL REGISTER READ")
                    {
                        if (string.IsNullOrWhiteSpace(fRegRead))
                        {
                            fRegRead = fieldValue.Trim();
                        }
                    }

                    if (normalizedFieldName == "OLD METER FINAL READ")
                    {
                        if (string.IsNullOrWhiteSpace(fMeterRead))
                        {
                            fMeterRead = fieldValue.Trim();
                        }
                    }

                    if (normalizedFieldName == "OLD METER SIZE")
                    {
                        if (string.IsNullOrWhiteSpace(oldMeterSize))
                        {
                            oldMeterSize = fieldValue.Trim();
                        }
                    }
                    if (normalizedFieldName == NormalizeBeaumontText("LCR INVESTIGATION (Y/N)")
    || normalizedFieldName.Contains("LCR INVESTIGATION"))
                    {
                        if (string.IsNullOrWhiteSpace(lcrInvestigation))
                        {
                            lcrInvestigation = fieldValue.Trim();
                        }
                    }

                    if (normalizedFieldName == NormalizeBeaumontText("CITY SIDE PIPE TYPE")
                        || normalizedFieldName.Contains("CITY SIDE PIPE TYPE"))
                    {
                        if (string.IsNullOrWhiteSpace(citySidePipeType))
                        {
                            citySidePipeType = fieldValue.Trim();
                        }
                    }

                    if (normalizedFieldName == NormalizeBeaumontText("CUSTOMER SIDE PIPE TYPE")
                        || normalizedFieldName.Contains("CUSTOMER SIDE PIPE TYPE"))
                    {
                        if (string.IsNullOrWhiteSpace(customerSidePipeType))
                        {
                            customerSidePipeType = fieldValue.Trim();
                        }
                    }

                    if (normalizedFieldName == "VERIFIED"
                        || normalizedFieldName.Contains("VERIFIED"))
                    {
                        if (string.IsNullOrWhiteSpace(verified))
                        {
                            verified = fieldValue.Trim();
                        }
                    }
                }
                
                /*
                    If you do not want WORK COMPLETED condition for this project,
                    remove this continue block.

                    But based on your old report logic, completed records should be counted only.
                */
                if (!isWorkCompleted)
                {
                    continue;
                }

                bool hasEndPointReading =
                    !string.IsNullOrWhiteSpace(endPointReading);

                bool hasFRegRead =
                    !string.IsNullOrWhiteSpace(fRegRead);

                bool hasFMeterRead =
                    !string.IsNullOrWhiteSpace(fMeterRead);

                bool isResidential =
                    NormalizeBeaumontText(serviceType).Contains("RESIDENTIAL");

                bool isCommercial =
                    NormalizeBeaumontText(serviceType).Contains("COMMERCIAL");

                /*
                    Service IDs from tblService:

                    7  = Reset Meter Box
                    10 = Meter Box Install 5-8 to 1 inch
                    20 = Line Inspection Dig Outside Box

                    For REG EXCH STAND ALONE, set correct ID after checking tblService.
                    If you do not know ID yet, run:
                    SELECT * FROM tblService WHERE Service LIKE '%REG%STAND%'
                */

                bool hasResetMeterBox =
         selectedServiceIds.Contains(BEAUMONT_SERVICE_RESET_METER_BOX);

                bool hasMeterBoxInstall58To1 =
                    selectedServiceIds.Contains(BEAUMONT_SERVICE_METER_BOX_INSTALL_58_TO_1);

                bool hasLineInspectionOutsideBox =
        IsValidBeaumontLeadLineInspection(
            lcrInvestigation,
            citySidePipeType,
            customerSidePipeType,
            verified
        );

                bool hasRegExchangeStandAlone =
     selectedServiceIds.Contains(BEAUMONT_SERVICE_REG_EXCH_STAND_ALONE);

                /*
                    1. ERT Mitigation

                    Endpoint Test data = END POINT READING.
                    If F-Reg Read is not available, count under ERT Mitigation.
                    If F-Reg Read is available, do not count under ERT Mitigation.
                */
                if (hasEndPointReading && !hasFRegRead)
                {
                    AddBeaumont21RowByCode(
                        result,
                        serviceConfig,
                        projectId,
                        "ERT_MITIGATION",
                        createdOn,
                        dayName,
                        time
                    );
                }

                /*
                    2. Register Exchange with ERT Mitigation Residential

                    SERVICE = Residential
                    AND OLD FINAL REGISTER READ has value.
                */
                if (isResidential && hasFRegRead)
                {
                    AddBeaumont21RowByCode(
                        result,
                        serviceConfig,
                        projectId,
                        "REG_EX_RES",
                        createdOn,
                        dayName,
                        time
                    );
                }

                /*
                    3. Register Exchange with ERT Mitigation Commercial

                    SERVICE = Commercial
                    AND OLD FINAL REGISTER READ has value.
                */
                if (isCommercial && hasFRegRead)
                {
                    AddBeaumont21RowByCode(
                        result,
                        serviceConfig,
                        projectId,
                        "REG_EX_COM",
                        createdOn,
                        dayName,
                        time
                    );
                }

                /*
                    4. Register Exchange Stand Alone

                    Count from SERVICES ID of REG EXCH STAND ALONE.
                */
                if (hasRegExchangeStandAlone)
                {
                    AddBeaumont21RowByCode(
                        result,
                        serviceConfig,
                        projectId,
                        "REG_EX_STANDALONE",
                        createdOn,
                        dayName,
                        time
                    );
                }

                /*
                    5-8. Meter Change Out & ERT Installation

                    F-Meter Read = OLD METER FINAL READ.
                    Meter Size = OLD METER SIZE.
                */
                if (hasFMeterRead)
                {
                    if (BeaumontSizeMatches(oldMeterSize, "5/8")
                        || BeaumontSizeMatches(oldMeterSize, "5/8 x 3/4")
                        || BeaumontSizeMatches(oldMeterSize, "5/8\" x 3/4"))
                    {
                        AddBeaumont21RowByCode(
                            result,
                            serviceConfig,
                            projectId,
                            "METER_58",
                            createdOn,
                            dayName,
                            time
                        );
                    }
                    else if (BeaumontSizeMatches(oldMeterSize, "1"))
                    {
                        AddBeaumont21RowByCode(
                            result,
                            serviceConfig,
                            projectId,
                            "METER_1",
                            createdOn,
                            dayName,
                            time
                        );
                    }
                    else if (BeaumontSizeMatches(oldMeterSize, "1.5")
                             || BeaumontSizeMatches(oldMeterSize, "1 1/2")
                             || BeaumontSizeMatches(oldMeterSize, "1-1/2"))
                    {
                        AddBeaumont21RowByCode(
                            result,
                            serviceConfig,
                            projectId,
                            "METER_15",
                            createdOn,
                            dayName,
                            time
                        );
                    }
                    else if (BeaumontSizeMatches(oldMeterSize, "2"))
                    {
                        AddBeaumont21RowByCode(
                            result,
                            serviceConfig,
                            projectId,
                            "METER_2",
                            createdOn,
                            dayName,
                            time
                        );
                    }
                }

                /*
                    9. Lead Line Inspection Outside Box

                    Service ID 20 = Line Inspection Dig Outside Box.
                */
                if (hasLineInspectionOutsideBox)
                {
                    AddBeaumont21RowByCode(
                        result,
                        serviceConfig,
                        projectId,
                        "LEAD_LINE",
                        createdOn,
                        dayName,
                        time
                    );
                }

                /*
                    10. Residential Box Replacement or Adjustment

                    Combined count:
                    Service ID 7  = Reset Meter Box
                    Service ID 10 = Meter Box Install 5-8 to 1 inch

                    If both IDs exist in same record, this adds 2 counts.
                */
                if (hasResetMeterBox)
                {
                    AddBeaumont21RowByCode(
                        result,
                        serviceConfig,
                        projectId,
                        "BOX_REPLACEMENT",
                        createdOn,
                        dayName,
                        time
                    );
                }

                if (hasMeterBoxInstall58To1)
                {
                    AddBeaumont21RowByCode(
                        result,
                        serviceConfig,
                        projectId,
                        "BOX_REPLACEMENT",
                        createdOn,
                        dayName,
                        time
                    );
                }
            }

            return result;
        }
        private void AddBeaumont21Row(
    List<Model_InstalledMeterDetailPayRate> result,
    long projectId,
    string displayName,
    string createdOn,
    string dayName,
    string time,
    string pay)
        {
            result.Add(new Model_InstalledMeterDetailPayRate
            {
                ProjectId = projectId,
                MeterSize = "",
                MeterType = "",
                MeterDisplayName = displayName,
                CreatedOn = createdOn,
                DayName = dayName,
                Time = time,
                Pay = pay
            });
        }

        private string NormalizeBeaumontText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }

            string normalized = value
                .Trim()
                .ToUpper()
                .Replace("\"", "")
                .Replace("'", "")
                .Replace("&", " AND ")
                .Replace("/", " / ")
                .Replace("_", " ")
                .Replace("-", " ");

            while (normalized.Contains("  "))
            {
                normalized = normalized.Replace("  ", " ");
            }

            return normalized.Trim();
        }
        private bool BeaumontHasService(List<string> serviceValues, string expectedService)
        {
            if (serviceValues == null || serviceValues.Count == 0)
            {
                return false;
            }

            string expected = NormalizeBeaumontText(expectedService);

            foreach (string service in serviceValues)
            {
                string actual = NormalizeBeaumontText(service);

                if (actual == expected)
                {
                    return true;
                }

                if (expected == "RESIDENTIAL" && actual.Contains("RESIDENTIAL"))
                {
                    return true;
                }

                if (expected == "COMMERCIAL" && actual.Contains("COMMERCIAL"))
                {
                    return true;
                }

                if (expected == NormalizeBeaumontText("REG EXCH STAND ALONE")
                    && (
                        actual.Contains("REG EXCH STAND ALONE")
                        || actual.Contains("REG EXCH STANDALONE")
                        || actual.Contains("REG EXCHANGE STAND ALONE")
                        || actual.Contains("REGISTER EXCHANGE STAND ALONE")
                    ))
                {
                    return true;
                }

                if (expected == NormalizeBeaumontText("Line Inspection Dig Outside Box")
                    && actual.Contains("LINE INSPECTION")
                    && actual.Contains("OUTSIDE BOX"))
                {
                    return true;
                }

                if (expected == NormalizeBeaumontText("Reset Meter Box")
                    && actual.Contains("RESET METER BOX"))
                {
                    return true;
                }

                if ((expected == NormalizeBeaumontText("Meter Box Install 5-8 to 1 inch")
                        || expected == NormalizeBeaumontText("Meter Box Install 5/8 to 1 Inch"))
                    && actual.Contains("METER BOX INSTALL")
                    && (
                        actual.Contains("5 8")
                        || actual.Contains("5 / 8")
                        || actual.Contains("5-8")
                        || actual.Contains("5/8")
                    )
                    && actual.Contains("1"))
                {
                    return true;
                }
            }

            return false;
        }

        private bool BeaumontSizeMatches(string actualSize, string expectedSize)
        {
            string actual = NormalizeBeaumontSize(actualSize);
            string expected = NormalizeBeaumontSize(expectedSize);

            if (string.IsNullOrWhiteSpace(actual) || string.IsNullOrWhiteSpace(expected))
            {
                return false;
            }

            return actual == expected || actual.Contains(expected);
        }
        private string NormalizeBeaumontSize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }

            return value
                .Trim()
                .ToUpper()
                .Replace("\"", "")
                .Replace("'", "")
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("X", "x")
                .Replace("INCH", "")
                .Replace("IN", "");
        }
        private DataTable GetProjectTimeClockMeterPayOrder(
    long projectId,
    SqlConnection con)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("MeterSize", typeof(string));
            dt.Columns.Add("Pay", typeof(decimal));
            dt.Columns.Add("MeterDisplayName", typeof(string));

            string sql = @"
        SELECT
            ServiceDisplayName AS MeterDisplayName,
            PayAmount AS Pay
        FROM TimeClockServiceMaster
        WHERE ProjectId = @ProjectId
          AND IsActive = 1
        ORDER BY DisplayOrder;
    ";

            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@ProjectId", projectId);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);
            }

            if (dt.Rows.Count == 0 && projectId == 10134)
            {
                Dictionary<string, TimeClockServiceConfig> defaults =
                    GetDefaultBeaumont21ServiceConfig();

                foreach (var item in defaults.Values.OrderBy(x => x.DisplayOrder))
                {
                    dt.Rows.Add("", item.PayAmount, item.ServiceDisplayName);
                }
            }

            return dt;
        }

        private class TimeClockServiceConfig
        {
            public string ServiceCode { get; set; }
            public string ServiceDisplayName { get; set; }
            public decimal PayAmount { get; set; }
            public int DisplayOrder { get; set; }
        }
        private Dictionary<string, TimeClockServiceConfig> GetTimeClockServiceConfig(
    long projectId,
    SqlConnection con)
        {
            Dictionary<string, TimeClockServiceConfig> config =
                GetDefaultBeaumont21ServiceConfig();

            string sql = @"
        SELECT
            ServiceCode,
            ServiceDisplayName,
            PayAmount,
            DisplayOrder
        FROM TimeClockServiceMaster
        WHERE ProjectId = @ProjectId
          AND IsActive = 1
        ORDER BY DisplayOrder;
    ";

            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@ProjectId", projectId);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    string serviceCode = Convert.ToString(row["ServiceCode"]).Trim();

                    if (string.IsNullOrWhiteSpace(serviceCode))
                    {
                        continue;
                    }

                    config[serviceCode] = new TimeClockServiceConfig
                    {
                        ServiceCode = serviceCode,
                        ServiceDisplayName = Convert.ToString(row["ServiceDisplayName"]).Trim(),
                        PayAmount = Convert.ToDecimal(row["PayAmount"]),
                        DisplayOrder = Convert.ToInt32(row["DisplayOrder"])
                    };
                }
            }

            return config;
        }
        private Dictionary<string, TimeClockServiceConfig> GetDefaultBeaumont21ServiceConfig()
        {
            return new Dictionary<string, TimeClockServiceConfig>
    {
        {
            "ERT_MITIGATION",
            new TimeClockServiceConfig
            {
                ServiceCode = "ERT_MITIGATION",
                ServiceDisplayName = "ERT Mitigation",
                PayAmount = 9.00m,
                DisplayOrder = 1
            }
        },
        {
            "METER_58",
            new TimeClockServiceConfig
            {
                ServiceCode = "METER_58",
                ServiceDisplayName = "5/8\" x 3/4 Meter Change Out & ERT Installation",
                PayAmount = 15.00m,
                DisplayOrder = 2
            }
        },
        {
            "REG_EX_RES",
            new TimeClockServiceConfig
            {
                ServiceCode = "REG_EX_RES",
                ServiceDisplayName = "Register Exchange with ERT Mitigation (Residential)",
                PayAmount = 12.00m,
                DisplayOrder = 3
            }
        },
        {
            "REG_EX_COM",
            new TimeClockServiceConfig
            {
                ServiceCode = "REG_EX_COM",
                ServiceDisplayName = "Register Exchange with ERT Mitigation (Commercial)",
                PayAmount = 18.00m,
                DisplayOrder = 4
            }
        },
        {
            "REG_EX_STANDALONE",
            new TimeClockServiceConfig
            {
                ServiceCode = "REG_EX_STANDALONE",
                ServiceDisplayName = "Register Exchange (Stand Alone)",
                PayAmount = 8.00m,
                DisplayOrder = 5
            }
        },
        {
            "LEAD_LINE",
            new TimeClockServiceConfig
            {
                ServiceCode = "LEAD_LINE",
                ServiceDisplayName = "Lead Line Inspection (Outside the Box) with ERT Mitigation",
                PayAmount = 12.00m,
                DisplayOrder = 6
            }
        },
        {
            "METER_1",
            new TimeClockServiceConfig
            {
                ServiceCode = "METER_1",
                ServiceDisplayName = "1\" Meter Change Out & ERT Installation",
                PayAmount = 15.00m,
                DisplayOrder = 7
            }
        },
        {
            "METER_15",
            new TimeClockServiceConfig
            {
                ServiceCode = "METER_15",
                ServiceDisplayName = "1.5\" Meter Change Out & ERT Installation",
                PayAmount = 65.00m,
                DisplayOrder = 8
            }
        },
        {
            "METER_2",
            new TimeClockServiceConfig
            {
                ServiceCode = "METER_2",
                ServiceDisplayName = "2\" Meter Change Out & ERT Installation",
                PayAmount = 70.00m,
                DisplayOrder = 9
            }
        },
        {
                "BOX_REPLACEMENT",
                new TimeClockServiceConfig
                {
                    ServiceCode = "BOX_REPLACEMENT",
                    ServiceDisplayName = "Residential Box Replacement or Adjustment",
                    PayAmount = 15.75m,
                    DisplayOrder = 10
                }
            }
    };
        }
        private void AddBeaumont21RowByCode(
    List<Model_InstalledMeterDetailPayRate> result,
    Dictionary<string, TimeClockServiceConfig> serviceConfig,
    long projectId,
    string serviceCode,
    string createdOn,
    string dayName,
    string time)
        {
            if (!serviceConfig.ContainsKey(serviceCode))
            {
                log.Info("Beaumont 2.1 service config missing for ServiceCode: " + serviceCode);
                return;
            }

            TimeClockServiceConfig config = serviceConfig[serviceCode];

            result.Add(new Model_InstalledMeterDetailPayRate
            {
                ProjectId = projectId,
                MeterSize = "",
                MeterType = "",
                MeterDisplayName = config.ServiceDisplayName,
                CreatedOn = createdOn,
                DayName = dayName,
                Time = time,
                Pay = config.PayAmount.ToString("0.00")
            });
        }
        private bool IsBeaumontSelectedValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string normalized = NormalizeBeaumontText(value);

            if (normalized == "0"
                || normalized == "NO"
                || normalized == "N"
                || normalized == "FALSE"
                || normalized == "NOT SELECTED"
                || normalized == "NA"
                || normalized == "N / A"
                || normalized == "N/A")
            {
                return false;
            }

            return true;
        }
        private List<int> ParseBeaumontServiceIds(string value)
        {
            List<int> serviceIds = new List<int>();

            if (string.IsNullOrWhiteSpace(value))
            {
                return serviceIds;
            }

            string[] parts = value.Split('|');

            foreach (string part in parts)
            {
                int serviceId;

                if (int.TryParse(part.Trim(), out serviceId))
                {
                    serviceIds.Add(serviceId);
                }
            }

            return serviceIds;
        }
        private bool IsValidBeaumontLeadLineInspection(
    string lcrInvestigation,
    string citySidePipeType,
    string customerSidePipeType,
    string verified)
        {
            bool isLcrYes =
                NormalizeBeaumontText(lcrInvestigation) == "Y"
                || NormalizeBeaumontText(lcrInvestigation) == "YES";

            bool isVerifiedBoth =
                NormalizeBeaumontText(verified) == "BOTH";

            bool isValidCitySidePipe =
                IsAllowedBeaumontCitySidePipeType(citySidePipeType);

            bool isValidCustomerSidePipe =
                IsAllowedBeaumontCustomerSidePipeType(customerSidePipeType);

            return isLcrYes
                && isValidCitySidePipe
                && isValidCustomerSidePipe
                && isVerifiedBoth;
        }

        private bool IsAllowedBeaumontCitySidePipeType(string pipeType)
        {
            string value = NormalizeBeaumontText(pipeType);

            return value == "COPPER"
                || value == "POLY"
                || value == "GALVANIZED"
                || value == "GALVANIZES"
                || value == "GALVANIZE"
                || value == "CAST IRON"
                || value == "PEX";
        }

        private bool IsAllowedBeaumontCustomerSidePipeType(string pipeType)
        {
            string value = NormalizeBeaumontText(pipeType);

            return value == "COPPER"
                || value == "POLY"
                || value == "GALVANIZED"
                || value == "GALVANIZES"
                || value == "GALVANIZE"
                || value == "CAST IRON"
                || value == "PEX"
                || value == "PVC"
                || value == "LEAD";
        }
        private PdfPCell CreateBorderPdfCell(string text, Font font, int alignment)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text ?? "", font));
            cell.HorizontalAlignment = alignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;

            cell.PaddingTop = 2f;
            cell.PaddingBottom = 2f;
            cell.PaddingLeft = 2.5f;
            cell.PaddingRight = 2.5f;

            cell.MinimumHeight = 14f;
            cell.NoWrap = false;
            cell.BorderWidth = 0.7f;

            return cell;
        }

        private PdfPCell CreateNoBorderPdfCell(string text, Font font, int alignment)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text ?? "", font));
            cell.Border = Rectangle.NO_BORDER;
            cell.HorizontalAlignment = alignment;
            cell.VerticalAlignment = Element.ALIGN_TOP;

            cell.PaddingTop = 1f;
            cell.PaddingBottom = 1f;
            cell.PaddingLeft = 2f;
            cell.PaddingRight = 2f;

            cell.NoWrap = false;

            return cell;
        }
        private PdfPCell CreateSignatureAreaCell(Font font)
        {
            PdfPCell outerCell = new PdfPCell();
            outerCell.Border = Rectangle.NO_BORDER;
            outerCell.PaddingTop = 4f;
            outerCell.PaddingBottom = 0f;
            outerCell.PaddingLeft = 20f;
            outerCell.PaddingRight = 20f;
            outerCell.VerticalAlignment = Element.ALIGN_TOP;

            PdfPTable signatureTable = new PdfPTable(1);
            signatureTable.WidthPercentage = 100;

            // Contractor signature
            PdfPCell contractorBox = new PdfPCell(new Phrase("Contractor Electronic Signature", font));
            contractorBox.HorizontalAlignment = Element.ALIGN_LEFT;
            contractorBox.VerticalAlignment = Element.ALIGN_BOTTOM;
            contractorBox.FixedHeight = 52f;
            contractorBox.PaddingBottom = 3f;
            contractorBox.Border = Rectangle.NO_BORDER;
            signatureTable.AddCell(contractorBox);

            // Reduced gap so Project Manager Signature moves upward
            PdfPCell gapCell = new PdfPCell(new Phrase(""));
            gapCell.Border = Rectangle.NO_BORDER;
            gapCell.FixedHeight = 20f;
            signatureTable.AddCell(gapCell);

            // Project manager signature
            PdfPCell managerBox = new PdfPCell(new Phrase("Project Manager Electronic Signature", font));
            managerBox.HorizontalAlignment = Element.ALIGN_LEFT;
            managerBox.VerticalAlignment = Element.ALIGN_BOTTOM;
            managerBox.FixedHeight = 38f;
            managerBox.PaddingBottom = 3f;
            managerBox.Border = Rectangle.NO_BORDER;
            signatureTable.AddCell(managerBox);

            outerCell.AddElement(signatureTable);

            return outerCell;
        }
        private PdfPCell CreateSingleSignatureColumnCell(string signatureText, string dateText, Font font)
        {
            PdfPCell outerCell = new PdfPCell();
            outerCell.Border = Rectangle.NO_BORDER;
            outerCell.PaddingTop = 6f;
            outerCell.PaddingBottom = 0f;
            outerCell.PaddingLeft = 8f;
            outerCell.PaddingRight = 8f;
            outerCell.VerticalAlignment = Element.ALIGN_TOP;

            PdfPTable innerTable = new PdfPTable(1);
            innerTable.WidthPercentage = 100;

            // Signature text
            PdfPCell signatureCell = new PdfPCell(new Phrase(signatureText, font));
            signatureCell.Border = Rectangle.NO_BORDER;
            signatureCell.HorizontalAlignment = Element.ALIGN_CENTER;
            signatureCell.VerticalAlignment = Element.ALIGN_BOTTOM;
            signatureCell.FixedHeight = 42f;
            signatureCell.PaddingBottom = 4f;
            innerTable.AddCell(signatureCell);

            // Date text below signature
            PdfPCell dateCell = new PdfPCell(new Phrase("Date " + dateText, font));
            dateCell.Border = Rectangle.NO_BORDER;
            dateCell.HorizontalAlignment = Element.ALIGN_CENTER;
            dateCell.VerticalAlignment = Element.ALIGN_TOP;
            dateCell.FixedHeight = 20f;
            dateCell.PaddingTop = 2f;
            innerTable.AddCell(dateCell);

            outerCell.AddElement(innerTable);

            return outerCell;
        }
    }
}
