using AutoMapper;
using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.IO;

namespace GarronT.CMS.Model.DAL
{
    public class InventoryDAL : IDisposable
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



        /// <summary>
        /// 
        /// </summary>
        /// <param name="UtilityId"></param>
        /// <returns></returns>
        public List<ProjectModel> GetProjectName(int UtilityId)
        {
            CompassEntities CompassEntities = new CompassEntities();

            string s = CompassEntities.UtilityMasters.Where(o => o.Id == UtilityId).Select(o => o.UtilityName).FirstOrDefault();
            List<ProjectModel> data = CompassEntities.STP_GetProjectCityState().Where(a => a.Active == 1 && a.Utilitytype == s
                && a.ProjectStatus == "Active").Select(o =>
                                       new ProjectModel
                                       {
                                           ProjectId = o.ProjectId,
                                           ProjectName = o.ProjectCityName
                                       }).OrderBy(a => a.ProjectCityState).ToList();
            return data;
        }

        public List<ProjectModel> GetInventoryProjectName(int UtilityId)
        {
            CompassEntities CompassEntities = new CompassEntities();

            string s = CompassEntities.UtilityMasters.Where(o => o.Id == UtilityId).Select(o => o.UtilityName).FirstOrDefault();
            List<ProjectModel> data = CompassEntities.STP_GetProjectCityState().Where(a => a.Active == 1 && a.Utilitytype == s && a.ManageInventory == "Yes"
                && a.ProjectStatus == "Active").Select(o =>
                                       new ProjectModel
                                       {
                                           ProjectId = o.ProjectId,
                                           ProjectName = o.ProjectCityName
                                       }).OrderBy(a => a.ProjectCityState).ToList();
            return data;
        }


        public long GetUtilityForProject(long projectId)
        {
            CompassEntities CompassEntities = new CompassEntities();

            var s = CompassEntities.tblProjects.Where(o => o.ProjectId == projectId).Select(o => o.Utilitytype).First();
            var UtilityId = CompassEntities.UtilityMasters.Where(o => o.UtilityName == s).Select(o => o.Id).FirstOrDefault();
            return UtilityId;
        }


        public List<UsersSmallModel> GetProjectInstallers(long projectId)
        {
            using (CompassEntities objCompassEntities = new CompassEntities())
            {
                WebServiceUserRoleDAL objWebServiceUserRoleDAL = new WebServiceUserRoleDAL();

                // Get all users with the "Installer" role
                var result = objWebServiceUserRoleDAL.GetAllUsersBasedOnRoleWithSelf("Installer");

                var installersList = new List<UsersSmallModel>();
                if (result != null && result.Count > 0)
                {
                    installersList = result.Select(o => new UsersSmallModel { UserId = o.ChildId, UserName = o.ChildName }).ToList();
                }

                // Get the list of installers associated with the current project
                var currentProjectUserIds = objCompassEntities.TblProjectInstallers
                    .Where(o => o.ProjectID == projectId)
                    .Select(o => o.tblUsers_UserID)
                    .ToList();

                // Filter the installers based on the current project
                installersList = installersList
                    .Where(o => currentProjectUserIds.Contains(o.UserId))
                    .ToList();

                // Additional filtering based on projectId using UserRoleDAL
                List<UsersSmallModel> filteredInstallers = new List<UsersSmallModel>();
                if (projectId > 0)
                {
                    using (UserRoleDAL obj = new UserRoleDAL())
                    {
                        filteredInstallers = obj.GeInstallerListCurrentProject(projectId).OrderBy(a => a.UserName).ToList();
                    }
                }

                // Further filter installersList based on the filteredInstallers
                installersList = installersList
                    .Where(o => filteredInstallers.Any(f => f.UserId == o.UserId))
                    .ToList();

                return installersList;
            }
        }





        /// <summary>
        /// Returns active project list. 
        /// </summary>
        /// <param name="proStatus">Active/Complete</param>
        /// <returns>Project List</returns>
        public List<ProjectModel> GetProjectName(string proStatus)
        {
            CompassEntities CompassEntities = new CompassEntities();

            List<ProjectModel> data = CompassEntities.STP_GetProjectCityState().Where(a => a.Active == 1 && a.ProjectStatus == proStatus).Select(o =>
                                       new ProjectModel
                                       {
                                           ProjectId = o.ProjectId,
                                           ProjectCityState = o.ProjectCityName
                                       }).OrderBy(a => a.ProjectCityState).ToList();
            return data;
        }

        public List<ProjectModel> GetProjectList()
        {
            CompassEntities CompassEntities = new CompassEntities();

            List<ProjectModel> data = CompassEntities.STP_GetProjectCityState().Where(a => a.Active == 1).Select(o =>
                                       new ProjectModel
                                       {
                                           ProjectId = o.ProjectId,
                                           ProjectCityState = o.ProjectCityName
                                       }).OrderBy(a => a.ProjectCityState).ToList();
            return data;
        }

        public List<UsersModel> GetInstallerList()
        {

            CompassEntities dbcontext = new CompassEntities();
            List<UsersModel> UsersModels = (from u in dbcontext.tblUsers.AsNoTracking()
                                            where u.Active == 1
                                            select new UsersModel()
                                            {
                                                UserId = u.UserID,
                                                UserFullName = u.FirstName

                                            }).ToList();
            return UsersModels;
        }

        public List<CategoryBO> GetCategoryList()
        {

            CompassEntities dbcontext = new CompassEntities();
            List<CategoryBO> categoryBO = (from u in dbcontext.Categories.AsNoTracking()
                                           where u.Active == 1
                                           select new CategoryBO()
                                           {

                                               CategoryId = u.CategoryId,
                                               CategoryName = u.CategoryName

                                           }).ToList();
            return categoryBO;
        }


        public List<WarehouseBO> GetWareHouseList()
        {
            CompassEntities CompassEntities = new CompassEntities();

            List<WarehouseBO> data = CompassEntities.WarehouseMasters.Where(a => a.Active == 1).Select(o =>
                                     new WarehouseBO
                                     {
                                         WarehouseId = o.Id,
                                         WarehouseName = o.WarehouseLocation + ", " + o.WarehouseName + ", " + o.TblCityMaster.CityName + ", " + o.tblStateMaster.StateName
                                     }).OrderBy(a => a.WarehouseName).ToList();
            return data;
        }

        public List<WarehouseBO> GetWareHouseList(long ProjectId)
        {
            CompassEntities CompassEntities = new CompassEntities();

            List<long> ObjList = CompassEntities.ProjectWarehouseRelations.Where(o => o.Active == 1 && o.FKProjectId == ProjectId).Select(o => o.FKWarehouseId).ToList();

            List<WarehouseBO> data = CompassEntities.WarehouseMasters.Where(a => a.Active == 1 && ObjList.Contains(a.Id)).Select(o =>
                                       new WarehouseBO
                                       {
                                           WarehouseId = o.Id,
                                           WarehouseName = o.WarehouseLocation + ", " + o.WarehouseName + ", " + o.TblCityMaster.CityName + ", " + o.tblStateMaster.StateName
                                       }).OrderBy(a => a.WarehouseName).ToList();
            return data;
        }


        public List<CommonDropDownClass> GetProductList(long ProjectId)
        {
            CompassEntities CompassEntities = new CompassEntities();

            List<long> ObjList = CompassEntities.ProjectServiceProductRelations.Where(o => o.Active == 1 && o.FKProjectID == ProjectId).Select(o => o.FKProductId).ToList();

            List<CommonDropDownClass> data = CompassEntities.ProductMasters.Where(a => a.Active == 1 && ObjList.Contains(a.Id)).Select(o =>
                                       new CommonDropDownClass
                                       {
                                           Id = o.Id,
                                           FieldValue = o.ProductName
                                       }).OrderBy(a => a.FieldValue).ToList();
            return data;
        }

        public List<LocationDropdownList> GetLocationList(long ProjectId)
        {
            CompassEntities CompassEntities = new CompassEntities();

            List<LocationDropdownList> data = CompassEntities.StockDetails.Where(p => p.FKProjectId == ProjectId && p.Active == 1 && p.Location != "" && p.Location != null).OrderBy(o => o.Location).Select(o =>
                                                  new LocationDropdownList
                                                  {
                                                      LocationId = o.Location,
                                                      Location = o.Location,
                                                      ProductId = o.FKProductId
                                                  }).Distinct().ToList();
            return data;
        }

        public List<InventoryUtilityTypeBO> GetUtilityTypeList()
        {
            CompassEntities CompassEntities = new CompassEntities();

            List<InventoryUtilityTypeBO> data = CompassEntities.UtilityMasters.Where(a => a.Active == 1).Select(o =>
                                       new InventoryUtilityTypeBO
                                       {
                                           Id = o.Id,
                                           UtilityName = o.UtilityName
                                       }).OrderBy(a => a.UtilityName).ToList();
            return data;
        }

        public ProductMasterBO GetProductbyId(long id)
        {
            CompassEntities CompassEntities = new CompassEntities();
            try
            {
                ProductMasterBO data = CompassEntities.VW_GetProductData.Where(a => a.Id == id).Select(o =>
                                            new ProductMasterBO
                                            {
                                                Id = o.Id,
                                                ProductName = o.ProductName,
                                                ProductDescription = o.ProductDescription,
                                                ProductPartNumber = o.PartNumber,
                                                ProductUPCCode = o.UPCCode,
                                                UtilityId = o.UtilityId.Value,
                                                UtilityName = o.UtilityName,
                                                CategoryName = o.CategoryName,
                                                Make = o.Make,
                                                Size = o.MeterSize,
                                                Type = o.MeterType,
                                                SerialNumberRequired = o.SerialNumberRequired,
                                            }).FirstOrDefault();
                return data;
            }
            catch (Exception)
            {

                throw;
            }

        }


        public List<ProductMasterBO> GetProductList()
        {
            CompassEntities CompassEntities = new CompassEntities();

            List<ProductMasterBO> data = CompassEntities.ProductMasters.Where(a => a.Active == 1).Select(o =>
                                       new ProductMasterBO
                                       {
                                           Id = o.Id,
                                           ProductName = o.ProductName
                                       }).OrderBy(a => a.ProductName).ToList();
            return data;
        }

        public List<ProductMasterBO> GetProductMakeTypeList()
        {
            CompassEntities CompassEntities = new CompassEntities();

            List<ProductMasterBO> data = (from p in CompassEntities.ProductMasters
                                          join m in CompassEntities.tblMeterMakes on p.MeterMakeId equals m.ID into ps
                                          from m in ps.DefaultIfEmpty()
                                          join t in CompassEntities.tblMeterTypes on p.MeterTypeId equals t.ID into ps1
                                          from t in ps1.DefaultIfEmpty()
                                          join s in CompassEntities.tblMeterSizes on p.MeterSizeId equals s.ID into ps2
                                          from s in ps2.DefaultIfEmpty()
                                          where p.Active == 1 //&& m.Active == 1 && t.Active == 1 && s.Active == 1
                                          select new ProductMasterBO
                                          {
                                              Id = p.Id,
                                              ProductName = p.ProductName + ((m.Make == "" || m.Make == null) ? "" : "," + m.Make) + ((t.MeterType == "" || t.MeterType == null) ? "" : "," + t.MeterType) + ((s.MeterSize == "" || s.MeterSize == null) ? "" : "," + s.MeterSize)
                                          }).ToList();
            return data;
        }


        public List<ProductMasterBO> GetProductMakeTypeListbyCategoryId(long categoryId)
        {
            CompassEntities CompassEntities = new CompassEntities();

            List<ProductMasterBO> data = new List<ProductMasterBO>();
            if (categoryId == 0)
            {
                data = (from p in CompassEntities.ProductMasters
                        join m in CompassEntities.tblMeterMakes on p.MeterMakeId equals m.ID into ps
                        from m in ps.DefaultIfEmpty()
                        join t in CompassEntities.tblMeterTypes on p.MeterTypeId equals t.ID into ps1
                        from t in ps1.DefaultIfEmpty()
                        join s in CompassEntities.tblMeterSizes on p.MeterSizeId equals s.ID into ps2
                        from s in ps2.DefaultIfEmpty()
                        where p.Active == 1 //&& m.Active == 1 && t.Active == 1 && s.Active == 1
                        select new ProductMasterBO
                        {
                            Id = p.Id,
                            ProductName = p.ProductName + ((m.Make == "" || m.Make == null) ? "" : "," + m.Make) + ((t.MeterType == "" || t.MeterType == null) ? "" : "," + t.MeterType) + ((s.MeterSize == "" || s.MeterSize == null) ? "" : "," + s.MeterSize)
                        }).ToList();
            }
            else
            {
                data = (from p in CompassEntities.ProductMasters
                        join m in CompassEntities.tblMeterMakes on p.MeterMakeId equals m.ID into ps
                        from m in ps.DefaultIfEmpty()
                        join t in CompassEntities.tblMeterTypes on p.MeterTypeId equals t.ID into ps1
                        from t in ps1.DefaultIfEmpty()
                        join s in CompassEntities.tblMeterSizes on p.MeterSizeId equals s.ID into ps2
                        from s in ps2.DefaultIfEmpty()
                        where p.Active == 1 && p.ProductCategoryId == categoryId //&& m.Active == 1 && t.Active == 1 && s.Active == 1 
                        select new ProductMasterBO
                        {
                            Id = p.Id,
                            ProductName = p.ProductName + ((m.Make == "" || m.Make == null) ? "" : "," + m.Make) + ((t.MeterType == "" || t.MeterType == null) ? "" : "," + t.MeterType) + ((s.MeterSize == "" || s.MeterSize == null) ? "" : "," + s.MeterSize)
                        }).ToList();
            }

            return data;
        }

        /// <summary>
        /// 21-09-2016
        /// Bharat
        /// Saves excel data & returns saved data from the table
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="excelDT"></param>
        /// <returns></returns>
        public List<InventoryUploadBO> SaveInvetoryExcel(DataTable excelDT, long WarehouseId, long currentUserId)
        {
            List<InventoryUploadBO> objInventoryUploadList = new List<InventoryUploadBO>();
            CompassEntities objCompassEntities = new CompassEntities();
            DateTime currentDate = new CommonFunctions().ServerDate();

            var UploadId = objCompassEntities.InventoryUploads.Select(o => o.UploadNumber).FirstOrDefault();
            if (UploadId != null && UploadId > 0)
            {
                UploadId = objCompassEntities.InventoryUploads.Max(o => o.UploadNumber);
            }
            UploadId = (UploadId == null && UploadId == 0) ? 1 : UploadId + 1;

            //save info in for loop
            for (int i = 0; i < excelDT.Rows.Count; i++)
            {
                if (Convert.ToString(excelDT.Rows[i]["Serial Number"]) != "")
                {
                    InventoryUpload objInventoryUpload = new InventoryUpload();
                    objInventoryUpload.Active = 1;
                    objInventoryUpload.CreatedOn = currentDate;
                    objInventoryUpload.CreatedBy = currentUserId;

                    objInventoryUpload.UploadNumber = UploadId;
                    objInventoryUpload.FKWarehouseId = WarehouseId;
                    objInventoryUpload.IsDuplicate = false;
                    objInventoryUpload.ExcelUpload = 1;

                    objInventoryUpload.SerialNo = Convert.ToString(excelDT.Rows[i]["Serial Number"]);
                    objInventoryUpload.MeterType = Convert.ToString(excelDT.Rows[i]["Meter Type"]);
                    objInventoryUpload.MeterSize = Convert.ToString(excelDT.Rows[i]["Meter Size"]);
                    objInventoryUpload.MeterMake = Convert.ToString(excelDT.Rows[i]["Meter Make"]);
                    objInventoryUpload.Quantity = 1;

                    objCompassEntities.InventoryUploads.Add(objInventoryUpload);
                    objCompassEntities.SaveChanges();
                }
                else
                {
                    if (Convert.ToString(excelDT.Rows[i]["Description"]) != "" && Convert.ToString(excelDT.Rows[i]["Quantity"]) != "")
                    {
                        InventoryUpload objInventoryUpload = new InventoryUpload();
                        objInventoryUpload.Active = 1;
                        objInventoryUpload.CreatedOn = currentDate;
                        objInventoryUpload.CreatedBy = currentUserId;

                        objInventoryUpload.UploadNumber = UploadId;
                        objInventoryUpload.FKWarehouseId = WarehouseId;
                        objInventoryUpload.IsDuplicate = false;
                        objInventoryUpload.ExcelUpload = 1;

                        objInventoryUpload.Description = Convert.ToString(excelDT.Rows[i]["Description"]);
                        objInventoryUpload.Quantity = int.Parse(Convert.ToString(excelDT.Rows[i]["Quantity"]));


                        objCompassEntities.InventoryUploads.Add(objInventoryUpload);
                        objCompassEntities.SaveChanges();
                    }
                }
            }

            //objCompassEntities.PROC_UpdateDuplicateFlag(projectId);

            objCompassEntities.PROC_InventoryUpload_UpdateDuplicateFlag(UploadId);

            // objCompassEntities = new CompassEntities();


            //get all project inventory data
            objInventoryUploadList = objCompassEntities.InventoryUploads.AsNoTracking().Where(o => o.Active == 1 && o.UploadNumber == UploadId)
                .Select(o =>
                    new InventoryUploadBO
                    {
                        MeterType = o.MeterType,
                        MeterSize = o.MeterSize,
                        Id = o.Id,
                        SerialNo = o.SerialNo,
                        IsDuplicate = (o.IsDuplicate.Value == true ? "Yes" : "No"),
                        MeterMake = o.MeterMake,
                        Description = o.Description,
                        UploadNumber = o.UploadNumber,
                        Quantity = o.Quantity

                    }).ToList();

            return objInventoryUploadList;
        }




        /// <summary>
        /// 21-09-2016
        /// Bharat
        /// returns saved data from the table for the project
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="excelDT"></param>
        /// <returns></returns>
        public List<InventoryUploadBO> GetInvetoryData(long projectId)
        {
            List<InventoryUploadBO> objInventoryUploadList = new List<InventoryUploadBO>();
            CompassEntities objCompassEntities = new CompassEntities();

            objInventoryUploadList = objCompassEntities.InventoryUploads.Where(o => o.Active == 1)
                .Select(o => new InventoryUploadBO { MeterType = o.MeterType, MeterSize = o.MeterSize, Id = o.Id, SerialNo = o.SerialNo, IsDuplicate = (o.IsDuplicate.Value == true ? "Yes" : "No") }).ToList();

            return objInventoryUploadList;
        }



        /// <summary>
        /// 21-09-2016
        /// Bharat
        /// returns the current row for Id
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public InventoryUpload GetInvetoryCurrentRecord(long recordId)
        {
            InventoryUpload objInventoryUpload = new InventoryUpload();
            CompassEntities objCompassEntities = new CompassEntities();

            objInventoryUpload = objCompassEntities.InventoryUploads.Where(o => o.Id == recordId).FirstOrDefault();

            return objInventoryUpload;
        }



        /// <summary>
        /// 21-09-2016
        /// Bharat
        /// saves the current row info
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="metersize"></param>
        /// <param name="meterType"></param>
        /// <param name="serialNo"></param>
        /// <param name="resultMessage"></param>
        /// <returns></returns>
        public bool SaveInvetoryCurrentRecord(long recordId, string metersize, string meterType, string serialNo, out string resultMessage)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            var objInventoryUpload = objCompassEntities.InventoryUploads.Where(o => o.Id == recordId).FirstOrDefault();
            var checkRecordPresent = objCompassEntities.InventoryUploads.Where(o => o.Id != recordId && o.SerialNo == serialNo
                ).Count();
            if (checkRecordPresent == 0)
            {
                objInventoryUpload.IsDuplicate = false;
                objInventoryUpload.MeterSize = metersize;
                objInventoryUpload.MeterType = meterType;
                objInventoryUpload.SerialNo = serialNo;
                objCompassEntities.SaveChanges();
                //  objCompassEntities.PROC_UpdateDuplicateFlag(objInventoryUpload.FK_InventoryUpload_Project);
                resultMessage = "Sucessfully updated the record.";
                return true;
            }
            else
            {
                resultMessage = "Serial number already in use. Please enter correct serial number.";
                return true;
            }
        }



        /// <summary>
        /// 04-Jul-2017
        /// Bharat
        /// Saves excel data & returns saved data from the table
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="excelDT"></param>
        /// <returns></returns>
        //public List<UploadInventoryBO> SaveInvetoryFromExcel(DataTable excelDT, long WarehouseId, long currentUserId)
        //{

        //    List<UploadInventoryBO> objUploadInventoryBOList = new List<UploadInventoryBO>();
        //    CompassEntities objCompassEntities = new CompassEntities();
        //    DateTime currentDate = new CommonFunctions().ServerDate();

        //    var UploadId = objCompassEntities.InventoyExcelUploads.Select(o => o.UploadNumber).FirstOrDefault();

        //    if (UploadId != null && UploadId > 0)
        //    {
        //        UploadId = objCompassEntities.InventoyExcelUploads.Max(o => o.UploadNumber);
        //    }

        //    UploadId = (UploadId == null && UploadId == 0) ? 1 : UploadId + 1;

        //    //save info in for loop
        //    for (int i = 0; i < excelDT.Rows.Count; i++)
        //    {

        //        if (!string.IsNullOrEmpty(Convert.ToString(excelDT.Rows[i]["Serial Number"])))
        //        {
        //            InventoyExcelUpload objInventoryUpload = new InventoyExcelUpload();
        //            objInventoryUpload.Active = 1;
        //            objInventoryUpload.CreatedOn = currentDate;
        //            objInventoryUpload.CreatedBy = currentUserId;

        //            objInventoryUpload.UploadNumber = UploadId;
        //            objInventoryUpload.FKWarehouseId = WarehouseId;
        //            objInventoryUpload.IsDuplicate = false;


        //            objInventoryUpload.UtilityType = Convert.ToString(excelDT.Rows[i]["Utility Type"]);
        //            objInventoryUpload.ProductName = Convert.ToString(excelDT.Rows[i]["Product Name"]);
        //            objInventoryUpload.PartNumber = Convert.ToString(excelDT.Rows[i]["Part Number"]);
        //            objInventoryUpload.UPCCode = Convert.ToString(excelDT.Rows[i]["UPC Code"]);
        //            objInventoryUpload.SerialNumber = Convert.ToString(excelDT.Rows[i]["Serial Number"]);
        //            objInventoryUpload.Make = Convert.ToString(excelDT.Rows[i]["Make"]);
        //            objInventoryUpload.Size = Convert.ToString(excelDT.Rows[i]["Size"]);
        //            objInventoryUpload.Type = Convert.ToString(excelDT.Rows[i]["Type"]);

        //            objInventoryUpload.Quantity = int.Parse(Convert.ToString(excelDT.Rows[i]["Quantity"]));

        //            objCompassEntities.InventoyExcelUploads.Add(objInventoryUpload);
        //            objCompassEntities.SaveChanges();
        //        }
        //        else
        //        {

        //            InventoyExcelUpload objInventoryUpload = new InventoyExcelUpload();
        //            objInventoryUpload.Active = 1;
        //            objInventoryUpload.CreatedOn = currentDate;
        //            objInventoryUpload.CreatedBy = currentUserId;

        //            objInventoryUpload.UploadNumber = UploadId;
        //            objInventoryUpload.FKWarehouseId = WarehouseId;
        //            objInventoryUpload.IsDuplicate = false;


        //            objInventoryUpload.UtilityType = Convert.ToString(excelDT.Rows[i]["Utility Type"]);
        //            objInventoryUpload.ProductName = Convert.ToString(excelDT.Rows[i]["Product Name"]);
        //            objInventoryUpload.PartNumber = Convert.ToString(excelDT.Rows[i]["Part Number"]);
        //            objInventoryUpload.UPCCode = Convert.ToString(excelDT.Rows[i]["UPC Code"]);
        //            objInventoryUpload.Make = Convert.ToString(excelDT.Rows[i]["Make"]);
        //            objInventoryUpload.Size = Convert.ToString(excelDT.Rows[i]["Size"]);
        //            objInventoryUpload.Type = Convert.ToString(excelDT.Rows[i]["Type"]);

        //            objInventoryUpload.Quantity = int.Parse(Convert.ToString(excelDT.Rows[i]["Quantity"]));

        //            objCompassEntities.InventoyExcelUploads.Add(objInventoryUpload);
        //            objCompassEntities.SaveChanges();

        //        }
        //    }

        //    //objCompassEntities.PROC_UpdateDuplicateFlag(projectId);

        //    // objCompassEntities.PROC_InventoryUpload_UpdateDuplicateFlag(UploadId);

        //    // objCompassEntities = new CompassEntities();


        //    //get all project inventory data
        //    objUploadInventoryBOList = objCompassEntities.InventoyExcelUploads.AsNoTracking().Where(o => o.Active == 1 && o.UploadNumber == UploadId)
        //        .Select(o =>
        //            new UploadInventoryBO
        //            {
        //                UploadNumber = o.UploadNumber,
        //                Id = o.Id,
        //                UtilityType = o.UtilityType,
        //                ProductName = o.ProductName,
        //                PartNumber = o.PartNumber,
        //                UPCCode = o.UPCCode,
        //                Make = o.Make,
        //                Type = o.Type,
        //                Size = o.Size,
        //                SerialNumber = o.SerialNumber,
        //                IsDuplicate = (o.IsDuplicate.Value == true ? "Yes" : "No"),
        //                Quantity = o.Quantity

        //            }).ToList();

        //    return objUploadInventoryBOList;
        //}




        /// <summary>
        /// 21-09-2016
        /// Bharat
        /// returns the current row for Id
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        //public UploadInventoryBO GetInvetoryRecord(long recordId)
        //{
        //    UploadInventoryBO objUploadInventoryBO = new UploadInventoryBO();
        //    CompassEntities objCompassEntities = new CompassEntities();

        //    var objInventoyExcelUpload = objCompassEntities.InventoyExcelUploads.Where(o => o.Id == recordId).FirstOrDefault();
        //    var objProduct = objCompassEntities.ProductMasters.Where(o => o.ProductName == objInventoyExcelUpload.ProductName).FirstOrDefault();
        //    objUploadInventoryBO.Id = objInventoyExcelUpload.Id;
        //    objUploadInventoryBO.UtilityId = objCompassEntities.UtilityMasters.Where(o => o.UtilityName == objInventoyExcelUpload.UtilityType).Select(o => o.Id).FirstOrDefault();
        //    objUploadInventoryBO.ProductId = objProduct.Id;
        //    objUploadInventoryBO.PartNumber = objInventoyExcelUpload.PartNumber;
        //    objUploadInventoryBO.UPCCode = objInventoyExcelUpload.UPCCode;
        //    objUploadInventoryBO.Make = objInventoyExcelUpload.Make;
        //    objUploadInventoryBO.Type = objInventoyExcelUpload.Type;
        //    objUploadInventoryBO.Size = objInventoyExcelUpload.Size;
        //    objUploadInventoryBO.SerialNumber = objInventoyExcelUpload.SerialNumber;
        //    if (objProduct.SerialNumberRequired == true)
        //    {
        //        objUploadInventoryBO.SerialNumberAllowded = true;
        //    }

        //    objUploadInventoryBO.Quantity = objInventoyExcelUpload.Quantity;
        //    return objUploadInventoryBO;
        //}



        /// <summary>
        /// 19-Jul-2017
        /// Aniket
        /// Check product/Make/Type/Size exist in master
        /// </summary>
        public List<UploadInventoryBO> CheckProductMakeTypeSizeExist(DataTable inventoryDataTable, string ProjectId, string WarehouseId, out long utilityId)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            List<UploadInventoryBO> objInventoryUploadModelList = new List<UploadInventoryBO>();

            var a1 = Convert.ToInt64(ProjectId);
            var projectRecord = objCompassEntities.tblProjects.Where(o => o.ProjectId == a1).FirstOrDefault();
            var ExistingProductsForProject = objCompassEntities.ProjectServiceProductRelations.AsNoTracking().Where(o => o.Active == 1 && o.FKProjectID == projectRecord.ProjectId).ToList();

            var _utilityId = objCompassEntities.UtilityMasters.Where(o => o.UtilityName == projectRecord.Utilitytype).FirstOrDefault().Id;
            utilityId = _utilityId;
            long warehouseId = Convert.ToInt64(WarehouseId);

            try
            {
                var warehouseName = objCompassEntities.WarehouseMasters.Where(a => a.Id == warehouseId).Select(a => a.WarehouseName).FirstOrDefault();
                var utilityName = projectRecord.Utilitytype;//objCompassEntities.UtilityMasters.Where(a => a.Id == utilityId).Select(a => a.UtilityName).FirstOrDefault();



                var categoryList = objCompassEntities.Categories.Where(a => a.Active == 1).ToList();

                int RowNo = 1;

                foreach (DataRow row in inventoryDataTable.Rows)
                {
                    VW_GetProductData productModel = new VW_GetProductData();
                    UploadInventoryBO uploadInventoryexist = new UploadInventoryBO();

                    var UtilityType = Convert.ToString(row["Utility Type"]).Trim();
                    var CategoryName = Convert.ToString(row["Category Name"]).Trim();
                    var ProductName = Convert.ToString(row["Product Name"]).Trim();
                    var Make = Convert.ToString(row["Make"]).Trim();
                    var Type = Convert.ToString(row["Type"]).Trim();
                    var Size = Convert.ToString(row["Size"]).Trim();
                    var PartNumber = Convert.ToString(row["Part Number"]).Trim();
                    var UPCCode = Convert.ToString(row["UPC Code"]).Trim();
                    var SerialNumber = Convert.ToString(row["Serial Number"]).Trim();
                    var Quantity = Convert.ToString(row["Quantity"]).Trim();
                    var Location = Convert.ToString(row["Location"]).Trim();

                    bool validQuantity = Quantity.All(char.IsDigit);

                    uploadInventoryexist.WarehouseId = warehouseId;
                    uploadInventoryexist.WarehouseName = warehouseName;
                    uploadInventoryexist.UtilityId = utilityId;
                    uploadInventoryexist.UtilityType = UtilityType;
                    uploadInventoryexist.Make = Make;
                    uploadInventoryexist.Type = Type;
                    uploadInventoryexist.ProductName = ProductName;
                    uploadInventoryexist.PartNumber = PartNumber;
                    uploadInventoryexist.UPCCode = UPCCode;
                    uploadInventoryexist.Size = Size;
                    uploadInventoryexist.SerialNumber = SerialNumber;
                    uploadInventoryexist.Location = Location;

                    //uploadInventoryexist.CategoryName = productModel.CategoryName;
                    //uploadInventoryexist.ProductId = productModel.Id;
                    //uploadInventoryexist.ProductDescription = productModel.ProductDescription;
                    //uploadInventoryexist.IsSerialNumber = productModel.SerialNumberRequired;
                    //uploadInventoryexist.Quantity = Convert.ToInt32(Quantity);
                    Category categoryexist = new Category();
                    bool productinCategory = false;

                    if (!string.IsNullOrEmpty(CategoryName))
                    {
                        categoryexist = categoryList.Where(a => a.CategoryName.Trim().ToLower() == CategoryName.ToLower()).FirstOrDefault();
                        if (categoryexist != null && !string.IsNullOrEmpty(ProductName))
                        {
                            var productexist = objCompassEntities.ProductMasters.Where(a => a.Active == 1 && a.ProductCategoryId == categoryexist.CategoryId && ProductName.ToLower() == a.ProductName.Trim().ToLower()).ToList();
                            if (productexist != null && productexist.Count > 0)
                            {
                                productinCategory = true;
                            }
                        }

                    }
                    ProductMaster IsUpccodeexist = null;
                    if (!string.IsNullOrEmpty(UPCCode))
                    {
                        IsUpccodeexist = new ProductMaster();
                        IsUpccodeexist = objCompassEntities.ProductMasters.Where(a => a.Active == 1 && a.UPCCode == UPCCode && ProductName.ToLower() == a.ProductName.Trim().ToLower()).FirstOrDefault();
                    }


                    if (string.IsNullOrEmpty(UtilityType))
                    {
                        uploadInventoryexist.InventryStatus = 2;  //Invalid data
                        uploadInventoryexist.IsDuplicate = "Utility Type required";
                    }
                    else if (utilityName.ToLower() != UtilityType.ToLower())
                    {
                        uploadInventoryexist.InventryStatus = 2;  //Invalid data
                        uploadInventoryexist.IsDuplicate = "Wrong Utility Type";
                    }
                    else if (utilityName.Length > 50)
                    {
                        uploadInventoryexist.InventryStatus = 2;  //Invalid data
                        uploadInventoryexist.IsDuplicate = "Utility Type name must be less than 50 characters";
                    }
                    else if (string.IsNullOrEmpty(CategoryName))
                    {
                        uploadInventoryexist.InventryStatus = 2;  //Invalid data
                        uploadInventoryexist.IsDuplicate = "Category Name required";
                    }
                    else if (categoryexist == null)
                    {
                        uploadInventoryexist.InventryStatus = 2;  //Invalid data
                        uploadInventoryexist.IsDuplicate = "Wrong Category Name";
                    }
                    else if (string.IsNullOrEmpty(ProductName))
                    {
                        uploadInventoryexist.InventryStatus = 2;  //Invalid data
                        uploadInventoryexist.IsDuplicate = "Product Name required";
                    }
                    else if (ProductName.Length > 250)
                    {
                        uploadInventoryexist.InventryStatus = 2;  //Invalid data
                        uploadInventoryexist.IsDuplicate = "Product Name must be less than 250 characters";
                    }
                    else if (productinCategory == false)
                    {
                        uploadInventoryexist.InventryStatus = 2;  //Invalid data
                        uploadInventoryexist.IsDuplicate = "Product does't exist in category";
                    }
                    else if (IsUpccodeexist == null)
                    {
                        uploadInventoryexist.InventryStatus = 2;  //Invalid data
                        uploadInventoryexist.IsDuplicate = "Wronge UPC Code";
                    }
                    else if (string.IsNullOrEmpty(Make))
                    {
                        uploadInventoryexist.InventryStatus = 2;  //Invalid data
                        uploadInventoryexist.IsDuplicate = "Product Make required";
                    }
                    else if (Make.Length > 250)
                    {
                        uploadInventoryexist.InventryStatus = 2;  //Invalid data
                        uploadInventoryexist.IsDuplicate = "Product Make must be less than 250 characters";
                    }
                    else if (string.IsNullOrEmpty(Quantity))
                    {
                        uploadInventoryexist.InventryStatus = 2;  //Invalid data
                        uploadInventoryexist.IsDuplicate = "Quantity required";
                    }
                    else if (Quantity.Length > 4)
                    {
                        uploadInventoryexist.InventryStatus = 2;  //Invalid data
                        uploadInventoryexist.IsDuplicate = "Quantity must be less than 4 characters";
                    }
                    else if (!validQuantity)
                    {
                        uploadInventoryexist.InventryStatus = 2;  //Invalid data
                        uploadInventoryexist.IsDuplicate = "Enter quantity in number";
                    }
                    else
                    {
                        #region check serialnumber exist


                        if (!string.IsNullOrEmpty(ProductName) && !string.IsNullOrEmpty(Make) && !string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Size))
                        {
                            productModel = objCompassEntities.VW_GetProductData.AsNoTracking().AsEnumerable()
                           .Where(a => a.UtilityId == _utilityId
                            && a.ProductName.Trim().ToLower() == ProductName.ToLower()
                            && a.Make != null && a.Make.Trim().ToLower() == Make.ToLower()
                            && a.MeterSize != null && a.MeterSize.Trim() == Size
                            && a.MeterType != null && a.MeterType.Trim().ToLower() == Type.ToLower()).FirstOrDefault();
                        }
                        else if (!string.IsNullOrEmpty(ProductName) && !string.IsNullOrEmpty(Make) && !string.IsNullOrEmpty(Type))
                        {
                            productModel = objCompassEntities.VW_GetProductData.AsNoTracking().AsEnumerable()
                           .Where(a => a.UtilityId == _utilityId
                            && a.ProductName.Trim().ToLower() == ProductName.ToLower()
                            && a.Make != null && a.Make.Trim().ToLower() == Make.ToLower()
                            && a.MeterType != null && a.MeterType.Trim().ToLower() == Type.ToLower()).FirstOrDefault();
                        }
                        else if (!string.IsNullOrEmpty(ProductName) && !string.IsNullOrEmpty(Make) && !string.IsNullOrEmpty(Size))
                        {
                            productModel = objCompassEntities.VW_GetProductData.AsNoTracking().AsEnumerable()
                           .Where(a => a.UtilityId == _utilityId
                            && a.ProductName.Trim().ToLower() == ProductName.ToLower()
                            && a.Make
                            != null && a.Make.Trim().ToLower() == Make.ToLower()
                            && a.MeterSize != null && a.MeterSize.Trim() == Size).FirstOrDefault();
                        }
                        else if (!string.IsNullOrEmpty(ProductName) && !string.IsNullOrEmpty(Make))
                        {
                            productModel = objCompassEntities.VW_GetProductData.AsNoTracking().AsEnumerable()
                           .Where(a => a.UtilityId == _utilityId
                            && a.ProductName.Trim().ToLower() == ProductName.ToLower()
                            && a.Make != null && a.Make.Trim().ToLower() == Make.ToLower()).FirstOrDefault();
                        }
                        if (!string.IsNullOrEmpty(ProductName) && !string.IsNullOrEmpty(UPCCode))
                        {
                            productModel = objCompassEntities.VW_GetProductData.AsNoTracking().AsEnumerable()
                           .Where(a => a.UtilityId == _utilityId
                            && a.ProductName.Trim().ToLower() == ProductName.ToLower()
                            && a.UPCCode != null && a.UPCCode.Trim().ToLower() == UPCCode.ToLower()
                           ).FirstOrDefault();
                        }

                        if (productModel != null)
                        {



                            uploadInventoryexist.InventryStatus = 0;  //valid data
                            uploadInventoryexist.IsDuplicate = "Successful";

                            if (productModel.SerialNumberRequired && string.IsNullOrEmpty(SerialNumber))
                            {
                                uploadInventoryexist.InventryStatus = 2;  //Invalid data
                                uploadInventoryexist.IsDuplicate = "Serial number required";
                            }
                            else if (productModel.SerialNumberRequired && SerialNumber.Length > 50)
                            {
                                uploadInventoryexist.InventryStatus = 2;  //Invalid data
                                uploadInventoryexist.IsDuplicate = "Serial Number must be less than 50 characters";
                            }
                            else if (productModel.SerialNumberRequired && Convert.ToInt32(Quantity) != 1)
                            {
                                uploadInventoryexist.InventryStatus = 2;  //Invalid data
                                uploadInventoryexist.IsDuplicate = "Invalid quantity, quantity must be 1";
                            }
                            else if (productModel.SerialNumberRequired)
                            {
                                if (objInventoryUploadModelList != null && objInventoryUploadModelList.Count > 0)
                                {
                                    var isSerialExist = objInventoryUploadModelList.Where(a => a.IsSerialNumber == true
                                        && a.SerialNumber == SerialNumber.Trim()).FirstOrDefault();
                                    if (isSerialExist != null)
                                    {
                                        uploadInventoryexist.InventryStatus = 1;  //already exist
                                        uploadInventoryexist.IsDuplicate = "Duplicate Serial number";
                                    }
                                    else
                                    {
                                        var a = ExistingProductsForProject.Where(o => o.FKProductId == productModel.Id).FirstOrDefault();

                                        if (a != null)
                                        {
                                            var serialexist = objCompassEntities.StockDetails.AsNoTracking().
                                                Where(o => o.Active == 1 && o.FKProjectId == a1 && o.SerialNumber == SerialNumber.Trim()).FirstOrDefault();
                                            if (serialexist != null)
                                            {
                                                uploadInventoryexist.InventryStatus = 1;  //already exist
                                                uploadInventoryexist.IsDuplicate = "Serial number already added for the project.";
                                            }
                                        }
                                        else
                                        {
                                            uploadInventoryexist.InventryStatus = 2;  //already exist
                                            uploadInventoryexist.IsDuplicate = "Product not mapped for the project.";
                                        }
                                    }

                                }
                                else
                                {
                                    var a = ExistingProductsForProject.Where(o => o.FKProductId == productModel.Id).FirstOrDefault();

                                    if (a != null)
                                    {
                                        var serialexist = objCompassEntities.StockDetails.AsNoTracking().
                                            Where(o => o.Active == 1 && o.FKProjectId == a1 && o.SerialNumber == SerialNumber.Trim()).FirstOrDefault();
                                        if (serialexist != null)
                                        {
                                            uploadInventoryexist.InventryStatus = 1;  //already exist
                                            uploadInventoryexist.IsDuplicate = "Serial number already added for the project.";
                                        }
                                    }
                                    else
                                    {
                                        uploadInventoryexist.InventryStatus = 2;  //already exist
                                        uploadInventoryexist.IsDuplicate = "Product not mapped for the project.";
                                    }
                                }

                            }
                            else if (productModel.SerialNumberRequired == false)
                            {

                                var a = ExistingProductsForProject.Where(o => o.FKProductId == productModel.Id).FirstOrDefault();

                                if (a == null)
                                {
                                    uploadInventoryexist.InventryStatus = 2;  //already exist
                                    uploadInventoryexist.IsDuplicate = "Product not mapped for the project.";
                                }
                            }

                        }
                        else
                        {
                            //uploadInventoryexist.WarehouseName = warehouseName;
                            //uploadInventoryexist.UtilityType = UtilityType;
                            //uploadInventoryexist.Make = Make;
                            //uploadInventoryexist.Type = Type;
                            //uploadInventoryexist.ProductName = ProductName;
                            //uploadInventoryexist.PartNumber = PartNumber;
                            //uploadInventoryexist.UPCCode = UPCCode;
                            //uploadInventoryexist.Size = Size;
                            //uploadInventoryexist.SerialNumber = SerialNumber;

                            uploadInventoryexist.InventryStatus = 2;  //Invalid data
                            uploadInventoryexist.IsDuplicate = "Product not exist";

                        }

                        #endregion
                    }

                    if (uploadInventoryexist.InventryStatus == 0)
                    {
                        uploadInventoryexist.UniqueId = RowNo;//Guid.NewGuid();
                        uploadInventoryexist.CategoryName = productModel.CategoryName;
                        uploadInventoryexist.ProductId = productModel.Id;
                        uploadInventoryexist.ProductDescription = productModel.ProductDescription;
                        uploadInventoryexist.IsSerialNumber = productModel.SerialNumberRequired;
                        uploadInventoryexist.Quantity = Convert.ToInt32(Quantity);
                        objInventoryUploadModelList.Add(uploadInventoryexist);
                    }
                    else
                    {
                        uploadInventoryexist.UniqueId = RowNo;//Guid.NewGuid();
                        uploadInventoryexist.ProductId = 0;
                        uploadInventoryexist.CategoryName = CategoryName;
                        uploadInventoryexist.ProductDescription = "";

                        uploadInventoryexist.Quantity = (validQuantity == true && Quantity != "") ? Convert.ToInt32(Quantity) : 0;
                        objInventoryUploadModelList.Add(uploadInventoryexist);
                    }

                    RowNo++;
                }




            }
            catch (Exception ex)
            {
                throw;
            }
            return objInventoryUploadModelList;
        }

        public bool SaveInventryInStockHeader(InwardHeaderBO inwardHeaderBO, out string Message)
        {
            bool result = false;
            Message = "Unable to Save Inventry.";
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();

                InwardHeader createOrUpdate = new InwardHeader();

                Mapper.DynamicMap<InwardHeaderBO, InwardHeader>(inwardHeaderBO, createOrUpdate);

                createOrUpdate.DeviceDateTime = new CommonFunctions().ServerDate();
                createOrUpdate.Active = 1;

                createOrUpdate.CreatedOn = new CommonFunctions().ServerDate();
                objCompassEntities.InwardHeaders.Add(createOrUpdate);
                objCompassEntities.SaveChanges();

                Message = "Inventry Saved Successfully";
                result = true;

            }

            catch (Exception ex)
            {
                Message += "<br/>" + ex.Message;
            }

            return result;
        }

        public bool WarehouseInsertMobileDataOld(List<ProductMasterBO> productModelList, ref List<ProductMasterBO> NewSerialNumberList)
        {
            bool result = false;

            try
            {
                CompassEntities objCompassEntities = new CompassEntities();

                string errormessage = "";

                string WebURLFilePath = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();



                var count1 = productModelList.Where(o => o.GenerateBarcode == true).ToList().Count();
                //check if generate barcode is required
                NewSerialNumberList = new List<ProductMasterBO>();

                if (count1 > 0)
                {
                    //Path on the current server
                    string MainPath = @"~\BarcodeFiles";

                    string dynamicFolderName = DateTime.Now.ToString("ddMMyyhhmmssfff");
                    //InwardRequestId
                    MainPath = MainPath + @"\" + dynamicFolderName;



                    //InwardRequestId
                    WebURLFilePath = WebURLFilePath + @"BarcodeFiles\" + dynamicFolderName;


                    int parentId = 1;
                    foreach (var item in productModelList.Where(o => o.GenerateBarcode == true).ToList())
                    {
                        #region generate barcode & add record in database

                        var CheckProductStatus = objCompassEntities.ProductMasters.Where(o => o.Id == item.Id).FirstOrDefault();


                        string dynamicFolderName1 = DateTime.Now.ToString("ddMMyyhhmmssfff");
                        //SubInwardRequestId
                        string subMainPath = MainPath + @"\" + dynamicFolderName1 + "\\BarcodeFile.pdf";

                        string filePath = HttpContext.Current.Request.MapPath(subMainPath);

                        grantAccess(System.IO.Path.GetDirectoryName(filePath));
                        //create Directory

                        string subWebURLFilePath = WebURLFilePath + @"\" + dynamicFolderName1 + "\\BarcodeFile.pdf";

                        // var doc = new Document();
                        var doc = new Document(PageSize.A4, 50, 50, 25, 25);

                        PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
                        //open the document for writing
                        doc.Open();
                        //get PdfContentByte object
                        PdfContentByte content = writer.DirectContent;

                        for (int i = 0; i < item.Quantity; i++)
                        {

                            //
                            bool stockMasterCheck = false;
                            var stringKey = "";


                            var titleFont = FontFactory.GetFont("Segoe UI", 7, BaseColor.BLACK);
                            doc.Add(new Paragraph("Item : " + CheckProductStatus.ProductName, titleFont));


                            if (CheckProductStatus.SerialNumberRequired == true)
                            {
                                while (!stockMasterCheck)
                                {
                                    stringKey = GetUniqueKey(13);

                                    var objStatus = objCompassEntities.StockDetails.Where(o => o.Active == 1 && o.SerialNumber == stringKey).FirstOrDefault();
                                    if (objStatus == null)
                                    {
                                        stockMasterCheck = true;
                                    }

                                }
                                if (!string.IsNullOrEmpty(CheckProductStatus.PartNumber))
                                {
                                    doc.Add(new Paragraph("PartNumber:" + CheckProductStatus.PartNumber, titleFont));
                                }
                            }
                            else
                            {
                                stringKey = CheckProductStatus.PartNumber == null ? "" : CheckProductStatus.PartNumber;

                            }




                            Barcode39 bar39 = new Barcode39();
                            bar39.Code = stringKey;//"12345ABCDE";
                            iTextSharp.text.Image img39 = bar39.CreateImageWithBarcode(content, null, null);
                            doc.Add(img39);
                            doc.Add(new Paragraph(""));
                            doc.Add(new Paragraph(""));

                            if (CheckProductStatus.SerialNumberRequired == true)
                            {
                                #region Insert record in NewSerialNumberList

                                ProductMasterBO obj = new ProductMasterBO();
                                obj.ParentId = parentId;
                                obj.Id = item.Id;
                                obj.SerialNumber = stringKey;
                                obj.ParentId = parentId;
                                obj.UtilityId = item.UtilityId;

                                obj.ProductName = item.ProductName;
                                obj.ProductDescription = item.ProductDescription;
                                obj.ProductPartNumber = item.ProductPartNumber;
                                obj.ProductUPCCode = item.ProductUPCCode;
                                obj.Make = item.Make;
                                obj.Type = item.Type;
                                obj.Size = item.Size;

                                obj.UtilityName = item.UtilityName;
                                obj.SerialNumberRequired = item.SerialNumberRequired;
                                obj.CategoryId = item.CategoryId;
                                obj.CategoryName = item.CategoryName;
                                obj.WarehouseId = item.WarehouseId;
                                obj.WarehouseName = item.WarehouseName;
                                obj.Quantity = 1;
                                obj.GenerateBarcode = true;
                                NewSerialNumberList.Add(obj);
                                #endregion
                            }
                        }

                        doc.Close();

                        item.WebUrlOfBarcodeFile = subMainPath;
                        if (CheckProductStatus.SerialNumberRequired == true)
                        {
                            item.ParentId = parentId;
                        }

                        parentId++;
                        #endregion
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        public bool WarehouseInsertReceiveData(long projectId, List<ProductMasterBO> productModelList, ref List<ProductMasterBO> NewSerialNumberList)
        {
            bool result = false;

            try
            {
                CompassEntities objCompassEntities = new CompassEntities();

                string errormessage = "";

                string WebURLFilePath = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();

                var count1 = productModelList.Where(o => o.GenerateBarcode == true).ToList().Count();
                //check if generate barcode is required
                NewSerialNumberList = new List<ProductMasterBO>();

                if (count1 > 0)
                {

                    //Path on the current server
                    string MainPath = @"~\BarcodeFiles";

                    string dynamicFolderName = DateTime.Now.ToString("ddMMyyhhmmssfff");
                    //InwardRequestId
                    MainPath = MainPath + @"\" + dynamicFolderName;

                    //InwardRequestId
                    WebURLFilePath = WebURLFilePath + @"BarcodeFiles\" + dynamicFolderName;

                    int parentId = 1;
                    foreach (var item in productModelList.Where(o => o.GenerateBarcode == true).ToList())
                    {

                        #region generate barcode & add record in database

                        var CheckProductStatus = objCompassEntities.ProductMasters.Where(o => o.Id == item.Id).FirstOrDefault();


                        string dynamicFolderName1 = DateTime.Now.ToString("ddMMyyhhmmssfff");
                        //SubInwardRequestId
                        string subMainPath = MainPath + @"\" + dynamicFolderName1 + "\\BarcodeFile.pdf";

                        string filePath = HttpContext.Current.Request.MapPath(subMainPath);

                        grantAccess(System.IO.Path.GetDirectoryName(filePath));
                        //create Directory

                        string subWebURLFilePath = WebURLFilePath + @"\" + dynamicFolderName1 + "\\BarcodeFile.pdf";

                        DataTable dt = new DataTable();
                        dt.Columns.Add("ProductName");
                        dt.Columns.Add("SerialNumber");
                        dt.Columns.Add("PartNumber");
                        dt.Columns.Add("UPC");
                        dt.Columns.Add("PartNumber1");
                        dt.Columns.Add("UPC1");

                        for (int i = 0; i < item.Quantity; i++)
                        {

                            bool stockMasterCheck = false;
                            var stringKey = "";

                            if (CheckProductStatus.SerialNumberRequired == true)
                            {
                                DataRow dr = dt.NewRow();
                                dr["ProductName"] = CheckProductStatus.ProductName;
                                dr["PartNumber"] = CheckProductStatus.PartNumber;
                                dr["UPC"] = CheckProductStatus.UPCCode;
                                dr["PartNumber1"] = "Part No. : " + CheckProductStatus.PartNumber;
                                dr["UPC1"] = "UPC : " + CheckProductStatus.UPCCode;

                                while (!stockMasterCheck)
                                {
                                    stringKey = GetUniqueKey(13);

                                    var objStatus = objCompassEntities.StockDetails.AsNoTracking().
                                        Where(o => o.Active == 1 && o.FKProjectId == projectId && o.SerialNumber == stringKey).FirstOrDefault();
                                    if (objStatus == null)
                                    {
                                        stockMasterCheck = true;
                                    }

                                }
                                dr["SerialNumber"] = stringKey;
                                dt.Rows.Add(dr);

                                if (!string.IsNullOrEmpty(CheckProductStatus.PartNumber))
                                {
                                    //doc.Add(new Paragraph("PartNumber:" + CheckProductStatus.PartNumber, titleFont));
                                }
                            }
                            else
                            {
                                stringKey = CheckProductStatus.PartNumber == null ? "" : CheckProductStatus.PartNumber;

                                DataRow dr = dt.NewRow();
                                dr["ProductName"] = CheckProductStatus.ProductName;
                                dr["SerialNumber"] = "";
                                dr["PartNumber"] = CheckProductStatus.PartNumber;
                                dr["UPC"] = CheckProductStatus.UPCCode;
                                dr["PartNumber1"] = "Part No. : " + CheckProductStatus.PartNumber;
                                dr["UPC1"] = "UPC : " + CheckProductStatus.UPCCode;
                                dt.Rows.Add(dr);
                            }


                            if (CheckProductStatus.SerialNumberRequired == true)
                            {
                                #region Insert record in NewSerialNumberList

                                ProductMasterBO obj = new ProductMasterBO();
                                obj.ParentId = parentId;
                                obj.Id = item.Id;
                                obj.SerialNumber = stringKey;
                                obj.ParentId = parentId;
                                obj.UtilityId = item.UtilityId;

                                obj.ProductName = item.ProductName;
                                obj.ProductDescription = item.ProductDescription;
                                obj.ProductPartNumber = item.ProductPartNumber;
                                obj.ProductUPCCode = item.ProductUPCCode;
                                obj.Make = item.Make;
                                obj.Type = item.Type;
                                obj.Size = item.Size;

                                obj.UtilityName = item.UtilityName;
                                obj.SerialNumberRequired = item.SerialNumberRequired;
                                obj.CategoryId = item.CategoryId;
                                obj.CategoryName = item.CategoryName;
                                obj.WarehouseId = item.WarehouseId;
                                obj.WarehouseName = item.WarehouseName;
                                obj.Quantity = 1;
                                obj.GenerateBarcode = true;
                                NewSerialNumberList.Add(obj);
                                #endregion
                            }
                        }
                        string ReportFilePath = "";
                        if (CheckProductStatus.SerialNumberRequired == true)
                        {
                            ReportFilePath = "~/RDLCReport/ReportBarcodeDynamic.rdlc";
                        }
                        else
                        {
                            ReportFilePath = "~/RDLCReport/ReportBarcode.rdlc";
                        }



                        DataSet ds = new DataSet();
                        ds.Tables.Add(dt);
                        ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                        Microsoft.Reporting.WebForms.ReportViewer ReportViewer1 = new Microsoft.Reporting.WebForms.ReportViewer();
                        ReportViewer1.LocalReport.DataSources.Clear();
                        ReportViewer1.LocalReport.DataSources.Add(datasource);
                        ReportViewer1.LocalReport.EnableHyperlinks = true;
                        ReportViewer1.LocalReport.ReportPath = HttpContext.Current.Request.MapPath(ReportFilePath);

                        System.IO.FileInfo fi = new System.IO.FileInfo(filePath);

                        // if (fi.Exists) fi.Delete();

                        Warning[] warnings;

                        string[] streamids;

                        string mimeType, encoding, filenameExtension;

                        //byte[] bytes = ReportViewer1.LocalReport.Render("Pdf", null, out mimeType, out encoding, out filenameExtension,

                        //out streamids, out warnings);

                        byte[] bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);


                        System.IO.FileStream fs = System.IO.File.Create(filePath);

                        fs.Write(bytes, 0, bytes.Length);

                        fs.Close();


                        //doc.Close();

                        item.WebUrlOfBarcodeFile = subMainPath;
                        if (CheckProductStatus.SerialNumberRequired == true)
                        {
                            item.ParentId = parentId;
                        }

                        parentId++;
                        #endregion
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool SaveInventryInStockDetail(UploadInventryVM uploadInventryVM, out string returnmessage, ref long inwardHeaderId)
        {
            bool result = false;
            DataTable ObjDataTable = new DataTable();
            CompassEntities objCompassEntities = new CompassEntities();

            ObjDataTable.Columns.Add("ParentId", typeof(long));
            ObjDataTable.Columns.Add("ProductId", typeof(long));
            ObjDataTable.Columns.Add("UtilityTypeId", typeof(long));
            ObjDataTable.Columns.Add("UtilityType", typeof(string));
            ObjDataTable.Columns.Add("ProductName", typeof(string));
            ObjDataTable.Columns.Add("PartNumber", typeof(string));
            ObjDataTable.Columns.Add("UPCCode", typeof(string));
            ObjDataTable.Columns.Add("Make", typeof(string));
            ObjDataTable.Columns.Add("Type", typeof(string));
            ObjDataTable.Columns.Add("Size", typeof(string));
            ObjDataTable.Columns.Add("IsSerialNumber", typeof(bool));
            ObjDataTable.Columns.Add("SerialNumber", typeof(string));
            ObjDataTable.Columns.Add("TotalQty", typeof(int));
            ObjDataTable.Columns.Add("GenerateBarcode", typeof(int));
            ObjDataTable.Columns.Add("BarCodeFilePath", typeof(string));
            ObjDataTable.Columns.Add("Location", typeof(string));

            if (uploadInventryVM.UploadType == 1)
            {
                foreach (var item in uploadInventryVM.UploadInventoryList.Where(a => a.IsSerialNumber == true).ToList())
                {

                    DataRow DR = ObjDataTable.NewRow();

                    DR["ParentId"] = DBNull.Value;
                    DR["ProductId"] = item.ProductId;
                    DR["UtilityTypeId"] = item.UtilityId;
                    DR["UtilityType"] = item.UtilityType;
                    DR["ProductName"] = item.ProductName;
                    DR["PartNumber"] = item.PartNumber;
                    DR["UPCCode"] = item.UPCCode;
                    DR["Make"] = item.Make;
                    DR["Type"] = item.Type;
                    DR["Size"] = item.Size;
                    DR["IsSerialNumber"] = item.IsSerialNumber;
                    DR["SerialNumber"] = item.SerialNumber;
                    DR["TotalQty"] = 1;
                    DR["GenerateBarcode"] = DBNull.Value;
                    DR["BarCodeFilePath"] = DBNull.Value;
                    DR["Location"] = item.Location;

                    ObjDataTable.Rows.Add(DR);
                }

                foreach (var item in uploadInventryVM.UploadInventoryList.Where(a => a.IsSerialNumber == false).ToList())
                {

                    DataRow DR = ObjDataTable.NewRow();

                    DR["ParentId"] = DBNull.Value;

                    DR["ProductId"] = item.ProductId;
                    DR["UtilityTypeId"] = item.UtilityId;
                    DR["UtilityType"] = item.UtilityType;
                    DR["ProductName"] = item.ProductName;
                    DR["PartNumber"] = item.PartNumber;
                    DR["UPCCode"] = item.UPCCode;
                    DR["Make"] = item.Make;
                    DR["Type"] = item.Type;
                    DR["Size"] = item.Size;

                    DR["IsSerialNumber"] = item.IsSerialNumber;
                    DR["SerialNumber"] = DBNull.Value;
                    DR["TotalQty"] = item.Quantity;
                    DR["GenerateBarcode"] = DBNull.Value;
                    DR["BarCodeFilePath"] = DBNull.Value;
                    DR["Location"] = item.Location;

                    ObjDataTable.Rows.Add(DR);
                }
            }
            else
            {
                foreach (var item in uploadInventryVM.ProductMasterList.Where(a => a.SerialNumberRequired == true).ToList())
                {

                    DataRow DR = ObjDataTable.NewRow();
                    DR["ParentId"] = item.ParentId;
                    DR["ProductId"] = item.Id;
                    DR["UtilityTypeId"] = item.UtilityId;
                    DR["UtilityType"] = item.UtilityName;
                    DR["ProductName"] = item.ProductName;
                    DR["PartNumber"] = item.ProductPartNumber;
                    DR["UPCCode"] = item.ProductUPCCode;
                    DR["Make"] = item.Make;
                    DR["Type"] = item.Type;
                    DR["Size"] = item.Size;

                    DR["IsSerialNumber"] = item.SerialNumberRequired;
                    DR["SerialNumber"] = item.SerialNumber;
                    DR["TotalQty"] = item.Quantity;
                    DR["GenerateBarcode"] = item.GenerateBarcode == true ? 1 : 0;
                    DR["BarCodeFilePath"] = item.GenerateBarcode == true ? item.WebUrlOfBarcodeFile : "";
                    //DR["Location"] = item.Location;

                    ObjDataTable.Rows.Add(DR);
                }

                foreach (var item in uploadInventryVM.ProductMasterList.Where(a => a.SerialNumberRequired == false).ToList())
                {

                    DataRow DR = ObjDataTable.NewRow();

                    DR["ParentId"] = item.ParentId;
                    DR["ProductId"] = item.Id;
                    DR["UtilityTypeId"] = item.UtilityId;
                    DR["UtilityType"] = item.UtilityName;
                    DR["ProductName"] = item.ProductName;
                    DR["PartNumber"] = item.ProductPartNumber;
                    DR["UPCCode"] = item.ProductUPCCode;
                    DR["Make"] = item.Make;
                    DR["Type"] = item.Type;
                    DR["Size"] = item.Size;

                    DR["IsSerialNumber"] = item.SerialNumberRequired;
                    DR["SerialNumber"] = DBNull.Value;
                    DR["TotalQty"] = item.Quantity;
                    DR["GenerateBarcode"] = DBNull.Value;
                    DR["BarCodeFilePath"] = DBNull.Value;
                    //DR["Location"] = item.Location;

                    ObjDataTable.Rows.Add(DR);
                }

                foreach (var item in uploadInventryVM.BarcodeProductList.Where(a => a.GenerateBarcode == true).ToList())
                {

                    DataRow DR = ObjDataTable.NewRow();

                    DR["ParentId"] = item.ParentId;
                    DR["ProductId"] = item.Id;
                    DR["UtilityTypeId"] = item.UtilityId;
                    DR["UtilityType"] = item.UtilityName;
                    DR["ProductName"] = item.ProductName;
                    DR["PartNumber"] = item.ProductPartNumber;
                    DR["UPCCode"] = item.ProductUPCCode;
                    DR["Make"] = item.Make;
                    DR["Type"] = item.Type;
                    DR["Size"] = item.Size;

                    DR["IsSerialNumber"] = item.SerialNumberRequired;
                    DR["SerialNumber"] = item.SerialNumber;
                    DR["TotalQty"] = item.Quantity;
                    DR["GenerateBarcode"] = 0;
                    DR["BarCodeFilePath"] = "";
                    //DR["Location"] = item.Location;

                    ObjDataTable.Rows.Add(DR);
                }
            }

            //var a=objCompassEntities.PROC_Insert_InwardFromMobile(objMobileWarehouseRecieveBo.DeviceDateTimeOfRequest, objMobileWarehouseRecieveBo.UserId,
            //    objMobileWarehouseRecieveBo.WarehouseId);
            returnmessage = "";
            try
            {
                uploadInventryVM.DeviceDateTime = new CommonFunctions().ServerDate();


                SqlConnection con = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("PROC_Insert_InwardFromWeb", con);
                cmd.Parameters.AddWithValue("@tblWebSubmit", ObjDataTable); // passing Datatable

                cmd.Parameters.AddWithValue("@UserId", uploadInventryVM.UserId);
                cmd.Parameters.AddWithValue("@ProjectId", uploadInventryVM.ProjectId);
                cmd.Parameters.AddWithValue("@WarehouseId", uploadInventryVM.WarehouseId);
                cmd.Parameters.AddWithValue("@UploadType", uploadInventryVM.UploadType);
                cmd.Parameters.AddWithValue("@StrFilePath", uploadInventryVM.StrFilePath);
                cmd.Parameters.AddWithValue("@deviceDateTime", uploadInventryVM.DeviceDateTime);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                con.Close();
                result = true;
                returnmessage = "Inventry data saved successfully";
            }
            catch (SqlException ex)
            {
                returnmessage = ex.Message;
            }
            if (result)
            {
                inwardHeaderId = objCompassEntities.InwardHeaders.Where(a => a.CreatedBy == uploadInventryVM.UserId && a.DeviceDateTime == uploadInventryVM.DeviceDateTime).Select(a => a.Id).FirstOrDefault();
            }

            return result;
        }

        public List<ViewUploadInventryVM> GetInwardHeader()
        {
            List<ViewUploadInventryVM> viewUploadInventryVM = new List<ViewUploadInventryVM>();
            CompassEntities objCompassEntities = new CompassEntities();
            try
            {


                viewUploadInventryVM = (from ih in objCompassEntities.InwardHeaders.AsNoTracking()
                                        join user in objCompassEntities.tblUsers.AsNoTracking() on ih.CreatedBy equals user.UserID
                                        where ih.Active == 1 && user.Active == 1
                                        select new ViewUploadInventryVM
                                        {
                                            RequestId = ih.Id,
                                            uploadType = ih.UploadType == 1 ? "Web Excel" : ih.UploadType == 2 ? "Web Manual" : "Mobile",
                                            DeviceDate = ih.DeviceDateTime,
                                            //DeviceDateTime = ih.DeviceDateTime.Value.ToString("MMM-dd-yyyy") + "  " + ih.DeviceDateTime.Value.ToString("h:mm tt"),
                                            UserId = ih.CreatedBy,
                                            UserName = user.UserName,
                                            WarehouseName = ih.WarehouseMaster.WarehouseName + ", " + ih.WarehouseMaster.TblCityMaster.CityName + ", " + ih.WarehouseMaster.tblStateMaster.StateName,
                                            // ProjectName = ih.FKProjectId.HasValue ?  : ""
                                            ProjectId = ih.FKProjectId
                                        }).OrderByDescending(a => a.DeviceDate).ToList();


                if (viewUploadInventryVM != null && viewUploadInventryVM.Count > 0)
                {

                    foreach (var item in viewUploadInventryVM)
                    {
                        if (item.ProjectId.HasValue)
                        {
                            item.ProjectName = objCompassEntities.tblProjects.Where(o => o.ProjectId == item.ProjectId).Select(o => o.ProjectName).FirstOrDefault();
                        }
                        else
                        {
                            item.ProjectName = "";
                        }
                        if (item.DeviceDate.HasValue)
                        {
                            item.DeviceDateTime = item.DeviceDate.Value.ToString("MMM-dd-yyyy") + "  " + item.DeviceDate.Value.ToString("h:mm tt");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return viewUploadInventryVM;
        }


        public ViewUploadInventryVM GetInwardHeader(long Id)
        {
            ViewUploadInventryVM viewUploadInventryVM = new ViewUploadInventryVM();
            CompassEntities objCompassEntities = new CompassEntities();
            try
            {


                viewUploadInventryVM = (from ih in objCompassEntities.InwardHeaders.AsEnumerable()
                                        join user in objCompassEntities.tblUsers on ih.CreatedBy equals user.UserID
                                        where ih.Active == 1 && user.Active == 1 && ih.Id == Id
                                        select new ViewUploadInventryVM
                                        {
                                            RequestId = ih.Id,
                                            uploadType = ih.UploadType == 1 ? "Web Excel" : ih.UploadType == 2 ? "Web Manual" : "Mobile",
                                            DeviceDate = ih.DeviceDateTime.Value,
                                            DeviceDateTime = ih.DeviceDateTime.Value.ToString("MMM-dd-yyyy") + "  " + ih.DeviceDateTime.Value.ToString("h:mm tt"),
                                            UserId = ih.CreatedBy,
                                            UserName = user.UserName,
                                            ProjectId = ih.FKProjectId,
                                            ProjectName = Convert.ToString(ih.FKProjectId),
                                            WarehouseName = ih.WarehouseMaster.WarehouseName + ", " + ih.WarehouseMaster.TblCityMaster.CityName + ", " + ih.WarehouseMaster.tblStateMaster.StateName
                                        }).FirstOrDefault();

                viewUploadInventryVM.ProjectName = objCompassEntities.tblProjects.Where(o => o.ProjectId.ToString() == viewUploadInventryVM.ProjectName).Select(o => o.ProjectName).FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
            return viewUploadInventryVM;
        }



        public List<ReceiveInvoiceSavedRecords> GetUploadedRecords(long Id)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            List<ReceiveInvoiceSavedRecords> ReceiveInvoiceSavedRecords = new List<BO.ReceiveInvoiceSavedRecords>();
            try
            {

                ReceiveInvoiceSavedRecords = objCompassEntities.PROC_GET_ReceiveInvoiceSavedRecords(Id).Select(a => new ReceiveInvoiceSavedRecords
                {
                    HeaderId = a.HeaderId,
                    ParentId = a.ParentId,
                    ChildParentId = a.ChildParentId,
                    GenerateBarcode = a.GenerateBarcode,
                    SystemGenBarcode = a.SystemGenBarcode,
                    BarCodeFilePath = a.BarCodeFilePath,
                    IsSerialNumber = a.IsSerialNumber,
                    SerialNumber = a.SerialNumber,
                    UploadQty = a.UploadQty,
                    FKWarehouseId = a.FKWarehouseId,
                    WarehouseName = a.WarehouseName,
                    FKProductId = a.FKProductId,
                    ProductName = a.ProductName,
                    ProductDescription = a.ProductDescription,
                    PartNumber = a.PartNumber,
                    UPCCode = a.UPCCode,
                    UtilityName = a.UtilityName,
                    CategoryName = a.CategoryName,
                    Make = a.Make,
                    MeterSize = a.MeterSize,
                    MeterType = a.MeterType

                }).ToList();

            }
            catch (Exception)
            {

                throw;
            }

            return ReceiveInvoiceSavedRecords;
        }

        public bool SaveReceiveInventryInStockDetail(UploadInventryVM uploadInventryVM, out string returnmessage, ref long inwardHeaderId)
        {
            bool result = false;
            DataTable ObjDataTable = new DataTable();
            CompassEntities objCompassEntities = new CompassEntities();

            ObjDataTable.Columns.Add("ParentId", typeof(long));
            ObjDataTable.Columns.Add("ProductId", typeof(long));
            ObjDataTable.Columns.Add("UtilityTypeId", typeof(long));
            ObjDataTable.Columns.Add("UtilityType", typeof(string));
            ObjDataTable.Columns.Add("ProductName", typeof(string));
            ObjDataTable.Columns.Add("PartNumber", typeof(string));
            ObjDataTable.Columns.Add("UPCCode", typeof(string));
            ObjDataTable.Columns.Add("Make", typeof(string));
            ObjDataTable.Columns.Add("Type", typeof(string));
            ObjDataTable.Columns.Add("Size", typeof(string));
            ObjDataTable.Columns.Add("IsSerialNumber", typeof(bool));
            ObjDataTable.Columns.Add("SerialNumber", typeof(string));
            ObjDataTable.Columns.Add("TotalQty", typeof(int));
            ObjDataTable.Columns.Add("GenerateBarcode", typeof(int));
            ObjDataTable.Columns.Add("BarCodeFilePath", typeof(string));



            if (uploadInventryVM.UploadType == 1)
            {
                foreach (var item in uploadInventryVM.UploadInventoryList.Where(a => a.IsSerialNumber == true).ToList())
                {

                    DataRow DR = ObjDataTable.NewRow();

                    DR["ParentId"] = DBNull.Value;
                    DR["ProductId"] = item.ProductId;
                    DR["UtilityTypeId"] = item.UtilityId;
                    DR["UtilityType"] = item.UtilityType;
                    DR["ProductName"] = item.ProductName;
                    DR["PartNumber"] = item.PartNumber;
                    DR["UPCCode"] = item.UPCCode;
                    DR["Make"] = item.Make;
                    DR["Type"] = item.Type;
                    DR["Size"] = item.Size;
                    DR["IsSerialNumber"] = item.IsSerialNumber;
                    DR["SerialNumber"] = item.SerialNumber;
                    DR["TotalQty"] = 1;
                    DR["GenerateBarcode"] = DBNull.Value;
                    DR["BarCodeFilePath"] = DBNull.Value;

                    ObjDataTable.Rows.Add(DR);
                }

                foreach (var item in uploadInventryVM.UploadInventoryList.Where(a => a.IsSerialNumber == false).ToList())
                {

                    DataRow DR = ObjDataTable.NewRow();

                    DR["ParentId"] = DBNull.Value;
                    DR["SubSourceId"] = DBNull.Value;
                    DR["ProductId"] = item.ProductId;
                    DR["UtilityTypeId"] = item.UtilityId;
                    DR["UtilityType"] = item.UtilityType;
                    DR["ProductName"] = item.ProductName;
                    DR["PartNumber"] = item.PartNumber;
                    DR["UPCCode"] = item.UPCCode;
                    DR["Make"] = item.Make;
                    DR["Type"] = item.Type;
                    DR["Size"] = item.Size;

                    DR["IsSerialNumber"] = item.IsSerialNumber;
                    DR["SerialNumber"] = DBNull.Value;
                    DR["TotalQty"] = item.Quantity;
                    DR["GenerateBarcode"] = DBNull.Value;
                    DR["BarCodeFilePath"] = DBNull.Value;

                    ObjDataTable.Rows.Add(DR);
                }
            }
            else
            {
                foreach (var item in uploadInventryVM.ProductMasterList.Where(a => a.SerialNumberRequired == true).ToList())
                {

                    DataRow DR = ObjDataTable.NewRow();
                    DR["ParentId"] = item.ParentId;
                    DR["ProductId"] = item.Id;
                    DR["UtilityTypeId"] = item.UtilityId;
                    DR["UtilityType"] = item.UtilityName;
                    DR["ProductName"] = item.ProductName;
                    DR["PartNumber"] = item.ProductPartNumber;
                    DR["UPCCode"] = item.ProductUPCCode;
                    DR["Make"] = item.Make;
                    DR["Type"] = item.Type;
                    DR["Size"] = item.Size;


                    DR["IsSerialNumber"] = item.SerialNumberRequired;
                    DR["SerialNumber"] = item.SerialNumber;
                    DR["TotalQty"] = item.Quantity;
                    DR["GenerateBarcode"] = item.GenerateBarcode == true ? 1 : 0;
                    DR["BarCodeFilePath"] = item.GenerateBarcode == true ? item.WebUrlOfBarcodeFile : "";

                    ObjDataTable.Rows.Add(DR);
                }

                foreach (var item in uploadInventryVM.ProductMasterList.Where(a => a.SerialNumberRequired == false).ToList())
                {

                    DataRow DR = ObjDataTable.NewRow();

                    DR["ParentId"] = item.ParentId;
                    DR["ProductId"] = item.Id;
                    DR["UtilityTypeId"] = item.UtilityId;
                    DR["UtilityType"] = item.UtilityName;
                    DR["ProductName"] = item.ProductName;
                    DR["PartNumber"] = item.ProductPartNumber;
                    DR["UPCCode"] = item.ProductUPCCode;
                    DR["Make"] = item.Make;
                    DR["Type"] = item.Type;
                    DR["Size"] = item.Size;

                    DR["IsSerialNumber"] = item.SerialNumberRequired;
                    DR["SerialNumber"] = DBNull.Value;
                    DR["TotalQty"] = item.Quantity;
                    DR["GenerateBarcode"] = item.GenerateBarcode == true ? 1 : 0;// DBNull.Value;
                    DR["BarCodeFilePath"] = item.GenerateBarcode == true ? item.WebUrlOfBarcodeFile : "";// DBNull.Value;

                    ObjDataTable.Rows.Add(DR);
                }

                foreach (var item in uploadInventryVM.BarcodeProductList.Where(a => a.GenerateBarcode == true).ToList())
                {

                    DataRow DR = ObjDataTable.NewRow();

                    DR["ParentId"] = item.ParentId;
                    DR["ProductId"] = item.Id;
                    DR["UtilityTypeId"] = item.UtilityId;
                    DR["UtilityType"] = item.UtilityName;
                    DR["ProductName"] = item.ProductName;
                    DR["PartNumber"] = item.ProductPartNumber;
                    DR["UPCCode"] = item.ProductUPCCode;
                    DR["Make"] = item.Make;
                    DR["Type"] = item.Type;
                    DR["Size"] = item.Size;

                    DR["IsSerialNumber"] = item.SerialNumberRequired;
                    DR["SerialNumber"] = item.SerialNumber;
                    DR["TotalQty"] = item.Quantity;
                    DR["GenerateBarcode"] = 0;
                    DR["BarCodeFilePath"] = "";

                    ObjDataTable.Rows.Add(DR);
                }
            }

            //var a=objCompassEntities.PROC_Insert_InwardFromMobile(objMobileWarehouseRecieveBo.DeviceDateTimeOfRequest, objMobileWarehouseRecieveBo.UserId,
            //    objMobileWarehouseRecieveBo.WarehouseId);
            returnmessage = "";
            try
            {
                uploadInventryVM.DeviceDateTime = new CommonFunctions().ServerDate();


                SqlConnection con = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand("PROC_Insert_InwardFromWeb", con);
                cmd.Parameters.AddWithValue("@tblWebSubmit", ObjDataTable); // passing Datatable

                cmd.Parameters.AddWithValue("@UserId", uploadInventryVM.UserId);
                cmd.Parameters.AddWithValue("@ProjectId", uploadInventryVM.ProjectId);
                cmd.Parameters.AddWithValue("@WarehouseId", uploadInventryVM.WarehouseId);
                cmd.Parameters.AddWithValue("@UploadType", uploadInventryVM.UploadType);
                cmd.Parameters.AddWithValue("@StrFilePath", uploadInventryVM.StrFilePath);
                cmd.Parameters.AddWithValue("@deviceDateTime", uploadInventryVM.DeviceDateTime);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                con.Close();
                result = true;
                returnmessage = "Inventry data saved successfully";
            }
            catch (SqlException ex)
            {
                returnmessage = ex.Message;
            }
            if (result)
            {
                inwardHeaderId = objCompassEntities.InwardHeaders.Where(a => a.CreatedBy == uploadInventryVM.UserId && a.DeviceDateTime == uploadInventryVM.DeviceDateTime).Select(a => a.Id).FirstOrDefault();
            }


            return result;
        }


        //public List<PROC_Report_InventryStockReport_Result> GetInventryStockReport(long projectId, long installerId, long categoryId, long ProductId, string txtFromDate, string txtToDate)
        //{
        //    CompassEntities objCompassEntities = new CompassEntities();
        //    List<PROC_Report_InventryStockReport_Result> reportData = new List<PROC_Report_InventryStockReport_Result>();
        //    try
        //    {
        //        var query = objCompassEntities.PROC_Report_InventryStockReport().ToList();
        //        if (projectId != 0)
        //        {
        //            //query= query.Where(a=>a.pr)
        //            List<PROC_Report_InventryStockReport_Result> objList = new List<PROC_Report_InventryStockReport_Result>();
        //            var UserList = objCompassEntities.TblProjectInstallers.Where(o => o.ProjectID == projectId).ToList();

        //            foreach (var item in UserList)
        //            {
        //                var data = query.Where(o => o.UserID == item.tblUsers_UserID).ToList();
        //                objList.AddRange(data);
        //            }
        //            query = objList;
        //        }
        //        if (installerId != 0)
        //        {
        //            query = query.Where(a => a.AllocatedTo == installerId).ToList();
        //        }
        //        if (categoryId != 0)
        //        {
        //            query = query.Where(a => a.CategoryId == categoryId).ToList();
        //        }
        //        if (ProductId != 0)
        //        {
        //            query = query.Where(a => a.FKProductId == ProductId).ToList();
        //        }
        //        if (!string.IsNullOrEmpty(txtFromDate) && !string.IsNullOrEmpty(txtToDate))
        //        {
        //            DateTime fromDate = Convert.ToDateTime(txtFromDate);
        //            DateTime toDate = Convert.ToDateTime(txtToDate);
        //            query = query.Where(a => a.CreatedOn.Date >= fromDate.Date && a.CreatedOn.Date <= toDate.Date).ToList();

        //        }
        //        reportData = query;

        //        if (installerId == 0)
        //        {
        //            reportData = reportData.OrderBy(a => a.serialnumber).ThenBy(a => a.CreatedOn).ToList();
        //        }
        //        else
        //        {
        //            reportData = reportData.OrderBy(a => a.InstallerName).ThenBy(a => a.serialnumber).ThenBy(a => a.CreatedOn).ToList();
        //        }

        //        //reportData = query.ToList();
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //    return reportData;


        //}


        public List<PROC_InstallStockDetailReport_Result> GetInventryStockReport(long projectId, long installerId, long ProductId, string txtFromDate, string txtToDate)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            objCompassEntities.Database.CommandTimeout = 0;

            List<PROC_InstallStockDetailReport_Result> reportData = new List<PROC_InstallStockDetailReport_Result>();
            try
            {
                DateTime _fromDate = DateTime.Parse(txtFromDate);
                DateTime _toDate = DateTime.Parse(txtToDate);

                var query = objCompassEntities.PROC_InstallStockDetailReport(projectId, _fromDate, _toDate).ToList();
                if (projectId != 0)
                {
                    //query= query.Where(a=>a.pr)
                    List<PROC_InstallStockDetailReport_Result> objList = new List<PROC_InstallStockDetailReport_Result>();
                    var UserList = objCompassEntities.TblProjectInstallers.Where(o => o.ProjectID == projectId).ToList();

                    foreach (var item in UserList)
                    {
                        var data = query.Where(o => o.UserId == item.tblUsers_UserID).ToList();
                        objList.AddRange(data);
                    }
                    query = objList;
                }
                if (installerId != 0)
                {
                    query = query.Where(a => a.UserId == installerId).ToList();
                }
                //if (categoryId != 0)
                //{
                //    query = query.Where(a => a.CategoryId == categoryId).ToList();
                //}
                if (ProductId != 0)
                {
                    query = query.Where(a => a.FKProductId == ProductId).ToList();
                }
                //if (!string.IsNullOrEmpty(txtFromDate) && !string.IsNullOrEmpty(txtToDate))
                //{
                //    DateTime fromDate = Convert.ToDateTime(txtFromDate);
                //    DateTime toDate = Convert.ToDateTime(txtToDate);
                //    query = query.Where(a => a.CreatedOn.Date >= fromDate.Date && a.CreatedOn.Date <= toDate.Date).ToList();

                //}

                DateTime _startDate = DateTime.Parse("01-Jan-2018");

                reportData = query;
                List<PROC_InstallStockDetailReport_Result> objOldDataList = new List<PROC_InstallStockDetailReport_Result>();
                if (_fromDate.Date > _startDate.Date)
                {

                    DateTime _oldEndDate = _fromDate.AddDays(-1);


                    List<PROC_InstallStockDetailReportOldData_Result> reportData1 = new List<PROC_InstallStockDetailReportOldData_Result>();
                    var query1 = objCompassEntities.PROC_InstallStockDetailReportOldData(_startDate, _oldEndDate, _fromDate, _toDate, projectId).ToList();
                    if (projectId != 0)
                    {
                        //query= query.Where(a=>a.pr)
                        List<PROC_InstallStockDetailReportOldData_Result> objList1 = new List<PROC_InstallStockDetailReportOldData_Result>();
                        var UserList = objCompassEntities.TblProjectInstallers.Where(o => o.ProjectID == projectId).ToList();

                        foreach (var item in UserList)
                        {
                            var data1 = query1.Where(o => o.UserId == item.tblUsers_UserID).ToList();
                            objList1.AddRange(data1);
                        }
                        query1 = objList1;
                    }
                    if (installerId != 0)
                    {
                        query1 = query1.Where(a => a.UserId == installerId).ToList();
                    }
                    //if (categoryId != 0)
                    //{
                    //    query1 = query1.Where(a => a.CategoryId == categoryId).ToList();
                    //}
                    if (ProductId != 0)
                    {
                        query1 = query1.Where(a => a.FKProductId == ProductId).ToList();
                    }


                    objOldDataList = query1.Select(o =>
                        new PROC_InstallStockDetailReport_Result
                        {
                            CategoryId = o.CategoryId,
                            AllocatedQty = o.AllocatedQty.Value,
                            CategoryName = o.CategoryName,
                            FKProductId = o.FKProductId,
                            Id = o.Id,
                            InHandQty = o.InHandQty,
                            InstalledQty = o.InstalledQty,
                            ProductName = o.ProductName,
                            ReturnedQty = o.ReturnedQty,
                            SerialNumber = o.SerialNumber,
                            TransferredQty = o.TransferredQty,
                            UserId = o.UserId,
                            UserName = o.UserName

                        }).ToList();

                }




                if (installerId == 0)
                {

                    reportData = reportData.OrderBy(a => a.UserName).ThenBy(a => a.SerialNumber).ToList();

                }

                if (objOldDataList != null)
                {
                    if (reportData == null)
                    {
                        reportData = new List<PROC_InstallStockDetailReport_Result>();
                    }
                    reportData.AddRange(objOldDataList);
                }

                //reportData = query.ToList();
            }
            catch (Exception ex)
            {

                throw;
            }

            return reportData;


        }




        //public List<PROC_Report_InventryStockReport_Summary_Result> GetInventryStockReportSummery(long projectId, long installerId, long categoryId, long ProductId, string txtFromDate, string txtToDate)
        //{
        //    CompassEntities objCompassEntities = new CompassEntities();
        //    List<PROC_Report_InventryStockReport_Summary_Result> reportData = new List<PROC_Report_InventryStockReport_Summary_Result>();
        //    try
        //    {
        //        DateTime? fromdate = Convert.ToDateTime(txtFromDate);
        //        DateTime? todate = Convert.ToDateTime(txtToDate);
        //        var query = objCompassEntities.PROC_Report_InventryStockReport_Summary(fromdate, todate).ToList();
        //        if (projectId != 0)
        //        {
        //            //query= query.Where(a=>a.pr)
        //            List<PROC_Report_InventryStockReport_Summary_Result> objList = new List<PROC_Report_InventryStockReport_Summary_Result>();
        //            var UserList = objCompassEntities.TblProjectInstallers.Where(o => o.ProjectID == projectId).ToList();

        //            foreach (var item in UserList)
        //            {
        //                var data = query.Where(o => o.UserID == item.tblUsers_UserID);
        //                objList.AddRange(data);
        //            }
        //            query = objList;
        //        }
        //        if (installerId != 0)
        //        {
        //            query = query.Where(a => a.AllocatedTo == installerId).ToList();
        //        }
        //        if (categoryId != 0)
        //        {
        //            query = query.Where(a => a.CategoryId == categoryId).ToList();
        //        }
        //        if (ProductId != 0)
        //        {
        //            query = query.Where(a => a.FKProductId == ProductId).ToList();
        //        }

        //        reportData = query;


        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //    return reportData;


        //}

        // open inventory get in list
        public List<OpeningInventoryStockReportSummaryBO> GetOpeningInventryStockReportSummery(long projectId, long installerId, long ProductId, DateTime Defualtfromdate, DateTime toDateOneDayBefore, List<OpeningInventoryStockReportSummaryBO> openinginventoryreportData)
        {

            CompassEntities objCompassEntities = new CompassEntities();
            objCompassEntities.Database.CommandTimeout = 0;

            //List<OpeningInventoryStockReportSummaryBO> openinginventoryreportData = new List<OpeningInventoryStockReportSummaryBO>();
            try
            {
                var query = objCompassEntities.PROC_InstallStockSummaryReport_Project(Defualtfromdate, toDateOneDayBefore, projectId).ToList();
                List<OpeningInventoryStockReportSummaryBO> objList = new List<OpeningInventoryStockReportSummaryBO>();
                var UserList = objCompassEntities.TblProjectInstallers.Where(o => o.ProjectID == projectId).ToList();
                foreach (var item in UserList)
                {
                    var data = query.Where(o => o.UserId == item.tblUsers_UserID).Select(o =>
                        new OpeningInventoryStockReportSummaryBO
                        {
                            UserId = o.UserId,
                            UserName = o.FirstName,
                            FKProductId = o.FKProductId,
                            ProductCategoryId = o.ProductCategoryId.Value,
                            CategoryName = o.CategoryName,
                            ProductName = o.ProductName,
                            OpeningBalance = o.InHandInventory
                        }
                        ).ToList();

                    objList.AddRange(data);
                }
                openinginventoryreportData = objList;
                if (installerId != 0)
                {
                    openinginventoryreportData = openinginventoryreportData.Where(a => a.UserId == installerId).ToList();
                }

                if (ProductId != 0)
                {
                    openinginventoryreportData = openinginventoryreportData.Where(a => a.FKProductId == ProductId).ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return openinginventoryreportData;
        }
        //

        public List<InstallerStockReportSummaryBO> GetInventryStockReportSummery(long projectId, long installerId, long ProductId, string txtFromDate, string txtToDate)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            objCompassEntities.Database.CommandTimeout = 0;
            List<InstallerStockReportSummaryBO> reportData = new List<InstallerStockReportSummaryBO>();
            List<TempInstallerStockReportSummaryBO> tempReportData = new List<TempInstallerStockReportSummaryBO>();
            try
            {
                DateTime? fromdate = Convert.ToDateTime(txtFromDate);
                DateTime? todate = Convert.ToDateTime(txtToDate);

                // Add opening inventory code
                //var projectStartDate = objCompassEntities.tblProjects.Where(o => o.ProjectId == projectId).Select(o => o.FromDate).First();
                DateTime Defualtfromdate = Convert.ToDateTime("Aug-01-2017");
                //DateTime Defualtfromdate = Convert.ToDateTime(projectStartDate.ToString());
                DateTime fromdateOneDayBefore = Convert.ToDateTime(txtFromDate);
                DateTime toDateOneDayBefore = fromdateOneDayBefore.AddDays(-1);
                List<OpeningInventoryStockReportSummaryBO> openinginventoryreportData = new List<OpeningInventoryStockReportSummaryBO>();
                openinginventoryreportData = GetOpeningInventryStockReportSummery(projectId, installerId, ProductId, Defualtfromdate, toDateOneDayBefore, openinginventoryreportData);
                // ends hare

                #region  For single project
                var query = objCompassEntities.PROC_InstallStockSummaryReport_Project(fromdate, todate, projectId).ToList();
                List<TempInstallerStockReportSummaryBO> objList = new List<TempInstallerStockReportSummaryBO>();
                var UserList = objCompassEntities.TblProjectInstallers.Where(o => o.ProjectID == projectId).ToList();

                foreach (var item in UserList)
                {
                    var data = query.Where(o => o.UserId == item.tblUsers_UserID).Select(o =>
                        new TempInstallerStockReportSummaryBO
                        {
                            UserId = o.UserId,
                            UserName = o.FirstName,
                            FKProductId = o.FKProductId,
                            ProductCategoryId = o.ProductCategoryId.Value,
                            ProductName = o.ProductName,
                            CategoryName = o.CategoryName,
                            TotalRelasedQty = o.TotalRelasedQty,
                            InstalledQty = o.InstalledQty,
                            ReturnedQty = o.ReturnedQty,
                            TransferredQty = o.TransferredQty,
                            InHandInventory = o.InHandInventory
                        }
                        ).ToList();

                    objList.AddRange(data);
                }
                tempReportData = objList;

                #endregion
                if (installerId != 0)
                {
                    tempReportData = tempReportData.Where(a => a.UserId == installerId).ToList();
                }
                if (ProductId != 0)
                {
                    tempReportData = tempReportData.Where(a => a.FKProductId == ProductId).ToList();
                }

                // Merge two list data get in common records in a list
                reportData = (from op in openinginventoryreportData
                              join o in tempReportData
                                  on op.ProductName equals o.ProductName where op.UserId == o.UserId
                              select new InstallerStockReportSummaryBO
                              {
                                  UserId = o.UserId,
                                  UserName = o.UserName,
                                  FKProductId = o.FKProductId,
                                  ProductCategoryId = o.ProductCategoryId,
                                  ProductName = o.ProductName,
                                  CategoryName = o.CategoryName,
                                  OpeningBalance = op.OpeningBalance,
                                  TotalRelasedQty = o.TotalRelasedQty,
                                  InstalledQty = o.InstalledQty,
                                  ReturnedQty = o.ReturnedQty,
                                  TransferredQty = o.TransferredQty,
                                  InHandInventory = o.InHandInventory + op.OpeningBalance
                              }).ToList();

                // Remove common recoreds in both list
                foreach (var pro in reportData)
                {
                    var itemToRemove = tempReportData.Single(p => p.ProductName == pro.ProductName && p.UserId == pro.UserId);
                    tempReportData.Remove(itemToRemove);
                    var itemToRemove2 = openinginventoryreportData.Single(p => p.ProductName == pro.ProductName && p.UserId == pro.UserId);
                    openinginventoryreportData.Remove(itemToRemove2);
                }
                
                foreach (var o in tempReportData)
                {
                    reportData.Add(new InstallerStockReportSummaryBO
                    {
                        UserId = o.UserId,
                        UserName = o.UserName,
                        FKProductId = o.FKProductId,
                        ProductCategoryId = o.ProductCategoryId,
                        ProductName = o.ProductName,
                        CategoryName = o.CategoryName,
                        OpeningBalance = 0, // InstallerStockReportSummary with not comming opening balance
                        TotalRelasedQty = o.TotalRelasedQty,
                        InstalledQty = o.InstalledQty,
                        ReturnedQty = o.ReturnedQty,
                        TransferredQty = o.TransferredQty,
                        InHandInventory = o.InHandInventory
                    });
                }
                foreach (var o in openinginventoryreportData)
                {
                    if (o.OpeningBalance > 0)
                    {
                        reportData.Add(new InstallerStockReportSummaryBO
                        {
                            UserId = o.UserId,
                            UserName = o.UserName,
                            FKProductId = o.FKProductId,
                            ProductCategoryId = o.ProductCategoryId,
                            ProductName = o.ProductName,
                            CategoryName = o.CategoryName,
                            OpeningBalance = o.OpeningBalance,
                            TotalRelasedQty = 0,
                            InstalledQty = 0,
                            ReturnedQty = 0,
                            TransferredQty = 0,
                            InHandInventory = 0 + o.OpeningBalance
                        });
                    }
                }
                // ends hare
            }
            catch (Exception ex)
            {
                throw;
            }
            return reportData;
        }




        //public List<InventryStockReportExport> ExportInventryStockReport(long projectId, long installerId, long categoryId, long ProductId, string txtFromDate, string txtToDate)
        //{
        //    CompassEntities objCompassEntities = new CompassEntities();
        //    List<InventryStockReportExport> reportData = new List<InventryStockReportExport>();
        //    try
        //    {
        //        var query = objCompassEntities.PROC_Report_InventryStockReport().AsEnumerable();
        //        if (projectId != 0)
        //        {
        //            //query= query.Where(a=>a.pr)
        //            List<PROC_Report_InventryStockReport_Result> objList = new List<PROC_Report_InventryStockReport_Result>();
        //            var UserList = objCompassEntities.TblProjectInstallers.Where(o => o.ProjectID == projectId).ToList();

        //            foreach (var item in UserList)
        //            {
        //                var data = query.Where(o => o.UserID == item.tblUsers_UserID).ToList();
        //                objList.AddRange(data);
        //            }
        //            query = objList;
        //        }
        //        if (installerId != 0)
        //        {
        //            query = query.Where(a => a.AllocatedTo == installerId);
        //        }
        //        if (categoryId != 0)
        //        {
        //            query = query.Where(a => a.CategoryId == categoryId);
        //        }
        //        if (ProductId != 0)
        //        {
        //            query = query.Where(a => a.FKProductId == ProductId);
        //        }
        //        if (!string.IsNullOrEmpty(txtFromDate) && !string.IsNullOrEmpty(txtToDate))
        //        {
        //            DateTime fromDate = Convert.ToDateTime(txtFromDate);
        //            DateTime toDate = Convert.ToDateTime(txtToDate);
        //            query = query.Where(a => a.CreatedOn.Date >= fromDate.Date && a.CreatedOn.Date <= toDate.Date);

        //        }
        //        reportData = query.Select(a => new InventryStockReportExport
        //        {
        //            InstallerName = a.InstallerName,
        //            CategoryName = a.CategoryName,
        //            SerialNumber = a.serialnumber,
        //            ScannedInventry = a.ScannedInventry,
        //            InstalledInventry = a.InstalledInventry,
        //            TransferredInventry = a.TransferredInventry,
        //            ReturnedInventry = a.ReturnedInventry,
        //            InventryInHand = a.InventryInHand,

        //        }).ToList();
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //    return reportData;


        //}



        public List<InventryStockReportExport> ExportInventryStockReport(long projectId, long installerId, long categoryId, long ProductId, string txtFromDate, string txtToDate)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            List<InventryStockReportExport> reportData = new List<InventryStockReportExport>();
            try
            {
                var query = objCompassEntities.PROC_Report_InventryStockReport().AsEnumerable();
                if (projectId != 0)
                {
                    //query= query.Where(a=>a.pr)
                    List<PROC_Report_InventryStockReport_Result> objList = new List<PROC_Report_InventryStockReport_Result>();
                    var UserList = objCompassEntities.TblProjectInstallers.Where(o => o.ProjectID == projectId).ToList();

                    foreach (var item in UserList)
                    {
                        var data = query.Where(o => o.UserID == item.tblUsers_UserID).ToList();
                        objList.AddRange(data);
                    }
                    query = objList;
                }
                if (installerId != 0)
                {
                    query = query.Where(a => a.AllocatedTo == installerId);
                }
                if (categoryId != 0)
                {
                    query = query.Where(a => a.CategoryId == categoryId);
                }
                if (ProductId != 0)
                {
                    query = query.Where(a => a.FKProductId == ProductId);
                }
                if (!string.IsNullOrEmpty(txtFromDate) && !string.IsNullOrEmpty(txtToDate))
                {
                    DateTime fromDate = Convert.ToDateTime(txtFromDate);
                    DateTime toDate = Convert.ToDateTime(txtToDate);
                    query = query.Where(a => a.CreatedOn.Date >= fromDate.Date && a.CreatedOn.Date <= toDate.Date);

                }
                reportData = query.Select(a => new InventryStockReportExport
                {
                    InstallerName = a.InstallerName,
                    CategoryName = a.CategoryName,
                    SerialNumber = a.serialnumber,
                    ScannedInventry = a.ScannedInventry,
                    InstalledInventry = a.InstalledInventry,
                    TransferredInventry = a.TransferredInventry,
                    ReturnedInventry = a.ReturnedInventry,
                    InventryInHand = a.InventryInHand,

                }).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }

            return reportData;


        }










        public List<InventryStockSummaryReportExport> ExportInventryStockSummeryReport(long projectId, long installerId, long categoryId, long ProductId, string txtFromDate, string txtToDate)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            List<InventryStockSummaryReportExport> reportData = new List<InventryStockSummaryReportExport>();
            try
            {
                DateTime? fromdate = Convert.ToDateTime(txtFromDate);
                DateTime? todate = Convert.ToDateTime(txtToDate);
                var query = objCompassEntities.PROC_Report_InventryStockReport_Summary(fromdate, todate).AsEnumerable();
                if (projectId != 0)
                {
                    //query= query.Where(a=>a.pr)
                    List<PROC_Report_InventryStockReport_Summary_Result> objList = new List<PROC_Report_InventryStockReport_Summary_Result>();
                    var UserList = objCompassEntities.TblProjectInstallers.Where(o => o.ProjectID == projectId).ToList();

                    foreach (var item in UserList)
                    {
                        var data = query.Where(o => o.UserID == item.tblUsers_UserID).ToList();
                        objList.AddRange(data);
                    }
                    query = objList;
                }
                if (installerId != 0)
                {
                    query = query.Where(a => a.AllocatedTo == installerId);
                }
                if (categoryId != 0)
                {
                    query = query.Where(a => a.CategoryId == categoryId);
                }
                if (ProductId != 0)
                {
                    query = query.Where(a => a.FKProductId == ProductId);
                }

                reportData = query.Select(a => new InventryStockSummaryReportExport
                {
                    InstallerName = a.InstallerName,
                    CategoryName = a.CategoryName,
                    ScannedInventry = a.ScannedInventry,
                    InstalledInventry = a.InstalledInventry,
                    TransferredInventry = a.TransferredInventry,
                    ReturnedInventry = a.ReturnedInventry,
                    InventryInHand = a.InventryInHand,

                }).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }

            return reportData;


        }


        public UploadInventoryBO CheckValidData(long ProjectId, UploadInventoryBO inventoryRecord, List<UploadInventoryBO> recordList)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            List<UploadInventoryBO> objInventoryUploadModelList = new List<UploadInventoryBO>();
            long utilityId = Convert.ToInt64(inventoryRecord.UtilityId);
            long warehouseId = Convert.ToInt64(inventoryRecord.WarehouseId);

            var ExistingProductsForProject = objCompassEntities.ProjectServiceProductRelations.AsNoTracking().Where(o => o.Active == 1 && o.FKProjectID == ProjectId).ToList();


            try
            {
                var warehouseName = objCompassEntities.WarehouseMasters.Where(a => a.Id == warehouseId).Select(a => a.WarehouseName).FirstOrDefault();
                var utilityName = objCompassEntities.UtilityMasters.Where(a => a.Id == utilityId).Select(a => a.UtilityName).FirstOrDefault();

                var categoryList = objCompassEntities.Categories.Where(a => a.Active == 1).ToList();

                VW_GetProductData productModel = new VW_GetProductData();
                UploadInventoryBO uploadInventoryexist = new UploadInventoryBO();

                var UtilityType = inventoryRecord.UtilityType != null ? inventoryRecord.UtilityType.Trim() : inventoryRecord.UtilityType;
                var CategoryName = inventoryRecord.CategoryName != null ? inventoryRecord.CategoryName.Trim() : inventoryRecord.CategoryName;
                var ProductName = inventoryRecord.ProductName != null ? inventoryRecord.ProductName.Trim() : inventoryRecord.ProductName;
                var Make = inventoryRecord.Make != null ? inventoryRecord.Make.Trim() : inventoryRecord.Make;
                var Type = inventoryRecord.Type != null ? inventoryRecord.Type.Trim() : inventoryRecord.Type;
                var Size = inventoryRecord.Size != null ? inventoryRecord.Size.Trim() : inventoryRecord.Size;
                var PartNumber = inventoryRecord.PartNumber != null ? inventoryRecord.PartNumber.Trim() : inventoryRecord.PartNumber;
                var UPCCode = inventoryRecord.UPCCode != null ? inventoryRecord.UPCCode.Trim() : inventoryRecord.UPCCode;
                var SerialNumber = inventoryRecord.SerialNumber != null ? inventoryRecord.SerialNumber.Trim() : inventoryRecord.SerialNumber;
                var Quantity = inventoryRecord.Quantity;



                uploadInventoryexist.WarehouseId = warehouseId;
                uploadInventoryexist.WarehouseName = warehouseName;
                uploadInventoryexist.UtilityId = utilityId;
                uploadInventoryexist.UtilityType = UtilityType;
                uploadInventoryexist.Make = Make;
                uploadInventoryexist.Type = Type;
                uploadInventoryexist.ProductName = ProductName;
                uploadInventoryexist.PartNumber = PartNumber;
                uploadInventoryexist.UPCCode = UPCCode;
                uploadInventoryexist.Size = Size;
                uploadInventoryexist.SerialNumber = SerialNumber;


                Category categoryexist = new Category();
                bool productinCategory = false;
                if (!string.IsNullOrEmpty(CategoryName))
                {
                    categoryexist = categoryList.Where(a => a.CategoryName.Trim().ToLower() == CategoryName.ToLower()).FirstOrDefault();
                    if (categoryexist != null && !string.IsNullOrEmpty(ProductName))
                    {
                        var productexist = objCompassEntities.ProductMasters.AsNoTracking().Where(a => a.Active == 1 && a.ProductCategoryId == categoryexist.CategoryId && ProductName.ToLower() == a.ProductName.Trim().ToLower()).ToList();
                        if (productexist != null && productexist.Count > 0)
                        {
                            productinCategory = true;
                        }
                    }
                }


                if (string.IsNullOrEmpty(UtilityType))
                {
                    uploadInventoryexist.InventryStatus = 2;  //Invalid data
                    uploadInventoryexist.IsDuplicate = "Utility Type required";

                    return uploadInventoryexist;
                }
                else if (utilityName.ToLower() != UtilityType.ToLower())
                {
                    uploadInventoryexist.InventryStatus = 2;  //Invalid data
                    uploadInventoryexist.IsDuplicate = "Wrong Utility Type";
                    return uploadInventoryexist;
                }
                else if (utilityName.Length > 50)
                {
                    uploadInventoryexist.InventryStatus = 2;  //Invalid data
                    uploadInventoryexist.IsDuplicate = "Utility Type name must be less than 50 characters";
                    return uploadInventoryexist;
                }
                else if (string.IsNullOrEmpty(CategoryName))
                {
                    uploadInventoryexist.InventryStatus = 2;  //Invalid data
                    uploadInventoryexist.IsDuplicate = "Category Name required";
                    return uploadInventoryexist;
                }
                else if (categoryexist == null)
                {
                    uploadInventoryexist.InventryStatus = 2;  //Invalid data
                    uploadInventoryexist.IsDuplicate = "Wrong Category Name";
                    return uploadInventoryexist;
                }
                else if (string.IsNullOrEmpty(ProductName))
                {
                    uploadInventoryexist.InventryStatus = 2;  //Invalid data
                    uploadInventoryexist.IsDuplicate = "Product Name required";
                    return uploadInventoryexist;
                }
                else if (ProductName.Length > 250)
                {
                    uploadInventoryexist.InventryStatus = 2;  //Invalid data
                    uploadInventoryexist.IsDuplicate = "Product Name must be less than 250 characters";
                    return uploadInventoryexist;
                }
                else if (productinCategory == false)
                {
                    uploadInventoryexist.InventryStatus = 2;  //Invalid data
                    uploadInventoryexist.IsDuplicate = "Product does't exist in category";
                    return uploadInventoryexist;
                }
                else if (string.IsNullOrEmpty(PartNumber))
                {
                    uploadInventoryexist.InventryStatus = 2;  //Invalid data
                    uploadInventoryexist.IsDuplicate = "Part Number required";
                    return uploadInventoryexist;
                }

                else if (!string.IsNullOrEmpty(PartNumber))
                {
                    var productexist = objCompassEntities.ProductMasters.Where(a => a.Active == 1 && a.PartNumber == PartNumber && ProductName.ToLower() == a.ProductName.Trim().ToLower()).FirstOrDefault();
                    if (productexist == null)
                    {
                        uploadInventoryexist.InventryStatus = 2;  //Invalid data
                        uploadInventoryexist.IsDuplicate = "Wronge Part Number";
                        return uploadInventoryexist;
                    }
                }

                else if (!string.IsNullOrEmpty(UPCCode))
                {
                    var productexist = objCompassEntities.ProductMasters.Where(a => a.Active == 1 && a.UPCCode == UPCCode && ProductName.ToLower() == a.ProductName.Trim().ToLower()).FirstOrDefault();
                    if (productexist == null)
                    {
                        uploadInventoryexist.InventryStatus = 2;  //Invalid data
                        uploadInventoryexist.IsDuplicate = "Wronge UPC Code";
                        return uploadInventoryexist;
                    }

                }
                if (string.IsNullOrEmpty(Make))
                {
                    uploadInventoryexist.InventryStatus = 2;  //Invalid data
                    uploadInventoryexist.IsDuplicate = "Product Make required";
                    return uploadInventoryexist;
                }
                //else if (!string.IsNullOrEmpty(Make))
                //{
                //    var productexist = objCompassEntities.ProductMasters.Where(a => a.Active == 1 && a.mak == UPCCode && ProductName.ToLower() == a.ProductName.Trim().ToLower()).FirstOrDefault();
                //    if (productexist == null)
                //    {
                //        uploadInventoryexist.InventryStatus = 2;  //Invalid data
                //        uploadInventoryexist.IsDuplicate = "Wronge UPC Code";
                //        return uploadInventoryexist;
                //    }

                //}


                else if (Make.Length > 250)
                {
                    uploadInventoryexist.InventryStatus = 2;  //Invalid data
                    uploadInventoryexist.IsDuplicate = "Product Make must be less than 250 characters";
                    return uploadInventoryexist;
                }
                else if (Quantity == 0)
                {
                    uploadInventoryexist.InventryStatus = 2;  //Invalid data
                    uploadInventoryexist.IsDuplicate = "Quantity required";
                    return uploadInventoryexist;
                }
                else if (Quantity.ToString().Length > 4)
                {
                    uploadInventoryexist.InventryStatus = 2;  //Invalid data
                    uploadInventoryexist.IsDuplicate = "Quantity must be less than 4 characters";
                    return uploadInventoryexist;
                }

                else
                {
                    #region check serialnumber exist


                    if (!string.IsNullOrEmpty(ProductName) && !string.IsNullOrEmpty(Make) && !string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Size))
                    {
                        productModel = objCompassEntities.VW_GetProductData.AsEnumerable()
                       .Where(a => a.UtilityId == utilityId
                        && a.ProductName.Trim().ToLower() == ProductName.ToLower()
                        && a.Make != null && a.Make.Trim().ToLower() == Make.ToLower()
                        && a.MeterSize != null && a.MeterSize.Trim() == Size
                        && a.MeterType != null && a.MeterType.Trim().ToLower() == Type.ToLower()).FirstOrDefault();
                    }

                    else if (!string.IsNullOrEmpty(ProductName) && !string.IsNullOrEmpty(Make) && !string.IsNullOrEmpty(Type))
                    {
                        productModel = objCompassEntities.VW_GetProductData.AsEnumerable()
                       .Where(a => a.UtilityId == utilityId
                        && a.ProductName.Trim().ToLower() == ProductName.ToLower()
                        && a.Make != null && a.Make.Trim().ToLower() == Make.ToLower()
                        && a.MeterType != null && a.MeterType.Trim().ToLower() == Type.ToLower()).FirstOrDefault();
                    }
                    else if (!string.IsNullOrEmpty(ProductName) && !string.IsNullOrEmpty(Make) && !string.IsNullOrEmpty(Size))
                    {
                        productModel = objCompassEntities.VW_GetProductData.AsEnumerable()
                       .Where(a => a.UtilityId == utilityId
                        && a.ProductName.Trim().ToLower() == ProductName.ToLower()
                        && a.Make
                        != null && a.Make.Trim().ToLower() == Make.ToLower()
                        && a.MeterSize != null && a.MeterSize.Trim() == Size).FirstOrDefault();
                    }
                    else if (!string.IsNullOrEmpty(ProductName) && !string.IsNullOrEmpty(Make))
                    {
                        productModel = objCompassEntities.VW_GetProductData.AsEnumerable()
                       .Where(a => a.UtilityId == utilityId
                        && a.ProductName.Trim().ToLower() == ProductName.ToLower()
                        && a.Make != null && a.Make.Trim().ToLower() == Make.ToLower()).FirstOrDefault();
                    }
                    if (productModel != null && !string.IsNullOrEmpty(ProductName) && !string.IsNullOrEmpty(UPCCode))
                    {
                        productModel = objCompassEntities.VW_GetProductData.AsEnumerable()
                       .Where(a => a.UtilityId == utilityId
                        && a.ProductName.Trim().ToLower() == ProductName.ToLower()
                        && a.UPCCode != null && a.UPCCode.Trim().ToLower() == UPCCode.ToLower()
                       ).FirstOrDefault();
                    }

                    if (productModel != null)
                    {

                        uploadInventoryexist.InventryStatus = 0;  //valid data
                        uploadInventoryexist.IsDuplicate = "Successful";

                        if (productModel.SerialNumberRequired && string.IsNullOrEmpty(SerialNumber))
                        {
                            uploadInventoryexist.InventryStatus = 2;  //Invalid data
                            uploadInventoryexist.IsDuplicate = "Serial number required";
                            return uploadInventoryexist;
                        }
                        else if (productModel.SerialNumberRequired && SerialNumber.Length > 50)
                        {
                            uploadInventoryexist.InventryStatus = 2;  //Invalid data
                            uploadInventoryexist.IsDuplicate = "Serial Number must be less than 50 characters";
                            return uploadInventoryexist;
                        }
                        else if (productModel.SerialNumberRequired && Convert.ToInt32(Quantity) != 1)
                        {
                            uploadInventoryexist.InventryStatus = 2;  //Invalid data
                            uploadInventoryexist.IsDuplicate = "Invalid Quantity,Quantity must be 1";
                            return uploadInventoryexist;
                        }
                        else if (productModel.SerialNumberRequired)
                        {
                            var serialexistintemp = recordList.Where(a => a.InventryStatus == 0 && a.SerialNumber == SerialNumber.Trim()).FirstOrDefault();
                            if (serialexistintemp != null)
                            {
                                uploadInventoryexist.InventryStatus = 1;  //already exist
                                uploadInventoryexist.IsDuplicate = "Duplicate Serial number";
                                return uploadInventoryexist;
                            }
                            else
                            {
                                var a = ExistingProductsForProject.Where(o => o.FKProductId == productModel.Id).FirstOrDefault();
                                if (a != null)
                                {
                                    var serialexist = objCompassEntities.StockDetails.AsNoTracking().
                                        Where(o => o.Active == 1 && o.FKProjectId == ProjectId && o.SerialNumber == SerialNumber.Trim()).FirstOrDefault();
                                    if (serialexist != null)
                                    {
                                        uploadInventoryexist.InventryStatus = 1;  //already exist
                                        uploadInventoryexist.IsDuplicate = "Serial number already added for the project.";
                                    }
                                }
                                else
                                {
                                    uploadInventoryexist.InventryStatus = 2;  //already exist
                                    uploadInventoryexist.IsDuplicate = "Product not mapped for the project.";
                                }
                            }

                        }
                        else if (productModel.SerialNumberRequired == false)
                        {
                            var a = ExistingProductsForProject.Where(o => o.FKProductId == productModel.Id).FirstOrDefault();
                            if (a == null)
                            {
                                uploadInventoryexist.InventryStatus = 2;  //already exist
                                uploadInventoryexist.IsDuplicate = "Product not mapped for the project.";
                            }
                        }
                    }
                    else
                    {

                        uploadInventoryexist.InventryStatus = 2;  //Invalid data
                        uploadInventoryexist.IsDuplicate = "Product not exist";
                        return uploadInventoryexist;

                    }

                    #endregion
                }

                if (uploadInventoryexist.InventryStatus == 0)
                {
                    // uploadInventoryexist.UniqueId = Guid.NewGuid();
                    uploadInventoryexist.CategoryName = productModel.CategoryName;
                    uploadInventoryexist.ProductId = productModel.Id;
                    uploadInventoryexist.ProductDescription = productModel.ProductDescription;
                    uploadInventoryexist.IsSerialNumber = productModel.SerialNumberRequired;
                    uploadInventoryexist.Quantity = Convert.ToInt32(Quantity);
                    return uploadInventoryexist;
                }
                else
                {
                    return uploadInventoryexist;
                }


            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public List<MovementReport> GetProductMovementReport(long projectId, string serialnumber)
        {

            CompassEntities objCompassEntities = new CompassEntities();
            List<MovementReport> reportdata = new List<MovementReport>();
            try
            {


                //DataSet ds = new DataSet();
                //SqlConnection con = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString);
                //con.Open();
                //SqlCommand cmd = new SqlCommand("PROC_Report_ProductMovementReport", con);
                //cmd.Parameters.AddWithValue("@SerialNumber", serialnumber); // passing Datatable

                //cmd.CommandType = CommandType.StoredProcedure;

                //SqlDataAdapter da = new SqlDataAdapter();
                //da.SelectCommand = cmd;
                //da.Fill(ds);

                //cmd.ExecuteNonQuery();
                //con.Close();
                /*var serialHistory = objCompassEntities.SerialNumberSummary.Where(o => o.SerialNumber == serialnumber).Select(a => new MovementReport
                {
                    SerialNumber = a.SerialNumber,
                    StockStatus = a.StockStatus,
                    Location = a.Location,
                    StockMovementDate = a.StockMovementDate,
                    StockMovementDateString = a.StockMovementDate.Value.ToString("MMM-dd-yyyy h:mm tt"),

                    UserName = a.UserName,
                    LatLong = a.LatLong,
                    Account = a.Account,
                    Street = a.Street,
                    ProjectName = a.ProjectName

                }).ToList();
                if(serialHistory != null && serialHistory.Count > 0)
                {
                    reportdata.AddRange(serialHistory);
                }*/

                reportdata = objCompassEntities.PROC_Report_ProductMovementReport(projectId, serialnumber).Select(a => new MovementReport
                {
                    SerialNumber = a.SerialNumber,
                    StockStatus = a.StockStatus,
                    Location = a.Location,
                    StockMovementDate = a.StockMovementDate,
                    StockMovementDateString = a.StockMovementDate.Value.ToString("MMM-dd-yyyy h:mm tt"),

                    UserName = a.UserName,
                    LatLong = a.LatLong,
                    Account = a.Account,
                    Street = a.Street,
                    ProjectName = a.ProjectName

                }).ToList();
                //reportdata.AddRange(currentData);




            }
            catch (Exception ex)
            {

                throw;
            }
            return reportdata;
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




        public List<string> CheckData(long ProjectId, List<string> recordList)
        {
            List<string> objList = new List<string>();

            CompassEntities objCompassEntities = new CompassEntities();

            foreach (var item in recordList)
            {
                var serialexist = objCompassEntities.StockDetails.AsNoTracking().
                                      Where(o => o.Active == 1 && o.FKProjectId == ProjectId && o.SerialNumber == item.Trim()).FirstOrDefault();
                if (serialexist == null)
                {
                    objList.Add(item);
                }
            }

            return objList;
        }

    }


}
