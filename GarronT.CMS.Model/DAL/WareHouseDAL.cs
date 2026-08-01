using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

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


        public List<WarehouseBO> GetAllRecord(bool Status, List<long> projectIds, long userId)
        {
            CompassEntities db = new CompassEntities();

            if (projectIds == null) projectIds = new List<long>();

            var warehouses = (from w in db.WarehouseMasters
                              join ih in db.InwardHeaders
                                   on w.Id equals ih.FKWarehouseId into whJoin
                              from ih in whJoin.DefaultIfEmpty()   // 🔥 left join
                              where w.Active == (Status ? 1 : 0)
                              && (
                                    // 🔥 Condition 1: Project me use hua ho
                                    (ih != null && ih.Active == 1
                                        && ih.FKProjectId.HasValue
                                        && projectIds.Contains(ih.FKProjectId.Value))

                                    // 🔥 Condition 2: User ne create kiya ho
                                    || w.CreatedBy == userId
                                 )
                              select new
                              {
                                  w.Id,
                                  w.WarehouseName,
                                  w.WarehouseLocation,
                                  CityName = w.TblCityMaster.CityName,
                                  StateName = w.tblStateMaster.StateName
                              })
                              .ToList()
                              .GroupBy(x => x.Id)
                              .Select(g => new WarehouseBO
                              {
                                  WarehouseId = g.First().Id,
                                  WarehouseName = g.First().WarehouseName,
                                  WarehouseLocation = g.First().WarehouseLocation,
                                  CityName = g.First().CityName,
                                  StateName = g.First().StateName
                              })
                              .ToList();

            return warehouses;
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
        public List<WarehouseStockReport> getwarehouselistnew(long[] warehouseId, long projectId, long productId,string LocationType)
        {
            List<WarehouseStockReport> productmasterbo = new List<WarehouseStockReport>();


            try
            {
                CompassEntities objcompassentities = new CompassEntities();

                foreach (var item in warehouseId)
                {
                    /*List<PROC_Report_WarehouseStockReport_Result> objList = new List<PROC_Report_WarehouseStockReport_Result>();
                    SqlConnection sqlConnection1 = new SqlConnection(objcompassentities.Database.Connection.ConnectionString);
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = "PROC_Report_WarehouseStockReport";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = sqlConnection1;
                    cmd.Parameters.Add("@ProjectId", projectId);
                    cmd.Parameters.Add("@warehouseId", item);
                    cmd.CommandTimeout = 600;
                    //cmd.CommandTimeout = 120;
                    sqlConnection1.Open();

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        objList.Add(new PROC_Report_WarehouseStockReport_Result
                        {
                            StockDetailId = long.Parse(reader["StockDetailId"].ToString()),
                            WarehouseId = long.Parse(reader["WarehouseId"].ToString()),
                            WarehouseName = reader["WarehouseName"].ToString(),
                            ProductId = long.Parse(reader["ProductId"].ToString()),
                            ProductName = reader["ProductName"].ToString(),
                            ProductDescription = reader["ProductDescription"].ToString(),
                            PartNumber = reader["PartNumber"].ToString(),
                            UPCCode = reader["UPCCode"].ToString(),
                            UtilityName = reader["UtilityName"].ToString(),
                            CategoryId = long.Parse(reader["CategoryId"].ToString()),
                            CategoryName = reader["CategoryName"].ToString(),
                            Make = reader["Make"].ToString(),
                            MeterType = reader["MeterType"].ToString(),
                            MeterSize = reader["MeterSize"].ToString(),
                            SerialNumberRequired = bool.Parse(reader["SerialNumberRequired"].ToString()),
                            serialnumber = reader["serialnumber"].ToString(),
                            TotalQty = int.Parse(reader["TotalQty"].ToString()),
                            BalanceQty = int.Parse(reader["BalanceQty"].ToString())
                        });
                    }
                    sqlConnection1.Close();
                    reader.Close();
                    reader.Dispose();
                    cmd.Dispose();
                    sqlConnection1.Dispose();*/

                    //var query = objcompassentities.PROC_Report_WarehouseStockReport(projectId, item).AsEnumerable();
                    var query = objcompassentities.PROC_Report_WarehouseStockReport(projectId, item).ToList();

                    //if (categoryId != 0)
                    //{
                    //    query = query.Where(f => f.CategoryId == categoryId);
                    //}
                    if (productId != 0)
                    {
                        query = query.Where(f => f.ProductId == productId).Select(f => f).ToList();
                    }
                    if (LocationType != "0" && !string.IsNullOrEmpty(LocationType))
                    {
                        if (LocationType == "null" || LocationType == null)
                        {
                            query = query.Where(f => f.Location == null).Select(f => f).ToList();
                        }
                        else
                        {
                            query = query.Where(f => f.Location == LocationType).Select(f => f).ToList();
                        }  
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
                                 Location = o.Location
                             }
                            ).ToList();

                    if (data != null && data.Count > 0)
                    {
                        PopulateCreatedOn(objcompassentities, data);
                        productmasterbo.AddRange(data);
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return productmasterbo
                .OrderByDescending(a => a.CreatedOn ?? DateTime.MinValue)
                .ThenBy(a => a.WarehouseName)
                .ThenBy(a => a.ProductName)
                .ThenBy(a => a.serialnumber)
                .ToList();
        }


        public List<WarehouseStockReport> GetWarehouseStockReport_Summary(long[] warehouseId, long projectId, long productId,string LocationType)
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
                    if (LocationType != "0" && !string.IsNullOrEmpty(LocationType))
                    {
                        query = query.Where(f => f.Location == LocationType);
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
                                 Location=o.Location
                             }
                            ).ToList();

                    if (data != null && data.Count > 0)
                    {
                        PopulateCreatedOnForSummary(objcompassentities, projectId, data);
                        productmasterbo.AddRange(data);
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }


            return productmasterbo
                .OrderByDescending(a => a.CreatedOn ?? DateTime.MinValue)
                .ThenBy(a => a.WarehouseName)
                .ThenBy(a => a.ProductName)
                .ToList();
        }

        public long GetWarehouseStockInventoryReport(long warehouseId, long projectId, long productId,string LocationFromValue)
        {
            long ablInventory = 0;
            try
            {
                CompassEntities objcompassentities = new CompassEntities();
                //var query = objcompassentities.PROC_Report_WarehouseStockReportSummary(projectId, warehouseId).AsEnumerable();
                var query = objcompassentities.PROC_Report_WarehouseStockReport(projectId, warehouseId).AsEnumerable();
                if (productId != 0)
                {
                    query = query.Where(f => f.ProductId == productId);
                }
                if (LocationFromValue != "0" && !string.IsNullOrEmpty(LocationFromValue))
                {
                    query = query.Where(f => f.Location == LocationFromValue);
                }
                ablInventory = query.Count();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ablInventory;
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

                PopulateCreatedOn(objcompassentities, productmasterbo);

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

                    var queryList = query.ToList();
                    var detailRows = queryList.Select(o => new WarehouseStockReport
                    {
                        StockDetailId = o.StockDetailId,
                        WarehouseId = o.WarehouseId,
                        ProductId = o.ProductId,
                        serialnumber = o.serialnumber,
                        Location = o.Location
                    }).ToList();
                    PopulateCreatedOn(objcompassentities, detailRows);

                    var data = queryList.Select((o, idx) => new WarehouseStockReportExport
                    {

                        WarehouseName = o.WarehouseName,
                        Location = o.Location,
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
                        DateAndTime = !string.IsNullOrEmpty(detailRows[idx].DateAndTime)
                            ? detailRows[idx].DateAndTime.Replace("<br/>", " ")
                            : null,
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

        private static Dictionary<long, DateTime> GetCreatedOnLookup(CompassEntities entities, IEnumerable<long> stockDetailIds)
        {
            var ids = stockDetailIds != null ? stockDetailIds.Where(id => id > 0).Distinct().ToList() : new List<long>();
            if (ids.Count == 0)
            {
                return new Dictionary<long, DateTime>();
            }

            var lookup = new Dictionary<long, DateTime>();
            const int chunkSize = 500;
            for (int i = 0; i < ids.Count; i += chunkSize)
            {
                var chunk = ids.Skip(i).Take(chunkSize).ToList();
                var idCsv = string.Join(",", chunk);
                var rows = entities.Database.SqlQuery<StockCreatedOnRow>(
                    "SELECT Id, CreatedOn FROM dbo.StockDetails WHERE Id IN (" + idCsv + ")").ToList();
                foreach (var row in rows)
                {
                    lookup[row.Id] = row.CreatedOn;
                }
            }
            return lookup;
        }

        private class StockCreatedOnRow
        {
            public long Id { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        private static string FormatDateAndTimeHtml(DateTime dt)
        {
            return dt.ToString("MM/dd/yyyy") + "<br/>" + dt.ToString("HH:mm:ss");
        }

        private static string FormatDateAndTimeExport(DateTime dt)
        {
            return dt.ToString("MM/dd/yyyy") + " " + dt.ToString("HH:mm:ss");
        }

        private static string NormalizeLocation(string location)
        {
            return string.IsNullOrWhiteSpace(location) ? "" : location.Trim();
        }

        private static void PopulateCreatedOn(CompassEntities entities, List<WarehouseStockReport> rows)
        {
            if (rows == null || rows.Count == 0)
            {
                return;
            }

            var createdLookup = GetCreatedOnLookup(entities, rows.Select(r => r.StockDetailId));
            foreach (var row in rows)
            {
                DateTime createdOn;
                if (createdLookup.TryGetValue(row.StockDetailId, out createdOn))
                {
                    row.CreatedOn = createdOn;
                    row.DateAndTime = FormatDateAndTimeHtml(createdOn);
                }
            }

            // Fallback for rows whose StockDetailId is missing/0 — match by warehouse + product + serial/location.
            var missing = rows.Where(r => string.IsNullOrEmpty(r.DateAndTime)).ToList();
            if (missing.Count == 0)
            {
                return;
            }

            var warehouseIds = missing.Select(r => r.WarehouseId).Distinct().ToList();
            var productIds = missing.Select(r => r.ProductId).Where(id => id > 0).Distinct().ToList();
            if (warehouseIds.Count == 0 || productIds.Count == 0)
            {
                return;
            }

            var stockList = entities.StockDetails
                .Where(s => warehouseIds.Contains(s.FKWarehouseId) && productIds.Contains(s.FKProductId))
                .Select(s => new { s.FKWarehouseId, s.FKProductId, s.SerialNumber, s.Location, s.CreatedOn })
                .ToList();

            var bySerial = stockList
                .Where(s => !string.IsNullOrEmpty(s.SerialNumber))
                .GroupBy(s => Tuple.Create(s.FKWarehouseId, s.FKProductId, s.SerialNumber))
                .ToDictionary(g => g.Key, g => g.Max(x => x.CreatedOn));

            var byLocation = stockList
                .GroupBy(s => Tuple.Create(s.FKWarehouseId, s.FKProductId, NormalizeLocation(s.Location)))
                .ToDictionary(g => g.Key, g => g.Max(x => x.CreatedOn));

            var byProduct = stockList
                .GroupBy(s => Tuple.Create(s.FKWarehouseId, s.FKProductId))
                .ToDictionary(g => g.Key, g => g.Max(x => x.CreatedOn));

            foreach (var row in missing)
            {
                DateTime createdOn;
                if (!string.IsNullOrEmpty(row.serialnumber)
                    && bySerial.TryGetValue(Tuple.Create(row.WarehouseId, row.ProductId, row.serialnumber), out createdOn))
                {
                    row.CreatedOn = createdOn;
                    row.DateAndTime = FormatDateAndTimeHtml(createdOn);
                }
                else if (byLocation.TryGetValue(Tuple.Create(row.WarehouseId, row.ProductId, NormalizeLocation(row.Location)), out createdOn))
                {
                    row.CreatedOn = createdOn;
                    row.DateAndTime = FormatDateAndTimeHtml(createdOn);
                }
                else if (byProduct.TryGetValue(Tuple.Create(row.WarehouseId, row.ProductId), out createdOn))
                {
                    row.CreatedOn = createdOn;
                    row.DateAndTime = FormatDateAndTimeHtml(createdOn);
                }
            }
        }

        private static void PopulateCreatedOnForSummary(CompassEntities entities, long projectId, List<WarehouseStockReport> rows)
        {
            if (rows == null || rows.Count == 0)
            {
                return;
            }

            var warehouseIds = rows.Select(r => r.WarehouseId).Distinct().ToList();
            var productIds = rows.Select(r => r.ProductId).Distinct().ToList();
            if (warehouseIds.Count == 0 || productIds.Count == 0)
            {
                return;
            }

            // Do not require Active=1 — report can include depleted/inactive stock rows.
            // Also include null project rows that still belong to the warehouse/product.
            var stockList = entities.StockDetails
                .Where(s => warehouseIds.Contains(s.FKWarehouseId)
                    && productIds.Contains(s.FKProductId)
                    && (s.FKProjectId == null || s.FKProjectId == projectId))
                .Select(s => new { s.FKWarehouseId, s.FKProductId, s.Location, s.CreatedOn })
                .ToList();

            var lookup = stockList
                .GroupBy(s => Tuple.Create(s.FKWarehouseId, s.FKProductId, NormalizeLocation(s.Location)))
                .ToDictionary(g => g.Key, g => g.Max(x => x.CreatedOn));

            var lookupByProduct = stockList
                .GroupBy(s => Tuple.Create(s.FKWarehouseId, s.FKProductId))
                .ToDictionary(g => g.Key, g => g.Max(x => x.CreatedOn));

            foreach (var row in rows)
            {
                var key = Tuple.Create(row.WarehouseId, row.ProductId, NormalizeLocation(row.Location));
                DateTime createdOn;
                if (lookup.TryGetValue(key, out createdOn)
                    || lookupByProduct.TryGetValue(Tuple.Create(row.WarehouseId, row.ProductId), out createdOn))
                {
                    row.CreatedOn = createdOn;
                    row.DateAndTime = FormatDateAndTimeHtml(createdOn);
                }
            }
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

                    var queryList = query.ToList();
                    var summaryRows = queryList.Select(o => new WarehouseStockReport
                    {
                        WarehouseId = o.WarehouseId,
                        ProductId = o.ProductId,
                        Location = o.Location
                    }).ToList();
                    PopulateCreatedOnForSummary(objcompassentities, projectId, summaryRows);

                    var data = queryList.Select((o, idx) => new WarehouseStockReportSummeryExport
                    {
                        WarehouseName = o.WarehouseName,
                        Location = o.Location,
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
                        DateAndTime = !string.IsNullOrEmpty(summaryRows[idx].DateAndTime)
                            ? summaryRows[idx].DateAndTime.Replace("<br/>", " ")
                            : null,
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
        /// Transfer Inventory
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="ToLocation"></param>
        /// <param name="TransferInventoryList"></param>
        public StatusMessage TransferInventory(long ProjectId, string ToLocation, string[] TransferInventoryList, long userId)
        {
            StatusMessage statusMessage = new StatusMessage();
            CompassEntities dbcontext = new CompassEntities();
            try
            {
                foreach (var inventoryNo in TransferInventoryList)
                {
                    var objInwordDetailData = dbcontext.InwardDetails.Where(o => o.SerialNumber == inventoryNo && o.Active == 1).ToList();
                    foreach (var item in objInwordDetailData)
                    {
                        item.Location = ToLocation;
                        item.ModifiedBy = userId;
                        item.ModifiedOn = new CommonFunctions().ServerDate();
                    }
                    var objStockDetailData = dbcontext.StockDetails.Where(o => o.SerialNumber == inventoryNo && o.FKProjectId == ProjectId && o.Active == 1).ToList();
                    foreach (var item in objStockDetailData)
                    {
                        item.Location = ToLocation;
                        item.ModifiedBy = userId;
                        item.ModifiedOn = new CommonFunctions().ServerDate();
                    }
                    dbcontext.SaveChanges();
                }
                statusMessage.Success = true;
                statusMessage.Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                statusMessage.Success = false;
                statusMessage.Message = "Unable to update record";
                throw ex;
            }
            return statusMessage;
        }

        public StatusMessage TransferInventoryNew(long ProjectId, long warehouseId, long productId, string ToLocation, string[] TransferInventoryList, long userId)
        {
            StatusMessage statusMessage = new StatusMessage();
            CompassEntities dbcontext = new CompassEntities();
            try
            {
                foreach (var inventoryNo in TransferInventoryList)
                {
                    /*var data = (from ind in dbcontext.InwardDetails.AsEnumerable()
                                join inh in dbcontext.InwardHeaders on ind.FKInwardHeaderId equals inh.Id
                                where inh.FKProjectId == ProjectId && inh.FKWarehouseId == warehouseId && ind.Active == 1 && inh.Active == 1 && ind.SerialNumber == inventoryNo
                                select new IndIdList
                                {
                                    IndId = ind.Id
                                }).ToList();*/
                    /*var data = (from ind in dbcontext.InwardDetails.AsEnumerable()
                                join inh in dbcontext.InwardHeaders on ind.FKInwardHeaderId equals inh.Id
                                where inh.FKProjectId == ProjectId && inh.FKWarehouseId == warehouseId && ind.Active == 1
                                && inh.Active == 1 && ind.FKProductId == productId && ind.SerialNumber == inventoryNo
                                select new { IndId = ind.Id }).FirstOrDefault();*/
                    /*foreach (var item in data)
                    {*/
                        //var objInwordDetailData = dbcontext.InwardDetails.Where(o => o.Id == item.IndId).FirstOrDefault();
                        /*var objInwordDetailData = dbcontext.InwardDetails.Where(o => o.Id == data.IndId).FirstOrDefault();
                        objInwordDetailData.Location = ToLocation;
                        objInwordDetailData.ModifiedBy = userId;
                        objInwordDetailData.ModifiedOn = new CommonFunctions().ServerDate();*/
                    //}
                    var objStockDetailData = dbcontext.StockDetails.Where(o => o.FKProjectId == ProjectId && o.FKWarehouseId == warehouseId 
                                             && o.FKProductId == productId && o.Active == 1 && o.SerialNumber == inventoryNo).ToList();
                    foreach (var item in objStockDetailData)
                    {
                        item.Location = ToLocation;
                        item.ModifiedBy = userId;
                        item.ModifiedOn = new CommonFunctions().ServerDate();
                    }
                    //dbcontext.SaveChanges();
                }
                dbcontext.SaveChanges();
                statusMessage.Success = true;
                statusMessage.Message = "Record Updated Successfully";
            }
            catch (Exception ex)
            {
                statusMessage.Success = false;
                statusMessage.Message = "Unable to update record";
                throw ex;
            }
            return statusMessage;
        }
        public class IndIdList
        {
            public long IndId { get; set; }
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
        public StatusMessage DeleteStockRecord(long StockDetailId, long userId) // 
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


                            //var ObjList = dbcontext.StockDetails.Where(o => o.Active == 1 && o.BalanceQty == 0 && o.IsSerialNumber == 1 && o.SerialNumber == "").ToList();
                            // Change this code
                            //var ObjList = dbcontext.StockDetails.Where(o => o.Active == 1 && o.BalanceQty == 0 && o.IsSerialNumber == 1 && o.SerialNumber == stockdetailRecord.SerialNumber).ToList();
                            var ObjList = dbcontext.StockDetails.Where(o => o.BalanceQty == 0 && o.IsSerialNumber == 1 && o.SerialNumber == stockdetailRecord.SerialNumber).ToList();

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
                            /*var inwardDeatil = dbcontext.InwardDetails.Where(a => a.Id == stockdetailRecord.FKInwardDetailId).FirstOrDefault();
                            inwardDeatil.Active = 0;
                            inwardDeatil.ModifiedBy = userId;
                            inwardDeatil.ModifiedOn = new CommonFunctions().ServerDate();*/

                            //Change code
                            var inwardDeatil = dbcontext.InwardDetails.Where(a => a.SerialNumber == stockdetailRecord.SerialNumber && a.Active==1).ToList(); ;
                            foreach (var objInward in inwardDeatil)
                            {
                                objInward.Active = 0;
                                objInward.ModifiedBy = userId;
                                objInward.ModifiedOn = new CommonFunctions().ServerDate();
                            }

                            var serialNumberSummary = dbcontext.SerialNumberSummary.Where(a => a.SerialNumber == stockdetailRecord.SerialNumber && a.Active == 1).ToList();
                            foreach (var objSrNo in serialNumberSummary)
                            {
                                objSrNo.Active = 0;
                            }
                            //end hare
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

        /// <summary>
        /// Deepak
        /// 07-Jan-2021
        /// Delete record from stock and InwardDetails
        /// Delete record from stock and Stock detail
        /// Delete record from stock and OutwardDetails
        /// </summary>
        /// <param name="StockDetailId"></param>
        /// <returns></returns>
        public StatusMessage DeleteStockRecord_new(long StockDetailId, long userId)
        {
            StatusMessage statusMessage = new StatusMessage();
            CompassEntities dbcontext = new CompassEntities();
            try
            {
                var stockdetailRecord = dbcontext.StockDetails.Where(a => a.Id == StockDetailId && a.Active == 1).FirstOrDefault();
                if (stockdetailRecord != null)
                {
                    var stockdetailAllRecord = dbcontext.StockDetails.Where(a => a.SerialNumber == stockdetailRecord.SerialNumber);
                    foreach (var sr in stockdetailAllRecord)    // sr=serial number
                    {
                        var OutwardDetailsAllRecord = dbcontext.OutwardDetails.Where(a => a.FKStockDetailsID == sr.Id);
                        foreach (var odr in OutwardDetailsAllRecord)    // odr=outword details records
                        {
                            var tblProjectFieldDataInventoryOutwardDetails = dbcontext.tblProjectFieldDataInventoryOutwardDetails.Where(a => a.FKOutwardDetailsId == odr.Id);
                            dbcontext.tblProjectFieldDataInventoryOutwardDetails.RemoveRange(tblProjectFieldDataInventoryOutwardDetails);
                            var tblProjectFieldDataInventoryDetails = dbcontext.tblProjectFieldDataInventoryDetails.Where(a => a.SerialNumber == stockdetailRecord.SerialNumber);
                            dbcontext.tblProjectFieldDataInventoryDetails.RemoveRange(tblProjectFieldDataInventoryDetails);
                        }
                        dbcontext.OutwardDetails.RemoveRange(OutwardDetailsAllRecord);
                    }
                    dbcontext.StockDetails.RemoveRange(stockdetailAllRecord);

                    var inwardDeatil = dbcontext.InwardDetails.Where(a => a.SerialNumber == stockdetailRecord.SerialNumber);
                    dbcontext.InwardDetails.RemoveRange(inwardDeatil);

                    var serialNumberSummary = dbcontext.SerialNumberSummary.Where(a => a.SerialNumber == stockdetailRecord.SerialNumber);
                    dbcontext.SerialNumberSummary.RemoveRange(serialNumberSummary);

                    dbcontext.SaveChanges();
                    statusMessage.Success = true;
                    statusMessage.Message = "Record Deleted Successfully";
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
        public StatusMessage TransferInventoryToInstaller(
    long ProjectId,
    long warehouseId,
    long productId,
    long InstallerTo,
    string[] TransferInventoryList,
    long userId)
        {
            StatusMessage statusMessage = new StatusMessage();
            CompassEntities dbcontext = new CompassEntities();

            try
            {
                var requestDetail = dbcontext.InventoryMobileStockRequestDetails
                    .FirstOrDefault(rd => rd.Active == 1
                                       && rd.InventoryMobileStockRequest.Active == 1
                                       && rd.InventoryMobileStockRequest.FKProjectId == ProjectId
                                       && rd.FKProductId == productId);

                InventoryMobileStockRequestDetail activeDetailObject = requestDetail;

                foreach (var serial in TransferInventoryList)
                {
                    var stockItem = dbcontext.StockDetails
                        .FirstOrDefault(s => s.FKProjectId == ProjectId
                                          && s.FKWarehouseId == warehouseId
                                          && s.FKProductId == productId
                                          && s.Active == 1
                                          && s.SerialNumber == serial);

                    if (stockItem == null) continue;

                    if (activeDetailObject == null)
                    {
                        var newRequest = new InventoryMobileStockRequest
                        {
                            DeviceDateTime = new CommonFunctions().ServerDate(),
                            JsonRequestData = "System Generated Bulk Transfer Request",
                            GPSLocation = "",
                            DeviceIP = "",
                            FKProjectId = ProjectId,
                            Active = 1,
                            CreatedBy = userId,
                            CreatedOn = new CommonFunctions().ServerDate()
                        };

                        var newDetail = new InventoryMobileStockRequestDetail
                        {
                            InventoryMobileStockRequest = newRequest,
                            FKProductId = productId,
                            IsSerialNumber = !string.IsNullOrEmpty(serial),
                            SerialNumber = serial,
                            Quantity = 1,
                            FKInventoryStatusMasterId = 1,
                            AddedInStock = false,
                            AddedInOutwardDetails = true,
                            Active = 1,
                            CreatedBy = userId,
                            CreatedOn = new CommonFunctions().ServerDate()
                        };

                        dbcontext.InventoryMobileStockRequestDetails.Add(newDetail);
                        activeDetailObject = newDetail;
                    }

                    // Record allocation in OutwardDetails
                    OutwardDetail od = new OutwardDetail
                    {
                        FKStockDetailsID = stockItem.Id,
                        AllocatedTo = InstallerTo,
                        AllocatedQty = 1,
                        InstalledQty = 0,
                        TransferredQty = 0,
                        ReturnToWarehouseQty = 0,
                        RemovedQty = 0,
                        AllStockUtilized = true,
                        Active = 1,
                        CreatedBy = userId,
                        CreatedOn = new CommonFunctions().ServerDate()
                    };

                    if (activeDetailObject.Id > 0)
                    {
                        od.FKInventoryMobileStockRequestDetailId = activeDetailObject.Id;
                    }
                    else
                    {
                        od.InventoryMobileStockRequestDetail = activeDetailObject;
                    }

                    dbcontext.OutwardDetails.Add(od);

                    stockItem.IsAllocated = true;
                    stockItem.AllStockUtilized = true;
                    stockItem.BalanceQty = 0;
                    stockItem.ModifiedBy = userId;
                    stockItem.ModifiedOn = new CommonFunctions().ServerDate();
                }

                dbcontext.SaveChanges();

                statusMessage.Success = true;
                statusMessage.Message = "Inventory successfully transferred to installer.";
            }
            catch (Exception ex)
            {
                statusMessage.Success = false;
                statusMessage.Message = "Error transferring inventory: " + ex.Message;
            }

            return statusMessage;
        }
    }

}
