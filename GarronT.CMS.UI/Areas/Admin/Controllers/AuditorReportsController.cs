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
    public class AuditorReportsController : Controller
    {

        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportsController));
        #endregion


        //
        // GET: /Admin/Reports/
        public ActionResult Index()
        {
            Session["A_objnewReportEditBO"] = null;
            if (TempData["A_isbackFromEdit"] == null)
            {
                TempData["AuditList"] = null;
                TempData["AProjectId"] = null;
                TempData["A_InstallerIdList"] = null;
                TempData["AfromDate"] = null;
                TempData["ATodate"] = null;
                TempData["AcycleIdList"] = null;
                TempData["ARouteIdList"] = null;
                TempData["A_AddressIdList"] = null;
            }
            else
            {
                //isbackFromEdit = false;
                TempData.Keep("AuditList");
                TempData.Keep("A_isbackFromEdit");
            }
            return View();
        }

        #region--Createdby AniketJ on 02-Jan-2016

        [HttpPost]
        public ActionResult GetPojectbyProjectStatus(List<string> ProjectStatusList)
        {
            List<ProjectModel> listProjectModel = new List<ProjectModel>();
            if (ProjectStatusList != null)
            {

                listProjectModel = new AuditReportDAL().GetProjectNamebyProjectStatus(ProjectStatusList);
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
        public JsonResult GetAuditorByProjectID(long projectId)
        {
            List<UsersSmallModel> AuditorList = new List<UsersSmallModel>();
            if (projectId > 0)
            {
                AuditorList = new AuditReportDAL().GeAuditorListbyProjectID(projectId);
            }

            return Json(AuditorList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetReopenData(long projectId)
        {

            var objPermissionCheck = UserPermissionCheck.checkUserStatus("AuditorReports", "Edit");

            if (objPermissionCheck)
            {
                List<UsersSmallModel> InstallerList = new List<UsersSmallModel>();
                List<ReasonBO> ReasonBOList = new List<ReasonBO>();
                if (projectId > 0)
                {
                    InstallerList = new AuditReportDAL().GeInstallerListbyProjectID(projectId);
                    ReasonBOList = new AuditReportDAL().GetReopenReasonList();
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

            List<string> CycleList = new AuditReportDAL().GetCycleByProjectID((long)ProjectID);

            return base.Json(CycleList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRouteListByCycle(long ProjectId, string[] Cycle)
        {

            List<string> routeList = new AuditReportDAL().GetRouteListByCycle_ProjectID(Cycle, ProjectId);

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

            List<tblUploadedData> Addresslist = new AuditReportDAL().GetAddress(Route, Cycle, ProjectID, fromDate, toDate);


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
            TempData["A_isbackFromEdit"] = "true";
            TempData.Keep("A_isbackFromEdit");
            return RedirectToAction("Index");
        }
        public ActionResult ResetData()
        {
            TempData["A_isbackFromEdit"] = null;
            //TempData.Keep("A_isbackFromEdit");
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
                if (TempData["AuditList"] == null)
                {
                    var jsonResult = Json(JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                else
                {
                    var list = TempData["AuditList"];
                    TempData.Keep("AuditList");

                    TempData["A_isbackFromEdit"] = null;

                    TempData.Keep("A_isbackFromEdit");
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
                if (TempData["A_isbackFromEdit"] != null)
                {
                    TempData.Keep("A_isbackFromEdit");
                    bool result = getdata();
                    if (result)
                    {
                        List<STP_GetReportTableNew_bak_Result> demo1 = (List<STP_GetReportTableNew_bak_Result>)TempData["AuditList"];
                        var jsonResult = Json(new { status = true, demo1 = demo1 }, JsonRequestBehavior.AllowGet);
                        jsonResult.MaxJsonLength = int.MaxValue;
                        return jsonResult;
                    }
                    else
                    {
                        List<STP_GetReportTableNew_Result> demo1 = (List<STP_GetReportTableNew_Result>)TempData["AuditList"];
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

            if (TempData["AuditList"] != null)
            {

                List<STP_GetReportTableNew_bak_Result> demo1 = (List<STP_GetReportTableNew_bak_Result>)TempData["AuditList"];
                if (demo1.Count > 0)
                {
                    //AddedBy AniketJ on 7-oct-2016 to get media
                    List<SkipRtuAttachmentModel> objAttachementList = new AuditReportDAL().getSkipRtuImages(_ID, _InstallerId, _FormID);
                    STP_GetReportTableNew_bak_Result skipRTUModel = new STP_GetReportTableNew_bak_Result();
                    skipRTUModel = demo1.Where(i => i.ID == _ID).FirstOrDefault();

                    //addedby AniketJ on 19-Jan-2016
                    skipRTUModel.skipGPSLocation = new AuditReportDAL().getSkipRtuGPSLocation(_ID, _InstallerId, _FormID);

                    foreach (var item in objAttachementList)
                    {
                        string strFilePath = item.strFilePath;
                        if (!string.IsNullOrEmpty(item.strFilePath))
                        {
                            //item.strFilePath = "" + item.strFilePath.Replace("..\\..\\", _webUrl);

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

                    skipRTUModel.SkipRtuAttachmentModel = objAttachementList;

                    TempData.Keep("AuditList");

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
        /// <param name="AuditorId"></param>
        /// <param name="fromDate"></param>
        /// <param name="Todate"></param>
        /// <param name="cycleId"></param>
        /// <param name="RouteId"></param>
        /// <param name="AdressId"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public ActionResult getListDetails(long ProjectId, List<string> AuditorId, string fromDate, string Todate, List<string> cycleId, List<string> RouteId, List<string> AdressId, List<string> Status)
        {
            try
            {
                TempData["AProjectId"] = ProjectId;
                //TempData["A_InstallerIdList"] = InstallerId;
                //if (InstallerId != null && InstallerId.Count != 0)
                //{
                //    TempData["A_InstallerIdList"] = InstallerId;
                //}
                TempData["AuditorIdList"] = AuditorId;
                if (AuditorId != null && AuditorId.Count != 0)
                {
                    TempData["AuditorIdList"] = AuditorId;
                }
                TempData["AfromDate"] = fromDate;
                TempData["ATodate"] = Todate;
                TempData["AcycleIdList"] = cycleId;
                TempData["ARouteIdList"] = RouteId;
                TempData["A_AddressIdList"] = AdressId;
                TempData["AStatusList"] = Status;
                TempData.Keep("AProjectId");
                TempData.Keep("AuditorIdList");
                TempData.Keep("AfromDate");
                TempData.Keep("ATodate");
                TempData.Keep("AcycleIdList");
                TempData.Keep("ARouteIdList");
                TempData.Keep("A_AddressIdList");

                bool result = getdata();
                if (result)
                {
                    List<STP_GetReportTableNew_bak_Result> demo1 = (List<STP_GetReportTableNew_bak_Result>)TempData["AuditList"];
                    TempData.Keep("AuditList");
                    demo1 = demo1.OrderBy(a => a.Dates).ToList();
                    var jsonResult = Json(new { status = true, demo1 = demo1 }, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                else
                {
                    List<STP_GetReportTableNew_bak_Result> demo1 = (List<STP_GetReportTableNew_bak_Result>)TempData["AuditList"];
                    demo1.OrderByDescending(a => a.Dates);
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

        public ActionResult SelectAll(long ProjectId, List<string> AuditorId, string fromDate, string Todate, List<string> cycleId, List<string> RouteId, List<string> AdressId, List<string> Status)
        {
            try
            {
                TempData["AProjectId"] = ProjectId;
                TempData["AuditorIdList"] = AuditorId;
                if (AuditorId != null && AuditorId.Count != 0)
                {
                    TempData["AuditorIdList"] = AuditorId;
                }
                TempData["AfromDate"] = fromDate;
                TempData["ATodate"] = Todate;
                TempData["AcycleIdList"] = cycleId;
                TempData["ARouteIdList"] = RouteId;
                TempData["A_AddressIdList"] = AdressId;
                TempData["AStatusList"] = Status;
                TempData.Keep("AProjectId");
                TempData.Keep("AuditorIdList");
                TempData.Keep("AfromDate");
                TempData.Keep("ATodate");
                TempData.Keep("AcycleIdList");
                TempData.Keep("ARouteIdList");
                TempData.Keep("A_AddressIdList");

                bool result = getdata();
                if (result)
                {
                    List<STP_GetReportTableNew_bak_Result> demo1 = (List<STP_GetReportTableNew_bak_Result>)TempData["AuditList"];
                    TempData.Keep("AuditList");
                    demo1 = demo1.OrderBy(a => a.Dates).ToList();
                    var jsonResult = Json(demo1, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                else
                {
                    List<STP_GetReportTableNew_bak_Result> demo1 = (List<STP_GetReportTableNew_bak_Result>)TempData["AuditList"];
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
            List<string> AuditorId = null;
            ProjectId = long.Parse(Convert.ToString(TempData["AProjectId"]));
            if (TempData["AuditorIdList"] != null)
            {
                AuditorId = (List<string>)TempData["AuditorIdList"];
            }

            fromDate = Convert.ToString(TempData["AfromDate"]);
            Todate = Convert.ToString(TempData["ATodate"]);

            if (TempData["AcycleIdList"] != null)
            {
                cycleId = (List<string>)TempData["AcycleIdList"];
            }
            if (TempData["ARouteIdList"] != null)
            {
                RouteId = (List<string>)TempData["ARouteIdList"];
            }
            if (TempData["A_AddressIdList"] != null)
            {
                AdressId = (List<string>)TempData["A_AddressIdList"];
            }
            if (TempData["AStatusList"] != null)
            {
                StatusList = (List<string>)TempData["AStatusList"];
            }



            TempData.Keep("AProjectId");
            TempData.Keep("AuditorIdList");
            TempData.Keep("AfromDate");
            TempData.Keep("ATodate");
            TempData.Keep("AcycleIdList");
            TempData.Keep("ARouteIdList");
            TempData.Keep("A_AddressIdList");
            TempData.Keep("AStatusList");
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


                        Result1 = new AuditReportDAL().GetReportsRecordsNew_bak(ProjectId, null, null);
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
                        demo = new AuditReportDAL().GetReportsRecordsNew_bak(ProjectId, fd1, to1);
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
                    if (AuditorId != null)
                    {
                        demo = demo.Where(a => AuditorId.Contains(Convert.ToString(a.AuditorId))).ToList();

                    }
                }
                else    // If visit status not selected
                {
                    var Result1 = new AuditReportDAL().GetReportsRecordsNew_bak(ProjectId, null, null);
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
                    if (AuditorId != null)
                    {
                        demo = demo.Where(a => AuditorId.Contains(Convert.ToString(a.AuditorId))).ToList();

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
                // TempData["AuditList"] = demo;
                //   list.AddRange(list);
                TempData["AuditList"] = demo1;
                TempData.Keep("AuditList");
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
        public ActionResult Create()
        {

            var objPermissionCheck = UserPermissionCheck.checkUserStatus("AuditorReports", "Edit");

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
        public ActionResult TransferBackToInstaller(AuditorAllocationData _objAuditorAllocation, string uploadedId, string projectId, string AuditerId)

        {
            bool success = false;
            string _OutResult = "Invalid request";
            AuditReportDAL objAuditReportDAL = new AuditReportDAL();

            try
            {
                long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());

                // Call DAL method to handle reallocation back to installer
                success = objAuditReportDAL.TransferBackToInstaller(_objAuditorAllocation, currentUserId, out _OutResult, uploadedId, AuditerId);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, returnMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = success, returnMsg = _OutResult }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DisplayGetListData(int Id)
        {

            List<FieldDDLMaster> FieldDllMaster = new List<FieldDDLMaster>();
            var FieldData = Session["A_Field"];
            foreach (var item in (dynamic)(FieldData))
            {
                IEnumerable<dynamic> fd = item.FieldList;
                var list = fd.Where(o => o.fieldId == Id);
                foreach (var item1 in list)
                {
                    var initalFiledDetaisl = item1.fieldInitialDataList;

                    foreach (var details in initalFiledDetaisl)
                    {

                        FieldDllMaster.Add(new FieldDDLMaster()
                        {
                            Id = details.initialFieldId,
                            Fieldvalue = details.initialFieldValue,
                            FieldText = details.initialFieldValue
                        });
                    }
                }

            }
            return Json(new { FieldDllMaster = FieldDllMaster }, JsonRequestBehavior.AllowGet);
        }

        //string services;
        [HttpPost]
        public ActionResult SaveData(FormCollection form)
        {

            //CompassEntities cmp = new CompassEntities();

            long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());

            try
            {
                newReportEditBO obj1 = (newReportEditBO)Session["A_objnewReportEditBO"];


                AuditReportDAL objAuditReportDAL = new AuditReportDAL();

                long accountinifiledId = Convert.ToInt64(Session["A_accountinifiledId"]);
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

                var resultList = objAuditReportDAL.saveHistory(obj1.reportFormId, obj1.accountinifiledId, obj1.userId1, currentUserId);

                bool result = objAuditReportDAL.saveReportEdit(obj1.reportFormId, obj1.accountinifiledId, strFieldNameList, strFieldValueList, strServiceList, strAddServiceList);


                return Json(new { result = result, message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult SectionDisplay()
        {
            newReportEditBO obj1 = (newReportEditBO)Session["A_objnewReportEditBO"];


            List<FromDetails> ReportName = new List<FromDetails>();
            List<SectionList> Ids = new List<SectionList>();
            // long accountinifiledId = accountinifiledId;

            var Details = new AuditReportDAL().detailsOfProjetcs(obj1.userId1, obj1.projectId1, obj1.reportFormId);
            Session["A_Details"] = Details;

            Session["A_accountinifiledId"] = obj1.accountinifiledId;
            MobileDAL objMobile = new MobileDAL();

            var result = objMobile.getFormListDataNew(obj1.userId1, obj1.reportFormId, obj1.accountinifiledId);
            Session["A_Field"] = result.FormNameList[0].SectionList;

            var FieldData = result.FormNameList[0].SectionList;

            foreach (var item in (dynamic)(FieldData))
            {
                Ids.Add(new SectionList() { SectionId = item.sectionId, SectionName = item.sectionName });
            }
            return Json(new { Ids = Ids }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult getNextPreviousRedirectData(bool _valRecord)
        {

            List<STP_GetReportTableNew_bak_Result> demo1 = (List<STP_GetReportTableNew_bak_Result>)TempData["AuditList"];
            TempData.Keep("AuditList");
            demo1 = demo1.OrderBy(o => o.Dates).ToList();
            newReportEditBO obj1 = (newReportEditBO)Session["A_objnewReportEditBO"];

            STP_GetReportTableNew_bak_Result obj = new STP_GetReportTableNew_bak_Result();
            if (_valRecord)
            {
                var obj2 = demo1.Where(o => o.ProjectId == obj1.projectId1 && o.ID == obj1.accountinifiledId && o.AuditorId == obj1.userId1 && o.FormId == obj1.reportFormId).FirstOrDefault();

                int index = demo1.IndexOf(obj2);
                if (index + 1 < demo1.Count)
                {
                    obj = demo1[(index + 1)];
                }
                else
                {
                    obj = null;
                }
            }
            else
            {
                var obj2 = demo1.Where(o => o.ProjectId == obj1.projectId1 && o.ID == obj1.accountinifiledId && o.AuditorId == obj1.userId1 && o.FormId == obj1.reportFormId).FirstOrDefault();
                int index = demo1.IndexOf(obj2);
                if (index - 1 >= 0)
                {
                    obj = demo1[(index - 1)];
                }
                else
                {
                    obj = null;
                }
            }

            if (obj != null)
            {
                newReportEditBO objnewReportEditBO = new newReportEditBO();
                objnewReportEditBO.userId1 = Convert.ToInt64(obj.AuditorId);
                objnewReportEditBO.projectId1 = int.Parse(obj.ProjectId.ToString());
                objnewReportEditBO.accountinifiledId = obj.ID;
                objnewReportEditBO.reportFormId = Convert.ToInt64(obj.FormId);
                Session["A_objnewReportEditBO"] = objnewReportEditBO;

                var jsonResult = getHtmlData();
                return jsonResult;
            }
            else
            {
                var jsonResult = Json(new { htmlData = "", Details1 = "" }, JsonRequestBehavior.AllowGet);
                return jsonResult;
            }

        }

        [HttpGet]
        public ActionResult getHtmlData()
        {
            #region
            try
            {


                int i = 1;
                StringBuilder htmlData = new StringBuilder();

                newReportEditBO obj1 = (newReportEditBO)Session["A_objnewReportEditBO"];


                List<FromDetails> ReportName = new List<FromDetails>();
                List<SectionList> Ids = new List<SectionList>();
                // long accountinifiledId = accountinifiledId;

                var Details = new AuditReportDAL().detailsOfProjetcs(obj1.userId1, obj1.projectId1, obj1.reportFormId);
                Session["A_Details"] = Details;

                Session["A_accountinifiledId"] = obj1.accountinifiledId;
                MobileDAL objMobile = new MobileDAL();

                var result = objMobile.getFormListDataNew(obj1.userId1, obj1.reportFormId, obj1.accountinifiledId);
                Session["A_Field"] = result.FormNameList[0].SectionList;

                var objAttachmentRecords = new AuditReportDAL().getAudioVIdeoList(obj1.userId1, obj1.reportFormId, obj1.accountinifiledId);

                var FieldData = result.FormNameList[0].SectionList;

                int SectionCount = result.FormNameList[0].SectionList.Count;
                int currentSectionNumber = 1;
                var servicedata = new AuditReportDAL().getServiceList();

                //AddedBy AniketJ on 18-Oct-2016 to get projectFieldData
                // var ProjectFieldProperty = new AuditReportDAL().ProjectFieldPropertyList(obj1.reportFormId, obj1.projectId1);


                foreach (var item in FieldData)
                {
                    if (currentSectionNumber % 2 == 1)
                    {
                        htmlData.Append("<div class='row' style='background-color:white;'>");
                    }
                    #region
                    htmlData.Append("<div id=" + i + " class='col-sm-6 col-lg-6 col-xs-12 rep-fulldiv' style='background: white;'><br/><div  style=' text-align: center; background: gray; font-size: 14px; color: white;margin-bottom:8px;'><b>"
                        + item.sectionName + "</b></div>");//open section dive


                    var sectionDate = DispalyRecordsNew(item.sectionId);

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
                            htmlData.Append("<br/><label id='" + item2.FieldId + "l' class='col-lg-1' style='color: red'></label>");
                            htmlData.Append("</div>");
                            #endregion
                        }
                        else if (item2.FielddatatypeNames == "SinglePhotoAndGallery" || item2.FielddatatypeNames == "MultiPhotoAndGallery")
                        {
                            #region

                            //gets id for the current field from field data master table.
                            long _fieldId = new AuditReportDAL().getFieldId(obj1.userId1, obj1.reportFormId, obj1.accountinifiledId, item2.FieldName);


                            string ImageAvilable = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 3 && o.FK_ProjectAuditFieldDataMaster_ID == _fieldId).Select(o => o.StrFilePath).FirstOrDefault());
                            ImageAvilable = string.IsNullOrEmpty(ImageAvilable) ? "btn btn-danger btn-xs" : "btn btn-success btn-xs";



                            htmlData.Append("<div class='col-lg-12 drp-div1'>");
                            htmlData.Append("<div class='col-lg-6 col-sm-12 fld-lab2' style='text-align:left'>" + item2.FieldName + requiredicon + " </div>");
                            htmlData.Append("<div  class='col-lg-5 col-sm-12 '>" +
                               " <a href='#' style='margin:3px 0 0 0' data-backdrop='static' data-toggle='modal' id='" + item2.FieldId + "'" +
                            "onclick='reportFieldImages(" + _fieldId + ",\"" + item2.FieldName + "\"," + item2.FieldId + ");' name='reportField' class='" + ImageAvilable + "' role='button' title='Picture'>" +
                            "<i class='fa fa-camera'></i></a>");
                            htmlData.Append("</div></div>");
                            #endregion
                        }

                        else if (item2.FielddatatypeNames == "Location/GPS")
                        {
                            #region
                            var serviceVal = new AuditReportDAL().getFieldValue(obj1.userId1, obj1.reportFormId, obj1.accountinifiledId, item2.FieldName);
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
                            var serviceVal = new AuditReportDAL().getFieldValue(obj1.userId1, obj1.reportFormId, obj1.accountinifiledId, item2.FieldName);
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

                                var serviceVal = new AuditReportDAL().getFieldValue(obj1.userId1, obj1.reportFormId, obj1.accountinifiledId, "SERVICES");
                                string _ser = serviceVal == null ? "" : serviceVal.FieldValue;
                                var list = item.FieldList.Where(o => o.fieldId == item2.FieldId).Select(o => o.fieldInitialDataList).FirstOrDefault();
                                string[] serviceList = (!string.IsNullOrEmpty(_ser) ? _ser.Split('|').ToArray() : null);
                                string serviceBase = "";
                                foreach (var item3 in list)
                                {
                                    string strVideoFilePath = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 2 && o.IsServiceData == true && o.Fk_tblService_Id == item3.initialFieldId).Select(o => o.StrFilePath).FirstOrDefault());
                                    string strAudioFilePath = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 1 && o.IsServiceData == true && o.Fk_tblService_Id == item3.initialFieldId).Select(o => o.StrFilePath).FirstOrDefault());

                                    string Video_AttachmentID = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 2
                                      && o.IsServiceData == true && o.Fk_tblService_Id == item3.initialFieldId).Select(o => o.ID).FirstOrDefault());

                                    string Audio_AttachmentID = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 1
                                          && o.IsServiceData == true && o.Fk_tblService_Id == item3.initialFieldId).Select(o => o.ID).FirstOrDefault());

                                    string ImageAvilable = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 3 && o.IsServiceData == true && o.Fk_tblService_Id == item3.initialFieldId).Select(o => o.StrFilePath).FirstOrDefault());
                                    ImageAvilable = string.IsNullOrEmpty(ImageAvilable) ? "btn btn-danger btn-xs" : "btn btn-success btn-xs";


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
                                    serviceBase = serviceBase + "<div class='col-lg-3 col-sm-3 col-xs-3' style='margin-left:10%'> <input style='margin:14px 0px 0px -6px;pointer-events: none;opacity: 0.4;'  type='checkbox' name = 'S_" + item3.initialFieldValue + "' id='" + item3.initialFieldId + "' " + (result1 == null ? "" : "checked") + "> </div>";

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
                                var serviceVal = new AuditReportDAL().getFieldValue(obj1.userId1, obj1.reportFormId, obj1.accountinifiledId, "ADDITIONAL SERVICES");
                                var list = item.FieldList.Where(o => o.fieldId == item2.FieldId).Select(o => o.fieldInitialDataList).FirstOrDefault();
                                string[] serviceList = (!string.IsNullOrEmpty(serviceVal.FieldValue) ? serviceVal.FieldValue.Split('|').ToArray() : null);

                                string serviceBase = "";
                                foreach (var item3 in list)
                                {
                                    string strVideoFilePath = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 2
                                        && o.IsServiceData == true && o.Fk_tblService_Id == item3.initialFieldId).Select(o => o.StrFilePath).FirstOrDefault());
                                    string strAudioFilePath = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 1
                                        && o.IsServiceData == true && o.Fk_tblService_Id == item3.initialFieldId).Select(o => o.StrFilePath).FirstOrDefault());

                                    string Video_AttachmentID = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 2
                                          && o.IsServiceData == true && o.Fk_tblService_Id == item3.initialFieldId).Select(o => o.ID).FirstOrDefault());

                                    string Audio_AttachmentID = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 1
                                          && o.IsServiceData == true && o.Fk_tblService_Id == item3.initialFieldId).Select(o => o.ID).FirstOrDefault());


                                    string ImageAvilable = Convert.ToString(objAttachmentRecords.Where(o => o.IsAudioVideoImage == 3 && o.IsServiceData == true && o.Fk_tblService_Id == item3.initialFieldId).Select(o => o.StrFilePath).FirstOrDefault());
                                    ImageAvilable = string.IsNullOrEmpty(ImageAvilable) ? "btn btn-danger btn-xs" : "btn btn-success btn-xs";

                                    if (!string.IsNullOrEmpty(strVideoFilePath))
                                    {
                                        //  strVideoFilePath = "" + strVideoFilePath.Replace("..\\..\\", _webUrl);
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
                                    serviceBase = serviceBase + "<div class='col-lg-3 col-sm-3 col-xs-3' style='margin-left:10%'> <input style='margin:14px 0px 0px -6px;pointer-events: none;opacity: 0.4;'  type='checkbox' name = 'A_" + item3.initialFieldValue + "' id='" + item3.initialFieldId + "' " + (result1 == null ? "" : "checked") + "> </div>";

                                    if (result1 != null)
                                    {
                                        serviceBase = serviceBase + "<div class='col-lg-3 col-sm-4 col-xs-4 rep-imgbtn2'style='margin-left:-13%;' ><a href='#' id='IMS_" + item3.initialFieldId + "' onclick='reportServiceFieldImages(" + serviceVal.ID + "," + item3.initialFieldId + ",\"" + item3.initialFieldValue + "\"," + item2.FieldId + ", true)' style='margin:9px 4px 0 0px' data-backdrop='static' data-toggle='modal' class='" + ImageAvilable + "' name='additionalservice'  role='button' title='Picture'><i class='fa fa-camera'></i></a>";
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
                                        if (_initialList.Count > 0)
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
                                        }
                                        else
                                        {
                                            if (item2.FieldName.ToLower() == "new meter radio" || item2.FieldName.ToLower() == "new meter number")
                                            {
                                                var ddlData = getDropdownData(obj1.reportFormId, obj1.accountinifiledId, item2.FieldName);
                                                if (ddlData != null)
                                                {
                                                    htmlData.Append("<option value='" + ddlData.FieldValue + "'>" + ddlData.FieldValue + "</option>");
                                                    //htmlData.Append("<option value='" + ddlData.FieldValue + "' selected='selected'>" + ddlData.FieldValue + "</option>");
                                                }
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
                var jsonResult = Json(new { status = true, htmlData = _result, Details1 = Details, obj1 = obj1 }, JsonRequestBehavior.AllowGet);
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
        public ProjectAuditFieldDataMaster getDropdownData(long formId, long UploadedExcelDataId, string initialFieldName)
        {
            try
            {


                CompassEntities dbcontext = new CompassEntities();
                ProjectAuditFieldDataMaster data = (from m in dbcontext.ProjectAuditDatas
                                                    join n in dbcontext.ProjectAuditFieldDataMasters on m.ID equals n.FK_ProjectAuditData
                                                    where m.FormId == formId && m.FK_UploadedExcelData_Id == UploadedExcelDataId
                                                    && n.InitialFieldName.ToLower() == initialFieldName.ToLower()
                                                    select n).FirstOrDefault();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }

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

            var details = new AuditReportDAL().getFieldImageData(initialFieldId);
            string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);
            int i = 1;
            foreach (var item in details)
            {
                string strFilePath = item.StrFilePath;
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
                    htmlData.Append("<img class='imgRotateFlip' style='display: inline;' id='" + item.ID + "' src=\"" + strFilePath + "\">");
                }
                else
                {
                    htmlData.Append("<img class ='imgRotateFlip' style='display: none; ' id='" + item.ID + "' src=\"" + strFilePath + "\">");
                }
                i++;


            }
            string _result = htmlData.ToString();
            var jsonResult = Json(new { htmlData = _result }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        [HttpGet]
        public ActionResult GetAllHtmlImagelink(long FieldDataMasterId)
        {
            StringBuilder htmlData = new StringBuilder();

            var details = new AuditReportDAL().getFieldImageData(FieldDataMasterId);
            string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);
            //int i = 1;

            List<ImageSliderList> objList = new List<Model.BO.ImageSliderList>();
            foreach (var item in details)
            {
                string strFilePath = item.StrFilePath;
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


            }



            string _result = htmlData.ToString();
            var jsonResult = Json(new { ImageList = objList }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }



        [HttpGet]
        public ActionResult SetGLocation()  //This code not use because partial view don't use because partial view Map Street view not working
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



        [HttpGet]
        public ActionResult getHtmlImagelinkForServices(long initialFieldId, long serviceId)
        {
            StringBuilder htmlData = new StringBuilder();

            var details = new AuditReportDAL().getServiceFieldImageData(initialFieldId, serviceId);
            string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);
            //    _webUrl = "https://localhost:6069/";
            int i = 1;

            //AddedBy AniketJ on 25-Oct-2016
            List<string> pictureList = new AuditReportDAL().GetPictureListData(serviceId);

            foreach (var item in details)
            {
                string strFilePath = item.StrFilePath;
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
                if (i == 1)
                {
                    htmlData.Append("<img class='imgRotateFlip' style='display: inline;' id='" + item.ID + "' src=\"" + strFilePath + "\">");
                }
                else
                {
                    htmlData.Append("<img class ='imgRotateFlip' style='display: none; ' id='" + item.ID + "' src=\"" + strFilePath + "\">");
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

            var details = new AuditReportDAL().getServiceFieldImageData(reportFieldId, serviceId);
            string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);
            //    _webUrl = "https://localhost:6069/";


            List<ImageSliderList> objList = new List<Model.BO.ImageSliderList>();

            //AddedBy AniketJ on 25-Oct-2016
            List<string> pictureList = new ReportDAL().GetPictureListData(serviceId);

            foreach (var item in details)
            {
                string strFilePath = item.StrFilePath;
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



            }
            string _result = htmlData.ToString();

            var jsonResult = Json(new { ImageList = objList, PictureList = pictureList }, JsonRequestBehavior.AllowGet);
            // var jsonResult = Json(new { htmlData = _result, PictureList = pictureList }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }






        [HttpPost]
        public void UploadImageFile(long Id)
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];

                var fileName = Path.GetFileName(file.FileName);

                var path = Path.Combine(Server.MapPath("~/MobileUploadDataTemp/"), fileName);
                file.SaveAs(path);
            }

        }


        public ActionResult DispalyRecords()
        {
            var FieldData = (List<MobileSectionBO>)Session["A_Field"];
            List<SectionList> Ids = new List<SectionList>();
            foreach (var item in (dynamic)(FieldData))
            {
                Ids.Add(new SectionList() { SectionId = item.sectionId, SectionName = item.sectionName });
            }
            return Json(new { Ids = Ids }, JsonRequestBehavior.AllowGet);

        }


        string initialName;
        string initialval;
        long? initialId;
        int count = 1;
        string sectionName;
        long SectionId;

        [HttpGet]
        public ActionResult DispalyRecords(long sectionIds)
        {
            CompassEntities cmp = new CompassEntities();
            long accountinifiledId = Convert.ToInt64(Session["A_accountinifiledId"]);
            List<FieldNames> Field = new List<FieldNames>();
            List<DetailsAll> detals = new List<DetailsAll>();
            var Details = (DetailsAll)Session["A_Details"];

            detals.Add(new DetailsAll()
            {
                FromName = Details.FromName,
                ProjectName = Details.ProjectName,
                InstallerName = Details.InstallerName
            });

            var FieldData = Session["A_Field"];
            foreach (var item in (dynamic)(FieldData))
            {
                sectionName = item.sectionName;
                SectionId = item.sectionId;
                Session["A_DataList"] = item.FieldList;
                IEnumerable<dynamic> fd = item.FieldList;
                var list = fd.Where(o => o.sectionId == sectionIds).ToList();
                foreach (var item1 in list)
                {
                    string name = item1.fieldName;
                    var id = cmp.TblProjectFieldDatas.Where(o => o.tblUploadedData_Id == accountinifiledId).Select(o => o.ID).FirstOrDefault();
                    var detailsdata = cmp.tblProjectFieldDataMasters.Where(o => o.TblProjectFieldData_Id == id && o.InitialFieldName == name).ToList();
                    var itemtext = item1.fieldInitialDataList;

                    foreach (var details in detailsdata)
                    {
                        initialId = details.InitialFieldId;
                        initialval = details.InitialFieldName;
                        initialName = details.FieldValue;
                    }

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
                        initialId = initialId,
                        FieldText = initialName,
                        Fieldvalue = initialval,
                        Count = count,
                        SectionId = Convert.ToInt64(SectionId),
                        SectionName = sectionName

                    });

                }
                count++;
            }
            //List<FieldNames> Field = new List<FieldNames>();
            // Field = new AuditReportDAL().FieldDetails(Id);
            return Json(new { Field = Field, detals = detals }, JsonRequestBehavior.AllowGet);
        }


        public List<FieldNames> DispalyRecordsNew(long sectionIds)
        {
            //CompassEntities cmp = new CompassEntities();

            newReportEditBO obj1 = (newReportEditBO)Session["A_objnewReportEditBO"];

            ProjectAuditData objProjectAuditData;
            List<ProjectAuditFieldDataMaster> objListProjectAuditFieldDataMaster;

            new AuditReportDAL().GetReportEditData(obj1.accountinifiledId, obj1.reportFormId, obj1.userId1, out objProjectAuditData, out objListProjectAuditFieldDataMaster);


            long accountinifiledId = Convert.ToInt64(Session["A_accountinifiledId"]);
            List<FieldNames> Field = new List<FieldNames>();
            List<DetailsAll> detals = new List<DetailsAll>();


            var FieldData = Session["A_Field"];
            foreach (var item in (dynamic)(FieldData))
            {
                sectionName = item.sectionName;
                SectionId = item.sectionId;
                Session["A_DataList"] = item.FieldList;
                IEnumerable<dynamic> fd = item.FieldList;
                var list = fd.Where(o => o.sectionId == sectionIds).ToList();
                foreach (var item1 in list)
                {
                    string name = item1.fieldName;

                    var detailsdata = objListProjectAuditFieldDataMaster.Where(o => o.InitialFieldName == name).FirstOrDefault();
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

            return Field;
        }


        public ActionResult RemoveImage(int imagesID)
        {
            CompassEntities cmp = new CompassEntities();
            var data = cmp.ProjectAuditFieldDataAttachments.Where(o => o.ID == imagesID && o.Active == 1).ToList();
            foreach (var item in data)
            {
                item.Active = 0;
            }
            UpdateModel(data);
            cmp.SaveChanges();
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetImage(long initialId)
        {
            List<ProjectFildDataTypeMaster> FieldDataAttach = new List<ProjectFildDataTypeMaster>();
            var list = new AuditReportDAL().ProjectFildDataTypeMasters(initialId);
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

        [HttpPost]
        public ActionResult Upload()
        {
            CompassEntities cmp = new CompassEntities();
            long accountinifiledId1 = Convert.ToInt64(Session["A_accountinifiledId"]);
            string projectId = ""; string AccountNumber = "";
            string formId = ""; string accountInitialFieldId = "";
            string imageName = "";

            var data = cmp.tblUploadedDatas.Where(o => o.ID == accountinifiledId1).FirstOrDefault();
            var data1 = cmp.TblProjectFieldDatas.Where(o => o.tblUploadedData_Id == accountinifiledId1).FirstOrDefault();

            projectId = Convert.ToString(data.ProjectId);
            AccountNumber = data.Account;
            formId = Convert.ToString(data1.FormId);
            imageName = "CAMERA IMAGE 1";
            accountInitialFieldId = Convert.ToString(accountinifiledId1);
            var projectdatamasterId = cmp.tblProjectFieldDataMasters.Where(o => o.TblProjectFieldData_Id == data1.ID).FirstOrDefault();
            long projectdatamasterIds = Convert.ToInt64(projectdatamasterId.ID);
            int InstallerId = (int)data1.InstallerId;

            string ServerUploadFolder = Convert.ToString(ConfigurationManager.AppSettings["UploadFilePath"]);
            string tempPath = ServerUploadFolder + projectId + @"\" + formId + @"\" +
                        AccountNumber + @"\" + accountInitialFieldId + @"\" + imageName + @"\";
            TblProjectFieldDataAttachment tblpa = new TblProjectFieldDataAttachment();
            tblpa.IsSkipped = false;
            tblpa.tblProjectFieldDataMaster_ID = projectdatamasterIds;
            tblpa.IsAudioVideoImage = 3;
            for (int i = 0; i < Request.Files.Count; i++)
            {
                Directory.CreateDirectory(Server.MapPath(tempPath));
                var file = Request.Files[i];
                var fileName = Path.GetFileName(file.FileName);
                var path1 = Path.Combine(Server.MapPath(tempPath), fileName);
                file.SaveAs(path1);
                tblpa.strFilePath = tempPath + @"\" + fileName;
            }
            tblpa.isServiceData = true;
            //tblpa.tblService_Id = item.tblService_Id;
            // tblpa.TblServicePicture_ID = item.TblServicePicture_ID;
            tblpa.Active = 1;
            tblpa.CreatedBy = InstallerId;
            tblpa.CreatedOn = DateTime.Now;
            cmp.TblProjectFieldDataAttachments.Add(tblpa);
            cmp.SaveChanges();
            return RedirectToAction("Create", "Reports");
            //return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetpicName(int imagesID)
        {
            CompassEntities cmp = new CompassEntities();
            var searvicesName = cmp.TblProjectFieldDataAttachments.Where(o => o.ID == imagesID && o.Active == 1).Select(o => o.TblServicePicture_ID).FirstOrDefault();
            var tblservicepicId = cmp.TblServicePictures.Where(o => o.ID == searvicesName).Select(o => o.tblPicture_PictureId).FirstOrDefault();
            var nameOfpic = cmp.tblPictures.Where(o => o.ID == tblservicepicId).Select(o => o.Picture).FirstOrDefault();
            return Json(new { nameOfpic = nameOfpic }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFieldservicesImages(long initialId, int Id)
        {
            CompassEntities cmp = new CompassEntities();
            long accountinifiledId = Convert.ToInt64(Session["A_accountinifiledId"]);
            long projectdataID = cmp.TblProjectFieldDatas.Where(o => o.tblUploadedData_Id == accountinifiledId).Select(o => o.ID).FirstOrDefault();
            var servicesId = cmp.tblProjectFieldDataMasters.Where(o => o.TblProjectFieldData_Id == projectdataID && o.InitialFieldName == "SERVICES").FirstOrDefault();
            // var projectFiledId = cmp.tblProjectFieldDataMasters.Where(o => o.InitialFieldId == initialId && o.TblProjectFieldData_Id == projectdataID).Select(o => o.ID).FirstOrDefault();
            long ids = servicesId.ID;
            var searvicesName = cmp.TblProjectFieldDataAttachments.Where(o => o.tblProjectFieldDataMaster_ID == ids && o.tblService_Id == Id && o.Active == 1).ToList();
            var servicesnamse1 = cmp.tblServices.Where(o => o.ID == Id).Select(o => o.Service).FirstOrDefault();
            List<ImageandServicesName> namses = new List<ImageandServicesName>();
            namses.Add(new ImageandServicesName { SeravicesName = servicesnamse1 });
            return Json(new { searvicesName = searvicesName, namses = namses }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFieldImages(long initialId, string FieldName)
        {
            List<ProjectFildDataTypeMaster> FieldDataAttach = new List<ProjectFildDataTypeMaster>();
            //var projectFiledDataId = new AuditReportDAL().projectFiledDataId(initialId);
            //foreach (var item in projectFiledDataId)
            //{
            var list = new AuditReportDAL().ProjectFildDataTypeMaster(initialId, FieldName);
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
            //FieldDataAttach.Add(new AuditReportDAL().ProjectFildDataTypeMaster(item.ID, FieldName));
            // }
            return Json(new { FieldDataAttach = FieldDataAttach }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DispalyRecords1(int Id)
        {

            List<FieldProperty> Fieldproperty = new List<FieldProperty>();
            Fieldproperty = new AuditReportDAL().GetAllFieldproperty(Id);
            return Json(new { Fieldproperty = Fieldproperty }, JsonRequestBehavior.AllowGet);
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
        public ActionResult EditRecords(long userId1, int projectId1, long accountinifiledId, long reportFormId)
        {
            //List<FromDetails> ReportName = new List<FromDetails>();
            //List<SectionList> Ids = new List<SectionList>();
            //// long accountinifiledId = accountinifiledId;

            //var Details = new AuditReportDAL().detailsOfProjetcs(userId1, projectId1, reportFormId);
            //Session["A_Details"] = Details;

            //Session["A_accountinifiledId"] = accountinifiledId;
            //MobileDAL objMobile = new MobileDAL();

            //var result = objMobile.getFormListDataNew(userId1, reportFormId, accountinifiledId);
            //Session["A_Field"] = result.FormNameList[0].SectionList;
            //return Json(new { msg = true }, JsonRequestBehavior.AllowGet);
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("AuditorReports", "Edit");

             if (objPermissionCheck)
             {
                 newReportEditBO obj1 = new newReportEditBO();
                 obj1.userId1 = userId1;
                 obj1.projectId1 = projectId1;
                 obj1.accountinifiledId = accountinifiledId;
                 obj1.reportFormId = reportFormId;
                 Session["A_objnewReportEditBO"] = obj1;
                 return Json(new { success = objPermissionCheck, returnMessage = "" }, JsonRequestBehavior.AllowGet);
             }
             else
             {
                 var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                 return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
             }
        }

        public ActionResult Edit(int Id, int userId1, int projectId1, double formDate, double toDate, long FormId)
        {

            List<FromDetails> ReportName = new List<FromDetails>();
            var Details = new AuditReportDAL().detailsOfProjetcs(userId1, projectId1, FormId);
            Session["A_Details"] = Details;
            //foreach (var it in Details)
            //{

            //}
            TempData["A_accountinifiledId"] = Id;
            TempData["A_UserID"] = userId1;
            try
            {
                MobileDAL objMobile = new MobileDAL();
                var result = objMobile.formListAgainstAccount(userId1, projectId1, Id);
                foreach (var item in result)
                {
                    ReportName.Add(new FromDetails() { FormId = Convert.ToInt64(item.formId), FormName = item.formName });
                }
            }
            catch (Exception ex)
            {
                ErrorModelStatus obj2 = new ErrorModelStatus();
                obj2.status = "Error";
                obj2.errorMessage = ex.Message;
            }


            List<SectionList> Ids = new List<SectionList>();


            return Json(new { ReportName = ReportName, Ids = Ids }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetFieldProperty(int FieldId)
        {
            List<FieldProperty> Fieldproperty = new List<FieldProperty>();
            Fieldproperty = new AuditReportDAL().GetAllFieldproperty(FieldId);
            return Json(new { Fieldproperty = Fieldproperty }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetProjectDate(int ProjectID)
        {
            var list = new AuditReportDAL().projectDates(ProjectID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPoject(int ClientID)
        {

            ProjectMasterDAL objProjectMasterDAL = new ProjectMasterDAL();
            var list = objProjectMasterDAL.GetProjectNamebyClientId(ClientID).OrderBy(a => a.ProjectName).Distinct();
            //var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
            //jsonResult.MaxJsonLength = int.MaxValue;
            //return jsonResult;
            return Json(list, JsonRequestBehavior.AllowGet);

        }



        public ActionResult getCurrentInstallerList()
        {

            InstallerDAL objProjectMasterDAL = new InstallerDAL();
            var list = objProjectMasterDAL.GetInstallerList().OrderBy(a => a.UserName).Distinct();
            //var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
            //jsonResult.MaxJsonLength = int.MaxValue;
            //return jsonResult;
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
            //List<ClientModel> objList1 = new List<ClientModel>();
            ////objList1=
            //using (ProjectMasterDAL obj = new ProjectMasterDAL())
            //{
            //    objList1 = obj.GetActiveClientRecords();
            //}

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
        //[HttpPost]
        //public ActionResult GetLat_by_Root(AddressModel AddressModel)//string [] Address)
        //{
        //    InstallerDAL obj = new InstallerDAL();
        //    List<tblUploadedData> data = new List<tblUploadedData>();// obj.GetLat_Lag(Address);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
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
        // string HTMLContent;
        //string AccounNo;
        // decimal? lat, lon;
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
                var list = new AuditReportDAL().GetReportsRecordsID(id, frmID);
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
                            listService = new AuditReportDAL().getServicesWithImages(item.FieldValues, item.IDS, true);
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
                            listService = new AuditReportDAL().getServicesWithImages(item.FieldValues, item.IDS, true);
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


        public ActionResult GetPDFDownloadID(string id, string frmID, string buttonclicked)
        {
            log.Info("in GetPDFDownloadID");
            if (buttonclicked != null && buttonclicked != "")
            {
                TempData["A_buttonclicked"] = buttonclicked;
                TempData.Keep("A_buttonclicked");
            }
            else
            {

                TempData["A_pid"] = Convert.ToInt64(id);
                TempData["A_pfrmid"] = Convert.ToInt64(frmID);
                TempData.Keep("A_pid");
                TempData.Keep("A_pfrmid");
            }

            return base.Json(frmID, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DownloadFile()
        {

            var objPermissionCheck = UserPermissionCheck.checkUserStatus("AuditorReports", "View");

            if (objPermissionCheck)
            {
                try
                {
                    log.Info("in DownloadFile");

                    var pid = Convert.ToInt64(TempData["A_pid"]);
                    var pfrmid = Convert.ToInt64(TempData["A_pfrmid"]);
                    TempData.Keep("A_pid");
                    TempData.Keep("A_pfrmid");
                    var buttonclicked = TempData["A_buttonclicked"];
                    string CategoryType = "";//Added by aniket to export report categorywise

                    bool nextbtn = true;
                    bool prevbtn = true;

                    List<STP_GetReportTableNew_bak_Result> ReportTableList = (List<STP_GetReportTableNew_bak_Result>)TempData["AuditList"];
                    TempData.Keep("AuditList");

                    STP_GetReportTableNew_bak_Result previousrecord = new STP_GetReportTableNew_bak_Result();
                    STP_GetReportTableNew_bak_Result nextrecord = new STP_GetReportTableNew_bak_Result();


                    List<STP_GetReportTableNew_bak_Result> ReportTable = new List<STP_GetReportTableNew_bak_Result>(ReportTableList);
                    ReportTable.Reverse();

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

                            TempData["A_pid"] = next.ID;
                            TempData["A_pfrmid"] = next.FormId;
                            TempData.Keep("A_pid");
                            TempData.Keep("A_pfrmid");

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

                            TempData["A_pid"] = prev.ID;
                            TempData["A_pfrmid"] = prev.FormId;
                            TempData.Keep("A_pid");
                            TempData.Keep("A_pfrmid");

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
                long ID = Convert.ToInt64(TempData["A_pid"]);
                long frmID = Convert.ToInt64(TempData["A_pfrmid"]);
                TempData.Keep("A_pid");
                TempData.Keep("A_pfrmid");
                var list = new AuditReportDAL().GetReportsRecordsID(ID, frmID);

                ReportHeaderModel reportheader = new AuditReportDAL().GetReportHeaderModel(ID); ;
                @ViewBag.FormName = reportheader.FormName;
                @ViewBag.ProjectName = reportheader.ProjectName;
                @ViewBag.ProjectDate = reportheader.ProjectDate;
                @ViewBag.AuditorName = reportheader.AuditorName;

                @ViewBag.UploadID = ID;
                @ViewBag.FormID = frmID;

                string footer = "--header-font-size \"9\" --header-spacing 5 --header-right \"Project name:-" + @ViewBag.ProjectName + " \n\t Auditor name:-" + @ViewBag.AuditorName + " \n\t Date submitted:-" + @ViewBag.ProjectDate + "  \"  --footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
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
                long skipRTU_id = Convert.ToInt64(TempData["A_pid"]);
                long skipRTU_formID = Convert.ToInt64(TempData["A_pfrmid"]);
                TempData.Keep("A_pid");
                TempData.Keep("A_pfrmid");

                ReportSkipRTUModel reportSkipRTUModel = new ReportSkipRTUModel();
                if (TempData["AuditList"] != null)
                {
                    List<STP_GetReportTableNew_bak_Result> reportList = (List<STP_GetReportTableNew_bak_Result>)TempData["AuditList"];
                    TempData.Keep("AuditList");
                    if (reportList.Count > 0)
                    {
                        reportSkipRTUModel = new AuditReportDAL().SKIP_RTU_Records(reportList, skipRTU_id, skipRTU_formID);
                    }
                    ReportHeaderModel reportheader = new AuditReportDAL().GetReportHeaderModel(skipRTU_id); ;
                    @ViewBag.FormName = reportheader.FormName;
                    @ViewBag.ProjectName = reportheader.ProjectName;
                    @ViewBag.ProjectDate = reportheader.ProjectDate;
                    @ViewBag.AuditorName = reportheader.AuditorName;
                }
                string footer = "--header-font-size \"9\" --header-spacing 5 --header-right \"Project name:-" + @ViewBag.ProjectName + " \n\t Auditor name:-" + @ViewBag.AuditorName + " \n\t Date submitted:-" + @ViewBag.ProjectDate + "  \"  --footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
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
                if (TempData["AuditList"] != null)
                {

                    long skipRTU_formID = Convert.ToInt64(TempData["A_pfrmid"]);
                    TempData.Keep("A_pfrmid");

                    List<STP_GetReportTableNew_bak_Result> reportList = (List<STP_GetReportTableNew_bak_Result>)TempData["AuditList"];
                    if (reportList.Count > 0)
                    {
                        TempData.Keep("AuditList");
                        reportSkipRTUModel = new AuditReportDAL().SKIP_RTU_Records(reportList, UploadID, skipRTU_formID);
                    }
                    ReportHeaderModel reportheader = new AuditReportDAL().GetReportHeaderModel(UploadID); ;
                    @ViewBag.FormName = reportheader.FormName;
                    @ViewBag.ProjectName = reportheader.ProjectName;
                    @ViewBag.ProjectDate = reportheader.ProjectDate;
                    @ViewBag.AuditorName = reportheader.AuditorName;

                }
                string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                return new Rotativa.ViewAsPdf("GeneratePDF_SkipRTU", reportSkipRTUModel)
                {
                    CustomSwitches = footer,
                    PageOrientation = Orientation.Portrait,
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
        }
        #endregion

        public ActionResult DownloadSinglePDF(long FormID, long UploadID)
        {
            CompassEntities CompassEntities = new CompassEntities();
            try
            {

                long ID = Convert.ToInt64(TempData["A_pid"]);
                long frmID = Convert.ToInt64(TempData["A_pfrmid"]);
                TempData.Keep("A_pid");
                TempData.Keep("A_pfrmid");

                ID = UploadID;
                frmID = FormID;


                var list = new AuditReportDAL().GetReportsRecordsID(ID, frmID);

                // var listitem = list.GroupBy(o => o.sectionName);
                ReportPDFVM ReportPDFVM = new ReportPDFVM();
                ReportPDFVM.sectionfiledval = list;



                // STP_GetReportHeaderFields_Result reportheader = CompassEntities.STP_GetReportHeaderFields(ID).FirstOrDefault();
                PROC_ReportPDF_Audit_GetReportHeaderFields_Result reportheader = CompassEntities.PROC_ReportPDF_Audit_GetReportHeaderFields(ID).FirstOrDefault();
                @ViewBag.FormName = reportheader.formname;
                @ViewBag.ProjectName = reportheader.projectname;
                @ViewBag.ProjectDate = Convert.ToDateTime(reportheader.visiteddate).ToString("MMM-dd-yyyy hh:mm tt");
                @ViewBag.AuditorName = reportheader.firstname;

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

        #endregion

        #region Generate pdf MultipleIDs

        public ActionResult ExportReportMultipleIDs(List<ReportRow> ReportRowList)
        {

            List<STP_GetReportTableNew_bak_Result> demo1 = new List<STP_GetReportTableNew_bak_Result>();

            demo1 = (List<STP_GetReportTableNew_bak_Result>)TempData["AuditList"];
            TempData.Keep("AuditList");

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

            TempData["A_ReportRowList"] = obj;

            TempData.Keep("A_ReportRowList");

            return base.Json(new { Status = status }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DownloadMultipleFile()
        {
            try
            {
                List<ReportRow> ReportRowList = TempData["A_ReportRowList"] as List<ReportRow>;
                TempData.Keep("A_ReportRowList");

                List<ReportPDFVM> Recordlist = new List<ReportPDFVM>();

                foreach (var ReportRow in ReportRowList)
                {
                    ReportPDFVM reportPDFVM = new ReportPDFVM();
                    ReportHeaderModel reportheader = new AuditReportDAL().GetReportHeaderModel(ReportRow.UploadedID);
                    reportPDFVM.FormName = reportheader.FormName;
                    reportPDFVM.ProjectName = reportheader.ProjectName;
                    reportPDFVM.VisitedDate = reportheader.ProjectDate;
                    reportPDFVM.AuditorName = reportheader.AuditorName;
                    reportPDFVM.CategoryType = ReportRow.CategoryType;

                    //Modified by AniketJ on 2-sep-2016 added RTU/SKIP records in ReportPDFVM to export
                    if (ReportRow.CategoryType == "COMPLETED")
                    {
                        var list = new AuditReportDAL().GetReportsRecordsID(ReportRow.UploadedID, ReportRow.FormID);
                        reportPDFVM.sectionfiledval = list;
                        reportPDFVM.FormID = ReportRow.FormID;
                        reportPDFVM.UploadID = ReportRow.UploadedID;

                    }
                    else if (ReportRow.CategoryType == "RTU" || ReportRow.CategoryType == "SKIP")
                    {
                        List<STP_GetReportTableNew_bak_Result> reportList = (List<STP_GetReportTableNew_bak_Result>)TempData["AuditList"];
                        TempData.Keep("AuditList");
                        var reportSkipRTUModel = new AuditReportDAL().SKIP_RTU_Records(reportList, ReportRow.UploadedID, ReportRow.FormID);
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

                List<ReportRow> ReportRowList = TempData["A_ReportRowList"] as List<ReportRow>;
                CompassEntities CompassEntities = new CompassEntities();

                TempData.Keep("A_ReportRowList");

                List<ReportPDFVM> Recordlist = new List<ReportPDFVM>();

                foreach (var ReportRow in ReportRowList)
                {

                    var list = new AuditReportDAL().GetReportsRecordsID(ReportRow.UploadedID, ReportRow.FormID);

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
                List<ReportRow> ReportRowList = TempData["A_ReportRowList"] as List<ReportRow>;
                CompassEntities CompassEntities = new CompassEntities();

                TempData.Keep("A_ReportRowList");

                List<ReportPDFVM> Recordlist = new List<ReportPDFVM>();

                foreach (var ReportRow in ReportRowList)
                {

                    ReportPDFVM reportPDFVM = new ReportPDFVM();
                    ReportHeaderModel reportheader = new AuditReportDAL().GetReportHeaderModel(ReportRow.UploadedID);
                    reportPDFVM.FormName = reportheader.FormName;
                    reportPDFVM.ProjectName = reportheader.ProjectName;
                    reportPDFVM.VisitedDate = reportheader.ProjectDate;
                    reportPDFVM.AuditorName = reportheader.AuditorName;
                    reportPDFVM.CategoryType = ReportRow.CategoryType;

                    //Modified by AniketJ on 2-sep-2016 added RTU/SKIP records in ReportPDFVM to export
                    if (ReportRow.CategoryType == "COMPLETED")
                    {
                        var list = new AuditReportDAL().GetReportsRecordsID(ReportRow.UploadedID, ReportRow.FormID);
                        reportPDFVM.FormID = ReportRow.FormID;
                        reportPDFVM.UploadID = ReportRow.UploadedID;
                        reportPDFVM.sectionfiledval = list;
                    }
                    else if (ReportRow.CategoryType == "RTU" || ReportRow.CategoryType == "SKIP")
                    {
                        List<STP_GetReportTableNew_bak_Result> reportList = (List<STP_GetReportTableNew_bak_Result>)TempData["AuditList"];
                        TempData.Keep("AuditList");
                        var reportSkipRTUModel = new AuditReportDAL().SKIP_RTU_Records(reportList, ReportRow.UploadedID, ReportRow.FormID);
                        reportPDFVM.skipRtuRecordModel = reportSkipRTUModel;
                    }
                    Recordlist.Add(reportPDFVM);
                }
                string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";

                return new Rotativa.ViewAsPdf("GenerateMultiplePDF", Recordlist)
                {
                    FileName = "ProjectReport.pdf",
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

        public ActionResult reOpenRecord(long uploadId, long installerId, long formId, long reOpenId, string reOpenComment)
        {
            try
            {
                AuditReportDAL objAuditReportDAL = new AuditReportDAL();
                bool result = objAuditReportDAL.reOpenRecords(uploadId, installerId, formId, reOpenId, reOpenComment);

                #region Added code by vis o send mail instaler when issue reopen
                if (result)
                {
                    List<String> mailto = new List<String>();
                    CompassEntities db = new CompassEntities();
                    //  string Subject = (db.tblSkipReasonMasters.Where(n => n.SkipId == reOpenId).FirstOrDefault().SkipReason);
                    var UserEmailId = (db.tblUsers.Where(n => n.UserID == installerId).FirstOrDefault().Email);
                    List<String> CCEmailTo = objAuditReportDAL.CCEmailTo();
                    string Subject = objAuditReportDAL.GetSubject();
                    Task.Factory.StartNew(() =>
                    {
                        mailto.Add(UserEmailId);
                        // mailto.Add("vishal.takawle@kairee.in");
                        // CCEmailTo.Add("bharat.magdum@kairee.in");
                        new GarronT.CMS.Model.clsMail().SendMail(mailto, CCEmailTo, objAuditReportDAL.SendMalil(reOpenComment, uploadId, installerId, formId), Subject);
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
            catch
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
                AuditReportDAL objAuditReportDAL = new AuditReportDAL();
                string path = objAuditReportDAL.getServiceFieldImagePath(attachmentId);//filePath;
                string orignalPath = path;
                if (!string.IsNullOrEmpty(path))
                {
                    //path = "~\\" + path.Replace("..\\..\\", "");

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

                long newFileId = objAuditReportDAL.saveRotatedImageFilePath(attachmentId, NewFileName);

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
        // public ActionResult UploadImage(string uploadedfrom)   //updatedby AniketJ on 24-oct-2016
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
                AuditReportDAL objAuditReportDAL = new AuditReportDAL();

                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    var stream = fileContent.InputStream;
                    // get a stream
                    //var stream = fileContent.InputStream;
                    // and optionally write the file to disk

                    string path1 = objAuditReportDAL.getServiceFieldImagePath(imagesID);//filePath;\ //  string dsf = HostingEnvironment.MapPath(path1)
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
                            result = objAuditReportDAL.UploadImages(newFileDBPath, imagesID, out returnMessage, out newAttachmentId, CreatedBy);

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
                AuditReportDAL objAuditReportDAL = new AuditReportDAL();

                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    var stream = fileContent.InputStream;

                    //get existing file path from database for give id
                    string path1 = objAuditReportDAL.getServiceFieldAudioVideoPath(AudioID);

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
                            result = objAuditReportDAL.UploadAudio(newFileDBPath, AudioID, out AttachmentId, CreatedBy);

                        }
                    }
                }
            }

            if (result)
            {
                GetAttachmentBo ObjList = new GetAttachmentBo();

                //get details for new uploaded file
                ObjList = new AuditReportDAL().GetAttachmentDetails(AttachmentId);

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
                AuditReportDAL objAuditReportDAL = new AuditReportDAL();

                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    var stream = fileContent.InputStream;

                    //returns the existing file path
                    string path1 = objAuditReportDAL.getServiceFieldAudioVideoPath(VideoId);

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
                            result = objAuditReportDAL.UploadVideo(newFileDBPath, VideoId, out AttachentID, CreatedBy);

                        }
                    }
                }
            }

            if (result)
            {
                GetAttachmentBo ObjList = new GetAttachmentBo();

                //get details for new uploaded file
                ObjList = new AuditReportDAL().GetAttachmentDetails(AttachentID);

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
                ObjList = new AuditReportDAL().GetAttachmentDetails(Audio_AttachmentId);//filePath;\
                string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);
                //    _webUrl= "https://localhost:6069/";
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
                TempData["A_SkipRTU_buttonclicked"] = buttonclicked;
                TempData.Keep("A_SkipRTU_buttonclicked");
            }
            else
            {
                TempData["A_SkipRTU_id"] = Convert.ToInt64(id);
                TempData["A_SkipRTU_formID"] = Convert.ToInt64(formID);
                TempData.Keep("A_SkipRTU_id");
                TempData.Keep("A_SkipRTU_formID");
            }

            return base.Json(formID, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadFileSkipRTU()
        {
            try
            {

                var skipRTU_id = Convert.ToInt64(TempData["A_SkipRTU_id"]);
                var skipRTU_formID = Convert.ToInt64(TempData["A_SkipRTU_formID"]);
                var SkipRTU_buttonclicked = TempData["A_SkipRTU_buttonclicked"];

                bool nextbtn = true;
                bool prevbtn = true;

                TempData.Keep("A_SkipRTU_id");
                TempData.Keep("A_SkipRTU_formID");

                List<STP_GetReportTableNew_bak_Result> ReportTableList = (List<STP_GetReportTableNew_bak_Result>)TempData["AuditList"];
                TempData.Keep("AuditList");

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

                        TempData["A_SkipRTU_id"] = next.ID;
                        TempData["A_SkipRTU_formID"] = next.FormId;
                        TempData.Keep("A_SkipRTU_id");
                        TempData.Keep("A_SkipRTU_formID");

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

                        TempData["A_SkipRTU_id"] = prev.ID;
                        TempData["A_SkipRTU_formID"] = prev.FormId;
                        TempData.Keep("A_SkipRTU_id");
                        TempData.Keep("A_SkipRTU_formID");

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

        //public ActionResult GeneratePDF_SkipRTU()
        //{
        //    CompassEntities CompassEntities = new CompassEntities();
        //    try
        //    {

        //        long skipRTU_id = Convert.ToInt64(TempData["A_SkipRTU_id"]);
        //        long skipRTU_formID = Convert.ToInt64(TempData["A_SkipRTU_formID"]);
        //        TempData.Keep("A_SkipRTU_id");
        //        TempData.Keep("A_SkipRTU_formID");


        //        var list = new AuditReportDAL().GetReportsRecordsID(skipRTU_id, skipRTU_formID);

        //        // var listitem = list.GroupBy(o => o.sectionName);
        //        ReportPDFVM ReportPDFVM = new ReportPDFVM();
        //        ReportPDFVM.sectionfiledval = list;



        //        STP_GetReportHeaderFields_Result reportheader = CompassEntities.STP_GetReportHeaderFields(skipRTU_id).FirstOrDefault();
        //        @ViewBag.FormName = reportheader.formname;
        //        @ViewBag.ProjectName = reportheader.projectname;
        //        @ViewBag.ProjectDate = Convert.ToDateTime(reportheader.visiteddate).ToString("MMM-dd-yyyy hh:mm tt");
        //        @ViewBag.InstallerName = reportheader.firstname;



        //        string footer = "--header-font-size \"9\" --header-spacing 5 --header-right \"Project name:-" + @ViewBag.ProjectName + " \n\t Installer name:-" + @ViewBag.InstallerName + " \n\t Date submitted:-" + @ViewBag.ProjectDate + "  \"  --footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
        //        return View("GeneratePDF", list);

        //        //return new Rotativa.ViewAsPdf("GeneratePDF", list)
        //        //{

        //        //    CustomSwitches = footer,
        //        //    PageOrientation = Orientation.Portrait,
        //        //    //PageMargins = { Top = 10, Bottom = 10 },

        //        //    PageSize = Size.A4,

        //        //    PageMargins = new Margins(13, 0, 10, 0),
        //        //    PageWidth = 210,
        //        //    PageHeight = 297
        //        //};
        //    }
        //    catch (Exception ex)
        //    {

        //        log.Error(ex.Message);
        //        return View("Index");
        //    }

        //    // return View("GeneratePDF", list);

        //    // return View("GeneratePDF", ReportPDFVM);
        //}

        public ActionResult DownloadPDF_SkipRTU(long FormID, long UploadID)
        {
            CompassEntities CompassEntities = new CompassEntities();
            try
            {

                long ID = Convert.ToInt64(TempData["A_pid"]);
                long frmID = Convert.ToInt64(TempData["A_pfrmid"]);
                TempData.Keep("A_pid");
                TempData.Keep("A_pfrmid");

                ID = UploadID;
                frmID = FormID;


                var list = new AuditReportDAL().GetReportsRecordsID(ID, frmID);

                // var listitem = list.GroupBy(o => o.sectionName);
                ReportPDFVM ReportPDFVM = new ReportPDFVM();
                ReportPDFVM.sectionfiledval = list;



                STP_GetReportHeaderFields_Result reportheader = CompassEntities.STP_GetReportHeaderFields(ID).FirstOrDefault();
                @ViewBag.FormName = reportheader.formname;
                @ViewBag.ProjectName = reportheader.projectname;
                @ViewBag.ProjectDate = Convert.ToDateTime(reportheader.visiteddate).ToString("MMM-dd-yyyy hh:mm tt");
                @ViewBag.AuditorName = reportheader.firstname;



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

        #endregion




    }





}
