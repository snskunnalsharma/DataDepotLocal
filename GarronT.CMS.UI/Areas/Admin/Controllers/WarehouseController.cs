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
using System.Data;
using OfficeOpenXml;
using System.Web.UI.WebControls;
using GarronT.CMS.UI.CustomHelpers;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    public class WarehouseController : Controller
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
            long userId = new UserDetails().GetUserId(User.Identity.GetUserId());

            CompassEntities db = new CompassEntities();

            var projectIds = db.TblProjectInstallers
                               .Where(x => x.tblUsers_UserID == userId && x.Active == 1)
                               .Select(x => x.ProjectID)
                               .ToList();

            WareHouseDAL obj = new WareHouseDAL();

            var objList = obj.GetAllRecord(Status, projectIds, userId);

            return Json(new { objList }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult AddNew()
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Warehouse", "Add");
            if (objPermissionCheck)
            {
                WarehouseBO objWarehouseBO = new WarehouseBO();
                ViewBag.Status = "Create";
                return PartialView("_AddWarehouse", objWarehouseBO);
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";

                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult AddNew(WarehouseBO objWarehouseBO)
        {

            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (ModelState.IsValid)
                {
                    WareHouseDAL objWareHouseDAL = new WareHouseDAL();
                    objWarehouseBO.CurrentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    statusMessage = objWareHouseDAL.CreateOrUpdate(objWarehouseBO);

                }
                else
                {
                    statusMessage.Success = false;
                    statusMessage.Message = "Submit failed. Warehouse information not complete. Please fill the all information and then try.";
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
        public ActionResult GetCity(long StateId)
        {

            WareHouseDAL objWareHouseDAL = new WareHouseDAL();
            var values = objWareHouseDAL.GetActiveCityRecordsForState(StateId);
            objWareHouseDAL.Dispose();
            IEnumerable<SelectListItem> items = (from value in values
                                                 select new SelectListItem()
                                                 {
                                                     Text = value.CityName,
                                                     Value = value.CityID.ToString()
                                                 }).OrderBy(o => o.Text).ToList(); ;



            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(long Id)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Warehouse", "Edit");
            if (objPermissionCheck)
            {
                WareHouseDAL objWareHouseDAL = new WareHouseDAL();
                WarehouseBO objWarehouseBO = objWareHouseDAL.GetRecord(Id);
                ViewBag.Status = "Edit";
                return PartialView("_AddWarehouse", objWarehouseBO);
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";

                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult GetUserBywarehouseId(long warehouseId)
        {

            WareHouseDAL objWareHouseDAL = new WareHouseDAL();
            var userlist = objWareHouseDAL.GetuserbywarehouseId(warehouseId);


            return Json(userlist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(WarehouseBO objWarehouseBO)
        {

            StatusMessage statusMessage = new StatusMessage();


            try
            {
                if (ModelState.IsValid)
                {
                    WareHouseDAL objWareHouseDAL = new WareHouseDAL();
                    objWarehouseBO.CurrentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    statusMessage = objWareHouseDAL.CreateOrUpdate(objWarehouseBO);

                }
                else
                {
                    statusMessage.Success = false;
                    statusMessage.Message = "Submit failed. Warehouse information not complete. Please fill the all information and then try.";
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
            if (Status == false)
            {
                var objPermissionCheck = UserPermissionCheck.checkUserStatus("Warehouse", "Delete");
                if (objPermissionCheck)
                {
                    WareHouseDAL objWareHouseDAL = new WareHouseDAL();
                    long CurrentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    statusMessage = objWareHouseDAL.ActivateOrDeactivate(Id, Status, CurrentUserId);
                    return Json(new { statusMessage }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    statusMessage.Success = false;
                    statusMessage.Message = UserPermissionCheck.PerMessage;
                    return Json(new { statusMessage }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                WareHouseDAL objWareHouseDAL = new WareHouseDAL();
                long CurrentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                statusMessage = objWareHouseDAL.ActivateOrDeactivate(Id, Status, CurrentUserId);
                return Json(new { statusMessage }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult GetUserList()
        {
            WareHouseDAL objWareHouseDAL = new WareHouseDAL();
            var userList = objWareHouseDAL.GetUserList();
            return Json(userList, JsonRequestBehavior.AllowGet);
        }



    }
}