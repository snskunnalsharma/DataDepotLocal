using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using KSPL.Indus.MSTS.UI.Filters;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;


namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class InventoryController : Controller
    {
        //
        // GET: /Admin/Inventory/
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// 2016/09/20 
        /// Bharat Magdum
        /// Method to return the active project list 
        /// </summary>
        /// <param name="status">Active or Complete</param>
        /// <returns>Project list</returns>
        [HttpGet]
        public JsonResult GetPoject()
        {
            using (InventoryDAL objDAL = new InventoryDAL())
            {
                string proStatus = "Active";
                var data = objDAL.GetProjectName(proStatus);

                return base.Json(data, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult GetWarehouse()
        {
            using (InventoryDAL objDAL = new InventoryDAL())
            {
                string proStatus = "Active";
                var data = objDAL.GetWareHouseList();

                return base.Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetPojectCurrentData(long projectId)
        {
            using (InventoryDAL objDAL = new InventoryDAL())
            {
                var data = objDAL.GetInvetoryData(projectId);
                return base.Json(new { varData = data }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Uploads inventory excel file
        /// </summary>
        /// <returns>Json result with status</returns>
        [HttpPost]
        public ActionResult UploadInventoryFile(string Id)
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

                var FileNames = fileName.Split('.').ToArray();
                string newFileName = FileNames[0] + DateTime.Now.ToString("_dd_MM_yyy_hh_MM_ss") + "." + FileNames[1];

                GrantAccess(Server.MapPath(inventoryFilePath));

                System.IO.FileStream fileStream = System.IO.File.Create(Server.MapPath(inventoryFilePath) + newFileName);


                fileContent.Seek(0, System.IO.SeekOrigin.Begin);

                //Copying file's content to FileStream
                fileContent.CopyTo(fileStream);

                fileStream.Dispose();


                DataTable inventoryDataTable = ValidateUploadedExcel(Convert.ToString(fileStream.Name));


                var checkColumnHeader = CheckDataTableColumns(ref inventoryDataTable);

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
                    



                    List<InventoryUploadBO> objInventoryUploadList = new List<InventoryUploadBO>();
                    using (InventoryDAL objDAL = new InventoryDAL())
                    {
                        long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                        objInventoryUploadList = objDAL.SaveInvetoryExcel(inventoryDataTable, long.Parse(Id), currentUserId);
                    }
                    return Json(new { success = true, excelData1 = "File uploaded successfully.", varData = objInventoryUploadList }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                TempData["ExcelData"] = null;
                return Json(new { success = false, excelData1 = ex.Message }, JsonRequestBehavior.AllowGet);
            }

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
        private bool CheckDataTableColumns(ref DataTable objDataTable)
        {

            var columnNamList = objDataTable.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
            var checkColumn = columnNamList.Where(o => o == "Meter Type").Count();
            if (checkColumn == 0)
            {
                return false;
            }
            checkColumn = columnNamList.Where(o => o == "Meter Size").Count();
            if (checkColumn == 0)
            {
                return false;
            }
            checkColumn = columnNamList.Where(o => o == "Serial Number").Count();
            if (checkColumn == 0)
            {
                return false;
            }

            checkColumn = columnNamList.Where(o => o == "Meter Make").Count();
            if (checkColumn == 0)
            {
                return false;
            }


            checkColumn = columnNamList.Where(o => o == "Quantity").Count();
            if (checkColumn == 0)
            {
                return false;
            }


            checkColumn = columnNamList.Where(o => o == "Description").Count();
            if (checkColumn == 0)
            {
                return false;
            }


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

    }
}