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
                                    DisplayWeekRangeDate = "Week Date Range : " + item.Item1.ToString("dd-MMM-yyyy")+" To "+ item.Item2.ToString("dd-MMM-yyyy")
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

        public List<Get_CTR> GetFormList(int installerid, string weekrangedate)         // CTR = Clock Time Report
        {
            try
            {
                int i = 0;
                List<string> weekDayName = new List<string>();      // First of all one static list create in week days as me requirement
                weekDayName.Add("Monday");
                weekDayName.Add("Tuesday");
                weekDayName.Add("Wednesday");
                weekDayName.Add("Thursday");
                weekDayName.Add("Friday");
                weekDayName.Add("Saturday");
                weekDayName.Add("Sunday");

                List<DetailDayTemp> objDetailDayTemp = new List<DetailDayTemp>();   // Temp list
                List<Get_CTR> final_CTR_List = new List<Get_CTR>();                 // CTR = Clock Time Report
                var WeekRangefromDate = Convert.ToDateTime(weekrangedate.Split()[0]).ToString("yyyy-MM-dd"); // From date format change as my requirement
                var WeekRangetoDate = Convert.ToDateTime(weekrangedate.Split()[3]).ToString("yyyy-MM-dd");  // To date format change as my requirement
                CompassEntities CompassEntities = new CompassEntities();
                SqlConnection con = new SqlConnection(CompassEntities.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;
                con.Open();

                // Add 21 Oct 2020
                // This function use to get All selected week date renage all date list in dynamic form show in html view table in first row
                final_CTR_List = getDynamicDates(Convert.ToDateTime(WeekRangefromDate), Convert.ToDateTime(WeekRangetoDate));
                //

                //get time Clock Id on installer id
                SqlDataAdapter sda_TimeClockId = new SqlDataAdapter("select Distinct(FKTimeClockId) from UserTimeClockDetail WHERE  (CONVERT(DATE, CreatedOn) between CONVERT(DATE, '" + WeekRangefromDate + "') and CONVERT(DATE, '" + WeekRangetoDate + "')) and FKInstallerId='" + installerid + "'", con);
                DataTable dt_TimeClockId = new DataTable();
                sda_TimeClockId.Fill(dt_TimeClockId);
                if (dt_TimeClockId.Rows.Count > 0)
                {
                    
                    foreach (DataRow TC_Id in dt_TimeClockId.Rows)      // TC_Id= TimeclockID   or allre=All Record
                    {
                        objDetailDayTemp = new List<DetailDayTemp>();
                        string _id = TC_Id["FKTimeClockId"].ToString();      // Get Time Colck Report use below query
                        SqlDataAdapter sda_allre = new SqlDataAdapter("Select TCM.Description,in_Out_Time = 'In : '+FORMAT(UTCD.InTime,'hh:mm tt')+' - Out : '+FORMAT(UTCD.OutTime,'hh:mm tt'),Datename(weekday,UTCD.CreatedOn) DayName from UserTimeClockDetail as UTCD join TimeClockMaster as TCM on UTCD.FKTimeClockId=TCM.Id WHERE (CONVERT(DATE, UTCD.CreatedOn) between CONVERT(DATE, '" + WeekRangefromDate + "') and CONVERT(DATE, '" + WeekRangetoDate + "')) and UTCD.FKInstallerId='" + installerid + "' and UTCD.FKTimeClockId='" + _id + "' order by DayName", con);
                        DataTable dt_allre = new DataTable();
                        sda_allre.Fill(dt_allre);
                        if (dt_allre.Rows.Count > 0)
                        {
                            foreach (var w in weekDayName)
                            {
                                string day = w.ToString();
                                i = 0;
                                string Dis = string.Empty;
                                foreach (DataRow iot in dt_allre.Rows)
                                {
                                    Dis = iot["Description"].ToString();
                                    string in_Out_Time = iot["in_Out_Time"].ToString();
                                    string DayName = iot["DayName"].ToString();
                                    if (day == DayName)
                                    {
                                        objDetailDayTemp.Add(new DetailDayTemp
                                        {
                                            Description = Dis,
                                            Day = day,
                                            ClockTime = in_Out_Time
                                        });
                                        i++;
                                    }
                                }
                                if (i == 0)
                                {
                                    objDetailDayTemp.Add(new DetailDayTemp
                                    {
                                        Description = Dis,
                                        Day = day,
                                        ClockTime = "No Record Found"
                                    });
                                }
                            }
                        }

                        // Store Time clock data day day one or more time break time use with "\n"
                        List<DetailDay> objDetailDay = new List<DetailDay>();  
                        if (objDetailDayTemp != null)
                        {
                            foreach (var w in weekDayName)
                            {
                                string day = w.ToString();
                                var filterlist = objDetailDayTemp.Where(o => o.Day == day);
                                String _dis = string.Empty;
                                String _ClockTime = string.Empty;
                                foreach (var m in filterlist)
                                {
                                    _dis = m.Description;
                                    _ClockTime += m.ClockTime + "\n";   // Store multiple time in a day
                                }
                                objDetailDay.Add(new DetailDay
                                {
                                    Description = _dis,
                                    Day = day,
                                    ClockTime = _ClockTime
                                });
                            }
                        }
                        
                        String __Dis = string.Empty;
                        String Monday = string.Empty;
                        String Tuesday = string.Empty;
                        String Wednesday = string.Empty;
                        String Thursday = string.Empty;
                        String Friday = string.Empty;
                        String Saturday = string.Empty;
                        if (objDetailDay != null)
                        {
                            foreach (var f in objDetailDay)
                            {
                                __Dis = f.Description;
                                if (f.Day == "Monday")
                                { Monday = f.ClockTime; }
                                if (f.Day == "Tuesday")
                                { Tuesday = f.ClockTime; }
                                if (f.Day == "Wednesday")
                                { Wednesday = f.ClockTime; }
                                if (f.Day == "Thursday")
                                { Thursday = f.ClockTime; }
                                if (f.Day == "Friday")
                                { Friday = f.ClockTime; }
                                if (f.Day == "Saturday")
                                { Saturday = f.ClockTime; }
                            }
                            final_CTR_List.Add(new Get_CTR      // Store final display record view in html table view page
                            {
                                Description = __Dis,
                                Monday = Monday,
                                Tuesday = Tuesday,
                                Wednesday = Wednesday,
                                Thursday = Thursday,
                                Friday = Friday,
                                Saturday = Saturday,
                                TotalWeekInstalls = ""
                            }) ;
                        }
                    }
                }

                //
                /*cmd.CommandText = "PROC_Get_ClockTimeReport";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;
                cmd.Parameters.Add("@WeekRangefromDate", WeekRangefromDate);
                cmd.Parameters.Add("@WeekRangetoDate", WeekRangetoDate);
                cmd.Parameters.Add("@installerId", installerid);
                
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Get_CTR
                    {
                        Description = Convert.ToString(reader["Description"]),
                        Monday = Convert.ToString(reader["Monday"]),
                        Tuesday = Convert.ToString(reader["Tuesday"]),
                        Wednesday = Convert.ToString(reader["Wednesday"]),
                        Thursday = Convert.ToString(reader["Thursday"]),
                        Friday = Convert.ToString(reader["Friday"]),
                        Saturday = Convert.ToString(reader["Saturday"])
                    });
                }
                con.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                con.Dispose();*/
                //return list.OrderBy(o => o.Description).ToList();
                con.Close();
                return final_CTR_List;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

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
                        __dis = "";                 // Discription Null store because discription not show any date
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
                        TotalWeekInstalls = ""
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
    }
}
