using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using log4net;
using KSPL.Indus.MSTS.UI.Filters;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class InventoryMasterController : Controller
    {

        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(CMS_MobileService));
        #endregion


        //
        // GET: /Admin/InventoryMaster/
        public ActionResult Index()
        {
            return View();
        }


        //GetAllData
        [HttpGet]
        public ActionResult GetAllData(bool Status)
        {
            StatusMessage statusMessage = new StatusMessage();

            InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();
            List<ProductMasterBO> objList = objInventoryMasterDAL.GetAllRecord(Status);
            return Json(new { objList }, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public ActionResult AddNew()
        {

            ProductMasterBO objProductMasterBO = new ProductMasterBO();

            ViewBag.Status = "Create";
            return PartialView("_AddInventory", objProductMasterBO);
        }


        [HttpGet]
        public ActionResult GetDropDownData(long RecordId)
        {
            InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();

            ProductMasterBO objInventoryMasterBO = new ProductMasterBO();


            if (RecordId > 0)
            {
                objInventoryMasterBO=objInventoryMasterDAL.GetRecord(RecordId);

            }

            objInventoryMasterBO.UtilityTypeList = objInventoryMasterDAL.GetUtilityTypeList();

            objInventoryMasterBO.MeterSizeList = objInventoryMasterDAL.GetMeterSizeList();
            objInventoryMasterBO.MeterTypeList = objInventoryMasterDAL.GetMeterTypeList();
            objInventoryMasterBO.MeterMakeList = objInventoryMasterDAL.GetMeterMakeList();

            return Json(new { objInventoryMasterBO }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddNew(ProductMasterBO objInventoryMasterBO)
        {

            StatusMessage statusMessage = new StatusMessage();

            try
            {
                if (ModelState.IsValid)
                {
                    InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();
                    objInventoryMasterBO.CurrentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    statusMessage = objInventoryMasterDAL.CreateOrUpdate(objInventoryMasterBO);

                }
                else
                {
                    statusMessage.Success = false;
                    statusMessage.Message = "Submit failed. Inventory information not complete. Please fill the all information and then try.";
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
            InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();
            ProductMasterBO objInventoryMasterBO = objInventoryMasterDAL.GetRecord(Id);

            ViewBag.Status = "Edit";
            return PartialView("_AddInventory", objInventoryMasterBO);
        }


        [HttpPost]
        public ActionResult Edit(ProductMasterBO objInventoryMasterBO)
        {

            StatusMessage statusMessage = new StatusMessage();


            try
            {
                if (ModelState.IsValid)
                {
                    InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();
                    objInventoryMasterBO.CurrentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    statusMessage = objInventoryMasterDAL.CreateOrUpdate(objInventoryMasterBO);

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

            InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();
            long CurrentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
            statusMessage = objInventoryMasterDAL.ActivateOrDeactivate(Id, Status, CurrentUserId);
            return Json(new { statusMessage }, JsonRequestBehavior.AllowGet);
        }



    }
}