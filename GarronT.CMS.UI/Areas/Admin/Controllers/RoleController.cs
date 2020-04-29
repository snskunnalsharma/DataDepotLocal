using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using KSPL.Indus.MSTS.UI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    /*
     * -------------------------------------------------------------------------------
     * Create By     : Aniket Jadhav 
     * Created On    : 07-Sep-2016 
     * Description   : Controller for Role Master 
     *--------------------------------------------------------------------------------
     */

    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class RoleController : Controller
    {
        // GET: Admin/Role
        public ActionResult Index()
        {
            return View();
        }


       /// <summary>
       ///To Get Role List
       /// </summary>
       /// <returns></returns>
        [HttpGet]
        public ActionResult GetRoleList()
        {
            List<RoleBO> roleList = new List<RoleBO>();
           
            roleList = new UserRoleDAL().GetRoleList();

            return Json(new { roleList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///To Create New Role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddNewRole(string roleName)
        {
            bool success = false;
            string returnMessage = "Please Enter Role Name";
            if (!string.IsNullOrEmpty(roleName))
            {
                success = new UserRoleDAL().CreateOrUpdate(roleName,out returnMessage);
            }

            return Json(new { success, returnMessage }, JsonRequestBehavior.AllowGet);
        }


    }
}
