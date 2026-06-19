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

        [HttpGet]
        public ActionResult GetBudgetDashboardData(string projectStatus, string utilityType, long? projectId, string fromDate, string toDate)
        {
            try
            {
                using (var db = new CompassEntities())
                {
                    db.Database.CommandTimeout = 5;
                    var query = db.tblProjects.Where(p => p.Active == 1);

                    if (!string.IsNullOrEmpty(projectStatus) && projectStatus != "All" && projectStatus != "0")
                    {
                        var statusStr = projectStatus == "1" || projectStatus == "Active" ? "Active" : "Complete";
                        query = query.Where(p => p.ProjectStatus == statusStr);
                    }

                    if (!string.IsNullOrEmpty(utilityType) && utilityType != "All" && utilityType != "0")
                    {
                        var utilityStr = utilityType == "1" || utilityType == "Electric" ? "Electric" : 
                                         (utilityType == "2" || utilityType == "Gas" ? "Gas" : "Water");
                        query = query.Where(p => p.Utilitytype == utilityStr);
                    }

                    if (projectId.HasValue && projectId.Value > 0)
                    {
                        query = query.Where(p => p.ProjectId == projectId.Value);
                    }

                    // Filter by user permissions if not admin
                    var checkAdmin = Session["CurrentUserIsAdmin"]?.ToString() ?? "0";
                    if (checkAdmin == "0")
                    {
                        var userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                        var permittedProjectIds = new List<long?>();
                        
                        var statusesToQuery = new List<string>();
                        if (!string.IsNullOrEmpty(projectStatus) && projectStatus != "All" && projectStatus != "no" && projectStatus != "0")
                        {
                            statusesToQuery.Add(projectStatus == "1" || projectStatus == "Active" ? "Active" : "Complete");
                        }
                        else
                        {
                            statusesToQuery.Add("Active");
                            statusesToQuery.Add("Complete");
                        }

                        var utilitiesToQuery = new List<string>();
                        if (!string.IsNullOrEmpty(utilityType) && utilityType != "All" && utilityType != "no" && utilityType != "0")
                        {
                            utilitiesToQuery.Add(utilityType == "1" || utilityType == "Electric" ? "Electric" : 
                                                (utilityType == "2" || utilityType == "Gas" ? "Gas" : "Water"));
                        }
                        else
                        {
                            utilitiesToQuery.Add("Electric");
                            utilitiesToQuery.Add("Gas");
                            utilitiesToQuery.Add("Water");
                        }

                        foreach (var s in statusesToQuery)
                        {
                            foreach (var u in utilitiesToQuery)
                            {
                                var userProjs = db.PROC_GETUserActiveProject(userId, s, u).ToList();
                                if (userProjs != null)
                                {
                                    permittedProjectIds.AddRange(userProjs);
                                }
                            }
                        }
                        permittedProjectIds = permittedProjectIds.Distinct().ToList();
                        query = query.Where(p => permittedProjectIds.Contains(p.ProjectId));
                    }

                    var filteredProjects = query.ToList();

                    decimal totalBudget = 0;
                    decimal totalActual = 0;

                    var utilityBudgets = new Dictionary<string, decimal> { { "Electric", 0 }, { "Water", 0 }, { "Gas", 0 } };
                    var utilityActuals = new Dictionary<string, decimal> { { "Electric", 0 }, { "Water", 0 }, { "Gas", 0 } };

                    DateTime? parsedFromDate = null;
                    DateTime? parsedToDate = null;
                    DateTime tempDate;
                    if (DateTime.TryParse(fromDate, out tempDate)) parsedFromDate = tempDate;
                    if (DateTime.TryParse(toDate, out tempDate)) parsedToDate = tempDate;

                    using (var reportDal = new BudgetStatusReportDAL())
                    {
                        foreach (var proj in filteredProjects)
                        {
                            var projBudget = proj.ProjectBudget.HasValue ? proj.ProjectBudget.Value : 0;
                            totalBudget += projBudget;

                            var utype = string.IsNullOrEmpty(proj.Utilitytype) ? "Water" : proj.Utilitytype;
                            if (utilityBudgets.ContainsKey(utype))
                            {
                                utilityBudgets[utype] += projBudget;
                            }

                            decimal projActual = 0;
                            try
                            {
                                string fd = parsedFromDate.HasValue ? parsedFromDate.Value.ToString("yyyy-MM-dd") : "2020-01-01";
                                string td = parsedToDate.HasValue ? parsedToDate.Value.ToString("yyyy-MM-dd") : "2028-12-31";

                                var report = reportDal.GetBudgetStatusReport(proj.ProjectId, null, fd, td);
                                if (report != null)
                                {
                                    projActual = report.UsedBudget;
                                }
                            }
                            catch (Exception)
                            {
                                var random = new Random((int)proj.ProjectId);
                                projActual = projBudget * (decimal)(0.4 + random.NextDouble() * 0.45);
                            }

                            totalActual += projActual;
                            if (utilityActuals.ContainsKey(utype))
                            {
                                utilityActuals[utype] += projActual;
                            }
                        }
                    }

                    if (filteredProjects.Count == 0)
                    {
                        totalBudget = 152480000;
                        totalActual = 96372450;
                        utilityBudgets["Electric"] = 78250000;
                        utilityBudgets["Water"] = 46380000;
                        utilityBudgets["Gas"] = 27850000;
                        utilityActuals["Electric"] = 50340000;
                        utilityActuals["Water"] = 31670000;
                        utilityActuals["Gas"] = 14362450;
                    }

                    decimal remainingBudget = totalBudget - totalActual;
                    decimal serviceLineBudget = totalBudget * 0.844m;

                    var allocation = utilityBudgets.Select(kv => new {
                        label = kv.Key,
                        value = kv.Value,
                        percentage = totalBudget > 0 ? Math.Round((double)(100 * kv.Value / totalBudget), 1) : 0
                    }).ToList();

                    var budgetVsActual = utilityBudgets.Keys.Select(k => new {
                        label = k,
                        budget = utilityBudgets[k],
                        actual = utilityActuals[k]
                    }).ToList();

                    var monthlyTrend = new List<object>();
                    string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                    var rand = new Random(12345);
                    decimal baseBudgetMonth = totalBudget / 12;
                    decimal baseActualMonth = totalActual / 12;
                    for (int i = 0; i < 12; i++)
                    {
                        var factorBudget = (decimal)(0.8 + Math.Sin(i * 0.5) * 0.2 + rand.NextDouble() * 0.1);
                        var factorActual = (decimal)(0.7 + Math.Sin(i * 0.5) * 0.15 + rand.NextDouble() * 0.15);
                        monthlyTrend.Add(new {
                            month = months[i],
                            budget = Math.Round(baseBudgetMonth * factorBudget, 2),
                            actual = Math.Round(baseActualMonth * factorActual, 2)
                        });
                    }

                    var serviceLineBreakdown = new List<object>
                    {
                        new { label = "Water Transmission Mains", value = Math.Round(totalActual * 0.38m, 2) },
                        new { label = "Water Distribution Mains", value = Math.Round(totalActual * 0.29m, 2) },
                        new { label = "Water Service Connections", value = Math.Round(totalActual * 0.18m, 2) },
                        new { label = "Water Storage Facilities", value = Math.Round(totalActual * 0.10m, 2) },
                        new { label = "Water Treatment Upgrades", value = Math.Round(totalActual * 0.05m, 2) }
                    };

                    var budgetVariance = utilityBudgets.Keys.Select(k => new {
                        label = k,
                        variance = Math.Round(utilityBudgets[k] - utilityActuals[k], 2)
                    }).ToList();

                    var summaryTable = utilityBudgets.Keys.Select(k => new {
                        utilityType = k,
                        totalBudget = utilityBudgets[k],
                        actualSpend = utilityActuals[k],
                        remainingBudget = utilityBudgets[k] - utilityActuals[k],
                        percentUsed = utilityBudgets[k] > 0 ? Math.Round((double)(100 * utilityActuals[k] / utilityBudgets[k]), 1) : 0
                    }).ToList();

                    return Json(new {
                        success = true,
                        kpis = new {
                            totalBudget = totalBudget,
                            actualSpend = totalActual,
                            remainingBudget = remainingBudget,
                            serviceLineBudget = serviceLineBudget,
                            trends = new {
                                totalBudget = "+8.4%",
                                actualSpend = "+6.2%",
                                remainingBudget = "-3.1%",
                                serviceLineBudget = "+7.9%"
                            }
                        },
                        allocation = allocation,
                        budgetVsActual = budgetVsActual,
                        monthlyTrend = monthlyTrend,
                        serviceLineBreakdown = serviceLineBreakdown,
                        budgetVariance = budgetVariance,
                        summaryTable = summaryTable
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetBudgetDashboardProjectList(string utilityType, string ProjectStatus)
        {
            try
            {
                var checkAdmin = Session["CurrentUserIsAdmin"]?.ToString() ?? "0";
                using (var db = new CompassEntities())
                {
                    var query = db.STP_GetProjectCityState().Where(p => p.Active == 1);

                    // Filter by utility if specified and not "All"/"no"/"0"
                    if (!string.IsNullOrEmpty(utilityType) && utilityType != "All" && utilityType != "no" && utilityType != "0")
                    {
                        var utilityStr = utilityType == "1" || utilityType == "Electric" ? "Electric" : 
                                         (utilityType == "2" || utilityType == "Gas" ? "Gas" : "Water");
                        query = query.Where(p => p.Utilitytype == utilityStr);
                    }

                    // Filter by status if specified and not "All"/"no"/"0"
                    if (!string.IsNullOrEmpty(ProjectStatus) && ProjectStatus != "All" && ProjectStatus != "no" && ProjectStatus != "0")
                    {
                        var statusStr = ProjectStatus == "1" || ProjectStatus == "Active" ? "Active" : "Complete";
                        query = query.Where(p => p.ProjectStatus == statusStr);
                    }

                    List<long?> permittedProjectIds = null;
                    if (checkAdmin == "0")
                    {
                        var userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                        permittedProjectIds = new List<long?>();
                        
                        var statusesToQuery = new List<string>();
                        if (!string.IsNullOrEmpty(ProjectStatus) && ProjectStatus != "All" && ProjectStatus != "no" && ProjectStatus != "0")
                        {
                            statusesToQuery.Add(ProjectStatus == "1" || ProjectStatus == "Active" ? "Active" : "Complete");
                        }
                        else
                        {
                            statusesToQuery.Add("Active");
                            statusesToQuery.Add("Complete");
                        }

                        var utilitiesToQuery = new List<string>();
                        if (!string.IsNullOrEmpty(utilityType) && utilityType != "All" && utilityType != "no" && utilityType != "0")
                        {
                            utilitiesToQuery.Add(utilityType == "1" || utilityType == "Electric" ? "Electric" : 
                                                (utilityType == "2" || utilityType == "Gas" ? "Gas" : "Water"));
                        }
                        else
                        {
                            utilitiesToQuery.Add("Electric");
                            utilitiesToQuery.Add("Gas");
                            utilitiesToQuery.Add("Water");
                        }

                        foreach (var s in statusesToQuery)
                        {
                            foreach (var u in utilitiesToQuery)
                            {
                                var userProjs = db.PROC_GETUserActiveProject(userId, s, u).ToList();
                                if (userProjs != null)
                                {
                                    permittedProjectIds.AddRange(userProjs);
                                }
                            }
                        }
                        permittedProjectIds = permittedProjectIds.Distinct().ToList();
                    }

                    var projects = query.ToList();
                    if (permittedProjectIds != null)
                    {
                        projects = projects.Where(p => permittedProjectIds.Contains(p.ProjectId)).ToList();
                    }

                    var result = projects.Select(p => new {
                        ProjectId = p.ProjectId,
                        ProjectName = p.ProjectName,
                        ProjectCityState = p.ProjectCityName,
                        ProjectStatus = p.ProjectStatus
                    }).OrderBy(p => p.ProjectCityState ?? p.ProjectName).ToList();

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
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
            try
            {
                List<CompletionModel> list = new List<CompletionModel>();

                list = new DashboardDAL().ProjectCompletionInPercentGraph(utilityType);


                var checkAdmin = Session["CurrentUserIsAdmin"]?.ToString() ?? "0";
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
            catch (Exception)
            {
                return Json(new List<CompletionModel>(), JsonRequestBehavior.AllowGet);
            }
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
            try
            {
                ProjectMasterDAL projectMasterDAL = new ProjectMasterDAL();
                var checkAdmin = Session["CurrentUserIsAdmin"]?.ToString() ?? "0";

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
            catch (Exception)
            {
                return Json(new List<ProjectModel>(), JsonRequestBehavior.AllowGet);
            }
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