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

                    if (projectStartDate.Year == item.Item2.Year)
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
        public List<Model_InstalledMeterDetailPayRate> getInstalledMetersDetails(long _ProjectID, int installerid, string WeekRangefromDate, string WeekRangetoDate, SqlConnection con)
        {
            // Replace (dm.CreatedOn = pf.visitdatetime) on installer date tiem in bothes queries
            // Tbl name -> tblProjectFieldDataMaster == dm.CreatedOn
            // Tbl name -> TblProjectFieldData       == pf.visitdatetime
            //SqlDataAdapter sdaMeterSizeData = new SqlDataAdapter("select ROW_NUMBER() OVER(ORDER BY (SELECT 1 as bigint)) AS SerianNo,(dm.FieldValue)as MeterSize,CONVERT(varchar, dm.CreatedOn, 101)as CreatedOn,Datename(weekday,dm.CreatedOn) as DayName,FORMAT(dm.CreatedOn,'HH:mm')as Time from uploadedexceldata as up join TblProjectFieldData as pf on up.id = pf.fk_uploadedexceldata_id join tblProjectFieldDataMaster as dm on pf.id = dm.TblProjectFieldData_Id join TblMeterSize as ms on dm.FieldValue = ms.MeterSize WHERE up.FKProjectId = " + _ProjectID + " and(CONVERT(DATE, dm.CreatedOn) between CONVERT(DATE, '" + WeekRangefromDate + "') and CONVERT(DATE, '" + WeekRangetoDate + "')) and dm.InitialFieldName = 'NEW METER SIZE' and dm.CreatedBy = '" + installerid + "' and dm.Active = 1 order by dm.id", con);
            SqlDataAdapter sdaMeterSizeData = new SqlDataAdapter("select ROW_NUMBER() OVER(ORDER BY (SELECT 1 as bigint)) AS SerianNo,(dm.FieldValue)as MeterSize,CONVERT(varchar, pf.visitdatetime, 101)as CreatedOn,Datename(weekday,pf.visitdatetime) as DayName,FORMAT(pf.visitdatetime,'HH:mm')as Time from uploadedexceldata as up join TblProjectFieldData as pf on up.id = pf.fk_uploadedexceldata_id join tblProjectFieldDataMaster as dm on pf.id = dm.TblProjectFieldData_Id join TblMeterSize as ms on dm.FieldValue = ms.MeterSize WHERE up.FKProjectId = " + _ProjectID + " and(CONVERT(DATE, pf.visitdatetime) between CONVERT(DATE, '" + WeekRangefromDate + "') and CONVERT(DATE, '" + WeekRangetoDate + "')) and dm.InitialFieldName = 'NEW METER SIZE' and dm.CreatedBy = '" + installerid + "' and pf.CreatedBy = '" + installerid + "' and dm.Active = 1 and pf.Active=1 order by dm.id", con);
            DataTable dtMeterSizesData = new DataTable();
            sdaMeterSizeData.Fill(dtMeterSizesData);
            
            //SqlDataAdapter sdaMeterTypeData = new SqlDataAdapter("select ROW_NUMBER() OVER(ORDER BY (SELECT 1 as bigint)) AS SerianNo,(dm.FieldValue)as MeterType,CONVERT(varchar, dm.CreatedOn, 101)as CreatedOn,Datename(weekday,dm.CreatedOn) as DayName,FORMAT(dm.CreatedOn,'HH:mm')as Time from uploadedexceldata as up join TblProjectFieldData as pf on up.id = pf.fk_uploadedexceldata_id join tblProjectFieldDataMaster as dm on pf.id = dm.TblProjectFieldData_Id join tblMeterType as mt on dm.FieldValue = mt.MeterType WHERE up.FKProjectId = " + _ProjectID + " and(CONVERT(DATE, dm.CreatedOn) between CONVERT(DATE, '" + WeekRangefromDate + "') and CONVERT(DATE, '" + WeekRangetoDate + "')) and dm.InitialFieldName = 'NEW METER TYPE' and dm.CreatedBy = '" + installerid + "' and dm.Active = 1 order by dm.id", con);
            SqlDataAdapter sdaMeterTypeData = new SqlDataAdapter("select ROW_NUMBER() OVER(ORDER BY (SELECT 1 as bigint)) AS SerianNo,(dm.FieldValue)as MeterType,CONVERT(varchar, pf.visitdatetime, 101)as CreatedOn,Datename(weekday,pf.visitdatetime) as DayName,FORMAT(pf.visitdatetime,'HH:mm')as Time from uploadedexceldata as up join TblProjectFieldData as pf on up.id = pf.fk_uploadedexceldata_id join tblProjectFieldDataMaster as dm on pf.id = dm.TblProjectFieldData_Id join tblMeterType as mt on dm.FieldValue = mt.MeterType WHERE up.FKProjectId = " + _ProjectID + " and(CONVERT(DATE, pf.visitdatetime) between CONVERT(DATE, '" + WeekRangefromDate + "') and CONVERT(DATE, '" + WeekRangetoDate + "')) and dm.InitialFieldName = 'NEW METER TYPE' and dm.CreatedBy = '" + installerid + "' and pf.CreatedBy = '" + installerid + "' and dm.Active = 1 and pf.Active=1 order by dm.id", con);
            DataTable dtMeterTypeData = new DataTable();
            sdaMeterTypeData.Fill(dtMeterTypeData);
            List<Model_InstalledMeterDetailPayRate> objIMDPR = new List<Model_InstalledMeterDetailPayRate>();
            if (dtMeterSizesData.Rows.Count > 0 && dtMeterTypeData.Rows.Count > 0 && dtMeterSizesData.Rows.Count == dtMeterTypeData.Rows.Count)
            {
                var combineData = (from ms in dtMeterSizesData.AsEnumerable()
                                   join mt in dtMeterTypeData.AsEnumerable()
                                   on ms.Field<long>("SerianNo") equals mt.Field<long>("SerianNo")
                                   select new Model_InstalledMeterDetail
                                   {
                                       ProjectId = _ProjectID,
                                       MeterSize = ms.Field<string>("MeterSize"),
                                       MeterType = mt.Field<string>("MeterType"),
                                       CreatedOn = ms.Field<string>("CreatedOn"),
                                       DayName = ms.Field<string>("DayName"),
                                       Time = ms.Field<string>("Time")
                                   }).ToList();
                // Getting meter pay rates
                CompassEntities objCompassEntities = new CompassEntities();
                objIMDPR = new List<Model_InstalledMeterDetailPayRate>();
                foreach (var item in combineData)
                {
                    var obj = objCompassEntities.tblMeterPays.Where(o => o.FK_Project_Id == item.ProjectId && o.MeterSize == item.MeterSize && o.MeterType == item.MeterType).FirstOrDefault();
                    objIMDPR.Add(new Model_InstalledMeterDetailPayRate
                    {
                        ProjectId = _ProjectID,
                        MeterSize = item.MeterSize,
                        MeterType = item.MeterType,
                        MeterDisplayName=obj.MeterDisplayName,
                        CreatedOn = item.CreatedOn,
                        DayName = item.DayName,
                        Time = item.Time,
                        Pay =Convert.ToString(obj.Pay)
                    });
                }
            }
            return objIMDPR;
        }
        //


        // Craete getting installer start or end time
        public List<Model_FinalResult> GetInstallerTotalWorkedTime(long _ProjectID, int installerid, string weekrangedate)
        {

            try
            {
                List<Model_FinalResult> finalResult = new List<Model_FinalResult>();

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

                var WeekRangefromDate = Convert.ToDateTime(weekrangedate.Split()[0]).ToString("yyyy-MM-dd"); // From date format change as my requirement
                var WeekRangetoDate = Convert.ToDateTime(weekrangedate.Split()[3]).ToString("yyyy-MM-dd");  // To date format change as my requirement
                CompassEntities CompassEntities = new CompassEntities();
                SqlConnection con = new SqlConnection(CompassEntities.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                con.Open();
                List<Model_InstalledMeterDetailPayRate> objInstalledMeterDetailPayRate = new List<Model_InstalledMeterDetailPayRate>();
                objInstalledMeterDetailPayRate = getInstalledMetersDetails(_ProjectID, installerid, WeekRangefromDate, WeekRangetoDate, con);
                // This function use to get All selected week date renage all date list in dynamic form show in html view table in first row
                final_CTR_List = getDynamicDates(Convert.ToDateTime(WeekRangefromDate), Convert.ToDateTime(WeekRangetoDate));

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

                    
                    SqlCommand cnn = new SqlCommand("PROC_GetMeterSizeOrder", con);
                    cnn.Parameters.AddWithValue("@projectId", _ProjectID);
                    cnn.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter sda = new SqlDataAdapter(cnn);
                    DataTable dt_AllMeterSizeOrPay = new DataTable();
                    sda.Fill(dt_AllMeterSizeOrPay);


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
                    var cityName = CompassEntities.TblCityMasters.Where(o => o.CityId == pd.CityId).FirstOrDefault();
                    var stateName = CompassEntities.tblStateMasters.Where(o => o.StateId == pd.StateId).FirstOrDefault();
                    var installer = CompassEntities.tblUsers.Where(o => o.UserID == installerid).FirstOrDefault();

                    string projectName = cityName.CityName + " " + stateName.StateName;
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
                throw ex;
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

        public List<Get_CTR> InstalledMeterCalculation(List<string> weekDayName, List<Model_InstalledMeterDetailPayRate> objInstalledMeterDetailPayRate, DataTable dt_AllMeterSizeOrPay, ref int totalInstalledMeterInWeek, ref decimal totalPay)
        {
            try
            {
                List<Model_MeterSizeDetail> MeterSizeDetail = new List<Model_MeterSizeDetail>();

                // Datatable set group by week DayName, MeterSizeWithDisplayName, Pay wise    //
                var groupedWeekDayNameWise = from imdr in objInstalledMeterDetailPayRate.AsEnumerable()
                                             group imdr by new { DayName = imdr.DayName, MeterSize=imdr.MeterSize, MeterDisplayName = imdr.MeterDisplayName, Pay = imdr.Pay } into groupby
                                             select new
                                             {
                                                 Value = groupby.Key,
                                                 metercount = groupby.Count()
                                             };

                foreach (var w in weekDayName)
                {
                    string Day = w.ToString();
                    string meterSize = string.Empty;
                    string meterDisplayName = string.Empty;
                    string meterCount = string.Empty;
                    string pay = string.Empty;
                    int i = 0;
                    foreach (var item in groupedWeekDayNameWise.ToList())
                    {
                        string dbDay = item.Value.DayName.ToString();
                        meterSize = string.Empty;
                        meterCount = string.Empty;
                        pay = string.Empty;
                        int dayWiseMeterCount = 0;
                        if (w.ToString() == dbDay)
                        {
                            meterSize = item.Value.MeterSize.ToString(); // Old code witte on meter size wise 
                            meterDisplayName = item.Value.MeterDisplayName.ToString();  // but new code MeterDisplayName wise 
                            meterCount = item.metercount.ToString();
                            pay = item.Value.Pay.ToString();
                            dayWiseMeterCount += Convert.ToInt32(meterCount);
                            MeterSizeDetail.Add(new Model_MeterSizeDetail       // this model add Day, meterSize, meterCount, dayWiseMeterCount
                            {
                                Day = Day,
                                MeterSize = meterSize,
                                MeterDisplayName= meterDisplayName,
                                MeterCount = meterCount,
                                TotalInstaledMeter = dayWiseMeterCount,
                                Pay = pay
                            });
                            i++;
                        }
                    }
                    if (i == 0)
                    {
                        MeterSizeDetail.Add(new Model_MeterSizeDetail   // this model add Day, meterSize, meterCount, dayWiseMeterCount
                        {
                            Day = Day,
                            MeterSize = meterSize,
                            MeterDisplayName = meterDisplayName,
                            MeterCount = meterCount,
                            TotalInstaledMeter = 0,
                            Pay = pay
                        });
                    }
                }

                List<Get_CTR> CTR_MeterSizeList = new List<Get_CTR>();
                string __Dis = string.Empty;
                string Monday = string.Empty;
                string Tuesday = string.Empty;
                string Wednesday = string.Empty;
                string Thursday = string.Empty;
                string Friday = string.Empty;
                string Saturday = string.Empty;
                int total_meter = 0;
                string _pay = string.Empty;
                decimal total_Pay = 0m;
                foreach (DataRow msize in dt_AllMeterSizeOrPay.Rows)
                {
                    //string _msize = msize["MeterSize"].ToString();
                    string _meterDisplayName = msize["MeterDisplayName"].ToString();
                    _pay = msize["Pay"].ToString();
                    var combinedaywise = MeterSizeDetail.Where(o => o.MeterDisplayName == _meterDisplayName);
                    total_meter = 0;
                    foreach (var f in combinedaywise)
                    {
                        __Dis = _meterDisplayName;
                        if (f.Day == "Monday")
                        { Monday = f.MeterCount; }
                        if (f.Day == "Tuesday")
                        { Tuesday = f.MeterCount; }
                        if (f.Day == "Wednesday")
                        { Wednesday = f.MeterCount; }
                        if (f.Day == "Thursday")
                        { Thursday = f.MeterCount; }
                        if (f.Day == "Friday")
                        { Friday = f.MeterCount; }
                        if (f.Day == "Saturday")
                        { Saturday = f.MeterCount; }
                        total_meter += Convert.ToInt32(f.MeterCount);
                    }
                    if (combinedaywise.Count() > 0)
                    {
                        total_Pay += total_meter * (Convert.ToDecimal(_pay));
                        CTR_MeterSizeList.Add(new Get_CTR           // Store final display meter count records save 
                        {
                            Description = __Dis,
                            Monday = Monday,
                            Tuesday = Tuesday,
                            Wednesday = Wednesday,
                            Thursday = Thursday,
                            Friday = Friday,
                            Saturday = Saturday,
                            TotalWorkInWeek = Convert.ToString(total_meter),
                            Pay = "$ " + _pay,
                            Total = "$ " + Convert.ToString(total_meter * (Convert.ToDecimal(_pay)))
                        });
                    }
                    else
                    {
                        __Dis = _meterDisplayName;
                        CTR_MeterSizeList.Add(new Get_CTR       // Store final display meter count records save is null 
                        {
                            Description = __Dis,
                            Monday = Monday,
                            Tuesday = Tuesday,
                            Wednesday = Wednesday,
                            Thursday = Thursday,
                            Friday = Friday,
                            Saturday = Saturday,
                            TotalWorkInWeek = Convert.ToString(total_meter),
                            Pay = "$ " + _pay,
                            Total = "$ 0.00"    // Null condition saore value 0.00
                        });
                    }
                    __Dis = string.Empty;
                    Monday = string.Empty;
                    Tuesday = string.Empty;
                    Wednesday = string.Empty;
                    Thursday = string.Empty;
                    Friday = string.Empty;
                    Saturday = string.Empty;
                    total_meter = 0;
                    _pay = string.Empty;
                    //total_Pay = 0;
                }

                int Mon = 0;
                int Tue = 0;
                int Wed = 0;
                int Thu = 0;
                int Fri = 0;
                int Sat = 0;

                foreach (var w in weekDayName)
                {
                    var datWiseMeterTotal = MeterSizeDetail.Where(o => o.Day == w.ToString());
                    foreach (var f in datWiseMeterTotal)
                    {
                        __Dis = "Total";
                        if (f.Day == "Monday")
                        { Mon += f.TotalInstaledMeter; }
                        if (f.Day == "Tuesday")
                        { Tue += f.TotalInstaledMeter; }
                        if (f.Day == "Wednesday")
                        { Wed += f.TotalInstaledMeter; }
                        if (f.Day == "Thursday")
                        { Thu += f.TotalInstaledMeter; }
                        if (f.Day == "Friday")
                        { Fri += f.TotalInstaledMeter; }
                        if (f.Day == "Saturday")
                        { Sat += f.TotalInstaledMeter; }
                        total_meter += f.TotalInstaledMeter;   
                    }
                }
                CTR_MeterSizeList.Add(new Get_CTR               // Store final display Tota meter count save in a day save in total Row
                {
                    Description = __Dis,
                    Monday = Convert.ToString(Mon),
                    Tuesday = Convert.ToString(Tue),
                    Wednesday = Convert.ToString(Wed),
                    Thursday = Convert.ToString(Thu),
                    Friday = Convert.ToString(Fri),
                    Saturday = Convert.ToString(Sat),
                    TotalWorkInWeek = Convert.ToString(total_meter),
                    Pay = "",
                    Total = "$ " + Convert.ToString(total_Pay)
                });
                totalInstalledMeterInWeek = total_meter;
                totalPay = total_Pay;
                return CTR_MeterSizeList;
            }
            #endregion
            catch (Exception ex)
            {
                throw ex;
            }
        }


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
        
        public void ExportToPdf(DataTable dt, string strFilePath,string[] collection)
        {
            try
            {
                //Document document = new Document(PageSize.A4);
                //Document document = new Document(PageSize.A4.Rotate());
                Document document = new Document(PageSize.A4.Rotate(), 50, 50, 25, 25);
                //Document document = new Document(PageSize.A4, 50, 50, 25, 25);
                //Document document = new Document(PageSize.A4, 100, 100, 100, 100);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(strFilePath, FileMode.Create));
                document.Open();
                //iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 7);
                iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK);

                /*PdfPTable table = new PdfPTable(dt.Columns.Count);
                PdfPRow row = null;*/
                /*float[] widths = new float[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                    widths[i] = 4f;

                table.SetWidths(widths);*/

                /*float[] widths = new float[] { 2f, 1f, 1f, 1f, 1f, 1f, 1f, 0.85f, 1f, 1f };
                table.SetWidths(widths);
                table.WidthPercentage = 100;*/

                BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                //System.Drawing.Font ft = new System.Drawing.Font(bf, 8, 2, Color.Gray);
                Paragraph pg = new Paragraph();
                pg.Alignment = Element.ALIGN_CENTER;
                pg.Add(new Chunk(collection[0].ToString() + " Project"));
                pg.Add(new Chunk("\n Contractor's Weekly Install Sheet"));
                pg.Add(new Chunk("\n \n \n"));
                document.Add(pg);

                //
                PdfPTable tab1 = new PdfPTable(2);
                float[] tabwidths1 = new float[] { 2f, 2f };
                tab1.SetWidths(tabwidths1);
                tab1.WidthPercentage = 100;
                tab1.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                
                //row 1
                tab1.AddCell("");
                //tab1.AddCell("         Contractor Name: ___________________");
                tab1.AddCell("         Contractor Name: " + collection[2].ToString());
                //row 2
                tab1.AddCell("");
                //tab1.AddCell("Contractor LLC. Name: ___________________");
                tab1.AddCell("Contractor LLC. Name: " + collection[3].ToString());
                //row 2
                //tab1.AddCell(("Week Ending: ________________"+ collection[6].ToString(), FontFactory.GetFont(Convert.ToString(System.Drawing.FontStyle.Underline)));
                //tab1.AddCell("Week Ending: ________________" + collection[6].ToString());
                tab1.AddCell("Week Ending: "+ collection[1].ToString());
                //tab1.AddCell("        Contractor Phone: ___________________");
                tab1.AddCell("        Contractor Phone: " + collection[4].ToString());
                //row 2
                tab1.AddCell("");
                //tab1.AddCell("        Contractor E-Mail: ___________________");
                tab1.AddCell("        Contractor E-Mail: " + collection[5].ToString());
                //row 2
                tab1.AddCell("Project Name: " +collection[0].ToString() + "");
                //tab1.AddCell("          Project Manager: ___________________");
                tab1.AddCell("          Project Manager: " + collection[6].ToString());
                //row 2
                tab1.AddCell("\n");
                tab1.AddCell("\n");
                document.Add(tab1);
                //

                /*Paragraph para = new Paragraph();
                Chunk glue = new Chunk(new iTextSharp.text.pdf.draw.VerticalPositionMark());
                Phrase ph1 = new Phrase();
                //ph1.Add(new Chunk(Environment.NewLine));
                string blank = "";
                string Contractor = "Contractor E-mail: "+collection[0].ToString()+"";
                Paragraph main = new Paragraph();
                ph1.Add(new Chunk(blank)); // Here I add projectname as a chunk into Phrase.    
                ph1.Add(glue); // Here I add special chunk to the same phrase.    
                ph1.Add(new Chunk(Contractor)); // Here I add date as a chunk into same phrase.   
                main.Add(ph1);
                para.Add(main);
                document.Add(para);

                Paragraph para2 = new Paragraph();
                Chunk glue2 = new Chunk(new iTextSharp.text.pdf.draw.VerticalPositionMark());
                Phrase ph2 = new Phrase();
                //ph2.Add(new Chunk(Environment.NewLine));
                string Project = "Project Name : "+ collection[1].ToString() + "";
                string Manager = "Project Manager: "+ collection[2].ToString() + "";
                Paragraph main2 = new Paragraph();
                ph2.Add(new Chunk(Project)); // Here I add projectname as a chunk into Phrase.    
                ph2.Add(glue2); // Here I add special chunk to the same phrase.    
                ph2.Add(new Chunk(Manager)); // Here I add date as a chunk into same phrase. 
                ph2.Add(new Chunk(Environment.NewLine));
                ph2.Add(new Chunk(Environment.NewLine));
                main2.Add(ph2);
                para2.Add(main2);
                document.Add(para2);*/

                //PdfPCell cell = new PdfPCell(new Phrase("Products"));
                //cell.Colspan = dt.Columns.Count;
                PdfPTable table = new PdfPTable(dt.Columns.Count);
                float[] widths = new float[] { 2f, 1f, 1f, 1f, 1f, 1f, 1f, 0.85f, 1f, 1f };
                table.SetWidths(widths);
                table.WidthPercentage = 100;

                foreach (DataColumn c in dt.Columns)
                {
                    PdfPCell cell = new PdfPCell();
                    //cell.Phrase = new Phrase(new Phrase(c.ColumnName, font5));
                    //table.AddCell(cell);
                    if (c.ColumnName == "TotalWorkInWeek")
                    {
                        //table.AddCell(new Phrase("Total Work\nIn Week", font5));
                        cell.HorizontalAlignment = 1;           // 0=left, 1=middle, 2 right
                        cell.VerticalAlignment = 1;
                        cell.Phrase = new Phrase("Total Work\nIn Week", font5);
                        table.AddCell(cell);
                    }
                    else if (c.ColumnName == "Pay" || c.ColumnName == "Total")
                    {
                        //table.AddCell(new Phrase("", font5));
                        cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Phrase = new Phrase("", font5);
                        table.AddCell(cell);
                    }
                    else
                    {
                        //table.AddCell(new Phrase(c.ColumnName, font5));
                        cell.HorizontalAlignment = 1;
                        cell.VerticalAlignment = 1;
                        cell.Phrase = new Phrase(c.ColumnName, font5);
                        table.AddCell(cell);
                    }

                }

                foreach (DataRow r in dt.Rows)
                {
                    if (dt.Rows.Count > 0)
                    {
                        int i = 0;
                        for (int h = 0; h < dt.Columns.Count; h++)
                        {
                            PdfPCell cell = new PdfPCell();
                            //table.AddCell(new Phrase(r[h].ToString(), font5));
                            i++;
                            string df = r[h].ToString();
                            /*if (i == 1) 
                            {
                                cell.HorizontalAlignment = 0;       // 0=left, 1=middle, 2 right
                                cell.VerticalAlignment = 0;
                                cell.Phrase = new Phrase(r[h].ToString(), font5);
                                table.AddCell(cell);
                            }
                     else*/ if ((i == 9 || i == 10) && df!= "Pay" && df != "Total")
                            {
                                cell.HorizontalAlignment = 2;       // 0=left, 1=middle, 2 right
                                cell.VerticalAlignment = 2;
                                cell.Phrase = new Phrase(r[h].ToString(), font5);
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell.HorizontalAlignment = 1;
                                cell.VerticalAlignment = 1;
                                cell.Phrase = new Phrase(r[h].ToString(), font5);
                                table.AddCell(cell);
                            }
                        }
                    }
                }
                document.Add(table);
                
                PdfPTable tab = new PdfPTable(5);
                float[] tabwidths = new float[] { 1f, 1f, 1f, 1.5f, 0.5f };
                tab.SetWidths(tabwidths);
                tab.WidthPercentage = 100;
                

                //cell.Border = Rectangle.NO_BORDER; // | Rectangle.TOP_BORDER;
                //cell.BorderWidthBottom = 5f;
                tab.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                
                //row 1
                tab.AddCell("\n\n\nTOTAL WO IN\n DATA DEPOT");
                tab.AddCell("\n\n\nTOTAL\n HOURS");
                tab.AddCell("\n\n\nAVG PER\n HOUR");
                tab.AddCell("\n\n\nContractor Signature       ");
                tab.AddCell("\n\n " + DateTime.Now.ToString("MM/dd/yyyy") + "\n    Date");

                //row 2
                tab.AddCell("     "+ collection[7].ToString() + "");
                tab.AddCell(" "+ collection[8].ToString() + "");
                tab.AddCell(" "+ collection[9].ToString() + "");
                tab.AddCell("Project Manager Signature  ");
                tab.AddCell("  " + DateTime.Now.ToString("MM/dd/yyyy") + "\n     Date");
                document.Add(tab);
                document.Close();
            }
            catch(Exception ex)
            { }
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
    }
}
