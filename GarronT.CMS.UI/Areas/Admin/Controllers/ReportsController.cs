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


namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class ReportsController : Controller
    {

        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportsController));
        #endregion


        //
        // GET: /Admin/Reports/
        public ActionResult Index()
        {
            Session["objnewReportEditBO"] = null;
            if (TempData["isbackFromEdit"] == null)
            {
                TempData["List"] = null;
                TempData["ProjectId"] = null;
                TempData["InstallerIdList"] = null;
                TempData["fromDate"] = null;
                TempData["Todate"] = null;
                TempData["cycleIdList"] = null;
                TempData["RouteIdList"] = null;
                TempData["AdressIdList"] = null;
            }
            else
            {
                //isbackFromEdit = false;
                TempData.Keep("List");
                TempData.Keep("isbackFromEdit");
            }
            return View();
        }

        #region--Createdby AniketJ on 30-5-2016--

        [HttpPost]
        public ActionResult GetPojectbyProjectStatus(List<string> ProjectStatusList)
        {
            List<ProjectModel> listProjectModel = new List<ProjectModel>();
            if (ProjectStatusList != null)
            {
                listProjectModel = new ReportDAL().GetProjectNamebyProjectStatus(ProjectStatusList);
            }

            var checkAdmin = Session["CurrentUserIsAdmin"].ToString();

            if (checkAdmin == "0")
            {
                var userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                var abcd = new ProjectMasterDAL().GetUserProjectList(userId);


                if (abcd == null || abcd.Count == 0)
                {
                    if (listProjectModel != null && listProjectModel.Count() > 0)
                    {
                        listProjectModel = listProjectModel.Take(0).ToList();
                    }
                }
                else
                {
                    if (listProjectModel != null && listProjectModel.Count() > 0)
                    {
                        listProjectModel = listProjectModel.Where(o => abcd.Contains(o.ProjectId)).ToList();
                    }
                }

            }
            return Json(listProjectModel, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetInstallerByProjectID(long projectId)
        {
            List<UsersSmallModel> InstallerList = new List<UsersSmallModel>();
            if (projectId > 0)
            {
                InstallerList = new ReportDAL().GeInstallerListbyProjectID(projectId);
            }

            return Json(InstallerList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetReopenData(long projectId)
        {


            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Reports", "Edit");

            if (objPermissionCheck)
            {
                List<UsersSmallModel> InstallerList = new List<UsersSmallModel>();
                List<ReasonBO> ReasonBOList = new List<ReasonBO>();
                if (projectId > 0)
                {
                    InstallerList = new ReportDAL().GeInstallerListbyProjectID(projectId);
                    ReasonBOList = new ReportDAL().GetReopenReasonList();
                }
                return Json(new { InstallerList = InstallerList, ReopenList = ReasonBOList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetCycleListByProjectID(int ProjectID)
        {

            List<string> CycleList = new ReportDAL().GetCycleByProjectID((long)ProjectID);

            return base.Json(CycleList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRouteListByCycle(long ProjectId, string[] Cycle)
        {

            List<string> routeList = new ReportDAL().GetRouteListByCycle_ProjectID(Cycle, ProjectId);

            System.Collections.Generic.List<string> sortedRouteList = new System.Collections.Generic.List<string>();


            //sorted numeric string list
            var sorted = routeList.Select(str => new NameAndNumber(str))
                        .OrderBy(n => n.Name)
                        .ThenBy(n => n.Number).ToList();

            sortedRouteList = (from m in sorted select m.OriginalString).ToList();

            return base.Json(new
            {
                Route = sortedRouteList,

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAddressbyProjectRouteCycle(List<string> Route, List<string> Cycle, long ProjectID, string fromDate, string toDate)
        {
            CompassEntities dbcontext = new CompassEntities();

            List<tblUploadedData> Addresslist = new ReportDAL().GetAddress(Route, Cycle, ProjectID, fromDate, toDate);


            JsonResult jsonResult = base.Json(new
            {
                AllAddress = Addresslist,

            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = new int?(2147483647);
            return jsonResult;
        }

        #endregion


        public ActionResult EditMode()
        {
            TempData["isbackFromEdit"] = "true";
            TempData.Keep("isbackFromEdit");
            return RedirectToAction("Index");
        }

        public ActionResult ResetData()
        {
            TempData["isbackFromEdit"] = null;
            //TempData.Keep("isbackFromEdit");
            var jsonResult = Json(true, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }



        [HttpPost]
        public ActionResult Save_Category()
        {
            string Name = Request.Form[1];
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
            }

            return View();
        }

        #region GetReportList changed by vishal

        public ActionResult GetReportsList()
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

        #endregion
        public ActionResult getListDetailsOnBack()
        {
            try
            {
                if (TempData["isbackFromEdit"] != null)
                {
                    TempData.Keep("isbackFromEdit");
                    bool result = getdata();
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

        #region Modified method By vishal

        public ActionResult GetSkipEditData(long _ID, long _InstallerId, long _FormID)
        {
            string _webUrl = ConfigurationManager.AppSettings["WebURL"].ToString();

            if (TempData["List"] != null)
            {

                List<STP_GetReportTableNew_bak_Result> demo1 = (List<STP_GetReportTableNew_bak_Result>)TempData["List"];
                if (demo1.Count > 0)
                {
                    //AddedBy AniketJ on 7-oct-2016 to get media
                    List<SkipRtuAttachmentModel> objAttachementList = new ReportDAL().getSkipRtuImagesWithoutMap(_ID, _InstallerId, _FormID);
                    STP_GetReportTableNew_bak_Result skipRTUModel = new STP_GetReportTableNew_bak_Result();
                    skipRTUModel = demo1.Where(i => i.ID == _ID).FirstOrDefault();

                    foreach (var item in objAttachementList)
                    {
                        string strFilePath = item.strFilePath;
                        if (!string.IsNullOrEmpty(item.strFilePath))
                        {
                            // item.strFilePath = "" + item.strFilePath.Replace("..\\..\\", _webUrl);

                            if (item.strFilePath.Contains("..\\..\\"))
                            {
                                item.strFilePath = "" + item.strFilePath.Replace("..\\..\\", _webUrl);
                            }

                            if (item.strFilePath.Contains("~"))
                            {
                                item.strFilePath = "" + item.strFilePath.Replace("~\\", _webUrl);
                            }
                            item.strFilePath = "" + item.strFilePath.Replace('\\', '/');
                            item.strFilePath = item.strFilePath.Trim();
                        }
                    }

                    string MapImagePath = new ReportDAL().getSkipRtuMapPath(_ID, _InstallerId, _FormID);

                    skipRTUModel.skipGPSLocation = new ReportDAL().getSkipRtuGPSLocation(_ID, _InstallerId, _FormID);


                    //MapImagePath = "" + MapImagePath.Replace("..\\..\\", _webUrl);

                    if (MapImagePath.Contains("..\\..\\"))
                    {
                        MapImagePath = "" + MapImagePath.Replace("..\\..\\", _webUrl);
                    }

                    if (MapImagePath.Contains("~"))
                    {
                        MapImagePath = "" + MapImagePath.Replace("~\\", _webUrl);
                    }
                    MapImagePath = "" + MapImagePath.Replace('\\', '/');
                    MapImagePath = MapImagePath.Trim();
                    skipRTUModel.mapImagePath = MapImagePath;
                    skipRTUModel.SkipRtuAttachmentModel = objAttachementList;

                    TempData.Keep("List");

                    var jsonResult = Json(skipRTUModel, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                else
                {
                    var jsonResult = Json(demo1, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
            }
            else
            {
                List<STP_GetReportTableNew_bak_Result> demo1 = new List<STP_GetReportTableNew_bak_Result>();
                var jsonResult = Json(demo1, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }

        }


        /// <summary>
        /// Button Search EVENT
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="InstallerId"></param>
        /// <param name="fromDate"></param>
        /// <param name="Todate"></param>
        /// <param name="cycleId"></param>
        /// <param name="RouteId"></param>
        /// <param name="AdressId"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public ActionResult getListDetails(long ProjectId, List<string> InstallerId, string fromDate, string Todate, List<string> cycleId, List<string> RouteId, List<string> AdressId, List<string> Status)
        {
            try
            {
                TempData["ProjectId"] = ProjectId;
                TempData["InstallerIdList"] = InstallerId;
                if (InstallerId != null && InstallerId.Count != 0)
                {
                    TempData["InstallerIdList"] = InstallerId;
                }
                TempData["fromDate"] = fromDate;
                TempData["Todate"] = Todate;
                TempData["cycleIdList"] = cycleId;
                TempData["RouteIdList"] = RouteId;
                TempData["AdressIdList"] = AdressId;
                TempData["StatusList"] = Status;
                TempData.Keep("ProjectId");
                TempData.Keep("InstallerIdList");
                TempData.Keep("fromDate");
                TempData.Keep("Todate");
                TempData.Keep("cycleIdList");
                TempData.Keep("RouteIdList");
                TempData.Keep("AdressIdList");

                bool result = getdata();
                if (result)
                {
                    List<STP_GetReportTableNew_bak_Result> demo1 = (List<STP_GetReportTableNew_bak_Result>)TempData["List"];
                    TempData.Keep("List");
                    //demo1 = demo1.OrderByDescending(a => a.Dates).ThenBy(a => a.Account).ToList();//demo1 = demo1.OrderBy(a => a.Dates).ToList();
                    demo1 = demo1.ToList();
                    var jsonResult = Json(new { status = true, demo1 = demo1 }, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                else
                {
                    List<STP_GetReportTableNew_bak_Result> demo1 = (List<STP_GetReportTableNew_bak_Result>)TempData["List"];
                    //demo1.OrderByDescending(a => a.Dates);
                    demo1.ToList();
                    var jsonResult = Json(new { status = true, demo1 = demo1 }, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, ErrorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult SelectAll(long ProjectId, List<string> InstallerId, string fromDate, string Todate, List<string> cycleId, List<string> RouteId, List<string> AdressId, List<string> Status)
        {
            try
            {
                TempData["ProjectId"] = ProjectId;
                TempData["InstallerIdList"] = InstallerId;
                if (InstallerId != null && InstallerId.Count != 0)
                {
                    TempData["InstallerIdList"] = InstallerId;
                }
                TempData["fromDate"] = fromDate;
                TempData["Todate"] = Todate;
                TempData["cycleIdList"] = cycleId;
                TempData["RouteIdList"] = RouteId;
                TempData["AdressIdList"] = AdressId;
                TempData["StatusList"] = Status;
                TempData.Keep("ProjectId");
                TempData.Keep("InstallerIdList");
                TempData.Keep("fromDate");
                TempData.Keep("Todate");
                TempData.Keep("cycleIdList");
                TempData.Keep("RouteIdList");
                TempData.Keep("AdressIdList");

                bool result = getdata();
                if (result)
                {
                    List<STP_GetReportTableNew_bak_Result> demo1 = (List<STP_GetReportTableNew_bak_Result>)TempData["List"];
                    TempData.Keep("List");
                    demo1 = demo1.OrderBy(a => a.Dates).ToList();
                    var jsonResult = Json(demo1, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                else
                {
                    List<STP_GetReportTableNew_bak_Result> demo1 = (List<STP_GetReportTableNew_bak_Result>)TempData["List"];
                    demo1.OrderByDescending(a => a.Dates);
                    var jsonResult = Json(demo1, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region getdata changed by vishal

        public bool getdata()
        {
            long ProjectId;
            List<string> InstallerId = null; string fromDate; string Todate;
            List<string> cycleId = null; List<string> RouteId = null; List<string> AdressId = null;
            List<string> StatusList = null;
            ProjectId = long.Parse(Convert.ToString(TempData["ProjectId"]));
            if (TempData["InstallerIdList"] != null)
            {
                InstallerId = (List<string>)TempData["InstallerIdList"];
            }

            fromDate = Convert.ToString(TempData["fromDate"]);
            Todate = Convert.ToString(TempData["Todate"]);

            if (TempData["cycleIdList"] != null)
            {
                cycleId = (List<string>)TempData["cycleIdList"];
            }
            if (TempData["RouteIdList"] != null)
            {
                RouteId = (List<string>)TempData["RouteIdList"];
            }
            if (TempData["AdressIdList"] != null)
            {
                AdressId = (List<string>)TempData["AdressIdList"];
            }
            if (TempData["StatusList"] != null)
            {
                StatusList = (List<string>)TempData["StatusList"];
            }



            TempData.Keep("ProjectId");
            TempData.Keep("InstallerIdList");
            TempData.Keep("fromDate");
            TempData.Keep("Todate");
            TempData.Keep("cycleIdList");
            TempData.Keep("RouteIdList");
            TempData.Keep("AdressIdList");
            TempData.Keep("StatusList");
            try
            {
                //System.Collections.Generic.List<STP_GetReportTable_Result> list2 = new System.Collections.Generic.List<STP_GetReportTable_Result>();
                string[] SplitFrm = fromDate.Split('-');
                string dtfrm = SplitFrm[1].ToString() + "-" + SplitFrm[0].ToString() + "-" + SplitFrm[2].ToString();
                string[] SplitTo = Todate.Split('-');
                string dtto = SplitTo[1].ToString() + "-" + SplitTo[0].ToString() + "-" + SplitTo[2].ToString();
                DateTime fd = Convert.ToDateTime(dtfrm);
                DateTime to = Convert.ToDateTime(dtto);
                string s = fd.ToString("dd MMM yyyy");
                string t = to.ToString("dd MMM yyyy");
                DateTime fd1 = Convert.ToDateTime(s);
                DateTime to1 = Convert.ToDateTime(t);


                List<STP_GetReportTableNew_bak_Result> demo = new List<STP_GetReportTableNew_bak_Result>();
                List<STP_GetReportTableNew_bak_Result> demo1 = new List<STP_GetReportTableNew_bak_Result>();
                List<STP_GetReportTableNew_bak_Result> demoList = new List<STP_GetReportTableNew_bak_Result>();
                if (StatusList != null)  // If visit status  selected
                {
                    if (StatusList.Contains("PENDING") || StatusList.Contains("NOT ALLOCATED"))
                    {
                        List<STP_GetReportTableNew_bak_Result> Result1 = new List<STP_GetReportTableNew_bak_Result>();


                        Result1 = new ReportDAL().GetReportsRecordsNew_bak(ProjectId, null, null);
                        if (StatusList.Contains("SKIP") || StatusList.Contains("COMPLETED") || StatusList.Contains("RTU"))
                        {
                            if (Result1.Count > 0)
                            {
                                var Result3 = Result1.Where(a => !string.IsNullOrEmpty(a.Dates)
                                    && Convert.ToDateTime(a.Dates).Date >= fd1.Date && Convert.ToDateTime(a.Dates).Date <= to1.Date
                                    && a.CategoryType != "PENDING").ToList();
                                var Result2 = Result1.Where(n => n.CategoryType == "PENDING" || n.CategoryType == "NOT ALLOCATED").ToList();
                                demo.AddRange(Result3);
                                demo.AddRange(Result2);
                                var routes = demo.Select(d => d.Route).Distinct();
                            }
                        }
                        else
                        {

                            var Result2 = Result1.Where(n => n.CategoryType == "PENDING" || n.CategoryType == "NOT ALLOCATED").ToList();
                            demo.AddRange(Result2);
                        }

                    }
                    else
                    {
                        demo = new ReportDAL().GetReportsRecordsNew_bak(ProjectId, fd1, to1);
                    }

                    if (cycleId != null)
                    {
                        demo = demo.Where(a => cycleId.Contains(a.Cycle)).ToList();

                    }
                    if (RouteId != null)
                    {
                        demo = demo.Where(a => RouteId.Contains(a.Route)).ToList();

                    }
                    if (AdressId != null)
                    {
                        demo = demo.Where(a => AdressId.Contains(a.Street)).ToList();

                    }
                    if (StatusList != null)
                    {
                        demo = demo.Where(a => StatusList.Contains(a.CategoryType)).ToList();
                        var routes = demo.Select(d => d.Route).Distinct();

                    }
                    if (InstallerId != null)
                    {
                        demo = demo.Where(a => InstallerId.Contains(Convert.ToString(a.InstallerId))).ToList();
                        var routes = demo.Select(d => d.Route).Distinct();

                    }
                }
                else    // If visit status not selected
                {
                    var Result1 = new ReportDAL().GetReportsRecordsNew_bak(ProjectId, null, null);
                    List<string> list = new List<string>();
                    list.Add("SKIP");
                    list.Add("COMPLETED");
                    list.Add("RTU");
                    list.Add("PENDING");
                    list.Add("NOT ALLOCATED");

                    StatusList = list;

                    if (StatusList.Contains("SKIP") || StatusList.Contains("COMPLETED") || StatusList.Contains("RTU"))
                    {
                        if (Result1.Count > 0)
                        {
                            var Result3 = Result1.Where(a => !string.IsNullOrEmpty(a.Dates)
                                && Convert.ToDateTime(a.Dates).Date >= fd1.Date && Convert.ToDateTime(a.Dates).Date <= to1.Date && a.CategoryType != "PENDING").ToList();
                            var Result2 = Result1.Where(n => n.CategoryType == "PENDING" || n.CategoryType == "NOT ALLOCATED").ToList();
                            demo.AddRange(Result3);
                            demo.AddRange(Result2);
                        }
                    }
                    else
                    {

                        var Result2 = Result1.Where(n => n.CategoryType == "PENDING" || n.CategoryType == "NOT ALLOCATED").ToList();
                        demo.AddRange(Result2);
                    }

                    if (cycleId != null)
                    {
                        demo = demo.Where(a => cycleId.Contains(a.Cycle)).ToList();

                    }
                    if (RouteId != null)
                    {
                        demo = demo.Where(a => RouteId.Contains(a.Route)).ToList();

                    }
                    if (AdressId != null)
                    {
                        demo = demo.Where(a => AdressId.Contains(a.Street)).ToList();

                    }
                    if (StatusList != null)
                    {
                        demo = demo.Where(a => StatusList.Contains(a.CategoryType)).ToList();

                    }
                    if (InstallerId != null)
                    {
                        demo = demo.Where(a => InstallerId.Contains(Convert.ToString(a.InstallerId))).ToList();

                    }
                }

                // get duplicate records.
                List<DuplicateReportBO> objDuplicateList = new List<DuplicateReportBO>();
                using (DuplicateReportDAL obj = new DuplicateReportDAL())
                {
                    //fetch duplicate list from database
                    objDuplicateList = obj.GetDuplicateRecords(ProjectId, 0, 0);
                }

                foreach (var item1 in demo)
                {
                    if (!string.IsNullOrEmpty(item1.Dates))
                    {
                        item1.Dates = DateTime.Parse(item1.Dates.ToString()).ToString("MMM-dd-yyyy hh:mm tt");
                    }
                    if (!string.IsNullOrEmpty(item1.ShortComment))
                    {
                        if (item1.ShortComment.Length > 20)
                            item1.ShortComment = item1.SkipComment.Substring(0, 20) + "....";
                    }

                    if (objDuplicateList == null || objDuplicateList.Count == 0)
                    {
                        item1.IsDuplicate = false;
                    }
                    else
                    {
                        var countDuplicateRecord = objDuplicateList.Where(o => o.ID == item1.ID).Count();

                        item1.IsDuplicate = countDuplicateRecord == 0 ? false : true;
                    }


                    demo1.Add(item1);
                }
                // TempData["List"] = demo;
                //   list.AddRange(list);
                //TempData["List"] = demo1.OrderByDescending(o => o.Dates).ThenBy(o => o.Account).ToList();
                TempData["List"] = demo1.ToList();
                TempData.Keep("List");
                return true;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #endregion

        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        static readonly double MaxUnixSeconds = (DateTime.MaxValue - UnixEpoch).TotalSeconds;

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            return unixTimeStamp > MaxUnixSeconds
               ? UnixEpoch.AddMilliseconds(unixTimeStamp)
               : UnixEpoch.AddSeconds(unixTimeStamp);
        }

        public ActionResult Create(long userId, int projectId, long uploadId, long reportFormId)
        {

            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Reports", "Edit");

            if (objPermissionCheck)
            {
                return View();
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveData(FormCollection form, long userId, int projectId, long uploadId, long reportFormId)
        {

            //CompassEntities cmp = new CompassEntities();

            long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());

            try
            {
                form.Remove("userId");
                form.Remove("projectId");
                form.Remove("uploadId");
                form.Remove("reportFormId");

                // newReportEditBO obj1 = (newReportEditBO)Session["objnewReportEditBO"];
                newReportEditBO obj1 = new newReportEditBO
                {
                    accountinifiledId = long.Parse(Convert.ToString(uploadId)), //long.Parse(Convert.ToString(form["A1"])),
                    projectId1 = (int)projectId, //int.Parse(Convert.ToString(form["P1"])),
                    reportFormId = reportFormId, //long.Parse(Convert.ToString(form["RF1"])),
                    userId1 = userId //long.Parse(Convert.ToString(form["U1"]))
                };

                ReportDAL objReportDAL = new ReportDAL();

                long accountinifiledId = uploadId;// Convert.ToInt64(Session["accountinifiledId"]);
                List<string> strFieldNameList = new List<string>();
                List<string> strFieldValueList = new List<string>();
                List<string> strServiceList = new List<string>();
                List<string> strAddServiceList = new List<string>();

                // var id = cmp.TblProjectFieldDatas.Where(o => o.tblUploadedData_Id == accountinifiledId).Select(o => o.ID).FirstOrDefault();
                List<ReportEditBO> report = new List<ReportEditBO>();

                foreach (string _formData in form)
                {
                    if (_formData.Contains("S_"))
                    {
                        string[] last = _formData.Split('_');
                        strServiceList.Add(last[1]);
                    }
                    else if (_formData.Contains("A_"))
                    {
                        string[] last = _formData.Split('_');
                        strAddServiceList.Add(last[1]);
                    }
                    else
                    {
                        strFieldNameList.Add(_formData);
                        strFieldValueList.Add(form[_formData]);
                    }
                }

                var resultList = objReportDAL.saveHistory(obj1.reportFormId, obj1.accountinifiledId, obj1.userId1, currentUserId);

                bool result = objReportDAL.saveReportEdit(obj1.reportFormId, obj1.accountinifiledId, strFieldNameList, strFieldValueList, strServiceList, strAddServiceList);


                return Json(new { result = result, message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult getNextPreviousRedirectData(bool _valRecord, long userId, int projectId, long uploadId, long reportFormId)
        {
            //int index = 0;
            List<STP_GetReportTableNew_bak_Result> demo1 = (List<STP_GetReportTableNew_bak_Result>)TempData["List"];
            TempData.Keep("List");
            //demo1 = demo1.Where(o=>o.CategoryType!="SKIP" && o.CategoryType!="RTU" && o.CategoryType!="PENDING").OrderBy(o => o.Dates).ToList();
            demo1 = demo1.Where(o => o.CategoryType != "SKIP" && o.CategoryType != "RTU" && o.CategoryType != "PENDING").OrderByDescending(a => a.Dates).ThenBy(a => a.Account).ToList();

            STP_GetReportTableNew_bak_Result obj = new STP_GetReportTableNew_bak_Result();
            if (_valRecord)
            {
                var obj2 = demo1.Where(o => o.ProjectId == projectId && o.ID == uploadId && o.InstallerId == userId && o.FormId == reportFormId).FirstOrDefault();
                int index = demo1.IndexOf(obj2);

                if (index + 1 == demo1.Count)
                {
                    string result = "1";
                    return Json(new { Sucess = false, returnMsg = result, InstallerId = obj2.InstallerId, UploadId = obj2.ID, FormId = obj2.FormId.ToString() }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    obj = (index + 1 < demo1.Count) ? demo1[(index + 1)] : null;
                }


            }
            else
            {
                var obj2 = demo1.Where(o => o.ProjectId == projectId && o.ID == uploadId && o.InstallerId == userId && o.FormId == reportFormId).FirstOrDefault();
                int index = demo1.IndexOf(obj2);
                if (index == 0)
                {

                    return Json(new { Sucess = false, InstallerId = obj2.InstallerId, UploadId = obj2.ID, FormId = obj2.FormId.ToString() }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    obj = (index - 1 < demo1.Count) ? demo1[(index - 1)] : null;
                }

            }

            if (obj != null)
            {
                //string createurl = "Reports/Create";
                //var jsonResult = getHtmlData(long.Parse(obj.InstallerId.ToString()), projectId, obj.ID, long.Parse(obj.FormId.ToString()));
                //getHtmlData(long.Parse(obj.InstallerId.ToString()), projectId, obj.ID, long.Parse(obj.FormId.ToString()));
                return Json(new { Sucess = true, InstallerId = obj.InstallerId, UploadId = obj.ID, FormId = obj.FormId.ToString() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var jsonResult = Json(new { Sucess = false, htmlData = "", Details1 = "" }, JsonRequestBehavior.AllowGet);
                return jsonResult;
            }

        }

        [HttpGet]
        public ActionResult getHtmlData(long userId, int projectId, long uploadId, long reportFormId)
        {
            #region
            try
            {

                string _avClass = "";
                int i = 1;
                StringBuilder htmlData = new StringBuilder();

                // newReportEditBO obj1 = (newReportEditBO)Session["objnewReportEditBO"];
                newReportEditBO obj1 = new newReportEditBO { accountinifiledId = uploadId, projectId1 = projectId, userId1 = userId, reportFormId = reportFormId };

                List<FromDetails> ReportName = new List<FromDetails>();
                List<SectionList> Ids = new List<SectionList>();
                // long accountinifiledId = accountinifiledId;

                var Details = new ReportDAL().detailsOfProjetcs(obj1.userId1, obj1.projectId1, obj1.reportFormId);
                // Session["Details"] = Details;

                //Session["accountinifiledId"] = obj1.accountinifiledId;
                MobileDAL objMobile = new MobileDAL();

                var result = objMobile.getFormListDataNew(obj1.userId1, obj1.reportFormId, obj1.accountinifiledId);
                // Session["Field"] = result.FormNameList[0].SectionList;

                var objAttachmentRecords = new ReportDAL().getAudioVIdeoList(obj1.userId1, obj1.reportFormId, obj1.accountinifiledId);

                var FieldData = result.FormNameList[0].SectionList;

                int SectionCount = result.FormNameList[0].SectionList.Count;
                int currentSectionNumber = 1;
                var servicedata = new ReportDAL().getServiceList();

                //AddedBy AniketJ on 18-Oct-2016 to get projectFieldData
                // var ProjectFieldProperty = new ReportDAL().ProjectFieldPropertyList(obj1.reportFormId, obj1.projectId1);


                foreach (var item in FieldData)
                {
                    if (currentSectionNumber % 2 == 1)
                    {
                        htmlData.Append("<div class='row' style='background-color:white;'>");
                    }
                    #region
                    htmlData.Append("<div id=" + i + " class='col-sm-6 col-lg-6 col-xs-12 rep-fulldiv' style='background: white;'><br/><div  style=' text-align: center; background: gray; font-size: 14px; color: white;margin-bottom:8px;'><b>"
                        + item.sectionName + "</b></div>");//open section dive


                    var sectionDate = DispalyRecordsNew(item.sectionId, userId, projectId, uploadId, reportFormId, result.FormNameList[0].SectionList);

                    foreach (var item2 in sectionDate)
                    {
                        #region
                        //AddedBy AniketJ on 18-Oct-2016
                        string requiredicon = "";
                        // var filedProperty = ProjectFieldProperty.Where(a => a.FormSectionFieldID == item2.FieldId).FirstOrDefault();





                        if (item2.IsRequired == true)
                        {
                            requiredicon = "<span style='color:red'>*</span>";
                        }


                        if (item2.FielddatatypeNames == "Text")
                        {
                            #region
                            htmlData.Append("<div class='col-lg-12 drp-div1'>");
                            htmlData.Append("<div class='col-lg-6 col-sm-12 fld-lab2' style='text-align:left'>" + item2.FieldName + requiredicon + " </div>");
                            htmlData.Append("<input class='col-lg-5 col-sm-12 rep-inpt' style='margin: 9px 0px 0px 0px;'id=''" + item2.FieldId + "' name = '" + item2.FieldName +
                                "' type='text' value = '" + item2.Fieldvalue + "' placeholder='" + item2.FieldLabel +
                                "' " + (item2.IsRequired == true ? "required " : "") +
                                  "' " + (!string.IsNullOrEmpty(item2.HintText) ? " title='" + item2.HintText + "'" : "") +
                                "' " + (item2.EnforceMinMax == true ? "minlength='" + item2.MinLengthValue + "' maxlength='" + item2.MaxLengthValue + "'" : "") +
                                " />");

                            if (item2.CaptureGeolocation == true)
                            {
                                try
                                {
                                    htmlData.Append("<br/><span style='display:none' class ='dynamiclatLong'>" + string.Format("{0:N6}", decimal.Parse(item2.CurrentLat)) + "," + string.Format("{0:N6}", decimal.Parse(item2.CurrentLong)) + "</span>"
                                  + "<button id=" + item2.tblProjectFieldDataMasterId + " class='btn btn-success MapUpdateLocation' type='button'><i class='fa fa-pencil'></i></button>");
                                }
                                catch
                                {                                 
                                    
                                }
                              
                            }


                            htmlData.Append("<br/><label id='" + item2.FieldId + "l' class='col-lg-1' style='color: red'></label>");
                            htmlData.Append("</div>");
                            #endregion
                        }
                        else if (item2.FielddatatypeNames == "Numeric")
                        {
                            #region
                            htmlData.Append("<div class='col-lg-12 drp-div1'>");
                            htmlData.Append("<div class='col-lg-6 col-sm-12 fld-lab2' style='text-align:left'>" + item2.FieldName + requiredicon + " </div>");
                            htmlData.Append("<input class='col-lg-5 col-sm-12 rep-inpt Numeric-Only' id='" + item2.FieldId + "' name = '" + item2.FieldName +
                                "' type='text'  value = '" + item2.Fieldvalue + "'  placeholder='" + item2.FieldLabel +
                                "' " + (item2.IsRequired == true ? "required " : " ") +
                                (item2.EnforceMinMax == true ? "minlength='" + item2.MinLengthValue + "' maxlength='" + item2.MaxLengthValue + "'" : "") +
                                " />");
                            if (item2.CaptureGeolocation == true)
                            {
                                try
                                {
                                    htmlData.Append("<br/><span style='display:none' class ='dynamiclatLong'>" + string.Format("{0:N6}", decimal.Parse(item2.CurrentLat)) + "," + string.Format("{0:N6}", decimal.Parse(item2.CurrentLong)) + "</span>"
                                       + "<button id=" + item2.tblProjectFieldDataMasterId + " class='btn btn-success MapUpdateLocation' type='button'><i class='fa fa-pencil'></i></button>");
                                }
                                catch
                                {

                                }
                                
                            }
                            htmlData.Append("<br/><label id='" + item2.FieldId + "l' class='col-lg-1' style='color: red'></label>");
                            htmlData.Append("</div>");
                            #endregion
                        }
                        else if (item2.FielddatatypeNames == "Long Text")
                        {
                            #region
                            htmlData.Append("<div class='col-lg-12 drp-div1'>");
                            htmlData.Append("<div class='col-lg-6 col-sm-12 fld-lab2' style='text-align:left'>" + item2.FieldName + requiredicon + " </div>");
                            htmlData.Append("<input class='col-lg-5 col-sm-12 rep-inpt' id=''" + item2.FieldId + "' name = '" + item2.FieldName +
                                "' type='text' value = '" + item2.Fieldvalue + "'  placeholder='" + item2.FieldLabel +
                                "' " + (item2.IsRequired == true ? "required " : "") +
                                 "' " + (!string.IsNullOrEmpty(item2.HintText) ? " title='" + item2.HintText + "'" : "") +
                                "' " + (item2.EnforceMinMax == true ? "minlength='" + item2.MinLengthValue + "' maxlength='" + item2.MaxLengthValue + "'" : "") +
                                " />");
                            if (item2.CaptureGeolocation == true)
                            {
                              
                                try
                                {
                                    htmlData.Append("<br/><span style='display:none' class ='dynamiclatLong'>" + string.Format("{0:N6}", decimal.Parse(item2.CurrentLat)) + "," + string.Format("{0:N6}", decimal.Parse(item2.CurrentLong)) + "</span>"
                                   + "<button id=" + item2.tblProjectFieldDataMasterId + " class='btn btn-success MapUpdateLocation' type='button'><i class='fa fa-pencil'></i></button>");
                                }
                                catch
                                {

                                }
                            }
                            htmlData.Append("<br/><label id='" + item2.FieldId + "l' class='col-lg-1' style='color: red'></label>");
                            htmlData.Append("</div>");
                            #endregion
                        }
                        else if (item2.FielddatatypeNames == "Barcode")
                        {
                            #region
                            htmlData.Append("<div class='col-lg-12 drp-div1'>");
                            htmlData.Append("<div class='col-lg-6 col-sm-12 fld-lab2' style='text-align:left'>" + item2.FieldName + requiredicon + " </div>");
                            htmlData.Append("<input class='col-lg-5 col-sm-12 rep-inpt' id='" + item2.FieldId + "' name = '" + item2.FieldName +
                                "' type='text' value = '" + item2.Fieldvalue + "'  placeholder='" + item2.FieldLabel +
                                "' " + (item2.IsRequired == true ? "required " : " ") +
                                (item2.EnforceMinMax == true ? "minlength='" + item2.MinLengthValue + "' maxlength='" + item2.MaxLengthValue + "'" : "") +
                                " />");
                            if (item2.CaptureGeolocation == true)
                            {
                                 try
                                {
                                htmlData.Append("<br/><span style='display:none' class ='dynamiclatLong'>" + string.Format("{0:N6}", decimal.Parse(item2.CurrentLat)) + "," + string.Format("{0:N6}", decimal.Parse(item2.CurrentLong)) + "</span>"
                                    + "<button id=" + item2.tblProjectFieldDataMasterId + " class='btn btn-success MapUpdateLocation' type='button'><i class='fa fa-pencil'></i></button>");
                                }
                                 catch
                                 {

                                 }
                            }
                            htmlData.Append("<br/><label id='" + item2.FieldId + "l' class='col-lg-1' style='color: red'></label>");
                            htmlData.Append("</div>");
                            #endregion
                        }
                        else if (item2.FielddatatypeNames == "Static text")
                        {
                            #region
                            htmlData.Append("<div class='col-lg-12 drp-div1'>");
                            htmlData.Append("<div class='col-lg-6 col-sm-12 fld-lab2' style='text-align:left'>" + item2.FieldName + requiredicon + " </div>");
                            htmlData.Append("<input class='col-lg-5 col-sm-12 rep-inpt' id=''" + item2.FieldId + "' name = '" + item2.FieldName +
                                "' type='text' value = '" + item2.Fieldvalue + "'  placeholder='" + item2.FieldLabel +
                                "' " + (item2.IsRequired == true ? "required " : "") +
                                 "' " + (!string.IsNullOrEmpty(item2.HintText) ? " title='" + item2.HintText + "'" : "") +
                                "' " + (item2.EnforceMinMax == true ? "minlength='" + item2.MinLengthValue + "' maxlength='" + item2.MaxLengthValue + "'" : "") +
                                " />");
                            if (item2.CaptureGeolocation == true)
                            {
                                try
                                {
                                htmlData.Append("<br/><span style='display:none' class ='dynamiclatLong'>" + string.Format("{0:N6}", decimal.Parse(item2.CurrentLat)) + "," + string.Format("{0:N6}", decimal.Parse(item2.CurrentLong)) + "</span>"
                                    + "<button id=" + item2.tblProjectFieldDataMasterId + " class='btn btn-success MapUpdateLocation' type='button'><i class='fa fa-pencil'></i></button>");
                                }
                                catch
                                {

                                }
                            }
                            htmlData.Append("<br/><label id='" + item2.FieldId + "l' class='col-lg-1' style='color: red'></label>");
                            htmlData.Append("</div>");
                            #endregion
                        }
                        else if (item2.FielddatatypeNames == "Date/Time")
                        {
                            #region
                            htmlData.Append("<div class='col-lg-12 drp-div1'>");
                            htmlData.Append("<div class='col-lg-6 col-sm-12 fld-lab2' style='text-align:left'>" + item2.FieldName + requiredicon + " </div>");
                            htmlData.Append("<input class='col-lg-5 col-sm-12 rep-inpt' id='" + item2.FieldId + "' name = '" + item2.FieldName +
                                "' type='Date' value = '" + item2.FieldText + "'  placeholder='" + item2.FieldLabel +
                                "' " + (item2.IsRequired == true ? "required " : "") +
                                 "' " + (!string.IsNullOrEmpty(item2.HintText) ? " title='" + item2.HintText + "'" : "") +
                                " />");
                            if (item2.CaptureGeolocation == true)
                            {
                                 try
                                {
                                htmlData.Append("<br/><span style='display:none' class ='dynamiclatLong'>" + string.Format("{0:N6}", decimal.Parse(item2.CurrentLat)) + "," + string.Format("{0:N6}", decimal.Parse(item2.CurrentLong)) + "</span>"
                                    + "<button id=" + item2.tblProjectFieldDataMasterId + " class='btn btn-success MapUpdateLocation' type='button'><i class='fa fa-pencil'></i></button>");
                                }
                                 catch
                                 {

                                 }
                            }
                            htmlData.Append("<br/><label id='" + item2.FieldId + "l' class='col-lg-1' style='color: red'></label>");
                            htmlData.Append("</div>");
                            #endregion
                        }
                        else if (item2.FielddatatypeNames == "SinglePhoto" || item2.FielddatatypeNames == "MultiPhoto")
                        {
                            #region
                            //gets id for the current field from field data master table.
                            long _fieldId = new ReportDAL().getFieldId(obj1.userId1, obj1.reportFormId, obj1.accountinifiledId, item2.FieldName);
                            //check media avilable
                            long countNoOfMedialFiles = new ReportDAL().CheckMedialAvilablity(_fieldId, 3);

                            _avClass = countNoOfMedialFiles == 0 ? "btn btn-danger btn-xs" : "btn btn-success btn-xs";
                            htmlData.Append("<div class='col-lg-12 drp-div1'>");
                            htmlData.Append("<div class='col-lg-6 col-sm-12 fld-lab2' style='text-align:left'>" + item2.FieldName + requiredicon + " </div>");
                            htmlData.Append("<div  class='col-lg-5 col-sm-12 '>" +
                               " <a href='#' style='margin:3px 0 0 0' data-backdrop='static' data-toggle='modal' id='" + item2.FieldId + "'" +
                            "onclick='reportFieldImages(" + _fieldId + ",\"" + item2.FieldName + "\"," + item2.FieldId + ");' name='reportField' class='" + _avClass + "' role='button' title='Picture'>" +
                            "<i class='fa fa-camera'></i></a>");
                            if (item2.CaptureGeolocation == true)
                            {
                                try
                                {
                                htmlData.Append("<br/><span style='display:none' class ='dynamiclatLong'>" + string.Format("{0:N6}", decimal.Parse(item2.CurrentLat)) + "," + string.Format("{0:N6}", decimal.Parse(item2.CurrentLong)) + "</span>"
                                    + "<button id=" + item2.tblProjectFieldDataMasterId + " class='btn btn-success MapUpdateLocation' type='button'><i class='fa fa-pencil'></i></button>");
                                }
                                catch
                                {

                                }
                            }
                            htmlData.Append("</div></div>");
                            #endregion
                        }
                        else if (item2.FielddatatypeNames == "Location/GPS")
                        {
                            #region
                            var serviceVal = new ReportDAL().getFieldValue(obj1.userId1, obj1.reportFormId, obj1.accountinifiledId, item2.FieldName);
                            htmlData.Append("<div class='col-lg-12 loc-gps'>");
                            htmlData.Append("<div class='col-lg-6 col-sm-12 fld-lab2' style='text-align:left'>" + item2.FieldName + " </div>");
                            htmlData.Append("<div class='col-lg-6 col-sm-12 lat-long' style='text-align:left'>");
                            if (serviceVal != null)
                            {
                                try
                                {
                                    var DData = serviceVal.FieldValue.ToArray();
                                    var DDD = string.Format("{0:N6}", decimal.Parse(item2.CurrentLat)) + "," + string.Format("{0:N6}", decimal.Parse(item2.CurrentLong));
                                    //htmlData.Append(serviceVal.FieldValue);
                                    htmlData.Append(DDD);
                                }
                                catch (Exception ex)
                                {

                                    htmlData.Append("-");
                                }
                              
                            }
                              try
                                {
                                    htmlData.Append("<span style='display:none' class ='dynamiclatLong'>" + string.Format("{0:N6}", decimal.Parse(item2.CurrentLat)) + "," + string.Format("{0:N6}", decimal.Parse(item2.CurrentLong)) + "</span>"
                                    + "<button id=" + item2.tblProjectFieldDataMasterId + " class='btn btn-success MapUpdateLocation' type='button'><i class='fa fa-pencil'></i></button>");
                                }
                              catch 
                              {                                
                              }
                            htmlData.Append("</div>");
                            htmlData.Append("</div>");
                            #endregion
                        }
                        else if (item2.FielddatatypeNames == "Yes/No")
                        {
                            #region
                            var serviceVal = new ReportDAL().getFieldValue(obj1.userId1, obj1.reportFormId, obj1.accountinifiledId, item2.FieldName);
                            htmlData.Append("<div class='col-lg-12'>");
                            htmlData.Append("<div class='col-lg-6 col-sm-6 col-xs-6 fld-lab3' style='text-align:left'>" + item2.FieldName + " </div>");
                            htmlData.Append("<div class='col-lg-6 col-sm-6 col-xs-6  ys-lab' style='text-align:left'>");
                            if (serviceVal != null && serviceVal.FieldValue == "true")
                            {
                                htmlData.Append("Yes");
                            }
                            else
                            {
                                htmlData.Append("No");
                            }

                            if (item2.CaptureGeolocation == true)
                            {
                                try
                                {
                                    htmlData.Append("<br/><span style='display:none' class ='dynamiclatLong'>" + string.Format("{0:N6}", decimal.Parse(item2.CurrentLat)) + "," + string.Format("{0:N6}", decimal.Parse(item2.CurrentLong)) + "</span>"
                                        + "<button id=" + item2.tblProjectFieldDataMasterId + " class='btn btn-success MapUpdateLocation' type='button'><i class='fa fa-pencil'></i></button>");
                                }
                                catch { }
                            }
                            htmlData.Append("</div>");
                            htmlData.Append("</div>");
                            #endregion
                        }
                        else if (item2.FielddatatypeNames == "List")
                        {
                            #region
                            if (item2.FieldName == "SERVICES")
                            {

                                string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);
                                //   _webUrl = "https://localhost:6069/";
                                #region get service data

                                var serviceVal = new ReportDAL().getFieldValue(obj1.userId1, obj1.reportFormId, obj1.accountinifiledId, "SERVICES");
                                string _ser = serviceVal == null ? "" : serviceVal.FieldValue;
                                var list = item.FieldList.Where(o => o.fieldId == item2.FieldId).Select(o => o.fieldInitialDataList).FirstOrDefault();
                                string[] serviceList = (!string.IsNullOrEmpty(_ser) ? _ser.Split('|').ToArray() : null);
                                string serviceBase = "";
                                foreach (var item3 in list)
                                {

                                    string ImageAvilable = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 3 && o.isServiceData == true && o.tblService_Id == item3.initialFieldId).Select(o => o.strFilePath).FirstOrDefault());


                                    ImageAvilable = string.IsNullOrEmpty(ImageAvilable) ? "btn btn-danger btn-xs" : "btn btn-success btn-xs";


                                    string strVideoFilePath = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 2 && o.isServiceData == true && o.tblService_Id == item3.initialFieldId).Select(o => o.strFilePath).FirstOrDefault());
                                    string strAudioFilePath = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 1 && o.isServiceData == true && o.tblService_Id == item3.initialFieldId).Select(o => o.strFilePath).FirstOrDefault());

                                    string Video_AttachmentID = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 2
                                      && o.isServiceData == true && o.tblService_Id == item3.initialFieldId).Select(o => o.ID).FirstOrDefault());

                                    string Audio_AttachmentID = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 1
                                          && o.isServiceData == true && o.tblService_Id == item3.initialFieldId).Select(o => o.ID).FirstOrDefault());


                                    if (!string.IsNullOrEmpty(strVideoFilePath))
                                    {
                                        //strVideoFilePath = "" + strVideoFilePath.Replace("..\\..\\", _webUrl);
                                        if (strVideoFilePath.Contains("..\\..\\"))
                                        {
                                            strVideoFilePath = "" + strVideoFilePath.Replace("..\\..\\", _webUrl);
                                        }

                                        if (strVideoFilePath.Contains("~"))
                                        {
                                            strVideoFilePath = "" + strVideoFilePath.Replace("~\\", _webUrl);
                                        }

                                        strVideoFilePath = "" + strVideoFilePath.Replace('\\', '/');
                                        strVideoFilePath = strVideoFilePath.Trim();
                                    }

                                    if (!string.IsNullOrEmpty(strAudioFilePath))
                                    {
                                        //strAudioFilePath = "" + strAudioFilePath.Replace("..\\..\\", _webUrl);

                                        if (strAudioFilePath.Contains("..\\..\\"))
                                        {
                                            strAudioFilePath = "" + strAudioFilePath.Replace("..\\..\\", _webUrl);
                                        }

                                        if (strAudioFilePath.Contains("~"))
                                        {
                                            strAudioFilePath = "" + strAudioFilePath.Replace("~\\", _webUrl);
                                        }
                                        strAudioFilePath = "" + strAudioFilePath.Replace('\\', '/');
                                        strAudioFilePath = strAudioFilePath.Trim();
                                    }

                                    strVideoFilePath = string.IsNullOrEmpty(strVideoFilePath) ? null : strVideoFilePath;
                                    strAudioFilePath = string.IsNullOrEmpty(strAudioFilePath) ? null : strAudioFilePath;




                                    var result1 = serviceList == null ? null : (serviceList.Where(o => o == item3.initialFieldId.ToString()).FirstOrDefault());
                                    serviceBase = serviceBase + "<select class='col-lg-6 col-sm-5 col-xs-5 sel-rep' style='margin:9px 0px 0 0px; text-align:left'>";
                                    serviceBase = serviceBase + "<option>" + item3.initialFieldValue + "</option>";
                                    serviceBase = serviceBase + "</select>";
                                    //serviceBase = serviceBase + "<div class='col-lg-3 col-sm-3 col-xs-3' style='margin-left:10%'> <input style='margin:14px 0px 0px -6px;pointer-events: none;opacity: 0.4;'  type='checkbox' name = 'S_" + item3.initialFieldValue + "' id='" + item3.initialFieldId + "' " + (result1 == null ? "" : "checked") + "> </div>";

                                    // Checked box editable requirement for clinet side
                                    serviceBase = serviceBase + "<div class='col-lg-3 col-sm-3 col-xs-3' style='margin-left:10%'> <input style='margin:14px 0px 0px -6px;opacity: 0.4;'  type='checkbox' name = 'S_" + item3.initialFieldValue + "' id='" + item3.initialFieldId + "' " + (result1 == null ? "" : "checked") + "> </div>";


                                    if (result1 != null)
                                    {
                                        serviceBase = serviceBase + "<div class='col-lg-3 col-sm-4 col-xs-4 rep-imgbtn'style='margin-left:-13%;'><a href='#' name='service' id='IMS_" + item3.initialFieldId + "' onclick='reportServiceFieldImages(" + serviceVal.ID + "," + item3.initialFieldId + ",\"" + item3.initialFieldValue + "\"," + item2.FieldId + ",false)' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='" + ImageAvilable + "' role='button' title='Picture'><i class='fa fa-camera'></i></a>";

                                        if (!string.IsNullOrEmpty(strAudioFilePath))
                                        {
                                            serviceBase = serviceBase + "<a href='#' id=" + Audio_AttachmentID + " enable onclick='showAudio(\"" + strAudioFilePath + "\",\"" + item3.initialFieldValue + "\",\"" + Audio_AttachmentID + "\");' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='btn btn-success btn-xs rep-audbtn' role='button' title='Audio'><i class='fa fa-file-audio-o' aria-hidden='true'></i></a>";
                                        }
                                        else
                                        {
                                            serviceBase = serviceBase + "<a href='#' enable onclick='return false;' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='btn btn-danger btn-xs rep-audbtn' role='button' title='Audio'><i class='fa fa-file-audio-o' aria-hidden='true'></i></a>";
                                        }

                                        if (!string.IsNullOrEmpty(strVideoFilePath))
                                        {
                                            serviceBase = serviceBase + "<a href='#' id=" + Video_AttachmentID + " onclick='showVideo(\"" + strVideoFilePath + "\",\"" + item3.initialFieldValue + "\",\"" + Video_AttachmentID + "\");' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='btn btn-success btn-xs rep-vidbtn' role='button' title='Video'><i class='fa fa-video-camera' aria-hidden='true'></i></a></div>";
                                        }
                                        else
                                        {
                                            serviceBase = serviceBase + "<a href='#' onclick='return false;' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='btn btn-danger btn-xs rep-vidbtn' role='button' title='Video'><i class='fa fa-video-camera' aria-hidden='true'></i></a></div>";
                                        }

                                    }
                                    else
                                    {
                                        serviceBase = serviceBase + "<span class='col-lg-3 col-sm-4 col-xs-4 rep-imgbtn'style='margin-left:-13%'><a href='#' onclick='return false;' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='btn btn-danger btn-xs' role='button' title='Picture'><i class='fa fa-camera'></i></a>";
                                        serviceBase = serviceBase + "<a href='#' enable onclick='return false;' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='btn btn-danger btn-xs rep-audbtn' role='button' title='Audio'><i class='fa fa-file-audio-o' aria-hidden='true'></i></a>";
                                        serviceBase = serviceBase + "<a href='#' onclick='return false;' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='btn btn-danger btn-xs rep-vidbtn' role='button' title='Video'><i class='fa fa-video-camera' aria-hidden='true'></i></a></span>";

                                    }
                                }

                                #endregion
                                htmlData.Append("<div class='col-lg-12 srv-hd'>");
                                htmlData.Append("<span class='col-lg-6 col-sm-5 col-xs-5 rep-servcs' style='text-align:left'><b>Services</b></span>");
                                htmlData.Append("<span class='col-lg-2 col-sm-3 col-xs-3 rep-done' style=''><b>Done</b></span>");
                                htmlData.Append("<span class='col-lg-4 col-sm-4 col-xs-4 rep-media'style=''><b>Media</b></span>");
                                htmlData.Append(serviceBase);
                                htmlData.Append("</div>");
                            }
                            else if (item2.FieldName == "ADDITIONAL SERVICES")
                            {
                                #region get service data
                                string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);
                                //   _webUrl = "https://localhost:6069/";
                                var serviceVal = new ReportDAL().getFieldValue(obj1.userId1, obj1.reportFormId, obj1.accountinifiledId, "ADDITIONAL SERVICES");
                                var list = item.FieldList.Where(o => o.fieldId == item2.FieldId).Select(o => o.fieldInitialDataList).FirstOrDefault();
                                string[] serviceList = (!string.IsNullOrEmpty(serviceVal.FieldValue) ? serviceVal.FieldValue.Split('|').ToArray() : null);

                                string serviceBase = "";
                                foreach (var item3 in list)
                                {
                                    string ImageAvilable = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 3 && o.isServiceData == true && o.tblService_Id == item3.initialFieldId).Select(o => o.strFilePath).FirstOrDefault());


                                    ImageAvilable = string.IsNullOrEmpty(ImageAvilable) ? "btn btn-danger btn-xs" : "btn btn-success btn-xs";

                                    string strVideoFilePath = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 2
                                        && o.isServiceData == true && o.tblService_Id == item3.initialFieldId).Select(o => o.strFilePath).FirstOrDefault());
                                    string strAudioFilePath = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 1
                                        && o.isServiceData == true && o.tblService_Id == item3.initialFieldId).Select(o => o.strFilePath).FirstOrDefault());

                                    string Video_AttachmentID = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 2
                                          && o.isServiceData == true && o.tblService_Id == item3.initialFieldId).Select(o => o.ID).FirstOrDefault());

                                    string Audio_AttachmentID = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 1
                                          && o.isServiceData == true && o.tblService_Id == item3.initialFieldId).Select(o => o.ID).FirstOrDefault());

                                    if (!string.IsNullOrEmpty(strVideoFilePath))
                                    {
                                        //strVideoFilePath = "" + strVideoFilePath.Replace("..\\..\\", _webUrl);
                                        if (strVideoFilePath.Contains("..\\..\\"))
                                        {
                                            strVideoFilePath = "" + strVideoFilePath.Replace("..\\..\\", _webUrl);
                                        }

                                        if (strVideoFilePath.Contains("~"))
                                        {
                                            strVideoFilePath = "" + strVideoFilePath.Replace("~\\", _webUrl);
                                        }
                                        strVideoFilePath = "" + strVideoFilePath.Replace('\\', '/');
                                        strVideoFilePath = strVideoFilePath.Trim();
                                    }

                                    if (!string.IsNullOrEmpty(strAudioFilePath))
                                    {
                                        // strAudioFilePath = "" + strAudioFilePath.Replace("..\\..\\", _webUrl);
                                        if (strAudioFilePath.Contains("..\\..\\"))
                                        {
                                            strAudioFilePath = "" + strAudioFilePath.Replace("..\\..\\", _webUrl);
                                        }

                                        if (strAudioFilePath.Contains("~"))
                                        {
                                            strAudioFilePath = "" + strAudioFilePath.Replace("~\\", _webUrl);
                                        }

                                        strAudioFilePath = "" + strAudioFilePath.Replace('\\', '/');
                                        strAudioFilePath = strAudioFilePath.Trim();
                                    }

                                    strVideoFilePath = string.IsNullOrEmpty(strVideoFilePath) ? null : strVideoFilePath;
                                    strAudioFilePath = string.IsNullOrEmpty(strAudioFilePath) ? null : strAudioFilePath;


                                    var result1 = serviceList == null ? null : (serviceList.Where(o => o == item3.initialFieldId.ToString()).FirstOrDefault());
                                    serviceBase = serviceBase + "<select class='col-lg-6 col-sm-5 col-xs-5 sel-rep' style='margin:9px 0px 0 0px; text-align:left'>";
                                    serviceBase = serviceBase + "<option>" + item3.initialFieldValue + "</option>";
                                    serviceBase = serviceBase + "</select>";
                                    //serviceBase = serviceBase + "<div class='col-lg-3 col-sm-3 col-xs-3' style='margin-left:10%'> <input style='margin:14px 0px 0px -6px;pointer-events: none;opacity: 0.4;'  type='checkbox' name = 'A_" + item3.initialFieldValue + "' id='" + item3.initialFieldId + "' " + (result1 == null ? "" : "checked") + "> </div>";

                                    // Checked box editable requirement for clinet side
                                    serviceBase = serviceBase + "<div class='col-lg-3 col-sm-3 col-xs-3' style='margin-left:10%'> <input style='margin:14px 0px 0px -6px;opacity: 0.4;'  type='checkbox' name = 'A_" + item3.initialFieldValue + "' id='" + item3.initialFieldId + "' " + (result1 == null ? "" : "checked") + "> </div>";

                                    if (result1 != null)
                                    {
                                        serviceBase = serviceBase + "<div class='col-lg-3 col-sm-4 col-xs-4 rep-imgbtn2'style='margin-left:-13%;' ><a href='#' id='IMS_" + item3.initialFieldId + "' onclick='reportServiceFieldImages(" + serviceVal.ID + "," + item3.initialFieldId + ",\"" + item3.initialFieldValue + "\"," + item2.FieldId + ", true)' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='" + ImageAvilable + "' name='additionalservice'  role='button' title='Picture'><i class='fa fa-camera'></i></a>";
                                        //serviceBase = serviceBase + "<a href='#' onclick='showAudio(\"" + strAudioFilePath + "\",\"" + item3.initialFieldValue + "\");' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='btn btn-danger btn-xs rep-audbtn' role='button' title='Picture'><i class='fa fa-file-audio-o' aria-hidden='true'></i></a>";
                                        //serviceBase = serviceBase + "<a href='#' onclick='showVideo(\"" + strVideoFilePath + "\",\"" + item3.initialFieldValue + "\");' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='btn btn-danger btn-xs rep-vidbtn' role='button' title='Picture'><i class='fa fa-video-camera' aria-hidden='true'></i></a></div>";
                                        if (!string.IsNullOrEmpty(strAudioFilePath))
                                        {
                                            serviceBase = serviceBase + "<a href='#' id=" + Audio_AttachmentID + " enable onclick='showAudio(\"" + strAudioFilePath + "\",\"" + item3.initialFieldValue + "\",\"" + Audio_AttachmentID + "\");' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='btn btn-success btn-xs rep-audbtn' role='button' title='Audio'><i class='fa fa-file-audio-o' aria-hidden='true'></i></a>";
                                        }
                                        else
                                        {
                                            serviceBase = serviceBase + "<a href='#' enable onclick='return false;' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='btn btn-danger btn-xs rep-audbtn' role='button' title='Audio'><i class='fa fa-file-audio-o' aria-hidden='true'></i></a>";
                                        }

                                        if (!string.IsNullOrEmpty(strVideoFilePath))
                                        {
                                            serviceBase = serviceBase + "<a href='#' id=" + Video_AttachmentID + " onclick='showVideo(\"" + strVideoFilePath + "\",\"" + item3.initialFieldValue + "\",\"" + Video_AttachmentID + "\");' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='btn btn-success btn-xs rep-vidbtn' role='button' title='Video'><i class='fa fa-video-camera' aria-hidden='true'></i></a></div>";
                                        }
                                        else
                                        {
                                            serviceBase = serviceBase + "<a href='#' onclick='return false;' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='btn btn-danger btn-xs rep-vidbtn' role='button' title='Video'><i class='fa fa-video-camera' aria-hidden='true'></i></a></div>";
                                        }
                                    }
                                    else
                                    {
                                        serviceBase = serviceBase + "<div class='col-lg-3 col-sm-4 col-xs-4 rep-imgbtn2'style='margin-left:-13%'><a href='#' onclick='return false;' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='btn btn-danger btn-xs' role='button' title='Picture'><i class='fa fa-camera'></i></a>";
                                        serviceBase = serviceBase + "<a href='#' onclick='return false;' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='btn btn-danger btn-xs rep-audbtn' role='button' title='Audio'><i class='fa fa-file-audio-o' aria-hidden='true'></i></a>";
                                        serviceBase = serviceBase + "<a href='#' onclick='return false;' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='btn btn-danger btn-xs rep-vidbtn' role='button' title='Video'><i class='fa fa-video-camera' aria-hidden='true'></i></a></div>";
                                    }
                                }
                                #endregion
                                htmlData.Append("<div class='col-lg-12 addserv-hd'>");
                                htmlData.Append("<span class='col-lg-6 col-sm-5 col-xs-5 rep-addserv' style='text-align:left'><b>Additional Services</b></span>");
                                htmlData.Append("<span class='col-lg-2 col-sm-3 col-xs-3 rep-done1' style=''><b>Done</b></span>");
                                htmlData.Append("<span class='col-lg-4 col-sm-4 col-xs-4 rep-media1'style=''><b>Media</b></span>");
                                htmlData.Append(serviceBase);
                                htmlData.Append("</div>");

                            }
                            else
                            {
                                #region
                                htmlData.Append("<div class='col-lg-12 drp-div'>");
                                htmlData.Append("<div class='col-lg-6 col-sm-12 fld-lab' style='text-align:left'>" + item2.FieldName + " </div>");
                                //<select class="col-lg-5 pincode"  id="' + item.FieldId + '" name = "' + item.FieldName + '" type="Date"  value = " " onKeyUp="return CheckIt(' + item.FieldId + ',event);" placeholder="' + item.FieldLabel + '"/>
                                htmlData.Append("<select class='col-lg-5 col-sm-12 rep-drpdwn' id='" + item2.FieldId + "' name = '" + item2.FieldName + "'>");


                                foreach (var details in FieldData)
                                {
                                    var _initialList = details.FieldList.Where(o1 => o1.fieldId == item2.FieldId).Select(o1 => o1.fieldInitialDataList).FirstOrDefault();
                                    if (_initialList != null)
                                    {
                                        foreach (var it in _initialList)
                                        {
                                            if (it.initialFieldValue == item2.Fieldvalue)
                                            {
                                                htmlData.Append("<option value='" + it.initialFieldValue + "' selected='selected'>" + it.initialFieldValue + "</option>");
                                            }
                                            else
                                            {
                                                htmlData.Append("<option value='" + it.initialFieldValue + "'>" + it.initialFieldValue + "</option>");
                                            }
                                        }
                                        break;
                                    }
                                }

                                htmlData.Append("</select>");
                                htmlData.Append("<br/><label id='" + item2.FieldId + "l' class='col-lg-1' style='color: red'></label>");
                                htmlData.Append("</div>");
                                #endregion
                            }
                            #endregion
                        }
                        #endregion
                    }


                    htmlData.Append(" </div>");// closer section div

                    i++;

                    #endregion

                    if (currentSectionNumber == SectionCount || currentSectionNumber % 2 == 0)
                    {
                        htmlData.Append("</div>");
                    }
                    currentSectionNumber++;
                }

                htmlData.Append("<div>");
                htmlData.Append("<input type='hidden' value='" + obj1.userId1 + "' id='hdnUserID' />");
                htmlData.Append("<input type='hidden' value='" + obj1.accountinifiledId + "' id='hdnAccountID' />");
                htmlData.Append("<input type='hidden' value='" + obj1.reportFormId + "' id='hdnFormID' />");
                htmlData.Append("</div>");

                string _result = htmlData.ToString();
                var jsonResult = Json(new { status = true, htmlData = _result, Details1 = Details }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;

            }
            catch (Exception ex)
            {
                var jsonResult = Json(new { status = false, errorData = ex.Message }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            #endregion
        }


        /// <summary>
        /// return html tag of image for the custom image fields.
        /// </summary>
        /// <param name="initialFieldId">tblFieldDataMaster id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult getHtmlImagelink(long initialFieldId)
        {
            StringBuilder htmlData = new StringBuilder();

            var details = new ReportDAL().getFieldImageData(initialFieldId);
            string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);
            int i = 1;
            foreach (var item in details)
            {
                string strFilePath = item.strFilePath;
                if (!string.IsNullOrEmpty(strFilePath))
                {
                    if (strFilePath.Contains("..\\..\\"))
                    {
                        strFilePath = "" + strFilePath.Replace("..\\..\\", _webUrl);
                    }

                    if (strFilePath.Contains("~"))
                    {
                        strFilePath = "" + strFilePath.Replace("~\\", _webUrl);
                    }

                    strFilePath = "" + strFilePath.Replace('\\', '/');
                    strFilePath = strFilePath.Trim();
                }

                //<img id="278" src="/client/cms/MobileUploadData\2\2\231352\1608\HOUSE PHOTO\2016-07-06-13-40-51.jpeg">
                //htmlData.Append("<img id='" + item.ID + "' src=\"" + strFilePath + "\">");

                if (i == 1)
                {
                    htmlData.Append("<img class='imgRotateFlip'  style ='display: inline;' id='" + item.ID + "' src=\"" + strFilePath + "\" data-zoom-image=\"" + strFilePath + "\"   >");
                }
                else
                {
                    htmlData.Append("<img class ='imgRotateFlip'  style='display: none; ' id='" + item.ID + "' src=\"" + strFilePath + "\"  data-zoom-image=\"" + strFilePath + "\"   >");
                }
                i++;
            }

            //string scriptForZoom = "<script type='text/javascript'> ";
            //foreach (var item in details)
            //{
            //    scriptForZoom = scriptForZoom + "$('#" + item.ID + "').imgViewer();";
            //}
            //scriptForZoom = scriptForZoom + "  </script>";

            //htmlData.Append(scriptForZoom);

            string _result = htmlData.ToString();
            var jsonResult = Json(new { htmlData = _result }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        [HttpGet]
        public ActionResult GetAllHtmlImagelink(long FieldDataMasterId)
        {
            StringBuilder htmlData = new StringBuilder();

            var details = new ReportDAL().getFieldImageData(FieldDataMasterId);
            string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);
            //int i = 1;

            List<ImageSliderList> objList = new List<Model.BO.ImageSliderList>();
            foreach (var item in details)
            {
                string strFilePath = item.strFilePath;
                if (!string.IsNullOrEmpty(strFilePath))
                {
                    if (strFilePath.Contains("..\\..\\"))
                    {
                        strFilePath = "" + strFilePath.Replace("..\\..\\", _webUrl);
                    }

                    if (strFilePath.Contains("~"))
                    {
                        strFilePath = "" + strFilePath.Replace("~\\", _webUrl);
                    }

                    strFilePath = "" + strFilePath.Replace('\\', '/');
                    strFilePath = strFilePath.Trim();
                }
                strFilePath = strFilePath.Replace(" ", "%20");

                objList.Add(new Model.BO.ImageSliderList { ImageId = item.ID, ImageName = "", ImagePath = strFilePath });


                //if (i == 1)
                //{
                //    htmlData.Append("<img class='imgRotateFlip'  style ='display: inline;' id='" + item.ID + "' src=\"" + strFilePath + "\" data-zoom-image=\"" + strFilePath + "\"   >");
                //}
                //else
                //{
                //    htmlData.Append("<img class ='imgRotateFlip'  style='display: none; ' id='" + item.ID + "' src=\"" + strFilePath + "\"  data-zoom-image=\"" + strFilePath + "\"   >");
                //}
                //i++;


            }



            string _result = htmlData.ToString();
            var jsonResult = Json(new { ImageList = objList }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        [HttpGet]
        public ActionResult getHtmlImagelinkForServices(long initialFieldId, long serviceId)
        {
            StringBuilder htmlData = new StringBuilder();

            var details = new ReportDAL().getServiceFieldImageData(initialFieldId, serviceId);
            string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);
            //    _webUrl = "https://localhost:6069/";
            int i = 1;

            //AddedBy AniketJ on 25-Oct-2016
            List<string> pictureList = new ReportDAL().GetPictureListData(serviceId);

            foreach (var item in details)
            {
                string strFilePath = item.strFilePath;
                if (!string.IsNullOrEmpty(strFilePath))
                {
                    //strFilePath = "" + strFilePath.Replace("..\\..\\", _webUrl);
                    if (strFilePath.Contains("..\\..\\"))
                    {
                        strFilePath = "" + strFilePath.Replace("..\\..\\", _webUrl);
                    }

                    if (strFilePath.Contains("~"))
                    {
                        strFilePath = "" + strFilePath.Replace("~\\", _webUrl);
                    }

                    strFilePath = "" + strFilePath.Replace('\\', '/');
                    strFilePath = strFilePath.Trim();
                }

                //<img id="278" src="/client/cms/MobileUploadData\2\2\231352\1608\HOUSE PHOTO\2016-07-06-13-40-51.jpeg">
                if (i == 1)
                {
                    htmlData.Append("<img class='imgRotateFlip' style='display: inline;' id='" + item.ID + "' src=\"" + strFilePath + "\" data-zoom-image=\"" + strFilePath + "\"   >");
                }
                else
                {
                    htmlData.Append("<img class ='imgRotateFlip' style='display: none; ' id='" + item.ID + "' src=\"" + strFilePath + "\" data-zoom-image=\"" + strFilePath + "\"   >");
                }
                i++;
            }
            string _result = htmlData.ToString();
            var jsonResult = Json(new { htmlData = _result, PictureList = pictureList }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        [HttpGet]
        public ActionResult GetHtmlImageForServices(long reportFieldId, long serviceId)
        {
            StringBuilder htmlData = new StringBuilder();

            var details = new ReportDAL().getServiceFieldImageData(reportFieldId, serviceId);
            string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);
            //    _webUrl = "https://localhost:6069/";
            int i = 1;

            List<ImageSliderList> objList = new List<Model.BO.ImageSliderList>();

            //AddedBy AniketJ on 25-Oct-2016
            List<string> pictureList = new ReportDAL().GetPictureListData(serviceId);

            foreach (var item in details)
            {
                string strFilePath = item.strFilePath;
                if (!string.IsNullOrEmpty(strFilePath))
                {
                    //strFilePath = "" + strFilePath.Replace("..\\..\\", _webUrl);
                    if (strFilePath.Contains("..\\..\\"))
                    {
                        strFilePath = "" + strFilePath.Replace("..\\..\\", _webUrl);
                    }

                    if (strFilePath.Contains("~"))
                    {
                        strFilePath = "" + strFilePath.Replace("~\\", _webUrl);
                    }

                    strFilePath = "" + strFilePath.Replace('\\', '/');
                    strFilePath = strFilePath.Trim();
                }

                strFilePath = strFilePath.Replace(" ", "%20");
                objList.Add(new Model.BO.ImageSliderList { ImageId = item.ID, ImageName = item.ImageName, ImagePath = strFilePath });

                //<img id="278" src="/client/cms/MobileUploadData\2\2\231352\1608\HOUSE PHOTO\2016-07-06-13-40-51.jpeg">
                //if (i == 1)
                //{
                //    htmlData.Append("<img class='imgRotateFlip' style='display: inline;' id='" + item.ID + "' src=\"" + strFilePath + "\" data-zoom-image=\"" + strFilePath + "\"   >");
                //}
                //else
                //{
                //    htmlData.Append("<img class ='imgRotateFlip' style='display: none; ' id='" + item.ID + "' src=\"" + strFilePath + "\" data-zoom-image=\"" + strFilePath + "\"   >");
                //}
                i++;
            }
            string _result = htmlData.ToString();

            var jsonResult = Json(new { ImageList = objList, PictureList = pictureList }, JsonRequestBehavior.AllowGet);
            // var jsonResult = Json(new { htmlData = _result, PictureList = pictureList }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        int count = 1;
        string sectionName;
        long SectionId;



        public List<FieldNames> DispalyRecordsNew(long sectionIds, long userId, int projectId, long uploadId, long reportFormId, List<MobileSectionBO> FieldData)
        {
            //CompassEntities cmp = new CompassEntities();

            // newReportEditBO obj1 = (newReportEditBO)Session["objnewReportEditBO"];
            newReportEditBO obj1 = new newReportEditBO
            {
                accountinifiledId = uploadId,
                projectId1 = projectId,
                reportFormId = reportFormId,
                userId1 = userId
            };


            TblProjectFieldData objTblProjectFieldData;
            List<tblProjectFieldDataMaster> objListtblProjectFieldDataMaster;

            new ReportDAL().GetReportEditData(obj1.accountinifiledId, obj1.reportFormId, obj1.userId1, out objTblProjectFieldData, out objListtblProjectFieldDataMaster);


            long accountinifiledId = uploadId;
            List<FieldNames> Field = new List<FieldNames>();

            // var FieldData = Session["Field"];
            foreach (var item in (dynamic)(FieldData))
            {
                sectionName = item.sectionName;
                SectionId = item.sectionId;
                //Session["DataList"] = item.FieldList;
                IEnumerable<dynamic> fd = item.FieldList;
                var list = fd.Where(o => o.sectionId == sectionIds).ToList();
                foreach (var item1 in list)
                {
                    string name = item1.fieldName;

                    var detailsdata = objListtblProjectFieldDataMaster.Where(o => o.InitialFieldName == name).FirstOrDefault();
                    var itemtext = item1.fieldInitialDataList;


                    Field.Add(new FieldNames()
                    {
                        FieldId = item1.fieldId,
                        FormSectionFieldID = item1.sectionId,
                        FieldName = item1.fieldName,
                        FielddatatypeNames = item1.fieldDataTypeName,
                        FieldLabel = item1.fieldLabel,
                        DefaultValue = Convert.ToString(item1.defaultValue),
                        DecimalPositions = item1.decimalPositions,
                        HintText = item1.hintText,
                        EnforceMinMax = item1.enforceMinMax,
                        MaxLengthValue = Convert.ToString(item1.maxLengthValue),
                        MinLengthValue = Convert.ToString(item1.minLengthValue),
                        IsRequired = item1.isRequired,
                        IsEnabled = item1.isEnabled,
                        CaptureGeolocation = item1.captureGeolocation,
                        CaptureTimestamp = item1.captureTimestamp,
                        HasAlert = item1.hasAlert,
                        Secure = item1.secure,
                        allowprii = item1.allowprii,
                        KeyboardType = item1.keyboardType,
                        FormatMask = Convert.ToString(item1.formatMask),
                        DisplayMask = Convert.ToString(item1.displayMask),
                        CaptureMode = Convert.ToString(item1.captureMode),
                        PhotoFile = item1.photoFile,
                        AllowAnnotation = item1.allowAnnotation,
                        PhotoQuality = Convert.ToString(item1.photoQuality),
                        MaximumHeight = Convert.ToString(item1.maximumHeight),
                        MaximumWidth = Convert.ToString(item1.maximumWidth),
                        ExcludeonSync = item1.excludeonSync,
                        PhotoLibraryEnabled = item1.photoLibraryEnabled,
                        CameraEnabled = item1.cameraEnabled,
                        GPSTagging = item1.gpsTagging,
                        AllowNotApplicable = item1.allowNotApplicable,
                        RatingMax = item1.ratingMax,
                        RatingLabels = item1.ratingLabels,
                        ShowMultiselect = item1.showMultiselect,
                        ShowRatingLabels = item1.showRatingLabels,
                        AllowMultiSelection = item1.allowMultiSelection,
                        NumberOfColumnsForPhones = Convert.ToString(item1.numberOfColumnsForPhones),
                        NumberOfColumnsForTablets = Convert.ToString(item1.numberOfColumnsForTablets),
                        HasValues = item1.hasValues,
                        FieldFilterkey = Convert.ToString(item1.fieldFilterkey),
                        AcknowlegementText = item1.acknowlegementText,
                        AcknowlegementButtonText = item1.acknowlegementButtonText,
                        AcknowlegementClickedButtonText = item1.acknowlegementClickedButtonText,
                        HasFile = item1.hasFile,
                        VideoQuality = Convert.ToString(item1.videoQuality),
                        MaxrecordabletimeInSeconds = Convert.ToString(item1.maxrecordabletimeInSeconds),
                        AudioQuality = Convert.ToString(item1.audioQuality),
                        initialId = detailsdata.InitialFieldId,
                        FieldText = detailsdata.InitialFieldName,
                        Fieldvalue = detailsdata.FieldValue,
                        CurrentLat = detailsdata.FieldLatitude,
                        CurrentLong = detailsdata.FieldLongitude,
                        tblProjectFieldDataMasterId = detailsdata.ID,
                        Count = count,
                        SectionId = Convert.ToInt64(SectionId),
                        SectionName = sectionName

                    });

                }
                count++;
            }
            //List<FieldNames> Field = new List<FieldNames>();
            // Field = new ReportDAL().FieldDetails(Id);
            //return Json(new { Field = Field, detals = detals }, JsonRequestBehavior.AllowGet);
            return Field;
        }

        public ActionResult RemoveImage(int imagesID)
        {
            CompassEntities cmp = new CompassEntities();
            var data = cmp.TblProjectFieldDataAttachments.Where(o => o.ID == imagesID).FirstOrDefault();
            data.Active = 0;
            data.ModifiedBy = 1;
            data.ModifiedOn = new CommonFunctions().ServerDate();
            cmp.SaveChanges();
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetImage(long initialId)
        {
            List<ProjectFildDataTypeMaster> FieldDataAttach = new List<ProjectFildDataTypeMaster>();
            var list = new ReportDAL().ProjectFildDataTypeMasters(initialId);
            foreach (var detaisl in list)
            {
                FieldDataAttach.Add(new ProjectFildDataTypeMaster()
                {
                    ID = detaisl.ID,
                    InitialFieldId = detaisl.InitialFieldId,
                    InitialFieldIdAttach = detaisl.InitialFieldIdAttach,
                    ProjectAttachmentId = detaisl.ProjectAttachmentId,
                    strFilePath = detaisl.strFilePath,
                    IsAudioVideoImage = detaisl.IsAudioVideoImage,
                    isservicesData = detaisl.isservicesData,
                    tblservices_Id = detaisl.tblservices_Id,
                    tblservicePic_Id = detaisl.tblservicePic_Id,
                    Active = detaisl.Active
                });
            }
            return Json(new { FieldDataAttach = FieldDataAttach }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFieldImages(long initialId, string FieldName)
        {
            List<ProjectFildDataTypeMaster> FieldDataAttach = new List<ProjectFildDataTypeMaster>();
            //var projectFiledDataId = new ReportDAL().projectFiledDataId(initialId);
            //foreach (var item in projectFiledDataId)
            //{
            var list = new ReportDAL().ProjectFildDataTypeMaster(initialId, FieldName);
            foreach (var detaisl in list)
            {
                FieldDataAttach.Add(new ProjectFildDataTypeMaster()
                {
                    ID = detaisl.ID,
                    InitialFieldId = detaisl.InitialFieldId,
                    InitialFieldIdAttach = detaisl.InitialFieldIdAttach,
                    ProjectAttachmentId = detaisl.ProjectAttachmentId,
                    strFilePath = detaisl.strFilePath,
                    IsAudioVideoImage = detaisl.IsAudioVideoImage,
                    isservicesData = detaisl.isservicesData,
                    tblservices_Id = detaisl.tblservices_Id,
                    tblservicePic_Id = detaisl.tblservicePic_Id,
                    Active = detaisl.Active

                });
            }
            //FieldDataAttach.Add(new ReportDAL().ProjectFildDataTypeMaster(item.ID, FieldName));
            // }
            return Json(new { FieldDataAttach = FieldDataAttach }, JsonRequestBehavior.AllowGet);
        }

        private double ConvertToUnixTimestamp(DateTime? date)
        {
            double returnvalue = 0;
            if (date != null)
            {
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);

                TimeSpan diff = Convert.ToDateTime(date).ToUniversalTime() - origin;
                return Math.Floor(diff.TotalMilliseconds);
            }
            else return returnvalue;
        }

        public ActionResult GetFieldProperty(int FieldId)
        {
            List<FieldProperty> Fieldproperty = new List<FieldProperty>();
            Fieldproperty = new ReportDAL().GetAllFieldproperty(FieldId);
            return Json(new { Fieldproperty = Fieldproperty }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetProjectDate(int ProjectID)
        {
            var list = new ReportDAL().projectDates(ProjectID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPoject(int ClientID)
        {

            ProjectMasterDAL objProjectMasterDAL = new ProjectMasterDAL();
            var list = objProjectMasterDAL.GetProjectNamebyClientId(ClientID).OrderBy(a => a.ProjectName).Distinct();

            return Json(list, JsonRequestBehavior.AllowGet);

        }



        public ActionResult getCurrentInstallerList()
        {

            InstallerDAL objProjectMasterDAL = new InstallerDAL();
            var list = objProjectMasterDAL.GetInstallerList().OrderBy(a => a.UserName).Distinct();

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getRowPartial()
        {
            return PartialView("Partial_InstallerAllocation");
        }
        public JsonResult GetAddress(List<string> Root, List<string> Cycle, long ProjectID, string installerId, string fromDate, string toDate)
        {
            string[] SplitFrm = fromDate.Split('-');
            string dtfrm = SplitFrm[1].ToString() + "-" + SplitFrm[0].ToString() + "-" + SplitFrm[2].ToString();
            string[] SplitTo = toDate.Split('-');
            string dtto = SplitTo[1].ToString() + "-" + SplitTo[0].ToString() + "-" + SplitTo[2].ToString();
            DateTime fd = Convert.ToDateTime(dtfrm);
            DateTime to = Convert.ToDateTime(dtto);
            string s = fd.ToString("dd MMM yyyy");
            string t = to.ToString("dd MMM yyyy");
            DateTime fd1 = Convert.ToDateTime(s);
            DateTime to1 = Convert.ToDateTime(t);
            System.Collections.Generic.List<tblUploadedData> installerAddress = new System.Collections.Generic.List<tblUploadedData>();
            System.Collections.Generic.List<tblUploadedData> list = new ProjectMasterDAL().GetAddress(Root, Cycle, ProjectID);
            if (!string.IsNullOrEmpty(installerId))
            {
                System.Collections.Generic.List<tblUploadedData> installerAddress2 = new ProjectMasterDAL().GetInstallerAddress(ProjectID, Root, Cycle, fd1, to, long.Parse(installerId));
                installerAddress = installerAddress2;
                foreach (tblUploadedData current in installerAddress2)
                {
                    list.Add(current);
                }
                list = (
                    from o in list
                    orderby o.Street
                    select o).ToList<tblUploadedData>();
            }
            JsonResult jsonResult = base.Json(new
            {
                AllAddress = list,
                InstallerAddress = installerAddress
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = new int?(2147483647);
            return jsonResult;
        }




        public ActionResult GetClientList()
        {

            ProjectMasterDAL objProjectMasterDAL = new ProjectMasterDAL();
            var list = objProjectMasterDAL.GetActiveClientRecords().OrderBy(a => a.ClientName).Distinct();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRootByCycle(long ProjectId, string[] Cycle, string InstallerId, string strDateTime, string strtoTime)
        {
            //System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
            //InstallerDAL installerDAL = new InstallerDAL();

            System.Collections.Generic.List<tblUploadedData> list2 = new System.Collections.Generic.List<tblUploadedData>();
            List<string> list = new InstallerDAL().GetRootByCycle(Cycle, ProjectId);

            //if (list2 != null && list2.Count > 0)
            //{
            //    foreach (tblUploadedData current in list2)
            //    {
            //        list.Add(current.Route);
            //    }
            //}

            string[] SplitFrm = strDateTime.Split('-');
            string dtfrm = SplitFrm[1].ToString() + "-" + SplitFrm[0].ToString() + "-" + SplitFrm[2].ToString();
            string[] SplitTo = strtoTime.Split('-');
            string dtto = SplitTo[1].ToString() + "-" + SplitTo[0].ToString() + "-" + SplitTo[2].ToString();
            DateTime fd = Convert.ToDateTime(dtfrm);
            DateTime to = Convert.ToDateTime(dtto);
            string s = fd.ToString("dd MMM yyyy");
            string t = to.ToString("dd MMM yyyy");
            DateTime fd1 = Convert.ToDateTime(s);
            DateTime to1 = Convert.ToDateTime(t);

            System.Collections.Generic.List<string> list3 = new System.Collections.Generic.List<string>();
            list3.AddRange(list.Distinct<string>());
            System.Collections.Generic.List<string> list4 = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrEmpty(InstallerId))
            {
                CompassEntities compassEntities = new CompassEntities();
                System.Collections.Generic.List<string> list5 = (
                    from o in compassEntities.STP_GetVisitForUserInstallation(fd1, to1, new long?(long.Parse(InstallerId)), new long?(ProjectId)).ToList<STP_GetVisitForUserInstallation_Result>()
                    select o.Route).Distinct<string>().ToList<string>();
                foreach (string current2 in list5)
                {
                    list4.Add(current2);
                }
            }
            return base.Json(new
            {
                Route = list3,
                selectedRoute = list4
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAddressByCycle_Root(string Cycle, string Root, long ProjectId)
        {
            List<string> Address = new List<string>();
            List<tblUploadedData> objtblUploadedData = new List<tblUploadedData>();
            InstallerDAL obj = new InstallerDAL();
            objtblUploadedData = obj.GetAddressByCycle_Root(Cycle, Root, ProjectId);
            if (objtblUploadedData != null && objtblUploadedData.Count > 0)
            {
                foreach (var item in objtblUploadedData)
                {
                    Address.Add(item.Street);
                }

            }
            return Json(Address, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLat_by_Root(List<long> Address)//string [] Address)
        {
            InstallerDAL obj = new InstallerDAL();

            List<tblUploadedDataBO> data = obj.GetLat_Lag(Address);
            return Json(data, JsonRequestBehavior.AllowGet);


        }
        public ActionResult GetLat_by_ProjectInstaller(long ProjectID, long InstallerID, string fromDate, string toDate)//string [] Address)
        {
            InstallerDAL obj = new InstallerDAL();

            object data = obj.GetExistingCycleList(ProjectID, InstallerID, fromDate, toDate);
            return Json(data, JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetInstallerList()
        {
            System.Collections.Generic.List<UsersModel> installerList = new InstallerDAL().GetInstallerList();
            return base.Json(installerList, JsonRequestBehavior.AllowGet);
        }
        public class Address
        {
            public int id { get; set; }
        }


        StringBuilder HTMLContent = new StringBuilder();
        StringBuilder HTMLContent1 = new StringBuilder();

        public ActionResult DownloadFile1()
        {

            //var t = new Thread(HtmlToPdf);
            //t.SetApartmentState(ApartmentState.STA);
            //t.Start();


            long id = Convert.ToInt64(Request.QueryString["id"]);
            long frmID = Convert.ToInt64(Request.QueryString["frmID"]);
            //From frmid get the Projects details
            var formObj = new FormDAL().GetFormmasterFields(frmID);
            ProjectModel objProjectModel = null;
            if (formObj != null)
            {
                objProjectModel = new ProjectMasterDAL().GetProjectDetail(formObj.ProjectId);
            }
            iTextSharp.text.Image gif = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/MSTS/img/6.png"));
            try
            {
                var list = new ReportDAL().GetReportsRecordsID(id, frmID);
                var listitem = list.GroupBy(o => o.sectionName);
                HTMLContent.Append("<html><head> <meta http-equiv='Content-Type' content='text/html;charset=utf-8' /> <meta http-equiv='X-UA-Compatible' content='IE=8,IE=9' /></head><body style='font-family:calibri;padding:0px; margin:36px auto; '><div style=\"margin:0 auto;  border:2px solid gray;  width:720px; z-index: 1;\"><div style=\"padding-left:35px;padding-top: 9px;\"> <b>Project Name:" + objProjectModel.ProjectName + " </b></div>");
                HTMLContent.Append(" <div style=\"padding-left:35px\">" + formObj.FormName + " </div> ");
                //HTMLContent.Append("<hr />");
                foreach (var item11 in listitem)
                {
                    string section = item11.Select(o => o.sectionName).FirstOrDefault();
                    HTMLContent.Append("<br/>  <div style=\"padding-left:36px\">" + section + "</div>");
                    HTMLContent.Append("<br/> <table style='border:1px solid gray;font-family:calibri; font-size:14px; border-radius:5px;margin-left:34px;width:90%;border-collapse:collapse;'>");
                    foreach (var item in item11)
                    {
                        if (item.FieldName.ToLower() == "gps location" || item.FieldName.ToLower() == "LOCATION/GPS".ToLower())
                        {
                            HTMLContent.Append("<tr>");
                            HTMLContent.Append("<td width='40%' style=\"border-bottom: 1px solid gray !important;\">" + item.FieldName + "</td>");
                            HTMLContent.Append("<td style=\"border-bottom: 1px solid gray !important;\">" + item.Fieldlatitude + " , " + item.Fieldlongitude + "</td>");
                            HTMLContent.Append("</tr>");

                            HTMLContent.Append("<tr>");
                            //HTMLContent.Append("<td width='100%'>&nbsp;</td>");
                            HTMLContent.Append("<td colspan='2' style=\"border-bottom: 1px solid gray !important;\"><img src='https://maps.googleapis.com/maps/api/staticmap?center=" + item.Fieldlatitude + "," + item.Fieldlongitude + " &markers=" + item.Fieldlatitude + "," + item.Fieldlongitude + "&size=300x350&sensor=false' /></td>");
                            HTMLContent.Append("</tr>");
                            // <img src='" + Server.MapPath("~/Content/MSTS/img/1.png") + "' height='200px' width='150px'/>
                        }
                        else if (item.FieldName.ToLower() == "services")
                        {

                            List<ServicesModel> listService = new List<ServicesModel>();
                            //get the list of services along with images
                            listService = new ReportDAL().getServicesWithImages(item.FieldValues, item.IDS, true);
                            if (listService != null)
                            {
                                foreach (ServicesModel obj in listService)
                                {
                                    HTMLContent.Append("<tr>");
                                    HTMLContent.Append("<td width='40%' style=\"border-bottom: 1px solid gray !important;\">" + obj.Service + "</td>");

                                    if (obj.lisServiceAttachment != null)
                                    {
                                        HTMLContent.Append("<td width='60%' style=\"border-bottom: 1px solid gray !important;\">");
                                        foreach (ServiceAttachmentModel objServiceAttachment in obj.lisServiceAttachment)
                                        {
                                            if (objServiceAttachment.IsAudioVideoImage == 3)
                                            {

                                                HTMLContent.Append("<img src='" + Convert.ToString(ConfigurationManager.AppSettings["WebURL"]) + objServiceAttachment.strFilePath.Replace("..\\", "") + "' height=\"200\" width=\"150\" />");


                                            }
                                        }
                                        HTMLContent.Append("</td>");
                                        HTMLContent.Append("</tr>");
                                    }
                                    else
                                    {
                                        HTMLContent.Append("<td style=\"border-bottom: 1px solid gray !important;\"> &nbsp;</td>");
                                        HTMLContent.Append("</tr>");
                                    }
                                }
                            }

                        }
                        else if (item.FieldName.ToLower() == "ADDITIONAL SERVICES".ToLower())
                        {
                            //HTMLContent.Append("<br/><div>" + section + "</div>");
                            List<ServicesModel> listService = new List<ServicesModel>();
                            //get the list of services along with images
                            listService = new ReportDAL().getServicesWithImages(item.FieldValues, item.IDS, true);
                            if (listService != null)
                            {
                                foreach (ServicesModel obj in listService)
                                {
                                    HTMLContent.Append("<tr>");
                                    HTMLContent.Append("<td width='40%' style=\"border-bottom: 1px solid gray !important;\" >" + obj.Service + "</td>");

                                    if (obj.lisServiceAttachment != null)
                                    {
                                        HTMLContent.Append("<td width='60%' style=\"border-bottom: 1px solid gray !important;\">");
                                        foreach (ServiceAttachmentModel objServiceAttachment in obj.lisServiceAttachment)
                                        {
                                            if (objServiceAttachment.IsAudioVideoImage == 3)
                                            {

                                                HTMLContent.Append("<img src='" + Convert.ToString(ConfigurationManager.AppSettings["WebURL"]) + objServiceAttachment.strFilePath.Replace("..\\", "") + "' height=\"200\" width=\"150\" />");


                                            }
                                        }
                                        HTMLContent.Append("</td>");
                                        HTMLContent.Append("</tr>");
                                    }
                                    else
                                    {
                                        HTMLContent.Append("<td style=\"border-bottom: 1px solid gray !important;\">&nbsp;</td>");
                                        HTMLContent.Append("</tr>");
                                    }
                                }
                            }

                        }
                        else
                        {
                            HTMLContent.Append("<tr  style=\"border-bottom: 1px solid gray !important;\">");
                            HTMLContent.Append("<td width='40%' style=\"border-bottom: 1px solid gray !important;\">" + item.FieldName + "</td>");
                            HTMLContent.Append("<td  width='60%' style=\"padding-left:-5px;border-bottom: 1px solid gray !important;\">:&nbsp;" + item.FieldValues + "</td>");
                            HTMLContent.Append("</tr>");

                            if (item.Fieldlatitude != "")
                            {
                                HTMLContent.Append("<tr style=\"border-bottom: 1px solid gray !important;\">");
                                HTMLContent.Append("<td width='20%' style=\"border-bottom: 1px solid gray !important;\">&nbsp;</td>");
                                HTMLContent.Append("<td width='60%' style=\"border-bottom: 1px solid gray !important;\">:&nbsp;lat: " + item.Fieldlatitude + ", long: " + item.Fieldlongitude + "</td>");
                                HTMLContent.Append("</tr>");
                            }
                        }
                    }

                    HTMLContent.Append("</table>");

                }
                HTMLContent.Append("<br/><br/>Form Locations");
                HTMLContent.Append("<br/><div></div></div></body></html>");


                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=MyPdf.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);

                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                StringReader sr = new StringReader(HTMLContent.ToString());


                Document PDFdoc = new Document(PageSize.A4, 10.0F, 10.0F, 10.0F, 0.0F);
                iTextSharp.text.html.simpleparser.HTMLWorker htmlparser = new iTextSharp.text.html.simpleparser.HTMLWorker(PDFdoc);
                // PdfWriter.GetInstance(PDFdoc, Response.OutputStream);



                //  htmlparser.Parse(sr);

                // using (Document document = new Document())
                // {
                PdfWriter writer = PdfWriter.GetInstance(PDFdoc, Response.OutputStream);
                PDFdoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, PDFdoc, sr);
                // }


                gif.ScaleAbsolute(500, 300);
                PDFdoc.Add(gif);
                PDFdoc.Close();
                Response.Write(PDFdoc);
                Response.End();
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return View(Response);
        }

        public ActionResult DownloadFile2()
        {
            long id = Convert.ToInt64(Request.QueryString["id"]);
            long frmID = Convert.ToInt64(Request.QueryString["frmID"]);

            return View("RdlcReport");
        }

        public ActionResult RdlcReport()
        {
            return View();
        }


        #region Generate pdf


        public ActionResult GetPDFDownloadID(string id, string frmID, string buttonclicked, bool showMap = false)
        {
            log.Info("in GetPDFDownloadID");
            TempData["showMap"] = showMap;
            TempData.Keep("showMap");

            if (buttonclicked != null && buttonclicked != "")
            {
                TempData["buttonclicked"] = buttonclicked;
                TempData.Keep("buttonclicked");
            }
            else
            {

                TempData["pid"] = Convert.ToInt64(id);
                TempData["pfrmid"] = Convert.ToInt64(frmID);
                TempData.Keep("pid");
                TempData.Keep("pfrmid");
            }

            return base.Json(frmID, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DownloadFile()
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Reports", "View");

            if (objPermissionCheck)
            {
                try
                {


                    TempData.Keep("showMap");
                    log.Info("in DownloadFile");

                    var pid = Convert.ToInt64(TempData["pid"]);
                    var pfrmid = Convert.ToInt64(TempData["pfrmid"]);
                    TempData.Keep("pid");
                    TempData.Keep("pfrmid");
                    var buttonclicked = TempData["buttonclicked"];
                    string CategoryType = "";//Added by aniket to export report categorywise

                    bool nextbtn = true;
                    bool prevbtn = true;

                    List<STP_GetReportTableNew_bak_Result> ReportTableList = (List<STP_GetReportTableNew_bak_Result>)TempData["List"];
                    TempData.Keep("List");

                    STP_GetReportTableNew_bak_Result previousrecord = new STP_GetReportTableNew_bak_Result();
                    STP_GetReportTableNew_bak_Result nextrecord = new STP_GetReportTableNew_bak_Result();


                    List<STP_GetReportTableNew_bak_Result> ReportTable = new List<STP_GetReportTableNew_bak_Result>(ReportTableList);
                    // ReportTable.Reverse();

                    var ReportTablelinkedList = new LinkedList<STP_GetReportTableNew_bak_Result>(ReportTable);

                    STP_GetReportTableNew_bak_Result currentitem = (from m in ReportTablelinkedList where m.FormId == pfrmid && m.ID == pid select m).FirstOrDefault();

                    var listNode = ReportTablelinkedList.Find(currentitem);
                    CategoryType = currentitem.CategoryType;

                    if (pid != 0 && pfrmid != 0)
                    {
                        if (buttonclicked == null)
                        {
                            var next = listNode.Next;
                            if (next == null)
                            {
                                nextbtn = false;
                            }

                            var Previous = listNode.Previous;
                            if (Previous == null)
                            {

                                prevbtn = false;
                            }
                        }

                        if (buttonclicked != null && buttonclicked.ToString() == "Next")
                        {
                            #region Get NextRecordID

                            var next = listNode.Next.Value; //probably a good idea to check for null
                            var listNode1 = ReportTablelinkedList.Find(next);
                            var nextrecord1 = listNode1.Next;

                            TempData["pid"] = next.ID;
                            TempData["pfrmid"] = next.FormId;
                            TempData.Keep("pid");
                            TempData.Keep("pfrmid");

                            pid = next.ID;
                            pfrmid = next.FormId.Value;
                            CategoryType = next.CategoryType;

                            if (nextrecord1 == null)
                            {
                                nextbtn = false;
                            }
                            #endregion

                        }
                        if (buttonclicked != null && buttonclicked.ToString() == "Prev")
                        {
                            #region Get PrevRecordID

                            var prev = listNode.Previous.Value;
                            var listNode1 = ReportTablelinkedList.Find(prev);
                            var nextrecord1 = listNode1.Previous;

                            TempData["pid"] = prev.ID;
                            TempData["pfrmid"] = prev.FormId;
                            TempData.Keep("pid");
                            TempData.Keep("pfrmid");

                            pid = prev.ID;
                            pfrmid = prev.FormId.Value;
                            CategoryType = prev.CategoryType;

                            if (nextrecord1 == null)
                            {
                                prevbtn = false;
                            }

                            #endregion
                        }
                    }
                    @ViewBag.Nextbutton = nextbtn;
                    @ViewBag.Prevbutton = prevbtn;

                    @ViewBag.UploadID = pid;
                    @ViewBag.FormID = pfrmid;
                    @ViewBag.CategoryType = CategoryType;
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
                return View();
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }


        }

        public ActionResult GeneratePDF()
        {
            try
            {
                var showMap = Convert.ToBoolean(TempData["showMap"]);
                TempData.Keep("showMap");
                long ID = Convert.ToInt64(TempData["pid"]);
                long frmID = Convert.ToInt64(TempData["pfrmid"]);
                TempData.Keep("pid");
                TempData.Keep("pfrmid");
                var list = new ReportDAL().GetReportsRecordsID(ID, frmID);

                ReportHeaderModel reportheader = new ReportDAL().GetReportHeaderModel(ID);
                @ViewBag.showMap = showMap;

                @ViewBag.FormName = reportheader.FormName;
                @ViewBag.ProjectName = reportheader.ProjectName;
                @ViewBag.ProjectDate = reportheader.ProjectDate;
                @ViewBag.InstallerName = reportheader.InstallerName;

                @ViewBag.UploadID = ID;
                @ViewBag.FormID = frmID;

                string footer = "--header-font-size \"9\" --header-spacing 5 --header-right \"Project name:-" + @ViewBag.ProjectName + " \n\t Installer name:-" + @ViewBag.InstallerName + " \n\t Date submitted:-" + @ViewBag.ProjectDate + "  \"  --footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                return View("GeneratePDF", list);

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return View("Index");
            }
        }

        #region Createdby AniketJ on 2-sep-2015 To Export SKIPRTU records

        /// <summary>
        /// To show SKIPRTU records in html formate
        /// </summary>
        /// <returns></returns>
        public ActionResult GeneratePDF_SkipRTU()
        {
            try
            {
                long skipRTU_id = Convert.ToInt64(TempData["pid"]);
                long skipRTU_formID = Convert.ToInt64(TempData["pfrmid"]);
                TempData.Keep("pid");
                TempData.Keep("pfrmid");

                ReportSkipRTUModel reportSkipRTUModel = new ReportSkipRTUModel();
                if (TempData["List"] != null)
                {
                    List<STP_GetReportTableNew_bak_Result> reportList = (List<STP_GetReportTableNew_bak_Result>)TempData["List"];
                    TempData.Keep("List");
                    if (reportList.Count > 0)
                    {
                        reportSkipRTUModel = new ReportDAL().SKIP_RTU_Records(reportList, skipRTU_id, skipRTU_formID);
                    }
                    ReportHeaderModel reportheader = new ReportDAL().GetReportHeaderModel(skipRTU_id); ;
                    @ViewBag.FormName = reportheader.FormName;
                    @ViewBag.ProjectName = reportheader.ProjectName;
                    @ViewBag.ProjectDate = reportheader.ProjectDate;
                    @ViewBag.InstallerName = reportheader.InstallerName;
                }
                string footer = "--header-font-size \"9\" --header-spacing 5 --header-right \"Project name:-" + @ViewBag.ProjectName + " \n\t Installer name:-" + @ViewBag.InstallerName + " \n\t Date submitted:-" + @ViewBag.ProjectDate + "  \"  --footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                return View("GeneratePDF_SkipRTU", reportSkipRTUModel);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return View("Index");
            }
        }

        /// <summary>
        /// To download SKIPRTU records in pdf formate
        /// </summary>
        /// <param name="UploadID"></param>
        /// <returns></returns>
        public ActionResult DownloadSinglePDF_SkipRTU(long UploadID)
        {
            try
            {
                ReportSkipRTUModel reportSkipRTUModel = new ReportSkipRTUModel();
                if (TempData["List"] != null)
                {

                    long skipRTU_formID = Convert.ToInt64(TempData["pfrmid"]);
                    TempData.Keep("pfrmid");

                    List<STP_GetReportTableNew_bak_Result> reportList = (List<STP_GetReportTableNew_bak_Result>)TempData["List"];
                    if (reportList.Count > 0)
                    {
                        TempData.Keep("List");
                        reportSkipRTUModel = new ReportDAL().SKIP_RTU_Records(reportList, UploadID, skipRTU_formID);
                    }
                    ReportHeaderModel reportheader = new ReportDAL().GetReportHeaderModel(UploadID); ;
                    @ViewBag.FormName = reportheader.FormName;
                    @ViewBag.ProjectName = reportheader.ProjectName;
                    @ViewBag.ProjectDate = reportheader.ProjectDate;
                    @ViewBag.InstallerName = reportheader.InstallerName;

                }
                string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                return new Rotativa.ViewAsPdf("GeneratePDF_SkipRTU", reportSkipRTUModel)
                {
                    CustomSwitches = footer,
                    PageOrientation = Orientation.Portrait,
                    FileName = @ViewBag.ProjectName + "-Report.pdf",
                    PageSize = Size.A4,
                    PageMargins = new Margins(2, 0, 10, 0),
                    PageWidth = 210,
                    PageHeight = 297,

                };
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return View("Index");
            }
        }
        #endregion

        public ActionResult DownloadSinglePDF(long FormID, long UploadID)
        {
            CompassEntities CompassEntities = new CompassEntities();
            try
            {
                var showMap = Convert.ToBoolean(TempData["showMap"]);
                TempData.Keep("showMap");
                @ViewBag.showMap = showMap;

                long ID = Convert.ToInt64(TempData["pid"]);
                long frmID = Convert.ToInt64(TempData["pfrmid"]);
                TempData.Keep("pid");
                TempData.Keep("pfrmid");

                ID = UploadID;
                frmID = FormID;


                var list = new ReportDAL().GetReportsRecordsID(ID, frmID);
                var accountno = list.Where(a => a.FieldName.ToLower() == "account").Select(a => a.FieldValues).FirstOrDefault();

                // var listitem = list.GroupBy(o => o.sectionName);
                ReportPDFVM ReportPDFVM = new ReportPDFVM();
                ReportPDFVM.sectionfiledval = list;



                // STP_GetReportHeaderFields_Result reportheader = CompassEntities.STP_GetReportHeaderFields(ID).FirstOrDefault();
                PROC_ReportPDF_GetReportHeaderFields_Result reportheader = CompassEntities.PROC_ReportPDF_GetReportHeaderFields(ID).FirstOrDefault();
                @ViewBag.FormName = reportheader.formname;
                @ViewBag.ProjectName = reportheader.projectname;
                @ViewBag.ProjectDate = Convert.ToDateTime(reportheader.visiteddate).ToString("MMM-dd-yyyy hh:mm tt");
                @ViewBag.InstallerName = reportheader.firstname;

                @ViewBag.UploadID = ID;
                @ViewBag.FormID = frmID;

                // string footer = "--header-font-size \"9\" --header-spacing 5 --header-right \"Project name:-" + @ViewBag.ProjectName + " \n\t Installer name:-" + @ViewBag.InstallerName + " \n\t Date submitted:-" + @ViewBag.ProjectDate + "  \"  --footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";

                // return View("GeneratePDF", list);

                return new Rotativa.ViewAsPdf("GeneratePDF", list)
                {

                    CustomSwitches = footer,
                    PageOrientation = Orientation.Portrait,
                    // PageMargins = { Top = 10, Bottom = 10 },
                    FileName = @ViewBag.ProjectName + "_" + accountno + "-Report.pdf",
                    PageSize = Size.A4,

                    PageMargins = new Margins(2, 0, 10, 0),
                    PageWidth = 210,
                    PageHeight = 297

                };
            }
            catch (Exception ex)
            {

                log.Error(ex.Message);
                return View("Index");
            }

            // return View("GeneratePDF", list);

            // return View("GeneratePDF", ReportPDFVM);
        }



        public ActionResult PrintSinglePDF(long FormID, long UploadID)
        {
            CompassEntities CompassEntities = new CompassEntities();
            try
            {
                var showMap = Convert.ToBoolean(TempData["showMap"]);
                TempData.Keep("showMap");
                @ViewBag.showMap = showMap;

                long ID = Convert.ToInt64(TempData["pid"]);
                long frmID = Convert.ToInt64(TempData["pfrmid"]);
                TempData.Keep("pid");
                TempData.Keep("pfrmid");

                ID = UploadID;
                frmID = FormID;


                var list = new ReportDAL().GetReportsRecordsID(ID, frmID);
                var accountno = list.Where(a => a.FieldName.ToLower() == "account").Select(a => a.FieldValues).FirstOrDefault();

                // var listitem = list.GroupBy(o => o.sectionName);
                ReportPDFVM ReportPDFVM = new ReportPDFVM();
                ReportPDFVM.sectionfiledval = list;



                // STP_GetReportHeaderFields_Result reportheader = CompassEntities.STP_GetReportHeaderFields(ID).FirstOrDefault();
                PROC_ReportPDF_GetReportHeaderFields_Result reportheader = CompassEntities.PROC_ReportPDF_GetReportHeaderFields(ID).FirstOrDefault();
                @ViewBag.FormName = reportheader.formname;
                @ViewBag.ProjectName = reportheader.projectname;
                @ViewBag.ProjectDate = Convert.ToDateTime(reportheader.visiteddate).ToString("MMM-dd-yyyy hh:mm tt");
                @ViewBag.InstallerName = reportheader.firstname;

                @ViewBag.UploadID = ID;
                @ViewBag.FormID = frmID;

                // string footer = "--header-font-size \"9\" --header-spacing 5 --header-right \"Project name:-" + @ViewBag.ProjectName + " \n\t Installer name:-" + @ViewBag.InstallerName + " \n\t Date submitted:-" + @ViewBag.ProjectDate + "  \"  --footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";

                // return View("GeneratePDF", list);

                return new Rotativa.ViewAsPdf("GeneratePDF", list)
                {

                    CustomSwitches = footer,
                    PageOrientation = Orientation.Portrait,
                    // PageMargins = { Top = 10, Bottom = 10 },
                    //FileName = @ViewBag.ProjectName + "_" + accountno + "-Report.pdf",
                    PageSize = Size.A4,

                    PageMargins = new Margins(2, 0, 10, 0),
                    PageWidth = 210,
                    PageHeight = 297

                };
            }
            catch (Exception ex)
            {

                log.Error(ex.Message);
                return View("Index");
            }

            // return View("GeneratePDF", list);

            // return View("GeneratePDF", ReportPDFVM);
        }

        #endregion

        #region Generate pdf MultipleIDs

        public ActionResult ExportReportMultipleIDs(List<ReportRow> ReportRowList, bool showMap = false)
        {
            TempData["showMap"] = showMap;
            TempData.Keep("showMap");


            List<STP_GetReportTableNew_bak_Result> demo1 = new List<STP_GetReportTableNew_bak_Result>();

            demo1 = (List<STP_GetReportTableNew_bak_Result>)TempData["List"];
            TempData.Keep("List");

            List<ReportRow> obj = new List<ReportRow>();
            bool status = false;
            foreach (var item in ReportRowList)
            {
                //Modified by AniketJ on 2-Sep-2016 to add categoryType= Skip & RTU
                // var a = demo1.Where(o => o.FormId == item.FormID && o.ID == item.UploadedID && o.CategoryType == "COMPLETED").FirstOrDefault();
                var a = demo1.Where(o => o.FormId == item.FormID && o.ID == item.UploadedID && (o.CategoryType == "COMPLETED" || o.CategoryType == "SKIP" || o.CategoryType == "RTU")).FirstOrDefault();
                if (a != null)
                {
                    ReportRow reportRow = new ReportRow();
                    reportRow.FormID = item.FormID;
                    reportRow.UploadedID = item.UploadedID;
                    reportRow.CategoryType = a.CategoryType;
                    obj.Add(reportRow);
                    status = true;
                }
            }

            TempData["ReportRowList"] = obj;

            TempData.Keep("ReportRowList");

            return base.Json(new { Status = status }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DownloadMultipleFile()
        {
            try
            {


                var showMap = Convert.ToBoolean(TempData["showMap"]);
                TempData.Keep("showMap");
                @ViewBag.showMap = showMap;

                List<ReportRow> ReportRowList = TempData["ReportRowList"] as List<ReportRow>;
                TempData.Keep("ReportRowList");

                List<ReportPDFVM> Recordlist = new List<ReportPDFVM>();

                foreach (var ReportRow in ReportRowList)
                {
                    ReportPDFVM reportPDFVM = new ReportPDFVM();
                    ReportHeaderModel reportheader = new ReportDAL().GetReportHeaderModel(ReportRow.UploadedID);
                    reportPDFVM.FormName = reportheader.FormName;
                    reportPDFVM.ProjectName = reportheader.ProjectName;
                    reportPDFVM.VisitedDate = reportheader.ProjectDate;
                    reportPDFVM.InstallerName = reportheader.InstallerName;
                    reportPDFVM.CategoryType = ReportRow.CategoryType;

                    //Modified by AniketJ on 2-sep-2016 added RTU/SKIP records in ReportPDFVM to export
                    if (ReportRow.CategoryType == "COMPLETED")
                    {
                        var list = new ReportDAL().GetReportsRecordsID(ReportRow.UploadedID, ReportRow.FormID);
                        reportPDFVM.sectionfiledval = list;
                        reportPDFVM.FormID = ReportRow.FormID;
                        reportPDFVM.UploadID = ReportRow.UploadedID;
                    }
                    else if (ReportRow.CategoryType == "RTU" || ReportRow.CategoryType == "SKIP")
                    {
                        List<STP_GetReportTableNew_bak_Result> reportList = (List<STP_GetReportTableNew_bak_Result>)TempData["List"];
                        TempData.Keep("List");
                        var reportSkipRTUModel = new ReportDAL().SKIP_RTU_Records(reportList, ReportRow.UploadedID, ReportRow.FormID);
                        reportPDFVM.skipRtuRecordModel = reportSkipRTUModel;
                    }
                    Recordlist.Add(reportPDFVM);
                }
                string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                return View("DownloadMultipleFile", Recordlist);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return View("Index");
            }
        }


        public ActionResult ViewMultiplePDF()
        {

            try
            {

                List<ReportRow> ReportRowList = TempData["ReportRowList"] as List<ReportRow>;
                CompassEntities CompassEntities = new CompassEntities();

                TempData.Keep("ReportRowList");

                List<ReportPDFVM> Recordlist = new List<ReportPDFVM>();

                foreach (var ReportRow in ReportRowList)
                {

                    var list = new ReportDAL().GetReportsRecordsID(ReportRow.UploadedID, ReportRow.FormID);

                    ReportPDFVM reportPDFVM = new ReportPDFVM();

                    STP_GetReportHeaderFields_Result reportheader = CompassEntities.STP_GetReportHeaderFields(ReportRow.UploadedID).FirstOrDefault();
                    reportPDFVM.FormName = reportheader.formname;
                    reportPDFVM.ProjectName = reportheader.projectname;
                    reportPDFVM.VisitedDate = Convert.ToDateTime(reportheader.visiteddate).ToString("MMM-dd-yyyy hh:mm tt");
                    reportPDFVM.InstallerName = reportheader.firstname;

                    reportPDFVM.sectionfiledval = list;
                    Recordlist.Add(reportPDFVM);

                }


                //string footer = "--header-font-size \"9\" --header-spacing 5 --header-right \"Project name:-" + @ViewBag.ProjectName + " \n\t Installer name:-" + @ViewBag.InstallerName + " \n\t Date submitted:-" + @ViewBag.ProjectDate + "  \"  --footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                //return View("ViewMultiplePDF", Recordlist);

                return new Rotativa.ViewAsPdf("GenerateMultiplePDF", Recordlist)
                {
                    FileName = "ProjectReport.pdf",
                    CustomSwitches = footer,
                    PageOrientation = Orientation.Portrait,
                    //PageMargins = { Top = 10, Bottom = 10 },

                    PageSize = Size.A4,

                    PageMargins = new Margins(2, 0, 10, 0),
                    PageWidth = 210,
                    PageHeight = 297
                };
            }
            catch (Exception ex)
            {

                log.Error(ex.Message);
                return View("Index");
            }

            // return View("GeneratePDF", list);

            // return View("GeneratePDF", ReportPDFVM);
        }

        public ActionResult GenerateMultiplePDF()
        {
            try
            {
                var showMap = Convert.ToBoolean(TempData["showMap"]);
                TempData.Keep("showMap");
                @ViewBag.showMap = showMap;

                List<ReportRow> ReportRowList = TempData["ReportRowList"] as List<ReportRow>;
                CompassEntities CompassEntities = new CompassEntities();

                TempData.Keep("ReportRowList");

                List<ReportPDFVM> Recordlist = new List<ReportPDFVM>();

                string ProjectName = "ProjectReport";
                foreach (var ReportRow in ReportRowList)
                {

                    ReportPDFVM reportPDFVM = new ReportPDFVM();
                    ReportHeaderModel reportheader = new ReportDAL().GetReportHeaderModel(ReportRow.UploadedID);
                    reportPDFVM.FormName = reportheader.FormName;
                    reportPDFVM.ProjectName = reportheader.ProjectName;
                    reportPDFVM.VisitedDate = reportheader.ProjectDate;
                    reportPDFVM.InstallerName = reportheader.InstallerName;
                    reportPDFVM.CategoryType = ReportRow.CategoryType;

                    ProjectName = reportPDFVM.ProjectName;

                    //Modified by AniketJ on 2-sep-2016 added RTU/SKIP records in ReportPDFVM to export
                    if (ReportRow.CategoryType == "COMPLETED")
                    {
                        var list = new ReportDAL().GetReportsRecordsID(ReportRow.UploadedID, ReportRow.FormID);
                        reportPDFVM.FormID = ReportRow.FormID;
                        reportPDFVM.UploadID = ReportRow.UploadedID;
                        reportPDFVM.sectionfiledval = list;
                    }
                    else if (ReportRow.CategoryType == "RTU" || ReportRow.CategoryType == "SKIP")
                    {
                        List<STP_GetReportTableNew_bak_Result> reportList = (List<STP_GetReportTableNew_bak_Result>)TempData["List"];
                        TempData.Keep("List");
                        var reportSkipRTUModel = new ReportDAL().SKIP_RTU_Records(reportList, ReportRow.UploadedID, ReportRow.FormID);
                        reportPDFVM.skipRtuRecordModel = reportSkipRTUModel;
                    }
                    Recordlist.Add(reportPDFVM);
                }
                string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";


                //return View(Recordlist);
                return new Rotativa.ViewAsPdf("GenerateMultiplePDF", Recordlist)
                {
                    FileName = ProjectName + ".pdf",
                    CustomSwitches = footer,
                    PageOrientation = Orientation.Portrait,
                    PageSize = Size.A4,
                    PageMargins = new Margins(2, 0, 10, 0),
                    PageWidth = 210,
                    PageHeight = 297
                };
            }
            catch (Exception ex)
            {

                log.Error(ex.Message);
                return View("Index");
            }

            // return View("GeneratePDF", list);

            // return View("GeneratePDF", ReportPDFVM);
        }

        #endregion

        #region reopen

        public ActionResult reOpenRecord(long uploadId, long installerId, long formId, long reOpenId, string reOpenComment, string taskStatus, string meterId, string radioId, long oldInstallerId)
        {
            try
            {
                ReportDAL objReportDAL = new ReportDAL();
                long userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                bool result = objReportDAL.reOpenRecords(uploadId, installerId, formId, reOpenId, reOpenComment, (int)userId, taskStatus, meterId, radioId, oldInstallerId);

                #region Added code by vis o send mail installer when issue reopen
                if (result)
                {
                    List<String> mailto = new List<String>();
                    CompassEntities db = new CompassEntities();
                    //  string Subject = (db.tblSkipReasonMasters.Where(n => n.SkipId == reOpenId).FirstOrDefault().SkipReason);
                    var UserEmailId = (db.tblUsers.Where(n => n.UserID == installerId).FirstOrDefault().Email);
                    List<String> CCEmailTo = objReportDAL.CCEmailTo();
                    string Subject = objReportDAL.GetSubject();
                    Task.Factory.StartNew(() =>
                    {
                        mailto.Add(UserEmailId);
                        // mailto.Add("vishal.takawle@kairee.in");
                        // CCEmailTo.Add("bharat.magdum@kairee.in");
                        new GarronT.CMS.Model.clsMail().SendMail(mailto, CCEmailTo, objReportDAL.SendMalil(reOpenComment, uploadId, installerId, formId), Subject);
                    });
                }
                #endregion


                return Json(new { success = result, status = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, status = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }


        #endregion

        public FileResult ShowGoogleStaticMap(String googleStaticMapAddress)
        {
            //Size up to 640-640 on the free version
            Task<byte[]> result = GetStaticMap(googleStaticMapAddress, "500x500");

            //Quick check to see if we have an image
            if (result == null)
                return null;

            var file = result.Result;
            return File(file, "image/jpeg");
        }

        public async Task<byte[]> GetStaticMap(string location, string size)
        {

            if (string.IsNullOrEmpty(location))
            {
                return null;
            }
            string[] St = location.Split(',');
            try
            {

                // var stasticMapApiUrl = "https://maps.googleapis.com/maps/api/staticmap?&markers=color:navy%7Clabel:R%7C62.107733," + location + "&zoom=12&maptype=hybrid&sensor=false";
                //var stasticMapApiUrl="https://maps.googleapis.com/maps/api/staticmap?key=" + Convert.ToString(ConfigurationManager.AppSettings["googleapiKey"]) + center + " &zoom=13&size=200x200&maptype=roadmap" + lastlist + "&sensor=false";
                var stasticMapApiUrl = "https://maps.googleapis.com/maps/api/staticmap?sensor=false&size=150x180&maptype=google.maps.MapTypeId.ROADMAP&visible=" + St[0] + "," + St[1] + "&markers=color:red%7Ccolor:red%7Clabel:Point%7C" + St[0] + "," + St[1];

                var formattedImageUrl = string.Format("{0}&size={1}", string.Format("{0}&center={1}", stasticMapApiUrl, location), size);
                var httpClient = new HttpClient();

                var imageTask = httpClient.GetAsync(formattedImageUrl);
                HttpResponseMessage response = imageTask.Result;
                response.EnsureSuccessStatusCode();
                await response.Content.LoadIntoBufferAsync();

                return await response.Content.ReadAsByteArrayAsync();

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        #region changed by Vishal 24_08_2016

        /// <summary>
        /// Rotates Image.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="attachmentId"></param>
        /// <returns></returns>
        public ActionResult RotateImage(string direction, long attachmentId)
        {
            try
            {
                ReportDAL objReportDAL = new ReportDAL();
                string path = objReportDAL.getServiceFieldImagePath(attachmentId);//filePath;
                string orignalPath = path;
                if (!string.IsNullOrEmpty(path))
                {
                    // path = "~\\" + path.Replace("..\\..\\", "");
                    if (path.Contains("..\\..\\"))
                    {
                        path = "~\\" + path.Replace("..\\..\\", "");
                    }

                    if (path.Contains("~"))
                    {
                        path = "~\\" + path.Replace("~\\", "");
                    }
                    path = "" + path.Replace('\\', '/');
                    path = path.Trim();



                }
                //create an image object from the image in that path


                path = Server.MapPath(path);
                log.Info("Get path " + path);
                //create an image object from the image in that path

                string fileName = Path.GetFileNameWithoutExtension(orignalPath);
                DateTime currentDateTime = new CommonFunctions().ServerDate();
                string newFileName = currentDateTime.ToString("dd-MM-yyyy-hh-mm-ss-tt").Replace('-', '_');
                string NewFileName = orignalPath.Replace(fileName, newFileName);
                string newPath = NewFileName;
                if (!string.IsNullOrEmpty(newPath))
                {
                    //newPath = "~\\" + newPath.Replace("..\\..\\", "");

                    if (newPath.Contains("..\\..\\"))
                    {
                        newPath = "~\\" + newPath.Replace("..\\..\\", "");
                    }

                    if (newPath.Contains("~"))
                    {
                        newPath = "~\\" + newPath.Replace("~\\", "");
                    }
                    newPath = "" + newPath.Replace('\\', '/');
                    newPath = newPath.Trim();
                }
                //create an image object from the image in that path


                newPath = Server.MapPath(newPath);
                System.IO.File.Copy(path, newPath);
                System.Drawing.Image img = System.Drawing.Image.FromFile(newPath);


                if (direction == "left")
                {
                    //Rotate the image in memory
                    img.RotateFlip(System.Drawing.RotateFlipType.Rotate270FlipNone);
                }
                else
                {
                    //Rotate the image in memory
                    img.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
                }




                //save the image out to the file
                img.Save(newPath);
                //release image file

                log.Info("Save " + NewFileName);
                img.Dispose();

                long newFileId = objReportDAL.saveRotatedImageFilePath(attachmentId, NewFileName);

                //Delete the file so the new image can be saved
                System.IO.File.Delete(path);
                log.Info("Delete " + path);


                string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);
                // NewFileName = "" + NewFileName.Replace("..\\..\\", _webUrl);

                if (NewFileName.Contains("..\\..\\"))
                {
                    NewFileName = "" + NewFileName.Replace("..\\..\\", _webUrl);
                }

                if (NewFileName.Contains("~"))
                {
                    NewFileName = "" + NewFileName.Replace("~\\", _webUrl);
                }


                NewFileName = "" + NewFileName.Replace('\\', '/');
                NewFileName = NewFileName.Trim();




                return Json(new { success = true, newId = newFileId, newPath = NewFileName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, status = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        public ActionResult UploadImage(long imagesID, string uploadedfrom)
        {
            CompassEntities cmp = new CompassEntities();
            bool result = false;
            string returnMessage = "";

            //string ServerUploadFolder = Convert.ToString(ConfigurationManager.AppSettings["UploadFilePath"]);


            //string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" +
            //          AccountNumber + @"\" + accountInitialFieldId + @"\" + imageName + @"\";


            foreach (string file in Request.Files)
            {

                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                var fileContent = Request.Files[file];
                ReportDAL objReportDAL = new ReportDAL();

                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    var stream = fileContent.InputStream;
                    // get a stream
                    //var stream = fileContent.InputStream;
                    // and optionally write the file to disk

                    string path1 = objReportDAL.getServiceFieldImagePath(imagesID);//filePath;\ //  string dsf = HostingEnvironment.MapPath(path1)
                    if (path1 != "")
                    {



                        DateTime DT = new CommonFunctions().ServerDate();

                        var fileName = Path.GetFileName(hpf.FileName);

                        string oldFilePath = HttpContext.Server.MapPath(path1);
                        string directoryPath = Path.GetDirectoryName(oldFilePath);

                        //created new file path
                        string newFilePath = directoryPath + "/" + DT.ToString("dd_MM_yyyy_hh_mm_ss_tt") + Path.GetExtension(oldFilePath);

                        //save new file on given new file path on server 
                        using (FileStream fileStream = System.IO.File.Create(newFilePath))
                        {
                            stream.CopyTo(fileStream);
                            log.Info("Copy File succesfully........");

                            string newfileName = Path.GetFileName(newFilePath);

                            string oldFileName = Path.GetFileName(oldFilePath);
                            string newFileDBPath = path1.Replace(oldFileName, newfileName);
                            log.Info("newFileDBPath " + newFileDBPath);

                            int CreatedBy = Convert.ToInt32(GetLoggedUserID());

                            long newAttachmentId = 0;

                            //method that insert new record.
                            result = objReportDAL.UploadImages(newFileDBPath, imagesID, out returnMessage, out newAttachmentId, CreatedBy);

                        }
                    }
                }
            }

            if (result)
            {
                return Json(new { success = result, status = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json(new { success = result, status = returnMessage }, JsonRequestBehavior.AllowGet);

            }
            // return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult RemoveImage(int imagesID)
        //{
        //    bool result = false;
        //    try
        //    {
        //        CompassEntities compassEntities = new CompassEntities();
        //        var data = compassEntities.TblProjectFieldDataAttachments.Where(o => o.ID == imagesID && o.Active == 1).ToList();
        //        foreach (var item in data)
        //        {
        //            item.Active = 0;
        //        }
        //        UpdateModel(data);
        //        compassEntities.SaveChanges();
        //        result = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;

        //        throw;
        //    }

        //    if (result)
        //        return Json(new { success = result, status = "Success" }, JsonRequestBehavior.AllowGet);
        //    else
        //        return Json(new { success = result, status = "UnSuccess" }, JsonRequestBehavior.AllowGet);

        //}



        /// <summary>
        /// Uploades new audio file in server & saves information in the database. 
        /// </summary>
        /// <param name="AudioID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadAudio(long AudioID)
        {

            bool result = false;
            string returnMessage = "";
            long AttachmentId = 0;
            foreach (string file in Request.Files)
            {

                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                var fileContent = Request.Files[file];
                ReportDAL objReportDAL = new ReportDAL();

                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    var stream = fileContent.InputStream;

                    //get existing file path from database for give id
                    string path1 = objReportDAL.getServiceFieldAudioVideoPath(AudioID);

                    if (path1 != "")
                    {
                        DateTime DT = new CommonFunctions().ServerDate();

                        var fileName = Path.GetFileName(hpf.FileName);

                        string oldFilePath = HttpContext.Server.MapPath(path1);
                        string directoryPath = Path.GetDirectoryName(oldFilePath);

                        //created new file path
                        string newFilePath = directoryPath + "/" + DT.ToString("dd_MM_yyyy_hh_mm_ss_tt") + Path.GetExtension(oldFilePath);

                        //save new file on given new file path on server 
                        using (FileStream fileStream = System.IO.File.Create(newFilePath))
                        {

                            stream.CopyTo(fileStream);
                            log.Info("Copy File succesfully........");

                            string newfileName = Path.GetFileName(newFilePath);

                            string oldFileName = Path.GetFileName(oldFilePath);
                            string newFileDBPath = path1.Replace(oldFileName, newfileName);
                            log.Info("newFileDBPath " + newFileDBPath);

                            int CreatedBy = Convert.ToInt32(GetLoggedUserID());
                            //method that deactivates existing record & insert new record.
                            result = objReportDAL.UploadAudio(newFileDBPath, AudioID, out AttachmentId, CreatedBy);

                        }
                    }
                }
            }

            if (result)
            {
                GetAttachmentBo ObjList = new GetAttachmentBo();

                //get details for new uploaded file
                ObjList = new ReportDAL().GetAttachmentDetails(AttachmentId);

                //get site url from config file
                string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);

                if (ObjList != null)
                {
                    if (!string.IsNullOrEmpty(ObjList.strPath))
                    {
                        //ObjList.strPath = "" + ObjList.strPath.Replace("..\\..\\", _webUrl);
                        if (ObjList.strPath.Contains("..\\..\\"))
                        {
                            ObjList.strPath = "" + ObjList.strPath.Replace("..\\..\\", _webUrl);
                        }

                        if (ObjList.strPath.Contains("~"))
                        {
                            ObjList.strPath = "" + ObjList.strPath.Replace("~\\", _webUrl);
                        }

                        ObjList.strPath = "" + ObjList.strPath.Replace('\\', '/');
                        ObjList.strPath = ObjList.strPath.Trim();
                    }
                }
                return Json(new { success = result, status = "Success", ObjList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = result, status = returnMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        
        /// <summary>
        /// Uploads new video file & saves info in database
        /// </summary>
        /// <param name="VideoId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadVideo(long VideoId)
        {

            bool result = false;
            string returnMessage = "";
            long AttachentID = 0;
            foreach (string file in Request.Files)
            {

                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                var fileContent = Request.Files[file];
                ReportDAL objReportDAL = new ReportDAL();

                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    var stream = fileContent.InputStream;

                    //returns the existing file path
                    string path1 = objReportDAL.getServiceFieldAudioVideoPath(VideoId);

                    if (path1 != "")
                    {
                        DateTime DT = new CommonFunctions().ServerDate();

                        var fileName = Path.GetFileName(hpf.FileName);
                        string oldFilePath = HttpContext.Server.MapPath(path1);
                        string directoryPath = Path.GetDirectoryName(oldFilePath);

                        //new file path for new file
                        string newFilePath = directoryPath + "/" + DT.ToString("dd_MM_yyyy_hh_mm_ss_tt") + Path.GetExtension(oldFilePath);


                        //code of save the new file on given path
                        using (FileStream fileStream = System.IO.File.Create(newFilePath))
                        {
                            stream.CopyTo(fileStream);
                            log.Info("Copy File Successfully...");

                            string newfileName = Path.GetFileName(newFilePath);

                            string oldFileName = Path.GetFileName(oldFilePath);
                            string newFileDBPath = path1.Replace(oldFileName, newfileName);
                            int CreatedBy = Convert.ToInt32(GetLoggedUserID());


                            //method that deactivates existing record & insert new record.
                            result = objReportDAL.UploadVideo(newFileDBPath, VideoId, out AttachentID, CreatedBy);

                        }
                    }
                }
            }

            if (result)
            {
                GetAttachmentBo ObjList = new GetAttachmentBo();

                //get details for new uploaded file
                ObjList = new ReportDAL().GetAttachmentDetails(AttachentID);

                //get site url from config file
                string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);


                if (ObjList != null)
                {
                    if (!string.IsNullOrEmpty(ObjList.strPath))
                    {
                        // ObjList.strPath = "" + ObjList.strPath.Replace("..\\..\\", _webUrl);
                        if (ObjList.strPath.Contains("..\\..\\"))
                        {
                            ObjList.strPath = "" + ObjList.strPath.Replace("..\\..\\", _webUrl);
                        }

                        if (ObjList.strPath.Contains("~"))
                        {
                            ObjList.strPath = "" + ObjList.strPath.Replace("~\\", _webUrl);
                        }
                        ObjList.strPath = "" + ObjList.strPath.Replace('\\', '/');
                        ObjList.strPath = ObjList.strPath.Trim();
                    }


                    ObjList.strPath = string.IsNullOrEmpty(ObjList.strPath) ? null : ObjList.strPath;
                    ObjList.strPath = string.IsNullOrEmpty(ObjList.strPath) ? null : ObjList.strPath;
                }

                return Json(new { success = result, status = "Success", ObjList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = result, status = returnMessage }, JsonRequestBehavior.AllowGet);

            }

        }


        // get User Id
        public long GetLoggedUserID()
        {

            var vr = User.Identity.Name;
            CompassEntities AIBCARMEntities = new CompassEntities();
            var userid = AIBCARMEntities.tblUsers.Where(x => x.UserName == vr).Select(x => x.UserID).FirstOrDefault();
            return userid;
        }

        [HttpGet]
        public ActionResult GetAudioPath(long Audio_AttachmentId)
        {
            try
            {
                GetAttachmentBo ObjList = new GetAttachmentBo();
                ObjList = new ReportDAL().GetAttachmentDetails(Audio_AttachmentId);//filePath;\
                string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);
                //    _webUrl= "https://localhost:6069/";
                if (ObjList != null)
                {
                    if (!string.IsNullOrEmpty(ObjList.strPath))
                    {
                        //ObjList.strPath = "" + ObjList.strPath.Replace("..\\..\\", _webUrl);
                        if (ObjList.strPath.Contains("..\\..\\"))
                        {
                            ObjList.strPath = "" + ObjList.strPath.Replace("..\\..\\", _webUrl);
                        }

                        if (ObjList.strPath.Contains("~"))
                        {
                            ObjList.strPath = "" + ObjList.strPath.Replace("~\\", _webUrl);
                        }

                        ObjList.strPath = "" + ObjList.strPath.Replace('\\', '/');
                        ObjList.strPath = ObjList.strPath.Trim();
                    }


                    ObjList.strPath = string.IsNullOrEmpty(ObjList.strPath) ? null : ObjList.strPath;
                    ObjList.strPath = string.IsNullOrEmpty(ObjList.strPath) ? null : ObjList.strPath;


                    return Json(new { success = true, ObjList }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, ObjList }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        #region Createdby AniketJ on 1-sep-2016

        public ActionResult GetSkipRTUID(string id, string formID, string buttonclicked)
        {

            if (buttonclicked != null && buttonclicked != "")
            {
                TempData["SkipRTU_buttonclicked"] = buttonclicked;
                TempData.Keep("SkipRTU_buttonclicked");
            }
            else
            {
                TempData["SkipRTU_id"] = Convert.ToInt64(id);
                TempData["SkipRTU_formID"] = Convert.ToInt64(formID);
                TempData.Keep("SkipRTU_id");
                TempData.Keep("SkipRTU_formID");
            }

            return base.Json(formID, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadFileSkipRTU()
        {
            try
            {

                var skipRTU_id = Convert.ToInt64(TempData["SkipRTU_id"]);
                var skipRTU_formID = Convert.ToInt64(TempData["SkipRTU_formID"]);
                var SkipRTU_buttonclicked = TempData["SkipRTU_buttonclicked"];

                bool nextbtn = true;
                bool prevbtn = true;

                TempData.Keep("SkipRTU_id");
                TempData.Keep("SkipRTU_formID");

                List<STP_GetReportTableNew_bak_Result> ReportTableList = (List<STP_GetReportTableNew_bak_Result>)TempData["List"];
                TempData.Keep("List");

                STP_GetReportTableNew_bak_Result previousrecord = new STP_GetReportTableNew_bak_Result();
                STP_GetReportTableNew_bak_Result nextrecord = new STP_GetReportTableNew_bak_Result();


                List<STP_GetReportTableNew_bak_Result> ReportTable = new List<STP_GetReportTableNew_bak_Result>(ReportTableList);
                ReportTable.Reverse();

                var ReportTablelinkedList = new LinkedList<STP_GetReportTableNew_bak_Result>(ReportTable);

                STP_GetReportTableNew_bak_Result currentitem = (from m in ReportTablelinkedList where m.FormId == skipRTU_formID && m.ID == skipRTU_id select m).FirstOrDefault();

                var listNode = ReportTablelinkedList.Find(currentitem);


                if (skipRTU_id != 0 && skipRTU_formID != 0)
                {

                    if (SkipRTU_buttonclicked == null)
                    {
                        var next = listNode.Next;
                        if (next == null)
                        {
                            nextbtn = false;
                        }

                        var Previous = listNode.Previous;
                        if (Previous == null)
                        {
                            prevbtn = false;
                        }
                    }

                    if (SkipRTU_buttonclicked != null && SkipRTU_buttonclicked.ToString() == "Next")
                    {
                        #region Get NextRecordID

                        var next = listNode.Next.Value; //probably a good idea to check for null

                        var listNode1 = ReportTablelinkedList.Find(next);

                        var nextrecord1 = listNode1.Next;

                        TempData["SkipRTU_id"] = next.ID;
                        TempData["SkipRTU_formID"] = next.FormId;
                        TempData.Keep("SkipRTU_id");
                        TempData.Keep("SkipRTU_formID");

                        if (nextrecord1 == null)
                        {
                            nextbtn = false;
                        }
                        else
                        {

                        }

                        #endregion

                    }
                    if (SkipRTU_buttonclicked != null && SkipRTU_buttonclicked.ToString() == "Prev")
                    {
                        #region Get PrevRecordID

                        var prev = listNode.Previous.Value;

                        var listNode1 = ReportTablelinkedList.Find(prev);

                        var nextrecord1 = listNode1.Previous;

                        TempData["SkipRTU_id"] = prev.ID;
                        TempData["SkipRTU_formID"] = prev.FormId;
                        TempData.Keep("SkipRTU_id");
                        TempData.Keep("SkipRTU_formID");

                        if (nextrecord1 == null)
                        {
                            prevbtn = false;
                        }
                        else
                        {

                        }
                        #endregion
                    }
                }
                @ViewBag.Nextbutton = nextbtn;
                @ViewBag.Prevbutton = prevbtn;

                @ViewBag.UploadID = skipRTU_id;
                @ViewBag.FormID = skipRTU_formID;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return View();
        }

      
        public ActionResult DownloadPDF_SkipRTU(long FormID, long UploadID)
        {
            CompassEntities CompassEntities = new CompassEntities();
            try
            {

                long ID = Convert.ToInt64(TempData["pid"]);
                long frmID = Convert.ToInt64(TempData["pfrmid"]);
                TempData.Keep("pid");
                TempData.Keep("pfrmid");

                ID = UploadID;
                frmID = FormID;


                var list = new ReportDAL().GetReportsRecordsID(ID, frmID);

                // var listitem = list.GroupBy(o => o.sectionName);
                ReportPDFVM ReportPDFVM = new ReportPDFVM();
                ReportPDFVM.sectionfiledval = list;



                STP_GetReportHeaderFields_Result reportheader = CompassEntities.STP_GetReportHeaderFields(ID).FirstOrDefault();
                @ViewBag.FormName = reportheader.formname;
                @ViewBag.ProjectName = reportheader.projectname;
                @ViewBag.ProjectDate = Convert.ToDateTime(reportheader.visiteddate).ToString("MMM-dd-yyyy hh:mm tt");
                @ViewBag.InstallerName = reportheader.firstname;



                // string footer = "--header-font-size \"9\" --header-spacing 5 --header-right \"Project name:-" + @ViewBag.ProjectName + " \n\t Installer name:-" + @ViewBag.InstallerName + " \n\t Date submitted:-" + @ViewBag.ProjectDate + "  \"  --footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";

                // return View("GeneratePDF", list);

                return new Rotativa.ViewAsPdf("GeneratePDF", list)
                {

                    CustomSwitches = footer,
                    PageOrientation = Orientation.Portrait,
                    //PageMargins = { Top = 10, Bottom = 10 },
                    FileName = @ViewBag.FormName + "-Report.pdf",
                    PageSize = Size.A4,

                    PageMargins = new Margins(2, 0, 10, 0),
                    PageWidth = 210,
                    PageHeight = 297

                };
            }
            catch (Exception ex)
            {

                log.Error(ex.Message);
                return View("Index");
            }

            // return View("GeneratePDF", list);

            // return View("GeneratePDF", ReportPDFVM);
        }

        #endregion GetDataForAuditorView

        #region get data on popup for Auditor Allocation  to view only In Audit  Created by Sominath

        [HttpGet]
        public ActionResult GetDataForAuditorView(string uploadedId, string projectId, string installerId, string formid)
        {
            ReportDAL objReportDAL = new ReportDAL();
            AuditorAllocationData AuditData = new AuditorAllocationData();

            // List<UsersSmallModel> auditorList = new List<UsersSmallModel>();
            try
            {
                long UplodedID = Convert.ToInt32(uploadedId);
                long ProjectIdID = Convert.ToInt32(projectId);
                long InstallerID = Convert.ToInt32(installerId);
                long FormID = Convert.ToInt32(formid);
                // auditorList = objReportDAL.GetAuditorUserList().ToList();
                AuditData = objReportDAL.GetAuditorInfoPopup(UplodedID, ProjectIdID, FormID);

                return Json(new { success = true, dataList = AuditData }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        
        #endregion

        #region get data on popup for Auditor Allocation to edit for auditor allocation  and Save it into DB  Created by Sominath

        [HttpGet]
        public ActionResult GetDataForAuditorAllocation(string uploadedId, string projectId, string installerId, string formid)
        {
            ReportDAL objReportDAL = new ReportDAL();
            AuditorAllocationData AuditData = new AuditorAllocationData();

            List<UsersSmallModel> auditorList = new List<UsersSmallModel>();
            try
            {
                long UplodedID = Convert.ToInt64(uploadedId);
                long ProjectIdID = Convert.ToInt64(projectId);
                long InstallerID = Convert.ToInt64(installerId);
                long FormID = Convert.ToInt64(formid);
                auditorList = objReportDAL.GetAuditorUserList(ProjectIdID, InstallerID);
                AuditData = objReportDAL.GetAuditorInfoPopup(UplodedID, ProjectIdID, FormID);

                return Json(new { success = true, AuditData = AuditData, auditorList = auditorList }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        
        #endregion

        #region Insert data into Project Audit Allocation table after auditor allocation from popup Created by Sominath

        public ActionResult InsertProjectAuditAllocation(AuditorAllocationData _objAuditorAllocation)
        {
            bool success = false;
            string _OutResult = "Invalid request";
            ReportDAL objReportDAL = new ReportDAL();
            try
            {
                long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());

                success = objReportDAL.InsertAuditorAllocation(_objAuditorAllocation, currentUserId, out _OutResult);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, returnMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }



            return base.Json(new { success = success, returnMsg = _OutResult }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        /*
         * 2018-07-16 Bharat
         * Method to get all images taken against the fields to show on index page.
         */
        [HttpGet]
        public ActionResult GetImagesList(long ProjectFieldDataId)
        {

            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Reports", "View");

            if (objPermissionCheck)
            {
                ReportDAL objReportDAL = new ReportDAL();

                var IsSkip = false;
                var objResult = objReportDAL.GetRecordImages(ProjectFieldDataId, out IsSkip);
                string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);


                foreach (var item in objResult)
                {
                    string strFilePath = item.strFilePath;
                    if (!string.IsNullOrEmpty(strFilePath))
                    {

                        if (strFilePath.Contains("..\\..\\"))
                        {
                            strFilePath = "" + strFilePath.Replace("..\\..\\", _webUrl);
                        }

                        if (strFilePath.Contains("~"))
                        {
                            strFilePath = "" + strFilePath.Replace("~\\", _webUrl);
                        }

                        strFilePath = "" + strFilePath.Replace('\\', '/');
                        strFilePath = strFilePath.Trim();
                    }

                    item.strFilePath = strFilePath.Replace(" ", "%20");

                }

                return base.Json(new { success = true, IsSkip = IsSkip, returnMsg = objResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpGet]
        public ActionResult SetGLocation()
        {
            return PartialView("_UpdateLatLong");
        }


        [HttpPost]
        public ActionResult UpdateLatLong(long ProjectFieldDataMasterId, string strLat, string strLong)
        {
            ReportDAL objReportDAL = new ReportDAL();

            bool Result = objReportDAL.UpdateLatitudeLongitude(ProjectFieldDataMasterId, strLat, strLong);

            return base.Json(new { success = Result }, JsonRequestBehavior.AllowGet);
        }


    }


}
