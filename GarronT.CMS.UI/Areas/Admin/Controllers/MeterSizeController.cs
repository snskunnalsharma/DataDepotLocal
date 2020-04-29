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
    public class MeterSizeController : Controller
    {
        // GET: Admin/MeterSize
        public ActionResult Index()
        {
            return View();
        }
        List<MeterSizeModel> list = new List<MeterSizeModel>();

        public ActionResult GetMeterSizeList()
        {
            try
            {
                var List = new MeterSizeDAL().GetMeterSize();
                return Json(List, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public ActionResult GetDeActiveRecords()
        {
            try
            {
                var list = new MeterSizeDAL().GetDeActiveRecords();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        // GET: /Admin/MeterSize/Create
        public ActionResult Create()
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("MeterSize", "Add");
            var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
            return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);

        }


        // POST: /Admin/MeterSize/Create
        [HttpPost]
        public ActionResult Create(MeterSizeModel MeterSizeModel)
        {
            try
            {
                // TODO: Add insert logic here
                DateTime dt = new CommonFunctions().ServerDate();
                if (MeterSizeModel.ID == 0)
                {
                    ModelState["ID"].Errors.Clear();
                }
                string returnMessage = "";
                bool success = false;

                if (ModelState.IsValid)
                {
                    if (MeterSizeModel.ID == 0)
                    {
                        MeterSizeModel.CreatedBy = null;
                        MeterSizeModel.CreatedOn = dt;
                        MeterSizeModel.Active = 1;
                    }
                    else
                    {
                        MeterSizeModel.Active = 1;
                        MeterSizeModel.ModifiedBy = null;
                        MeterSizeModel.ModifiedOn = dt;
                    }
                    success = new MeterSizeDAL().CreateOrUpdate(MeterSizeModel, out returnMessage);

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
            try
            {
                var objPermissionCheck = UserPermissionCheck.checkUserStatus("MeterSize", "Edit");
                if (objPermissionCheck == true)
                {
                    MeterSizeModel modelData = new MeterSizeDAL().GetByMeterSizeId(editId);
                    return Json(modelData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                    return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
        // POST: /Admin/MeterSizeModel/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, string msg)
        {
            try
            {
                string returnMessage;
                bool success;

                var objPermissionCheck = UserPermissionCheck.checkUserStatus("MeterSize", "Delete");
                if (objPermissionCheck == true)
                {
                    if (msg == "Activate")
                    {
                        var model = new MeterSizeDAL().GetByAllMeterSizeID(Convert.ToInt64(id));
                        model.Active = 1;

                        success = new MeterSizeDAL().ActivateRecord(Convert.ToInt32(model.ID), out returnMessage);
                        returnMessage = (success == true ? CommonFunctions.strRecordActivated : CommonFunctions.strRecordActivatingError);

                    }
                    else
                    {

                        CompassEntities cmp = new CompassEntities();
                        long meterSizeID = Convert.ToInt64(id);
                        var MID = cmp.TblProjectMasterFieldRelations.Where(o => o.MasterFieldID == meterSizeID && o.Active == 1 && o.MasterTableName == "tblMeterSize").FirstOrDefault();
                        if (MID == null)
                        {

                            var model = new MeterSizeDAL().GetByAllMeterSizeID(Convert.ToInt64(id));
                            model.Active = 0;

                            success = new MeterSizeDAL().CreateOrUpdate(model, out returnMessage);

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
                            returnMessage = "Meter size is Used in Active Project";
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