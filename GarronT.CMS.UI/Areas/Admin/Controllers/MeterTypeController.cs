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
    public class MeterTypeController : Controller
    {
        // GET: Admin/MeterType
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Admin/MeterTypeModel/GetMeterMakeList
        public ActionResult GetMeterTypeList()
        {
            try
            {
                var list = new MeterTypeDAL().GetMakeMeters();
                return Json(list, JsonRequestBehavior.AllowGet);
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
                var list = new MeterTypeDAL().GetDeActiveRecords();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        // GET: /Admin/MeterTypeModel/Create
        public ActionResult Create()
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("MeterType", "Add");
            if (objPermissionCheck)
            {
                return Json(new { success = true, returnMessage = "" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, returnMessage = UserPermissionCheck.PerMessage }, JsonRequestBehavior.AllowGet);
            }
        }


        // POST: /Admin/MeterTypeModel/Create
        [HttpPost]
        public ActionResult Create(MeterTypeModel MeterTypeModel)
        {
            try
            {
                // TODO: Add insert logic here
                DateTime dt = new CommonFunctions().ServerDate();
                if (MeterTypeModel.ID == 0)
                {
                    ModelState["ID"].Errors.Clear();
                }
                string returnMessage = "";
                bool success = false;

                if (ModelState.IsValid)
                {
                    if (MeterTypeModel.ID == 0)
                    {
                        MeterTypeModel.CreatedBy = null;
                        MeterTypeModel.CreatedOn = dt;
                        MeterTypeModel.Active = 1;
                    }
                    else
                    {
                        MeterTypeModel.Active = 1;
                        MeterTypeModel.ModifiedBy = null;
                        MeterTypeModel.ModifiedOn = dt;
                    }
                    success = new MeterTypeDAL().CreateOrUpdate(MeterTypeModel, out returnMessage);

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
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("MeterType", "Edit");
            if (objPermissionCheck)
            {
                try
                {
                    MeterTypeModel modelData = new MeterTypeDAL().GetByMakeMeterId(editId);
                    return Json(modelData, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                return Json(new { success = false, returnMessage = UserPermissionCheck.PerMessage }, JsonRequestBehavior.AllowGet);
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
        // POST: /Admin/MeterTypeModel/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, string msg)
        {
            try
            {
                string returnMessage;
                bool success;

                var objPermissionCheck = UserPermissionCheck.checkUserStatus("MeterType", "Delete");
                if (objPermissionCheck)
                {
                    if (msg == "Activate")
                    {
                        var model = new MeterTypeDAL().GetAllMakeMeterId(Convert.ToInt64(id));
                        model.Active = 1;

                        success = new MeterTypeDAL().ActivateRecord(Convert.ToInt32(model.ID), out returnMessage);

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
                        long metertypeID = Convert.ToInt64(id);
                        var MID = cmp.TblProjectMasterFieldRelations.Where(o => o.MasterFieldID == metertypeID && o.Active == 1 && o.MasterTableName == "tblMeterType").FirstOrDefault();
                        if (MID == null)
                        {
                            var model = new MeterTypeDAL().GetAllMakeMeterId(Convert.ToInt64(id));
                            model.Active = 0;

                            success = new MeterTypeDAL().CreateOrUpdate(model, out returnMessage);

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
                            returnMessage = "Meter type is Used in Active Project";
                        }


                    }

                }
                else
                {
                    return Json(new { success = false, returnMessage = UserPermissionCheck.PerMessage }, JsonRequestBehavior.AllowGet);
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