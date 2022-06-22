using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using GarronT.CMS.UI.CustomHelpers;
using KSPL.Indus.MSTS.UI.Filters;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{

    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class InventoryTransferController : Controller
    {
        // GET: Admin/InventoryTransferController
        public ActionResult Index()
        {
            return View();
        }

        #region Inventorty Transfer Report
        public ActionResult InventoryTransferReport()
        {
            InventoryDAL objDAL = new InventoryDAL();
            var utilitylist = objDAL.GetUtilityTypeList();
            utilitylist.Add(new InventoryUtilityTypeBO { Id = 0, UtilityName = "" });
            IEnumerable<SelectListItem> utility = (from value in utilitylist
                                                   select new SelectListItem()
                                                   {
                                                       Text = value.UtilityName,
                                                       Value = value.Id.ToString()
                                                   }).OrderBy(o => o.Text).ToList();


            ViewBag.UtilityListItem = utility;

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "", Value = "0" });

            ViewBag.ProjectList = items;

            ViewBag.WarehouseList = items;

            ViewBag.ProductList = items;
            #endregion

            return View();
        }

        [HttpGet]
        public ActionResult GetProject(int UtilityId)
        {
            var checkAdmin = Session["CurrentUserIsAdmin"].ToString();

            InventoryDAL objInventoryDAL = new InventoryDAL();
            List<ProjectModel> data = objInventoryDAL.GetInventoryProjectName(UtilityId);

            if (checkAdmin == "0")
            {
                var userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                var abcd = new ProjectMasterDAL().GetUserProjectList(userId);


                if (abcd == null || abcd.Count == 0)
                {
                    if (data != null && data.Count() > 0)
                    {
                        data = data.Take(0).ToList();
                    }
                }
                else
                {
                    if (data != null && data.Count() > 0)
                    {
                        data = data.Where(o => abcd.Contains(o.ProjectId)).ToList();
                    }
                }

            }

            return base.Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetWarehouseAndProduct(long ProjectId)
        {
            InventoryDAL objInventoryDAL = new InventoryDAL();
            List<WarehouseBO> data = objInventoryDAL.GetWareHouseList(ProjectId);
            List<CommonDropDownClass> data1 = objInventoryDAL.GetProductList(ProjectId);
            //List<LocationDropdownList> data2 = objInventoryDAL.GetLocationList(ProjectId);
            return base.Json(new { WarehouseList = data, ProductList = data1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult getLocationList(long ProjectId,long ProductId)
        {
            InventoryDAL objInventoryDAL = new InventoryDAL();
            List<LocationDropdownList> data2 = objInventoryDAL.GetLocationList(ProjectId);
            var toList = data2.Where(o => o.ProductId == ProductId).OrderBy(o => o.Location).ToList();
            //List<LocationDropdownList> data2 = getList(ProjectId, ProductId);
            return base.Json(new {LocationList = toList }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult getToLocationList(long ProjectId, long ProductId, string LocationFromValue)
        {
            InventoryDAL objInventoryDAL = new InventoryDAL();
            List<LocationDropdownList> data2 = objInventoryDAL.GetLocationList(ProjectId);
            var toList = data2.Where(o => o.ProductId == ProductId && o.Location != LocationFromValue).OrderBy(o => o.Location).ToList();
            //List<LocationDropdownList> data2 = getList(ProjectId, ProductId);
            //var toList = data2.Where(o => o.Location != LocationFromValue);
            return base.Json(new { LocationList = toList }, JsonRequestBehavior.AllowGet);
        }

        ///<summary>
        /// <param name="warehouseId"></param>
        /// <param name="categoryId"></param>
        /// <param name="productId"></param>
        /// <param name="LocationFromValue"></param>
        /// <returns></returns>
        /// </summary>
        [HttpPost]
        public ActionResult GetStockReport(long projectId, long warehouseId, long productId,string LocationFromValue)
        {
            StatusMessage statusMessage = new StatusMessage();
            long ablInventory = 0;
            WareHouseDAL objWareHouseDAL = new WareHouseDAL();
            try
            {
                ablInventory = objWareHouseDAL.GetWarehouseStockInventoryReport(warehouseId, projectId, productId, LocationFromValue);
                statusMessage.Success = true;

            }
            catch (Exception ex)
            {
                statusMessage.Success = false;
                statusMessage.Message = "Unable to get report data";
            }
            var jsonResult = Json(new { statusMessage, ablInventory }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
            //return Json(new { statusMessage, ablInventory }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult getInventoryData(long projectId, long warehouseId, long productId, string locationFromValue, int transferQty)
        {
            long[] w = { warehouseId };
            StatusMessage statusMessage = new StatusMessage();
            WareHouseDAL objWareHouseDAL = new WareHouseDAL();
            List<WarehouseStockReport> WarehouseStockReportData = new List<WarehouseStockReport>();
            try
            {
                WarehouseStockReportData = objWareHouseDAL.getwarehouselistnew(w, projectId, productId, locationFromValue).Take(transferQty).ToList();
                statusMessage.Success = true;

            }
            catch (Exception ex)
            {
                statusMessage.Success = false;
                statusMessage.Message = "Unable to get report data";
            }
            var jsonResult = Json(new { statusMessage, WarehouseStockReportData }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        /// <summary>
        /// Transfer Inventroy
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="ToLocation"></param>
        /// <param name="TransferInventoryList"></param>
        /// <returns></returns>
        public ActionResult saveTransferInventory(long ProjectId, string ToLocation, string[] TransferInventoryList)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                var objPermissionCheck = UserPermissionCheck.checkUserStatus("WarehouseStockReport", "Edit");
                if (objPermissionCheck)
                {
                    long userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    statusMessage = new WareHouseDAL().TransferInventory(ProjectId, ToLocation, TransferInventoryList, userId);
                }
                else
                {
                    statusMessage.Success = false;
                    statusMessage.Message = "You do not have permission to transfer inventory.";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Message = ex.Message;
            }
            return Json(statusMessage, JsonRequestBehavior.AllowGet);
        }

    }
}