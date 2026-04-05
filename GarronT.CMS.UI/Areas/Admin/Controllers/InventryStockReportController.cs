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
using System.Text.RegularExpressions;

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
                if (projectId != 10135 && projectId != 10136)
                    return Json(new { success = false, message = "This feature is enabled only for Projects 10135 and 10136." });

                if (file == null || file.ContentLength == 0)
                    return Json(new { success = false, message = "No file uploaded." });

                CompletedInventoryPayload payload = ReadCompletedInventoryFromFile(file);

                Session["CompletedInventory_" + projectId] = payload;

                return Json(new
                {
                    success = true,
                    serialCount = payload.SerialItems.Count,
                    //productCount = payload.ProductQty.Count
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
                if (projectId != 10135 && projectId != 10136)
                    return Json(new { success = false, message = "Comparison allowed only for Projects 10135 and 10136." });

                var payload = Session["CompletedInventory_" + projectId] as CompletedInventoryPayload;

                if (payload == null)
                    return Json(new { success = false, message = "No uploaded inventory found." });

                return Json(new
                {
                    success = true,
                    inventoryList = payload.SerialItems.Select(x => new
                    {
                        productName = x.ProductName,
                        serialNumber = x.SerialNumber,
                        technicianName = x.TechnicianName,
                        installerEmail = x.InstallerEmail
                    }).ToList()
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


        private string NormalizeHeader(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            return input
                .Replace("\uFEFF", "")
                .Replace("\n", " ")
                .Replace("\r", " ")
                .Replace("\t", " ")
                .Trim()
                .ToUpper();
        }

        private string NormalizeHeaderKey(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            return new string(
                input.Replace("\uFEFF", "")
                     .Trim()
                     .ToUpperInvariant()
                     .Where(char.IsLetterOrDigit)
                     .ToArray()
            );
        }

        private bool IsMeterOrErtHeader(string headerKey)
        {
            return headerKey.Contains("METERNUMBER")
                || headerKey.Contains("NEWMETERNUMBER")
                || headerKey.Contains("ERTNUMBER")
                || headerKey.Contains("NEWERTNUMBER");
        }

        private static readonly Regex EmailRegex = new Regex(
            @"[A-Z0-9._%+\-]+@[A-Z0-9.\-]+\.[A-Z]{2,}",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        private string ExtractEmail(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            var m = EmailRegex.Match(text);
            return m.Success ? m.Value.Trim() : "";
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
                    if (h == "Meter NUMBER" || h == "METER NUMBER ") idxNewErt = i; // safe
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

                if (ws.Dimension == null)
                    return serials;

                int colCount = ws.Dimension.Columns;
                int rowCount = ws.Dimension.Rows;

                int idxSerial = -1;   // 🔥 single index enough

                // 🔥 HEADER DETECTION
                for (int c = 1; c <= colCount; c++)
                {
                    var rawHeader = ws.Cells[1, c].Text;
                    var header = NormalizeHeader(rawHeader);

                    // DEBUG (optional)
                    // throw new Exception("Header: " + header);

                    if (
                        header.Contains("METER") && header.Contains("NUMBER") ||
                        header.Contains("ERT")
                    )
                    {
                        idxSerial = c;
                        break;
                    }
                }

                if (idxSerial == -1)
                    throw new Exception("Meter/ERT Number column not found in completed inventory file.");

                // 🔥 DATA READ
                for (int r = 2; r <= rowCount; r++)
                {
                    string value = ws.Cells[r, idxSerial].Text;

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        serials.Add(value);
                    }
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

            string clean = v
                .Replace("\uFEFF", "")
                .Replace("\"", "")
                .Trim()
                .Replace(" ", "");

            // only numeric values are zero-padded; alphanumeric serials are preserved
            if (clean.All(char.IsDigit))
                clean = clean.PadLeft(8, '0');

            return clean.ToUpperInvariant();
        }

        [HttpPost]
        public ActionResult DownloadComparisonExcel(
     long projectId,
     string greenJson,
     string yellowJson,
     string redJson
 )
        {
            var green = JsonConvert.DeserializeObject<List<CompletedInventorySerialItem>>(greenJson ?? "[]");
            var yellow = JsonConvert.DeserializeObject<List<CompletedInventorySerialItem>>(yellowJson ?? "[]");
            var red = JsonConvert.DeserializeObject<List<CompletedInventorySerialItem>>(redJson ?? "[]");
            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Inventory Comparison");

                ws.Cells[1, 1].Value = "Product Name";
                ws.Cells[1, 2].Value = "Serial Number";
                ws.Cells[1, 3].Value = "Technician";
                ws.Cells[1, 4].Value = "Status";
                ws.Cells[1, 5].Value = "Remaining Qty";
                ws.Row(1).Style.Font.Bold = true;

                int row = 2;

                string Normalize(string v)
                {
                    return (v ?? "")
                        .Trim()
                        .Replace(".", "")
                        .Replace(" ", "")
                        .ToUpper();
                }

                // ================= GREEN =================
                foreach (var item in green)
                {
                    ws.Cells[row, 1].Value = item.ProductName ?? "";
                    ws.Cells[row, 2].Value = item.SerialNumber ?? "";
                    ws.Cells[row, 3].Value = item.TechnicianName ?? "";
                    ws.Cells[row, 4].Value = "MATCHED";
                    ws.Cells[row, 5].Value = 0;

                    ws.Cells[row, 1, row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, 1, row, 5].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);

                    row++;
                }

                // ================= YELLOW =================
                foreach (var item in yellow)
                {
                    ws.Cells[row, 1].Value = item.ProductName ?? "";
                    ws.Cells[row, 2].Value = item.SerialNumber ?? "";
                    ws.Cells[row, 3].Value = item.TechnicianName ?? "";
                    ws.Cells[row, 4].Value = "SYSTEM_ONLY";
                    ws.Cells[row, 5].Value = 1;

                    ws.Cells[row, 1, row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, 1, row, 5].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                    row++;
                }

                // ================= RED / FILE_ONLY =================
                var uniqueRed = red
                    .Where(x => x != null && !string.IsNullOrWhiteSpace(x.SerialNumber))
                    .GroupBy(x => Normalize(x.SerialNumber))
                    .Select(g => g.First())
                    .ToList();

                foreach (var item in uniqueRed)
                {
                    string serialNumber = Normalize(item.SerialNumber);
                    string installerEmail = (item.InstallerEmail ?? "").Trim().ToUpper();

                    ws.Cells[row, 1].Value = item.ProductName ?? "";
                    ws.Cells[row, 2].Value = serialNumber;
                    ws.Cells[row, 3].Value = item.TechnicianName ?? "";
                    ws.Cells[row, 4].Value = "Not In Inventory";
                    ws.Cells[row, 5].Value = 1;
                    ws.Cells[row, 6].Value = installerEmail;

                    ws.Cells[row, 1, row, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, 1, row, 6].Style.Fill.BackgroundColor.SetColor(Color.LightCoral);

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

        [HttpPost]
        public JsonResult SaveMissingInventory(
            long projectId,
            string redJson
        )
        {
            try
            {
                if (projectId != 10135 && projectId != 10136)
                    return Json(new { success = false, message = "Comparison allowed only for Projects 10135 and 10136." });

                var red = JsonConvert.DeserializeObject<List<CompletedInventorySerialItem>>(redJson ?? "[]");

                using (var db = new CompassEntities())
                {
                    string Normalize(string v)
                    {
                        return (v ?? "")
                            .Trim()
                            .Replace(".", "")
                            .Replace(" ", "")
                            .ToUpper();
                    }

                    // Deduplicate by normalized serial; only red/file-only rows persist into Missing_Inventory.
                    var uniqueRed = red
                        .Where(x => x != null && !string.IsNullOrWhiteSpace(x.SerialNumber))
                        .GroupBy(x => Normalize(x.SerialNumber))
                        .Select(g => g.First())
                        .ToList();

                    foreach (var item in uniqueRed)
                    {
                        string serialNumber = Normalize(item.SerialNumber);
                        string installerEmail = (item.InstallerEmail ?? "").Trim().ToUpper();

                        long? installerId = null;
                        if (!string.IsNullOrWhiteSpace(installerEmail))
                        {
                            installerId = db.tblUsers
                                .Where(x => x.Email != null && x.Email.Trim().ToUpper() == installerEmail)
                                .Select(x => (long?)x.UserID)
                                .FirstOrDefault();
                        }

                        var existing = db.Missing_Inventory.FirstOrDefault(x =>
                            x.SerialNumber != null &&
                            x.SerialNumber.Trim().ToUpper() == serialNumber
                        );

                        if (existing == null)
                        {
                            db.Database.ExecuteSqlCommand(
                                @"INSERT INTO Missing_Inventory
                                  (FKUserId, ProductName, SerialNumber, Qty, UserEmail, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy)
                                  VALUES
                                  (@p0, @p1, @p2, @p3, @p4, @p5, @p6, NULL, NULL)",
                                installerId.HasValue ? (object)installerId.Value : DBNull.Value,
                                (object)(item.ProductName ?? ""),
                                (object)serialNumber,
                                1,
                                string.IsNullOrWhiteSpace(installerEmail) ? (object)DBNull.Value : installerEmail,
                                DateTime.Now,
                                installerId.HasValue ? (object)installerId.Value : DBNull.Value
                            );
                        }
                        else
                        {
                            db.Database.ExecuteSqlCommand(
                                @"UPDATE Missing_Inventory
                                  SET FKUserId = @p0,
                                      UserEmail = @p1,
                                      ModifiedOn = @p2,
                                      ModifiedBy = @p3
                                  WHERE Id = @p4",
                                installerId.HasValue ? (object)installerId.Value : DBNull.Value,
                                string.IsNullOrWhiteSpace(installerEmail) ? (object)DBNull.Value : installerEmail,
                                DateTime.Now,
                                installerId.HasValue ? (object)installerId.Value : DBNull.Value,
                                existing.Id
                            );
                        }
                    }

                    db.SaveChanges();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public class CompletedInventorySerialItem
        {
           
            
                public string ProductName { get; set; }
                public string SerialNumber { get; set; }
                public string TechnicianName { get; set; }
                public string InstallerEmail { get; set; }   // ✅ ADD THIS
            
        }

        public class CompletedInventoryPayload
        {
            public List<CompletedInventorySerialItem> SerialItems { get; set; } = new List<CompletedInventorySerialItem>();

            //public Dictionary<string, decimal> ProductQty { get; set; } = new Dictionary<string, decimal>();
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
                if (string.IsNullOrWhiteSpace(headerLine))
                    return payload;

                char delimiter = headerLine.Contains(";") ? ';' : ',';
                var headers = headerLine.Split(delimiter);

                int idxSerial = -1;
                int idxEmail = -1;
                string fileLevelInstallerEmail = ExtractEmail(headerLine);

                for (int i = 0; i < headers.Length; i++)
                {
                    var key = NormalizeHeaderKey(headers[i]);

                    if (IsMeterOrErtHeader(key))
                        idxSerial = i;

                    if (key.Contains("INSTALLER") || key.Contains("EMAIL"))
                        idxEmail = i;
                }

                if (idxSerial == -1)
                    throw new Exception("Meter Number column not found in completed inventory file.");

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var cols = line.Split(delimiter);

                    string serial = (cols.Length > idxSerial) ? NormalizeSerialValue(cols[idxSerial]) : "";
                    string email = (idxEmail >= 0 && cols.Length > idxEmail) ? ExtractEmail(cols[idxEmail]) : "";

                    // fallback: if file carries one installer email in metadata/cell text
                    if (string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(fileLevelInstallerEmail))
                        email = fileLevelInstallerEmail;

                    if (string.IsNullOrWhiteSpace(email))
                    {
                        // pick first email seen in row text (works for mixed cells like "Installer: x@y.com")
                        email = ExtractEmail(line);
                        if (!string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(fileLevelInstallerEmail))
                            fileLevelInstallerEmail = email;
                    }

                    if (string.IsNullOrWhiteSpace(serial))
                        continue;

                    payload.SerialItems.Add(new CompletedInventorySerialItem
                    {
                        ProductName = "",
                        SerialNumber = serial,
                        TechnicianName = "",
                        InstallerEmail = email
                    });
                }
            }

            payload.SerialItems = payload.SerialItems
                .Where(x => !string.IsNullOrWhiteSpace(x.SerialNumber))
                .GroupBy(x => x.SerialNumber.Trim().ToUpper())
                .Select(g => g.First())
                .ToList();

            return payload;
        }
        private CompletedInventoryPayload ReadCompletedFromExcel(HttpPostedFileBase file)
        {
            var payload = new CompletedInventoryPayload();

            using (var package = new ExcelPackage(file.InputStream))
            {
                ExcelWorksheet targetSheet = null;
                int headerRow = -1;
                int idxSerial = -1;
                int idxEmail = -1;
                string fileLevelInstallerEmail = "";
                bool foundBest = false;

                // scan all sheets and first 25 rows to find meter header
                foreach (var ws in package.Workbook.Worksheets)
                {
                    if (ws.Dimension == null) continue;

                    int rowCount = ws.Dimension.Rows;
                    int colCount = ws.Dimension.Columns;

                    for (int r = 1; r <= Math.Min(25, rowCount); r++)
                    {
                        int serialColTemp = -1;
                        int emailColTemp = -1;

                        for (int c = 1; c <= colCount; c++)
                        {
                            string key = NormalizeHeaderKey(ws.Cells[r, c].Text);

                            if (IsMeterOrErtHeader(key))
                                serialColTemp = c;

                            if (key.Contains("INSTALLER") || key.Contains("EMAIL"))
                                emailColTemp = c;

                            if (string.IsNullOrWhiteSpace(fileLevelInstallerEmail))
                            {
                                var detectedEmail = ExtractEmail(ws.Cells[r, c].Text);
                                if (!string.IsNullOrWhiteSpace(detectedEmail))
                                    fileLevelInstallerEmail = detectedEmail;
                            }
                        }

                        if (serialColTemp != -1)
                        {
                            // Prefer header that has both serial + installer/email column
                            if (emailColTemp != -1)
                            {
                                targetSheet = ws;
                                headerRow = r;
                                idxSerial = serialColTemp;
                                idxEmail = emailColTemp;
                                foundBest = true;
                                break;
                            }

                            // fallback candidate (serial-only header)
                            if (targetSheet == null)
                            {
                                targetSheet = ws;
                                headerRow = r;
                                idxSerial = serialColTemp;
                                idxEmail = -1;
                            }
                        }
                    }

                    if (foundBest) break;
                }

                if (targetSheet == null || idxSerial == -1)
                    throw new Exception("Meter Number column not found in completed inventory file.");

                int totalRows = targetSheet.Dimension.Rows;
                for (int r = headerRow + 1; r <= totalRows; r++)
                {
                    string serial = NormalizeSerialValue(targetSheet.Cells[r, idxSerial].Text);
                    string email = "";

                    if (idxEmail > 0)
                        email = ExtractEmail(targetSheet.Cells[r, idxEmail].Text);

                    if (string.IsNullOrWhiteSpace(email))
                    {
                        // fallback: detect email from row text
                        for (int c = 1; c <= targetSheet.Dimension.Columns; c++)
                        {
                            email = ExtractEmail(targetSheet.Cells[r, c].Text);
                            if (!string.IsNullOrWhiteSpace(email)) break;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(email))
                        email = fileLevelInstallerEmail;

                    if (string.IsNullOrWhiteSpace(serial))
                        continue;

                    payload.SerialItems.Add(new CompletedInventorySerialItem
                    {
                        ProductName = "",
                        SerialNumber = serial,
                        TechnicianName = "",
                        InstallerEmail = email
                    });
                }
            }

            payload.SerialItems = payload.SerialItems
                .Where(x => !string.IsNullOrWhiteSpace(x.SerialNumber))
                .GroupBy(x => x.SerialNumber.Trim().ToUpperInvariant())
                .Select(g => g.First())
                .ToList();

            return payload;
        }
        public JsonResult InstallMatchedInventory(int projectId, string serialList)
        {
            try
            {
                long userId = 0;

                if (Session["UserId"] != null)
                    long.TryParse(Session["UserId"].ToString(), out userId);

                var serials = JsonConvert.DeserializeObject<List<CompletedInventorySerialItem>>(serialList ?? "[]");

                using (var db = new CompassEntities())
                {
                    var master = db.tblProjectFieldDataMasters
                        .Where(x => x.Active == 1)
                        .OrderByDescending(x => x.ID)
                        .FirstOrDefault();

                    if (master == null)
                        return Json(new { success = false, message = "Master record not found." });

                    long masterId = master.ID;

                    InventoryDAL dal = new InventoryDAL();

                    var userMapByEmail = db.tblUsers
                        .Where(x => x.Active == 1 && x.Email != null)
                        .AsEnumerable()
                        .GroupBy(x => x.Email.Trim().ToUpper())
                        .ToDictionary(
                            g => g.Key,
                            g => g.First().UserID
                        );

                    foreach (var item in serials.Where(x => x != null && !string.IsNullOrWhiteSpace(x.SerialNumber)))
                    {
                        string serial = item.SerialNumber.Trim();
                        long installerUserId = userId;

                        string installerEmail = (item.InstallerEmail ?? "").Trim().ToUpper();

                        if (!string.IsNullOrEmpty(installerEmail) && userMapByEmail.ContainsKey(installerEmail))
                        {
                            installerUserId = userMapByEmail[installerEmail];
                        }

                        dal.MoveInventoryToInstalled(
                            projectId,
                            installerUserId,
                            serial,
                            masterId
                        );
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string error = ex.Message;

                if (ex.InnerException != null)
                    error += " | " + ex.InnerException.Message;

                if (ex.InnerException?.InnerException != null)
                    error += " | " + ex.InnerException.InnerException.Message;

                return Json(new { success = false, message = error });
            }
        }

    }
}