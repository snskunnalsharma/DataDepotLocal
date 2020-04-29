using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.DAL
{
    public class WebServiceProductDAL : IDisposable
    {

        bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        ///// <summary>
        ///// returns productlist
        ///// </summary>
        ///// <param name="utilityId"></param>
        ///// <returns></returns>
        //public List<MobileProductBO> GetProductList(long utilityId)
        //{

        //    List<MobileProductBO> objMobileProductBOList = new List<MobileProductBO>();


        //    try
        //    {
        //        CompassEntities objCompassEntities = new CompassEntities();

        //        objMobileProductBOList = objCompassEntities.VW_GetProductData.Where(o => o.UtilityId == utilityId).Select(o =>
        //                new MobileProductBO
        //                {
        //                    Id = o.Id,
        //                    ProductName = o.ProductName,
        //                    PartNumber = o.PartNumber,
        //                    ProductHasSerialNumber = o.SerialNumberRequired,
        //                    UPCCode = o.UPCCode,
        //                    Make = o.Make,
        //                    Type = o.MeterType,
        //                    Size = o.MeterSize
        //                }
        //                ).ToList();

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }


        //    return objMobileProductBOList;
        //}

        /// <summary>
        /// returns productlist
        /// </summary>
        /// <param name="utilityId"></param>
        /// <returns></returns>
        public List<MobileProductBO> GetProductList(long projectId)
        {

            List<MobileProductBO> objMobileProductBOList = new List<MobileProductBO>();


            try
            {
                CompassEntities objCompassEntities = new CompassEntities();

                var a = objCompassEntities.VW_GetProductDataProject.Where(o => o.FKProjectID == projectId).Select(o =>
                        new MobileProductBO
                        {
                            Id = o.Id,
                            ProductName = o.ProductName,
                            PartNumber = o.PartNumber,
                            ProductHasSerialNumber = o.SerialNumberRequired,
                            UPCCode = o.UPCCode,
                            Make = o.Make,
                            Type = o.MeterType,
                            Size = o.MeterSize
                        }
                        ).ToList();

                if (a != null)
                {
                    foreach (var item in a)
                    {
                        var objCheck = objMobileProductBOList.Where(o => o.Id == item.Id).FirstOrDefault();
                        if (objCheck == null)
                        {
                            objMobileProductBOList.Add(item);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }


            return objMobileProductBOList;
        }


        public List<MobileWarehouseBO> GetWarehouseListNew(long projectId)
        {
            List<MobileWarehouseBO> objMobileWarehouseBOList = new List<MobileWarehouseBO>();


            try
            {
                CompassEntities objCompassEntities = new CompassEntities();

                List<long> listObj = objCompassEntities.ProjectWarehouseRelations.AsNoTracking().Where(o => o.Active == 1 && o.FKProjectId == projectId).Select(o => o.FKWarehouseId).ToList();
                objMobileWarehouseBOList = objCompassEntities.WarehouseMasters.Where(o => o.Active == 1 && listObj.Contains(o.Id)).Select(o =>
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


        public List<MobileUtilityType> GetUtilityTypeList()
        {
            List<MobileUtilityType> objMobileUtilityTypeList = new List<MobileUtilityType>();


            try
            {
                CompassEntities objCompassEntities = new CompassEntities();

                objMobileUtilityTypeList = objCompassEntities.UtilityMasters.Where(o => o.Active == 1).Select(o =>
                        new MobileUtilityType
                        {
                            Id = o.Id,
                            UtilityName = o.UtilityName
                        }
                        ).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }


            return objMobileUtilityTypeList;
        }


        public WebServiceProductBO GetAllSourceInfoForProduct(long utlityId)
        {

            WebServiceProductBO ObjWebServiceProductBO = new WebServiceProductBO();

            try
            {
                CompassEntities objCompassEntities = new CompassEntities();

                ObjWebServiceProductBO.UtilityList = objCompassEntities.UtilityMasters.Where(o => o.Active == 1).Select(o =>
                      new WebServiceUtilityBO
                      {
                          UtilityId = o.Id,
                          UtilityName = o.UtilityName
                      }
                      ).ToList();

                ObjWebServiceProductBO.CategoryList = objCompassEntities.Categories.Where(o => o.Active == 1).Select(o =>
                        new WebServiceCategoryBO
                        {
                            CategoryId = o.CategoryId,
                            CategoryName = o.CategoryName
                        }
                        ).ToList();

                ObjWebServiceProductBO.MakeList = objCompassEntities.tblMeterMakes.Where(o => o.Active == 1).Select(o =>
                      new WebServiceMakeBO
                      {
                          MakeId = o.ID,
                          MakeName = o.Make
                      }
                      ).ToList();

                ObjWebServiceProductBO.TypeList = objCompassEntities.tblMeterTypes.Where(o => o.Active == 1).Select(o =>
                      new WebServiceTypeBO
                      {
                          TypeId = o.ID,
                          TypeName = o.MeterType
                      }
                      ).ToList();

                ObjWebServiceProductBO.SizeList = objCompassEntities.tblMeterSizes.Where(o => o.Active == 1).Select(o =>
                      new WebServiceSizeBO
                      {
                          SizeId = o.ID,
                          SizeName = o.MeterSize
                      }
                      ).ToList();

                ObjWebServiceProductBO.ManagerList = new List<WebUserListBO>();
                var manager = new ApplicationDbContext.UserManager();

                var UseNames = manager.GetUsersInRole("Manager");

                if (UseNames != null)
                {
                    foreach (var item in UseNames)
                    {
                        var record = objCompassEntities.tblUsers.Where(o => o.Active == 1 && o.aspnet_UserId.ToString() == item.Id.ToString()).
                            Select(o => new WebUserListBO { UserId = o.UserID, UserName = o.FirstName }).FirstOrDefault();
                        if (record != null)
                        {
                            ObjWebServiceProductBO.ManagerList.Add(record);
                        }
                    }
                    ObjWebServiceProductBO.ManagerList = ObjWebServiceProductBO.ManagerList.OrderBy(o => o.UserName).ToList();
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ObjWebServiceProductBO;

        }




    }
}
