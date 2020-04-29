using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using KSPL.Indus.MSTS.UI.Filters;
using GarronT.CMS.UI.CustomHelpers;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{

    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class FormTypeController : Controller
    {
        //
        // GET: /Admin/FormType/
        public ActionResult Index()
        {
            return View();
        }

        List<FormTypeDOL> list = new List<FormTypeDOL>();

        [HttpGet]
        public ActionResult GetFormTypeList(bool isActive)
        {
            try
            {
                var List = new FormTypeDAL().GetFormTypeList(isActive);
                return Json(List, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: /Admin/MeterSize/Create
        public ActionResult Create()
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("FormType", "Add");
            var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
            return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);

        }

        // POST: /Admin/MeterSize/Create
        [HttpPost]
        public ActionResult Create(long Id, string FormType, string FormTypeDescription, string[] FType, string[] FtypeBool)
        {
            try
            {

                if (Id == 0)
                {
                    ModelState["ID"].Errors.Clear();
                }
                string returnMessage = "";
                bool success = false;
                FormTypeDOL objFormTypeDOL = new FormTypeDOL();
                objFormTypeDOL.ID = Id;
                objFormTypeDOL.strFormType = FormType;
                objFormTypeDOL.FormTypeDescription = FormTypeDescription;

                success = new FormTypeDAL().CreateOrUpdate(objFormTypeDOL, FType, FtypeBool, out returnMessage);

                return Json(new { success = success, returnMessage = returnMessage }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return View();
            }

        }


        public ActionResult Edit(long editId)
        {
            try
            {
                var objPermissionCheck = UserPermissionCheck.checkUserStatus("FormType", "Edit");
                if (objPermissionCheck == true)
                {
                    FormTypeDOL modelData = new FormTypeDAL().GetFormType(editId);
                    return Json(modelData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                    return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception)
            {
                throw;
            }

        }


        public ActionResult AddNew()
        {
            try
            {
                List<BaseFormTypeModel> modelData = new FormTypeDAL().GetBaseFormType();
                return Json(modelData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }

        }

        // POST: /Admin/MeterSizeModel/Delete/5
        [HttpPost]
        public ActionResult ActivateOrDeactivate(long id, bool activate)
        {
            try
            {
                string returnMessage;
                bool success;

                var objPermissionCheck = UserPermissionCheck.checkUserStatus("FormType", "Delete");
                if (objPermissionCheck == true)
                {
                    if (activate)
                    {
                        success = new FormTypeDAL().ActivateRecord(id, out returnMessage);

                        if (success)
                        {
                            returnMessage = CommonFunctions.strRecordActivated;
                        }
                        else
                        {
                            returnMessage = CommonFunctions.strRecordActivatingError;
                        }
                    }
                    else
                    {

                        success = new FormTypeDAL().Deactivate(id);
                        if (success)
                        {
                            returnMessage = CommonFunctions.strRecordDeactivated;
                        }
                        else
                        {
                            returnMessage = CommonFunctions.strRecordDeactivatingError;
                        }
                    }



                }
                else
                {
                    success = false;
                    returnMessage = UserPermissionCheck.PerMessage;
                }

                return Json(new { success = success, returnMessage = returnMessage }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return View();
            }
        }


    }
}