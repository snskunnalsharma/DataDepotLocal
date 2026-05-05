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
using System.Configuration;
using System.IO;
using System.Net.Mail;
using log4net;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class InventryStockReportController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(InventryStockReportController));

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


        public void DownloadExcelExport(long projectId, long installerId, long ProductId, string txtFromDate, string txtToDate, long ReportType, bool sendEmail = false)
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

                    byte[] excelBytes = pck.GetAsByteArray();

                    // Email on export disabled for now — re-enable when needed.
                    //if (sendEmail)
                    //{
                    //    TryEmailInventryStockReport(excelBytes);
                    //}

                    Response.BinaryWrite(excelBytes);
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

        // Email alert disabled for now — restore method body when re-enabling sendEmail on export.
        //private void TryEmailInventryStockReport(byte[] excelBytes)
        //{
        //    try
        //    {
        //        if (excelBytes == null || excelBytes.Length == 0) return;
        //        string toEmail = "kunal.sharma@snssystem.com";
        //        string fromEmail = (ConfigurationManager.AppSettings["WebMasterMailId"] ?? "").Trim();
        //        string password = (ConfigurationManager.AppSettings["WebMasterMailPassword"] ?? "").Trim();
        //        string host = (ConfigurationManager.AppSettings["WebMasterSMTPServer"] ?? "").Trim();
        //        string portRaw = (ConfigurationManager.AppSettings["WebMasterPort"] ?? "").Trim();
        //        string sslRaw = (ConfigurationManager.AppSettings["WebMasterEnableSSL"] ?? "").Trim();
        //        int port;
        //        if (!int.TryParse(portRaw, out port)) port = 587;
        //        bool enableSsl;
        //        if (!bool.TryParse(sslRaw, out enableSsl)) enableSsl = true;
        //        using (var mail = new MailMessage())
        //        {
        //            mail.From = new MailAddress(fromEmail);
        //            mail.To.Add(new MailAddress(toEmail));
        //            mail.Subject = "Inventory Stock Report";
        //            mail.Body = "Please find the Inventory Stock Report attached.";
        //            mail.IsBodyHtml = false;
        //            using (var ms = new MemoryStream(excelBytes))
        //            using (var attachment = new Attachment(
        //                ms,
        //                "InventryStockReport.xlsx",
        //                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        //            ))
        //            {
        //                mail.Attachments.Add(attachment);
        //                using (var smtp = new SmtpClient(host, port))
        //                {
        //                    smtp.EnableSsl = enableSsl;
        //                    smtp.UseDefaultCredentials = false;
        //                    smtp.Credentials = new System.Net.NetworkCredential(fromEmail, password);
        //                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        //                    smtp.Send(mail);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("InventryStockReport email failed.", ex);
        //    }
        //}

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

        private DateTime? ToDateSafe(string v)
        {
            if (string.IsNullOrWhiteSpace(v)) return null;
            DateTime dt;
            return DateTime.TryParse(v.Trim(), out dt) ? (DateTime?)dt : null;
        }

        private string ExtractHeaderValue(string text, params string[] keys)
        {
            // Keep for backward compatibility, but we no longer override WorkOrderId/Status from header.
            // (Fields now come from Excel/CSV columns.)
            if (string.IsNullOrWhiteSpace(text) || keys == null || keys.Length == 0) return "";
            var upper = text.ToUpperInvariant();
            foreach (var k in keys)
            {
                if (string.IsNullOrWhiteSpace(k)) continue;
                var key = k.Trim().ToUpperInvariant();
                var idx = upper.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                if (idx < 0) continue;

                var after = text.Substring(idx + key.Length);
                after = after.TrimStart(' ', ':', '-', '\t');
                if (!string.IsNullOrWhiteSpace(after))
                    return after.Trim();
            }
            return "";
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
     string redJson,
     bool sendEmail = false
 )
        {
            var green = JsonConvert.DeserializeObject<List<CompletedInventorySerialItem>>(greenJson ?? "[]");
            var yellow = JsonConvert.DeserializeObject<List<CompletedInventorySerialItem>>(yellowJson ?? "[]");
            var red = JsonConvert.DeserializeObject<List<CompletedInventorySerialItem>>(redJson ?? "[]");

            byte[] excelBytes = BuildComparisonExcelBytes(green, yellow, red);

            // Email on comparison export disabled for now.
            //if (sendEmail)
            //{
            //    TrySendInventoryComparisonEmail(excelBytes);
            //}

            return File(
                excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Inventory_Comparison_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx"
            );
        }

        //private void TrySendInventoryComparisonEmail(byte[] excelBytes)
        //{
        //    try
        //    {
        //        SendInventoryComparisonEmail(excelBytes);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("InventoryComparison email failed.", ex);
        //    }
        //}

        private byte[] BuildComparisonExcelBytes(
            List<CompletedInventorySerialItem> green,
            List<CompletedInventorySerialItem> yellow,
            List<CompletedInventorySerialItem> red)
        {
            green = green ?? new List<CompletedInventorySerialItem>();
            yellow = yellow ?? new List<CompletedInventorySerialItem>();
            red = red ?? new List<CompletedInventorySerialItem>();

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Inventory Comparison");

                ws.Cells[1, 1].Value = "Product Name";
                ws.Cells[1, 2].Value = "Serial Number";
                ws.Cells[1, 3].Value = "Remaining Qty";
                ws.Cells[1, 4].Value = "Technician";
                ws.Cells[1, 5].Value = "Status";
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

                foreach (var item in green)
                {
                    ws.Cells[row, 1].Value = item.ProductName ?? "";
                    ws.Cells[row, 2].Value = item.SerialNumber ?? "";
                    ws.Cells[row, 3].Value = 0;
                    ws.Cells[row, 4].Value = item.TechnicianName ?? "";
                    ws.Cells[row, 5].Value = "MATCHED (Green)";

                    ws.Cells[row, 1, row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, 1, row, 5].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);

                    row++;
                }

                // Unused: same as UI .notmatch-yellow (#fff3cd)
                Color unusedFill = Color.FromArgb(255, 243, 205);
                // Installed Not In Inventory: blue (matches compare-result popup)
                Color installedNotInInvFill = Color.FromArgb(207, 226, 255);
                Color installedNotInInvText = Color.FromArgb(8, 66, 152);

                foreach (var item in yellow)
                {
                    ws.Cells[row, 1].Value = item.ProductName ?? "";
                    ws.Cells[row, 2].Value = item.SerialNumber ?? "";
                    ws.Cells[row, 3].Value = 1;
                    ws.Cells[row, 4].Value = item.TechnicianName ?? "";
                    ws.Cells[row, 5].Value = "Unused (Yellow)";

                    ws.Cells[row, 1, row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, 1, row, 5].Style.Fill.BackgroundColor.SetColor(unusedFill);

                    row++;
                }

                var uniqueRed = red
                    .Where(x => x != null && !string.IsNullOrWhiteSpace(x.SerialNumber))
                    .GroupBy(x => Normalize(x.SerialNumber))
                    .Select(g => g.First())
                    .ToList();

                foreach (var item in uniqueRed)
                {
                    string serialNumber = Normalize(item.SerialNumber);
                    string installerEmail = (item.InstallerEmail ?? "").Trim().ToUpper();
                    string technicianOrEmail = !string.IsNullOrWhiteSpace(item.TechnicianName)
                        ? item.TechnicianName
                        : installerEmail;

                    ws.Cells[row, 1].Value = item.ProductName ?? "";
                    ws.Cells[row, 2].Value = serialNumber;
                    ws.Cells[row, 3].Value = 1;
                    ws.Cells[row, 4].Value = technicianOrEmail;
                    ws.Cells[row, 5].Value = "Installed Not In Inventory (Blue)";

                    ws.Cells[row, 1, row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, 1, row, 5].Style.Fill.BackgroundColor.SetColor(installedNotInInvFill);
                    ws.Cells[row, 1, row, 5].Style.Font.Color.SetColor(installedNotInInvText);

                    row++;
                }

                ws.Cells.AutoFitColumns();

                return pck.GetAsByteArray();
            }
        }

        //private void SendInventoryComparisonEmail(byte[] excelBytes)
        //{
        //    if (excelBytes == null || excelBytes.Length == 0)
        //        throw new InvalidOperationException("No Excel data to send.");
        //    string toEmail = "kunal.sharma@snssystem.com";
        //    string fromEmail = (ConfigurationManager.AppSettings["WebMasterMailId"] ?? "").Trim();
        //    string password = (ConfigurationManager.AppSettings["WebMasterMailPassword"] ?? "").Trim();
        //    string host = (ConfigurationManager.AppSettings["WebMasterSMTPServer"] ?? "").Trim();
        //    string portRaw = (ConfigurationManager.AppSettings["WebMasterPort"] ?? "").Trim();
        //    string sslRaw = (ConfigurationManager.AppSettings["WebMasterEnableSSL"] ?? "").Trim();
        //    if (string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(password))
        //        throw new InvalidOperationException("Email is not configured (WebMasterMailId / WebMasterMailPassword).");
        //    int port;
        //    if (!int.TryParse(portRaw, out port)) port = 587;
        //    bool enableSsl;
        //    if (!bool.TryParse(sslRaw, out enableSsl)) enableSsl = true;
        //    string fileName = "Inventory_Comparison_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
        //    using (var mail = new MailMessage())
        //    {
        //        mail.From = new MailAddress(fromEmail);
        //        mail.To.Add(new MailAddress(toEmail));
        //        mail.CC.Add("snssystem.akash@gmail.com");
        //        mail.CC.Add("snssystem.tarun@gmail.com");
        //        mail.Bcc.Add("yogendra.rajpoot@snssystem.com");
        //        mail.Subject = "Inventory comparison report";
        //        mail.Body = "Please find the inventory comparison Excel attached (same as Download Inventory Report).";
        //        mail.IsBodyHtml = false;
        //        using (var ms = new MemoryStream(excelBytes))
        //        using (var attachment = new Attachment(
        //            ms,
        //            fileName,
        //            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
        //        {
        //            mail.Attachments.Add(attachment);
        //            using (var smtp = new SmtpClient(host, port))
        //            {
        //                smtp.EnableSsl = enableSsl;
        //                smtp.UseDefaultCredentials = false;
        //                smtp.Credentials = new System.Net.NetworkCredential(fromEmail, password);
        //                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        //                smtp.Send(mail);
        //            }
        //        }
        //    }
        //}

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
                // INVInstallerReport fields (read from uploaded compare file)
                public DateTime? CompletionDate { get; set; }
                public string InstallerName { get; set; }
                public string WorkOrderId { get; set; }
                public string ServiceAddress { get; set; }
                public decimal? Latitude { get; set; }
                public decimal? Longitude { get; set; }
                public string Status { get; set; }
                public string ProjectName { get; set; }
            
        }

        public class CompletedInventoryPayload
        {
            public List<CompletedInventorySerialItem> SerialItems { get; set; } = new List<CompletedInventorySerialItem>();

            //public Dictionary<string, decimal> ProductQty { get; set; } = new Dictionary<string, decimal>();
            // File-level header values kept but not used for report save anymore
            public string HeaderWorkOrderId { get; set; }
            public string HeaderStatus { get; set; }
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
                int idxCompletionDate = -1;
                int idxInstallerName = -1;
                int idxWorkOrderId = -1;
                int idxServiceAddress = -1;
                int idxLatitude = -1;
                int idxLongitude = -1;
                int idxProvidedLatitude = -1;
                int idxProvidedLongitude = -1;
                int idxStatus = -1;
                int idxProjectName = -1;
                string fileLevelInstallerEmail = ExtractEmail(headerLine);

                for (int i = 0; i < headers.Length; i++)
                {
                    var key = NormalizeHeaderKey(headers[i]);

                    if (IsMeterOrErtHeader(key))
                        idxSerial = i;

                    if (key.Contains("INSTALLER") || key.Contains("EMAIL"))
                        idxEmail = i;

                    if (key.Contains("COMPLETIONDATE") || (key.Contains("COMPLETION") && key.Contains("DATE")))
                        idxCompletionDate = i;
                    if (key.Contains("INSTALLERNAME") || (key.Contains("INSTALLER") && key.Contains("NAME")))
                        idxInstallerName = i;
                    // Work Order Id: prefer explicit "WORK ORDER ID" style headers
                    if (key == "WORKORDERID" || key == "WORKORDER" || key.Contains("WORKORDERID"))
                        idxWorkOrderId = i;
                    if (key.Contains("SERVICEADDRESS") || (key.Contains("SERVICE") && key.Contains("ADDRESS")) || key.EndsWith("ADDRESS"))
                        idxServiceAddress = i;
                    if (key.Contains("PROVIDED") && key.Contains("LATITUDE"))
                        idxProvidedLatitude = i;
                    if (key.Contains("PROVIDED") && key.Contains("LONGITUDE"))
                        idxProvidedLongitude = i;
                    if (idxProvidedLatitude < 0 && key.Contains("LATITUDE"))
                        idxLatitude = i;
                    if (idxProvidedLongitude < 0 && key.Contains("LONGITUDE"))
                        idxLongitude = i;
                    // Status: prefer explicit STATUS column
                    if (key == "STATUS")
                        idxStatus = i;
                    if (key.Contains("PROJECTNAME") || (key.Contains("PROJECT") && key.Contains("NAME")))
                        idxProjectName = i;
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
                        InstallerEmail = email,
                        CompletionDate = (idxCompletionDate >= 0 && cols.Length > idxCompletionDate) ? ToDateSafe(cols[idxCompletionDate]) : null,
                        InstallerName = (idxInstallerName >= 0 && cols.Length > idxInstallerName) ? (cols[idxInstallerName] ?? "").Trim() : "",
                        WorkOrderId = (idxWorkOrderId >= 0 && cols.Length > idxWorkOrderId) ? (cols[idxWorkOrderId] ?? "").Trim() : "",
                        ServiceAddress = (idxServiceAddress >= 0 && cols.Length > idxServiceAddress) ? (cols[idxServiceAddress] ?? "").Trim() : "",
                        Latitude = (idxProvidedLatitude >= 0 && cols.Length > idxProvidedLatitude)
                            ? (decimal?)ToDecimalSafe(cols[idxProvidedLatitude])
                            : ((idxLatitude >= 0 && cols.Length > idxLatitude) ? (decimal?)ToDecimalSafe(cols[idxLatitude]) : null),
                        Longitude = (idxProvidedLongitude >= 0 && cols.Length > idxProvidedLongitude)
                            ? (decimal?)ToDecimalSafe(cols[idxProvidedLongitude])
                            : ((idxLongitude >= 0 && cols.Length > idxLongitude) ? (decimal?)ToDecimalSafe(cols[idxLongitude]) : null),
                        Status = (idxStatus >= 0 && cols.Length > idxStatus) ? (cols[idxStatus] ?? "").Trim() : "",
                        ProjectName = (idxProjectName >= 0 && cols.Length > idxProjectName) ? (cols[idxProjectName] ?? "").Trim() : ""
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
                int idxCompletionDate = -1;
                int idxInstallerName = -1;
                int idxWorkOrderId = -1;
                int idxServiceAddress = -1;
                int idxLatitude = -1;
                int idxLongitude = -1;
                int idxProvidedLatitude = -1;
                int idxProvidedLongitude = -1;
                int idxStatus = -1;
                int idxProjectName = -1;
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
                        int completionDateColTemp = -1;
                        int installerNameColTemp = -1;
                        int workOrderIdColTemp = -1;
                        int serviceAddressColTemp = -1;
                        int latitudeColTemp = -1;
                        int longitudeColTemp = -1;
                        int providedLatitudeColTemp = -1;
                        int providedLongitudeColTemp = -1;
                        int statusColTemp = -1;
                        int projectNameColTemp = -1;

                        for (int c = 1; c <= colCount; c++)
                        {
                            string key = NormalizeHeaderKey(ws.Cells[r, c].Text);
                            string cellText = ws.Cells[r, c].Text ?? "";

                            // Do not read WorkOrderId/Status from header text; these come from columns now.

                            if (IsMeterOrErtHeader(key))
                                serialColTemp = c;

                            if (key.Contains("INSTALLER") || key.Contains("EMAIL"))
                                emailColTemp = c;

                            if (key.Contains("COMPLETIONDATE") || (key.Contains("COMPLETION") && key.Contains("DATE")))
                                completionDateColTemp = c;
                            if (key.Contains("INSTALLERNAME") || (key.Contains("INSTALLER") && key.Contains("NAME")))
                                installerNameColTemp = c;
                            // Work Order Id: prefer explicit "WORK ORDER ID" style headers
                            if (key == "WORKORDERID" || key == "WORKORDER" || key.Contains("WORKORDERID"))
                                workOrderIdColTemp = c;
                            if (key.Contains("SERVICEADDRESS") || (key.Contains("SERVICE") && key.Contains("ADDRESS")) || key.EndsWith("ADDRESS"))
                                serviceAddressColTemp = c;
                            if (key.Contains("PROVIDED") && key.Contains("LATITUDE"))
                                providedLatitudeColTemp = c;
                            if (key.Contains("PROVIDED") && key.Contains("LONGITUDE"))
                                providedLongitudeColTemp = c;
                            if (providedLatitudeColTemp <= 0 && key.Contains("LATITUDE"))
                                latitudeColTemp = c;
                            if (providedLongitudeColTemp <= 0 && key.Contains("LONGITUDE"))
                                longitudeColTemp = c;
                            // Status: prefer explicit STATUS column
                            if (key == "STATUS")
                                statusColTemp = c;
                            if (key.Contains("PROJECTNAME") || (key.Contains("PROJECT") && key.Contains("NAME")))
                                projectNameColTemp = c;

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
                                idxCompletionDate = completionDateColTemp;
                                idxInstallerName = installerNameColTemp;
                                idxWorkOrderId = workOrderIdColTemp;
                                idxServiceAddress = serviceAddressColTemp;
                                idxLatitude = latitudeColTemp;
                                idxLongitude = longitudeColTemp;
                                idxProvidedLatitude = providedLatitudeColTemp;
                                idxProvidedLongitude = providedLongitudeColTemp;
                                idxStatus = statusColTemp;
                                idxProjectName = projectNameColTemp;
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
                                idxCompletionDate = completionDateColTemp;
                                idxInstallerName = installerNameColTemp;
                                idxWorkOrderId = workOrderIdColTemp;
                                idxServiceAddress = serviceAddressColTemp;
                                idxLatitude = latitudeColTemp;
                                idxLongitude = longitudeColTemp;
                                idxProvidedLatitude = providedLatitudeColTemp;
                                idxProvidedLongitude = providedLongitudeColTemp;
                                idxStatus = statusColTemp;
                                idxProjectName = projectNameColTemp;
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
                        InstallerEmail = email,
                        CompletionDate = (idxCompletionDate > 0) ? ToDateSafe(targetSheet.Cells[r, idxCompletionDate].Text) : null,
                        InstallerName = (idxInstallerName > 0) ? (targetSheet.Cells[r, idxInstallerName].Text ?? "").Trim() : "",
                        WorkOrderId = (idxWorkOrderId > 0) ? (targetSheet.Cells[r, idxWorkOrderId].Text ?? "").Trim() : "",
                        ServiceAddress = (idxServiceAddress > 0) ? (targetSheet.Cells[r, idxServiceAddress].Text ?? "").Trim() : "",
                        Latitude = (idxProvidedLatitude > 0)
                            ? (decimal?)ToDecimalSafe(targetSheet.Cells[r, idxProvidedLatitude].Text)
                            : ((idxLatitude > 0) ? (decimal?)ToDecimalSafe(targetSheet.Cells[r, idxLatitude].Text) : null),
                        Longitude = (idxProvidedLongitude > 0)
                            ? (decimal?)ToDecimalSafe(targetSheet.Cells[r, idxProvidedLongitude].Text)
                            : ((idxLongitude > 0) ? (decimal?)ToDecimalSafe(targetSheet.Cells[r, idxLongitude].Text) : null),
                        Status = (idxStatus > 0) ? (targetSheet.Cells[r, idxStatus].Text ?? "").Trim() : "",
                        ProjectName = (idxProjectName > 0) ? (targetSheet.Cells[r, idxProjectName].Text ?? "").Trim() : ""
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
        [HttpPost]
        public JsonResult InstallMatchedInventory(int projectId, string serialList)
        {
            try
            {
                long userId = 0;

                if (Session["UserId"] != null)
                    long.TryParse(Session["UserId"].ToString(), out userId);

                var serials = JsonConvert.DeserializeObject<List<CompletedInventorySerialItem>>(serialList ?? "[]") ?? new List<CompletedInventorySerialItem>();

                // Prefer taking full row details from the uploaded file already stored in Session (more reliable than JS payload).
                var sessionPayload = Session["CompletedInventory_" + projectId] as CompletedInventoryPayload;
                // WorkOrderId/Status come from file columns (not header)

                string NormalizeSerialKey(string v)
                {
                    return (v ?? "").Trim().Replace(".", "").Replace(" ", "").ToUpperInvariant();
                }

                var fileBySerial = (sessionPayload?.SerialItems ?? new List<CompletedInventorySerialItem>())
                    .Where(x => x != null && !string.IsNullOrWhiteSpace(x.SerialNumber))
                    .GroupBy(x => NormalizeSerialKey(x.SerialNumber))
                    .Select(g => g.First())
                    .ToDictionary(g => NormalizeSerialKey(g.SerialNumber), g => g);

                using (var db = new CompassEntities())
                {
                    // Always take ProjectName from selected projectId
                    string selectedProjectName = db.tblProjects
                        .Where(p => p.ProjectId == projectId)
                        .Select(p => p.ProjectName)
                        .FirstOrDefault() ?? "";
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

                    string Normalize(string v) => NormalizeSerialKey(v);

                    var unique = serials
                        .Where(x => x != null && !string.IsNullOrWhiteSpace(x.SerialNumber))
                        .GroupBy(x => Normalize(x.SerialNumber))
                        .Select(g => g.First())
                        .ToList();

                    int installed = 0, skipped = 0, errors = 0;
                    int invReportSaved = 0;
                    var errorList = new List<string>();

                    foreach (var item in unique)
                    {
                        string serial = item.SerialNumber?.Trim() ?? "";
                        if (string.IsNullOrWhiteSpace(serial))
                        {
                            skipped++;
                            continue;
                        }

                        // Enrich item fields from uploaded file payload (if present).
                        var key = NormalizeSerialKey(serial);
                        if (fileBySerial.ContainsKey(key))
                        {
                            var fromFile = fileBySerial[key];
                            // Keep InstallerName from UI datatable (sent as TechnicianName/InstallerName) if present;
                            // otherwise take from file.
                            item.CompletionDate = item.CompletionDate ?? fromFile.CompletionDate;
                            item.WorkOrderId = string.IsNullOrWhiteSpace(item.WorkOrderId) ? fromFile.WorkOrderId : item.WorkOrderId;
                            item.ServiceAddress = string.IsNullOrWhiteSpace(item.ServiceAddress) ? fromFile.ServiceAddress : item.ServiceAddress;
                            item.Latitude = item.Latitude ?? fromFile.Latitude;
                            item.Longitude = item.Longitude ?? fromFile.Longitude;
                            item.Status = string.IsNullOrWhiteSpace(item.Status) ? fromFile.Status : item.Status;
                            item.ProjectName = selectedProjectName;
                            if (string.IsNullOrWhiteSpace(item.InstallerEmail))
                                item.InstallerEmail = fromFile.InstallerEmail;
                            if (string.IsNullOrWhiteSpace(item.InstallerName))
                                item.InstallerName = fromFile.InstallerName;
                        }
                        else
                        {
                            // Ensure project name even when file row missing
                            item.ProjectName = selectedProjectName;
                        }

                        // WorkOrderId/Status are taken from file columns; no header override.

                        long installerUserId = userId;

                        string installerEmail = (item.InstallerEmail ?? "").Trim().ToUpperInvariant();
                        if (!string.IsNullOrEmpty(installerEmail) && userMapByEmail.ContainsKey(installerEmail))
                        {
                            installerUserId = userMapByEmail[installerEmail];
                        }

                        try
                        {
                            dal.MoveInventoryToInstalled(projectId, installerUserId, serial, masterId);
                            installed++;

                            // Save matched serial details into INVInstallerReport (from uploaded file)
                            try
                            {
                                DateTime now = DateTime.Now;
                                string serialNorm = (serial ?? "").Trim();
                                string installerNameFinal = (!string.IsNullOrWhiteSpace(item.TechnicianName) ? item.TechnicianName : item.InstallerName) ?? "";

                                // Upsert using raw SQL (edmx may not have operation_date even if DB has it)
                                string projectIdStr = projectId.ToString();
                                int affected = db.Database.ExecuteSqlCommand(@"
                                                                                    IF EXISTS (
                                                                                        SELECT 1 
                                                                                        FROM dbo.INVInstallerReport 
                                                                                        WHERE project_id = @p0 
                                                                                          AND serial_number = @p1
                                                                                    )
                                                                                    BEGIN
                                                                                        UPDATE dbo.INVInstallerReport
                                                                                        SET completion_date = @p2,
                                                                                            installer_name = @p3,
                                                                                            work_order_id = @p4,
                                                                                            service_address = @p5,
                                                                                            latitude = @p6,
                                                                                            longitude = @p7,
                                                                                            status = @p8,
                                                                                            project_name = @p9,
                                                                                            updated_at = @p10,
                                                                                            operation_date = @p11
                                                                                        WHERE project_id = @p0 
                                                                                          AND serial_number = @p1;
                                                                                    END
                                                                                    ELSE
                                                                                    BEGIN
                                                                                        INSERT INTO dbo.INVInstallerReport
                                                                                        (
                                                                                            project_id,
                                                                                            completion_date,
                                                                                            installer_name,
                                                                                            work_order_id,
                                                                                            serial_number,
                                                                                            service_address,
                                                                                            latitude,
                                                                                            longitude,
                                                                                            status,
                                                                                            project_name,
                                                                                            created_at,
                                                                                            updated_at,
                                                                                            operation_date
                                                                                        )
                                                                                        VALUES
                                                                                        (
                                                                                            @p0,
                                                                                            @p2,
                                                                                            @p3,
                                                                                            @p4,
                                                                                            @p1,
                                                                                            @p5,
                                                                                            @p6,
                                                                                            @p7,
                                                                                            @p8,
                                                                                            @p9,
                                                                                            @p10,
                                                                                            @p10,
                                                                                            @p11
                                                                                        );
                                                                                    END
                                                                                    ",
                                    projectIdStr,
                                    serialNorm,
                                    (object)(item.CompletionDate.HasValue ? (object)item.CompletionDate.Value : DBNull.Value),
                                    (object)installerNameFinal,
                                    (object)(item.WorkOrderId ?? ""),
                                    (object)(item.ServiceAddress ?? ""),
                                    (object)(item.Latitude.HasValue ? (object)item.Latitude.Value : DBNull.Value),
                                    (object)(item.Longitude.HasValue ? (object)item.Longitude.Value : DBNull.Value),
                                    (object)(item.Status ?? ""),
                                    (object)(item.ProjectName ?? ""),
                                    now,
                                    now
                                );
                                if (affected > 0) invReportSaved++;
                            }
                            catch (Exception opDateEx)
                            {
                                // Fallback: DB without operation_date column
                                DateTime now2 = DateTime.Now;
                                string serialNorm2 = (serial ?? "").Trim();
                                string installerNameFinal2 = (!string.IsNullOrWhiteSpace(item.TechnicianName) ? item.TechnicianName : item.InstallerName) ?? "";
                                try
                                {
                                    string projectIdStr2 = projectId.ToString();
                                    int affected2 = db.Database.ExecuteSqlCommand(@"
IF EXISTS (SELECT 1 FROM dbo.INVInstallerReport WHERE project_id = @p0 AND serial_number = @p1)
BEGIN
    UPDATE dbo.INVInstallerReport
    SET completion_date = @p2,
        installer_name = @p3,
        work_order_id = @p4,
        service_address = @p5,
        latitude = @p6,
        longitude = @p7,
        status = @p8,
        project_name = @p9,
        updated_at = @p10
    WHERE project_id = @p0 AND serial_number = @p1;
END
ELSE
BEGIN
    DECLARE @newId BIGINT;
    SELECT @newId = ISNULL(MAX(id), 0) + 1
    FROM dbo.INVInstallerReport WITH (UPDLOCK, HOLDLOCK);

    INSERT INTO dbo.INVInstallerReport
    (id, project_id, completion_date, installer_name, work_order_id, serial_number, service_address, latitude, longitude, status, project_name, created_at, updated_at)
    VALUES
    (@newId, @p0, @p2, @p3, @p4, @p1, @p5, @p6, @p7, @p8, @p9, @p10, @p10);
END
",
                                    projectIdStr2,
                                    serialNorm2,
                                    (object)(item.CompletionDate.HasValue ? (object)item.CompletionDate.Value : DBNull.Value),
                                    (object)installerNameFinal2,
                                    (object)(item.WorkOrderId ?? ""),
                                    (object)(item.ServiceAddress ?? ""),
                                    (object)(item.Latitude.HasValue ? (object)item.Latitude.Value : DBNull.Value),
                                    (object)(item.Longitude.HasValue ? (object)item.Longitude.Value : DBNull.Value),
                                    (object)(item.Status ?? ""),
                                    (object)(item.ProjectName ?? ""),
                                    now2
                                );
                                    if (affected2 > 0) invReportSaved++;
                                }
                                catch (Exception fallbackEx)
                                {
                                    // bubble up meaningful error so UI can show it
                                    throw new Exception("INVInstallerReport save failed (op_date err: " + opDateEx.Message + "). Fallback err: " + fallbackEx.Message);
                                }
                            }
                        }
                        catch (Exception itemEx)
                        {
                            errors++;
                            errorList.Add(serial + ": " + itemEx.Message);
                        }
                    }

                    return Json(new { success = true, installed, skipped, errors, invReportSaved, errorList });
                }
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