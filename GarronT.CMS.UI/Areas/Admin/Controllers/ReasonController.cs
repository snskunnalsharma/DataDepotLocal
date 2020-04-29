using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using KSPL.Indus.MSTS.UI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using GarronT.CMS.UI.CustomHelpers;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class ReasonController : Controller
    {
        //
        // GET: /Admin/Reason/
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult GetActiveDeactiveReasonList(bool ActiveStatus)
        {
            try
            {
                var list = new ReasonDAL().GetReasonList(ActiveStatus);
                return Json(new { success = true, status = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, status = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Reason", "Add");
            var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
            return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult Create(string _Reason, string _categoryType, bool imageRequired, bool audioRequired, bool videoRequired, string[] FormTypeList)
        {
            try
            {
                ReasonBO obj = new ReasonBO();
                obj.SkipReason = _Reason;
                obj.CategoryType = _categoryType;
                obj.IsAudioRequired = audioRequired;
                obj.IsVideoRequired = videoRequired;
                obj.IsImageRequired = imageRequired;
                obj.FormTypeList = new List<long>();
                foreach (var item in FormTypeList)
                {
                    obj.FormTypeList.Add(long.Parse(item));
                }

                string _stroutput;
                long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                obj.CreatedBy = currentUserId;

                bool a = new ReasonDAL().InsertRecord(obj, out _stroutput);
                return Json(new { success = a, returnMessage = _stroutput }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, status = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Edit(long skipId, string _Reason, string _categoryType, bool imageRequired, bool audioRequired, bool videoRequired, string[] FormTypeList)
        {
            try
            {

                string _stroutput;
                ReasonBO obj = new ReasonBO();
                obj.SkipReason = _Reason;
                obj.CategoryType = _categoryType;
                obj.SkipId = skipId;
                obj.CreatedBy = new UserDetails().GetUserId(User.Identity.GetUserId());
                obj.IsAudioRequired = audioRequired;
                obj.IsVideoRequired = videoRequired;
                obj.IsImageRequired = imageRequired;
                obj.FormTypeList = new List<long>();
                foreach (var item in FormTypeList)
                {
                    obj.FormTypeList.Add(long.Parse(item));
                }
                bool a = new ReasonDAL().UpdateRecord(obj, out _stroutput);
                return Json(new { success = a, returnMessage = _stroutput }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, status = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult GetOldDate(long skipId)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Reason", "Edit");
            if (objPermissionCheck)
            {

                var data = new ReasonDAL().GetEditRecord(skipId);
                return Json(new { success = true, status = data }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult _activateRecord(long id)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Reason", "Edit");
            if (objPermissionCheck)
            {
                bool a = new ReasonDAL().ActivateRecord(id);
                return Json(new { success = a }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
            
        }



        public ActionResult Delete(long id)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Reason", "Delete");
            if (objPermissionCheck)
            {
                bool a = new ReasonDAL().DeactivateRecord(id);
                return Json(new { success = a }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }



        // GET: /Admin/Country/Delete/5
        public ActionResult GetFormType()
        {
            try
            {
                var formTypeList = new ReasonDAL().GetFormTypeRecord();
                return Json(new { success = true, formTypeList = formTypeList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, status = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}