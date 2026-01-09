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
    public class ReceiveInventryController : Controller
    {
        // GET: Admin/ReceiveInventry
        [OutputCache(Duration = 0, VaryByParam = "none")]
        [SessionExpire]
        [Authorize]
        [RBAC]
        public ActionResult Index()
        {
            InventoryDAL objDAL = new InventoryDAL();
            TempData.Remove("ReceiveInventryModelList");


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
            List<WarehouseBO> data = objInventoryDAL.GetWareHouseList(ProjectId);
            List<CommonDropDownClass> data1 = objInventoryDAL.GetProductList(ProjectId);
            return base.Json(new { WarehouseList = data, ProductList = data1 }, JsonRequestBehavior.AllowGet);
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
        public ActionResult AddInventry(long warehouseId, long projectId, long productId, int quantity, int RecordType, List<string> serialNos, bool isGenerateBarcode)
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
                var warehouseName = objCompassEntities.WarehouseMasters.Where(a => a.Id == warehouseId).Select(a => a.WarehouseName).FirstOrDefault();
                productModel = obj.GetProductbyId(productId);

                if (productModel.SerialNumberRequired && RecordType == 2)
                {
                    if (serialNos != null && serialNos.Count > 0)
                    {
                        var ValidList = obj.CheckData(projectId, serialNos);

                        foreach (var serialno in serialNos)
                        {
                            bool checkRecord = false;
                            if (ValidList.Count > 0 && ValidList.Where(o => o == serialno).FirstOrDefault() != null)
                            {
                                checkRecord = true;
                            }


                            ProductMasterBO newproductModel = new ProductMasterBO();

                            #region add record in temp
                            //Check serial number in temp list
                            datavalid = true;
                            productModelList = TempData["ReceiveInventryModelList"] as List<ProductMasterBO>;
                            TempData.Keep("ReceiveInventryModelList");
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
                                newproductModel.WarehouseName = warehouseName;
                                newproductModel.SerialNumber = serialno;
                                newproductModel.Quantity = 1;
                                newproductModel.ProjectId = projectId;


                                newproductModel.RecordStatus = checkRecord == true ? "Successful" : "Already present";
                                newproductModel.ValidRecords = checkRecord == true ? 0 : 1;
                                productModelList.Add(newproductModel);
                                TempData["ReceiveInventryModelList"] = productModelList;

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
                    if (quantity > 0)
                    {
                        productModelList = TempData["ReceiveInventryModelList"] as List<ProductMasterBO>;
                        TempData.Keep("ReceiveInventryModelList");

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
                        newproductModel.WarehouseName = warehouseName;
                        newproductModel.ProjectId = projectId;
                        newproductModel.Quantity = quantity;
                        newproductModel.GenerateBarcode = isGenerateBarcode;
                        if (productModel.SerialNumberRequired && RecordType == 3)
                        {
                            newproductModel.GenerateBarcode = true;
                        }
                        newproductModel.RecordStatus = "";
                        newproductModel.ValidRecords = 0;
                        productModelList.Add(newproductModel);
                        TempData["ReceiveInventryModelList"] = productModelList;

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

                    // return PartialView("_ReceiveInventryTable", productModelList);
                    return Json(new { success, ErrorMessage, productModelList, duplicateFound, duplicateSerialNumberMsg }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                ErrorMessage = "Error in adding record";
            }

            return Json(new { success, ErrorMessage }, JsonRequestBehavior.AllowGet);

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
                productModelList = TempData["ReceiveInventryModelList"] as List<ProductMasterBO>;
                TempData.Keep("ReceiveInventryModelList");
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
                                editrecord.SerialNumber = serialNo;
                                success = true;
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
                    TempData["ReceiveInventryModelList"] = productModelList;
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
                productModelList = TempData["ReceiveInventryModelList"] as List<ProductMasterBO>;
                TempData.Keep("ReceiveInventryModelList");
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
                            productModelList = TempData["ReceiveInventryModelList"] as List<ProductMasterBO>;
                            TempData.Keep("ReceiveInventryModelList");
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
                                    TempData["ReceiveInventryModelList"] = productModelList;
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
                productModelList = TempData["ReceiveInventryModelList"] as List<ProductMasterBO>;
                TempData.Keep("ReceiveInventryModelList");
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

        public ActionResult SaveInventryList(long warehouseId)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            List<ProductMasterBO> productModelList = new List<ProductMasterBO>();
            List<ProductMasterBO> uniqeproductModelList = new List<ProductMasterBO>();
            List<ProductMasterBO> NewSerialNumberList = new List<ProductMasterBO>();
            long inwardHeaderId = 0;

            bool status = false;
            string resultMessage = "";
            try
            {
                productModelList = TempData["ReceiveInventryModelList"] as List<ProductMasterBO>;
                TempData.Keep("ReceiveInventryModelList");

                if (productModelList.Where(o => o.ValidRecords == 0).Count() > 0)
                {

                    productModelList = productModelList.Where(o => o.ValidRecords == 0).ToList();


                    long currentUserId = new UserDetails().GetUserId(User.Identity.GetUserId());

                    UploadInventryVM uploadInventryVM = new UploadInventryVM();

                    //foreach (var product in productModelList)
                    //{
                    //    if (product.SerialNumberRequired == true && product.GenerateBarcode == false)
                    //    {
                    //        var objStatus = objCompassEntities.StockDetails.AsNoTracking().
                    //            Where(o => o.Active == 1 && o.SerialNumber == product.SerialNumber && o.FKProjectId == product.ProjectId).FirstOrDefault();
                    //        if (objStatus == null)
                    //        {
                    //            uniqeproductModelList.Add(product);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        uniqeproductModelList.Add(product);
                    //    }
                    //}

                    uploadInventryVM.ProjectId = productModelList[0].ProjectId;
                    uploadInventryVM.UserId = currentUserId;
                    uploadInventryVM.WarehouseId = Convert.ToInt64(warehouseId);
                    uploadInventryVM.StrFilePath = "";
                    //uploadInventryVM.UtilityId = Convert.ToInt64(utilityId);
                    uploadInventryVM.UploadType = 2; // web

                    if (productModelList != null && productModelList.Count > 0)
                    {
                        uploadInventryVM.ProductMasterList = productModelList;

                        status = new InventoryDAL().WarehouseInsertReceiveData(productModelList[0].ProjectId, uploadInventryVM.ProductMasterList, ref NewSerialNumberList);
                        if (status)
                        {
                            uploadInventryVM.BarcodeProductList = NewSerialNumberList;
                        }
                        status = new InventoryDAL().SaveReceiveInventryInStockDetail(uploadInventryVM, out resultMessage, ref inwardHeaderId);
                        //if (status)
                        //{
                        //    //return RedirectToAction("ViewUploadRecords", "ViewUploadRecords", inwardHeaderId);
                        //}

                    }
                    else
                    {
                        resultMessage = "Record not found for save";
                        status = false;
                    }
                }
                else
                {
                    resultMessage = "Save failed. No record found for submit.";
                    status = false;
                }

            }
            catch (Exception ex)
            {

                //throw ex;
                return Json(new { status = false, ex.Message, inwardHeaderId }, JsonRequestBehavior.AllowGet);
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
            TempData.Remove("ReceiveInventryModelList");

            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}