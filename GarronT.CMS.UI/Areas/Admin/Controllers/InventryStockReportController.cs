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
                    // Summary export: no Date/Time column (align with on-screen summary).
                    var summaryExportRows = (inventryStockSummaryReportExport ?? new List<InstallerStockReportSummaryBO>())
                        .Select(r => new InventryStockSummaryExportNoDate
                        {
                            InstallerName = r.UserName,
                            CategoryName = r.CategoryName,
                            ProductName = r.ProductName,
                            OpeningInventory = r.OpeningBalance,
                            ScannedInventory = r.TotalRelasedQty,
                            InstalledInventory = r.InstalledQty,
                            TransferredInventory = r.TransferredQty,
                            ReturnedInventory = r.ReturnedQty,
                            InventoryInHand = r.InHandInventory
                        }).ToList();
                    ExportInventryStockReport = new CommonFunctions().ToDataTable(summaryExportRows);
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
                            DateAndTime = !string.IsNullOrEmpty(a.DateAndTime) ? a.DateAndTime.Replace("<br/>", " ") : null,

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
        public class InventryStockSummaryExportNoDate
        {
            public string InstallerName { get; set; }
            public string CategoryName { get; set; }
            public string ProductName { get; set; }
            public int? OpeningInventory { get; set; }
            public int ScannedInventory { get; set; }
            public int InstalledInventory { get; set; }
            public int TransferredInventory { get; set; }
            public int ReturnedInventory { get; set; }
            public int? InventoryInHand { get; set; }
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
            public string CustomerName { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string WorkOrderType { get; set; }
            public string Cycle { get; set; }
            public string AppointmentDate { get; set; }
            public string AppointmentTime { get; set; }
            public string Route { get; set; }
            public string Sequence { get; set; }
            public string Area { get; set; }
            public string DispatchedFromDate { get; set; }
            public string DispatchedToDate { get; set; }
            public string OnsiteStartTime { get; set; }
            public string OnsiteEndTime { get; set; }
            public string LastCCReasonLegacy { get; set; }
            public string LastCCDate { get; set; }
            public string QAUser { get; set; }
            public string CCReason { get; set; }
            public string TotalVisits { get; set; }
            public string ProvidedLatitudeText { get; set; }
            public string ProvidedLongitudeText { get; set; }
            public string MeterNumber { get; set; }
            public string WorkFlowType { get; set; }
            public string ResponsibleParty { get; set; }
            public string CreateDateExcel { get; set; }
            public string QualityResults { get; set; }
            public string ExceptionReason { get; set; }
            public string RouteReleaseDate { get; set; }
            public string SyncDate { get; set; }
            public string WorkOrdersDEId { get; set; }
            public string UpdatedCCReason { get; set; }
            public string CustomSort { get; set; }
            public string StandardSort { get; set; }
            public string GPSAccuracy { get; set; }
            public string GPSSource { get; set; }
            public string LinkToWorkOrder { get; set; }
            public string ExceptionStatus { get; set; }
            public string WorkOrders { get; set; }
            public string Events { get; set; }
            public string CustomerEventsURL { get; set; }
            public string CustomerUniqueId { get; set; }
            public string WorkOrderIdURL { get; set; }
            public string WorkOrderRequestId { get; set; }
            public string CustomerId { get; set; }
            public string DeviceId { get; set; }
            public string NGSector { get; set; }
            public string CustomerIdAdd { get; set; }

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

                            if (IsMeterOrErtHeader(key))
                                serialColTemp = c;

                            if (key.Contains("INSTALLER") || key.Contains("EMAIL"))
                                emailColTemp = c;

                            if (key.Contains("COMPLETIONDATE") || (key.Contains("COMPLETION") && key.Contains("DATE")))
                                completionDateColTemp = c;

                            if (key.Contains("INSTALLERNAME") || (key.Contains("INSTALLER") && key.Contains("NAME")))
                                installerNameColTemp = c;

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

                // ✅ Build header map once from detected header row
                var headerMap = new Dictionary<string, int>();

                for (int c = 1; c <= targetSheet.Dimension.Columns; c++)
                {
                    string headerText = (targetSheet.Cells[headerRow, c].Text ?? "").Trim();
                    string key = NormalizeHeaderKey(headerText);

                    if (!string.IsNullOrWhiteSpace(key) && !headerMap.ContainsKey(key))
                        headerMap.Add(key, c);
                }

                int totalRows = targetSheet.Dimension.Rows;

                for (int r = headerRow + 1; r <= totalRows; r++)
                {
                    string serial = NormalizeSerialValue(targetSheet.Cells[r, idxSerial].Text);
                    string email = "";

                    if (idxEmail > 0)
                        email = ExtractEmail(targetSheet.Cells[r, idxEmail].Text);

                    if (string.IsNullOrWhiteSpace(email))
                    {
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

                        CompletionDate = (idxCompletionDate > 0)
                            ? ToDateSafe(targetSheet.Cells[r, idxCompletionDate].Text)
                            : null,

                        InstallerName = (idxInstallerName > 0)
                            ? (targetSheet.Cells[r, idxInstallerName].Text ?? "").Trim()
                            : "",

                        WorkOrderId = (idxWorkOrderId > 0)
                            ? (targetSheet.Cells[r, idxWorkOrderId].Text ?? "").Trim()
                            : "",

                        ServiceAddress = (idxServiceAddress > 0)
                            ? (targetSheet.Cells[r, idxServiceAddress].Text ?? "").Trim()
                            : "",

                        Latitude = (idxProvidedLatitude > 0)
                            ? (decimal?)ToDecimalSafe(targetSheet.Cells[r, idxProvidedLatitude].Text)
                            : ((idxLatitude > 0) ? (decimal?)ToDecimalSafe(targetSheet.Cells[r, idxLatitude].Text) : null),

                        Longitude = (idxProvidedLongitude > 0)
                            ? (decimal?)ToDecimalSafe(targetSheet.Cells[r, idxProvidedLongitude].Text)
                            : ((idxLongitude > 0) ? (decimal?)ToDecimalSafe(targetSheet.Cells[r, idxLongitude].Text) : null),

                        Status = (idxStatus > 0)
                            ? (targetSheet.Cells[r, idxStatus].Text ?? "").Trim()
                            : "",

                        ProjectName = (idxProjectName > 0)
                            ? (targetSheet.Cells[r, idxProjectName].Text ?? "").Trim()
                            : "",

                        // ✅ Add-on Excel header fields
                        CustomerName = GetCellByHeader(targetSheet, r, headerMap, "Customer Name"),
                        City = GetCellByHeader(targetSheet, r, headerMap, "City"),
                        State = GetCellByHeader(targetSheet, r, headerMap, "State"),
                        WorkOrderType = GetCellByHeader(targetSheet, r, headerMap, "Work Order Type"),
                        Cycle = GetCellByHeader(targetSheet, r, headerMap, "Cycle"),
                        AppointmentDate = GetCellByHeader(targetSheet, r, headerMap, "Appointment Date"),
                        AppointmentTime = GetCellByHeader(targetSheet, r, headerMap, "Appointment Time"),
                        Route = GetCellByHeader(targetSheet, r, headerMap, "Route"),
                        Sequence = GetCellByHeader(targetSheet, r, headerMap, "Sequence"),
                        Area = GetCellByHeader(targetSheet, r, headerMap, "Area"),
                        DispatchedFromDate = GetCellByHeader(targetSheet, r, headerMap, "Dispatched From Date"),
                        DispatchedToDate = GetCellByHeader(targetSheet, r, headerMap, "Dispatched To Date"),
                        OnsiteStartTime = GetCellByHeader(targetSheet, r, headerMap, "Onsite Start Time"),
                        OnsiteEndTime = GetCellByHeader(targetSheet, r, headerMap, "Onsite End Time"),
                        LastCCReasonLegacy = GetCellByHeader(targetSheet, r, headerMap, "Last CC Reason Legacy"),
                        LastCCDate = GetCellByHeader(targetSheet, r, headerMap, "Last CC Date"),
                        QAUser = GetCellByHeader(targetSheet, r, headerMap, "QA User"),
                        CCReason = GetCellByHeader(targetSheet, r, headerMap, "CC Reason"),
                        TotalVisits = GetCellByHeader(targetSheet, r, headerMap, "Total Visits"),
                        ProvidedLatitudeText = GetCellByHeader(targetSheet, r, headerMap, "Provided Latitude"),
                        ProvidedLongitudeText = GetCellByHeader(targetSheet, r, headerMap, "Provided Longitude"),
                        MeterNumber = GetCellByHeader(targetSheet, r, headerMap, "Meter Number"),
                        WorkFlowType = GetCellByHeader(targetSheet, r, headerMap, "Work Flow Type"),
                        ResponsibleParty = GetCellByHeader(targetSheet, r, headerMap, "Responsible Party"),
                        CreateDateExcel = GetCellByHeader(targetSheet, r, headerMap, "Create Date"),
                        QualityResults = GetCellByHeader(targetSheet, r, headerMap, "Quality Results"),
                        ExceptionReason = GetCellByHeader(targetSheet, r, headerMap, "Exception Reason"),
                        RouteReleaseDate = GetCellByHeader(targetSheet, r, headerMap, "Route Release Date"),
                        SyncDate = GetCellByHeader(targetSheet, r, headerMap, "Sync Date"),
                        WorkOrdersDEId = GetCellByHeader(targetSheet, r, headerMap, "Work Orders DE ID"),
                        UpdatedCCReason = GetCellByHeader(targetSheet, r, headerMap, "Updated CC Reason"),
                        CustomSort = GetCellByHeader(targetSheet, r, headerMap, "Custom Sort"),
                        StandardSort = GetCellByHeader(targetSheet, r, headerMap, "Standard Sort"),
                        GPSAccuracy = GetCellByHeader(targetSheet, r, headerMap, "GPS Accuracy"),
                        GPSSource = GetCellByHeader(targetSheet, r, headerMap, "GPS Source"),
                        LinkToWorkOrder = GetCellByHeader(targetSheet, r, headerMap, "Link To Work Order"),
                        ExceptionStatus = GetCellByHeader(targetSheet, r, headerMap, "Exception Status"),
                        WorkOrders = GetCellByHeader(targetSheet, r, headerMap, "Work Orders"),
                        Events = GetCellByHeader(targetSheet, r, headerMap, "Events"),
                        CustomerEventsURL = GetCellByHeader(targetSheet, r, headerMap, "Customer Events URL"),
                        CustomerUniqueId = GetCellByHeader(targetSheet, r, headerMap, "Customer Unique Id"),
                        WorkOrderIdURL = GetCellByHeader(targetSheet, r, headerMap, "Work Order Id URL"),
                        WorkOrderRequestId = GetCellByHeader(targetSheet, r, headerMap, "Work Order Request Id"),
                        CustomerId = GetCellByHeader(targetSheet, r, headerMap, "Customer Id"),
                        DeviceId = GetCellByHeader(targetSheet, r, headerMap, "Device Id"),
                        NGSector = GetCellByHeader(targetSheet, r, headerMap, "NG Sector"),
                        CustomerIdAdd = GetCellByHeader(targetSheet, r, headerMap, "CustomerId Add")
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

                var serials = JsonConvert.DeserializeObject<List<CompletedInventorySerialItem>>(serialList ?? "[]")
                    ?? new List<CompletedInventorySerialItem>();

                // Prefer taking full row details from the uploaded file already stored in Session
                var sessionPayload = Session["CompletedInventory_" + projectId] as CompletedInventoryPayload;

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

                    int installed = 0;
                    int skipped = 0;
                    int errors = 0;
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

                        var key = NormalizeSerialKey(serial);

                        if (fileBySerial.ContainsKey(key))
                        {
                            var fromFile = fileBySerial[key];

                            // Existing fields from uploaded file/session
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

                            // Add-on Excel header fields from uploaded file/session
                            item.CustomerName = fromFile.CustomerName;
                            item.City = fromFile.City;
                            item.State = fromFile.State;
                            item.WorkOrderType = fromFile.WorkOrderType;
                            item.Cycle = fromFile.Cycle;
                            item.AppointmentDate = fromFile.AppointmentDate;
                            item.AppointmentTime = fromFile.AppointmentTime;
                            item.Route = fromFile.Route;
                            item.Sequence = fromFile.Sequence;
                            item.Area = fromFile.Area;
                            item.DispatchedFromDate = fromFile.DispatchedFromDate;
                            item.DispatchedToDate = fromFile.DispatchedToDate;
                            item.OnsiteStartTime = fromFile.OnsiteStartTime;
                            item.OnsiteEndTime = fromFile.OnsiteEndTime;
                            item.LastCCReasonLegacy = fromFile.LastCCReasonLegacy;
                            item.LastCCDate = fromFile.LastCCDate;
                            item.QAUser = fromFile.QAUser;
                            item.CCReason = fromFile.CCReason;
                            item.TotalVisits = fromFile.TotalVisits;
                            item.ProvidedLatitudeText = fromFile.ProvidedLatitudeText;
                            item.ProvidedLongitudeText = fromFile.ProvidedLongitudeText;
                            item.MeterNumber = fromFile.MeterNumber;
                            item.WorkFlowType = fromFile.WorkFlowType;
                            item.ResponsibleParty = fromFile.ResponsibleParty;
                            item.CreateDateExcel = fromFile.CreateDateExcel;
                            item.QualityResults = fromFile.QualityResults;
                            item.ExceptionReason = fromFile.ExceptionReason;
                            item.RouteReleaseDate = fromFile.RouteReleaseDate;
                            item.SyncDate = fromFile.SyncDate;
                            item.WorkOrdersDEId = fromFile.WorkOrdersDEId;
                            item.UpdatedCCReason = fromFile.UpdatedCCReason;
                            item.CustomSort = fromFile.CustomSort;
                            item.StandardSort = fromFile.StandardSort;
                            item.GPSAccuracy = fromFile.GPSAccuracy;
                            item.GPSSource = fromFile.GPSSource;
                            item.LinkToWorkOrder = fromFile.LinkToWorkOrder;
                            item.ExceptionStatus = fromFile.ExceptionStatus;
                            item.WorkOrders = fromFile.WorkOrders;
                            item.Events = fromFile.Events;
                            item.CustomerEventsURL = fromFile.CustomerEventsURL;
                            item.CustomerUniqueId = fromFile.CustomerUniqueId;
                            item.WorkOrderIdURL = fromFile.WorkOrderIdURL;
                            item.WorkOrderRequestId = fromFile.WorkOrderRequestId;
                            item.CustomerId = fromFile.CustomerId;
                            item.DeviceId = fromFile.DeviceId;
                            item.NGSector = fromFile.NGSector;
                            item.CustomerIdAdd = fromFile.CustomerIdAdd;
                        }
                        else
                        {
                            item.ProjectName = selectedProjectName;
                        }

                        long installerUserId = userId;

                        string installerEmail = (item.InstallerEmail ?? "").Trim().ToUpperInvariant();

                        if (!string.IsNullOrEmpty(installerEmail) && userMapByEmail.ContainsKey(installerEmail))
                        {
                            installerUserId = userMapByEmail[installerEmail];
                        }

                        try
                        {
                            // Existing logic: Inventory In Hand se Installed me move
                            dal.MoveInventoryToInstalled(projectId, installerUserId, serial, masterId);
                            installed++;

                            // New logic: INVInstallerReport me stored procedure se save/update
                            try
                            {
                                string serialNorm = (serial ?? "").Trim();

                                string installerNameFinal =
                                    (!string.IsNullOrWhiteSpace(item.TechnicianName)
                                        ? item.TechnicianName
                                        : item.InstallerName) ?? "";

                                db.Database.ExecuteSqlCommand(@"
EXEC dbo.SP_Upsert_INVInstallerReport
    @project_id,
    @serial_number,
    @completion_date,
    @installer_name,
    @work_order_id,
    @service_address,
    @latitude,
    @longitude,
    @status,
    @project_name,
    @operation_date,

    @customer_name,
    @city,
    @state,
    @work_order_type,
    @cycle,
    @appointment_date,
    @appointment_time,
    @route,
    @sequence,
    @area,
    @dispatched_from_date,
    @dispatched_to_date,
    @onsite_start_time,
    @onsite_end_time,
    @last_cc_reason_legacy,
    @last_cc_date,
    @qa_user,
    @cc_reason,
    @total_visits,
    @provided_latitude,
    @provided_longitude,
    @meter_number,
    @work_flow_type,
    @responsible_party,
    @create_date_excel,
    @quality_results,
    @exception_reason,
    @route_release_date,
    @sync_date,
    @work_orders_de_id,
    @updated_cc_reason,
    @custom_sort,
    @standard_sort,
    @gps_accuracy,
    @gps_source,
    @link_to_work_order,
    @exception_status,
    @work_orders,
    @events,
    @customer_events_url,
    @customer_unique_id,
    @work_order_id_url,
    @work_order_request_id,
    @customer_id,
    @device_id,
    @ng_sector,
    @customerid_add
",
                                    new SqlParameter("@project_id", projectId),
                                    new SqlParameter("@serial_number", (object)serialNorm ?? DBNull.Value),

                                    new SqlParameter("@completion_date", item.CompletionDate.HasValue ? (object)item.CompletionDate.Value : DBNull.Value),
                                    new SqlParameter("@installer_name", (object)installerNameFinal ?? DBNull.Value),
                                    new SqlParameter("@work_order_id", (object)(item.WorkOrderId ?? "") ?? DBNull.Value),
                                    new SqlParameter("@service_address", (object)(item.ServiceAddress ?? "") ?? DBNull.Value),
                                    new SqlParameter("@latitude", item.Latitude.HasValue ? (object)item.Latitude.Value.ToString() : DBNull.Value),
                                    new SqlParameter("@longitude", item.Longitude.HasValue ? (object)item.Longitude.Value.ToString() : DBNull.Value),
                                    new SqlParameter("@status", (object)(item.Status ?? "") ?? DBNull.Value),
                                    new SqlParameter("@project_name", (object)(item.ProjectName ?? "") ?? DBNull.Value),
                                    new SqlParameter("@operation_date", item.CompletionDate.HasValue ? (object)item.CompletionDate.Value : DateTime.Now),

                                    new SqlParameter("@customer_name", (object)(item.CustomerName ?? "") ?? DBNull.Value),
                                    new SqlParameter("@city", (object)(item.City ?? "") ?? DBNull.Value),
                                    new SqlParameter("@state", (object)(item.State ?? "") ?? DBNull.Value),
                                    new SqlParameter("@work_order_type", (object)(item.WorkOrderType ?? "") ?? DBNull.Value),
                                    new SqlParameter("@cycle", (object)(item.Cycle ?? "") ?? DBNull.Value),
                                    new SqlParameter("@appointment_date", (object)(item.AppointmentDate ?? "") ?? DBNull.Value),
                                    new SqlParameter("@appointment_time", (object)(item.AppointmentTime ?? "") ?? DBNull.Value),
                                    new SqlParameter("@route", (object)(item.Route ?? "") ?? DBNull.Value),
                                    new SqlParameter("@sequence", (object)(item.Sequence ?? "") ?? DBNull.Value),
                                    new SqlParameter("@area", (object)(item.Area ?? "") ?? DBNull.Value),
                                    new SqlParameter("@dispatched_from_date", (object)(item.DispatchedFromDate ?? "") ?? DBNull.Value),
                                    new SqlParameter("@dispatched_to_date", (object)(item.DispatchedToDate ?? "") ?? DBNull.Value),
                                    new SqlParameter("@onsite_start_time", (object)(item.OnsiteStartTime ?? "") ?? DBNull.Value),
                                    new SqlParameter("@onsite_end_time", (object)(item.OnsiteEndTime ?? "") ?? DBNull.Value),
                                    new SqlParameter("@last_cc_reason_legacy", (object)(item.LastCCReasonLegacy ?? "") ?? DBNull.Value),
                                    new SqlParameter("@last_cc_date", (object)(item.LastCCDate ?? "") ?? DBNull.Value),
                                    new SqlParameter("@qa_user", (object)(item.QAUser ?? "") ?? DBNull.Value),
                                    new SqlParameter("@cc_reason", (object)(item.CCReason ?? "") ?? DBNull.Value),
                                    new SqlParameter("@total_visits", (object)(item.TotalVisits ?? "") ?? DBNull.Value),
                                    new SqlParameter("@provided_latitude", (object)(item.ProvidedLatitudeText ?? "") ?? DBNull.Value),
                                    new SqlParameter("@provided_longitude", (object)(item.ProvidedLongitudeText ?? "") ?? DBNull.Value),
                                    new SqlParameter("@meter_number", (object)(item.MeterNumber ?? "") ?? DBNull.Value),
                                    new SqlParameter("@work_flow_type", (object)(item.WorkFlowType ?? "") ?? DBNull.Value),
                                    new SqlParameter("@responsible_party", (object)(item.ResponsibleParty ?? "") ?? DBNull.Value),
                                    new SqlParameter("@create_date_excel", (object)(item.CreateDateExcel ?? "") ?? DBNull.Value),
                                    new SqlParameter("@quality_results", (object)(item.QualityResults ?? "") ?? DBNull.Value),
                                    new SqlParameter("@exception_reason", (object)(item.ExceptionReason ?? "") ?? DBNull.Value),
                                    new SqlParameter("@route_release_date", (object)(item.RouteReleaseDate ?? "") ?? DBNull.Value),
                                    new SqlParameter("@sync_date", (object)(item.SyncDate ?? "") ?? DBNull.Value),
                                    new SqlParameter("@work_orders_de_id", (object)(item.WorkOrdersDEId ?? "") ?? DBNull.Value),
                                    new SqlParameter("@updated_cc_reason", (object)(item.UpdatedCCReason ?? "") ?? DBNull.Value),
                                    new SqlParameter("@custom_sort", (object)(item.CustomSort ?? "") ?? DBNull.Value),
                                    new SqlParameter("@standard_sort", (object)(item.StandardSort ?? "") ?? DBNull.Value),
                                    new SqlParameter("@gps_accuracy", (object)(item.GPSAccuracy ?? "") ?? DBNull.Value),
                                    new SqlParameter("@gps_source", (object)(item.GPSSource ?? "") ?? DBNull.Value),
                                    new SqlParameter("@link_to_work_order", (object)(item.LinkToWorkOrder ?? "") ?? DBNull.Value),
                                    new SqlParameter("@exception_status", (object)(item.ExceptionStatus ?? "") ?? DBNull.Value),
                                    new SqlParameter("@work_orders", (object)(item.WorkOrders ?? "") ?? DBNull.Value),
                                    new SqlParameter("@events", (object)(item.Events ?? "") ?? DBNull.Value),
                                    new SqlParameter("@customer_events_url", (object)(item.CustomerEventsURL ?? "") ?? DBNull.Value),
                                    new SqlParameter("@customer_unique_id", (object)(item.CustomerUniqueId ?? "") ?? DBNull.Value),
                                    new SqlParameter("@work_order_id_url", (object)(item.WorkOrderIdURL ?? "") ?? DBNull.Value),
                                    new SqlParameter("@work_order_request_id", (object)(item.WorkOrderRequestId ?? "") ?? DBNull.Value),
                                    new SqlParameter("@customer_id", (object)(item.CustomerId ?? "") ?? DBNull.Value),
                                    new SqlParameter("@device_id", (object)(item.DeviceId ?? "") ?? DBNull.Value),
                                    new SqlParameter("@ng_sector", (object)(item.NGSector ?? "") ?? DBNull.Value),
                                    new SqlParameter("@customerid_add", (object)(item.CustomerIdAdd ?? "") ?? DBNull.Value)
                                );

                                invReportSaved++;
                            }
                            catch (Exception reportEx)
                            {
                                throw new Exception("INVInstallerReport SP save failed: " + reportEx.Message);
                            }
                        }
                        catch (Exception itemEx)
                        {
                            errors++;
                            errorList.Add(serial + ": " + itemEx.Message);
                        }
                    }

                    return Json(new
                    {
                        success = true,
                        installed,
                        skipped,
                        errors,
                        invReportSaved,
                        errorList
                    });
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

        [HttpGet]
        public ActionResult GetWarehouseByProject(long projectId)
        {
            try
            {
                if (projectId <= 0)
                {
                    return Json(new List<WarehouseBO>(), JsonRequestBehavior.AllowGet);
                }

                List<WarehouseBO> warehouses = new InventoryDAL().GetWareHouseList(projectId);
                return Json(warehouses ?? new List<WarehouseBO>(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error("GetWarehouseByProject failed", ex);
                return Json(new List<WarehouseBO>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ReturnToWarehouse(long projectId, long userId, long productId, string serialNumber, int qty, long warehouseId)
        {
            try
            {
                if (projectId <= 0)
                {
                    return Json(new { success = false, message = "Please select a project." });
                }
                if (userId <= 0)
                {
                    return Json(new { success = false, message = "Installer is missing for this inventory row." });
                }
                if (warehouseId <= 0)
                {
                    return Json(new { success = false, message = "Please select a warehouse." });
                }
                if (qty <= 0)
                {
                    return Json(new { success = false, message = "Return quantity must be greater than 0." });
                }

                List<WarehouseBO> projectWarehouses = new InventoryDAL().GetWareHouseList(projectId);
                if (projectWarehouses == null || !projectWarehouses.Any(w => w.WarehouseId == warehouseId))
                {
                    return Json(new { success = false, message = "Selected warehouse does not belong to this project." });
                }

                string serial = (serialNumber ?? "").Trim();
                bool isSerialItem = !string.IsNullOrWhiteSpace(serial);

                if (isSerialItem)
                {
                    qty = 1;
                }
                else if (productId <= 0)
                {
                    return Json(new { success = false, message = "Product is missing for this inventory row." });
                }

                string productName = "";
                using (CompassEntities db = new CompassEntities())
                {
                    productName = db.ProductMasters
                        .Where(p => p.Id == productId)
                        .Select(p => p.ProductName)
                        .FirstOrDefault() ?? "";
                }

                var returnStock = new MobileReturnStockBO
                {
                    UserId = userId,
                    ProjectId = projectId,
                    Id = 1,
                    ChildId = warehouseId,
                    DeviceIPAddress = Request.UserHostAddress ?? "WEB",
                    DeviceGpsLocation = "WEB",
                    SerialNumberList = isSerialItem
                        ? new List<MobileReturnStockSerialChildBO>
                        {
                            new MobileReturnStockSerialChildBO { SerialNumber = serial, TotalQty = 1 }
                        }
                        : null,
                    ProductQuantityList = isSerialItem
                        ? null
                        : new List<MobileReturnStockNonSerialChildBO>
                        {
                            new MobileReturnStockNonSerialChildBO
                            {
                                ProductId = productId,
                                ProductName = productName,
                                TotalQty = qty
                            }
                        }
                };

                string jsonData = JsonConvert.SerializeObject(returnStock);
                string message;
                bool result;

                using (WebServiceReturnInventoryDAL returnDal = new WebServiceReturnInventoryDAL())
                {
                    result = returnDal.ReturnStockToWarehouseFromWeb(
                        userId,
                        projectId,
                        warehouseId,
                        productId,
                        productName,
                        serial,
                        qty,
                        jsonData,
                        Request.UserHostAddress ?? "WEB",
                        out message);
                }

                if (!result)
                {
                    Log.Error("ReturnToWarehouse failed: " + message);
                    return Json(new
                    {
                        success = false,
                        message = string.IsNullOrWhiteSpace(message) ? "Unable to return inventory to warehouse." : message
                    });
                }

                return Json(new { success = true, message = string.IsNullOrWhiteSpace(message) ? "Inventory returned to warehouse successfully." : message });
            }
            catch (Exception ex)
            {
                Log.Error("ReturnToWarehouse failed", ex);
                string error = ex.Message;
                if (ex.InnerException != null)
                {
                    error += " | " + ex.InnerException.Message;
                }
                return Json(new { success = false, message = error });
            }
        }

        [HttpGet]
        public ActionResult GetUnusedInventoryPreview(long projectId, long installerId, long productId, string txtFromDate, string txtToDate)
        {
            try
            {
                if (projectId <= 0)
                {
                    return Json(new { success = false, message = "Please select a project." }, JsonRequestBehavior.AllowGet);
                }

                var unusedRows = GetUnusedInventoryRows(projectId, installerId, productId, txtFromDate, txtToDate);
                var users = unusedRows
                    .GroupBy(x => new { x.UserId, x.UserName })
                    .Select(g => new
                    {
                        UserId = g.Key.UserId,
                        UserName = g.Key.UserName,
                        SerialItems = g.Where(x => !string.IsNullOrWhiteSpace(x.SerialNumber))
                            .Select(x => new
                            {
                                UserId = g.Key.UserId,
                                ProductId = x.FKProductId,
                                ProductName = x.ProductName,
                                SerialNumber = (x.SerialNumber ?? "").Trim(),
                                Qty = 1,
                                MaxQty = 1
                            })
                            .OrderBy(x => x.ProductName)
                            .ThenBy(x => x.SerialNumber)
                            .ToList(),
                        ConsumableItems = g.Where(x => string.IsNullOrWhiteSpace(x.SerialNumber))
                            .GroupBy(x => new { x.FKProductId, x.ProductName })
                            .Select(p => new
                            {
                                UserId = g.Key.UserId,
                                ProductId = p.Key.FKProductId,
                                ProductName = p.Key.ProductName,
                                SerialNumber = "",
                                Qty = p.Sum(x => x.InHandQty ?? 0),
                                MaxQty = p.Sum(x => x.InHandQty ?? 0)
                            })
                            .Where(x => x.Qty > 0)
                            .OrderBy(x => x.ProductName)
                            .ToList()
                    })
                    .Select(u => new
                    {
                        u.UserId,
                        u.UserName,
                        ItemCount = u.SerialItems.Count + u.ConsumableItems.Count,
                        Qty = u.SerialItems.Sum(x => x.Qty) + u.ConsumableItems.Sum(x => x.Qty),
                        SerialItems = u.SerialItems,
                        ConsumableItems = u.ConsumableItems
                    })
                    .OrderBy(x => x.UserName)
                    .ToList();

                var jsonResult = Json(new
                {
                    success = true,
                    userCount = users.Count,
                    itemCount = users.Sum(x => x.ItemCount),
                    totalQty = users.Sum(x => x.Qty),
                    users = users
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                Log.Error("GetUnusedInventoryPreview failed", ex);
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ReturnUnusedInventory(long projectId, long installerId, long productId, string txtFromDate, string txtToDate, long warehouseId, string itemsJson)
        {
            try
            {
                if (projectId <= 0)
                {
                    return Json(new { success = false, message = "Please select a project." });
                }
                if (warehouseId <= 0)
                {
                    return Json(new { success = false, message = "Please select a warehouse." });
                }
                if (string.IsNullOrWhiteSpace(itemsJson))
                {
                    return Json(new { success = false, message = "Please select unused inventory to return." });
                }

                List<WarehouseBO> projectWarehouses = new InventoryDAL().GetWareHouseList(projectId);
                if (projectWarehouses == null || !projectWarehouses.Any(w => w.WarehouseId == warehouseId))
                {
                    return Json(new { success = false, message = "Selected warehouse does not belong to this project." });
                }

                List<WebReturnStockItem> postedItems = JsonConvert.DeserializeObject<List<WebReturnStockItem>>(itemsJson)
                    ?? new List<WebReturnStockItem>();

                var unusedRows = GetUnusedInventoryRows(projectId, installerId, productId, txtFromDate, txtToDate);
                var unusedSerials = new HashSet<string>(unusedRows
                    .Where(x => !string.IsNullOrWhiteSpace(x.SerialNumber))
                    .Select(x => (x.UserId + "|" + x.FKProductId + "|" + (x.SerialNumber ?? "").Trim()).ToLower()));
                var unusedConsumableQty = unusedRows
                    .Where(x => string.IsNullOrWhiteSpace(x.SerialNumber))
                    .GroupBy(x => x.UserId + "|" + x.FKProductId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.InHandQty ?? 0));

                postedItems = postedItems
                    .Where(x => x != null && x.UserId > 0 && x.Qty > 0)
                    .Select(x =>
                    {
                        string serial = (x.SerialNumber ?? "").Trim();
                        if (!string.IsNullOrWhiteSpace(serial))
                        {
                            string serialKey = (x.UserId + "|" + x.ProductId + "|" + serial).ToLower();
                            x.Qty = unusedSerials.Contains(serialKey) ? 1 : 0;
                        }
                        else
                        {
                            int maxQty;
                            unusedConsumableQty.TryGetValue(x.UserId + "|" + x.ProductId, out maxQty);
                            if (x.Qty > maxQty)
                            {
                                x.Qty = maxQty;
                            }
                        }
                        return x;
                    })
                    .Where(x => x.Qty > 0)
                    .ToList();

                if (postedItems.Count == 0)
                {
                    return Json(new { success = false, message = "Please select unused inventory to return." });
                }

                var userGroups = postedItems.GroupBy(x => x.UserId).ToList();
                int successUsers = 0;
                int failedUsers = 0;
                int returnedQty = 0;
                List<string> errors = new List<string>();
                DateTime requestBase = DateTime.Now;

                using (WebServiceReturnInventoryDAL returnDal = new WebServiceReturnInventoryDAL())
                {
                    int userIndex = 0;
                    foreach (var userGroup in userGroups)
                    {
                        long userId = userGroup.Key;
                        List<WebReturnStockItem> items = userGroup.Select(x => new WebReturnStockItem
                        {
                            ProductId = x.ProductId,
                            ProductName = x.ProductName,
                            SerialNumber = (x.SerialNumber ?? "").Trim(),
                            Qty = !string.IsNullOrWhiteSpace(x.SerialNumber) ? 1 : x.Qty
                        }).Where(x => x.Qty > 0).ToList();

                        if (items.Count == 0)
                        {
                            continue;
                        }

                        string jsonData = JsonConvert.SerializeObject(new
                        {
                            UserId = userId,
                            ProjectId = projectId,
                            WarehouseId = warehouseId,
                            Source = "WEB_BULK_UNUSED",
                            Items = items
                        });

                        string message;
                        DateTime userRequestDate = requestBase.AddSeconds(userIndex);
                        bool result = returnDal.ReturnStockToWarehouseFromWeb(
                            userId,
                            projectId,
                            warehouseId,
                            items,
                            jsonData,
                            Request.UserHostAddress ?? "WEB",
                            userRequestDate,
                            out message);

                        if (result)
                        {
                            successUsers++;
                            returnedQty += items.Sum(x => x.Qty);
                        }
                        else
                        {
                            failedUsers++;
                            errors.Add("User " + userId + ": " + (string.IsNullOrWhiteSpace(message) ? "Unable to return unused inventory." : message));
                        }

                        userIndex++;
                    }
                }

                if (successUsers == 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = errors.Count > 0 ? string.Join(" ", errors) : "Unable to return unused inventory to warehouse."
                    });
                }

                string resultMessage = "Unused inventory returned to warehouse for " + successUsers + " user(s). Total qty: " + returnedQty + ".";
                if (failedUsers > 0)
                {
                    resultMessage += " Failed for " + failedUsers + " user(s).";
                }

                return Json(new { success = true, message = resultMessage, errors });
            }
            catch (Exception ex)
            {
                Log.Error("ReturnUnusedInventory failed", ex);
                string error = ex.Message;
                if (ex.InnerException != null)
                {
                    error += " | " + ex.InnerException.Message;
                }
                return Json(new { success = false, message = error });
            }
        }

        private List<PROC_InstallStockDetailReport_Result> GetUnusedInventoryRows(long projectId, long installerId, long productId, string txtFromDate, string txtToDate)
        {
            List<PROC_InstallStockDetailReport_Result> reportData =
                new InventoryDAL().GetInventryStockReport(projectId, installerId, productId, txtFromDate, txtToDate)
                ?? new List<PROC_InstallStockDetailReport_Result>();

            return reportData
                .Where(x => x != null && x.UserId > 0 && (x.InHandQty ?? 0) > 0)
                .ToList();
        }

        private string GetCellByHeader(ExcelWorksheet ws, int row, Dictionary<string, int> headerMap, string headerName)
        {
            if (ws == null || headerMap == null || string.IsNullOrWhiteSpace(headerName))
                return "";

            string key = NormalizeHeaderKey(headerName);

            if (!headerMap.ContainsKey(key))
                return "";

            int col = headerMap[key];

            return (ws.Cells[row, col].Text ?? "").Trim();
        }

    }
}