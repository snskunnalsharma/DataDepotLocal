using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using OfficeOpenXml;
using GarronT.CMS.Model;
using System.Collections;
using Microsoft.AspNet.Identity;
using GarronT.CMS.UI.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Web.UI;
using System.Text;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using KSPL.Indus.MSTS.UI.Filters;
using AutoMapper;
using System.Configuration;
using System.Threading;
using iTextSharp.tool.xml;
using Rotativa.Options;
using log4net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Hosting;
using GarronT.CMS.UI.CustomHelpers;
using System.Globalization;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    // this page create by Deepak date 28 sep 2020
    public class ClockTimeReportController : Controller
    {
        
        // GET: Admin/ClockTimeReport
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getListDetailsOnBack()      // Copy other page code changes are required
        {
            try
            {
                if (TempData["isbackFromEdit"] != null)
                {
                    TempData.Keep("isbackFromEdit");
                    //bool result = getdata();
                    bool result = false;    // Use other call function
                    if (result)
                    {
                        List<STP_GetReportTableNew_bak_Result> demo1 = (List<STP_GetReportTableNew_bak_Result>)TempData["List"];
                        var jsonResult = Json(new { status = true, demo1 = demo1 }, JsonRequestBehavior.AllowGet);
                        jsonResult.MaxJsonLength = int.MaxValue;
                        return jsonResult;
                    }
                    else
                    {
                        List<STP_GetReportTableNew_Result> demo1 = (List<STP_GetReportTableNew_Result>)TempData["List"];
                        var jsonResult = Json(new { status = true, demo1 = demo1 }, JsonRequestBehavior.AllowGet);
                        jsonResult.MaxJsonLength = int.MaxValue;
                        return jsonResult;
                    }
                }
                else
                {
                    var jsonResult = Json(null, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ActionResult GetReportsList()    // Copy other page code changes are required
        {
            try
            {
                if (TempData["List"] == null)
                {
                    var jsonResult = Json(JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                else
                {
                    var list = TempData["List"];
                    TempData.Keep("List");

                    TempData["isbackFromEdit"] = null;

                    TempData.Keep("isbackFromEdit");
                    var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        // Get installer name list
        [HttpGet]
        public JsonResult GetInstallerList()
        {
            try
            {
                List<UsersSmallModel> InstallerList = new List<UsersSmallModel>();
                InstallerList = new ReportDAL().GetAllInstallerList();
                return Json(InstallerList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //

        // Add this method By Deepak 15 Oct 2020
        [HttpGet]
        public JsonResult GetWeekDayRangeList(int _year, int _month)
        {
            try
            {
                //DateTime reference = DateTime.Now;
                Calendar calendar = CultureInfo.CurrentCulture.Calendar;
                //IEnumerable<int> daysInMonth = Enumerable.Range(1, calendar.GetDaysInMonth(reference.Year, reference.Month));
                //List<Tuple<DateTime, DateTime>> weeks2 = daysInMonth.Select(day => new DateTime(reference.Year, reference.Month, day)).GroupBy(d => calendar.GetWeekOfYear(d, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)).Select(g => new Tuple<DateTime, DateTime>(g.First(), g.Last())).ToList();

                IEnumerable<int> daysInMonth = Enumerable.Range(1, calendar.GetDaysInMonth(_year, _month));
                List<Tuple<DateTime, DateTime>> weeksrangedate = daysInMonth.Select(day => new DateTime(_year, _month, day))
                    .GroupBy(d => calendar.GetWeekOfYear(d, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday))
                    .Select(g => new Tuple<DateTime, DateTime>(g.First(), g.Last()))
                    .ToList();
                //weeks2.ForEach(x => Console.WriteLine("{0:MM/dd/yyyy} - {1:MM/dd/yyyy}", x.Item1, x.Item2));

                List<WeeksModel> objWeeksModel = new List<WeeksModel>();
                if (weeksrangedate != null)
                {
                    objWeeksModel = new ClockTimeReportDAL().GetWeekRangeDateCollection(weeksrangedate);
                }
                else
                {

                }
                return Json(objWeeksModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // Add this method By Deepak 17 Oct 2020
        public ActionResult GetUserTimeClockReport(int _installer,string _WeekRangeDate)
        {
            try
            {
                List<Get_CTR> objGet_CTR = new List<Get_CTR>(); // CTR = Clock Time Report
                objGet_CTR = new ClockTimeReportDAL().GetFormList(_installer, _WeekRangeDate);
                var jsonResult = Json(new { status = true, demo1 = objGet_CTR }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
                //return Json(objGet_CTR, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
       
    }

}