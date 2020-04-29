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
using Microsoft.AspNet.Identity;
using GarronT.CMS.UI.CustomHelpers;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class CustomerController : Controller
    {
        // GET: Admin/Client
        public ActionResult Index()
        {
            TempData["TempClientContactList"] = null;
            return View();
        }

        //  List<DeviceMasterModel> list = new List<DeviceMasterModel>();

        public ActionResult GetCityList(long stateId, string cityName)
        {
            List<CityModel> objList1 = new List<CityModel>();
            //objList1=
            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                objList1 = obj.GetActiveCityRecords(stateId, "");
            }

            return Json(objList1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStateList()
        {
            List<StateModel> objList1 = new List<StateModel>();
            //objList1=
            using (ProjectMasterDAL obj = new ProjectMasterDAL())
            {
                objList1 = obj.GetActiveStateRecords();
            }

            return Json(objList1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClientsList()
        {
            try
            {
                var list = new ClientDAL().GetActiveClientRecords();
                var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ActionResult GetCity(int id)
        {
            try
            {
                var list = new CityDAL().GetCityByIDs(id);
                var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public ActionResult GetState()
        {
            try
            {
                var list = new StateDAL().GetStates();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ActionResult GetStateListByCityID(string city)
        {
            try
            {
                var list = new StateDAL().GetStatesByCityID(city);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //
        //GET: /Admin/Client/Create
        public ActionResult Create()
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Customer", "Add");


            var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
            return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);


        }



        [HttpPost]
        public ActionResult Create(ClientModel ClientModel)
        {
            try
            {
                CompassEntities ObjCompassEntities = new CompassEntities();

                DateTime dt = new CommonFunctions().ServerDate();
                if (ClientModel.ClientId == 0)
                {
                    ModelState["ClientId"].Errors.Clear();
                }
                string returnMessage = "";
                string returnMessage1 = "";
                bool success = false;
                long ClientId = 0;
                string MSg = "";

                if (ModelState.IsValid)
                {
                    if (ClientModel.ClientId == 0)
                    {

                        ClientModel.CreatedBy = null;
                        ClientModel.CreatedOn = dt;
                        ClientModel.Active = 1;
                    }
                    else
                    {
                        ClientModel.Active = 1;
                        ClientModel.ModifiedBy = null;
                        ClientModel.ModifiedOn = dt;
                    }


                    if (ClientModel.State > 0)
                    {
                        long state = Convert.ToInt64(ClientModel.State);
                        var City = (from b in ObjCompassEntities.TblCityMasters.Where(x => x.StateId == state)
                                    select new CityModel
                                    {
                                        CityID = b.CityId
                                    }).FirstOrDefault();
                        if (City != null)
                        {
                            ClientModel.City = ClientModel.City;

                        }
                        else
                        {
                            ClientModel.City = ClientModel.City;
                        }
                    }
                    else
                    {
                        ClientModel.City = ClientModel.City;

                    }
                    success = new ClientDAL().CreateOrUpdate(ClientModel, out returnMessage, out ClientId);

                    MSg = returnMessage;
                    if (ClientId == 0)
                        ClientId = ClientModel.ClientId;
                    if (ClientId > 0)
                    {

                        using (CompassEntities CompassEntities = new CompassEntities())
                        {

                            var ClientContactDetails = (from s1 in CompassEntities.tblClientContactDetails
                                                        where s1.tblClient_ClientId == ClientId
                                                        select s1).ToList();

                            //Delete it from memory

                            if (ClientContactDetails != null)
                            {
                                foreach (var objClientContactDetails in ClientContactDetails)
                                {
                                    CompassEntities.tblClientContactDetails.Remove(objClientContactDetails);
                                    //Save to database
                                    CompassEntities.SaveChanges();
                                }
                            }
                            List<ClientContactDetailsModel> TempClientContactList = TempData["TempClientContactList"] as List<ClientContactDetailsModel>;

                            if (TempClientContactList != null)
                            {
                                foreach (var b in TempClientContactList)
                                {
                                    ClientContactDetailsModel objCatAttr = new ClientContactDetailsModel();
                                    objCatAttr.FirstName = b.FirstName;
                                    objCatAttr.tblClient_ClientId = ClientId;
                                    objCatAttr.LastName = b.LastName;
                                    objCatAttr.MobileNumber = b.MobileNumber;
                                    objCatAttr.EmailId = b.EmailId;
                                    objCatAttr.CreatedBy = null;
                                    objCatAttr.CreatedOn = dt;
                                    objCatAttr.Active = 1;
                                    success = new ClientContactDetailsDAL().CreateOrUpdate(objCatAttr, out returnMessage1);
                                }
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



        public ActionResult Edit(int id)
        {

            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Customer", "Edit");

            if (objPermissionCheck)
            {
                DateTime dt = new CommonFunctions().ServerDate();
                ClientModel modelData = new ClientModel();
                modelData = new ClientDAL().GetClientById(id);

                List<ClientContactDetailsModel> ModelCD = new List<ClientContactDetailsModel>();
                ModelCD = new ClientContactDetailsDAL().GetClientContactDeatils(id);

                var TCount = ModelCD.Count;

                TempData["TempClientContactList"] = ModelCD;
                TempData.Keep("TempClientContactList");


                return Json(new { modelData = modelData, ModelCA = ModelCD, TCount = TCount, }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }


        }

        // GET: /Admin/Client/EditContactDetails/5
        public ActionResult EditContactDetails(int id)
        {

            List<ClientContactDetailsModel> tempdata = TempData["TempClientContactList"] as List<ClientContactDetailsModel>;
            var item = new ClientContactDetailsModel();
            if (tempdata != null)
            {
                if (id != 0)
                {
                    item = tempdata.SingleOrDefault(x => x.TempID == id);
                }
            }
            TempData["TempClientContactList"] = tempdata;
            TempData.Keep("TempClientContactList");
            return Json(item, JsonRequestBehavior.AllowGet);

        }


        //
        // GET: /Admin/Client/Delete/5
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

                var objPermissionCheck = UserPermissionCheck.checkUserStatus("Customer", "Delete");
                if (objPermissionCheck)
                {

                    if (msg == "Activate")
                    {
                        var ClientModel = new ClientDAL().GetClientById(Convert.ToInt64(id));
                        ClientModel.Active = 1;

                        success = new ClientDAL().ActivateRecord(Convert.ToInt32(id), out returnMessage);

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
                        long id1 = Convert.ToInt64(id);
                        var find = cmp.tblProjects.Where(o => o.ClientId == id1 && o.Active == 1).FirstOrDefault();
                        if (find == null)
                        {
                            ClientModel ClientModel = new ClientModel();
                            ClientModel = new ClientDAL().GetClientById(Convert.ToInt64(id));


                            ClientModel.Active = 0;
                            long TerritoryID = 0;
                            //success = false;
                            // success = new CompanyDAL().ActivateRecord(Convert.ToInt32(model.PlanID), out returnMessage);
                            success = new ClientDAL().CreateOrUpdate(ClientModel, out returnMessage, out TerritoryID);
                            if (success)
                            {
                                returnMessage = CommonFunctions.strRecordDeactivated;
                            }

                        }
                        else
                        {
                            success = false;
                            returnMessage = "Client is used in Active Project";
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



        [HttpPost]
        public ActionResult GetDeActiveRecords()
        {


            var list = new ClientDAL().GetDeActiveRecords();
            return Json(list, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public ActionResult TmpListClear()
        {
            List<ClientContactDetailsModel> tempdata = TempData["TempClientContactList"] as List<ClientContactDetailsModel>;
            TempData.Keep("TempClientContactList");
            if (tempdata != null)
            {
                tempdata.Clear();
            }
            TempData.Keep("TempClientContactList");
            return Json(tempdata, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DuplicateCategoryCheck(int id)
        {
            List<ClientContactDetailsModel> tempdata = TempData["TempClientContactList"] as List<ClientContactDetailsModel>;

            bool flag = false;
            if (tempdata != null)
            {

                if (id != 0)
                {
                    var item = tempdata.SingleOrDefault(x => x.TempID == id);
                    if (item != null)
                        flag = true;

                }
                //return Json(new { TempClientContactList, flag }, JsonRequestBehavior.AllowGet);

            }

            TempData.Keep("TempClientContactList");
            return Json(new { success = flag, returnMessage = "" }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult TmpListRemove(int id)
        {

            List<ClientContactDetailsModel> tempdata = TempData["TempClientContactList"] as List<ClientContactDetailsModel>;
            if (tempdata != null)
            {
                if (id != 0)
                {

                    var item = tempdata.SingleOrDefault(x => x.TempID == id);
                    if (item != null)
                        tempdata.Remove(item);

                }
            }

            TempData["TempClientContactList"] = tempdata;
            TempData.Keep("TempClientContactList");
            if (tempdata == null)
            {
                tempdata = new List<ClientContactDetailsModel>();
            }
            return Json(tempdata, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult TmpList(ClientContactDetailsModel CategoryAttributeModel)
        {

            List<ClientContactDetailsModel> tempdata = new List<ClientContactDetailsModel>();

            if (TempData["TempClientContactList"] == null)
            {
                tempdata.Add(CategoryAttributeModel);
            }
            else
            {
                tempdata = TempData["TempClientContactList"] as List<ClientContactDetailsModel>;

                if (tempdata != null)
                {
                    tempdata.Add(CategoryAttributeModel);
                }
                else
                {
                    tempdata = new List<ClientContactDetailsModel>();
                    tempdata.Add(CategoryAttributeModel);
                    TempData["TempClientContactList"] = tempdata;
                }
            }
            TempData["TempClientContactList"] = tempdata;
            TempData.Keep("TempClientContactList");
            return Json(tempdata, JsonRequestBehavior.AllowGet);

        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public ActionResult AuditSettings(long CustomerId)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Customer", "Edit");

            if (objPermissionCheck)
            {
                ViewBag.CustomerId = CustomerId;
                return View();
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="activeDeactive"></param>
        /// <returns></returns>
        public ActionResult GetAuditPercentage(long customerId, int activeDeactive)
        {
            try
            {
                var list = new ClientDAL().GetCustomerAuditSettings(customerId, activeDeactive);
                return Json(new { status = true, list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="activeDeactive"></param>
        /// <returns></returns>
        public ActionResult GetFailPercentage(long customerId, int activeDeactive)
        {

            try
            {
                var list = new ClientDAL().GetCustomerFailSettings(customerId, activeDeactive);
                return Json(new { status = true, list = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="auditId"></param>
        /// <param name="auditPercent"></param>
        /// <param name="startDay"></param>
        /// <param name="endDay"></param>
        /// <returns></returns>
        public ActionResult SaveAuditPercentage(string customerId, string auditId, string auditPercent, string startDay, string endDay)
        {
            try
            {
                string resultMessage = "";
                bool result = new ClientDAL().SaveAuditPercentage(long.Parse(customerId), long.Parse(auditId), decimal.Parse(auditPercent), int.Parse(startDay), int.Parse(endDay), out resultMessage);
                return Json(new { status = result, resultMessage = resultMessage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, resultMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="Id"></param>
        /// <param name="failPercent"></param>
        /// <param name="increasedDays"></param>
        /// <returns></returns>
        public ActionResult SaveFailPercentage(string customerId, string Id, string failPercentStart, string failPercentEnd, string increasedDays)
        {
            try
            {
                string resultMessage = "";

                CustomerFailPercentage obj = new CustomerFailPercentage();
                obj.FailPercentageEnd = decimal.Parse(failPercentEnd);
                obj.FailPercentageStart = decimal.Parse(failPercentStart);
                obj.Id = long.Parse(Id);
                obj.IncreaseByDays = int.Parse(increasedDays);
                obj.userId = int.Parse(new UserDetails().GetUserId(User.Identity.GetUserId()).ToString());
                obj.FK_CustomerId = long.Parse(customerId);
                bool result = new ClientDAL().SaveFailPercentage(obj, out resultMessage);
                return Json(new { status = result, resultMessage = resultMessage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, resultMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditId"></param>
        /// <returns></returns>
        public ActionResult GetAuditPercentageRecord(long auditId)
        {
            try
            {
                var result = new ClientDAL().GetAuditPercentageRecord(auditId);
                return Json(new { status = true, result = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, resultMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult GetFailPercentageRecord(long Id)
        {
            try
            {
                var result = new ClientDAL().GetFailPercentageRecord(Id);
                return Json(new { status = true, result = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, resultMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="isAuditId"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public ActionResult ActivateDeactivateRecord(long Id, bool isAuditId, bool isActive)
        {
            try
            {
                StatusMessage statusMessage = new StatusMessage();
                var objPermissionCheck = UserPermissionCheck.checkUserStatus("Customer", "Delete");
                if (objPermissionCheck)
                {
                    string resultMessage = "";
                    var result = new ClientDAL().ActivateDeactivateRecord(Id, isAuditId, isActive, out resultMessage);
                    return Json(new { status = result, resultMessage = resultMessage }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    statusMessage.Success = false;
                    statusMessage.Message = UserPermissionCheck.PerMessage;
                    return Json(new { statusMessage }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { status = false, resultMessage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }




    }
}