using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.DAL
{
    public class WareHouseDAL : IDisposable
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

        public List<CityModel> GetActiveCityRecordsForState(long stateId)
        {
            var list = new List<CityModel>();
            CompassEntities CompassEntities = new CompassEntities();

            list = CompassEntities.TblCityMasters.Where(o => o.StateId == stateId).Select(x => new CityModel { CityID = x.CityId, CityName = x.CityName }).ToList();
            return list;
        }

        public List<long> GetuserbywarehouseId(long warehouseId)
        {
            var list = new List<long>();
            CompassEntities CompassEntities = new CompassEntities();

            list = CompassEntities.WarehouseUserRelations.Where(o => o.FKWarehouseId == warehouseId && o.Active == 1).Select(x => x.FKUserId).ToList();
            return list;
        }


        public List<WarehouseBO> GetAllRecord(bool Status)
        {

            CompassEntities objCompassEntities = new CompassEntities();

            var objWarehouseBO = objCompassEntities.WarehouseMasters.Where(o =>
                       o.Active == (Status ? 1 : 0)).Select(o =>
                           new WarehouseBO
                           {
                               WarehouseId = o.Id,
                               WarehouseName = o.WarehouseName,
                               WarehouseLocation = o.WarehouseLocation,
                               CityName = o.TblCityMaster.CityName,
                               StateName = o.tblStateMaster.StateName
                           }).ToList();

            return objWarehouseBO;
        }



        public WarehouseBO GetRecord(long WarehouseId)
        {

            CompassEntities objCompassEntities = new CompassEntities();

            var objWarehouseBO = objCompassEntities.WarehouseMasters.Where(o =>
                        o.Id == WarehouseId).Select(o => new WarehouseBO
                        {
                            WarehouseId = o.Id,
                            WarehouseName = o.WarehouseName,
                            WarehouseLocation = o.WarehouseLocation,
                            CityId = o.FKCityId,
                            StateId = o.FKStateId,

                        }).FirstOrDefault();

            return objWarehouseBO;
        }



        public StatusMessage CreateOrUpdate(WarehouseBO ObjectToCreateOrUpdate)
        {
            StatusMessage objStatusMessage = new StatusMessage();
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();

                if (ObjectToCreateOrUpdate.WarehouseId == 0)
                {
                    #region Insert

                    var CheckDuplicate = objCompassEntities.WarehouseMasters.Where(o => o.WarehouseName == ObjectToCreateOrUpdate.WarehouseName
                        && o.WarehouseLocation == ObjectToCreateOrUpdate.WarehouseLocation
                        ).FirstOrDefault();
                    if (CheckDuplicate != null)
                    {
                        objStatusMessage.Success = false;
                        objStatusMessage.Message = "Insert failed. Warehouse name and location already in use.";
                    }
                    else
                    {
                        WarehouseMaster obj = new WarehouseMaster();
                        obj.WarehouseName = ObjectToCreateOrUpdate.WarehouseName;
                        obj.WarehouseLocation = ObjectToCreateOrUpdate.WarehouseLocation;
                        obj.FKStateId = ObjectToCreateOrUpdate.StateId;
                        obj.FKCityId = ObjectToCreateOrUpdate.CityId;

                        obj.Active = 1;
                        obj.CreatedBy = ObjectToCreateOrUpdate.CurrentUserId;
                        obj.CreatedOn = new CommonFunctions().ServerDate();
                        objCompassEntities.WarehouseMasters.Add(obj);
                        objCompassEntities.SaveChanges();
                        objStatusMessage.Success = true;
                        objStatusMessage.Message = "Record created successfully.";


                        if (ObjectToCreateOrUpdate.userList != null && ObjectToCreateOrUpdate.userList.Count > 0)
                        {
                            foreach (var item in ObjectToCreateOrUpdate.userList)
                            {
                                WarehouseUserRelation warehouseuserbo = new WarehouseUserRelation();
                                warehouseuserbo.FKWarehouseId = obj.Id;
                                warehouseuserbo.FKUserId = item;
                                warehouseuserbo.Active = 1;
                                warehouseuserbo.CreatedBy = ObjectToCreateOrUpdate.CurrentUserId;
                                warehouseuserbo.CreatedOn = new CommonFunctions().ServerDate();
                                objCompassEntities.WarehouseUserRelations.Add(warehouseuserbo);
                                objCompassEntities.SaveChanges();
                            }

                        }
                    }
                    #endregion

                }
                else
                {
                    #region Update


                    var CheckDuplicate = objCompassEntities.WarehouseMasters.Where(o => o.WarehouseName == ObjectToCreateOrUpdate.WarehouseName &&
                         o.WarehouseLocation == ObjectToCreateOrUpdate.WarehouseLocation && o.Id != ObjectToCreateOrUpdate.WarehouseId).FirstOrDefault();
                    if (CheckDuplicate != null)
                    {
                        objStatusMessage.Success = false;
                        objStatusMessage.Message = "Update failed. Warehouse name & location already in use.";
                    }
                    else
                    {
                        CheckDuplicate = objCompassEntities.WarehouseMasters.Where(o => o.Id == ObjectToCreateOrUpdate.WarehouseId).FirstOrDefault();
                        CheckDuplicate.WarehouseName = ObjectToCreateOrUpdate.WarehouseName;
                        CheckDuplicate.WarehouseLocation = ObjectToCreateOrUpdate.WarehouseLocation;
                        CheckDuplicate.FKStateId = ObjectToCreateOrUpdate.StateId;
                        CheckDuplicate.FKCityId = ObjectToCreateOrUpdate.CityId;

                        CheckDuplicate.ModifiedBy = ObjectToCreateOrUpdate.CurrentUserId;
                        CheckDuplicate.ModifiedOn = DateTime.Now;

                        objCompassEntities.SaveChanges();

                        objStatusMessage.Success = true;
                        objStatusMessage.Message = "Record updated successfully.";

                        objCompassEntities.WarehouseUserRelations
                        .Where(x => x.FKWarehouseId == ObjectToCreateOrUpdate.WarehouseId)
                        .ToList()
                        .ForEach(a => a.Active = 0);

                        objCompassEntities.SaveChanges();
                        if (ObjectToCreateOrUpdate.userList != null && ObjectToCreateOrUpdate.userList.Count > 0)
                        {
                            foreach (var item in ObjectToCreateOrUpdate.userList)
                            {
                                var recordexist = objCompassEntities.WarehouseUserRelations.Where(a => a.FKUserId == item && a.FKWarehouseId == ObjectToCreateOrUpdate.WarehouseId).FirstOrDefault();
                                if (recordexist != null)
                                {
                                    recordexist.Active = 1;
                                    recordexist.ModifiedBy = ObjectToCreateOrUpdate.CurrentUserId;
                                    recordexist.ModifiedOn = new CommonFunctions().ServerDate();
                                }
                                else
                                {
                                    WarehouseUserRelation warehouseuserbo = new WarehouseUserRelation();
                                    warehouseuserbo.FKWarehouseId = ObjectToCreateOrUpdate.WarehouseId;
                                    warehouseuserbo.FKUserId = item;
                                    warehouseuserbo.Active = 1;
                                    warehouseuserbo.CreatedBy = ObjectToCreateOrUpdate.CurrentUserId;
                                    warehouseuserbo.CreatedOn = new CommonFunctions().ServerDate();
                                    objCompassEntities.WarehouseUserRelations.Add(warehouseuserbo);
                                }
                                objCompassEntities.SaveChanges();
                            }

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
                var ObjMenu = objCompassEntities.WarehouseMasters.Where(o => o.Id == Id).FirstOrDefault();

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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="CurrentDate"></param>
        /// <returns></returns>
        public List<InventorySourceChildBO> GetWareHouseList()
        {
            List<InventorySourceChildBO> objList = new List<InventorySourceChildBO>();
            CompassEntities objCompassEntities = new CompassEntities();
            objList = objCompassEntities.WarehouseMasters.Where(o => o.Active == 1).
                Select(o => new InventorySourceChildBO { ChildId = o.Id, ChildName = o.WarehouseName + ", " + o.WarehouseLocation + ", " + o.TblCityMaster.CityName }).ToList();

            return objList;
        }



        public List<MobileWarehouseBO> GetWarehouseListNew()
        {
            List<MobileWarehouseBO> objMobileWarehouseBOList = new List<MobileWarehouseBO>();


            try
            {
                CompassEntities objCompassEntities = new CompassEntities();

                objMobileWarehouseBOList = objCompassEntities.WarehouseMasters.Where(o => o.Active == 1).Select(o =>
                        new MobileWarehouseBO
                        {
                            Id = o.Id,
                            WarehouseName = o.WarehouseLocation + ", " + o.WarehouseName + ", " + o.TblCityMaster.CityName + ", " + o.tblStateMaster.StateName
                        }
                        ).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }


            return objMobileWarehouseBOList;
        }

        public List<ProductMasterBO> GetProductList()
        {
            CompassEntities CompassEntities = new CompassEntities();

            List<ProductMasterBO> data = CompassEntities.ProductMasters.Where(a => a.Active == 1).Select(o =>
                                       new ProductMasterBO
                                       {
                                           Id = o.Id,
                                           ProductName = o.ProductName,
                                           CategoryId = o.ProductCategoryId
                                       }).OrderBy(a => a.ProductName).ToList();
            return data;
        }

        public List<CategoryBO> GetCategoryList()
        {
            CompassEntities CompassEntities = new CompassEntities();

            List<CategoryBO> data = CompassEntities.Categories.Where(a => a.Active == 1).Select(o =>
                                       new CategoryBO
                                       {
                                           CategoryId = o.CategoryId,
                                           CategoryName = o.CategoryName
                                       }).OrderBy(a => a.CategoryName).ToList();
            return data;
        }

        /// <summary>
        /// 24-jul-2017
        /// Aniket
        /// Get reprt data
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="categoryId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>        
        public List<WarehouseStockReport> getwarehouselistnew(long[] warehouseId, long projectId, long productId)
        {
            List<WarehouseStockReport> productmasterbo = new List<WarehouseStockReport>();


            try
            {
                CompassEntities objcompassentities = new CompassEntities();

                foreach (var item in warehouseId)
                {
                    var query = objcompassentities.PROC_Report_WarehouseStockReport(projectId, item).AsEnumerable();

                    //if (categoryId != 0)
                    //{
                    //    query = query.Where(f => f.CategoryId == categoryId);
                    //}
                    if (productId != 0)
                    {
                        query = query.Where(f => f.ProductId == productId);
                    }

                    var data = query.Select(o =>
                             new WarehouseStockReport
                             {
                                 StockDetailId = o.StockDetailId,
                                 WarehouseId = o.WarehouseId,
                                 WarehouseName = o.WarehouseName,
                                 ProductId = o.ProductId,
                                 ProductName = o.ProductName,
                                 ProductDescription = o.ProductDescription,
                                 PartNumber = o.PartNumber,
                                 UPCCode = o.UPCCode,
                                 UtilityName = o.UtilityName,
                                 CategoryId = o.CategoryId,
                                 CategoryName = o.CategoryName,
                                 Make = o.Make,
                                 MeterType = o.MeterType,
                                 MeterSize = o.MeterSize,
                                 serialnumber = o.serialnumber,
                                 BalanceQty = o.BalanceQty,
                                 SerialNumberRequired = o.SerialNumberRequired,
                                 TotalQty = o.TotalQty,
                             }
                            ).ToList();

                    if (data != null && data.Count > 0)
                    {
                        productmasterbo.AddRange(data);
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }


            return productmasterbo;
        }


        public List<WarehouseStockReport> GetWarehouseStockReport_Summary(long[] warehouseId, long projectId, long productId)
        {
            List<WarehouseStockReport> productmasterbo = new List<WarehouseStockReport>();


            try
            {
                CompassEntities objcompassentities = new CompassEntities();


                foreach (var item in warehouseId)
                {
                    var query = objcompassentities.PROC_Report_WarehouseStockReportSummary(projectId, item).AsEnumerable();

                    //if (categoryId != 0)
                    //{
                    //    query = query.Where(f => f.CategoryId == categoryId);
                    //}
                    if (productId != 0)
                    {
                        query = query.Where(f => f.ProductId == productId);
                    }

                    var data = query.Select(o =>
                             new WarehouseStockReport
                             {

                                 WarehouseId = o.WarehouseId,
                                 WarehouseName = o.WarehouseName,
                                 ProductId = o.ProductId,
                                 ProductName = o.ProductName,
                                 ProductDescription = o.ProductDescription,
                                 PartNumber = o.PartNumber,
                                 UPCCode = o.UPCCode,
                                 UtilityName = o.UtilityName,
                                 CategoryId = o.CategoryId,
                                 CategoryName = o.CategoryName,
                                 Make = o.Make,
                                 MeterType = o.MeterType,
                                 MeterSize = o.MeterSize,
                                 BalanceQty = o.BalanceQty,
                                 SerialNumberRequired = o.SerialNumberRequired,
                                 TotalQty = o.TotalQty,
                             }
                            ).ToList();

                    if (data != null && data.Count > 0)
                    {
                        productmasterbo.AddRange(data);
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }


            return productmasterbo;
        }

        public List<WarehouseStockReport> getwarehouselistServerSide(string sortColumn, string sortColumnDir, string searchvalue, int skip, int pagesize, out int totalRecords, long warehouseId, long projectId, long productId)
        {
            List<WarehouseStockReport> productmasterbo = new List<WarehouseStockReport>();

            try
            {
                CompassEntities objcompassentities = new CompassEntities();

                var query = objcompassentities.PROC_Report_WarehouseStockReport(projectId, warehouseId).AsEnumerable();

                //if (categoryId != 0)
                //{
                //    query = query.Where(f => f.CategoryId == categoryId);
                //}
                if (productId != 0)
                {
                    query = query.Where(f => f.ProductId == productId);
                }

                if (!string.IsNullOrEmpty(searchvalue))
                {
                    query = query.Where(m => m.WarehouseName.Contains(searchvalue)
                     || m.ProductName.Contains(searchvalue)
                     || m.PartNumber.Contains(searchvalue)
                     || m.UPCCode.Contains(searchvalue)
                     || m.UtilityName.Contains(searchvalue)
                     || m.CategoryName.Contains(searchvalue)
                     || m.Make.Contains(searchvalue)
                     || m.MeterType.Contains(searchvalue)
                     || m.MeterSize.Contains(searchvalue)
                     || m.serialnumber.Contains(searchvalue)
                     || m.BalanceQty.ToString().Contains(searchvalue));
                };

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    query = query.OrderBy(sortColumn + " " + sortColumnDir);
                }

                totalRecords = query.Count();

                productmasterbo = query.Skip(skip).Take(pagesize).AsEnumerable().Select(o =>
                         new WarehouseStockReport
                         {
                             StockDetailId = o.StockDetailId,
                             WarehouseId = o.WarehouseId,
                             WarehouseName = o.WarehouseName,
                             ProductId = o.ProductId,
                             ProductName = o.ProductName,
                             ProductDescription = o.ProductDescription,
                             PartNumber = o.PartNumber,
                             UPCCode = o.UPCCode,
                             UtilityName = o.UtilityName,
                             CategoryId = o.CategoryId,
                             CategoryName = o.CategoryName,
                             Make = o.Make,
                             MeterType = o.MeterType,
                             MeterSize = o.MeterSize,
                             serialnumber = o.serialnumber,
                             BalanceQty = o.BalanceQty,
                             SerialNumberRequired = o.SerialNumberRequired,
                             TotalQty = o.TotalQty,
                         }
                        ).ToList();


            }
            catch (Exception ex)
            {
                throw ex;
            }


            return productmasterbo;
        }

        /// <summary>
        /// 25-Jul-2017
        /// Aniket
        /// Get data to export into excel
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="categoryId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public List<WarehouseStockReportExport> exportwarehouseReport(long[] warehouseId, long projectId, long productId)
        {
            List<WarehouseStockReportExport> productmasterbo = new List<WarehouseStockReportExport>();
            try
            {
                CompassEntities objcompassentities = new CompassEntities();

                foreach (var item in warehouseId)
                {
                    var query = objcompassentities.PROC_Report_WarehouseStockReport(projectId, item).AsEnumerable();

                    //if (categoryId != 0)
                    //{
                    //    query = query.Where(f => f.CategoryId == categoryId);
                    //}
                    if (productId != 0)
                    {
                        query = query.Where(f => f.ProductId == productId);
                    }

                    var data = query.Select(o => new WarehouseStockReportExport
                    {

                        WarehouseName = o.WarehouseName,
                        CategoryName = o.CategoryName,
                        ProductName = o.ProductName,
                        ProductDescription = o.ProductDescription,
                        PartNumber = o.PartNumber,
                        UPCCode = o.UPCCode,
                        UtilityName = o.UtilityName,
                        Make = o.Make,
                        MeterType = o.MeterType,
                        MeterSize = o.MeterSize,
                        serialnumber = o.serialnumber,
                        BalanceQty = o.BalanceQty,
                    }).ToList();

                    if (data != null && data.Count > 0)
                    {
                        productmasterbo.AddRange(data);
                    }
                }

             
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return productmasterbo;
        }

        public List<WarehouseStockReportSummeryExport> exportwarehouseSummeryReport(long[] warehouseId, long projectId, long productId)
        {
            List<WarehouseStockReportSummeryExport> productmasterbo = new List<WarehouseStockReportSummeryExport>();
            try
            {
                CompassEntities objcompassentities = new CompassEntities();

                foreach (var item in warehouseId)
                {
                    var query = objcompassentities.PROC_Report_WarehouseStockReportSummary(projectId, item).AsEnumerable();

                    //if (categoryId != 0)
                    //{
                    //    query = query.Where(f => f.CategoryId == categoryId);
                    //}
                    if (productId != 0)
                    {
                        query = query.Where(f => f.ProductId == productId);
                    }

                    var data = query.Select(o => new WarehouseStockReportSummeryExport
                    {

                        WarehouseName = o.WarehouseName,
                        CategoryName = o.CategoryName,
                        ProductName = o.ProductName,
                        ProductDescription = o.ProductDescription,
                        PartNumber = o.PartNumber,
                        UPCCode = o.UPCCode,
                        UtilityName = o.UtilityName,
                        Make = o.Make,
                        MeterType = o.MeterType,
                        MeterSize = o.MeterSize,
                        BalanceQty = o.BalanceQty,
                    }).ToList();
                    if (data != null && data.Count > 0)
                    {
                        productmasterbo.AddRange(data);
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }


            return productmasterbo;
        }

        public List<UsersModel> GetUserList()
        {

            CompassEntities dbcontext = new CompassEntities();
            List<UsersModel> UsersModels = (from u in dbcontext.tblUsers.AsNoTracking()
                                            select new UsersModel()
                                            {

                                                UserId = u.UserID,
                                                UserFullName = u.UserName

                                            }).ToList();
            return UsersModels;
        }

        /// <summary>
        /// Aniket
        /// 9-Aug-2017
        /// update record in stock detail
        /// </summary>
        /// <param name="StockDetailId"></param>
        /// <param name="Serialnumber"></param>
        /// <param name="Quantity"></param>
        /// <returns></returns>
        public StatusMessage UpdateStockRecord(long StockDetailId, string Serialnumber, int Quantity, long userId)
        {
            StatusMessage statusMessage = new StatusMessage();
            CompassEntities dbcontext = new CompassEntities();
            try
            {
                var stockdetailRecord = dbcontext.StockDetails.Where(a => a.Id == StockDetailId && a.Active == 1).FirstOrDefault();
                if (stockdetailRecord != null)
                {
                    var productmaster = dbcontext.ProductMasters.Where(a => a.Id == stockdetailRecord.FKProductId && a.Active == 1).FirstOrDefault();
                    if (productmaster.SerialNumberRequired)
                    {
                        ///Check new serial number is duplicate in current database.

                        var checkDuplicate = dbcontext.StockDetails.AsNoTracking().Where(o => o.Active == 1 && o.SerialNumber == Serialnumber).FirstOrDefault();

                        if (checkDuplicate == null)
                        {
                            //serial number product
                            if (stockdetailRecord.AllStockUtilized == false && stockdetailRecord.BalanceQty > 0)
                            {
                                stockdetailRecord.SerialNumber = Serialnumber;
                                stockdetailRecord.ModifiedBy = userId;
                                stockdetailRecord.ModifiedOn = new CommonFunctions().ServerDate();

                                var inwardDeatil = dbcontext.InwardDetails.Where(a => a.Id == stockdetailRecord.FKInwardDetailId).FirstOrDefault();
                                inwardDeatil.SerialNumber = Serialnumber;
                                inwardDeatil.ModifiedBy = userId;
                                inwardDeatil.ModifiedOn = new CommonFunctions().ServerDate();

                                dbcontext.SaveChanges();
                                statusMessage.Success = true;
                                statusMessage.Message = "Record Updated Successfully";
                            }
                            else
                            {
                                statusMessage.Success = false;
                                statusMessage.Message = "Update failed. Product Already in use";
                            }
                        }
                        else
                        {
                            statusMessage.Success = false;
                            statusMessage.Message = "Update failed. Serial number already present in the inventory.";
                        }


                    }
                    else
                    {
                        //Quantity product
                        if (stockdetailRecord.AllStockUtilized == false && stockdetailRecord.BalanceQty > 0)
                        {
                            var existingQuantity = stockdetailRecord.BalanceQty;
                            var additionalQuantity = (Quantity - existingQuantity);

                            stockdetailRecord.TotalQty = (stockdetailRecord.TotalQty + additionalQuantity);

                            stockdetailRecord.BalanceQty = Quantity;
                            stockdetailRecord.ModifiedBy = userId;
                            stockdetailRecord.ModifiedOn = new CommonFunctions().ServerDate();

                            var inwardDeatil = dbcontext.InwardDetails.Where(a => a.Id == stockdetailRecord.FKInwardDetailId).FirstOrDefault();
                            inwardDeatil.UploadQty = stockdetailRecord.TotalQty;
                            inwardDeatil.ModifiedBy = userId;
                            inwardDeatil.ModifiedOn = new CommonFunctions().ServerDate();

                            dbcontext.SaveChanges();
                            statusMessage.Success = true;
                            statusMessage.Message = "Record Updated Successfully";
                        }
                        else
                        {
                            statusMessage.Success = false;
                            statusMessage.Message = "Product Already in use";
                        }

                    }


                }
                else
                {
                    statusMessage.Success = false;
                    statusMessage.Message = "Unable to update record";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Success = false;
                statusMessage.Message = "Unable to update record";
                throw ex;
            }

            return statusMessage;
        }

        /// <summary>
        /// Aniket
        /// 9-Aug-2017
        /// Delete record from stock and inward
        /// </summary>
        /// <param name="StockDetailId"></param>
        /// <returns></returns>
        public StatusMessage DeleteStockRecord(long StockDetailId, long userId)
        {
            StatusMessage statusMessage = new StatusMessage();
            CompassEntities dbcontext = new CompassEntities();
            try
            {
                var stockdetailRecord = dbcontext.StockDetails.Where(a => a.Id == StockDetailId && a.Active == 1).FirstOrDefault();
                if (stockdetailRecord != null)
                {
                    var productmaster = dbcontext.ProductMasters.Where(a => a.Id == stockdetailRecord.FKProductId && a.Active == 1).FirstOrDefault();
                    if (productmaster.SerialNumberRequired)
                    {

                        if (stockdetailRecord.AllStockUtilized == false && stockdetailRecord.BalanceQty > 0)
                        {
                            stockdetailRecord.Active = 0;
                            stockdetailRecord.ModifiedBy = userId;
                            stockdetailRecord.ModifiedOn = new CommonFunctions().ServerDate();


                            var ObjList = dbcontext.StockDetails.Where(o => o.Active == 1 && o.BalanceQty == 0 && o.IsSerialNumber == 1 && o.SerialNumber == "").ToList();

                            foreach (var item in ObjList)
                            {
                                item.Active = 0;
                                item.ModifiedBy = userId;
                                item.ModifiedOn = new CommonFunctions().ServerDate();


                                var objActiveList = dbcontext.OutwardDetails.Where(o => o.Active == 1 && o.FKStockDetailsID == item.Id).ToList();

                                foreach (var item1 in objActiveList)
                                {
                                    item1.Active = 0;
                                    item1.ModifiedBy = userId;
                                    item1.ModifiedOn = new CommonFunctions().ServerDate();
                                }

                            }




                            var inwardDeatil = dbcontext.InwardDetails.Where(a => a.Id == stockdetailRecord.FKInwardDetailId).FirstOrDefault();
                            inwardDeatil.Active = 0;
                            inwardDeatil.ModifiedBy = userId;
                            inwardDeatil.ModifiedOn = new CommonFunctions().ServerDate();


                            dbcontext.SaveChanges();
                            statusMessage.Success = true;
                            statusMessage.Message = "Record Deleted Successfully.";
                        }
                        else
                        {
                            statusMessage.Success = false;
                            statusMessage.Message = "Delete failed as product is released to user.";
                        }


                    }
                    else
                    {
                        if (stockdetailRecord.TotalQty == stockdetailRecord.BalanceQty)
                        {
                            stockdetailRecord.Active = 0;

                            var inwardDeatil = dbcontext.InwardDetails.Where(a => a.Id == stockdetailRecord.FKInwardDetailId).FirstOrDefault();
                            inwardDeatil.Active = 0;
                            dbcontext.SaveChanges();
                            statusMessage.Success = true;
                            statusMessage.Message = "Record Deleted Successfully";
                        }
                        else
                        {
                            statusMessage.Success = false;
                            statusMessage.Message = "Unable to delete record.</br> Product is released to user";
                        }
                    }

                }
                else
                {
                    statusMessage.Success = false;
                    statusMessage.Message = "Unable to delete record";
                }

            }
            catch (Exception ex)
            {
                statusMessage.Success = false;
                statusMessage.Message = "Unable to delete record ";
                throw ex;
            }

            return statusMessage;
        }




    }

}
