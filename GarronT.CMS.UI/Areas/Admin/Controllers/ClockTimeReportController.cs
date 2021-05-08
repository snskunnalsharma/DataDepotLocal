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
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class ClockTimeReportController : Controller
    {
        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(CMS_MobileService));
        #endregion
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
                List<Get_CTR_New> objGet_CTR = new List<Get_CTR_New>();
                objGet_CTR = null;
                int objTotalInstalledMeterInWeek = 0;
                decimal objTotalWorkingHoursInWeek = 0.00m;
                decimal objAvrPerHours = 0m;
                var jsonResult = Json(new { status = true, demo1 = objGet_CTR, TotalInstalledMeterInWeek = objTotalInstalledMeterInWeek, TotalWorkingHoursInWeek = objTotalWorkingHoursInWeek, AvrPerHours = objAvrPerHours }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        // Get installer Active project list
        [HttpGet]
        public ActionResult GetProject()
        {
            try
            {
                ProjectMasterDAL projectMasterDAL = new ProjectMasterDAL();
                IEnumerable<ProjectModel> data = (from a in projectMasterDAL.GetProjectName()
                                                  orderby a.ProjectName
                                                  select a).Distinct<ProjectModel>();
                var checkAdmin = Session["CurrentUserIsAdmin"].ToString();
                return base.Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public JsonResult GetInstallerList(long ProjectID)
        {
            List<UsersSmallModel> obj1 = new List<UsersSmallModel>();
            if (ProjectID > 0)
            {
                using (UserRoleDAL obj = new UserRoleDAL())
                {
                    obj1 = obj.GeInstallerListCurrentProject(ProjectID).OrderBy(a => a.UserName).ToList();
                }
            }

            return Json(obj1, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetYearList(long ProjectID)
        {
            #region get year list from start project to till current date fill the dropdown in OrderByDescending
            CompassEntities objCompassEntities = new CompassEntities();
            List<YearsListModel> obj = new List<YearsListModel>();
            var projectDetail = objCompassEntities.tblProjects.Where(o => o.ProjectId == ProjectID).FirstOrDefault();
            var projectStartYear =Convert.ToDateTime(projectDetail.FromDate);
            var currentYear = DateTime.Now;
            for (var i = projectStartYear.Year; i <= currentYear.Year; i++)
            {
                obj.Add(new YearsListModel
                {
                    YearId = i,
                    YearName = Convert.ToString(i)
                });
            }
            var list= obj.OrderByDescending(o => o.YearId).ToList();
            return base.Json(list, JsonRequestBehavior.AllowGet);
            #endregion
        }

        [HttpGet]
        public JsonResult GetWeekDayRangeList_old(int _year, int _month)
        {
            try
            {
                log.Info("GetWeekDayRangeList Started at - " + DateTime.Now.ToString());
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
                log.Info("Error at for this Method GetWeekDayRangeList - " + DateTime.Now.ToString() + " -" + ex.Message);
                return null;
            }
        }

        public JsonResult GetWeekDayRangeList(int _year, long ProjectID)
        {
            try
            {
                log.Info("GetWeekDayRangeList Started at - " + DateTime.Now.ToString());

                CompassEntities objCompassEntities = new CompassEntities();
                var projectDetail = objCompassEntities.tblProjects.Where(o => o.ProjectId == ProjectID).FirstOrDefault();
                var projectStartDate = Convert.ToDateTime(projectDetail.FromDate);

                var jan1 = new DateTime(_year, 1, 1);
                var startOfFirstWeek = jan1.AddDays(1 - (int)(jan1.DayOfWeek));
                var week =
                    Enumerable
                        .Range(0, 54)
                        .Select(i => new {
                            weekStart = startOfFirstWeek.AddDays(i * 7)
                        })
                        .TakeWhile(x => x.weekStart.Year <= jan1.Year)
                        .Select(x => new {
                            x.weekStart,
                            weekFinish = x.weekStart.AddDays(6)
                        })
                        .SkipWhile(x => x.weekFinish < jan1.AddDays(1))
                        .Select((x, i) => new {
                            x.weekStart,
                            x.weekFinish,
                            weekNum = i + 1
                        });

                List<Tuple<DateTime, DateTime, int>> weeksrangedate = week.Select(g => new Tuple<DateTime, DateTime,int>(g.weekStart, g.weekFinish,g.weekNum)).ToList();
                List<WeeksModel> objWeeksModel = new List<WeeksModel>();
                if (weeksrangedate != null)
                {
                    objWeeksModel = new ClockTimeReportDAL().GetWeekRangeDateCollection_new(weeksrangedate, projectStartDate);
                }
                return Json(objWeeksModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Info("Error at for this Method GetWeekDayRangeList - " + DateTime.Now.ToString() + " -" + ex.Message);
                return null;
            }
        }

        public ActionResult GetUserTimeClockReport(long _ProjectID, int _installer, string _WeekRangeDate)
        {
            List<Model_FinalResult> finalResult = new List<Model_FinalResult>();
            List<Get_CTR_New> objGet_CTR = new List<Get_CTR_New>();
            try
            {
                log.Info("GetUserTimeClockReport Started at - " + DateTime.Now.ToString());
                finalResult = new List<Model_FinalResult>();
                finalResult = new ClockTimeReportDAL().GetInstallerTotalWorkedTime(_ProjectID, _installer, _WeekRangeDate);
                var objList = finalResult.Select(o => o._GetCTR).ToList();
                int objTotalInstalledMeterInWeek = 0;
                decimal objTotalWorkingHoursInWeek = 0.00m;
                decimal objAvrPerHours = 0m;
                string objfilePath = string.Empty;
                foreach (var item in finalResult)
                {
                    objTotalInstalledMeterInWeek = item.TotalInstalledMeterInWeek;
                    objTotalWorkingHoursInWeek = item.TotalWorkingHoursInWeek;
                    objAvrPerHours = item.AvgPerHours;
                    objfilePath = item.filePath;
                }
                foreach (var item in objList)
                {
                    foreach (var i in item) 
                    {
                        objGet_CTR.Add(new Get_CTR_New
                        {
                            Description = i.Description,
                            Monday = i.Monday,
                            Tuesday = i.Tuesday,
                            Wednesday = i.Wednesday,
                            Thursday = i.Thursday,
                            Friday = i.Friday,
                            Saturday = i.Saturday,
                            TotalWorkInWeek = i.TotalWorkInWeek,
                            Pay = i.Pay,
                            Total = i.Total
                        });
                    }
                }
                var jsonResult = Json(new { status = true, demo1 = objGet_CTR, TotalInstalledMeterInWeek = objTotalInstalledMeterInWeek, TotalWorkingHoursInWeek = objTotalWorkingHoursInWeek, AvrPerHours = "$ " + objAvrPerHours, filePath= objfilePath }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                log.Info("Error at for this Method GetUserTimeClockReport - " + DateTime.Now.ToString() + " -" + ex.Message);
                objGet_CTR = null;
                return Json(new { status = false, demo1 = objGet_CTR }, JsonRequestBehavior.AllowGet);
            }
        }
    }

}