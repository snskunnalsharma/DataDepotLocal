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
    public class PictureController : Controller
    {
        //
        // GET: /Admin/Picture/
        public ActionResult Index()
        {
            return View();
        }


        // GET: /Admin/PictureModel/GetMeterMakeList
        public ActionResult GetPictureList()
        {
            try
            {
                var list = new PictureDAL().GetPictures();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public ActionResult GetDeActiveRecords()
        {
            try
            {
                var list = new PictureDAL().GetDeActiveRecords();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        // GET: /Admin/PictureModel/Create
        public ActionResult Create()
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Picture", "Add");
            var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
            return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
        }


        // POST: /Admin/PictureModel/Create
        [HttpPost]
        public ActionResult Create(PictureModel PictureModel)
        {
            try
            {
                // TODO: Add insert logic here
                DateTime dt = new CommonFunctions().ServerDate();
                if (PictureModel.ID == 0)
                {
                    ModelState["ID"].Errors.Clear();
                }
                string returnMessage = "";
                bool success = false;

                if (ModelState.IsValid)
                {
                    if (PictureModel.ID == 0)
                    {
                        PictureModel.CreatedBy = null;
                        PictureModel.CreatedOn = dt;
                        PictureModel.Active = 1;
                    }
                    else
                    {
                        PictureModel.Active = 1;
                        PictureModel.ModifiedBy = null;
                        PictureModel.ModifiedOn = dt;
                    }
                    success = new PictureDAL().CreateOrUpdate(PictureModel, out returnMessage);

                }
                return Json(new { success = success, returnMessage = returnMessage }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return View();
            }
            return RedirectToAction("Index");
        }


        public ActionResult Edit(long editId)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Picture", "Edit");

            if (objPermissionCheck)
            {
                try
                {
                    PictureModel modelData = new PictureDAL().GetByPictureId(editId);
                    return Json(modelData, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }

        }

        //
        // POST: /Admin/MeterMake/Edit/5
        [HttpPost]

        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Admin/Country/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Admin/PictureModel/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, string msg)
        {
            try
            {
                string returnMessage;
                bool success;

                var objPermissionCheck = UserPermissionCheck.checkUserStatus("Picture", "Delete");

                if (objPermissionCheck)
                {

                    if (msg == "Activate")
                    {
                        var model = new PictureDAL().GetAllPictureId(Convert.ToInt64(id));
                        model.Active = 1;

                        success = new PictureDAL().ActivateRecord(Convert.ToInt32(model.ID), out returnMessage);

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

                        CompassEntities cmp = new CompassEntities();
                        long picID = Convert.ToInt64(id);
                        var MID = cmp.TblServicePictures.Where(o => o.tblPicture_PictureId == picID && o.Active == 1).FirstOrDefault();
                        if (MID == null)
                        {

                            var model = new PictureDAL().GetAllPictureId(Convert.ToInt64(id));
                            model.Active = 0;

                            success = new PictureDAL().CreateOrUpdate(model, out returnMessage);

                            if (success)
                            {
                                returnMessage = CommonFunctions.strRecordDeactivated;
                            }
                            else
                            {
                                if (returnMessage == CommonFunctions.strRecordDeactivatingExist)
                                    returnMessage = CommonFunctions.strRecordDeactivatingExist;
                                else
                                    returnMessage = CommonFunctions.strRecordDeactivatingError;
                            }
                        }
                        else
                        {
                            success = false;
                            returnMessage = "Picture is Used in Active Project";
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