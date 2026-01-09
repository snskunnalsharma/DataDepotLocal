using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using KSPL.Indus.MSTS.UI.Filters;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;


namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class InventryStockReportController : Controller
    {
        // GET: Admin/InventryStockReport
        public ActionResult Index()
        {
            InventoryDAL objDAL = new InventoryDAL();

            #region ProjectList

            //var values = objDAL.GetProjectList();
            //values.Add(new ProjectModel { ProjectId = 0, ProjectCityState = "" });
            //IEnumerable<SelectListItem> items = (from value in values
            //                                     select new SelectListItem()
            //                                     {
            //                                         Text = value.ProjectCityState,
            //                                         Value = value.ProjectId.ToString()
            //                                     }).OrderBy(o => o.Text).ToList();



            // ViewBag.ProjectList = items;
            #endregion

            #region InstallerList

            //var installers = objDAL.GetInstallerList().OrderBy(a => a.UserFullName).ToList();
            //installers.Insert(0, new UsersModel { UserId = 0, UserFullName = "All Installers/Managers" });
            ////installers.Add(new UsersModel { UserId = 0, UserFullName = "" });
            //IEnumerable<SelectListItem> installerList = (from value in installers
            //                                             select new SelectListItem()
            //                                             {
            //                                                 Text = value.UserFullName,
            //                                                 Value = value.UserId.ToString()
            //                                             }).ToList();



            //ViewBag.installerList = installerList;
            #endregion

            #region CategoryList

            //var categories = objDAL.GetCategoryList().OrderBy(o => o.CategoryName).ToList();
            //categories.Insert(0, new CategoryBO { CategoryId = 0, CategoryName = "All Category" });
            ////categories.Add(new CategoryBO { CategoryId = 0, CategoryName = "" });
            //IEnumerable<SelectListItem> categoryList = (from value in categories
            //                                            select new SelectListItem()
            //                                            {
            //                                                Text = value.CategoryName,
            //                                                Value = value.CategoryId.ToString()
            //                                            }).ToList();



            //ViewBag.categoryList = categoryList;
            #endregion

            #region ProductList

            //var products = objDAL.GetProductMakeTypeList().OrderBy(o => o.ProductName).ToList();
            //products.Insert(0, new ProductMasterBO { Id = 0, ProductName = "All Products" });
            ////products.Add(new ProductMasterBO { Id = 0, ProductName = "" });
            //IEnumerable<SelectListItem> productList = (from value in products
            //                                           select new SelectListItem()
            //                                           {
            //                                               Text = value.ProductName,
            //                                               Value = value.Id.ToString()
            //                                           }).ToList();



            //ViewBag.productList = productList;
            #endregion


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

            ViewBag.ProductList = items;

            ViewBag.installerList = items;

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
                    data = new List<ProjectModel>();
                }
                else
                {
                    data = data.Where(o => abcd.Contains(o.ProjectId)).ToList();
                }
            }

            // 🔹 Ascending order (change field if needed)
            data = data.OrderBy(o => o.ProjectName).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetInstallerByProjectId(long ProjectId)
        {
            //List<ProductMasterBO> productList = new List<ProductMasterBO>();
            StatusMessage statusMessage = new StatusMessage();
            List<UsersSmallModel> installerList = new List<UsersSmallModel>();
            List<CommonDropDownClass> ProductList = new List<CommonDropDownClass>();
            try
            {

                InventoryDAL objInventoryDAL = new InventoryDAL();

                installerList = objInventoryDAL.GetProjectInstallers(ProjectId);

                ProductList = objInventoryDAL.GetProductList(ProjectId);


                statusMessage.Success = true;


            }
            catch (Exception ex)
            {
                statusMessage.Success = false;
                statusMessage.Message = "Unable to get Installer list.";
            }
            return Json(new { installerList, ProductList, statusMessage }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetProductbyCategoryId(long categoryId)
        {
            List<ProductMasterBO> productList = new List<ProductMasterBO>();
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                productList = new InventoryDAL().GetProductMakeTypeListbyCategoryId(categoryId);
                statusMessage.Success = true;
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(new { statusMessage, productList }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetData(long projectId, long installerId, long ProductId, string txtFromDate, string txtToDate, long ReportType)
        {
            bool success = false;
            string ErrorMessage = "";
            List<PROC_InstallStockDetailReport_Result> ReportData = new List<PROC_InstallStockDetailReport_Result>();
            List<InstallerStockReportSummaryBO> SummeryReportData = new List<InstallerStockReportSummaryBO>();
            try
            {
                if (ReportType == 0)
                {
                    //summary
                    SummeryReportData = new InventoryDAL().GetInventryStockReportSummery(projectId, installerId, ProductId, txtFromDate, txtToDate);
                    success = true;
                    return Json(new { success, ErrorMessage, SummeryReportData }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //detail
                    ReportData = new InventoryDAL().GetInventryStockReport(projectId, installerId, ProductId, txtFromDate, txtToDate);
                    
                    success = true;
                    return Json(new { success, ErrorMessage, ReportData }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {

                ErrorMessage = "Error in geting report data";
            }

            return Json(new { success, ErrorMessage }, JsonRequestBehavior.AllowGet);

        }


        public void DownloadExcelExport(long projectId, long installerId, long ProductId, string txtFromDate, string txtToDate, long ReportType)
        {

            StatusMessage statusMessage = new StatusMessage();

            List<InventryStockReportExport> inventryStockReportExport = new List<InventryStockReportExport>();
            // List<InventryStockSummaryReportExport> inventryStockSummaryReportExport = new List<InventryStockSummaryReportExport>();

            List<InstallerStockReportSummaryBO> inventryStockSummaryReportExport = new List<InstallerStockReportSummaryBO>();
            WareHouseDAL objWareHouseDAL = new WareHouseDAL();
            DataTable ExportInventryStockReport = null;
            try
            {
                if (ReportType == 0)
                {
                    inventryStockSummaryReportExport = new InventoryDAL().GetInventryStockReportSummery(projectId, installerId, ProductId, txtFromDate, txtToDate);
                    ExportInventryStockReport = new CommonFunctions().ToDataTable(inventryStockSummaryReportExport);
                }
                else
                {
                    //var inventryStockReportExport = new InventoryDAL().ExportInventryStockReport(projectId, installerId, categoryId, ProductId, txtFromDate, txtToDate);
                    var query = new InventoryDAL().GetInventryStockReport(projectId, installerId, ProductId, txtFromDate, txtToDate);
                    if (query != null)
                    {
                        inventryStockReportExport = query.Select(a => new InventryStockReportExport
                        {
                            InstallerName = a.UserName,
                            CategoryName = a.CategoryName,
                            ProductName = a.ProductName,
                            SerialNumber = a.SerialNumber,
                            ScannedInventry = a.AllocatedQty,
                            InstalledInventry = a.InstalledQty,
                            TransferredInventry = a.TransferredQty,
                            ReturnedInventry = a.ReturnedQty,
                            InventryInHand = a.InHandQty,

                        }).ToList();
                    }
                    ExportInventryStockReport = new CommonFunctions().ToDataTable(inventryStockReportExport);
                }
                statusMessage.Success = true;

                #region Export to excel
                GridView gv = new GridView();
                gv.DataSource = ExportInventryStockReport;// data.OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street).ToList();
                gv.DataBind();
                Response.Clear();
                Response.Charset = "";
                Response.BufferOutput = true;
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=InventryStockReport.xlsx");
                System.Data.DataTable dt = ExportInventryStockReport;//new CommonFunctions().ToDataTable(data.ToList());

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
                Server.ClearError();
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
    }
}