using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using KSPL.Indus.MSTS.UI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;


namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class ProjectStatusController : Controller
    {
        #region Project status

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getRowPartial()
        {
            return base.PartialView("Partial_ProjectStatus");
        }

        public ActionResult GetAddressMap(long projectID, long InstallerID, long formTypeId,string locationId, List<string> Cycle, List<string> Route)
        {
            List<addressVisitInfo> list = new List<addressVisitInfo>();

            //long projectID = 1;
            try
            {
                if (formTypeId == 2)
                {
                    list = new ProjectStatusDAL().GetProjectStaus(projectID, InstallerID, formTypeId, locationId, Cycle, Route);
                }
                else if (formTypeId == 1)
                {
                    list = new ProjectStatusDAL().GetAuditProjectStaus(projectID, InstallerID, formTypeId);
                }

            }
            catch (Exception ex)
            {
            }
            var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }
        // cycle method start
        public ActionResult GetFreshCycle(long projectID)
        {
            try
            {
                using (ProjectStatusDAL projectStatusDAL = new ProjectStatusDAL())
                {
                    if (!projectStatusDAL.HasActiveAccounts(projectID))
                    {
                        return base.Json(new List<string>(), JsonRequestBehavior.AllowGet);
                    }

                    return base.Json(projectStatusDAL.GetDynamicFreshCycleByProjectIDs(projectID), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return base.Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetFreshCycleByOffice(long projectID, string locationID)
        {
            List<string> cyclelist = new List<string>();

            //cyclelist = new InstallerDAL().GetFreshCycleByProjectID(projectID);
            //cyclelist = new ProjectStatusDAL().GetDynamicFreshCycleByProjectIDs(projectID);
            cyclelist = new ProjectStatusDAL().GetDynamicFreshCycleByOffice(projectID, locationID);

            return base.Json(cyclelist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCycledatabyInstaller (long projectID, long InstallerId)
        {
            List<string> cyclelist = new List<string>();

            cyclelist = new ProjectStatusDAL().GetRoute_Cycle_By_Installer(projectID, InstallerId);
            return base.Json(cyclelist, JsonRequestBehavior.AllowGet);
        }
        // root method start
        public ActionResult GetRootByCycles(long ProjectId, string[] Cycle, long InstallerId)
        {

            List<string> routeList = new ProjectStatusDAL().GetDynamicFreshRootByCycles(Cycle, ProjectId, InstallerId);
            List<PROC_ExistingRoute_Result> ObjExistingList = new ProjectStatusDAL().GetDynamicExistingRootByCycles(Cycle, ProjectId, InstallerId);
            if (routeList == null)
            {
                routeList = new List<string>();
            }
            foreach (var item in ObjExistingList)
            {
                var checkRoutePresent = routeList.Where(o => o == item.Route).FirstOrDefault();
                if (checkRoutePresent == null)
                {
                    routeList.Add(item.Route);
                }
            }
            var InstallerRouteList = new List<RouteMaster>();
            foreach (var item in ObjExistingList)
            {
                InstallerRouteList.Add(new RouteMaster { RouteId = item.Route, RouteDisabled = item.Disabled == 1 ? true : false });
            }
            return base.Json(new { Route = routeList, selectedRoute = InstallerRouteList }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetPoject(string utilityType, string ProjectStatus)
        {

            ProjectStatusDAL projectStatusDAL = new ProjectStatusDAL();
            IEnumerable<ProjectModel> data = projectStatusDAL.GetProjectName(utilityType, ProjectStatus).Distinct<ProjectModel>();

            var checkAdmin = Session["CurrentUserIsAdmin"].ToString();


            if (checkAdmin == "0")
            {
                var userId = new UserDetails().GetUserId(User.Identity.GetUserId());

                ProjectStatus = ProjectStatus == "2" ? "Complete" : "Active";

                var abcd = new ProjectMasterDAL().GetUserProjectListForDashBoard(utilityType, ProjectStatus, userId);


                if (abcd == null || abcd.Count == 0)
                {
                    if (data != null && data.Count() > 0)
                    {
                        data = data.Take(0);
                    }
                }
                else
                {
                    data = data.Where(o => abcd.Contains(o.ProjectId)).ToList();
                }

            }

            return base.Json(data, JsonRequestBehavior.AllowGet);
        }

   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult getCurrentInstallerList(long projectId, long formType)
        {
            List<UsersSmallModel> obj1 = new List<UsersSmallModel>();
            if (projectId > 0)
            {
                using (UserRoleDAL obj = new UserRoleDAL())
                {
                    obj1 = obj.GeInstallerListCurrentProject(projectId).OrderBy(a => a.UserName).ToList();
                }
            }

            return Json(obj1, JsonRequestBehavior.AllowGet);
        }


        

        /// <summary>
        /// AddedBy AniketJ on 15-Oct-2016
        /// TO get FormType
        /// </summary>
        /// <returns></returns>
        public JsonResult GetFormType()
        {
            using (ProjectStatusDAL projectStatusDAL = new ProjectStatusDAL())
            {
                var formTypeList = projectStatusDAL.GetFormTypeList();
                return base.Json(formTypeList, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetLocationList(long projectId)
        {
            using (ProjectStatusDAL projectStatusDAL = new ProjectStatusDAL())
            {
                var objLocationList = projectStatusDAL.LocationList(projectId);
                return base.Json(objLocationList, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult HasProjectAccounts(long projectId)
        {
            using (ProjectStatusDAL projectStatusDAL = new ProjectStatusDAL())
            {
                return Json(new { hasAccounts = projectStatusDAL.HasActiveAccounts(projectId) }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        //public ActionResult CheckAddressExistby_ProjectInstallerDat(long ProjectID, long InstallerID)
        //{

        //    using (ProjectStatusDAL objDAL = new ProjectStatusDAL())
        //    {
                
        //        var lat_by_ProjectInstaller = objDAL.CheckAddressExistby_ProjectInstallerDates(ProjectID, InstallerID);
        //        CompassEntities db = new CompassEntities();
        //        var ExcludeSkip = db.tblUsers.Where(n => n.UserID == InstallerID).FirstOrDefault().ExcludeSkip;
        //        //foreach (var item in lat_by_ProjectInstaller)
        //        //{
        //        //    item.strFormDate = item.FromDate.Value.ToString("MMM-dd-yyyy");
        //        //    item.strToDate = item.ToDate.Value.ToString("MMM-dd-yyyy");
        //        //}

        //        return base.Json(new { lat_by_ProjectInstaller, ExcludeSkip }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //public ActionResult GetRouteDatabyCycle(long ProjectId,long InstallerId, string[] Cycle)
        //{
        //    List<string> routeList = new ProjectStatusDAL().GetDynamicRoutebyCycle(ProjectId, InstallerId, Cycle);
        //    List<PROC_ExistingRoute_Result> ObjExistingList = new ProjectStatusDAL().GetDynamicRoutebyCycle1(ProjectId, InstallerId, Cycle);
        //    if (routeList == null)
        //    {
        //        routeList = new List<string>();
        //    }
        //    foreach (var item in ObjExistingList)
        //    {
        //        var checkRoutePresent = routeList.Where(o => o == item.Route).FirstOrDefault();
        //        if (checkRoutePresent == null)
        //        {
        //            routeList.Add(item.Route);
        //        }
        //    }
        //    var InstallerRouteList = new List<RouteMaster>();
        //    foreach (var item in ObjExistingList)
        //    {
        //        InstallerRouteList.Add(new RouteMaster { RouteId = item.Route, RouteDisabled = item.Disabled == 1 ? true : false });
        //    }
        //    return base.Json(new { Route = routeList, selectedRoute = InstallerRouteList }, JsonRequestBehavior.AllowGet);
        //}
    }
}