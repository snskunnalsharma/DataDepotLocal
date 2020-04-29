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
using Microsoft.AspNet.Identity;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ComingSoon()
        {
            return View();
        }


        /*    -------------------------------
            Code changed by sominath on 04/01/2017
            Display the project info on dashboard 
        */

        [HttpGet]
        public ActionResult GetCurrentActiveProject()
        {
            //Find current active project by to date in project master table.

            DashboardDAL dashboardDAL = new DashboardDAL();

            string data = dashboardDAL.GetRunningProjectUtility();

            return base.Json(new { Utilitytype = data }, JsonRequestBehavior.AllowGet);

        }



        #region Get projectinfo  for Dashboard graph
        [HttpGet]
        public ActionResult GetAuditProjectInfoDashBord(long projectID)
        {


            List<AuditChartModel> fullList = new List<AuditChartModel>();


            decimal TotalCompleteAudit = 0;
            DashboardDAL ObjDash = new DashboardDAL();
            if (projectID != null)
            {

                //1>get Installation count per installer by SP
                var _record1 = ObjDash.GetInstallatioCount(projectID).ToList();
                //2>Get Total Assigned Audit
                var _record2 = ObjDash.GetAssignedAudit(projectID).ToList();

                foreach (var item in _record1)
                {
                    AuditChartModel totalInstallation = new AuditChartModel();
                    totalInstallation.InstallerID = item.Fk_InstallerId;
                    totalInstallation.Installer = item.FirstName;
                    totalInstallation.TotalInstallation = item.TotalInstallation != null ? int.Parse(item.TotalInstallation.ToString()) : 0;

                    fullList.Add(totalInstallation);
                }

                foreach (var item in _record2)
                {
                    var record = fullList.Where(o => o.InstallerID == item.Fk_InstallerId).FirstOrDefault();
                    if (record != null)
                    {
                        record.TotalAssignedAudit = item.TotalAssignedAudit != null ? int.Parse(item.TotalAssignedAudit.ToString()) : 0;
                    }
                }


                //3>Get Total Passed Audit
                var _record3 = ObjDash.GetPassedAudit(projectID);
                foreach (var item in _record3)
                {
                    var record = fullList.Where(o => o.InstallerID == item.Fk_InstallerId).FirstOrDefault();
                    if (record != null)
                    {
                        record.TotalPassedAudit = item.TotalPassedAudit != null ? int.Parse(item.TotalPassedAudit.ToString()) : 0;
                    }
                }


                //4>Get Total Failed Audits
                var _record4 = ObjDash.GetFailedAudit(projectID);
                foreach (var item in _record4)
                {
                    var record = fullList.Where(o => o.InstallerID == item.Fk_InstallerId).FirstOrDefault();
                    if (record != null)
                    {
                        record.TotalFailedAudit = item.TotalFailedAudit != null ? int.Parse(item.TotalFailedAudit.ToString()) : 0;
                    }
                }

                //5>Get total Audits combining Passed and Failed Audits
                foreach (var item in fullList)
                {
                    item.TotalCompleteAudit = item.TotalPassedAudit + item.TotalFailedAudit;
                }
            }

            return Json(fullList, JsonRequestBehavior.AllowGet);
        }


        #endregion



        [HttpGet]
        public ActionResult GetCompleteVsPendingChartData(long projectID)
        {
            List<CompletionModel> list = new List<CompletionModel>();

            list = new DashboardDAL().GetCompleteVsPendingChartData(projectID);

            return Json(list, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetInstallationspermonth(long projectID)
        {
            List<InstallationModel> list = new List<InstallationModel>();

            int totaluploadedrecords = 0;
            int PendingRecords = 0;
            list = new DashboardDAL().GetInstallationspermonthNew(projectID, out totaluploadedrecords, out PendingRecords);
            return Json(new { list, totaluploadedrecords, PendingRecords }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetInstallationPerInstaller(long projectID)
        {
            List<InstallationModel> list = new List<InstallationModel>();

            int totaluploadedrecords = 0;
            list = new DashboardDAL().GetInstallationPerInstaller(projectID, out totaluploadedrecords);

            return Json(new { list, totaluploadedrecords }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetProjectInsatallation()
        {
            List<ProjectInstallationModel> list = new List<ProjectInstallationModel>();

            list = new DashboardDAL().GetProjectInsatallation();

            // list = new DashboardDAL().GetInsatallationPerInstaller();

            return Json(list, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetCityListHeader()
        {
            List<DashboardHeader> Citylist = new List<DashboardHeader>();

            Citylist = new DashboardDAL().GetCityListHeader();

            return Json(Citylist, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// Gets the project completion percentage status.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult LoadProjectCompletionInPercentGraph(string utilityType, string ProjectStatus)
        {
            List<CompletionModel> list = new List<CompletionModel>();

            list = new DashboardDAL().ProjectCompletionInPercentGraph(utilityType);


            var checkAdmin = Session["CurrentUserIsAdmin"].ToString();
            if (checkAdmin == "0")
            {
                ProjectMasterDAL projectMasterDAL = new ProjectMasterDAL();
                var userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                var abcd = projectMasterDAL.GetUserProjectListForDashBoard(utilityType, ProjectStatus, userId);


                if (list == null || list.Count == 0)
                {
                    if (list != null && list.Count() > 0)
                    {
                        list = list.Take(0).ToList();
                    }
                }
                else
                {
                    list = list.Where(o => abcd.Contains(o.projectId)).ToList();
                }
            }



            return Json(list, JsonRequestBehavior.AllowGet);

        }



        [HttpGet]
        public ActionResult GetProjectByCityID(long CityID)
        {
            List<DashboardHeader> Citylist = new List<DashboardHeader>();

            if (CityID != null)
            {
                Citylist = new DashboardDAL().GetProjectByCityID(CityID);
            }

            return Json(Citylist, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetPoject(string utilityType, string ProjectStatus)
        {


            ProjectMasterDAL projectMasterDAL = new ProjectMasterDAL();
            var checkAdmin = Session["CurrentUserIsAdmin"].ToString();

            IEnumerable<ProjectModel> data = (
                from a in projectMasterDAL.GetProjectNameFOrDashBoard(utilityType, ProjectStatus)
                orderby a.ProjectStatus, a.ProjectName
                select a).Distinct<ProjectModel>();

            if (checkAdmin == "0")
            {
                var userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                var abcd = projectMasterDAL.GetUserProjectListForDashBoard(utilityType, ProjectStatus, userId);


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

        #region Project status

        public ActionResult ProjectStatus()
        {
            return View();
        }

        public ActionResult getRowPartial()
        {
            return base.PartialView("Partial_ProjectStatus");
        }

        public ActionResult GetAddressMap()
        {
            CompassEntities dbcontext = new CompassEntities();
            List<tblUploadedDataBO> Get_lat_lagnew = new List<tblUploadedDataBO>();

            List<tblUploadedDataBO> Get_lat_lagall = new List<tblUploadedDataBO>();


            //var result = (from a in dbcontext.STP_GetAddress()
            //              where Address.Contains(a.Id)
            //              select new tblUploadedDataBO { Latitude = a.Latitude, Longitude = a.Longitude, Street = a.Street, ID = a.Id }).ToList();
            var result = dbcontext.tblUploadedDatas.ToList();

            var completed = result.
              Select(a => new tblUploadedDataBO
              {
                  Latitude = a.Latitude,
                  Longitude = a.Longitude,
                  Street = a.Street,
                  AdressStatus = "Completed",
                  ID = a.ID,
                  Street_route = Regex.Replace(a.Street, @"[\d-]", string.Empty),
                  street_number = Regex.Replace(a.Street, "[^0-9]+", string.Empty)
              }).OrderBy(a => a.Street_route).ThenBy(a => a.street_number).ToList().Take(50);
            Get_lat_lagall.AddRange(completed);

            var pending = result.
        Select(a => new tblUploadedDataBO
        {
            Latitude = a.Latitude,
            Longitude = a.Longitude,
            Street = a.Street,
            AdressStatus = "Pending",
            ID = a.ID,
            Street_route = Regex.Replace(a.Street, @"[\d-]", string.Empty),
            street_number = Regex.Replace(a.Street, "[^0-9]+", string.Empty)
        }).OrderBy(a => a.Street_route).ThenBy(a => a.street_number).ToList().Skip(50).Take(50);
            Get_lat_lagall.AddRange(pending);

            var skipped = result.Select(a => new tblUploadedDataBO
            {
                Latitude = a.Latitude,
                Longitude = a.Longitude,
                Street = a.Street,
                AdressStatus = "Skipped",
                ID = a.ID,
                Street_route = Regex.Replace(a.Street, @"[\d-]", string.Empty),
                street_number = Regex.Replace(a.Street, "[^0-9]+", string.Empty)
            }).OrderBy(a => a.Street_route).ThenBy(a => a.street_number).ToList().Skip(100).Take(50);

            Get_lat_lagall.AddRange(skipped);
            foreach (var item in Get_lat_lagall)
            {
                var checkExistStreet = Get_lat_lagnew.Where(o => o.Street.Contains(item.Street)).FirstOrDefault();
                if (checkExistStreet == null)
                {

                    var checkExistLatLon = Get_lat_lagnew.Where(o => o.Latitude == item.Latitude && o.Longitude == o.Longitude).FirstOrDefault();
                    if (checkExistLatLon == null)
                    {
                        Get_lat_lagnew.Add(item);
                    }
                    else
                    {
                        checkExistLatLon.Street = checkExistLatLon.Street + ",<br/>" + item.Street;
                    }

                }
            }

            return Json(Get_lat_lagnew, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}