
using GarronT.CMS.Model;
using GarronT.CMS.Model.DAL;
using GarronT.CMS.UI.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using GarronT.CMS.Model.BO;
using KSPL.Indus.MSTS.UI.Filters;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class PermissionController : Controller
    {
        //
        // GET: /Admin/Permission/
        public ActionResult Index()
        {
            AspNetPermissionVM AspNetPermissionVM = new AspNetPermissionVM();

            AspNetPermissionVM = new PermissionBAL().GetPermissionData();
            return View(AspNetPermissionVM);
        }



        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult AssignPermissionRole(List<AccessPermissionBo> PermissionList)
        {
            if (PermissionList != null && PermissionList.Count > 0)
            {
                int currentUserId = (int)new UserDetails().GetUserId(User.Identity.GetUserId());
                string Message = "";
                var result = new PermissionBAL().SavePermissionData(PermissionList, currentUserId, out Message);

                return Json(new { result = result, resultMessage = Message }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = false, resultMessage = "Save failed. Please try again!" }, JsonRequestBehavior.AllowGet);
            }

        }


    }


}