using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using GarronT.CMS.UI.CustomHelpers;
using KSPL.Indus.MSTS.UI.Filters;
using log4net;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    /*
    * -------------------------------------------------------------------------------
    * Create By     : Aniket Jadhav 
    * Created On    : 07-Sep-2016 
    * Description   : Controller for Installer Map Data History
    *--------------------------------------------------------------------------------
    */

    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class InstallerMapDataHistoryController : Controller
    {
        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(InstallerMapDataHistoryController));
        #endregion

        // GET: Admin/InstallerMapDataHistory
        public ActionResult Index()
        {
            return View();
        }



        /// <summary>
        /// Get InstallerMapData History
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetInstMapHistoryList(long projectID, long installerID, string SkipReason,string FromDate,string ToDate)
        {
            bool success = false;
            string returnMessage = "";

            List<InstallerMapHistoryVM> installerMapHistoryList = new List<InstallerMapHistoryVM>();
            try
            {
                using (InstallerMapHistoryDAL objInstallerMapHistoryDAL = new InstallerMapHistoryDAL())
                {
                    //get skip records submitted from mobile app.
                    installerMapHistoryList = objInstallerMapHistoryDAL.GetInstallerMapHistoryList(projectID, installerID, SkipReason, FromDate, ToDate);
                }
                success = true;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                success = false;
                returnMessage = "Unable To Get Installer Re-Assign Data";
            }
            return Json(new { installerMapHistoryList, success, returnMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetSkipAccountDetail(long ProjectId, long AccountId, string AccountNo)
        {
            StatusMessage statusMessage = new StatusMessage();
            List<SkipAccountDetailModel> objSkipAccountDetailLIst = new List<SkipAccountDetailModel>();
            InstallerMapHistoryDAL objInstallerMapHistoryDAL = new InstallerMapHistoryDAL();
            try
            {
                objSkipAccountDetailLIst = objInstallerMapHistoryDAL.getSkipAccountHistorySummary(ProjectId, AccountId, AccountNo);
                statusMessage.Success = true;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                statusMessage.Success = false;
                statusMessage.Message = "Unable to get summary data";
            }
            var jsonResult = Json(new { statusMessage, objSkipAccountDetailLIst }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        /// <summary>
        /// Get ProjectList by status
        /// Created by AniketJ on 08-Sep-2016
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult GetPoject(string status)
        {
            bool success = false;
            string returnMessage = "";
            List<ProjectModel> projectList = new List<ProjectModel>();

            //Get ProjectList by Project Status
            projectList = new InstallerMapHistoryDAL().GetProjectName(status);
            success = true;


            var checkAdmin = Session["CurrentUserIsAdmin"].ToString();

            if (checkAdmin == "0")
            {
                var userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                var abcd = new ProjectMasterDAL().GetUserProjectList(userId);


                if (abcd == null || abcd.Count == 0)
                {
                    if (projectList != null && projectList.Count() > 0)
                    {
                        projectList = projectList.Take(0).ToList();
                    }
                }
                else
                {
                    if (projectList != null && projectList.Count() > 0)
                    {
                        projectList = projectList.Where(o => abcd.Contains(o.ProjectId)).ToList();
                    }
                }

            }
            return base.Json(new { projectList, success, returnMessage }, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// Get InstallerList by ProjectID
        /// Created by AniketJ on 08-Sep-2016
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public JsonResult GetInstallerList(long projectID)
        {
            bool success = false;
            string returnMessage = "";
            List<UsersSmallModel> installerList = new List<UsersSmallModel>();
            try
            {
                //Get Installer List by ProjectID
                installerList = new InstallerMapHistoryDAL().GeInstallerListbyProjectID(projectID);
                success = true;
            }
            catch (Exception ex)
            {
                returnMessage = "Unable to get Installers";
                log.Error(ex.Message);
            }
            return base.Json(new { installerList, success, returnMessage }, JsonRequestBehavior.AllowGet);
        }


        //To store skippedrecord id in session
        public ActionResult StoreSkippedID(long uploadId)
        {
            bool success = false;
            if (uploadId > 0)
            {
                TempData["skippedUploadID"] = Convert.ToInt64(uploadId);
                TempData.Keep("skippedUploadID");
                success = true;
            }
            return base.Json(success, JsonRequestBehavior.AllowGet);
        }

        //View skipped record
        public ActionResult ViewSkippedRecords(string id)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("InstallerMapDataHistory", "View");

            if (objPermissionCheck)
            {

                @ViewBag.UploadID = id;
                return View();

            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }


        }


        /// <summary>
        /// To view multiple skipped records in Iframe 
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewMultiSkippedRecords(string id)
        {
            try
            {
                List<SkippedPDFVM> skippedList = new List<SkippedPDFVM>();

                InstallerMapHistoryDAL objInstallerMapHistoryDAL = new InstallerMapHistoryDAL();

                //get skip records submitted from mobile app.
                long id1 = long.Parse(id);
                long projectID = objInstallerMapHistoryDAL.GetProjectIdForCurrent(id1);

                //List<InstallerMapHistoryVM> installerMapHistoryList = objInstallerMapHistoryDAL.GetInstallerMapHistoryList(projectID, 0);GetAllInstallerMapHistoryList
                List<InstallerMapHistoryVM> installerMapHistoryList = objInstallerMapHistoryDAL.GetAllInstallerMapHistoryList(projectID, 0);
                var uploadId = installerMapHistoryList.Where(o => o.ProjectFieldDataId == id1).FirstOrDefault();

                installerMapHistoryList = installerMapHistoryList.Where(o => o.ID == uploadId.ID).ToList();

                foreach (var ReportRow in installerMapHistoryList)
                {

                    SkippedPDFVM reportPDFVM = new SkippedPDFVM();
                    reportPDFVM.FormName = ReportRow.FormName;
                    reportPDFVM.ProjectName = ReportRow.projectname;
                    reportPDFVM.VisitedDate = ReportRow.VisitedDate;
                    reportPDFVM.InstallerName = ReportRow.InstallerName;
                    reportPDFVM.FormId = ReportRow.FormId;
                    reportPDFVM.skipRecordModel = ReportRow;
                    reportPDFVM.ID = ReportRow.ID;
                    reportPDFVM.ProjectFieldDataId = ReportRow.ProjectFieldDataId;
                    skippedList.Add(reportPDFVM);
                }

                string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";
                @ViewBag.UploadID = id1;
                return View(skippedList);

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return View("Index");
            }
        }

        /// <summary>
        /// To Download multiple skipped records in Iframe 
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadMultiSkippedRecords(string UploadID)
        {
            try
            {
                List<SkippedPDFVM> skippedList = new List<SkippedPDFVM>();

                InstallerMapHistoryDAL objInstallerMapHistoryDAL = new InstallerMapHistoryDAL();

                //get skip records submitted from mobile app.
                long id = long.Parse(UploadID);
                long projectID = objInstallerMapHistoryDAL.GetProjectIdForCurrent(id);
                //List<InstallerMapHistoryVM> installerMapHistoryList = objInstallerMapHistoryDAL.GetInstallerMapHistoryList(projectID, 0);
                List<InstallerMapHistoryVM> installerMapHistoryList = objInstallerMapHistoryDAL.GetAllInstallerMapHistoryList(projectID, 0);//  Changed by sominath to get all skipped record.

                var uploadId = installerMapHistoryList.Where(o => o.ProjectFieldDataId == id).FirstOrDefault();

                installerMapHistoryList = installerMapHistoryList.Where(o => o.ID == uploadId.ID).ToList();

                foreach (var ReportRow in installerMapHistoryList)
                {
                    SkippedPDFVM reportPDFVM = new SkippedPDFVM();
                    reportPDFVM.FormName = ReportRow.FormName;
                    reportPDFVM.ProjectName = ReportRow.projectname;
                    reportPDFVM.VisitedDate = ReportRow.VisitedDate;
                    reportPDFVM.InstallerName = ReportRow.InstallerName;
                    reportPDFVM.FormId = ReportRow.FormId;
                    reportPDFVM.skipRecordModel = ReportRow;
                    reportPDFVM.ID = ReportRow.ID;
                    reportPDFVM.ProjectFieldDataId = ReportRow.ProjectFieldDataId;
                    skippedList.Add(reportPDFVM);
                }

                string footer = "--footer-center \"Printed on: " + DateTime.Now.Date.ToString("MM/dd/yyyy") + "  Page: [page]/[toPage]\"" + " --footer-line --footer-font-size \"9\" --footer-spacing 6 --footer-font-name \"calibri light\"";

                return new Rotativa.ViewAsPdf("DownloadMultiSkippedRecords", skippedList)
                {
                    FileName = "SkippedRecords.pdf",
                    CustomSwitches = footer,
                    PageOrientation = Orientation.Portrait,
                    PageSize = Size.A4,
                    PageMargins = new Margins(2, 0, 10, 0),
                    PageWidth = 210,
                    PageHeight = 290
                };
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return View("Index");
            }
        }

    }
}