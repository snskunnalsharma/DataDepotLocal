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
    public class MeterMakeController : Controller
    {
        // GET: Admin/MeterMake
        public ActionResult Index()
        {
            return View();
        }


        // GET: /Admin/MeterMakeModel/GetMeterMakeList
        public ActionResult GetMeterMakeList()
        {
            try
            {
                var list = new MeterMakeDAL().GetMakeMeters();
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
                var list = new MeterMakeDAL().GetDeActiveRecords();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        // GET: /Admin/MeterMakeModel/Create
        public ActionResult Create()
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("MeterMake", "Add");
            var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
            return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
        }


        // POST: /Admin/MeterMakeModel/Create
        [HttpPost]
        public ActionResult Create(MeterMakeModel MeterMakeModel)
        {
            try
            {
                // TODO: Add insert logic here
                DateTime dt = new CommonFunctions().ServerDate();
                if (MeterMakeModel.ID == 0)
                {
                    ModelState["ID"].Errors.Clear();
                }
                string returnMessage = "";
                bool success = false;

                if (ModelState.IsValid)
                {
                    if (MeterMakeModel.ID == 0)
                    {
                        MeterMakeModel.CreatedBy = null;
                        MeterMakeModel.CreatedOn = dt;
                        MeterMakeModel.Active = 1;
                    }
                    else
                    {
                        MeterMakeModel.Active = 1;
                        MeterMakeModel.ModifiedBy = null;
                        MeterMakeModel.ModifiedOn = dt;
                    }
                    success = new MeterMakeDAL().CreateOrUpdate(MeterMakeModel, out returnMessage);

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
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("MeterMake", "Edit");

            if (objPermissionCheck)
            {
                try
                {
                    MeterMakeModel modelData = new MeterMakeDAL().GetByMakeMeterId(editId);
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
        // POST: /Admin/MeterMakeModel/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, string msg)
        {
            try
            {
                string returnMessage;
                bool success;

                var objPermissionCheck = UserPermissionCheck.checkUserStatus("MeterMake", "Delete");

                if (objPermissionCheck)
                {
                    if (msg == "Activate")
                    {
                        var model = new MeterMakeDAL().GetAllMakeMeterId(Convert.ToInt64(id));
                        model.Active = 1;

                        success = new MeterMakeDAL().ActivateRecord(Convert.ToInt32(model.ID), out returnMessage);

                        returnMessage = success ? CommonFunctions.strRecordActivated : CommonFunctions.strRecordActivatingError;

                    }
                    else
                    {


                        var model = new MeterMakeDAL().GetAllMakeMeterId(Convert.ToInt64(id));
                        model.Active = 0;

                        success = new MeterMakeDAL().CreateOrUpdate(model, out returnMessage);

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

                }
                else
                {
                    success = objPermissionCheck;
                    returnMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                }
                return Json(new { success = success, returnMessage = returnMessage }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}