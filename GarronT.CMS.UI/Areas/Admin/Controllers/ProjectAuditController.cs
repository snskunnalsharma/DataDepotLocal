using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using GarronT.CMS.UI.CustomHelpers;
using GarronT.CMS.UI.Models;
using KSPL.Indus.MSTS.UI.Filters;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class ProjectAuditController : Controller
    {
        //
        // GET: /Admin/ProjectAudit/
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ActiveStatus"></param>
        /// <returns></returns>
        public ActionResult GetAuditProjectList(int activeStatus)
        {
            AuditDAL obj = new AuditDAL();
            List<AuditBO> objList = obj.GetAuditProjectList(activeStatus);
            obj.Dispose();
            return Json(objList, JsonRequestBehavior.AllowGet);
        }






        public ActionResult Create()
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("ProjectAudit", "Add");
            if (objPermissionCheck)
            {

                return View();
            }
            else
            {
                return Json(new { success = objPermissionCheck, returnMessage = UserPermissionCheck.PerMessage }, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpGet]
        public ActionResult CloneAuditProject(long CloneAuditId)
        {
            //TempData["CloneAuditId"] = null;
            //TempData["CloneAuditId"] = CloneAuditId.ToString();
            //TempData.Keep("CloneAuditId");


            var objPermissionCheck = UserPermissionCheck.checkUserStatus("ProjectAudit", "Add");
            if (objPermissionCheck)
            {
                @ViewBag.CloneAuditId = CloneAuditId;
                return View();
            }
            else
            {
                return Json(new { success = objPermissionCheck, returnMessage = UserPermissionCheck.PerMessage }, JsonRequestBehavior.AllowGet);

            }

        }




        [HttpGet]
        public JsonResult GetCloneProjectDetails(long CloneAuditId)
        {

            bool result = false;
            string resultMessage = "";
            ProjectSmallModel obj1 = new ProjectSmallModel();

            using (AuditDAL obj = new AuditDAL())
            {
                obj1 = obj.GetProjectforUpdate(CloneAuditId);
                //obj1.objPresentList = obj.GetUserList();
                //obj1.objLongList = obj.GetProjectUserRecords(auditId);
            }

            return Json(new { objProjectSmallModel = obj1 }, JsonRequestBehavior.AllowGet);


        }

        [HttpGet]
        public JsonResult GetCloneProjectDetailsNew(long CloneAuditId, long NewProjectId)
        {


            ProjectSmallModel obj1 = new ProjectSmallModel();

            using (AuditDAL obj = new AuditDAL())
            {
                obj1 = obj.GetProjectforUpdate(CloneAuditId);

                var m = obj.GetInstallerForProject(NewProjectId);
                var obj2 = obj1.objList;
                var obj3 = obj1.objAuditProjectFailBOList;
                if (m != null && m.Count > 0)
                {

                    if (obj2 != null && obj2.Count > 0)
                    {

                        foreach (var item in m)
                        {
                            var CheckInst = obj2.Where(o => o.InstallerId == item.ChildId).FirstOrDefault();
                            if (CheckInst == null)
                            {
                                obj2.Add(new AuditCycleBO
                                {
                                    InstallerId = item.ChildId,
                                    InstallerName = item.ChildName,
                                    Active = 1,
                                    AuditStartDate = 1,
                                    AuditEndDate = 10,
                                    AuditFailed = "No",
                                    AuditPercentage = 100
                                });
                            }
                        }


                        var OldArrayList = obj2.Select(o => o.InstallerId).Distinct().ToArray();

                        foreach (var item in OldArrayList)
                        {
                            var objcc = m.Where(o => o.ChildId == item).FirstOrDefault();
                            if (objcc == null)
                            {
                                obj2 = obj2.Where(o => o.InstallerId != item).ToList();
                            }
                        }
                        // obj1.objList = obj2;
                    }

                }

                obj1 = obj.GetProjectDetails(NewProjectId);
                obj1.objList = obj2;
                obj1.objAuditProjectFailBOList = obj3;

            }

            return Json(new { objProjectSmallModel = obj1 }, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditId"></param>
        /// <returns></returns>
        public ActionResult Edit(string auditId)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("ProjectAudit", "Edit");
            if (objPermissionCheck)
            {
                long FKProjectId = new AuditDAL().GetAuditRecordProjectId(int.Parse(auditId));

                @ViewBag.ProjectId = auditId;
                @ViewBag.FKProjectId = FKProjectId;

                return View();
            }
            else
            {
                return Json(new { success = false, returnMessage = UserPermissionCheck.PerMessage }, JsonRequestBehavior.AllowGet);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="utilityType"></param>
        /// <returns></returns>
        public ActionResult GetProjectData(string utilityType)
        {
            AuditDAL obj = new AuditDAL();
            IEnumerable<ProjectModel> data = obj.GetProjectName(utilityType).Distinct<ProjectModel>();


            // return base.Json(data, JsonRequestBehavior.AllowGet);

            List<UsersSmallModel> list = obj.GetUserList();
            return base.Json(new { data = data, list = list }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetProjectDetails(long projectId)
        {
            bool result = false;
            string resultMessage = "";
            ProjectSmallModel obj1 = new ProjectSmallModel();

            if (projectId > 0)
            {
                using (AuditDAL obj = new AuditDAL())
                {
                    result = obj.GetProjectDetails(projectId, out resultMessage);
                    if (result == false)
                    {
                        obj1 = obj.GetProjectDetails(projectId);
                    }
                }
            }

            return Json(new { result = result, resultMessage = resultMessage, objProjectSmallModel = obj1 }, JsonRequestBehavior.AllowGet);
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetProjectDataForUpdate(long auditId)
        {

            ProjectSmallModel obj1 = new ProjectSmallModel();

            using (AuditDAL obj = new AuditDAL())
            {
                obj1 = obj.GetProjectforUpdate(auditId);
                //obj1.objPresentList = obj.GetUserList();
                //obj1.objLongList = obj.GetProjectUserRecords(auditId);
            }

            return Json(new { objProjectSmallModel = obj1 }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="auditorList"></param>
        /// <returns></returns>
        //public JsonResult SaveAuditDetails(long projectId, string startDate, string endDate, string[] auditorList)
        //{
        public JsonResult SaveAuditDetails(AuditCreateBO objAuditCreateBO)
        {
            bool result = false;


            if (objAuditCreateBO.auditorList == null || objAuditCreateBO.auditorList.Count() == 0)
            {
                return base.Json(new { Status = false, ResultMessage = "At least one auditor need to select." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                using (AuditDAL obj = new AuditDAL())
                {
                    string rMessage = "";
                    var checkValid = obj.ProjectAuditReadyValidate(objAuditCreateBO.projectId, out rMessage);

                    if (checkValid == false)
                    {
                        return base.Json(new { Status = false, ResultMessage = rMessage }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                        var obj1 = obj.GetProjectDetails(objAuditCreateBO.projectId);

                        DateTime projectStartDate = DateTime.Parse(obj1.stringFromDate);
                        DateTime projectEndDate = DateTime.Parse(obj1.stringToDate);

                        DateTime auditStartDate = DateTime.Parse(objAuditCreateBO.startDate);
                        DateTime auditEndDate = DateTime.Parse(objAuditCreateBO.endDate);

                        if (auditStartDate.Date < projectStartDate.Date)
                        {
                            return base.Json(new { Status = false, ResultMessage = "Audit start date can not be less than project start date." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {

                            int currentUserId = (int)new UserDetails().GetUserId(User.Identity.GetUserId());

                            //result = obj.SaveData(projectId, currentUserId, startDate, endDate, auditorList);
                            result = obj.SaveData(objAuditCreateBO, currentUserId);
                            return base.Json(new { Status = result, ResultMessage = "" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="auditorList"></param>
        /// <returns></returns>
        //public JsonResult UpdateAuditDetails(long auditId, string startDate, string endDate, string[] auditorList)
        //{
        public JsonResult UpdateAuditDetails(AuditCreateBO objAuditCreateBO)
        {
            bool result = false;


            if (objAuditCreateBO.auditorList == null || objAuditCreateBO.auditorList.Count() == 0)
            {
                return base.Json(new { Status = false, ResultMessage = "At least one auditor need to select." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                using (AuditDAL obj = new AuditDAL())
                {

                    var obj1 = obj.GetProjectFromAuditId(objAuditCreateBO.auditProjectId);

                    DateTime projectStartDate = DateTime.Parse(obj1.stringFromDate);
                    DateTime projectEndDate = DateTime.Parse(obj1.stringToDate);

                    DateTime auditStartDate = DateTime.Parse(objAuditCreateBO.startDate);
                    DateTime auditEndDate = DateTime.Parse(objAuditCreateBO.endDate);

                    if (auditStartDate.Date < projectStartDate.Date)
                    {
                        return base.Json(new { Status = false, ResultMessage = "Audit start date can not be less than project start date." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        int currentUserId = (int)new UserDetails().GetUserId(User.Identity.GetUserId());
                        string strMessage = "";
                        result = obj.UpdateData(objAuditCreateBO, currentUserId, out strMessage);
                        return base.Json(new { Status = result, ResultMessage = strMessage }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public JsonResult GetInstallerAuditCycleData(long projectId)
        {

            List<InstallerAuditCycleBO> objList = new List<InstallerAuditCycleBO>();
            using (AuditDAL obj = new AuditDAL())
            {
                objList = obj.GetInstallerAuditCycleData(projectId);
            }


            if (objList != null && objList.Count > 0)
            {
                return base.Json(new { Status = true, objList = objList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return base.Json(new { Status = false, objList = objList }, JsonRequestBehavior.AllowGet);
            }

        }



        public JsonResult ActiveDeactivateRecord(long auditId, bool activate)
        {

            var objPermissionCheck = UserPermissionCheck.checkUserStatus("ProjectAudit", "Delete");
            if (objPermissionCheck)
            {
                bool result = false;
                using (AuditDAL obj = new AuditDAL())
                {
                    result = obj.ActiveDeactivateRecord(auditId, activate);
                }

                return base.Json(new { Status = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, returnMessage = UserPermissionCheck.PerMessage }, JsonRequestBehavior.AllowGet);
            }


        }


        public JsonResult ActiveDeactivateInstallerRecord(long Id, bool activate)
        {
            bool result = false;
            using (AuditDAL obj = new AuditDAL())
            {
                result = obj.ActiveDeactivateRecord(Id, activate);
            }

            return base.Json(new { Status = result }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetCustData(long auditorCycleId)
        {
            InstallerAuditCycleBO result = new InstallerAuditCycleBO();
            using (AuditDAL obj = new AuditDAL())
            {
                result = obj.GetCustData(auditorCycleId);
            }

            return base.Json(new { Status = result }, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="auditorList"></param>
        /// <returns></returns>
        public JsonResult UpdateInstallerAuditCycles(long auditId, string auditPercent, int startDay, int endDay)
        {

            bool result = false;

            using (AuditDAL obj = new AuditDAL())
            {
                int currentUserId = (int)new UserDetails().GetUserId(User.Identity.GetUserId());
                long projectId = 0;
                decimal PT = decimal.Parse(auditPercent);
                string resultMessage = "";
                result = obj.UpdateInstallerAuditCycles(auditId, PT, startDay, endDay, currentUserId, out resultMessage, out projectId);
                return base.Json(new { Status = result, projectId = projectId, ResultMessage = resultMessage }, JsonRequestBehavior.AllowGet);

            }

        }


        [HttpGet]
        public ActionResult AddNewAuditCycle(long projectId)
        {

            AuditCycleBO objAuditCycleBO = new AuditCycleBO();

            var m = new AuditDAL().GetInstallerForProject(projectId);

            IEnumerable<SelectListItem> t = new List<SelectListItem>();
            if (m != null && m.Count > 0)
            {
                t = m.Select(o => new SelectListItem { Value = o.ChildId.ToString(), Text = o.ChildName }).ToList();
            }

            ViewBag.ObjList = t;
            ViewBag.Status = "Create";
            return PartialView("_AddAuditCycle", objAuditCycleBO);
        }

        [HttpGet]
        public ActionResult EditAuditCycle(long projectId)
        {

            AuditCycleBO objAuditCycleBO = new AuditCycleBO();

            var m = new AuditDAL().GetInstallerForProject(projectId);

            IEnumerable<SelectListItem> t = new List<SelectListItem>();
            if (m != null && m.Count > 0)
            {
                t = m.Select(o => new SelectListItem { Value = o.ChildId.ToString(), Text = o.ChildName }).ToList();
            }

            ViewBag.ObjList = t;
            ViewBag.Status = "Edit";
            return PartialView("_AddAuditCycle", objAuditCycleBO);
        }


        [HttpGet]
        public ActionResult AddNewFailCycle(bool IsCreate)
        {
            AuditProjectFailBO objAuditProjectFailBO = new AuditProjectFailBO();
            ViewBag.Status = IsCreate == true ? "Create" : "Edit";
            return PartialView("_AddFailCycle", objAuditProjectFailBO);
        }




    }
}