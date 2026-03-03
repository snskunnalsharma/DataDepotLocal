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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Newtonsoft.Json;
using System.Data.SqlClient;

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

        [HttpPost]
        public JsonResult UploadCompletedInventory(HttpPostedFileBase file, long projectId)
        {
            try
            {
                if (projectId != 10135)
                    return Json(new { success = false, message = "This feature is enabled only for Project 10135." });

                if (file == null || file.ContentLength == 0)
                    return Json(new { success = false, message = "No file uploaded." });

                CompletedInventoryPayload payload = ReadCompletedInventoryFromFile(file);

                Session["CompletedInventory_" + projectId] = payload;

                return Json(new
                {
                    success = true,
                    serialCount = payload.Serials.Count,
                    productCount = payload.ProductQty.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetCompletedInventoryForCompare(long projectId)
        {
            try
            {
                if (projectId != 10135)
                    return Json(new { success = false, message = "Comparison allowed only for Project 10135." });

                var payload = Session["CompletedInventory_" + projectId] as CompletedInventoryPayload;

                if (payload == null || ((payload.Serials?.Count ?? 0) == 0 && (payload.ProductQty?.Count ?? 0) == 0))
                    return Json(new { success = false, message = "No uploaded inventory found." });

                return Json(new
                {
                    success = true,
                    inventoryList = payload.Serials,
                    productQtyList = payload.ProductQty.Select(x => new { product = x.Key, qty = x.Value }).ToList()
                });
            }
            catch
            {
                return Json(new { success = false, message = "Unable to fetch completed inventory." });
            }
        }


        private List<string> ReadInventorySerialsFromFile(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0) return new List<string>();

            var ext = System.IO.Path.GetExtension(file.FileName)?.ToLower();

            if (ext == ".csv")
                return ReadFromCsv(file);

            if (ext == ".xlsx" || ext == ".xls")
                return ReadFromExcel(file);

            throw new Exception("Unsupported file type. Please upload CSV or Excel.");
        }


        private List<string> ReadInventorySerialsFromExcel(HttpPostedFileBase file)
        {
            List<string> serials = new List<string>();

            using (var package = new ExcelPackage(file.InputStream))
            {
                var ws = package.Workbook.Worksheets.First();
                int colCount = ws.Dimension.Columns;
                int rowCount = ws.Dimension.Rows;

                int serialCol = -1;

                for (int col = 1; col <= colCount; col++)
                {
                    var header = ws.Cells[1, col].Text;
                    if (header.Trim().Equals("NEW Meter Number", StringComparison.OrdinalIgnoreCase))
                    {
                        serialCol = col;
                        break;
                    }
                }

                if (serialCol == -1)
                    throw new Exception("NEW Meter Number column not found.");

                for (int row = 2; row <= rowCount; row++)
                {
                    var val = ws.Cells[row, serialCol].Text;
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        serials.Add(val);
                    }
                }
            }

            return serials
                .Select(s => s.Trim().Replace(" ", "").ToUpper())
                .Distinct()
                .ToList();
        }


        private string NormalizeHeader(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            return value.Replace("\uFEFF", "").Replace("\"", "").Trim().ToUpper();
        }

     

        private string NormalizeProduct(string v)
        {
            if (string.IsNullOrWhiteSpace(v)) return "";
            return v.Replace("\uFEFF", "").Replace("\"", "").Trim().ToUpper();
        }

        private decimal ToDecimalSafe(string v)
        {
            if (string.IsNullOrWhiteSpace(v)) return 0;
            decimal d;
            return decimal.TryParse(v.Trim(), out d) ? d : 0;
        }


        private List<string> ReadFromCsv(HttpPostedFileBase file)
        {
            List<string> serials = new List<string>();

            using (var reader = new System.IO.StreamReader(file.InputStream))
            {
                string headerLine = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(headerLine)) return serials;

                char delimiter = headerLine.Contains(";") ? ';' : ',';
                var headers = headerLine.Split(delimiter);

                int idxNewMeter = -1;
                int idxNewErt = -1;

                for (int i = 0; i < headers.Length; i++)
                {
                    var h = NormalizeHeader(headers[i]);
                    if (h == "NEW METER NUMBER") idxNewMeter = i;
                    if (h == "NEW ERT NUMBER" || h == "NEW ERT NUMBER ") idxNewErt = i; // safe
                }

                if (idxNewMeter == -1 && idxNewErt == -1)
                    throw new Exception("Required column not found. Need 'NEW METER NUMBER' or 'NEW ERT Number'.");

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var cols = line.Split(delimiter);

                    string vMeter = (idxNewMeter >= 0 && cols.Length > idxNewMeter) ? cols[idxNewMeter] : null;
                    string vErt = (idxNewErt >= 0 && cols.Length > idxNewErt) ? cols[idxNewErt] : null;

                    var picked = !string.IsNullOrWhiteSpace(vMeter) ? vMeter : vErt;

                    if (!string.IsNullOrWhiteSpace(picked))
                        serials.Add(picked);
                }
            }

            return serials
                .Select(s => NormalizeSerialValue(s))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToList();
        }

        private List<string> ReadFromExcel(HttpPostedFileBase file)
        {
            List<string> serials = new List<string>();

            using (var package = new ExcelPackage(file.InputStream))
            {
                var ws = package.Workbook.Worksheets.First();
                int colCount = ws.Dimension.Columns;
                int rowCount = ws.Dimension.Rows;

                int idxNewMeter = -1;
                int idxNewErt = -1;

                for (int c = 1; c <= colCount; c++)
                {
                    var header = NormalizeHeader(ws.Cells[1, c].Text);
                    if (header == "NEW METER NUMBER") idxNewMeter = c;
                    if (header == "NEW ERT NUMBER") idxNewErt = c;
                }

                if (idxNewMeter == -1 && idxNewErt == -1)
                    throw new Exception("Required column not found. Need 'NEW METER NUMBER' or 'NEW ERT Number'.");

                for (int r = 2; r <= rowCount; r++)
                {
                    string vMeter = (idxNewMeter > 0) ? ws.Cells[r, idxNewMeter].Text : null;
                    string vErt = (idxNewErt > 0) ? ws.Cells[r, idxNewErt].Text : null;

                    var picked = !string.IsNullOrWhiteSpace(vMeter) ? vMeter : vErt;

                    if (!string.IsNullOrWhiteSpace(picked))
                        serials.Add(picked);
                }
            }

            return serials
                .Select(s => NormalizeSerialValue(s))
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToList();
        }

        private string NormalizeSerialValue(string v)
        {
            if (string.IsNullOrWhiteSpace(v)) return "";

            return v
                .Replace("\uFEFF", "")
                .Replace("\"", "")
                .Trim()
                .Replace(" ", "")
                .ToUpper();
        }

        [HttpPost]
        public ActionResult DownloadComparisonExcel(
    long projectId,
    string greenJson,
    string yellowJson,
    string redJson,
    string consumablePurpleJson
)
        {
            var green = JsonConvert.DeserializeObject<List<string>>(greenJson ?? "[]");
            var yellow = JsonConvert.DeserializeObject<List<string>>(yellowJson ?? "[]");
            var red = JsonConvert.DeserializeObject<List<string>>(redJson ?? "[]");
            var consumable = JsonConvert.DeserializeObject<List<string>>(consumablePurpleJson ?? "[]");

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Inventory Comparison");

                // HEADER
                ws.Cells[1, 1].Value = "Item";
                ws.Cells[1, 2].Value = "Status";
                ws.Cells[1, 3].Value = "Remaining Qty";
                ws.Row(1).Style.Font.Bold = true;

                int row = 2;

                // ---------------- SERIAL MATCHED (GREEN) ----------------
                foreach (var s in green)
                {
                    ws.Cells[row, 1].Value = s;
                    ws.Cells[row, 2].Value = "MATCHED";
                    ws.Cells[row, 3].Value = "";

                    ws.Cells[row, 1, row, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, 1, row, 3].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);

                    row++;
                }

                // ---------------- SERIAL SYSTEM ONLY (YELLOW) ----------------
                foreach (var s in yellow)
                {
                    ws.Cells[row, 1].Value = s;
                    ws.Cells[row, 2].Value = "SYSTEM_ONLY";
                    ws.Cells[row, 3].Value = "";

                    ws.Cells[row, 1, row, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, 1, row, 3].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                    row++;
                }

                // ---------------- SERIAL FILE ONLY (RED) ----------------
                foreach (var s in red)
                {
                    ws.Cells[row, 1].Value = s;
                    ws.Cells[row, 2].Value = "FILE_ONLY";
                    ws.Cells[row, 3].Value = "";

                    ws.Cells[row, 1, row, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, 1, row, 3].Style.Fill.BackgroundColor.SetColor(Color.LightCoral);

                    row++;
                }

                // ---------------- CONSUMABLE LOGIC ----------------
                // CONSUMABLE
                foreach (var x in consumable)
                {
                    var parts = (x ?? "").Split('|');
                    if (parts.Length < 4) continue;

                    var product = parts[1];
                    var status = parts[2];
                    int qty = 0;
                    int.TryParse(parts[3], out qty);

                    ws.Cells[row, 1].Value = product;
                    ws.Cells[row, 2].Value = status;
                    ws.Cells[row, 3].Value = qty;

                    ws.Cells[row, 1, row, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;

                    if (status == "MATCHED")
                        ws.Cells[row, 1, row, 3].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                    else if (status == "SYSTEM_ONLY")
                        ws.Cells[row, 1, row, 3].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                    else
                        ws.Cells[row, 1, row, 3].Style.Fill.BackgroundColor.SetColor(Color.LightCoral);

                    row++;
                }

                ws.Cells.AutoFitColumns();

                return File(
                    pck.GetAsByteArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Inventory_Comparison_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx"
                );
            }
        }

        public class CompletedInventoryPayload
        {
            public List<string> Serials { get; set; } = new List<string>();

            // key = normalized product name, value = total quantity in file
            public Dictionary<string, decimal> ProductQty { get; set; } = new Dictionary<string, decimal>();
        }

        private CompletedInventoryPayload ReadCompletedInventoryFromFile(HttpPostedFileBase file)
        {
            var ext = System.IO.Path.GetExtension(file.FileName)?.ToLower();

            if (ext == ".csv")
                return ReadCompletedFromCsv(file);

            if (ext == ".xlsx" || ext == ".xls")
                return ReadCompletedFromExcel(file);

            throw new Exception("Unsupported file type. Please upload CSV or Excel.");
        }

        private CompletedInventoryPayload ReadCompletedFromCsv(HttpPostedFileBase file)
        {
            var payload = new CompletedInventoryPayload();

            using (var reader = new System.IO.StreamReader(file.InputStream))
            {
                string headerLine = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(headerLine)) return payload;

                char delimiter = headerLine.Contains(";") ? ';' : ',';
                var headers = headerLine.Split(delimiter);

                int idxNewMeter = -1, idxNewErt = -1, idxSerial = -1;
                int idxProduct = -1, idxQty = -1, idxType = -1;

                for (int i = 0; i < headers.Length; i++)
                {
                    var h = NormalizeHeader(headers[i]);

                    if (h == "NEW METER NUMBER") idxNewMeter = i;
                    if (h == "NEW ERT NUMBER") idxNewErt = i;
                    if (h == "SERIAL NUMBER") idxSerial = i;

                    if (h == "PRODUCT NAME") idxProduct = i;
                    if (h == "QUANTITY") idxQty = i;
                    if (h == "TYPE") idxType = i;
                }

                if (idxNewMeter == -1 && idxNewErt == -1 && idxSerial == -1 && (idxProduct == -1 || idxQty == -1))
                    throw new Exception("Required columns not found. Need Serial columns OR (Product Name + Quantity).");

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var cols = line.Split(delimiter);

                    string type = (idxType >= 0 && cols.Length > idxType) ? NormalizeHeader(cols[idxType]) : "";

                    // ---------- SERIAL PICK ----------
                    string vSerial = null;
                    if (idxSerial >= 0 && cols.Length > idxSerial) vSerial = cols[idxSerial];
                    if (string.IsNullOrWhiteSpace(vSerial) && idxNewMeter >= 0 && cols.Length > idxNewMeter) vSerial = cols[idxNewMeter];
                    if (string.IsNullOrWhiteSpace(vSerial) && idxNewErt >= 0 && cols.Length > idxNewErt) vSerial = cols[idxNewErt];

                    vSerial = NormalizeSerialValue(vSerial);

                    if (!string.IsNullOrWhiteSpace(vSerial))
                    {
                        payload.Serials.Add(vSerial);
                        continue;
                    }

                    // ---------- MISC / PRODUCT-QTY ----------
                    // If serial blank, then treat as product+qty (especially for Miscellaneous)
                    if (idxProduct >= 0 && idxQty >= 0 && cols.Length > idxProduct && cols.Length > idxQty)
                    {
                        var product = NormalizeProduct(cols[idxProduct]);
                        var qty = ToDecimalSafe(cols[idxQty]);

                        if (!string.IsNullOrWhiteSpace(product) && qty > 0)
                        {
                            if (!payload.ProductQty.ContainsKey(product)) payload.ProductQty[product] = 0;
                            payload.ProductQty[product] += qty;
                        }
                    }
                }
            }

            payload.Serials = payload.Serials.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            return payload;
        }

        private CompletedInventoryPayload ReadCompletedFromExcel(HttpPostedFileBase file)
        {
            var payload = new CompletedInventoryPayload();

            using (var package = new ExcelPackage(file.InputStream))
            {
                var ws = package.Workbook.Worksheets.First();
                int colCount = ws.Dimension.Columns;
                int rowCount = ws.Dimension.Rows;

                int idxNewMeter = -1, idxNewErt = -1, idxSerial = -1;
                int idxProduct = -1, idxQty = -1, idxType = -1;

                for (int c = 1; c <= colCount; c++)
                {
                    var header = NormalizeHeader(ws.Cells[1, c].Text);

                    if (header == "NEW METER NUMBER") idxNewMeter = c;
                    if (header == "NEW ERT NUMBER") idxNewErt = c;
                    if (header == "SERIAL NUMBER") idxSerial = c;

                    if (header == "PRODUCT NAME") idxProduct = c;
                    if (header == "QUANTITY") idxQty = c;
                    if (header == "TYPE") idxType = c;
                }

                if (idxNewMeter == -1 && idxNewErt == -1 && idxSerial == -1 && (idxProduct == -1 || idxQty == -1))
                    throw new Exception("Required columns not found. Need Serial columns OR (Product Name + Quantity).");

                for (int r = 2; r <= rowCount; r++)
                {
                    string type = (idxType > 0) ? NormalizeHeader(ws.Cells[r, idxType].Text) : "";

                    // ---------- SERIAL PICK ----------
                    string vSerial = null;
                    if (idxSerial > 0) vSerial = ws.Cells[r, idxSerial].Text;
                    if (string.IsNullOrWhiteSpace(vSerial) && idxNewMeter > 0) vSerial = ws.Cells[r, idxNewMeter].Text;
                    if (string.IsNullOrWhiteSpace(vSerial) && idxNewErt > 0) vSerial = ws.Cells[r, idxNewErt].Text;

                    vSerial = NormalizeSerialValue(vSerial);

                    if (!string.IsNullOrWhiteSpace(vSerial))
                    {
                        payload.Serials.Add(vSerial);
                        continue;
                    }

                    // ---------- PRODUCT-QTY ----------
                    if (idxProduct > 0 && idxQty > 0)
                    {
                        var product = NormalizeProduct(ws.Cells[r, idxProduct].Text);
                        var qty = ToDecimalSafe(ws.Cells[r, idxQty].Text);

                        if (!string.IsNullOrWhiteSpace(product) && qty > 0)
                        {
                            if (!payload.ProductQty.ContainsKey(product)) payload.ProductQty[product] = 0;
                            payload.ProductQty[product] += qty;
                        }
                    }
                }
            }

            payload.Serials = payload.Serials.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            return payload;
        }

        
    }
}