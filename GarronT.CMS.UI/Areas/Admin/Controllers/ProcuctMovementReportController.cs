using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KSPL.Indus.MSTS.UI.Filters;




namespace GarronT.CMS.UI.Areas.Admin.Controllers
{


    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class ProcuctMovementReportController : Controller
    {
        // GET: Admin/ProcuctMovementReport
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


            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "", Value = "0" });


            ViewBag.ProjectList = items;

            return View();
        }


        [HttpGet]
        public ActionResult GetProject(int UtilityId)
        {
            var checkAdmin = Session["CurrentUserIsAdmin"].ToString();

            InventoryDAL objInventoryDAL = new InventoryDAL();
            List<ProjectModel> data = objInventoryDAL.GetInventoryProjectName(UtilityId);

            if (checkAdmin == "0")
            {
                var userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                var abcd = new ProjectMasterDAL().GetUserProjectList(userId);


                if (abcd == null || abcd.Count == 0)
                {
                    if (data != null && data.Count() > 0)
                    {
                        data = data.Take(0).ToList();
                    }
                }
                else
                {
                    if (data != null && data.Count() > 0)
                    {
                        data = data.Where(o => abcd.Contains(o.ProjectId)).ToList();
                    }
                }

            }

            return base.Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getReportData(long projectId, string serialnumber)
        {
            StatusMessage StatusMessage = new StatusMessage();
            List<MovementReport> reportdata = new List<MovementReport>();
            try
            {
                reportdata = new InventoryDAL().GetProductMovementReport(projectId,serialnumber);
                StatusMessage.Success = true;
            }
            catch (Exception)
            {
                StatusMessage.Message = "Error in getting report data";
                //throw;
            }

            return Json(new { StatusMessage, reportdata }, JsonRequestBehavior.AllowGet);
        }
    }
}