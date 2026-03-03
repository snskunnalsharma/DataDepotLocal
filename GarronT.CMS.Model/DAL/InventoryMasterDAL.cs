using GarronT.CMS.Model.BO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Data.SqlClient;

namespace GarronT.CMS.Model.DAL
{
    public class InventoryMasterDAL : IDisposable
    {

        bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            //if (!disposed)
            //{
            //    if (disposing)
            //    {
            //        //dispose managed ressources

            //    }
            //}
            //dispose unmanaged ressources
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public List<ProductMasterBO> GetAllRecord(bool Status)
        {

            CompassEntities objCompassEntities = new CompassEntities();


            var objProductMasterBO = objCompassEntities.ProductMasters.AsEnumerable().Where(o =>
                       o.Active == (Status ? 1 : 0)).Select(o =>
                           new ProductMasterBO
                           {
                               Id = o.Id,
                               ProductName = o.ProductName,
                               ProductUPCCode = o.UPCCode,
                               ProductPartNumber = o.PartNumber,
                               CategoryName = o.Category.CategoryName,
                               UtilityName = objCompassEntities.UtilityMasters.Where(o1 => o1.Id == o.UtilityId).Select(o1 => o1.UtilityName).FirstOrDefault(),
                               // strConsumableProduct = o.Consumables == false ? "No" : "Yes",
                               strConsumableProduct = o.SerialNumberRequired == true ? "No" : "Yes",
                               strSerialNumberTracking = o.SerialNumberRequired == false ? "No" : "Yes",
                               Size = o.MeterSizeId != null ? o.tblMeterSize.MeterSize : "",
                               Make = o.MeterMakeId != null ? o.tblMeterMake.Make : "",
                               Type = o.MeterTypeId != null ? o.tblMeterType.MeterType : "",
                           }).ToList();



            return objProductMasterBO;
        }



        public ProductMasterBO GetRecord(long Id)
        {

            CompassEntities objCompassEntities = new CompassEntities();

            var objProductMastersBO = objCompassEntities.ProductMasters.Where(o =>
                        o.Id == Id).Select(o =>
                            new ProductMasterBO
                            {
                                Id = o.Id,
                                ProductName = o.ProductName,
                                ProductDescription = o.ProductDescription,
                                ProductUPCCode = o.UPCCode,
                                ProductPartNumber = o.PartNumber,
                                Base = o.Base,
                                CategoryId = o.Category.CategoryId,
                                // BarcodeRequired = (o.BarcodeRequired == 0 ? false : true),
                                //SelectedUtilityTypeList = o.ProductMasterFields.Where(o1 => o1.Active == 1 && o1.FieldNameId == 1).Select(o1 => o1.FKFieldMasterId).ToList(),
                                //SelectedMeterMakeList = o.ProductMasterFields.Where(o1 => o1.Active == 1 && o1.FieldNameId == 2).Select(o1 => o1.FKFieldMasterId).ToList(),
                                //SelectedMeterSizeList = o.ProductMasterFields.Where(o1 => o1.Active == 1 && o1.FieldNameId == 3).Select(o1 => o1.FKFieldMasterId).ToList(),
                                //SelectedMeterTypeList = o.ProductMasterFields.Where(o1 => o1.Active == 1 && o1.FieldNameId == 4).Select(o1 => o1.FKFieldMasterId).ToList(),
                                UtilityId = o.UtilityId,
                                MeterMakeId = o.MeterMakeId,
                                MeterTypeId = o.MeterTypeId,
                                MeterSizeId = o.MeterSizeId,
                                SerialNumberRequired = o.SerialNumberRequired,
                                GenerateBarcode = (o.GenerateBarcode == null || o.GenerateBarcode == false) ? false : true,
                                ConsumableProduct = o.Consumables,
                                SerialNumberTracking = o.SerialNumberRequired,


                                WarehouseMinimumStock = o.WarehouseMinimumStock,
                                UserMinimumStock = o.UserMinimumStock,
                                AlertTypeEmail = o.AlertTypeEmail ?? false,//(o.AlertTypeEmail == null || o.AlertTypeEmail == false) ? false : true,
                                AlertTypeSMS = o.AlertTypeSMS ?? false //(o.AlertTypeSMS == null || o.AlertTypeSMS == false) ? false : true,
                            }
                            ).FirstOrDefault();

            return objProductMastersBO;
        }


        public List<CommonDropDownClass> GetProductList()
        {

            CompassEntities objCompassEntities = new CompassEntities();

            var objProductMasterBO = objCompassEntities.ProductMasters.Where(o => o.Active == 1).Select(o =>
                           new CommonDropDownClass
                           {
                               Id = o.Id,
                               FieldValue = o.ProductName,

                           }).ToList();

            return objProductMasterBO;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<CommonDropDownClass> GetUtilityTypeList()
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<CommonDropDownClass> MeterSizeModel = CompassEntities.UtilityMasters.Where(o => o.Active == 1).
                                                      Select(o => new CommonDropDownClass
                                                      {
                                                          Id = o.Id,
                                                          FieldValue = o.UtilityName
                                                      }).OrderBy(t => t.FieldValue).ToList();

                return MeterSizeModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<CommonDropDownClass> GetMeterSizeList()
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<CommonDropDownClass> MeterSizeModel = CompassEntities.tblMeterSizes.Where(o => o.Active == 1).
                                                      Select(o => new CommonDropDownClass
                                                      {
                                                          Id = o.ID,
                                                          FieldValue = o.MeterSize
                                                      }).OrderBy(t => t.FieldValue).ToList();

                return MeterSizeModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<CommonDropDownClass> GetMeterTypeList()
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<CommonDropDownClass> MeterSizeModel = CompassEntities.tblMeterTypes.Where(o => o.Active == 1).
                                                      Select(o => new CommonDropDownClass
                                                      {
                                                          Id = o.ID,
                                                          FieldValue = o.MeterType
                                                      }).OrderBy(t => t.FieldValue).ToList();

                return MeterSizeModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<CommonDropDownClass> GetMeterMakeList()
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<CommonDropDownClass> MeterSizeModel = CompassEntities.tblMeterMakes.Where(o => o.Active == 1).
                                                      Select(o => new CommonDropDownClass
                                                      {
                                                          Id = o.ID,
                                                          FieldValue = o.Make
                                                      }).OrderBy(t => t.FieldValue).ToList();

                return MeterSizeModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public StatusMessage CreateOrUpdateProduct(ProductMasterBO ObjectToCreateOrUpdate, ref long productId)
        {
            productId = 0;
            StatusMessage objStatusMessage = new StatusMessage();
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();

                if (ObjectToCreateOrUpdate.Id == 0)
                {
                    #region Insert

                    var CheckDuplicate = objCompassEntities.ProductMasters.Where(o => o.ProductName.ToLower() == ObjectToCreateOrUpdate.ProductName.ToLower()
                        && o.UtilityId == ObjectToCreateOrUpdate.UtilityId
                        && o.MeterMakeId == ObjectToCreateOrUpdate.MeterMakeId
                        && o.MeterTypeId == ObjectToCreateOrUpdate.MeterTypeId
                        && o.MeterSizeId == ObjectToCreateOrUpdate.MeterSizeId).FirstOrDefault();
                    if (CheckDuplicate != null)
                    {
                        objStatusMessage.Success = false;
                        if (CheckDuplicate.Active == 0)
                        {
                            objStatusMessage.Message = "Insert failed. Product name already in deactive records.";
                        }
                        else
                        {
                            objStatusMessage.Message = "Insert failed. Product name already in active records.";
                        }
                    }
                    else
                    {
                        ProductMaster obj = new ProductMaster();
                        obj.ProductName = ObjectToCreateOrUpdate.ProductName;
                        obj.ProductDescription = ObjectToCreateOrUpdate.ProductDescription;
                        //obj.BarcodeRequired = ObjectToCreateOrUpdate.BarcodeRequired == true ? 1 : 0;
                        obj.PartNumber = ObjectToCreateOrUpdate.ProductPartNumber;
                        obj.Base = ObjectToCreateOrUpdate.Base;
                        if (string.IsNullOrEmpty(ObjectToCreateOrUpdate.ProductUPCCode))
                        {
                            var tempId = DateTime.Now.ToString("yyyyMMddhhmms");
                            if (tempId.Length == 14)
                            {
                                tempId = tempId.Substring(0, 13);
                            }
                            obj.UPCCode = tempId;

                        }
                        else
                        {

                            obj.UPCCode = ObjectToCreateOrUpdate.ProductUPCCode;
                        }

                        // obj.SerialNumberRequired = ObjectToCreateOrUpdate.SerialNumberTracking;
                        obj.SerialNumberRequired = ObjectToCreateOrUpdate.SerialNumberRequired;
                        obj.GenerateBarcode = ObjectToCreateOrUpdate.GenerateBarcode;
                        //  obj.Consumables = ObjectToCreateOrUpdate.ConsumableProduct;
                        obj.ProductCategoryId = ObjectToCreateOrUpdate.CategoryId;

                        //Addedby Aniket on 10-Aug-2017
                        obj.WarehouseMinimumStock = ObjectToCreateOrUpdate.WarehouseMinimumStock;
                        obj.UserMinimumStock = ObjectToCreateOrUpdate.UserMinimumStock;
                        obj.AlertTypeEmail = ObjectToCreateOrUpdate.AlertTypeEmail;
                        obj.AlertTypeSMS = ObjectToCreateOrUpdate.AlertTypeSMS;

                        obj.Active = 1;
                        obj.CreatedBy = ObjectToCreateOrUpdate.CurrentUserId;
                        obj.CreatedOn = DateTime.Now;
                        obj.UtilityId = ObjectToCreateOrUpdate.UtilityId;
                        obj.MeterMakeId = ObjectToCreateOrUpdate.MeterMakeId;
                        obj.MeterTypeId = ObjectToCreateOrUpdate.MeterTypeId;
                        obj.MeterSizeId = ObjectToCreateOrUpdate.MeterSizeId;



                        objCompassEntities.ProductMasters.Add(obj);
                        objCompassEntities.SaveChanges();

                        productId = obj.Id;

                        if (ObjectToCreateOrUpdate.WarehouseMinimumStock != null && ObjectToCreateOrUpdate.WarehouseMinimumStock > 0 && ObjectToCreateOrUpdate.ManagerList != null && ObjectToCreateOrUpdate.ManagerList.Count > 0)
                        {
                            foreach (var item in ObjectToCreateOrUpdate.ManagerList)
                            {
                                LowInventoryAlert lowInventoryAlert = new LowInventoryAlert();
                                lowInventoryAlert.FKProductId = obj.Id;
                                lowInventoryAlert.FKUserId = item;
                                lowInventoryAlert.Active = 1;
                                lowInventoryAlert.CreatedBy = ObjectToCreateOrUpdate.CurrentUserId;
                                lowInventoryAlert.CreatedOn = new CommonFunctions().ServerDate();
                                objCompassEntities.LowInventoryAlerts.Add(lowInventoryAlert);
                                objCompassEntities.SaveChanges();
                            }
                        }


                        objStatusMessage.Success = true;
                        objStatusMessage.Message = "Record created successfully.";
                    }
                    #endregion

                }
                else
                {
                    #region Update


                    var CheckDuplicate = objCompassEntities.ProductMasters.Where(o => o.ProductName.ToLower() == ObjectToCreateOrUpdate.ProductName.ToLower()
                        && o.UtilityId == ObjectToCreateOrUpdate.UtilityId
                        && o.MeterMakeId == ObjectToCreateOrUpdate.MeterMakeId
                        && o.MeterTypeId == ObjectToCreateOrUpdate.MeterTypeId
                        && o.MeterSizeId == ObjectToCreateOrUpdate.MeterSizeId
                        && o.Id != ObjectToCreateOrUpdate.Id).FirstOrDefault();
                    if (CheckDuplicate != null)
                    {
                        objStatusMessage.Success = false;
                        if (CheckDuplicate.Active == 0)
                        {
                            objStatusMessage.Message = "Insert failed. Product name already in deactive records.";
                        }
                        else
                        {
                            objStatusMessage.Message = "Insert failed. Product name already in active records.";
                        }
                    }
                    else
                    {
                        var obj = objCompassEntities.ProductMasters.Where(o => o.Id == ObjectToCreateOrUpdate.Id).FirstOrDefault();
                        obj.ProductName = ObjectToCreateOrUpdate.ProductName;
                        obj.ProductDescription = ObjectToCreateOrUpdate.ProductDescription;
                        //obj.BarcodeRequired = ObjectToCreateOrUpdate.BarcodeRequired == true ? 1 : 0;
                        obj.PartNumber = ObjectToCreateOrUpdate.ProductPartNumber;
                        obj.UPCCode = ObjectToCreateOrUpdate.ProductUPCCode;
                        obj.Base = ObjectToCreateOrUpdate.Base;
                        //obj.SerialNumberRequired = ObjectToCreateOrUpdate.SerialNumberTracking;
                        obj.SerialNumberRequired = ObjectToCreateOrUpdate.SerialNumberRequired;
                        obj.GenerateBarcode = ObjectToCreateOrUpdate.GenerateBarcode;

                        //obj.Consumables = ObjectToCreateOrUpdate.ConsumableProduct;
                        obj.ProductCategoryId = ObjectToCreateOrUpdate.CategoryId;

                        obj.Active = 1;
                        obj.ModifiedBy = ObjectToCreateOrUpdate.CurrentUserId;
                        obj.ModifiedOn = DateTime.Now;

                        obj.UtilityId = ObjectToCreateOrUpdate.UtilityId;
                        obj.MeterMakeId = ObjectToCreateOrUpdate.MeterMakeId;
                        obj.MeterTypeId = ObjectToCreateOrUpdate.MeterTypeId;
                        obj.MeterSizeId = ObjectToCreateOrUpdate.MeterSizeId;

                        //Addedby Aniket on 10-Aug-2017
                        obj.WarehouseMinimumStock = ObjectToCreateOrUpdate.WarehouseMinimumStock;
                        obj.UserMinimumStock = ObjectToCreateOrUpdate.UserMinimumStock;
                        obj.AlertTypeEmail = ObjectToCreateOrUpdate.AlertTypeEmail;
                        obj.AlertTypeSMS = ObjectToCreateOrUpdate.AlertTypeSMS;


                        objCompassEntities.SaveChanges();


                        objCompassEntities.LowInventoryAlerts
                        .Where(x => x.Id == obj.Id)
                        .ToList()
                        .ForEach(a => a.Active = 0);


                        if (ObjectToCreateOrUpdate.WarehouseMinimumStock != null && ObjectToCreateOrUpdate.WarehouseMinimumStock > 0 && ObjectToCreateOrUpdate.ManagerList != null && ObjectToCreateOrUpdate.ManagerList.Count > 0)
                        {
                            foreach (var item in ObjectToCreateOrUpdate.ManagerList)
                            {
                                var recordexist = objCompassEntities.LowInventoryAlerts.Where(a => a.FKProductId == obj.Id && a.FKUserId == item).FirstOrDefault();
                                if (recordexist != null)
                                {
                                    recordexist.Active = 1;
                                    recordexist.ModifiedBy = ObjectToCreateOrUpdate.CurrentUserId;
                                    recordexist.ModifiedOn = new CommonFunctions().ServerDate();
                                }
                                else
                                {
                                    LowInventoryAlert lowInventoryAlert = new LowInventoryAlert();
                                    lowInventoryAlert.FKProductId = obj.Id;
                                    lowInventoryAlert.FKUserId = item;
                                    lowInventoryAlert.Active = 1;
                                    lowInventoryAlert.CreatedBy = ObjectToCreateOrUpdate.CurrentUserId;
                                    lowInventoryAlert.CreatedOn = new CommonFunctions().ServerDate();
                                    objCompassEntities.LowInventoryAlerts.Add(lowInventoryAlert);
                                }

                            }
                        }
                        objCompassEntities.SaveChanges();


                        objStatusMessage.Success = true;
                        objStatusMessage.Message = "Record updated successfully.";
                    }

                    #endregion
                }


            }
            catch (Exception)
            {

                throw;
            }

            return objStatusMessage;

        }

        public StatusMessage CreateOrUpdate(ProductMasterBO ObjectToCreateOrUpdate)
        {
            StatusMessage objStatusMessage = new StatusMessage();
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();

                if (ObjectToCreateOrUpdate.Id == 0)
                {
                    #region Insert

                    var CheckDuplicate = objCompassEntities.ProductMasters.Where(o => o.ProductName.ToLower() == ObjectToCreateOrUpdate.ProductName.ToLower()
                        && o.UtilityId == ObjectToCreateOrUpdate.UtilityId
                        && o.MeterMakeId == ObjectToCreateOrUpdate.MeterMakeId
                        && o.MeterTypeId == ObjectToCreateOrUpdate.MeterTypeId
                        && o.MeterSizeId == ObjectToCreateOrUpdate.MeterSizeId).FirstOrDefault();
                    if (CheckDuplicate != null)
                    {
                        objStatusMessage.Success = false;
                        if (CheckDuplicate.Active == 0)
                        {
                            objStatusMessage.Message = "Insert failed. Product name already in deactive records.";
                        }
                        else
                        {
                            objStatusMessage.Message = "Insert failed. Product name already in active records.";
                        }
                    }
                    else
                    {
                        ProductMaster obj = new ProductMaster();
                        obj.ProductName = ObjectToCreateOrUpdate.ProductName;
                        obj.ProductDescription = ObjectToCreateOrUpdate.ProductDescription;
                        //obj.BarcodeRequired = ObjectToCreateOrUpdate.BarcodeRequired == true ? 1 : 0;
                        obj.PartNumber = ObjectToCreateOrUpdate.ProductPartNumber;
                        obj.Base = ObjectToCreateOrUpdate.Base;
                        obj.UPCCode = ObjectToCreateOrUpdate.ProductUPCCode;
                        // obj.SerialNumberRequired = ObjectToCreateOrUpdate.SerialNumberTracking;
                        obj.SerialNumberRequired = ObjectToCreateOrUpdate.SerialNumberRequired;
                        obj.GenerateBarcode = ObjectToCreateOrUpdate.GenerateBarcode;
                        obj.Consumables = ObjectToCreateOrUpdate.ConsumableProduct;
                        obj.ProductCategoryId = ObjectToCreateOrUpdate.CategoryId;
                        obj.Active = 1;
                        obj.CreatedBy = ObjectToCreateOrUpdate.CurrentUserId;
                        obj.CreatedOn = DateTime.Now;
                        obj.UtilityId = ObjectToCreateOrUpdate.UtilityId;
                        obj.MeterMakeId = ObjectToCreateOrUpdate.MeterMakeId;
                        obj.MeterTypeId = ObjectToCreateOrUpdate.MeterTypeId;
                        obj.MeterSizeId = ObjectToCreateOrUpdate.MeterSizeId;

                        //Addedby Aniket on 10-Aug-2017
                        obj.WarehouseMinimumStock = ObjectToCreateOrUpdate.WarehouseMinimumStock;
                        obj.UserMinimumStock = ObjectToCreateOrUpdate.UserMinimumStock;
                        obj.AlertTypeEmail = ObjectToCreateOrUpdate.AlertTypeEmail;
                        obj.AlertTypeSMS = ObjectToCreateOrUpdate.AlertTypeSMS;

                        //if (ObjectToCreateOrUpdate.SelectedUtilityTypeList != null && ObjectToCreateOrUpdate.SelectedUtilityTypeList.Count > 0)
                        //{
                        //    foreach (var item in ObjectToCreateOrUpdate.SelectedUtilityTypeList)
                        //    {
                        //        ProductMasterField objProductMasterField = new ProductMasterField();
                        //        objProductMasterField.FieldNameId = 1;
                        //        objProductMasterField.FKProductMasterId = obj.Id;
                        //        objProductMasterField.FKFieldMasterId = item;
                        //        objProductMasterField.Active = 1;
                        //        objProductMasterField.CreatedBy = ObjectToCreateOrUpdate.CurrentUserId;
                        //        objProductMasterField.CreatedOn = DateTime.Now;
                        //        objCompassEntities.ProductMasterFields.Add(objProductMasterField);
                        //    }

                        //}

                        //if (ObjectToCreateOrUpdate.SelectedMeterMakeList != null && ObjectToCreateOrUpdate.SelectedMeterMakeList.Count > 0)
                        //{
                        //    foreach (var item in ObjectToCreateOrUpdate.SelectedMeterMakeList)
                        //    {
                        //        ProductMasterField objProductMasterField = new ProductMasterField();
                        //        objProductMasterField.FieldNameId = 2;
                        //        objProductMasterField.FKProductMasterId = obj.Id;
                        //        objProductMasterField.FKFieldMasterId = item;
                        //        objProductMasterField.Active = 1;
                        //        objProductMasterField.CreatedBy = ObjectToCreateOrUpdate.CurrentUserId;
                        //        objProductMasterField.CreatedOn = DateTime.Now;
                        //        objCompassEntities.ProductMasterFields.Add(objProductMasterField);
                        //    }

                        //}


                        //if (ObjectToCreateOrUpdate.SelectedMeterSizeList != null && ObjectToCreateOrUpdate.SelectedMeterSizeList.Count > 0)
                        //{
                        //    foreach (var item in ObjectToCreateOrUpdate.SelectedMeterSizeList)
                        //    {
                        //        ProductMasterField objProductMasterField = new ProductMasterField();
                        //        objProductMasterField.FieldNameId = 3;
                        //        objProductMasterField.FKProductMasterId = obj.Id;
                        //        objProductMasterField.FKFieldMasterId = item;
                        //        objProductMasterField.Active = 1;
                        //        objProductMasterField.CreatedBy = ObjectToCreateOrUpdate.CurrentUserId;
                        //        objProductMasterField.CreatedOn = DateTime.Now;
                        //        objCompassEntities.ProductMasterFields.Add(objProductMasterField);
                        //    }

                        //}


                        //if (ObjectToCreateOrUpdate.SelectedMeterTypeList != null && ObjectToCreateOrUpdate.SelectedMeterTypeList.Count > 0)
                        //{
                        //    foreach (var item in ObjectToCreateOrUpdate.SelectedMeterTypeList)
                        //    {
                        //        ProductMasterField objProductMasterField = new ProductMasterField();
                        //        objProductMasterField.FieldNameId = 4;
                        //        objProductMasterField.FKProductMasterId = obj.Id;
                        //        objProductMasterField.FKFieldMasterId = item;
                        //        objProductMasterField.Active = 1;
                        //        objProductMasterField.CreatedBy = ObjectToCreateOrUpdate.CurrentUserId;
                        //        objProductMasterField.CreatedOn = DateTime.Now;
                        //        objCompassEntities.ProductMasterFields.Add(objProductMasterField);
                        //    }

                        //}


                        objCompassEntities.ProductMasters.Add(obj);
                        objCompassEntities.SaveChanges();
                        objStatusMessage.Success = true;
                        objStatusMessage.Message = "Record created successfully.";
                    }
                    #endregion

                }
                else
                {
                    #region Update


                    var CheckDuplicate = objCompassEntities.ProductMasters.Where(o => o.ProductName.ToLower() == ObjectToCreateOrUpdate.ProductName.ToLower()
                        && o.UtilityId == ObjectToCreateOrUpdate.UtilityId
                        && o.MeterMakeId == ObjectToCreateOrUpdate.MeterMakeId
                        && o.MeterTypeId == ObjectToCreateOrUpdate.MeterTypeId
                        && o.MeterSizeId == ObjectToCreateOrUpdate.MeterSizeId
                        && o.Id != ObjectToCreateOrUpdate.Id).FirstOrDefault();
                    if (CheckDuplicate != null)
                    {
                        objStatusMessage.Success = false;
                        if (CheckDuplicate.Active == 0)
                        {
                            objStatusMessage.Message = "Insert failed. Product name already in deactive records.";
                        }
                        else
                        {
                            objStatusMessage.Message = "Insert failed. Product name already in active records.";
                        }
                    }
                    else
                    {

                        var obj = objCompassEntities.ProductMasters.Where(o => o.Id == ObjectToCreateOrUpdate.Id).FirstOrDefault();


                        bool CheckIsSerialNumberFlag = false;

                        if (obj.SerialNumberRequired != ObjectToCreateOrUpdate.SerialNumberRequired)
                        {
                            var CheckInventoryPresent = objCompassEntities.StockDetails.Where(o => o.Active == 1  && o.FKProductId==obj.Id).FirstOrDefault();

                            if (CheckInventoryPresent != null)
                            {
                                CheckIsSerialNumberFlag = true;
                            }
                        }


                        if (CheckIsSerialNumberFlag)
                        {
                            objStatusMessage.Success = false;
                            objStatusMessage.Message = "Update failed. Product added in stock so serial number status can not be changed.";
                        }
                        else
                        {

                            obj.ProductName = ObjectToCreateOrUpdate.ProductName;
                            obj.ProductDescription = ObjectToCreateOrUpdate.ProductDescription;
                            //obj.BarcodeRequired = ObjectToCreateOrUpdate.BarcodeRequired == true ? 1 : 0;
                            obj.PartNumber = ObjectToCreateOrUpdate.ProductPartNumber;
                            obj.Base = ObjectToCreateOrUpdate.Base;
                            obj.UPCCode = ObjectToCreateOrUpdate.ProductUPCCode;
                            //obj.SerialNumberRequired = ObjectToCreateOrUpdate.SerialNumberTracking;
                            obj.SerialNumberRequired = ObjectToCreateOrUpdate.SerialNumberRequired;
                            obj.GenerateBarcode = ObjectToCreateOrUpdate.GenerateBarcode;

                            obj.Consumables = ObjectToCreateOrUpdate.ConsumableProduct;
                            obj.ProductCategoryId = ObjectToCreateOrUpdate.CategoryId;

                            obj.Active = 1;
                            obj.ModifiedBy = ObjectToCreateOrUpdate.CurrentUserId;
                            obj.ModifiedOn = DateTime.Now;

                            obj.UtilityId = ObjectToCreateOrUpdate.UtilityId;
                            obj.MeterMakeId = ObjectToCreateOrUpdate.MeterMakeId;
                            obj.MeterTypeId = ObjectToCreateOrUpdate.MeterTypeId;
                            obj.MeterSizeId = ObjectToCreateOrUpdate.MeterSizeId;

                            //Addedby Aniket on 10-Aug-2017
                            obj.WarehouseMinimumStock = ObjectToCreateOrUpdate.WarehouseMinimumStock;
                            obj.UserMinimumStock = ObjectToCreateOrUpdate.UserMinimumStock;
                            obj.AlertTypeEmail = ObjectToCreateOrUpdate.AlertTypeEmail;
                            obj.AlertTypeSMS = ObjectToCreateOrUpdate.AlertTypeSMS;

                            //foreach (var objUtilityType in obj.ProductMasterFields.Where(o => o.Active == 1 && o.FieldNameId == 1))
                            //{
                            //    objUtilityType.Active = 0;
                            //    objUtilityType.ModifiedBy = ObjectToCreateOrUpdate.CurrentUserId;
                            //    objUtilityType.ModifiedOn = DateTime.Now;
                            //}



                            //if (ObjectToCreateOrUpdate.SelectedUtilityTypeList != null && ObjectToCreateOrUpdate.SelectedUtilityTypeList.Count > 0)
                            //{
                            //    foreach (var item in ObjectToCreateOrUpdate.SelectedUtilityTypeList)
                            //    {
                            //        if (obj.ProductMasterFields.Count(c => c.FieldNameId == 1 && c.FKFieldMasterId == item) > 0)
                            //        {
                            //            obj.ProductMasterFields.FirstOrDefault(c => c.FieldNameId == 1 && c.FKFieldMasterId == item).Active = 1;
                            //        }
                            //        else
                            //        {
                            //            ProductMasterField objProductMasterField = new ProductMasterField();
                            //            objProductMasterField.FieldNameId = 1;
                            //            objProductMasterField.FKProductMasterId = obj.Id;
                            //            objProductMasterField.FKFieldMasterId = item;
                            //            objProductMasterField.Active = 1;
                            //            objProductMasterField.CreatedBy = ObjectToCreateOrUpdate.CurrentUserId;
                            //            objProductMasterField.CreatedOn = DateTime.Now;
                            //            objCompassEntities.ProductMasterFields.Add(objProductMasterField);
                            //        }
                            //    }

                            //}



                            //foreach (var objMeterMake in obj.ProductMasterFields.Where(o => o.Active == 1 && o.FieldNameId == 2))
                            //{
                            //    objMeterMake.Active = 0;
                            //    objMeterMake.ModifiedBy = ObjectToCreateOrUpdate.CurrentUserId;
                            //    objMeterMake.ModifiedOn = DateTime.Now;
                            //}



                            //if (ObjectToCreateOrUpdate.SelectedMeterMakeList != null && ObjectToCreateOrUpdate.SelectedMeterMakeList.Count > 0)
                            //{
                            //    foreach (var item in ObjectToCreateOrUpdate.SelectedMeterMakeList)
                            //    {
                            //        if (obj.ProductMasterFields.Count(c => c.FieldNameId == 2 && c.FKFieldMasterId == item) > 0)
                            //        {
                            //            obj.ProductMasterFields.FirstOrDefault(c => c.FieldNameId == 2 && c.FKFieldMasterId == item).Active = 1;
                            //        }
                            //        else
                            //        {
                            //            ProductMasterField objProductMasterField = new ProductMasterField();
                            //            objProductMasterField.FieldNameId = 2;
                            //            objProductMasterField.FKProductMasterId = obj.Id;
                            //            objProductMasterField.FKFieldMasterId = item;
                            //            objProductMasterField.Active = 1;
                            //            objProductMasterField.CreatedBy = ObjectToCreateOrUpdate.CurrentUserId;
                            //            objProductMasterField.CreatedOn = DateTime.Now;
                            //            objCompassEntities.ProductMasterFields.Add(objProductMasterField);
                            //        }
                            //    }

                            //}

                            //foreach (var objMeterSize in obj.ProductMasterFields.Where(o => o.Active == 1 && o.FieldNameId == 3))
                            //{
                            //    objMeterSize.Active = 0;
                            //    objMeterSize.ModifiedBy = ObjectToCreateOrUpdate.CurrentUserId;
                            //    objMeterSize.ModifiedOn = DateTime.Now;
                            //}


                            //if (ObjectToCreateOrUpdate.SelectedMeterSizeList != null && ObjectToCreateOrUpdate.SelectedMeterSizeList.Count > 0)
                            //{
                            //    foreach (var item in ObjectToCreateOrUpdate.SelectedMeterSizeList)
                            //    {
                            //        if (obj.ProductMasterFields.Count(c => c.FieldNameId == 3 && c.FKFieldMasterId == item) > 0)
                            //        {
                            //            obj.ProductMasterFields.FirstOrDefault(c => c.FieldNameId == 3 && c.FKFieldMasterId == item).Active = 1;
                            //        }
                            //        else
                            //        {

                            //            ProductMasterField objProductMasterField = new ProductMasterField();
                            //            objProductMasterField.FieldNameId = 3;
                            //            objProductMasterField.FKProductMasterId = obj.Id;
                            //            objProductMasterField.FKFieldMasterId = item;
                            //            objProductMasterField.Active = 1;
                            //            objProductMasterField.CreatedBy = ObjectToCreateOrUpdate.CurrentUserId;
                            //            objProductMasterField.CreatedOn = DateTime.Now;
                            //            objCompassEntities.ProductMasterFields.Add(objProductMasterField);
                            //        }
                            //    }

                            //}

                            //foreach (var objMeterType in obj.ProductMasterFields.Where(o => o.Active == 1 && o.FieldNameId == 4))
                            //{
                            //    objMeterType.Active = 0;
                            //    objMeterType.ModifiedBy = ObjectToCreateOrUpdate.CurrentUserId;
                            //    objMeterType.ModifiedOn = DateTime.Now;
                            //}

                            //if (ObjectToCreateOrUpdate.SelectedMeterTypeList != null && ObjectToCreateOrUpdate.SelectedMeterTypeList.Count > 0)
                            //{
                            //    foreach (var item in ObjectToCreateOrUpdate.SelectedMeterTypeList)
                            //    {
                            //        if (obj.ProductMasterFields.Count(c => c.FieldNameId == 4 && c.FKFieldMasterId == item) > 0)
                            //        {
                            //            obj.ProductMasterFields.FirstOrDefault(c => c.FieldNameId == 4 && c.FKFieldMasterId == item).Active = 1;
                            //        }
                            //        else
                            //        {
                            //            ProductMasterField objProductMasterField = new ProductMasterField();
                            //            objProductMasterField.FieldNameId = 4;
                            //            objProductMasterField.FKProductMasterId = obj.Id;
                            //            objProductMasterField.FKFieldMasterId = item;
                            //            objProductMasterField.Active = 1;
                            //            objProductMasterField.CreatedBy = ObjectToCreateOrUpdate.CurrentUserId;
                            //            objProductMasterField.CreatedOn = DateTime.Now;
                            //            objCompassEntities.ProductMasterFields.Add(objProductMasterField);
                            //        }
                            //    }

                            //}


                            objCompassEntities.LowInventoryAlerts
                           .Where(x => x.FKProductId == obj.Id && x.Active == 1)
                           .ToList()
                           .ForEach(a => a.Active = 0);
                            objCompassEntities.SaveChanges();

                            if (ObjectToCreateOrUpdate.WarehouseMinimumStock != null && ObjectToCreateOrUpdate.WarehouseMinimumStock > 0 && ObjectToCreateOrUpdate.ManagerList != null && ObjectToCreateOrUpdate.ManagerList.Count > 0)
                            {
                                foreach (var item in ObjectToCreateOrUpdate.ManagerList)
                                {
                                    var recordexist = objCompassEntities.LowInventoryAlerts.Where(a => a.FKProductId == obj.Id && a.FKUserId == item).FirstOrDefault();
                                    if (recordexist != null)
                                    {
                                        recordexist.Active = 1;
                                        recordexist.ModifiedBy = ObjectToCreateOrUpdate.CurrentUserId;
                                        recordexist.ModifiedOn = new CommonFunctions().ServerDate();
                                    }
                                    else
                                    {
                                        LowInventoryAlert lowInventoryAlert = new LowInventoryAlert();
                                        lowInventoryAlert.FKProductId = obj.Id;
                                        lowInventoryAlert.FKUserId = item;
                                        lowInventoryAlert.Active = 1;
                                        lowInventoryAlert.CreatedBy = ObjectToCreateOrUpdate.CurrentUserId;
                                        lowInventoryAlert.CreatedOn = new CommonFunctions().ServerDate();
                                        objCompassEntities.LowInventoryAlerts.Add(lowInventoryAlert);
                                    }
                                    objCompassEntities.SaveChanges();
                                }
                            }





                            objCompassEntities.SaveChanges();

                            objStatusMessage.Success = true;
                            objStatusMessage.Message = "Record updated successfully.";
                        }
                    }

                    #endregion
                }


            }
            catch (Exception)
            {

                throw;
            }

            return objStatusMessage;

        }

        public StatusMessage ActivateOrDeactivate(long Id, bool Status, long CurrentUserId)
        {
            StatusMessage objStatusMessage = new StatusMessage();
            try
            {

                CompassEntities objCompassEntities = new CompassEntities();
                var ObjMenu = objCompassEntities.ProductMasters.Where(o => o.Id == Id).FirstOrDefault();

                ObjMenu.Active = Status == true ? 1 : 0;
                ObjMenu.ModifiedBy = CurrentUserId;
                ObjMenu.ModifiedOn = DateTime.Now;

                objCompassEntities.SaveChanges();
                objStatusMessage.Success = true;
                objStatusMessage.Message = Status == true ? "Record activated successfully." : "Record deactivated successfully.";

            }
            catch (Exception)
            {
                throw;
            }

            return objStatusMessage;
        }



        public bool GeneratePartNoBarcode(string productName, string partNumber, ref string FilePath)
        {
            bool result = false;

            try
            {
                CompassEntities objCompassEntities = new CompassEntities();

                string WebURLFilePath = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();

                if (!string.IsNullOrEmpty(productName) && !string.IsNullOrEmpty(partNumber))
                {
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

                    string filePath = HttpContext.Current.Request.MapPath(subMainPath);

                    grantAccess(System.IO.Path.GetDirectoryName(filePath));

                    FilePath = WebURLFilePath + "\\BarcodeFile.pdf";

                    // var doc = new Document();
                    var doc = new Document(PageSize.A4, 50, 50, 25, 25);

                    PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
                    //open the document for writing
                    doc.Open();


                    iTextSharp.text.pdf.PdfPTable table = new iTextSharp.text.pdf.PdfPTable(1);
                    table.WidthPercentage = 100;
                    table.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfContentByte content = writer.DirectContent;
                    var stringKey = GetUniqueKey(13);
                    Barcode39 bar39 = new Barcode39();

                    bar39.Code = partNumber;//"12345ABCDE";
                    iTextSharp.text.Image img39 = bar39.CreateImageWithBarcode(content, null, null);

                    PdfPCell cell01 = CreateTableCell("Item- " + productName, 0, PdfPCell.ALIGN_LEFT, fontSize: 10);
                    PdfPCell cell02 = CreateTableCell("Part No- " + partNumber, 0, PdfPCell.ALIGN_MIDDLE, fontSize: 7, fontStyle: Font.NORMAL);
                    PdfPCell cell03 = new PdfPCell(img39);
                    cell03.Border = Rectangle.NO_BORDER;
                    cell01.PaddingBottom = 2;

                    table.Rows.Add(new PdfPRow(new PdfPCell[] { cell01 }));
                    table.Rows.Add(new PdfPRow(new PdfPCell[] { cell02 }));
                    table.Rows.Add(new PdfPRow(new PdfPCell[] { cell03 }));


                    //get PdfContentByte object



                    doc.Add(table);
                    //doc.Add(img39);
                    //doc.Add(new Paragraph(""));
                    //doc.Add(new Paragraph(""));

                    doc.Close();

                    #endregion

                }
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool GenerateUpcCodeBarcode(string productName, string UpcCode, ref string FilePath)
        {
            bool result = false;

            try
            {
                CompassEntities objCompassEntities = new CompassEntities();

                string WebURLFilePath = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();

                if (!string.IsNullOrEmpty(productName) && !string.IsNullOrEmpty(UpcCode))
                {
                    //Path on the current server
                    string MainPath = @"~\TempBarcodeFiles";

                    string dynamicFolderName = DateTime.Now.ToString("ddMMyyhhmmssfff");


                    #region generate barcode & add record in database

                    string dynamicFolderName1 = DateTime.Now.ToString("ddMMyyhhmmssfff");
                    //SubInwardRequestId
                    string subMainPath = MainPath + @"\" + dynamicFolderName1 + "\\BarcodeFile.pdf";
                    WebURLFilePath = WebURLFilePath + @"TempBarcodeFiles\" + dynamicFolderName1;

                    string filePath = HttpContext.Current.Request.MapPath(subMainPath);

                    grantAccess(System.IO.Path.GetDirectoryName(filePath));

                    FilePath = WebURLFilePath + "\\BarcodeFile.pdf";

                    // var doc = new Document();
                    var doc = new Document(PageSize.A4, 50, 50, 25, 25);

                    PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
                    //open the document for writing
                    doc.Open();


                    iTextSharp.text.pdf.PdfPTable table = new iTextSharp.text.pdf.PdfPTable(1);
                    table.WidthPercentage = 100;
                    table.DefaultCell.Border = Rectangle.NO_BORDER;

                    PdfContentByte content = writer.DirectContent;
                    var stringKey = GetUniqueKey(13);
                    Barcode39 bar39 = new Barcode39();

                    bar39.Code = UpcCode;//"12345ABCDE";
                    iTextSharp.text.Image img39 = bar39.CreateImageWithBarcode(content, null, null);

                    PdfPCell cell01 = CreateTableCell("Item- " + productName, 0, PdfPCell.ALIGN_LEFT, fontSize: 10);
                    PdfPCell cell02 = CreateTableCell("UPC Code- " + UpcCode, 0, PdfPCell.ALIGN_MIDDLE, fontSize: 7, fontStyle: Font.NORMAL);
                    PdfPCell cell03 = new PdfPCell(img39);
                    cell03.Border = Rectangle.NO_BORDER;
                    cell01.PaddingBottom = 2;

                    table.Rows.Add(new PdfPRow(new PdfPCell[] { cell01 }));
                    table.Rows.Add(new PdfPRow(new PdfPCell[] { cell02 }));
                    table.Rows.Add(new PdfPRow(new PdfPCell[] { cell03 }));


                    //get PdfContentByte object



                    doc.Add(table);
                    //doc.Add(img39);
                    //doc.Add(new Paragraph(""));
                    //doc.Add(new Paragraph(""));

                    doc.Close();

                    #endregion

                }
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public static string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            chars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }


        private PdfPCell CreateTableCell(string phrase, int borderWidth = 1, int align = PdfPCell.ALIGN_LEFT, Phrase customPhrase = null, int fontStyle = Font.BOLD, string fontName = "times new roman", int fontSize = 12, int colspan = 0, int rowspan = 0)
        {
            PdfPCell cell = new PdfPCell();
            if (colspan > 0)
            {
                cell.Colspan = colspan;
            }

            var fo = FontFactory.RegisteredFonts;
            Font font = FontFactory.GetFont(fontName, fontSize, fontStyle);
            if (fontStyle == Font.UNDERLINE)
            {
                font = FontFactory.GetFont(fontName, fontSize, Font.BOLD | fontStyle);
            }

            if (fontStyle == 1000)
            {
                font = FontFactory.GetFont(fontName, fontSize, Font.BOLDITALIC | Font.UNDERLINE);
            }

            if (fontStyle == 2000)
            {
                font = FontFactory.GetFont(fontName, fontSize, Font.BOLDITALIC);
            }

            if (customPhrase == null)
            {
                Phrase phr = new Phrase(phrase, font);
                cell = new PdfPCell(phr);
            }
            else
            {
                cell = new PdfPCell(customPhrase);
            }

            cell.HorizontalAlignment = align;
            cell.PaddingLeft = 5;
            cell.PaddingRight = 5;
            cell.PaddingTop = 2;
            cell.PaddingBottom = 4;
            if (borderWidth == 0)
            {
                cell.BorderWidth = 0;
            }
            //if (rowspan > 0)
            //{
            //    cell.Rowspan = rowspan;
            //}
            //cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;

            return cell;
        }


        public List<InventorySourceChildBO> GetAllUsersBasedOnRoleWithSelf(string RoleName)
        {
            List<InventorySourceChildBO> objInventorySourceChildBOList = new List<InventorySourceChildBO>();
            CompassEntities objCompassEntities = new CompassEntities();


            var manager = new ApplicationDbContext.UserManager();

            var UseNames = manager.GetUsersInRole(RoleName);

            if (UseNames != null)
            {
                foreach (var item in UseNames)
                {
                    var record = objCompassEntities.tblUsers.Where(o => o.Active == 1 && o.aspnet_UserId.ToString() == item.Id.ToString()).
                        Select(o => new InventorySourceChildBO { ChildId = o.UserID, ChildName = o.FirstName }).FirstOrDefault();
                    if (record != null)
                    {
                        objInventorySourceChildBOList.Add(record);
                    }
                }
                objInventorySourceChildBOList = objInventorySourceChildBOList.OrderBy(o => o.ChildName).ToList();
            }
            return objInventorySourceChildBOList;
        }

        public List<long> GetuserbyproductId(long productId)
        {
            var list = new List<long>();
            CompassEntities CompassEntities = new CompassEntities();

            list = CompassEntities.LowInventoryAlerts.Where(o => o.FKProductId == productId && o.Active == 1).Select(x => x.FKUserId).ToList();
            return list;
        }


        public static bool grantAccess(string _direcotryPath)
        {
            bool isSucessed = false;
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
                isSucessed = true;
            }

            catch (Exception ex) { throw ex; }
            return isSucessed;
        }

        

    }
}
