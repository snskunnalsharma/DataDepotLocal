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
    public class InventoryBulkTransferController : Controller
    {
        // GET: Admin/InventoryBulkTransfer
        public ActionResult Index()
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

            #region Report Type
            List<SelectListItem> reportType = new List<SelectListItem>() { new SelectListItem {
                Text = "Summary",
                Value="0"
            } ,
            /*new SelectListItem {
                Text = "Detail",
                Value="1"
            }*/ }.ToList();

            ViewBag.ReportType = reportType;
            ViewBag.LocationType = items;
            #endregion
            return View();
        }

        #region InventoryBulkTransfer

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
            List<LocationDropdownList> data2 = GetLocationList(ProjectId);
            return base.Json(new { WarehouseList = data, ProductList = data1, LocationType = data2 }, JsonRequestBehavior.AllowGet);
        }

        public List<LocationDropdownList> GetLocationList(long ProjectId)
        {
            CompassEntities CompassEntities = new CompassEntities();
            List<LocationDropdownList> data = CompassEntities.StockDetails.Where(p => p.FKProjectId == ProjectId && p.Active == 1 && p.Location != "" && p.Location != null).OrderBy(o => o.Location).Select(o =>
                                                         new LocationDropdownList
                                                         {
                                                             LocationId = o.Location,
                                                             Location = o.Location
                                                         }).Distinct().ToList();
            return data;
        }

        [HttpGet]
        public ActionResult getToLocationList(long ProjectId, long ProductId, string LocationFromValue)
        {
            InventoryDAL objInventoryDAL = new InventoryDAL();
            List<LocationDropdownList> data1 = GetAllLocations(ProjectId);
            //List<LocationDropdownList> data2 = GetLocationList(ProjectId);
            var toList = data1.Where(o => o.Location != LocationFromValue).OrderBy(o => o.Location).ToList();
            /*List<LocationDropdownList> data2 = objInventoryDAL.GetLocationList(ProjectId);
            var toList = data2.Where(o => o.ProductId == ProductId && o.Location != LocationFromValue).OrderBy(o => o.Location).ToList();*/
            return base.Json(new { LocationList = toList }, JsonRequestBehavior.AllowGet);
        }

        public List<LocationDropdownList> GetAllLocations(long ProjectId)
        {
            CompassEntities CompassEntities = new CompassEntities();
            List<LocationDropdownList> data = new List<LocationDropdownList>();
            InstallerDAL inDal = new InstallerDAL();
            string dyOfficeColName = inDal.getDynamicColumnName("OFFICE", Convert.ToString(ProjectId));
            if (!string.IsNullOrEmpty(dyOfficeColName))
            {
                var tempdata = (from u in CompassEntities.UploadedExcelDatas.AsEnumerable()
                        where u.FKProjectId == 10085 && u.Active == 1
                        && (string)u.GetType().GetProperty(dyOfficeColName).GetValue(u, null) != ""
                        && (string)u.GetType().GetProperty(dyOfficeColName).GetValue(u, null) != null
                        select new LocationDropdownList
                        {
                            LocationId = (string)u.GetType().GetProperty(dyOfficeColName).GetValue(u, null),
                            Location = (string)u.GetType().GetProperty(dyOfficeColName).GetValue(u, null)
                        }).ToList();
                 var objData = tempdata.GroupBy(o => o.Location).OrderBy(o => o.Key).ToList();
                foreach (var i in objData)
                {
                    data.Add(new LocationDropdownList
                    {
                        Location= i.Key.ToString(),
                        LocationId= i.Key.ToString()
                    });
                }
            }
                return data;
        }

        /// <summary>
        /// 24-jul-2017
        /// Aniket
        /// GetStockReport
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="categoryId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetStockReport(long projectId, long[] warehouseId, long productId, long ReportType, string LocationType)
        {
            StatusMessage statusMessage = new StatusMessage();
            List<WarehouseStockReport> WarehouseStockReportList = new List<WarehouseStockReport>();

            List<WarehouseStockReportExport> warehouseStockReportExport = new List<WarehouseStockReportExport>();
            WareHouseDAL objWareHouseDAL = new WareHouseDAL();
            try
            {
                if (ReportType == 0)
                {
                    WarehouseStockReportList = objWareHouseDAL.GetWarehouseStockReport_Summary(warehouseId, projectId, productId, LocationType);
                }
                else
                {
                    WarehouseStockReportList = objWareHouseDAL.getwarehouselistnew(warehouseId, projectId, productId, LocationType);
                }
                statusMessage.Success = true;

            }
            catch (Exception ex)
            {
                statusMessage.Success = false;
                statusMessage.Message = "Unable to get report data";
            }
            var jsonResult = Json(new { statusMessage, WarehouseStockReportList }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult GetTransferInventoryDetail(long projectId, long warehouseId, long productId, string locationFromValue, int transferQty)
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
        /// <param name="warehouseId"></param>
        /// <param name="ToLocation"></param>
        /// <param name="TransferInventoryList"></param>
        /// <returns></returns>
        public ActionResult saveTransferInventory(long ProjectId,long warehouseId, long productId, string ToLocation, string[] TransferInventoryList)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                var objPermissionCheck = UserPermissionCheck.checkUserStatus("WarehouseStockReport", "Edit");
                if (objPermissionCheck)
                {
                    long userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    statusMessage = new WareHouseDAL().TransferInventoryNew(ProjectId, warehouseId, productId, ToLocation, TransferInventoryList, userId);
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

        
        /// <summary>
        /// 25-Jul-2017
        /// Aniket
        /// Export excel report
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="categoryId"></param>
        /// <param name="productId"></param>
        public void DownloadExcelExport(long projectId, string warehouseId, long productId, long ReportType)
        {

            StatusMessage statusMessage = new StatusMessage();


            List<WarehouseStockReportExport> warehouseStockReportExport = new List<WarehouseStockReportExport>();
            List<WarehouseStockReportSummeryExport> warehouseStockReportSummeryExport = new List<WarehouseStockReportSummeryExport>();
            DataTable ExportWarehouseStockReport = null;


            WareHouseDAL objWareHouseDAL = new WareHouseDAL();
            try
            {
                long[] warehouseList = warehouseId.Split(',').Select(Int64.Parse).ToArray();

                if (ReportType == 0)
                {
                    warehouseStockReportSummeryExport = objWareHouseDAL.exportwarehouseSummeryReport(warehouseList, projectId, productId);
                    ExportWarehouseStockReport = new CommonFunctions().ToDataTable(warehouseStockReportSummeryExport);
                }
                else
                {
                    warehouseStockReportExport = objWareHouseDAL.exportwarehouseReport(warehouseList, projectId, productId);
                    ExportWarehouseStockReport = new CommonFunctions().ToDataTable(warehouseStockReportExport);
                }
                statusMessage.Success = true;

                //DataTable ExportWarehouseStockReport = new CommonFunctions().ToDataTable(warehouseStockReportExport);

                #region Export to excel
                GridView gv = new GridView();
                gv.DataSource = ExportWarehouseStockReport;// data.OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street).ToList();
                gv.DataBind();
                Response.Clear();
                Response.Charset = "";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=WarehouseStockReport.xlsx");
                System.Data.DataTable dt = ExportWarehouseStockReport;//new CommonFunctions().ToDataTable(data.ToList());

                // gv.DataSource as System.Data.DataTable;
                using (ExcelPackage pck = new ExcelPackage())
                {

                    ExcelWorksheet wsDt = pck.Workbook.Worksheets.Add("Sheet1");
                    wsDt.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.None);
                    wsDt.Cells[wsDt.Dimension.Address].AutoFitColumns();

                    Response.BinaryWrite(pck.GetAsByteArray());
                }

                Response.Flush();
                Response.End();
                #endregion

            }
            catch (Exception)
            {

                statusMessage.Success = false;
                statusMessage.Message = "Unable to export report data";
            }

            // return Json(new { statusMessage}, JsonRequestBehavior.AllowGet);
            //return new EmptyResult();
        }


        #endregion
    }
}