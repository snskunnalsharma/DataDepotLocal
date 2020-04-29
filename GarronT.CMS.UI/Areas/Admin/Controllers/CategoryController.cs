using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KSPL.Indus.MSTS.UI.Filters;
using GarronT.CMS.UI.CustomHelpers;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class CategoryController : Controller
    {
        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(CMS_MobileService));
        #endregion

        //
        // GET: /Admin/Warehouse/
        public ActionResult Index()
        {
            return View();
        }

        //GetAllData
        [HttpGet]
        public ActionResult GetAllData(bool Status)
        {
            StatusMessage statusMessage = new StatusMessage();
            CategoryDAL objCategoryDALL = new CategoryDAL();
            List<CategoryBO> objList = objCategoryDALL.GetAllRecord(Status);
            return Json(new { objList }, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public ActionResult AddNew()
        {

            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Category", "Add");

            if (objPermissionCheck)
            {
                CategoryBO objCategoryBO = new CategoryBO();
                ViewBag.Status = "Create";
                return PartialView("_AddCategory", objCategoryBO);
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult AddNew(CategoryBO objCategoryBO)
        {

            StatusMessage statusMessage = new StatusMessage();

            try
            {

                if (ModelState.IsValid)
                {
                    CategoryDAL objCategoryDAL = new CategoryDAL();
                    objCategoryBO.CurrentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    statusMessage = objCategoryDAL.CreateOrUpdate(objCategoryBO);

                }
                else
                {
                    statusMessage.Success = false;
                    statusMessage.Message = "Submit failed. Category information not complete. Please fill all the information and then try.";
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                statusMessage.Message = ex.Message;
            }

            return Json(new { statusMessage }, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public ActionResult Edit(long Id)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Category", "Edit");

            if (objPermissionCheck)
            {
                CategoryDAL objCategoryDAL = new CategoryDAL();
                CategoryBO objCategoryBO = objCategoryDAL.GetRecord(Id);
                ViewBag.Status = "Edit";
                return PartialView("_AddCategory", objCategoryBO);
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult Edit(CategoryBO objCategoryBO)
        {

            StatusMessage statusMessage = new StatusMessage();


            try
            {
                if (ModelState.IsValid)
                {
                    CategoryDAL objCategoryDAL = new CategoryDAL();
                    objCategoryBO.CurrentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    statusMessage = objCategoryDAL.CreateOrUpdate(objCategoryBO);

                }
                else
                {
                    statusMessage.Success = false;
                    statusMessage.Message = "Submit failed. Category information not complete. Please fill all the information and then try.";
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                statusMessage.Success = false;
                statusMessage.Message = ex.Message;
            }

            return Json(new { statusMessage }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult ActiveDeactive(long Id, bool Status)
        {
            StatusMessage statusMessage = new StatusMessage();
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Category", "Delete");
            if (objPermissionCheck)
            {
                if (Status == false)
                {

                    CategoryDAL objCategoryDAL = new CategoryDAL();
                    long CurrentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    statusMessage = objCategoryDAL.ActivateOrDeactivate(Id, Status, CurrentUserId);
                    return Json(new { statusMessage }, JsonRequestBehavior.AllowGet);


                }
                else
                {

                    CategoryDAL objCategoryDAL = new CategoryDAL();
                    long CurrentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    statusMessage = objCategoryDAL.ActivateOrDeactivate(Id, Status, CurrentUserId);
                    return Json(new { statusMessage }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                statusMessage.Success = false;
                statusMessage.Message = UserPermissionCheck.PerMessage;
                return Json(new { statusMessage }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}