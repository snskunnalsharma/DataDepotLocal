using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using KSPL.Indus.MSTS.UI.Filters;
using System.Configuration;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    public class WarehouseTransferController : Controller
    {
        // GET: Admin/ReceiveInventry
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [SessionExpire]
        [Authorize]
        [RBAC]
        public ActionResult Index()
        {
            InventoryDAL objDAL = new InventoryDAL();
            TempData.Remove("WarehouseTransferModelList");


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

            #region WarehouseList

            //var values = objDAL.GetWareHouseList();
            //values.Add(new WarehouseBO { WarehouseId = 0, WarehouseName = "" });
            //IEnumerable<SelectListItem> items = (from value in values
            //                                     select new SelectListItem()
            //                                     {
            //                                         Text = value.WarehouseName,
            //                                         Value = value.WarehouseId.ToString()
            //                                     }).OrderBy(o => o.Text).ToList();



            //ViewBag.WarehouseList = items;



            #endregion

            #region product list

            //var productList = objDAL.GetProductList();
            //productList.Add(new ProductMasterBO { Id = 0, ProductName = "" });
            //IEnumerable<SelectListItem> products = (from value in productList
            //                                        select new SelectListItem()
            //                                        {
            //                                            Text = value.ProductName,
            //                                            Value = value.Id.ToString()
            //                                        }).OrderBy(o => o.Text).ToList();



            //ViewBag.ProductList = products;

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
            catch (Exception)
            {

                throw;
            }


            return View(viewUploadInventryVM);
        }


        [HttpGet]
        public ActionResult GetProject(int UtilityId)
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
        public ActionResult GetWarehouseAndProduct(long ProjectId)
        {
            InventoryDAL objInventoryDAL = new InventoryDAL();

            // 1️⃣ Get warehouses for project
            List<WarehouseBO> warehouses = objInventoryDAL.GetWareHouseList(ProjectId);

            // 2️⃣ Get products for project
            List<CommonDropDownClass> products = objInventoryDAL.GetProductList(ProjectId);

            // 3️⃣ Ensure both lists are not null
            if (warehouses == null)
                warehouses = new List<WarehouseBO>();
            if (products == null)
                products = new List<CommonDropDownClass>();

            // 4️⃣ Return JSON
            return Json(new
            {
                WarehouseList = warehouses,
                ProductList = products
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetProductData(long productId)
        {
            bool success = false;
            string ErrorMessage = "";
            ProductMasterBO productModel = new ProductMasterBO();
            try
            {
                productModel = new InventoryDAL().GetProductbyId(productId);
                success = true;
                if (productModel == null)
                {
                    ErrorMessage = "unable to get product data.";
                    success = false;
                }
                success = true;
            }
            catch (Exception)
            {
                ErrorMessage = "unable to get product data.";
                throw;
            }

            return Json(new { success, ErrorMessage, productModel }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddInventry(long warehouseId, long toWarehouseId, long projectId, long productId, int quantity, int RecordType, List<string> serialNos, bool isGenerateBarcode)
        {

            bool datavalid = false;
            bool success = false;
            string ErrorMessage = "";
            CompassEntities objCompassEntities = new CompassEntities();

            InventoryDAL obj = new InventoryDAL();
            ProductMasterBO productModel = new ProductMasterBO();

            List<ProductMasterBO> productModelList = new List<ProductMasterBO>();

            List<string> duplicateSerialNumber = new List<string>();
            try
            {
                productModel = obj.GetProductbyId(productId);
                if (productModel == null)
                {
                    return Json(new { success = false, ErrorMessage = "Product not found." }, JsonRequestBehavior.AllowGet);
                }

                if (warehouseId == toWarehouseId)
                {
                    return Json(new { success = false, ErrorMessage = "From warehouse and To warehouse must be different." }, JsonRequestBehavior.AllowGet);
                }

                var toWarehouseName = objCompassEntities.WarehouseMasters.Where(a => a.Id == toWarehouseId).Select(a => a.WarehouseName).FirstOrDefault();

                if (productModel.SerialNumberRequired && RecordType == 3)
                {
                    return Json(new { success = false, ErrorMessage = "Generate barcode is not supported for warehouse transfer. Enter existing serial numbers from the From warehouse." }, JsonRequestBehavior.AllowGet);
                }

                if (productModel.SerialNumberRequired && RecordType == 2)
                {
                    if (serialNos != null && serialNos.Count > 0)
                    {
                        var missingInFrom = obj.GetSerialsMissingFromWarehouse(projectId, warehouseId, productId, serialNos);
                        if (missingInFrom != null && missingInFrom.Count > 0)
                        {
                            ErrorMessage = "This serial number is not in the selected From warehouse. Add it there first: " + string.Join(", ", missingInFrom);
                            return Json(new { success = false, ErrorMessage }, JsonRequestBehavior.AllowGet);
                        }

                        foreach (var serialno in serialNos)
                        {
                            ProductMasterBO newproductModel = new ProductMasterBO();

                            #region add record in temp
                            datavalid = true;
                            productModelList = TempData["WarehouseTransferModelList"] as List<ProductMasterBO>;
                            TempData.Keep("WarehouseTransferModelList");
                            if (productModelList != null && productModelList.Count > 0)
                            {
                                var serialexistintemp = productModelList.Where(a => a.SerialNumber == serialno).FirstOrDefault();
                                if (serialexistintemp != null)
                                {
                                    duplicateSerialNumber.Add(serialno);
                                    ErrorMessage = "Serial number already added in list";
                                    datavalid = false;
                                }
                            }
                            else
                            {
                                productModelList = new List<ProductMasterBO>();

                            }

                            if (datavalid)
                            {

                                newproductModel.Id = productModel.Id;
                                newproductModel.ProductName = productModel.ProductName;
                                newproductModel.ProductDescription = productModel.ProductDescription;
                                newproductModel.ProductPartNumber = productModel.ProductPartNumber;
                                newproductModel.ProductUPCCode = productModel.ProductUPCCode;
                                newproductModel.Make = productModel.Make;
                                newproductModel.Type = productModel.Type;
                                newproductModel.Size = productModel.Size;
                                newproductModel.UtilityId = productModel.UtilityId;

                                newproductModel.UtilityName = productModel.UtilityName;
                                newproductModel.SerialNumberRequired = productModel.SerialNumberRequired;
                                newproductModel.GenerateBarcode = false;
                                newproductModel.CategoryId = productModel.CategoryId;
                                newproductModel.CategoryName = productModel.CategoryName;

                                newproductModel.UniqueId = Guid.NewGuid();
                                newproductModel.WarehouseId = warehouseId;
                                newproductModel.WarehouseName = toWarehouseName;
                                newproductModel.ToWarehouseId = toWarehouseId;
                                newproductModel.SerialNumber = serialno;
                                newproductModel.Quantity = 1;
                                newproductModel.ProjectId = projectId;


                                newproductModel.RecordStatus = "Pending transfer";
                                newproductModel.ValidRecords = 0;
                                productModelList.Add(newproductModel);
                                TempData["WarehouseTransferModelList"] = productModelList;

                                success = true;
                            }

                            #endregion
                        }
                    }
                    else
                    {
                        ErrorMessage = "Serial number records not found.";
                        success = false;
                    }
                }
                else
                {
                    if (productModel.SerialNumberRequired)
                    {
                        return Json(new { success = false, ErrorMessage = "Enter serial numbers using the serial dialog (Add), or disable Generate Barcode." }, JsonRequestBehavior.AllowGet);
                    }

                    if (quantity > 0)
                    {
                        var available = obj.GetTotalAvailableBalanceNonSerial(projectId, warehouseId, productId);
                        if (available < quantity)
                        {
                            return Json(new { success = false, ErrorMessage = "Insufficient quantity in From warehouse. Available: " + available + "." }, JsonRequestBehavior.AllowGet);
                        }

                        productModelList = TempData["WarehouseTransferModelList"] as List<ProductMasterBO>;
                        TempData.Keep("WarehouseTransferModelList");

                        if (productModelList == null)
                        {
                            productModelList = new List<ProductMasterBO>();
                        }

                        ProductMasterBO newproductModel = new ProductMasterBO();
                        newproductModel.Id = productModel.Id;
                        newproductModel.ProductName = productModel.ProductName;
                        newproductModel.ProductDescription = productModel.ProductDescription;
                        newproductModel.ProductPartNumber = productModel.ProductPartNumber;
                        newproductModel.ProductUPCCode = productModel.ProductUPCCode;
                        newproductModel.Make = productModel.Make;
                        newproductModel.Type = productModel.Type;
                        newproductModel.Size = productModel.Size;
                        newproductModel.UtilityId = productModel.UtilityId;

                        newproductModel.UtilityName = productModel.UtilityName;
                        newproductModel.SerialNumberRequired = productModel.SerialNumberRequired;
                        newproductModel.CategoryId = productModel.CategoryId;
                        newproductModel.CategoryName = productModel.CategoryName;

                        newproductModel.UniqueId = Guid.NewGuid();
                        newproductModel.WarehouseId = warehouseId;
                        newproductModel.WarehouseName = toWarehouseName;
                        newproductModel.ToWarehouseId = toWarehouseId;
                        newproductModel.ProjectId = projectId;
                        newproductModel.Quantity = quantity;
                        newproductModel.GenerateBarcode = false;
                        newproductModel.RecordStatus = "Pending transfer";
                        newproductModel.ValidRecords = 0;
                        productModelList.Add(newproductModel);
                        TempData["WarehouseTransferModelList"] = productModelList;

                        success = true;
                    }
                    else
                    {
                        ErrorMessage = "please enter quantity";
                        success = false;
                    }
                }

                if (success)
                {
                    bool duplicateFound = false;
                    string duplicateSerialNumberMsg = "";
                    if (duplicateSerialNumber.Count > 0)
                    {

                        duplicateFound = true;
                        duplicateSerialNumberMsg = string.Join(",", duplicateSerialNumber.ToArray());
                    }

                    return Json(new { success, ErrorMessage, productModelList, duplicateFound, duplicateSerialNumberMsg }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                ErrorMessage = "Error in adding record";
            }

            return Json(new { success, ErrorMessage }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult GetInventoryBySearchTerm(long projectId, long warehouseId, string searchTerm)
        {
            try
            {
                InventoryDAL dal = new InventoryDAL();
                var data = dal.GetStockDetailsBySearchTerm(projectId, warehouseId, searchTerm);
                var jsonResult = Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public class AddBatchInventoryRequest
        {
            public long warehouseId { get; set; }
            public long toWarehouseId { get; set; }
            public long projectId { get; set; }
            public List<ProductMasterBO> selectedItems { get; set; }
        }

        [HttpPost]
        public JsonResult AddBatchInventry(AddBatchInventoryRequest request)
        {
            try
            {
                InventoryDAL dal = new InventoryDAL();

                List<ProductMasterBO> existingList =
                    TempData["WarehouseTransferModelList"] as List<ProductMasterBO>
                    ?? new List<ProductMasterBO>();

                bool duplicateFound = false;
                string duplicateMsg = "";

                dal.AddBatchInventryToTemp(
                    request.warehouseId,
                    request.toWarehouseId,
                    request.projectId,
                    request.selectedItems,
                    existingList,
                    out duplicateFound,
                    out duplicateMsg
                );

                TempData["WarehouseTransferModelList"] = existingList;
                TempData.Keep("WarehouseTransferModelList");

                return Json(new
                {
                    success = true,
                    message = "Selected inventory added successfully.",
                    productModelList = existingList,
                    duplicateFound = duplicateFound,
                    duplicateSerialNumberMsg = duplicateMsg
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        public ActionResult EditInventry(string InventryId, string serialNo, int quantity, bool serialNumberRequired)
        {
            List<ProductMasterBO> productModelList = new List<ProductMasterBO>();
            bool success = false;
            string ErrorMessage = "";
            CompassEntities objCompassEntities = new CompassEntities();
            ProductMasterBO productModel = new ProductMasterBO();
            try
            {
                productModelList = TempData["WarehouseTransferModelList"] as List<ProductMasterBO>;
                TempData.Keep("WarehouseTransferModelList");
                if (productModelList != null && productModelList.Count > 0)
                {
                    var editrecord = productModelList.Where(a => a.UniqueId == new Guid(InventryId)).FirstOrDefault();
                    if (editrecord == null)
                    {
                        ErrorMessage = "Record not exist for edit";
                        success = false;
                    }
                    else
                    {
                        if (serialNumberRequired && editrecord.GenerateBarcode == false)
                        {
                            var serialexistintemp = productModelList.Where(a => a.SerialNumber == serialNo && a.UniqueId != new Guid(InventryId)).FirstOrDefault();
                            if (serialexistintemp != null)
                            {
                                ErrorMessage = "Serial number already added in list";
                                success = false;
                            }
                            else
                            {
                                var dal = new InventoryDAL();
                                var missing = dal.GetSerialsMissingFromWarehouse(editrecord.ProjectId, editrecord.WarehouseId, editrecord.Id, new List<string> { serialNo });
                                if (missing != null && missing.Count > 0)
                                {
                                    ErrorMessage = "This serial number is not in the selected From warehouse. Add it there first.";
                                    success = false;
                                }
                                else
                                {
                                    editrecord.SerialNumber = serialNo;
                                    success = true;
                                }
                            }
                        }
                        else
                        {
                            if (quantity > 0)
                            {
                                editrecord.Quantity = quantity;
                                success = true;
                            }
                            else
                            {
                                ErrorMessage = "Quantity must be greater than zero";
                                success = false;

                            }

                        }



                    }
                }
                else
                {
                    productModelList = new List<ProductMasterBO>();
                }

                if (success)
                {
                    TempData["WarehouseTransferModelList"] = productModelList;
                    //return PartialView("_ReceiveInventryTable", productModelList);

                    return Json(new { success, ErrorMessage, productModelList }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = "Error in edit record";
            }

            return Json(new { success, ErrorMessage }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult EditInventryold(string InventryId, long warehouseId, long productId, int quantity, string serialNo)
        {
            List<ProductMasterBO> productModelList = new List<ProductMasterBO>();
            bool success = false;
            string ErrorMessage = "";
            CompassEntities objCompassEntities = new CompassEntities();
            ProductMasterBO productModel = new ProductMasterBO();
            try
            {
                productModelList = TempData["WarehouseTransferModelList"] as List<ProductMasterBO>;
                TempData.Keep("WarehouseTransferModelList");
                if (productModelList != null && productModelList.Count > 0)
                {
                    var editrecord = productModelList.Where(a => a.UniqueId == new Guid(InventryId)).FirstOrDefault();
                    if (editrecord == null)
                    {
                        ErrorMessage = "Record not exist for edit";
                        success = false;
                    }
                    else
                    {
                        var warehouseName = objCompassEntities.WarehouseMasters.Where(a => a.Id == warehouseId).Select(a => a.WarehouseName).FirstOrDefault();
                        productModel = new InventoryDAL().GetProductbyId(productId);


                        if (productModel.SerialNumberRequired)
                        {
                            success = true;
                            if (quantity == 0)
                            {
                                ErrorMessage = "please enter quantity";
                                success = false;
                            }
                            if (quantity != 1)
                            {
                                ErrorMessage = "please enter valid quantity";
                                success = false;
                            }
                            else if (serialNo == "" || serialNo == null)
                            {
                                ErrorMessage = "please enter serial number";
                                success = false;
                            }
                        }
                        else
                        {
                            if (quantity == 0)
                            {
                                ErrorMessage = "please enter quantity";
                                success = false;
                            }
                            else
                            {
                                success = true;
                            }
                        }
                        if (success)
                        {
                            //Check serial number in temp list
                            productModelList = TempData["WarehouseTransferModelList"] as List<ProductMasterBO>;
                            TempData.Keep("WarehouseTransferModelList");
                            if (productModelList != null && productModelList.Count > 0)
                            {
                                var serialexistintemp = productModelList.Where(a => a.SerialNumber == serialNo && a.UniqueId != new Guid(InventryId)).FirstOrDefault();
                                if (serialexistintemp != null)
                                {
                                    ErrorMessage = "Serial number already added in list";
                                    success = false;
                                }
                            }
                            else
                            {
                                productModelList = new List<ProductMasterBO>();
                            }

                            if (success)
                            {
                                //Check serial number exist in stock details
                                var serialexist = objCompassEntities.StockDetails.Where(a => a.SerialNumber == serialNo).FirstOrDefault();
                                if (serialexist != null)
                                {
                                    ErrorMessage = "Serial number already exist in stock";
                                    success = false;
                                }
                                else
                                {
                                    //productModel.UniqueId = new Guid(InventryId);
                                    //productModel.WarehouseId = warehouseId;
                                    //productModel.WarehouseName = warehouseName;
                                    //productModel.SerialNumber = serialNo;
                                    //productModel.Quantity = quantity;


                                    editrecord.Id = productModel.Id;
                                    editrecord.ProductName = productModel.ProductName;
                                    editrecord.ProductDescription = productModel.ProductDescription;
                                    editrecord.ProductPartNumber = productModel.ProductPartNumber;
                                    editrecord.ProductUPCCode = productModel.ProductUPCCode;
                                    editrecord.UtilityName = productModel.UtilityName;
                                    editrecord.CategoryName = productModel.CategoryName;
                                    editrecord.Make = productModel.Make;
                                    editrecord.Size = productModel.Size;
                                    editrecord.Type = productModel.Type;
                                    editrecord.SerialNumberRequired = productModel.SerialNumberRequired;

                                    editrecord.WarehouseId = warehouseId;
                                    editrecord.WarehouseName = warehouseName;
                                    editrecord.SerialNumber = serialNo;
                                    editrecord.Quantity = quantity;



                                    //editrecord = productModel;
                                    TempData["WarehouseTransferModelList"] = productModelList;
                                    success = true;

                                    return PartialView("_ReceiveInventryTable", productModelList);
                                }
                            }



                        }


                    }
                }
                else
                {
                    productModelList = new List<ProductMasterBO>();
                }
            }
            catch (Exception)
            {

                throw;
            }


            return Json(new { success, ErrorMessage, productModel }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteInventry(string InventryId)
        {
            List<ProductMasterBO> productModelList = new List<ProductMasterBO>();
            bool success = false;
            string ErrorMessage = "";

            ProductMasterBO productModel = new ProductMasterBO();


            try
            {
                productModelList = TempData["WarehouseTransferModelList"] as List<ProductMasterBO>;
                TempData.Keep("WarehouseTransferModelList");
                if (productModelList != null && productModelList.Count > 0)
                {
                    var editrecord = productModelList.Where(a => a.UniqueId == new Guid(InventryId)).FirstOrDefault();
                    if (editrecord == null)
                    {
                        ErrorMessage = "Record not exist for delete";
                        success = false;
                    }
                    else
                    {
                        productModelList.Remove(editrecord);
                        //return PartialView("_ReceiveInventryTable", productModelList);
                        success = true;
                        return Json(new { success, ErrorMessage, productModelList }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    ErrorMessage = "Record not exist for delete";
                    success = false;
                }
            }
            catch (Exception)
            {

                throw;
            }

            return Json(new { success, ErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveInventryList(long fromWarehouseId, long toWarehouseId)
        {
            long inwardHeaderId = 0;

            bool status = false;
            string resultMessage = "";
            try
            {
                var productModelList = TempData["WarehouseTransferModelList"] as List<ProductMasterBO>;

                if (productModelList == null || productModelList.Count == 0)
                {
                    return Json(new { status = false, resultMessage = "No records to transfer.", inwardHeaderId }, JsonRequestBehavior.AllowGet);
                }

                if (fromWarehouseId == toWarehouseId)
                {
                    return Json(new { status = false, resultMessage = "From warehouse and To warehouse must be different.", inwardHeaderId }, JsonRequestBehavior.AllowGet);
                }

                long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                long projectId = productModelList[0].ProjectId;

                var dal = new InventoryDAL();
                status = dal.ExecuteWarehouseTransfer(projectId, fromWarehouseId, toWarehouseId, productModelList, currentUserId, out resultMessage);

                if (status)
                {
                    TempData.Remove("WarehouseTransferModelList");
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, resultMessage = ex.Message, inwardHeaderId }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status, resultMessage, inwardHeaderId }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InsertSerialNumber(long Quantity)
        {
            ViewBag.Quantity = Quantity;
            return PartialView("_InsertSerialNumber");
        }
        public ActionResult ResetAllData()
        {
            TempData.Remove("WarehouseTransferModelList");

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        // Controller
        [HttpPost]
        public ActionResult SaveWarehouseTransfer(List<ProductMasterBO> transferList, long projectId)
        {
            string message = "";
            bool success = false;
            try
            {
                // Keep as long
                long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());
                InventoryDAL dal = new InventoryDAL();

                // Call DAL method updated to accept long
                success = dal.SaveWarehouseTransfer(projectId, transferList, currentUserId, out message);
            }
            catch (Exception ex)
            {
                message = "Error: " + ex.Message;
            }

            return Json(new { success, message }, JsonRequestBehavior.AllowGet);
        }
    }
}