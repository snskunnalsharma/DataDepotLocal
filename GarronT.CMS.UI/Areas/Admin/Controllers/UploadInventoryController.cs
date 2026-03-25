using GarronT.CMS.Model;
using GarronT.CMS.Model.DAL;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using GarronT.CMS.Model.BO;
using System.Web.UI.WebControls;
using KSPL.Indus.MSTS.UI.Filters;
using System.Configuration;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class UploadInventoryController : Controller
    {
        //
        // GET: /Admin/UploadInventory/
        public ActionResult Index()
        {
            InventoryDAL objDAL = new InventoryDAL();
            //var values = objDAL.GetWareHouseList();


            //values.Add(new WarehouseBO { WarehouseId = 0, WarehouseName = "" });
            //IEnumerable<SelectListItem> items = (from value in values
            //                                     select new SelectListItem()
            //                                     {
            //                                         Text = value.WarehouseName,
            //                                         Value = value.WarehouseId.ToString()
            //                                     }).OrderBy(o => o.Text).ToList();


            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "", Value = "0" });
            ViewBag.ListItem = items;

            #region Utility Dropdown
            var utilitylist = objDAL.GetUtilityTypeList();
            utilitylist.Add(new InventoryUtilityTypeBO { Id = 0, UtilityName = "" });
            IEnumerable<SelectListItem> utility = (from value in utilitylist
                                                   select new SelectListItem()
                                                   {
                                                       Text = value.UtilityName,
                                                       Value = value.Id.ToString()
                                                   }).OrderBy(o => o.Text).ToList();


            ViewBag.UtilityListItem = utility;
            #endregion



            return View();
        }


        public ActionResult ViewUpload(long Id)
        {
            ViewUploadInventryVM viewUploadInventryVM = new ViewUploadInventryVM();
            try
            {
                var records = new InventoryDAL().GetInwardHeader(Id);

                viewUploadInventryVM.ReceiveInvoiceSavedRecordList = new List<ReceiveInvoiceSavedRecords>();
                viewUploadInventryVM.RequestId = records.RequestId;
                viewUploadInventryVM.uploadType = records.uploadType;
                viewUploadInventryVM.UserName = records.UserName;
                viewUploadInventryVM.DeviceDateTime = records.DeviceDateTime;
                viewUploadInventryVM.ProjectName = records.ProjectName;
                viewUploadInventryVM.WarehouseName = records.WarehouseName;

                string _webUrl = Convert.ToString(ConfigurationSettings.AppSettings["WebURL"]);


                viewUploadInventryVM.ReceiveInvoiceSavedRecordList = new InventoryDAL().GetUploadedRecords(Id);

                foreach (var item in viewUploadInventryVM.ReceiveInvoiceSavedRecordList)
                {
                    string strFilePath = item.BarCodeFilePath;
                    if (!string.IsNullOrEmpty(strFilePath))
                    {
                        if (strFilePath.Contains("..\\..\\"))
                        {
                            strFilePath = "" + strFilePath.Replace("..\\..\\", _webUrl);
                        }

                        if (strFilePath.Contains("~"))
                        {
                            strFilePath = "" + strFilePath.Replace("~\\", _webUrl);
                        }

                        strFilePath = "" + strFilePath.Replace('\\', '/');
                        item.BarCodeFilePath = strFilePath.Trim();
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }


            return View(viewUploadInventryVM);
        }


        [HttpGet]
        public ActionResult GetPoject(int UtilityId)
        {


            InventoryDAL objInventoryDAL = new InventoryDAL();
            List<ProjectModel> data = objInventoryDAL.GetInventoryProjectName(UtilityId);

            var checkAdmin = Session["CurrentUserIsAdmin"].ToString();


            if (checkAdmin == "0")
            {
                var userId = new UserDetails().GetUserId(User.Identity.GetUserId());

                string utilityType = UtilityId == 1 ? "Electric" : (UtilityId == 2 ? "Gas" : "Water");

                var abcd = new ProjectMasterDAL().GetUserProjectListForDashBoard(utilityType, "Active", userId);


                if (abcd == null || abcd.Count == 0)
                {
                    if (data != null && data.Count() > 0)
                    {
                        data = data.Take(0).ToList();
                    }
                }
                else
                {
                    data = data.Where(o => abcd.Contains(o.ProjectId)).ToList();
                }

            }
            data = data.OrderBy(o => o.ProjectName).ToList();

            return base.Json(data, JsonRequestBehavior.AllowGet);
        }

        //GetWarehouse
        [HttpGet]
        public ActionResult GetWarehouse(long ProjectId)
        {

            InventoryDAL objInventoryDAL = new InventoryDAL();
            List<WarehouseBO> data = objInventoryDAL.GetWareHouseList(ProjectId);
            return base.Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Uploads inventory excel file
        /// </summary>
        /// <returns>Json result with status</returns>
        [HttpPost]
        public ActionResult UploadInventoryFile(string ProjectId, string WarehouseId)
        {
            try
            {
                //long _projectId = long.Parse(projectId);
                string fileName = Request.Headers["X-File-Name"];
                string fileType = Request.Headers["X-File-Type"];
                int fileSize = Convert.ToInt32(Request.Headers["X-File-Size"]);

                //File's content is available in Request.InputStream property
                System.IO.Stream fileContent = Request.InputStream;

                //Creating a FileStream to save file's content
                string inventoryFilePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["InventoryUploadPath"]);

                GrantAccess(Server.MapPath(inventoryFilePath));

                var FileNames = fileName.Split('.').ToArray();
                string newFileName = FileNames[0] + DateTime.Now.ToString("_dd_MM_yyy_hh_MM_ss") + "." + FileNames[1];



                System.IO.FileStream fileStream = System.IO.File.Create(Server.MapPath(inventoryFilePath) + newFileName);
                var filepath = inventoryFilePath + newFileName;

                fileContent.Seek(0, System.IO.SeekOrigin.Begin);

                //Copying file's content to FileStream
                fileContent.CopyTo(fileStream);

                fileStream.Dispose();


                DataTable inventoryDataTable = ValidateUploadedExcel(Convert.ToString(fileStream.Name));

                long UtilityId;
                long projectId = Convert.ToInt64(ProjectId);
                var checkColumnHeader = CheckDataTableColumns(ref inventoryDataTable, projectId);

                if (inventoryDataTable == null || inventoryDataTable.Rows.Count == 0)
                {
                    return Json(new { success = false, excelData1 = "Excel file has no data." }, JsonRequestBehavior.AllowGet);
                }
                else if (checkColumnHeader == false)
                {
                    return Json(new { success = false, excelData1 = "Please check the column header in excel file. The column header must match the sample excel file column headers." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<UploadInventoryBO> productModelList = new List<UploadInventoryBO>();
                    #region Check product exist in master

                    //long projectId = Convert.ToInt64(ProjectId);
                    using (InventoryDAL objDAL = new InventoryDAL())
                    {
                        productModelList = objDAL.CheckProductMakeTypeSizeExist(inventoryDataTable, ProjectId, WarehouseId, out UtilityId);
                     

                        // 🔥 LOCATION OVERRIDE LOGIC
                        foreach (var item in productModelList)
                        {
                            if (projectId == 10136)
                            {
                                item.Location = string.IsNullOrEmpty(item.TrayId) ? item.Location : item.TrayId;
                            }
                            else if (projectId == 10135)
                            {
                                item.Location = string.IsNullOrEmpty(item.PONumber) ? item.Location : item.PONumber;
                            }
                        }
                    }

                    #region validList
                    var validList = productModelList.Where(a => a.InventryStatus == 0).ToList();
                    #endregion

                    #region DuplicateList
                    var DuplicateList = productModelList.Where(a => a.InventryStatus == 1);
                    #endregion

                    #region InvalidList
                    var InvalidList = productModelList.Where(a => a.InventryStatus == 2);
                    #endregion


                    #endregion

                    #region Download Inverty excel with status
                    List<DownloadInventory> DownloadInventoryList = productModelList.Select(a => new DownloadInventory
                    {

                        UtilityType = a.UtilityType,
                        CategoryName = a.CategoryName,
                        ProductName = a.ProductName,
                        PartNumber = a.PartNumber,
                        UPCCode = a.UPCCode,
                        Make = a.Make,
                        Type = a.Type,
                        Size = a.Size,
                        SerialNumber = a.SerialNumber,
                        Quantity = a.Quantity,
                        Status = a.IsDuplicate,

                    }).ToList();

                    //convert list to datatable
                    DataTable data = new CommonFunctions().ToDataTable(DownloadInventoryList);
                    TempData["ExportInventryList"] = data;

                    #endregion

                    long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());

                    UploadInventryVM uploadInventryVM = new UploadInventryVM();

                    uploadInventryVM.UploadInventoryList = validList;
                    uploadInventryVM.UserId = currentUserId;
                    uploadInventryVM.WarehouseId = Convert.ToInt64(WarehouseId);
                    uploadInventryVM.ProjectId = long.Parse(ProjectId);
                    uploadInventryVM.StrFilePath = filepath;
                    uploadInventryVM.UtilityId = UtilityId;
                    uploadInventryVM.UploadType = 1; // excel

                    TempData["UploadInventryVM"] = uploadInventryVM;

                    TempData["ProductModelList"] = productModelList;
                    var jsonResult = Json(new { success = true, excelData1 = "File uploaded successfully.", varData = productModelList }, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                    //return Json(new { success = true, excelData1 = "File uploaded successfully.", varData = productModelList }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                TempData["ExcelData"] = null;
                return Json(new { success = false, excelData1 = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Aniket
        /// 20-Jul-2017
        /// To save valid uploaded inventry records
        /// </summary>
        /// <returns></returns>
        public ActionResult SaveInventryRecord()
        {
            string message = "";
            bool success = false;
            long inwardHeaderId = 0;
            try
            {


                UploadInventryVM data = TempData["UploadInventryVM"] as UploadInventryVM;
                if (data != null)
                {
                    List<UploadInventoryBO> productModelList = TempData["ProductModelList"] as List<UploadInventoryBO>;
                    TempData.Keep("ProductModelList");
                    data.UploadInventoryList = productModelList.Where(a => a.InventryStatus == 0).ToList();
                    if (data.UploadInventoryList.Count > 0)
                    {
                        success = new InventoryDAL().SaveInventryInStockDetail(data, out message, ref inwardHeaderId);
                        TempData.Remove("UploadInventryVM");
                    }
                    else
                    {
                        success = false;
                        message = "Excel upload failed as no valid data found to save. Please correct the data & try again!";
                    }
                }
                else
                {
                    success = false;
                    message = "Excel upload failed as no valid data found to save. Please correct the data & try again!";
                }


                //long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                //InwardHeaderBO data = TempData["ValidInventryList"] as InwardHeaderBO;
                //if (data != null)
                //{
                //    data.CreatedBy = currentUserId;
                //    success = new InventoryDAL().SaveInventryInStockHeader(data, out message);

                //    TempData.Remove("ValidInventryList");
                //}
            }
            catch (Exception ex)
            {
                message = "Unable to save Inventry";

            }

            return Json(new { status = success, resultMessage = message, inwardHeaderId = inwardHeaderId }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Aniket
        /// 20-Jul-2017
        /// To download Uploaded excel status excel
        /// </summary>
        public void DownloadExcelExport()
        {

            ProjectMasterDAL projectMasterDAL = new ProjectMasterDAL();
            DataTable data = TempData["ExportInventryList"] as DataTable;


            #region Export to excel
            GridView gv = new GridView();
            gv.DataSource = data;// data.OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street).ToList();
            gv.DataBind();
            Response.Clear();
            Response.Charset = "";
            Response.BufferOutput = true;
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=InventryUploadReport.xlsx");
            System.Data.DataTable dt = data;//new CommonFunctions().ToDataTable(data.ToList());

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
            // Response.ClearContent();    // Add this line
            // Response.ClearHeaders();    // Add this line
            #endregion


        }

        /// <summary>
        /// create directory with access right.
        /// </summary>
        /// <param name="_direcotryPath"></param>
        private void GrantAccess(string _direcotryPath)
        {

            try
            {
                string folderPath = _direcotryPath;
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                DirectoryInfo dInfo = new DirectoryInfo(folderPath);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();
                dSecurity.AddAccessRule(new FileSystemAccessRule("everyone", FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);
            }

            catch (Exception ex) { }

        }


        /// <summary>
        /// Returns datatable for the excel file.
        /// </summary>
        /// <param name="newFilePath"></param>
        /// <param name="List1"></param>
        /// <returns></returns>
        private DataTable ValidateUploadedExcel(string newFilePath)
        {

            try
            {

                #region getExcel data
                DataTable inventoryDataTable = new DataTable();

                FileInfo existingFile = new FileInfo(newFilePath);
                using (ExcelPackage xlPackage = new ExcelPackage(existingFile))
                {
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];

                    //adding columns  
                    int iCol = 1;
                    int iRow = 1;
                    bool CanRead = true;
                    while (CanRead)
                    {
                        if (worksheet.Cells[iRow, iCol].Value != null)
                        {
                            if (worksheet.Cells[iRow, iCol].Value != DBNull.Value && !string.IsNullOrEmpty(Convert.ToString(worksheet.Cells[iRow, iCol].Value)))
                            {
                                inventoryDataTable.Columns.Add(Convert.ToString(worksheet.Cells[iRow, iCol].Value));
                                iCol++;
                            }
                            else
                            {
                                CanRead = false;
                            }
                        }
                        else
                        {
                            CanRead = false;
                        }
                    }
                    //adding rows  
                    iRow = 2;
                    bool canRowRead = true;
                    while (canRowRead)
                    {
                        DataRow dr = inventoryDataTable.NewRow();
                        bool rowVal = true;
                        int colCount = 1;
                        while (colCount <= iCol)
                        {
                            if (worksheet.Cells[iRow, colCount].Value != DBNull.Value && !string.IsNullOrEmpty(Convert.ToString(worksheet.Cells[iRow, colCount].Value)))
                            {
                                try
                                {
                                    dr[colCount - 1] = worksheet.Cells[iRow, colCount].Value;
                                }
                                catch
                                {
                                }
                                rowVal = false;
                            }
                            colCount++;
                        }



                        if (rowVal)
                        {
                            canRowRead = false;
                        }
                        else
                        {
                            inventoryDataTable.Rows.Add(dr);
                            iRow++;
                        }

                    }
                }//using closed

                #endregion

                return inventoryDataTable;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// checks for valid column header name.
        /// </summary>
        /// <param name="objDataTable"></param>
        /// <param name="columnMessage"></param>
        /// <returns></returns>
        private bool CheckDataTableColumns(ref DataTable objDataTable, long projectId)
        {
            var columnNamList = objDataTable.Columns.Cast<DataColumn>()
                .Select(x => x.ColumnName.Trim())
                .ToList();

            // ❌ DEBUG REMOVE KAR DIYA (IMPORTANT)
            // string cols = string.Join(",", columnNamList);
            // throw new Exception(cols);

            var checkColumn = columnNamList.Where(o => o.Trim().ToLower() == "utility type").Count();
            if (checkColumn == 0)
            {
                return false;
            }

            checkColumn = columnNamList.Where(o => o.Trim().ToLower() == "category name").Count();
            if (checkColumn == 0)
            {
                return false;
            }

            checkColumn = columnNamList.Where(o => o.Trim().ToLower() == "product name").Count();
            if (checkColumn == 0)
            {
                return false;
            }

            checkColumn = columnNamList.Where(o => o.Trim().ToLower() == "part number").Count();
            if (checkColumn == 0)
            {
                return false;
            }

            checkColumn = columnNamList.Where(o => o.Trim().ToLower() == "upc code").Count();
            if (checkColumn == 0)
            {
                return false;
            }

            checkColumn = columnNamList.Where(o => o.Trim().ToLower() == "make").Count();
            if (checkColumn == 0)
            {
                return false;
            }

            checkColumn = columnNamList.Where(o => o.Trim().ToLower() == "type").Count();
            if (checkColumn == 0)
            {
                return false;
            }

            checkColumn = columnNamList.Where(o => o.Trim().ToLower() == "size").Count();
            if (checkColumn == 0)
            {
                return false;
            }

            checkColumn = columnNamList.Where(o => o.Trim().ToLower() == "serial number").Count();
            if (checkColumn == 0)
            {
                return false;
            }

            checkColumn = columnNamList.Where(o => o.Trim().ToLower() == "quantity").Count();
            if (checkColumn == 0)
            {
                return false;
            }

            // 🔥 Location check (dynamic)
            if (projectId != 10136 && projectId != 10135)
            {
                checkColumn = columnNamList.Where(o => o.Trim().ToLower() == "location").Count();
                if (checkColumn == 0)
                {
                    return false;
                }
            }

            // ✅ OPTIONAL COLUMNS (ignore if missing)
            checkColumn = columnNamList.Where(o => o.Trim().ToLower() == "inventory type").Count();

            checkColumn = columnNamList.Where(o => o.Replace(".", "").Trim().ToLower() == "po number").Count();

            checkColumn = columnNamList.Where(o => o.Trim().ToLower() == "manufacturer").Count();

            checkColumn = columnNamList.Where(o => o.Trim().ToLower() == "pallet number").Count();

            checkColumn = columnNamList.Where(o => o.Trim().ToLower() == "tray id").Count();

            return true;
        }


        /// <summary>
        /// returns current record row details from database.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetCurrentData(long recordId)
        {
            using (InventoryDAL objDAL = new InventoryDAL())
            {
                var data = objDAL.GetInvetoryCurrentRecord(recordId);
                return base.Json(new { varData = data }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// saves current row in database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult SaveCurrentData(long recordId, string meterSize, string meterType, string serialNo)
        {
            using (InventoryDAL objDAL = new InventoryDAL())
            {
                string resultMessage = string.Empty;
                var data = objDAL.SaveInvetoryCurrentRecord(recordId, meterSize, meterType, serialNo, out resultMessage);
                return base.Json(new { status = data, varData = resultMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Edit(long EditId)
        {

            string message = "";
            bool success = false;
            List<UploadInventoryBO> data = TempData["ProductModelList"] as List<UploadInventoryBO>;
            TempData.Keep("ProductModelList");
            if (data != null && data.Count > 0)
            {

                var editedRecord = data.Where(a => a.UniqueId == EditId).FirstOrDefault();
                if (editedRecord != null)
                {
                    return PartialView("_EditUploadedRecord", editedRecord);
                }


            }
            else
            {
                success = false;
                message = "Excel upload failed as no valid data found to save. Please correct the data & try again!";
            }

            return Json(new { status = success, Message = message }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult UpdateRecord(UploadInventoryBO UploadInventoryBO)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {

                UploadInventryVM uploadInventryVM = (UploadInventryVM)TempData["UploadInventryVM"];

                TempData.Keep("UploadInventryVM");

                List<UploadInventoryBO> recordList = TempData["ProductModelList"] as List<UploadInventoryBO>;
                TempData.Keep("ProductModelList");


                var validrecord = new InventoryDAL().CheckValidData(uploadInventryVM.ProjectId, UploadInventoryBO, recordList);
                if (validrecord.InventryStatus == 0)
                {

                    if (recordList != null && recordList.Count > 0)
                    {

                        var editedRecord = recordList.Where(a => a.UniqueId == UploadInventoryBO.UniqueId).FirstOrDefault();
                        if (editedRecord != null)
                        {
                            editedRecord.WarehouseId = validrecord.WarehouseId;
                            editedRecord.WarehouseName = validrecord.WarehouseName;
                            editedRecord.UtilityId = validrecord.UtilityId;
                            editedRecord.UtilityType = validrecord.UtilityType;
                            editedRecord.Make = validrecord.Make;
                            editedRecord.Type = validrecord.Type;
                            editedRecord.ProductName = validrecord.ProductName;
                            editedRecord.PartNumber = validrecord.PartNumber;
                            editedRecord.UPCCode = validrecord.UPCCode;
                            editedRecord.Size = validrecord.Size;
                            editedRecord.SerialNumber = validrecord.SerialNumber;
                            editedRecord.CategoryName = validrecord.CategoryName;
                            editedRecord.ProductId = validrecord.ProductId;
                            editedRecord.ProductDescription = validrecord.ProductDescription;
                            editedRecord.IsSerialNumber = validrecord.IsSerialNumber;
                            editedRecord.Quantity = validrecord.Quantity;
                            editedRecord.InventryStatus = validrecord.InventryStatus;  //Invalid data
                            editedRecord.IsDuplicate = validrecord.IsDuplicate;
                            statusMessage.Success = true;
                            statusMessage.Message = "Record updated successfully";

                            TempData["ProductModelList"] = recordList;
                            return Json(new { statusMessage, recordList }, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        statusMessage.Success = false;
                        statusMessage.Message = "Unable to update record";
                        return Json(new { statusMessage }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    statusMessage.Success = false;
                    statusMessage.Message = validrecord.IsDuplicate;
                    return Json(new { statusMessage }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

    }
}