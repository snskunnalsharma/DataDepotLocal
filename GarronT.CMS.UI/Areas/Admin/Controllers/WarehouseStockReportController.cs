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
    public class WarehouseStockReportController : Controller
    {
        // GET: Admin/WarehouseStockReport
        public ActionResult Index()
        {
            return View();
        }

        #region Stock Report
        public ActionResult StockReport()
        {
            #region WarehouseList

            //var warehouseList = new WareHouseDAL().GetWarehouseListNew();
            //warehouseList.Add(new MobileWarehouseBO { Id = 0, WarehouseName = "" });
            //IEnumerable<SelectListItem> Witems = (from value in warehouseList
            //                                      select new SelectListItem()
            //                                      {
            //                                          Text = value.WarehouseName,
            //                                          Value = value.Id.ToString()
            //                                      }).OrderBy(o => o.Text).ToList();



            //ViewBag.WarehouseList = Witems;
            #endregion

            #region category list

            //var categoryList = new WareHouseDAL().GetCategoryList().OrderBy(a => a.CategoryName).ToList();
            //categoryList.Insert(0, new CategoryBO { CategoryId = 0, CategoryName = "All Category" });
            //IEnumerable<SelectListItem> categories = (from value in categoryList
            //                                          select new SelectListItem()
            //                                          {
            //                                              Text = value.CategoryName,
            //                                              Value = value.CategoryId.ToString()
            //                                          }).OrderBy(o => o.Text).ToList();



            //ViewBag.CategoryList = categories;

            #endregion

            #region product list

            //var productList = new WareHouseDAL().GetProductList().OrderBy(a => a.ProductName).ToList();

            //productList.Insert(0, new ProductMasterBO { Id = 0, ProductName = "All Product" });
            //IEnumerable<SelectListItem> products = (from value in productList
            //                                        select new SelectListItem()
            //                                        {
            //                                            Text = value.ProductName,
            //                                            Value = value.Id.ToString()
            //                                        }).ToList();



            //ViewBag.ProductList = products;

            #endregion


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
            new SelectListItem {
                Text = "Detail",
                Value="1"
            } }.ToList();



            ViewBag.ReportType = reportType;


            // Get Location list for pull on dropdown
            ViewBag.LocationType = items;
            //end hare


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
            data = data.OrderBy(o => o.ProjectName).ToList();

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
        public ActionResult GetStockReport(long projectId, long[] warehouseId, long productId, long ReportType,string LocationType)
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
            //return Json(new { statusMessage, WarehouseStockReportList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductbyCategoryId(long categoryId)
        {
            List<ProductMasterBO> productList = new List<ProductMasterBO>();
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                productList = new InventoryDAL().GetProductMakeTypeListbyCategoryId(categoryId);
                //if (categoryId == 0)
                //{
                //    productList = new WareHouseDAL().GetProductList();
                //}
                //else
                //{
                //    productList = new WareHouseDAL().GetProductList().Where(a => a.CategoryId == categoryId).ToList();
                //}

                statusMessage.Success = true;
            }
            catch (Exception ex)
            {

                throw;
            }
            return Json(new { statusMessage, productList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetStockReportNew()
        {
            StatusMessage statusMessage = new StatusMessage();
            List<WarehouseStockReport> WarehouseStockReportList = new List<WarehouseStockReport>();

            List<WarehouseStockReportExport> warehouseStockReportExport = new List<WarehouseStockReportExport>();
            WareHouseDAL objWareHouseDAL = new WareHouseDAL();
            try
            {
                //Get parameters
                long warehouseId = 0;
                long projectId = 0;
                long productId = 0;

                if (!string.IsNullOrEmpty(Convert.ToString(Request.Form["warehouseId"])))
                {
                    warehouseId = Convert.ToInt64(Request.Form["warehouseId"]);
                }

                if (!string.IsNullOrEmpty(Convert.ToString(Request.Form["projectId"])))
                {
                    projectId = Convert.ToInt64(Request.Form["projectId"]);
                }

                if (!string.IsNullOrEmpty(Convert.ToString(Request.Form["productId"])))
                {
                    productId = Convert.ToInt64(Request.Form["productId"]);
                }


                // get Start (paging start index) and length (page size for paging)
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                //Get Sort columns value

                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0;

                WarehouseStockReportList = objWareHouseDAL.getwarehouselistServerSide(sortColumn, sortColumnDir, searchValue, skip, pageSize, out totalRecords, warehouseId, projectId, productId);

                statusMessage.Success = true;
                return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = WarehouseStockReportList }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {

                statusMessage.Success = false;
                statusMessage.Message = "Unable to get report data";
            }

            return Json(new { statusMessage, WarehouseStockReportList }, JsonRequestBehavior.AllowGet);
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


        /// <summary>
        /// Aniket
        /// 9-Aug-2017
        /// Update stock detail record
        /// </summary>
        /// <param name="StockDetailId"></param>
        /// <param name="Serialnumber"></param>
        /// <param name="Quantity"></param>
        /// <returns></returns>
        public ActionResult SaveEditedRecord(long StockDetailId, string Serialnumber, int Quantity)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                long userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                statusMessage = new WareHouseDAL().UpdateStockRecord(StockDetailId, Serialnumber, Quantity, userId);
            }
            catch (Exception ex)
            {
                statusMessage.Message = ex.Message;
            }
            return Json(statusMessage, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Aniket
        /// 9-Aug-2017
        /// Delete product from stock and inward detail
        /// </summary>
        /// <param name="StockDetailId"></param>
        /// <returns></returns>
        public ActionResult DeleteProductFromStock(long StockDetailId)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                long userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                statusMessage = new WareHouseDAL().DeleteStockRecord(StockDetailId, userId);
            }
            catch (Exception ex)
            {
                statusMessage.Message = ex.Message;
            }
            return Json(statusMessage, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditPermission()
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("WarehouseStockReport", "Edit");
            var strMessage = "";
            if (!objPermissionCheck)
            {
                strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
            }
            return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DeletePermission()
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("WarehouseStockReport", "Delete");

            var strMessage = "";
            if (!objPermissionCheck)
            {
                strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
            }
            return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}