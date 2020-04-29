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
    public class ServicesController : Controller
    {
        // GET: Admin/Services
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult GetServicesList()
        {
            try
            {
                var list = new ServiceDAL().GetActiveServiceRecords();
                var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        [HttpGet]
        public ActionResult Create()
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Services", "Add");
            var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
            return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);

        }



        [HttpPost]
        public ActionResult Create(ServicesModel ServicesModel, string objServicePictureList)
        {
            try
            {
                DateTime dt = new CommonFunctions().ServerDate();
                if (ServicesModel.ID == 0)
                {
                    ModelState["ID"].Errors.Clear();
                }
                string returnMessage = "";
                string returnMessage1 = "";
                bool success = false;
                long ID = 0;
                string MSg = "";

                if (ModelState.IsValid)
                {
                    if (ServicesModel.ID == 0)
                    {
                        ServicesModel.CreatedBy = null;
                        ServicesModel.CreatedOn = dt;
                        ServicesModel.Active = 1;
                    }
                    else
                    {
                        ServicesModel.Active = 1;
                        ServicesModel.ModifiedBy = null;
                        ServicesModel.ModifiedOn = dt;
                    }
                    success = new ServiceDAL().CreateOrUpdate(ServicesModel, out returnMessage, out ID);

                    MSg = returnMessage;
                    if (ID == 0)
                        ID = ServicesModel.ID;
                    if (ID > 0)
                    {

                        // bool a = ServicePictureDAL.CreateOrUpdate(ID, objServicePictureList, out returnMessage1);
                        using (CompassEntities CompassEntities = new CompassEntities())
                        {

                            var ClientContactDetails = (from s1 in CompassEntities.TblServicePictures
                                                        where s1.tblService_ServiceId == ID
                                                        select s1).ToList();

                            //Delete it from memory

                            if (ClientContactDetails != null)
                            {
                                foreach (var obj in ClientContactDetails)
                                {
                                    CompassEntities.TblServicePictures.Remove(obj);
                                    //Save to database
                                    CompassEntities.SaveChanges();
                                }
                            }
                            ServicePictureDAL Obj = new ServicePictureDAL();

                            if (objServicePictureList != "null")
                            {
                                List<string> names = objServicePictureList.Split(',').ToList<string>();

                                success = Obj.CreateOrUpdate(ID, names, out returnMessage1);


                            }


                        }


                    }

                }
                return Json(new { success = success, returnMessage = returnMessage }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return View();
            }
        }



        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(string id, string msg)
        {
            try
            {
                string returnMessage;
                bool success;

                var objPermissionCheck = UserPermissionCheck.checkUserStatus("Services", "Delete");
                if (objPermissionCheck)
                {
                    if (msg == "Activate")
                    {
                        var ServicesModel = new ServiceDAL().GetServiceById(Convert.ToInt64(id));
                        ServicesModel.Active = 1;

                        success = new ServiceDAL().ActivateRecord(Convert.ToInt32(id), out returnMessage);

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
                        long meterServicesID = Convert.ToInt64(id);
                        var MID = cmp.TblProjectMasterFieldRelations.Where(o => o.MasterFieldID == meterServicesID
                            && o.Active == 1 && o.MasterTableName == "tblService").FirstOrDefault();
                        if (MID == null)
                        {
                            ServicesModel ServicesModel = new ServicesModel();
                            ServicesModel = new ServiceDAL().GetServiceById(Convert.ToInt64(id));


                            ServicesModel.Active = 0;
                            long TerritoryID = 0;
                            //success = false;
                            // success = new CompanyDAL().ActivateRecord(Convert.ToInt32(model.PlanID), out returnMessage);
                            success = new ServiceDAL().CreateOrUpdate(ServicesModel, out returnMessage, out TerritoryID);
                            if (success)
                            {
                                returnMessage = CommonFunctions.strRecordDeactivated;
                            }


                        }
                        else
                        {
                            success = objPermissionCheck;
                            returnMessage = "Service is Used in Active Project";
                        }
                    }

                }
                else
                {
                    success = false;
                    returnMessage = UserPermissionCheck.PerMessage;//

                }


                return Json(new { success = success, returnMessage = returnMessage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult GetDeActiveRecords()
        {
            var list = new ServiceDAL().GetDeActiveRecords();
            return Json(list, JsonRequestBehavior.AllowGet);
        }






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



        public ActionResult Edit(int ID)
        {

            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Services", "Edit");

            if (objPermissionCheck)
            {
                ServicesModel modelData = new ServicesModel();
                modelData = new ServiceDAL().GetServiceById(ID);

                List<ServicesPicture> ModelCD = new List<ServicesPicture>();
                ModelCD = new ServicePictureDAL().GetServicePictureDeatils(ID);

                return Json(new { modelData = modelData, ModelCD = ModelCD }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}