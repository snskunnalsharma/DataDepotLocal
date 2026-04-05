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
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using System.Drawing;

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

        [HttpPost]
        public JsonResult UploadCompletedInventory(HttpPostedFileBase file, long projectId)
        {
            try
            {
                if (projectId != 10135 && projectId != 10136)
                    return Json(new { success = false, message = "This feature is enabled only for Projects 10135 and 10136." });

                if (file == null || file.ContentLength == 0)
                    return Json(new { success = false, message = "No file uploaded." });

                var payload = ReadCompletedInventoryFromFile(file);
                Session["CompletedInventory_" + projectId] = payload;

                return Json(new
                {
                    success = true,
                    serialCount = payload.SerialItems.Count
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

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult DownloadComparisonExcel(long projectId, string greenJson, string yellowJson, string redJson)
        {
            var green = JsonConvert.DeserializeObject<List<CompletedInventorySerialItem>>(greenJson ?? "[]");
            var red = JsonConvert.DeserializeObject<List<CompletedInventorySerialItem>>(redJson ?? "[]");

            using (ExcelPackage pck = new ExcelPackage())
            {
                // Cache installer email -> installer userId to avoid repeated DB lookups.
                var installerIdByEmail = new Dictionary<string, long?>(StringComparer.OrdinalIgnoreCase);
                long? ResolveInstallerId(string installerEmail)
                {
                    var email = (installerEmail ?? "").Trim();
                    if (string.IsNullOrWhiteSpace(email)) return null;
                    var key = email.ToUpperInvariant();
                    if (installerIdByEmail.TryGetValue(key, out var cached)) return cached;

                    using (var db = new CompassEntities())
                    {
                        var installerId = db.tblUsers
                            .Where(x => x.Email != null && x.Email.Trim().ToUpper() == key)
                            .Select(x => (long?)x.UserID)
                            .FirstOrDefault();

                        installerIdByEmail[key] = installerId;
                        return installerId;
                    }
                }

                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Inventory Comparison");

                ws.Cells[1, 1].Value = "Product Name";
                ws.Cells[1, 2].Value = "Serial Number";
                ws.Cells[1, 3].Value = "Technician";
                ws.Cells[1, 4].Value = "Status";
                ws.Cells[1, 5].Value = "Remaining Qty";
                ws.Cells[1, 6].Value = "Installer_Email";
                ws.Row(1).Style.Font.Bold = true;

                int row = 2;
                string Normalize(string v)
                {
                    return (v ?? "").Trim().Replace(".", "").Replace(" ", "").ToUpper();
                }

                // Build product lookup for matched serials from stock details.
                var matchedSerialKeys = green
                    .Where(x => x != null && !string.IsNullOrWhiteSpace(x.SerialNumber))
                    .Select(x => Normalize(x.SerialNumber))
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToList();

                var productNameBySerial = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (matchedSerialKeys.Count > 0)
                {
                    using (var db = new CompassEntities())
                    {
                        var stockRows = db.StockDetails
                            .Where(x => x.Active == 1 && x.FKProjectId == projectId && x.SerialNumber != null)
                            .Select(x => new { x.SerialNumber, x.ProductName, x.Id })
                            .ToList();

                        productNameBySerial = stockRows
                            .Select(x => new
                            {
                                SerialKey = Normalize(x.SerialNumber),
                                ProductName = x.ProductName,
                                x.Id
                            })
                            .Where(x => matchedSerialKeys.Contains(x.SerialKey))
                            .GroupBy(x => x.SerialKey)
                            .ToDictionary(
                                g => g.Key,
                                g => g.OrderByDescending(x => x.Id)
                                      .Select(x => x.ProductName)
                                      .FirstOrDefault() ?? "",
                                StringComparer.OrdinalIgnoreCase
                            );
                    }
                }

                foreach (var item in green)
                {
                    var installerEmail = (item.InstallerEmail ?? "").Trim();
                    var serialKey = Normalize(item.SerialNumber);
                    var resolvedProductName = item.ProductName ?? "";
                    if (string.IsNullOrWhiteSpace(resolvedProductName) && !string.IsNullOrWhiteSpace(serialKey))
                    {
                        resolvedProductName = productNameBySerial.ContainsKey(serialKey) ? productNameBySerial[serialKey] : "";
                    }

                    ws.Cells[row, 1].Value = resolvedProductName;
                    ws.Cells[row, 2].Value = item.SerialNumber ?? "";
                    ws.Cells[row, 3].Value = item.TechnicianName ?? "";

                    bool hasInstallerEmail = !string.IsNullOrWhiteSpace(installerEmail);
                    bool emailExistsInUsers = false;
                    if (hasInstallerEmail)
                    {
                        // If ResolveInstallerId returns a value, email exists in tblUsers.
                        var installerId = ResolveInstallerId(installerEmail);
                        emailExistsInUsers = installerId.HasValue;
                    }

                    if (!hasInstallerEmail)
                    {
                        ws.Cells[row, 4].Value = "MATCHED - Error (No Installer Email)";
                    }
                    else if (!emailExistsInUsers)
                    {
                        ws.Cells[row, 4].Value = "MATCHED - Error (Installer Email Not Found)";
                    }
                    else
                    {
                        ws.Cells[row, 4].Value = "MATCHED";
                    }

                    ws.Cells[row, 5].Value = 1;
                    ws.Cells[row, 6].Value = hasInstallerEmail ? installerEmail : "";

                    ws.Cells[row, 1, row, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, 1, row, 6].Style.Fill.BackgroundColor.SetColor(
                        hasInstallerEmail && emailExistsInUsers ? Color.LightGreen : Color.Silver
                    );
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
                    "Warehouse_Inventory_Comparison_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx"
                );
            }
        }

        [HttpPost]
        public JsonResult SaveMissingInventory(long projectId, string redJson)
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
                        return (v ?? "").Trim().Replace(".", "").Replace(" ", "").ToUpper();
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

        [HttpPost]
        public JsonResult MoveMatchedToOutward(long projectId, string greenJson)
        {
            try
            {
                if (projectId != 10135 && projectId != 10136)
                    return Json(new { success = false, message = "Allowed only for Projects 10135 and 10136." });

                var green = JsonConvert.DeserializeObject<List<CompletedInventorySerialItem>>(greenJson ?? "[]");
                if (green == null || green.Count == 0)
                    return Json(new { success = true, moved = 0, skipped = 0, errors = 0 });

                using (var db = new CompassEntities())
                {
                    var master = db.tblProjectFieldDataMasters
                        .Where(x => x.Active == 1)
                        .OrderByDescending(x => x.ID)
                        .FirstOrDefault();

                    if (master == null)
                        return Json(new { success = false, message = "Master record not found." });

                    var userMapByEmail = db.tblUsers
                        .Where(x => x.Active == 1 && x.Email != null)
                        .AsEnumerable()
                        .GroupBy(x => x.Email.Trim().ToUpper())
                        .ToDictionary(g => g.Key, g => g.First().UserID);

                    string Normalize(string v)
                    {
                        return (v ?? "").Trim().Replace(".", "").Replace(" ", "").ToUpper();
                    }

                    var unique = green
                        .Where(x => x != null && !string.IsNullOrWhiteSpace(x.SerialNumber))
                        .GroupBy(x => Normalize(x.SerialNumber))
                        .Select(g => g.First())
                        .ToList();

                    int moved = 0, skipped = 0, errors = 0;
                    var dal = new InventoryDAL();

                    foreach (var item in unique)
                    {
                        var installerEmail = (item.InstallerEmail ?? "").Trim().ToUpper();
                        if (string.IsNullOrWhiteSpace(installerEmail) || !userMapByEmail.ContainsKey(installerEmail))
                        {
                            skipped++;
                            continue;
                        }

                        try
                        {
                            dal.MoveInventoryToInstalled(
                                projectId,
                                userMapByEmail[installerEmail],
                                item.SerialNumber,
                                master.ID
                            );
                            moved++;
                        }
                        catch
                        {
                            errors++;
                        }
                    }

                    return Json(new { success = true, moved, skipped, errors });
                }
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
            public string InstallerEmail { get; set; }
        }

        public class CompletedInventoryPayload
        {
            public List<CompletedInventorySerialItem> SerialItems { get; set; } = new List<CompletedInventorySerialItem>();
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
                for (int i = 0; i < headers.Length; i++)
                {
                    var key = NormalizeHeaderKey(headers[i]);
                    if (IsMeterOrErtHeader(key)) idxSerial = i;
                    if (key.Contains("INSTALLER") || key.Contains("EMAIL")) idxEmail = i;
                }
                if (idxSerial == -1)
                    throw new Exception("Meter Number column not found in completed inventory file.");
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var cols = line.Split(delimiter);
                    string serial = (cols.Length > idxSerial) ? NormalizeSerialValue(cols[idxSerial]) : "";
                    string email = (idxEmail >= 0 && cols.Length > idxEmail) ? ExtractEmail(cols[idxEmail]) : "";
                    if (string.IsNullOrWhiteSpace(serial)) continue;
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
                // If same serial appears multiple times in upload, prefer a record with non-empty InstallerEmail.
                .Select(g => g.OrderByDescending(x => string.IsNullOrWhiteSpace(x.InstallerEmail) ? 0 : 1).First())
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
                bool foundBest = false;
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
                            if (IsMeterOrErtHeader(key)) serialColTemp = c;
                            if (key.Contains("INSTALLER") || key.Contains("EMAIL")) emailColTemp = c;
                        }
                        if (serialColTemp != -1)
                        {
                            if (emailColTemp != -1)
                            {
                                targetSheet = ws;
                                headerRow = r;
                                idxSerial = serialColTemp;
                                idxEmail = emailColTemp;
                                foundBest = true;
                                break;
                            }
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
                    if (string.IsNullOrWhiteSpace(serial)) continue;
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
                // If same serial appears multiple times in upload, prefer a record with non-empty InstallerEmail.
                .Select(g => g.OrderByDescending(x => string.IsNullOrWhiteSpace(x.InstallerEmail) ? 0 : 1).First())
                .ToList();
            return payload;
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
        private string ExtractEmail(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            var m = System.Text.RegularExpressions.Regex.Match(
                text,
                @"[A-Z0-9._%+\-]+@[A-Z0-9.\-]+\.[A-Z]{2,}",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
            return m.Success ? m.Value.Trim() : "";
        }
        private string NormalizeSerialValue(string v)
        {
            if (string.IsNullOrWhiteSpace(v)) return "";
            string clean = v.Replace("\uFEFF", "").Replace("\"", "").Trim().Replace(" ", "");
            if (clean.All(char.IsDigit))
                clean = clean.PadLeft(8, '0');
            return clean.ToUpperInvariant();
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