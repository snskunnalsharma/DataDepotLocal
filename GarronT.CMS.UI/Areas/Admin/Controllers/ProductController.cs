using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using log4net;
using KSPL.Indus.MSTS.UI.Filters;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.IO;
using GarronT.CMS.UI.CustomHelpers;


namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class ProductController : Controller
    {
        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(CMS_MobileService));
        #endregion


        //
        // GET: /Admin/Product/
        public ActionResult Index()
        {

            //try
            //{
            //    DataTable dt = new DataTable();
            //    dt.Columns.Add("ProductName");
            //    dt.Columns.Add("SerailNumber");
            //    dt.Columns.Add("PartNumber");
            //    dt.Columns.Add("UPC");
            //    dt.Columns.Add("PartNumber1");
            //    dt.Columns.Add("UPC1");

            //    DataRow dr = dt.NewRow();
            //    dr["ProductName"] = "1 Inch Badger Meter";
            //    dr["SerailNumber"] = "12345678";
            //    dr["PartNumber"] = "ABCDEFG";
            //    dr["UPC"] = "ERD343434343";
            //    dr["PartNumber1"] = "Part No. : " + "ABCdefgh";
            //    dr["UPC1"] = "UPC : " + "ERD343434343";
            //    DataRow dr1 = dt.NewRow();
            //    dr1["ProductName"] = "1 Inch Badger Meter";
            //    dr1["SerailNumber"] = "1234334455";
            //    dr1["PartNumber"] = "ABCDEFG";
            //    dr1["UPC"] = "ERD343434343";
            //    dr1["PartNumber1"] = "Part No. : " + "ABCdefgh";
            //    dr1["UPC1"] = "UPC : " + "ERD343434343";
            //    dt.Rows.Add(dr);
            //    dt.Rows.Add(dr1);

            //    DataSet ds = new DataSet();
            //    ds.Tables.Add(dt);
            //    ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
            //    Microsoft.Reporting.WebForms.ReportViewer ReportViewer1 = new Microsoft.Reporting.WebForms.ReportViewer();
            //    ReportViewer1.LocalReport.DataSources.Clear();
            //    ReportViewer1.LocalReport.DataSources.Add(datasource);
            //    ReportViewer1.LocalReport.EnableHyperlinks = true;
            //    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/RDLCReport/ReportBarcode.rdlc");

            //    //byte[] Bytes = ReportViewer1.LocalReport.Render(format: "PDF", deviceInfo: "");

            //    //using (FileStream stream = new FileStream("f://1.pdf", FileMode.Create))
            //    //{
            //    //    stream.Write(Bytes, 0, Bytes.Length);
            //    //}


            //    System.IO.FileInfo fi = new System.IO.FileInfo("f://1.pdf");

            //    if (fi.Exists) fi.Delete();

            //    Warning[] warnings;

            //    string[] streamids;

            //    string mimeType, encoding, filenameExtension;

            //    //byte[] bytes = ReportViewer1.LocalReport.Render("Pdf", null, out mimeType, out encoding, out filenameExtension,

            //    //out streamids, out warnings);

            //    byte[] bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);


            //    System.IO.FileStream fs = System.IO.File.Create("f://1.pdf");

            //    fs.Write(bytes, 0, bytes.Length);

            //    fs.Close();
            //}
            //catch (Exception ex)
            //{

            //    //throw;
            //}
            return View();
        }


        //GetAllData
        [HttpGet]
        public ActionResult GetAllData(bool Status)
        {
            InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();

            long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());

            bool isAdmin = User.IsInRole("Admin");

            List<ProductMasterBO> objList = objInventoryMasterDAL.GetAllRecord(
                Status,
                currentUserId,
                isAdmin
            );

            return Json(new { objList }, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public ActionResult AddNew()
        {

            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Product", "Add");
            if (objPermissionCheck)
            {
                ProductMasterBO objProductMasterBO = new ProductMasterBO();
                objProductMasterBO.WarehouseMinimumStock = 100;
                objProductMasterBO.UserMinimumStock = 10;
                ViewBag.Status = "Create";
                return PartialView("_AddProduct", objProductMasterBO);
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }


        }




        [HttpGet]
        public ActionResult GetDropDownData(long RecordId)
        {
            InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();

            ProductMasterBO objProductMasterBO = new ProductMasterBO();


            if (RecordId > 0)
            {
                objProductMasterBO = objInventoryMasterDAL.GetRecord(RecordId);

            }

            objProductMasterBO.UtilityTypeList = objInventoryMasterDAL.GetUtilityTypeList();

            objProductMasterBO.MeterSizeList = objInventoryMasterDAL.GetMeterSizeList();
            objProductMasterBO.MeterTypeList = objInventoryMasterDAL.GetMeterTypeList();
            objProductMasterBO.MeterMakeList = objInventoryMasterDAL.GetMeterMakeList();

            return Json(new { objProductMasterBO }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddNew(ProductMasterBO objProductMasterBO)
        {
            long productId = 0;
            StatusMessage statusMessage = new StatusMessage();
            ProductMasterBO objProductMasterBO1 = new ProductMasterBO();
            try
            {
                if (ModelState.IsValid)
                {
                    InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();
                    objProductMasterBO.CurrentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    statusMessage = objInventoryMasterDAL.CreateOrUpdateProduct(objProductMasterBO, ref productId);
                    objProductMasterBO1 = objInventoryMasterDAL.GetRecord(productId);
                }
                else
                {
                    statusMessage.Success = false;
                    statusMessage.Message = "Submit failed. Product information not complete. Please fill all the information and then try.";
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                statusMessage.Message = ex.Message;
            }

            string ProductUPCCode = objProductMasterBO1 != null ? objProductMasterBO1.ProductUPCCode : "";

            return Json(new { productId, ProductUPCCode, statusMessage }, JsonRequestBehavior.AllowGet);

        }





        [HttpGet]
        public ActionResult Edit(long Id)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Product", "Edit");
            if (objPermissionCheck)
            {
                InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();
                ProductMasterBO objProductMasterBO = objInventoryMasterDAL.GetRecord(Id);

                ViewBag.Status = "Edit";
                return PartialView("_AddProduct", objProductMasterBO);
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public ActionResult Edit(ProductMasterBO objProductMasterBO)
        {
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                if (ModelState.IsValid)
                {
                    InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();
                    objProductMasterBO.CurrentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    statusMessage = objInventoryMasterDAL.CreateOrUpdate(objProductMasterBO);
                }
                else
                {
                    statusMessage.Success = false;
                    statusMessage.Message = "Submit failed. Product information not complete. Please fill all the information and then try.";
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                statusMessage.Success = false;
                statusMessage.Message = ex.Message;
            }

            return Json(new { statusMessage }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult ActiveDeactive(long Id, bool Status)
        {
            StatusMessage statusMessage = new StatusMessage();
            if (Status == false)
            {
                var objPermissionCheck = UserPermissionCheck.checkUserStatus("Product", "Delete");
                if (objPermissionCheck)
                {
                    InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();
                    long CurrentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    statusMessage = objInventoryMasterDAL.ActivateOrDeactivate(Id, Status, CurrentUserId);
                    return Json(new { statusMessage }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    statusMessage.Success = false;
                    statusMessage.Message = UserPermissionCheck.PerMessage;
                    return Json(new { statusMessage }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();
                long CurrentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                statusMessage = objInventoryMasterDAL.ActivateOrDeactivate(Id, Status, CurrentUserId);
                return Json(new { statusMessage }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GenerateBarcode(string productName, string partNumber)
        {
            StatusMessage statusMessage = new StatusMessage();
            string filepath = "";
            try
            {
                InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();
                statusMessage.Success = objInventoryMasterDAL.GeneratePartNoBarcode(productName, partNumber, ref filepath);

            }
            catch (Exception ex)
            {
                statusMessage.Success = false;
                statusMessage.Message = "Unable to generat barcode";
            }


            return Json(new { filepath, statusMessage }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GenerateBarcodeUPC(string productName, string ProductUPCCode)
        {
            StatusMessage statusMessage = new StatusMessage();
            string filepath = "";
            try
            {
                InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();
                statusMessage.Success = objInventoryMasterDAL.GenerateUpcCodeBarcode(productName, ProductUPCCode, ref filepath);

            }
            catch (Exception ex)
            {
                statusMessage.Success = false;
                statusMessage.Message = "Unable to generat barcode";
            }


            return Json(new { filepath, statusMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetManagerList()
        {

            InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();

            var managerList = objInventoryMasterDAL.GetAllUsersBasedOnRoleWithSelf("Manager");

            return Json(managerList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetUserByProductId(long productId)
        {

            InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();
            var userlist = objInventoryMasterDAL.GetuserbyproductId(productId);


            return Json(userlist, JsonRequestBehavior.AllowGet);
        }



        public ActionResult PrintBarcode(long productId)
        {
            StatusMessage statusMessage = new StatusMessage();
            string filepath = "";
            try
            {


                string WebURLFilePath = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();

                InventoryMasterDAL objInventoryMasterDAL = new InventoryMasterDAL();
                ProductMasterBO objProductMasterBO = objInventoryMasterDAL.GetRecord(productId);


                //Path on the current server
                string MainPath = @"~\TempBarcodeFiles";

                string dynamicFolderName = DateTime.Now.ToString("ddMMyyhhmmssfff");
                //InwardRequestId


                //InwardRequestId


                #region generate barcode & add record in database

                string dynamicFolderName1 = DateTime.Now.ToString("ddMMyyhhmmssfff");
                //SubInwardRequestId
                string subMainPath = MainPath + @"\" + dynamicFolderName1 + "\\BarcodeFile.pdf";
                WebURLFilePath = WebURLFilePath + @"TempBarcodeFiles\" + dynamicFolderName1;

                string filePath1 = HttpContext.Request.MapPath(subMainPath);

                InventoryMasterDAL.grantAccess(System.IO.Path.GetDirectoryName(filePath1));

                filepath = WebURLFilePath + "\\BarcodeFile.pdf";

                // var doc = new Document();

                DataTable dt = new DataTable();
                dt.Columns.Add("ProductName");
                dt.Columns.Add("SerailNumber");
                dt.Columns.Add("PartNumber");
                dt.Columns.Add("UPC");
                dt.Columns.Add("PartNumber1");
                dt.Columns.Add("UPC1");

                DataRow dr = dt.NewRow();
                dr["ProductName"] = objProductMasterBO.ProductName;
                dr["SerailNumber"] = "";
                dr["PartNumber"] = objProductMasterBO.ProductPartNumber;
                dr["UPC"] = objProductMasterBO.ProductUPCCode;
                dr["PartNumber1"] = "Part No. : " + objProductMasterBO.ProductPartNumber;
                dr["UPC1"] = "UPC : " + objProductMasterBO.ProductUPCCode;
                dt.Rows.Add(dr);
                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
                ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                Microsoft.Reporting.WebForms.ReportViewer ReportViewer1 = new Microsoft.Reporting.WebForms.ReportViewer();
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(datasource);
                ReportViewer1.LocalReport.EnableHyperlinks = true;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/RDLCReport/ReportBarcode.rdlc");




                System.IO.FileInfo fi = new System.IO.FileInfo(filePath1);

                if (fi.Exists) fi.Delete();

                Warning[] warnings;

                string[] streamids;

                string mimeType, encoding, filenameExtension;


                byte[] bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);


                System.IO.FileStream fs = System.IO.File.Create(filePath1);

                fs.Write(bytes, 0, bytes.Length);

                fs.Close();
                #endregion



            }
            catch (Exception ex)
            {
                log.Info("PrintBarcode ERROR at - " + DateTime.Now.ToString());
                log.Info("PrintBarcode ERROR  ex.Message - " + (ex.Message != null ? ex.Message : ""));

                statusMessage.Success = false;
                statusMessage.Message = "Unable to generate barcode";
            }




            return Json(new { filepath, statusMessage }, JsonRequestBehavior.AllowGet);
        }


    }
}