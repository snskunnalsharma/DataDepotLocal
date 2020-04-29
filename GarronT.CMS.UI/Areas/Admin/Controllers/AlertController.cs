using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using GarronT.CMS.UI.CustomHelpers;
using KSPL.Indus.MSTS.UI.Filters;
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
    public class AlertController : Controller
    {
        // GET: Admin/Alert
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetActiveDeactiveAlertList(bool ActiveStatus)
        {
            try
            {
                var list = new AlertDAL().GetAlertList(ActiveStatus);
                return Json(new { success = true, status = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, status = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetUserName()
        {
            try
            {
                var list = new AlertDAL().GetUserList();
                return Json(new { data = true, status = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { data = false, status = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetOldData(long AlertID)
        {

            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Alert", "Edit");

            if (objPermissionCheck)
            {
                var data = new AlertDAL().GetEditRecord(AlertID);
                return Json(new { success = true, status = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetUserDetails(string[] Userid)
        {
            try
            {
                var list = new AlertDAL().GetUserDeatils(Userid);
                return Json(new { success = true, status = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, status = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpGet]
        public ActionResult GetEmailConfigureOldData(long AlertID)
        {
            try
            {
                EmailConfigureBO result = new AlertDAL().GetEmailConfigureDetails(AlertID);
                return Json(new { success = true, status = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, status = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Create(EmailConfigureBO __EmailConfigureBO)
        {
            try
            {
                string _stroutput;
                bool a = new AlertDAL().InsertRecord(__EmailConfigureBO, __EmailConfigureBO.Userid, out _stroutput);
                return Json(new { success = a, returnMessage = _stroutput }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, status = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}