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

        public ActionResult GetAddressMap(long projectID, long InstallerID, long formTypeId)
        {
            List<addressVisitInfo> list = new List<addressVisitInfo>();

            //long projectID = 1;
            try
            {
                if (formTypeId == 2)
                {
                    list = new ProjectStatusDAL().GetProjectStaus(projectID, InstallerID, formTypeId);
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
                using (ProjectStatusDAL obj = new ProjectStatusDAL())
                {
                    obj1 = obj.GeInstallerListCurrentProject(projectId, formType).OrderBy(a => a.UserName).ToList();
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


        #endregion
    }
}