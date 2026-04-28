using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using KSPL.Indus.MSTS.UI.Filters;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class INVInstallerReportController : Controller
    {
        public ActionResult Index()
        {
            InventoryDAL objDAL = new InventoryDAL();
            var utilitylist = objDAL.GetUtilityTypeList();
            utilitylist.Add(new InventoryUtilityTypeBO { Id = 0, UtilityName = "" });
            IEnumerable<SelectListItem> utility = (from value in utilitylist
                                                   select new SelectListItem()
                                                   {
                                                       Text = value.UtilityName,
                                                       Value = value.Id.ToString()
                                                   }).OrderBy(o => o.Text).ToList();

            ViewBag.UtilityListItem = utility;

            List<SelectListItem> empty = new List<SelectListItem>
            {
                new SelectListItem { Text = "", Value = "0" }
            };
            ViewBag.ProjectList = empty;
            ViewBag.InstallerList = empty;

            return View();
        }

        [HttpGet]
        public ActionResult GetProject(int UtilityId)
        {
            var checkAdmin = Session["CurrentUserIsAdmin"]?.ToString() ?? "1";

            InventoryDAL objInventoryDAL = new InventoryDAL();
            List<ProjectModel> data = objInventoryDAL.GetInventoryProjectName(UtilityId);

            if (checkAdmin == "0")
            {
                var userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                var allowedProjects = new ProjectMasterDAL().GetUserProjectList(userId);

                if (allowedProjects == null || allowedProjects.Count == 0)
                {
                    data = new List<ProjectModel>();
                }
                else if (data != null && data.Count > 0)
                {
                    data = data.Where(o => allowedProjects.Contains(o.ProjectId)).ToList();
                }
            }

            data = (data ?? new List<ProjectModel>()).OrderBy(o => o.ProjectName).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetInstallers(long projectId)
        {
            if (projectId <= 0)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }

            var installers = new InventoryDAL().GetProjectInstallers(projectId)
                .OrderBy(o => o.UserName)
                .Select(o => new { UserId = o.UserId, UserName = o.UserName })
                .ToList();

            return Json(installers, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetINVInstallerReportData(long projectId, long installerId, string search)
        {
            // NOTE: This is a placeholder endpoint to render the screen.
            // Hook up SP/DAL here when the report query is finalized.
            var rows = new List<INVInstallerReportRow>();

            return Json(new
            {
                success = true,
                rows
            });
        }

        private class INVInstallerReportRow
        {
            public string CompletionDate { get; set; }
            public string InstallerName { get; set; }
            public string WorkOrderId { get; set; }
            public string SerialNumber { get; set; }
            public string ServiceAddress { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string Status { get; set; }
            public string ProjectName { get; set; }
        }
    }
}

