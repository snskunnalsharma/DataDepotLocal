using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Security.Cryptography;
using System.Web;
using System.Security.AccessControl;
using System.Data;
using System.Data.SqlClient;
using log4net;
using System.Transactions;
using System.Data.Entity.Core.Objects;
using Microsoft.Reporting.WebForms;

namespace GarronT.CMS.Model.DAL
{
    public class InventoryWebServiceDAL : IDisposable
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



        #region get source data  methods



        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="CurrentDate"></param>
        /// <returns></returns>
        public List<InventorySourceBO> GetInventorySourceData(long UserId, DateTime CurrentDate, string userRoles)
        {
            List<InventorySourceBO> objInventorySourceBOList = new List<InventorySourceBO>();

            CompassEntities objCompassEntities = new CompassEntities();
            objInventorySourceBOList = objCompassEntities.InventorySourceMasters.AsNoTracking().
                Where(o => o.Active == 1).Select(o => new InventorySourceBO { Id = o.Id, InventorySourceName = o.InventorySourceName }).ToList();

            WebServiceUserRoleDAL objWebServiceUserRoleDAL = new WebServiceUserRoleDAL();
            foreach (var item in objInventorySourceBOList)
            {
                if (item.InventorySourceName.ToLower() == "warehouse")
                {
                    item.ObjInventorySourceChildBOList = new WareHouseDAL().GetWareHouseList();
                }
                else if (item.InventorySourceName.ToLower() == "manager")
                {
                    // 
                    var data = objWebServiceUserRoleDAL.GetAllUsersBasedOnRole(UserId, "Manager", CurrentDate);
                    if (userRoles.Contains("2"))
                    {
                        //current user is manager - get all data
                        item.ObjInventorySourceChildBOList = data;
                    }
                    else if (userRoles.Contains("3"))
                    {
                        //current user is field supervisor - get only his manager

                        var userManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == UserId).FirstOrDefault();
                        if (userManager != null)
                        {
                            //check user present
                            item.ObjInventorySourceChildBOList = data.Where(o => o.ChildId == userManager.ManagerId).ToList();
                        }
                    }
                    else if (userRoles.Contains("1"))
                    {
                        //current user is installer - get only his manager
                        var userManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == UserId).FirstOrDefault();
                        if (userManager != null)
                        {
                            //check user present
                            item.ObjInventorySourceChildBOList = data.Where(o => o.ChildId == userManager.ManagerId).ToList();
                        }
                    }
                }
                else if (item.InventorySourceName.ToLower() == "field supervisor")
                {
                    var data = objWebServiceUserRoleDAL.GetAllUsersBasedOnRole(UserId, "Field Supervisor", CurrentDate);


                    if (userRoles.Contains("2"))
                    {
                        //current user is manager - get all data
                        item.ObjInventorySourceChildBOList = data;
                    }
                    else if (userRoles.Contains("3"))
                    {
                        //current user is field supervisor - get only supervisors which are under current manager

                        var userManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == UserId).FirstOrDefault();
                        var data1 = new List<InventorySourceChildBO>();
                        if (userManager != null && userManager.ManagerId != null && data != null)
                        {
                            foreach (var item1 in data)
                            {
                                var d = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == item1.ChildId && o.ManagerId == userManager.ManagerId).FirstOrDefault();

                                if (d != null)
                                {
                                    data1.Add(item1);
                                }
                            }
                            //check user present
                            item.ObjInventorySourceChildBOList = data1;
                        }
                    }
                    else if (userRoles.Contains("1"))
                    {
                        //current user is  installer - get only supervisors which are under current manager
                        var userManager = objCompassEntities.tblUsers.Where(o => o.UserID == UserId).FirstOrDefault();

                        var data1 = new List<InventorySourceChildBO>();
                        if (userManager != null && userManager.ManagerId != null && data != null)
                        {
                            foreach (var item1 in data)
                            {
                                var d = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == item1.ChildId && o.ManagerId == userManager.ManagerId).FirstOrDefault();

                                if (d != null)
                                {
                                    data1.Add(item1);
                                }
                            }
                            //check user present
                            item.ObjInventorySourceChildBOList = data1;
                        }
                    }
                }
                else if (item.InventorySourceName.ToLower() == "installer")
                {
                    var data = objWebServiceUserRoleDAL.GetAllUsersBasedOnRole(UserId, "Installer", CurrentDate);

                    if (userRoles.Contains("2"))
                    {
                        //current user is manager - get all data
                        item.ObjInventorySourceChildBOList = data;
                    }
                    else if (userRoles.Contains("3"))
                    {
                        //current user is field supervisor - get only supervisors which are under current manager

                        var userManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == UserId).FirstOrDefault();
                        var data1 = new List<InventorySourceChildBO>();
                        if (userManager != null && userManager.ManagerId != null && data != null)
                        {
                            foreach (var item1 in data)
                            {
                                var d = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == item1.ChildId && o.ManagerId == userManager.ManagerId).FirstOrDefault();

                                if (d != null)
                                {
                                    data1.Add(item1);
                                }
                            }
                            //check user present
                            item.ObjInventorySourceChildBOList = data1;
                        }
                    }
                    else if (userRoles.Contains("1"))
                    {
                        //current user is  installer - get only supervisors which are under current manager
                        var userManager = objCompassEntities.tblUsers.Where(o => o.UserID == UserId).FirstOrDefault();

                        var data1 = new List<InventorySourceChildBO>();
                        if (userManager != null && userManager.ManagerId != null && data != null)
                        {
                            foreach (var item1 in data)
                            {
                                var d = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == item1.ChildId && o.ManagerId == userManager.ManagerId).FirstOrDefault();

                                if (d != null)
                                {
                                    data1.Add(item1);
                                }
                            }
                            //check user present
                            item.ObjInventorySourceChildBOList = data1;
                        }
                    }


                }
                else if (item.InventorySourceName.ToLower() == "warehouse manager")
                {
                    item.ObjInventorySourceChildBOList = objWebServiceUserRoleDAL.GetAllUsersBasedOnRole(UserId, "Warehouse Manager", CurrentDate);
                }
            }



            objCompassEntities.Dispose();
            objWebServiceUserRoleDAL.Dispose();
            return objInventorySourceBOList;
        }

        public List<InventorySourceBO> GetInventorySourceData(long projectId, long UserId, string userRoles)
        {
            List<InventorySourceBO> objInventorySourceBOList = new List<InventorySourceBO>();

            CompassEntities objCompassEntities = new CompassEntities();
            objInventorySourceBOList = objCompassEntities.InventorySourceMasters.
                Where(o => o.Active == 1 && o.InventorySourceName != "Warehouse Manager").Select(o => new InventorySourceBO { Id = o.Id, InventorySourceName = o.InventorySourceName }).ToList();

            WebServiceUserRoleDAL objWebServiceUserRoleDAL = new WebServiceUserRoleDAL();
            foreach (var item in objInventorySourceBOList)
            {
                if (item.InventorySourceName.ToLower() == "warehouse")
                {
                    // item.ObjInventorySourceChildBOList = new WareHouseDAL().GetWareHouseList();
                    List<long> listObj = objCompassEntities.ProjectWarehouseRelations.AsNoTracking().Where(o => o.Active == 1 && o.FKProjectId == projectId).Select(o => o.FKWarehouseId).ToList();
                    item.ObjInventorySourceChildBOList = objCompassEntities.WarehouseMasters.Where(o => o.Active == 1 && listObj.Contains(o.Id)).Select(o =>
                            new InventorySourceChildBO
                            {
                                ChildId = o.Id,
                                ChildName = o.WarehouseLocation + ", " + o.WarehouseName + ", " + o.TblCityMaster.CityName + ", " + o.tblStateMaster.StateName
                            }
                            ).ToList();
                }
                else if (item.InventorySourceName.ToLower() == "manager")
                {
                    //item.ObjInventorySourceChildBOList = objWebServiceUserRoleDAL.GetAllUsersBasedOnRole(UserId, "Manager");
                    var data = objWebServiceUserRoleDAL.GetAllUsersBasedOnRole(UserId, "Manager");
                    if (userRoles.Contains("2"))
                    {
                        //current user is manager - Manager to Manager transfer will happen

                        item.ObjInventorySourceChildBOList = data.Where(o => o.ChildId != UserId).ToList();
                    }
                    else if (userRoles.Contains("3"))
                    {
                        //current user is field supervisor - get only his manager

                        var userManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == UserId).FirstOrDefault();
                        if (userManager != null)
                        {
                            //check user present
                            item.ObjInventorySourceChildBOList = data.Where(o => o.ChildId == userManager.ManagerId).ToList();
                        }
                    }
                    else if (userRoles.Contains("1"))
                    {
                        //current user is installer - get only his manager
                        var userManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == UserId).FirstOrDefault();
                        if (userManager != null)
                        {
                            //check user present
                            item.ObjInventorySourceChildBOList = data.Where(o => o.ChildId == userManager.ManagerId).ToList();
                        }
                    }
                }
                else if (item.InventorySourceName.ToLower() == "field supervisor")
                {
                    // item.ObjInventorySourceChildBOList = objWebServiceUserRoleDAL.GetAllUsersBasedOnRole(UserId, "Field Supervisor");
                    var data = objWebServiceUserRoleDAL.GetAllUsersBasedOnRole(UserId, "Field Supervisor");


                    if (userRoles.Contains("2"))
                    {
                        //current user is manager - only give field supervisors which are currently working under this manager
                        //item.ObjInventorySourceChildBOList = data.Where(o => o.ChildId != UserId).ToList();


                        var data1 = new List<InventorySourceChildBO>();
                        foreach (var item1 in data)
                        {
                            var d = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == item1.ChildId && o.ManagerId == UserId).FirstOrDefault();

                            if (d != null)
                            {
                                data1.Add(item1);
                            }
                        }
                        item.ObjInventorySourceChildBOList = data1;


                    }
                    else if (userRoles.Contains("3"))
                    {
                        //current user is field supervisor - get only supervisors which are under current manager

                        var userManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == UserId).FirstOrDefault();
                        var data1 = new List<InventorySourceChildBO>();
                        if (userManager != null && userManager.ManagerId != null && data != null)
                        {
                            foreach (var item1 in data)
                            {
                                var d = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == item1.ChildId && o.ManagerId == userManager.ManagerId).FirstOrDefault();

                                if (d != null)
                                {
                                    data1.Add(item1);
                                }
                            }
                            //check user present
                            item.ObjInventorySourceChildBOList = data1;
                        }


                    }
                    else if (userRoles.Contains("1"))
                    {
                        //current user is  installer - get only supervisors which are under current manager
                        var userManager = objCompassEntities.tblUsers.Where(o => o.UserID == UserId).FirstOrDefault();

                        var data1 = new List<InventorySourceChildBO>();
                        if (userManager != null && userManager.ManagerId != null && data != null)
                        {
                            foreach (var item1 in data)
                            {
                                var d = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == item1.ChildId && o.ManagerId == userManager.ManagerId).FirstOrDefault();

                                if (d != null || userManager.ManagerId == item1.ChildId)
                                {
                                    data1.Add(item1);
                                }

                            }
                            //check user present
                            item.ObjInventorySourceChildBOList = data1;
                        }
                    }

                }
                else if (item.InventorySourceName.ToLower() == "installer")
                {
                    //item.ObjInventorySourceChildBOList = objWebServiceUserRoleDAL.GetAllUsersBasedOnRole(UserId, "Installer");

                    var data = objWebServiceUserRoleDAL.GetAllUsersBasedOnRole(UserId, "Installer");

                    if (userRoles.Contains("2"))
                    {
                        //current user is manager - get all installers under current manager
                        // item.ObjInventorySourceChildBOList = data.Where(o => o.ChildId != UserId).ToList();


                        var data1 = new List<InventorySourceChildBO>();
                        foreach (var item1 in data)
                        {
                            var d = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == item1.ChildId && o.ManagerId == UserId).FirstOrDefault();

                            if (d != null)
                            {
                                data1.Add(item1);
                            }
                        }
                        item.ObjInventorySourceChildBOList = data1;

                    }
                    else if (userRoles.Contains("3"))
                    {
                        //current user is field supervisor - get only supervisors which are under current manager

                        var userManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == UserId).FirstOrDefault();
                        var data1 = new List<InventorySourceChildBO>();
                        if (userManager != null && userManager.ManagerId != null && data != null)
                        {
                            foreach (var item1 in data)
                            {
                                var d = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == item1.ChildId && o.ManagerId == userManager.ManagerId).FirstOrDefault();

                                if (d != null)
                                {
                                    data1.Add(item1);
                                }
                            }
                            //check user present
                            item.ObjInventorySourceChildBOList = data1;
                        }
                    }
                    else if (userRoles.Contains("1"))
                    {
                        //current user is  installer - get only supervisors which are under current manager
                        var userManager = objCompassEntities.tblUsers.Where(o => o.UserID == UserId).FirstOrDefault();

                        var data1 = new List<InventorySourceChildBO>();
                        if (userManager != null && userManager.ManagerId != null && data != null)
                        {
                            foreach (var item1 in data)
                            {
                                var d = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == item1.ChildId && o.ManagerId == userManager.ManagerId).FirstOrDefault();

                                if (d != null)
                                {
                                    data1.Add(item1);
                                }
                            }
                            //check user present
                            item.ObjInventorySourceChildBOList = data1;
                        }
                    }

                }
                else if (item.InventorySourceName.ToLower() == "warehouse manager")
                {
                    //item.ObjInventorySourceChildBOList = objWebServiceUserRoleDAL.GetAllUsersBasedOnRole(UserId, "Warehouse Manager");
                }
            }



            var CurrentProjectUsers = objCompassEntities.TblProjectInstallers.
                            Where(o => o.ProjectID == projectId).Select(o => o.tblUsers_UserID).ToList();

            foreach (var item in objInventorySourceBOList)
            {
                if (item.InventorySourceName != "Warehouse")
                {
                    /*Filter current project users*/
                    var x = item.ObjInventorySourceChildBOList.Where(o => CurrentProjectUsers.Contains(o.ChildId)).Select(o => o).ToList();

                    item.ObjInventorySourceChildBOList = x;
                }
            }


            objCompassEntities.Dispose();
            objWebServiceUserRoleDAL.Dispose();
            return objInventorySourceBOList;
        }




        //public InventoryNewRequestSourceBO GetInventoryNewRequestSourceData(long UserId, DateTime CurrentDate)
        //{

        //    InventoryNewRequestSourceBO objInventoryNewRequestSourceBO = new InventoryNewRequestSourceBO();

        //    objInventoryNewRequestSourceBO.ObjInventorySourceBOList = GetInventorySourceData(UserId, CurrentDate);

        //    CompassEntities objCompassEntities = new CompassEntities();

        //    List<InventoryRequestNameBO> ObjInventoryRequestNameBOList = new List<InventoryRequestNameBO>();

        //    var MeterData = objCompassEntities.tblMeterSizes.Where(o => o.Active == 1).Select(o => new InventoryRequestNameBO { InventoryName = o.MeterSize }).ToList();

        //    if (MeterData != null)
        //    {
        //        MeterData.ForEach(o => ObjInventoryRequestNameBOList.Add(o));
        //    }

        //    var data =objCompassEntities.InventoryMasters.Where(o => o.Active == 1).Select(o => new InventoryRequestNameBO { InventoryName = o.InventoryName }).ToList();

        //    if (data != null)
        //    {
        //        data.ForEach(o => ObjInventoryRequestNameBOList.Add(o));
        //    }

        //    objInventoryNewRequestSourceBO.ObjInventoryRequestNameBOList = ObjInventoryRequestNameBOList;

        //    var data1 = objCompassEntities.UtilityMasters.Where(o => o.Active == 1).Select(o => new InventoryUtilityTypeBO { Id = o.Id, UtilityName = o.UtilityName }).ToList();

        //    objInventoryNewRequestSourceBO.ObjInventoryUtilityTypeBOList = data1;


        //    objCompassEntities.Dispose();
        //    return objInventoryNewRequestSourceBO;

        //}





        #endregion




        public bool ValidateInventoryData(ref InwardInventoryBo objInwardInventoryBo, string JsonData, DateTime DeviceRequestDate)
        {
            bool result = false;

            objInwardInventoryBo.IsRecordProcessed = true;


            //try
            //{


            //    CompassEntities objCompassEntities = new CompassEntities();


            //    InwardRequestMaster objInwardRequestMaster = new InwardRequestMaster();


            //    objInwardRequestMaster.Active = 1;
            //    objInwardRequestMaster.CreatedBy = long.Parse(objInwardInventoryBo.UserId);
            //    objInwardRequestMaster.CreatedOn = DateTime.Now;

            //    objInwardRequestMaster.DeviceDateTimeOfRequest = DeviceRequestDate;
            //    objInwardRequestMaster.InventorySourceId = objInwardInventoryBo.InventorySourceId;
            //    objInwardRequestMaster.InventorySubSourceId = objInwardInventoryBo.InventorySubSourceId;
            //    objInwardRequestMaster.JsonRequestData = JsonData;

            //    objCompassEntities.InwardRequestMasters.Add(objInwardRequestMaster);
            //    objCompassEntities.SaveChanges();



            //    if (objInwardInventoryBo.InventorySourceId == 1)
            //    {
            //        foreach (var item in objInwardInventoryBo.BarcodeDataList)
            //        {

            //            var countInList = objInwardInventoryBo.BarcodeDataList.Where(o => o.BarcodeSerialNumber == item.BarcodeSerialNumber).Count();

            //            if (countInList != 1)
            //            {
            //                item.InventoryStatus = "Duplicate Within Data";
            //            }
            //            else
            //            {
            //                var checkBarcodeInUseMasterTable = objCompassEntities.InventoryUploads.Where(o => o.Active == 1 && o.SerialNo == item.BarcodeSerialNumber).FirstOrDefault();

            //                if (checkBarcodeInUseMasterTable != null)
            //                {
            //                    bool ifUsed = (checkBarcodeInUseMasterTable.IsUsed == null || checkBarcodeInUseMasterTable.IsUsed == 0) ? false : true;
            //                    if (ifUsed == true)
            //                    {
            //                        item.InventoryStatus = "Invalid Inventory";
            //                    }
            //                    else
            //                    {
            //                        item.InventoryStatus = "Successful";
            //                    }
            //                }
            //                else
            //                {
            //                    var checkBarcodeInUse = objCompassEntities.InwardSubRequestMasters.Where(o => o.Active == 1 && o.SerialNumber == item.BarcodeSerialNumber).FirstOrDefault();

            //                    if (checkBarcodeInUse != null)
            //                    {
            //                        item.InventoryStatus = "Invalid Inventory";
            //                    }
            //                    else
            //                    {
            //                        item.InventoryStatus = "Not in Inventory";
            //                    }

            //                }
            //            }

            //            if (item.InventoryStatus == "Successful" || item.InventoryStatus == "Not in Inventory")
            //            {
            //                InwardSubRequestMaster objInwardSubRequestMaster = new InwardSubRequestMaster();

            //                objInwardSubRequestMaster.Active = 1;
            //                objInwardSubRequestMaster.CreatedBy = long.Parse(objInwardInventoryBo.UserId);
            //                objInwardSubRequestMaster.CreatedOn = objInwardRequestMaster.CreatedOn;
            //                objInwardSubRequestMaster.InwardID = objInwardRequestMaster.Id;
            //                objInwardSubRequestMaster.GenerateBarcode = 0;
            //                objInwardSubRequestMaster.BalanceQuantity = 1;
            //                objInwardSubRequestMaster.Quantity = 1;

            //                objInwardSubRequestMaster.BarcodeOrQRCode = true;
            //                objInwardSubRequestMaster.SerialNumber = item.BarcodeSerialNumber;

            //                objCompassEntities.InwardSubRequestMasters.Add(objInwardSubRequestMaster);
            //                objCompassEntities.SaveChanges();
            //            }

            //        }

            //        foreach (var item in objInwardInventoryBo.QRcodeDataList)
            //        {
            //            InwardSubRequestMaster objInwardSubRequestMaster = new InwardSubRequestMaster();

            //            objInwardSubRequestMaster.Active = 1;
            //            objInwardSubRequestMaster.CreatedBy = long.Parse(objInwardInventoryBo.UserId);
            //            objInwardSubRequestMaster.CreatedOn = objInwardRequestMaster.CreatedOn;
            //            objInwardSubRequestMaster.InwardID = objInwardRequestMaster.Id;

            //            objInwardSubRequestMaster.BalanceQuantity = item.InventoryQty;
            //            objInwardSubRequestMaster.Quantity = item.InventoryQty;

            //            objInwardSubRequestMaster.BarcodeOrQRCode = false;
            //            objInwardSubRequestMaster.Description = item.InventoryName;
            //            objInwardSubRequestMaster.GenerateBarcode = item.GenerateBarcode == true ? 1 : 0;

            //            objCompassEntities.InwardSubRequestMasters.Add(objInwardSubRequestMaster);
            //            objCompassEntities.SaveChanges();

            //            item.InventoryStatus = "Successful";
            //        }





            //        //Generate Barcode for the fields

            //        var objInwardSubRequestMasterList = objCompassEntities.InwardSubRequestMasters.Where(o => o.InwardID == objInwardRequestMaster.Id && o.Active == 1 && o.GenerateBarcode == 1).ToList();

            //        if (objInwardSubRequestMasterList != null && objInwardSubRequestMasterList.Count > 0)
            //        {

            //            //Path on the current server
            //            string MainPath = @"~\BarcodeFiles";

            //            //InwardRequestId
            //            MainPath = MainPath + @"\" + objInwardRequestMaster.Id;

            //            string WebURLFilePath = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();

            //            //InwardRequestId
            //            WebURLFilePath = WebURLFilePath + @"BarcodeFiles\" + objInwardRequestMaster.Id;

            //            foreach (var item in objInwardSubRequestMasterList)
            //            {
            //                //SubInwardRequestId
            //                string subMainPath = MainPath + @"\" + item.Id + "\\BarcodeFile.pdf";

            //                string filePath = HttpContext.Current.Request.MapPath(subMainPath);
            //                grantAccess(Path.GetDirectoryName(filePath));
            //                //create Directory

            //                string subWebURLFilePath = WebURLFilePath + @"\" + item.Id + "\\BarcodeFile.pdf";

            //                // var doc = new Document();
            //                var doc = new Document(PageSize.A4, 50, 50, 25, 25);

            //                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            //                //open the document for writing
            //                doc.Open();
            //                //get PdfContentByte object
            //                PdfContentByte content = writer.DirectContent;

            //                for (int i = 0; i < item.Quantity; i++)
            //                {
            //                    Barcode39 bar39 = new Barcode39();
            //                    bar39.Code = GetUniqueKey(13);//"12345ABCDE";
            //                    iTextSharp.text.Image img39 = bar39.CreateImageWithBarcode(content, null, null);
            //                    doc.Add(img39);
            //                    doc.Add(new Paragraph(""));
            //                    doc.Add(new Paragraph(""));
            //                }

            //                doc.Close();

            //                item.BarCodeFilePath = filePath;
            //                objCompassEntities.SaveChanges();

            //                var objUpdate = objInwardInventoryBo.QRcodeDataList.Where(o => o.GenerateBarcode == true && o.InventoryName == item.Description).FirstOrDefault();
            //                objUpdate.WebUrlOfBarcodeFile = subWebURLFilePath;
            //                objUpdate.InventoryStatus = "Print Barcode";
            //            }


            //        }

            //    }

            //    result = true;
            //}
            //catch (Exception ex)
            //{

            //    throw ex;
            //}

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

        //public bool SaveNewRequest(RequestNewInventoryBo objRequestNewInventoryBo, string JsonData, DateTime DeviceRequestDate)
        //{
        //    bool result = true;


        //    try
        //    {

        //        CompassEntities objCompassEntities = new CompassEntities();

        //        InventoryTransferRequestMaster objBO = new InventoryTransferRequestMaster();

        //        objBO.Active = 1;
        //        objBO.CreatedBy = long.Parse(objRequestNewInventoryBo.UserId);
        //        objBO.CreatedOn = DateTime.Now;

        //        objBO.DeviceDateTimeOfRequest = DeviceRequestDate;
        //        objBO.FkUtilityId = objRequestNewInventoryBo.UtilityId;
        //        objBO.InventorySourceId = objRequestNewInventoryBo.InventorySourceId;
        //        objBO.InventorySubSourceId = objRequestNewInventoryBo.InventorySubSourceId;
        //        objBO.JsonRequestData = JsonData;

        //        objCompassEntities.InventoryTransferRequestMasters.Add(objBO);
        //        objCompassEntities.SaveChanges();


        //        foreach (var item in objRequestNewInventoryBo.InventoryRequestNameBOList)
        //        {
        //            InventoryTransferRequestSubMaster objSubRequestMaster = new InventoryTransferRequestSubMaster();

        //            objSubRequestMaster.Active = 1;
        //            objSubRequestMaster.CreatedBy = long.Parse(objRequestNewInventoryBo.UserId);
        //            objSubRequestMaster.CreatedOn = objBO.CreatedOn;
        //            objSubRequestMaster.FKTransferId = objBO.Id;

        //            objSubRequestMaster.Quantity = item.InventoryQty;
        //            objSubRequestMaster.Description = item.InventoryName;

        //            objCompassEntities.InventoryTransferRequestSubMasters.Add(objSubRequestMaster);
        //            objCompassEntities.SaveChanges();

        //        }



        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //    return result;
        //}



        //public List<ViewInventoryRequestBo> GetRequestInformations(long userId, DateTime _currentDate)
        //{

        //    List<ViewInventoryRequestBo> objList = new List<ViewInventoryRequestBo>();


        //    try
        //    {
        //        CompassEntities objCompassEntities = new CompassEntities();

        //        objList = objCompassEntities.InventoryTransferRequestMasters.Where(o => o.Active == 1
        //            && o.DeviceDateTimeOfRequest.Value.Date == _currentDate.Date).Select(o =>
        //                new ViewInventoryRequestBo
        //                {
        //                    ApprovedDateTime = o.DeviceDateTimeOfApproval != null ? o.DeviceDateTimeOfApproval.Value.ToString("MMM-dd-yyyy hh:mm tt") : "",
        //                    ApprovedUserName = o.ApprovedBy != null ? objCompassEntities.tblUsers.Where(o1 => o1.UserID == o.ApprovedBy).Select(o1 => o1.FirstName).FirstOrDefault() : "",
        //                    QRcodeDataList = o.InventoryTransferRequestSubMasters.Where(o1 => o1.Active == 1).Select(o1 => new QRcodeDataItems { InventoryName = o1.Description, InventoryQty = o1.Quantity }).ToList(),
        //                    RequestId = o.Id,
        //                    SubmittedDateTime = o.DeviceDateTimeOfRequest.Value.ToString("MMM-dd-yyyy hh:mm tt"),
        //                    UserName = objCompassEntities.tblUsers.Where(o1 => o1.UserID == o.InventorySubSourceId).Select(o1 => o1.FirstName).FirstOrDefault()

        //                }
        //                ).ToList();





        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }


        //    return objList;
        //}


        //public bool ApproveRequest(long userId, long RequestId, DateTime _currentDate)
        //{
        //    bool result = false;

        //    try
        //    {
        //        CompassEntities objCompassEntities = new CompassEntities();

        //        var data = objCompassEntities.InventoryTransferRequestMasters.Where(o => o.Active == 1 && o.Id == RequestId).FirstOrDefault();


        //        data.ApprovedBy = userId;
        //        data.DeviceDateTimeOfApproval = _currentDate;
        //        data.ModifiedBy = userId;
        //        data.ModifiedOn = DateTime.Now;

        //        objCompassEntities.SaveChanges();
        //        result = true;

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }


        //    return result;
        //}



        //List<MobileProductBO> objMobileProductBOList









        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(InventoryWebServiceDAL));
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objMobileWarehouseRecieveBo"></param>
        /// <param name="JsonData"></param>
        /// <param name="DeviceRequestDate"></param>
        /// <param name="objMobileWarehouseResponseBo"></param>
        /// <returns></returns>
        public bool WarehouseInsertMobileData(ref MobileWarehouseReceiveBo objMobileWarehouseRecieveBo, string JsonData, DateTime DeviceRequestDate, out MobileWarehouseResponseBo objMobileWarehouseResponseBo, out string _message)
        {
            bool result = false;

            objMobileWarehouseResponseBo = new MobileWarehouseResponseBo();

            long userId = objMobileWarehouseRecieveBo.UserId;
            try
            {

                CompassEntities objCompassEntities = new CompassEntities();


                var devicedatetime = objMobileWarehouseRecieveBo.DeviceDateTimeOfRequest;
                var currentUserId = objMobileWarehouseRecieveBo.UserId;

                var ObjHeader = objCompassEntities.InwardHeaders.Where(o => o.Active == 1 && o.DeviceDateTime == DeviceRequestDate
                    && o.CreatedBy == currentUserId).FirstOrDefault();




                string errormessage = "";

                string WebURLFilePath = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();

                if (ObjHeader == null)
                {


                    objMobileWarehouseResponseBo.WarehouseId = objMobileWarehouseRecieveBo.WarehouseId;
                    objMobileWarehouseResponseBo.DeviceDateTimeOfRequest = objMobileWarehouseRecieveBo.DeviceDateTimeOfRequest;
                    objMobileWarehouseResponseBo.UserId = objMobileWarehouseRecieveBo.UserId;

                    var count1 = objMobileWarehouseRecieveBo.ProductQuantityList.Where(o => o.GenerateBarcode == true).ToList().Count();
                    //check if generate barcode is required
                    List<MobileSerialNumberData> NewSerialNumberList = new List<MobileSerialNumberData>();

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
                        foreach (var item in objMobileWarehouseRecieveBo.ProductQuantityList.Where(o => o.GenerateBarcode == true).ToList())
                        {


                            var CheckProductStatus = objCompassEntities.ProductMasters.Where(o => o.Id == item.ProductId).FirstOrDefault();


                            #region generate barcode & add record in database

                            string dynamicFolderName1 = DateTime.Now.ToString("ddMMyyhhmmssfff");
                            //SubInwardRequestId
                            string subMainPath = MainPath + @"\" + dynamicFolderName1 + "\\BarcodeFile.pdf";

                            string filePath = HttpContext.Current.Request.MapPath(subMainPath);
                            grantAccess(Path.GetDirectoryName(filePath));
                            //create Directory

                            string subWebURLFilePath = WebURLFilePath + @"\" + dynamicFolderName1 + "\\BarcodeFile.pdf";



                            DataTable dt = new DataTable();
                            dt.Columns.Add("ProductName");
                            dt.Columns.Add("SerialNumber");
                            dt.Columns.Add("PartNumber");
                            dt.Columns.Add("UPC");
                            dt.Columns.Add("PartNumber1");
                            dt.Columns.Add("UPC1");

                            for (int i = 0; i < item.ProductQuantity; i++)
                            {

                                DataRow dr = dt.NewRow();
                                dr["ProductName"] = CheckProductStatus.ProductName;
                                dr["PartNumber"] = CheckProductStatus.PartNumber;
                                dr["UPC"] = CheckProductStatus.UPCCode;
                                dr["PartNumber1"] = "Part No. : " + CheckProductStatus.PartNumber;
                                dr["UPC1"] = "UPC : " + CheckProductStatus.UPCCode;


                                bool stockMasterCheck = false;
                                var stringKey = "";


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
                                    dr["SerialNumber"] = stringKey;
                                }
                                dt.Rows.Add(dr);


                                if (CheckProductStatus.SerialNumberRequired == true)
                                {

                                    #region Insert record in NewSerialNumberList
                                    MobileSerialNumberData obj = new MobileSerialNumberData();

                                    obj.ProductId = item.ProductId;
                                    obj.SerialNumber = stringKey;
                                    obj.ParentId = parentId;
                                    obj.UtilityId = item.UtilityId;
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
                            ReportViewer ReportViewer1 = new ReportViewer();
                            ReportViewer1.LocalReport.DataSources.Clear();
                            ReportViewer1.LocalReport.DataSources.Add(datasource);
                            ReportViewer1.LocalReport.EnableHyperlinks = true;
                            ReportViewer1.LocalReport.ReportPath = HttpContext.Current.Request.MapPath(ReportFilePath);




                            System.IO.FileInfo fi = new System.IO.FileInfo(filePath);

                            // if (fi.Exists) fi.Delete();

                            Warning[] warnings;

                            string[] streamids;

                            string mimeType, encoding, filenameExtension;


                            byte[] bytes = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);


                            System.IO.FileStream fs = System.IO.File.Create(filePath);

                            fs.Write(bytes, 0, bytes.Length);

                            fs.Close();

                            item.WebUrlOfBarcodeFile = subMainPath;
                            if (CheckProductStatus.SerialNumberRequired == true)
                            {
                                item.ParentId = parentId;
                            }

                            parentId++;
                            #endregion
                        }


                    }
                    long projectId = objMobileWarehouseRecieveBo.ProjectId;
                    var project = objCompassEntities.tblProjects.Where(o => o.ProjectId == projectId).First();
                    var utility = objCompassEntities.UtilityMasters.Where(o => o.Active == 1 && o.UtilityName == project.Utilitytype).First();
                    var location = objMobileWarehouseRecieveBo.Location;
                    foreach (var item in objMobileWarehouseRecieveBo.SerialNumberList)
                    {
                        item.UtilityId = utility.Id;
                        item.Location = location;
                    }

                    foreach (var item in NewSerialNumberList)
                    {
                        item.UtilityId = utility.Id;
                        item.Location = location;
                    }

                    foreach (var item in objMobileWarehouseRecieveBo.ProductQuantityList)
                    {
                        item.UtilityId = utility.Id;
                        item.Location = location;
                    }

                    DataTable ObjDataTable = new DataTable();
                    ObjDataTable.Columns.Add("ParentId", typeof(long));
                    ObjDataTable.Columns.Add("ProductId", typeof(long));
                    ObjDataTable.Columns.Add("UtilityTypeId", typeof(long));
                    ObjDataTable.Columns.Add("SerialNumber", typeof(string));
                    ObjDataTable.Columns.Add("TotalQty", typeof(int));
                    ObjDataTable.Columns.Add("GenerateBarcode", typeof(int));
                    ObjDataTable.Columns.Add("BarcodeFilePath", typeof(string));
                    ObjDataTable.Columns.Add("Location", typeof(string));

                    foreach (var item in objMobileWarehouseRecieveBo.SerialNumberList)
                    {
                        DataRow DR = ObjDataTable.NewRow();
                        DR["ParentId"] = DBNull.Value;
                        DR["ProductId"] = item.ProductId;
                        DR["UtilityTypeId"] = item.UtilityId;
                        DR["SerialNumber"] = item.SerialNumber;
                        DR["TotalQty"] = 1;
                        DR["GenerateBarcode"] = 0;
                        DR["BarcodeFilePath"] = DBNull.Value;
                        DR["Location"] = item.Location;
                        ObjDataTable.Rows.Add(DR);
                    }

                    foreach (var item in NewSerialNumberList)
                    {
                        DataRow DR = ObjDataTable.NewRow();
                        DR["ParentId"] = item.ParentId;
                        DR["ProductId"] = item.ProductId;
                        DR["UtilityTypeId"] = item.UtilityId;
                        DR["SerialNumber"] = item.SerialNumber;
                        DR["TotalQty"] = 1;
                        DR["GenerateBarcode"] = 0;
                        DR["BarcodeFilePath"] = DBNull.Value;
                        DR["Location"] = item.Location;
                        ObjDataTable.Rows.Add(DR);
                    }

                    foreach (var item in objMobileWarehouseRecieveBo.ProductQuantityList)
                    {
                        DataRow DR = ObjDataTable.NewRow();
                        DR["ParentId"] = item.ParentId;
                        DR["ProductId"] = item.ProductId;
                        DR["UtilityTypeId"] = item.UtilityId;
                        DR["SerialNumber"] = DBNull.Value;
                        DR["TotalQty"] = item.ProductQuantity;
                        DR["GenerateBarcode"] = item.GenerateBarcode == true ? 1 : 0;
                        DR["BarcodeFilePath"] = item.WebUrlOfBarcodeFile;
                        DR["Location"] = item.Location;
                        ObjDataTable.Rows.Add(DR);
                    }


                    //var a=objCompassEntities.PROC_Insert_InwardFromMobile(objMobileWarehouseRecieveBo.DeviceDateTimeOfRequest, objMobileWarehouseRecieveBo.UserId,
                    //    objMobileWarehouseRecieveBo.WarehouseId);

                    try
                    {
                        SqlConnection con = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString);
                        con.Open();
                        SqlCommand cmd = new SqlCommand("PROC_Insert_InwardFromMobile", con);
                        cmd.Parameters.AddWithValue("@tblInventory", ObjDataTable); // passing Datatable
                        cmd.Parameters.AddWithValue("@deviceDateTime", DeviceRequestDate);
                        cmd.Parameters.AddWithValue("@UserId", objMobileWarehouseRecieveBo.UserId);
                        cmd.Parameters.AddWithValue("@WarehouseId", objMobileWarehouseRecieveBo.WarehouseId);
                        cmd.Parameters.AddWithValue("@ProjectId", objMobileWarehouseRecieveBo.ProjectId);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (SqlException ex)
                    {
                        errormessage = ex.Message;
                    }


                }

                ObjHeader = objCompassEntities.InwardHeaders.Where(o => o.Active == 1 && o.DeviceDateTime == DeviceRequestDate
                    && o.CreatedBy == currentUserId).FirstOrDefault();
                if (ObjHeader != null)
                {
                    ObjHeader.JsonRequestData = JsonData;
                    ObjHeader.GPSLocation = objMobileWarehouseRecieveBo.GPSLocation;
                    ObjHeader.DeviceIP = objMobileWarehouseRecieveBo.DeviceIp;
                    objCompassEntities.SaveChanges();


                    var objData1 = objCompassEntities.InwardDetails.Where(o => o.Active == 1 &&
                        o.FKInwardHeaderId == ObjHeader.Id && (o.ParentId == null || o.ParentId == 0))
                        .Select(o => new MobileWarehouseResponseProductData
                        {
                            UtilityName = o.UtilityMaster.UtilityName,
                            ProductName = o.ProductMaster.ProductName,
                            Quantity = o.UploadQty,
                            SerialNumber = o.SerialNumber,
                            GenerateBarcode = o.GenerateBarcode == 0 ? false : true,
                            WebUrlOfBarcodeFile = o.BarCodeFilePath,
                            ServerReponseStatus = o.InventoryStatusMaster.InventoryStatusName
                        }).ToList();

                    var objData2 = objCompassEntities.InwardDetails_RejectedData.Where(o => o.Active == 1 && o.FKInwardHeaderId == ObjHeader.Id).
                        Select(o => new MobileWarehouseResponseProductData
                        {
                            UtilityName = o.UtilityMaster.UtilityName,
                            ProductName = o.ProductMaster.ProductName,
                            Quantity = o.UploadQty,
                            SerialNumber = o.SerialNumber,
                            GenerateBarcode = o.GenerateBarcode == 0 ? false : true,
                            WebUrlOfBarcodeFile = o.BarCodeFilePath,
                            ServerReponseStatus = o.InventoryStatusMaster.InventoryStatusName
                        }).ToList();


                    List<MobileWarehouseResponseProductData> objData = new List<MobileWarehouseResponseProductData>();



                    if (objData1 != null)
                    {
                        objData.AddRange(objData1);
                    }
                    if (objData2 != null)
                    {
                        objData.AddRange(objData2);
                    }


                    WebURLFilePath = System.Configuration.ConfigurationManager.AppSettings["WebURL"].ToString();
                    foreach (var item in objData.Where(o => o.WebUrlOfBarcodeFile != null).ToList())
                    {
                        item.WebUrlOfBarcodeFile = WebURLFilePath + item.WebUrlOfBarcodeFile.Replace(@"~\", "");
                    }


                    objMobileWarehouseResponseBo.objData = objData;
                    _message = "Success.";
                    result = true;
                }
                else
                {
                    // _message = "Error.";

                    _message = errormessage;
                    result = false;

                    log.Info("Json rejected ");

                }


            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;
        }


        /// <summary>
        /// Warehouse data allocated to installer.
        /// </summary>
        /// <param name="objMobileUserReceiveBo"></param>
        /// <param name="JsonData"></param>
        /// <param name="DeviceRequestDate"></param>
        /// <returns></returns>
        public bool ManageStock(ref MobileUserReceiveBo objMobileUserReceiveBo, string JsonData, DateTime DeviceRequestDate, out string _message)
        {
            bool result = false;

            try
            {

                CompassEntities objCompassEntities = new CompassEntities();
                long userId = objMobileUserReceiveBo.UserId;

                var checkRequestAlreadyMade = objCompassEntities.InventoryMobileStockRequests.Where(o =>
                    o.DeviceDateTime == DeviceRequestDate && o.CreatedBy == userId).FirstOrDefault();

                string errormessage = "";

                if (checkRequestAlreadyMade == null)
                {


                    DataTable ObjDataTable = new DataTable();

                    ObjDataTable.Columns.Add("SourceId", typeof(long));
                    ObjDataTable.Columns.Add("SubSourceId", typeof(long));
                    ObjDataTable.Columns.Add("ProductId", typeof(long));
                    ObjDataTable.Columns.Add("IsSerialNumber", typeof(bool));
                    ObjDataTable.Columns.Add("SerialNumber", typeof(string));
                    ObjDataTable.Columns.Add("TotalQty", typeof(int));


                    foreach (var item in objMobileUserReceiveBo.SerialNumberList)
                    {

                        DataRow DR = ObjDataTable.NewRow();
                        DR["ProductId"] = DBNull.Value;
                        DR["IsSerialNumber"] = true;
                        DR["SerialNumber"] = item.SerialNumber;
                        DR["TotalQty"] = 1;
                        ObjDataTable.Rows.Add(DR);
                    }



                    foreach (var item in objMobileUserReceiveBo.ProductQuantityList)
                    {

                        DataRow DR = ObjDataTable.NewRow();
                        DR["SourceId"] = item.ReceiveFromId;
                        DR["SubSourceId"] = item.ReceiveFromSubId;
                        DR["ProductId"] = item.ProductId;
                        DR["IsSerialNumber"] = false;
                        DR["TotalQty"] = item.ProductQuantity;
                        ObjDataTable.Rows.Add(DR);
                    }


                    try
                    {
                        SqlConnection con = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString);
                        con.Open();
                        //SqlCommand cmd = new SqlCommand("PROC_Insert_UserStockRequestFromMobile", con);

                        SqlCommand cmd = new SqlCommand("PROC_Insert_UserStockRequestFromMobile_Upgrade", con);
                        cmd.Parameters.AddWithValue("@tblStockRequest", ObjDataTable); // passing Datatable
                        cmd.Parameters.AddWithValue("@deviceDateTime", DeviceRequestDate);
                        cmd.Parameters.AddWithValue("@UserId", objMobileUserReceiveBo.UserId);
                        cmd.Parameters.AddWithValue("@ProjectId", objMobileUserReceiveBo.ProjectId);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (SqlException ex)
                    {
                        errormessage = ex.Message;
                    }

                }

                var devicedatetime = objMobileUserReceiveBo.DeviceDateTimeOfRequest;
                var currentUserId = objMobileUserReceiveBo.UserId;
                var ObjHeader = objCompassEntities.InventoryMobileStockRequests.Where(o => o.Active == 1 && o.DeviceDateTime == DeviceRequestDate
                    && o.CreatedBy == currentUserId).FirstOrDefault();

                if (ObjHeader == null)
                {
                    _message = errormessage;
                    result = false;

                    log.Info("Json rejected ");
                }
                else
                {
                    ObjHeader.JsonRequestData = JsonData;
                    ObjHeader.DeviceIP = objMobileUserReceiveBo.DeviceIp;
                    ObjHeader.GPSLocation = objMobileUserReceiveBo.GPSLocation;
                    ObjHeader.FKProjectId = objMobileUserReceiveBo.ProjectId;
                    objCompassEntities.SaveChanges();

                    var objList = objCompassEntities.InventoryMobileStockRequestDetails.Where(o => o.Active == 1 && o.FKInventoryMobileStockRequestId == ObjHeader.Id).ToList();

                    foreach (var item in objList)
                    {
                        if (item.IsSerialNumber)
                        {
                            var obj = objMobileUserReceiveBo.SerialNumberList.Where(o => o.SerialNumber == item.SerialNumber).FirstOrDefault();
                            obj.ServerRequestId = item.Id;
                            obj.ServerReponseStatus = item.InventoryStatusMaster.InventoryStatusName;
                            if (item.FKProductId != null)
                            {
                                obj.ProductId = long.Parse(item.FKProductId.ToString());
                                obj.ProductName = item.ProductMaster.ProductName;
                                obj.ProductMake = item.ProductMaster.MeterMakeId != null ? item.ProductMaster.tblMeterMake.Make : "";
                                obj.ProductSize = item.ProductMaster.MeterSizeId != null ? item.ProductMaster.tblMeterSize.MeterSize : "";
                                obj.ProductType = item.ProductMaster.MeterTypeId != null ? item.ProductMaster.tblMeterType.MeterType : "";
                                obj.PartNumber = item.ProductMaster.PartNumber != null ? item.ProductMaster.PartNumber : "";
                                obj.Base = item.ProductMaster.Base != null ? item.ProductMaster.Base : "";
                            }
                            else
                            {
                                obj.ProductName = "";
                                obj.ProductMake = "";
                                obj.ProductSize = "";
                                obj.ProductType = "";
                                obj.PartNumber = "";
                                obj.Base = "";
                            }
                        }
                        else
                        {
                            var obj = objMobileUserReceiveBo.ProductQuantityList.Where(o => o.ProductId == item.FKProductId).FirstOrDefault();
                            obj.ServerRequestId = item.Id;
                            obj.ServerReponseStatus = item.InventoryStatusMaster.InventoryStatusName;
                            obj.ProductName = item.ProductMaster.ProductName;
                        }
                    }

                    _message = "Success";
                    result = true;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;
        }



        //public bool UserStockFlagSuccess(long userId, DateTime deviceRequestDateTime, out string _message)
        //{
        //    bool Status = false;
        //    try
        //    {

        //        CompassEntities objCompassEntities = new CompassEntities();

        //        var ObjHeader = objCompassEntities.InventoryMobileStockRequests.Where(o => o.Active == 1 && o.DeviceDateTime == deviceRequestDateTime
        //                && o.CreatedBy == userId).FirstOrDefault();

        //        if (ObjHeader == null)
        //        {
        //            _message = "Record not exist";
        //            Status = false;
        //        }
        //        else
        //        {
        //            SqlConnection con = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString);
        //            con.Open();
        //            SqlCommand cmd = new SqlCommand("PROC_Insert_UserStockRequestFromMobile_SetFlag", con);
        //            cmd.Parameters.AddWithValue("@deviceDateTime", deviceRequestDateTime);
        //            cmd.Parameters.AddWithValue("@UserId", userId);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.ExecuteNonQuery();
        //            con.Close();
        //            _message = "Success";
        //            Status = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return Status;

        //}






        public bool AddNotInInventoryInStock(long UserId, long AddToInventorySubId, long ProductId, long WarehouseId, out string _message)
        {
            bool result = false;

            _message = "";


            try
            {

                CompassEntities objCompassEntities = new CompassEntities();




                var checkRecordPresent = objCompassEntities.InventoryMobileStockRequestDetails.Where(o => o.Id == AddToInventorySubId && o.FKInventoryStatusMasterId == 3).FirstOrDefault();




                if (checkRecordPresent == null)
                {
                    _message = "Record Not Present.";
                }
                else if (checkRecordPresent != null && checkRecordPresent.ReceiveFromSubId != WarehouseId)
                {
                    _message = "Save failed. Please select the same warehouse & then try.";
                }
                else
                {
                    checkRecordPresent.FKProductId = ProductId;
                    objCompassEntities.SaveChanges();

                    DataTable ObjDataTable = new DataTable();

                    ObjDataTable.Columns.Add("ParentId", typeof(long));
                    ObjDataTable.Columns.Add("ProductId", typeof(long));
                    ObjDataTable.Columns.Add("UtilityTypeId", typeof(long));
                    ObjDataTable.Columns.Add("SerialNumber", typeof(string));
                    ObjDataTable.Columns.Add("TotalQty", typeof(int));
                    ObjDataTable.Columns.Add("GenerateBarcode", typeof(int));
                    ObjDataTable.Columns.Add("BarcodeFilePath", typeof(string));

                    if (checkRecordPresent.IsSerialNumber == true)
                    {

                        DataRow DR = ObjDataTable.NewRow();
                        DR["ParentId"] = DBNull.Value;
                        DR["ProductId"] = checkRecordPresent.FKProductId;
                        DR["UtilityTypeId"] = objCompassEntities.ProductMasters.Where(o => o.Id == checkRecordPresent.FKProductId).Select(o => o.UtilityId).FirstOrDefault();
                        DR["SerialNumber"] = checkRecordPresent.SerialNumber;
                        DR["TotalQty"] = 1;
                        DR["GenerateBarcode"] = 0;
                        DR["BarcodeFilePath"] = DBNull.Value;
                        ObjDataTable.Rows.Add(DR);
                    }
                    else
                    {
                        DataRow DR = ObjDataTable.NewRow();

                        DR["ProductId"] = checkRecordPresent.FKProductId;
                        DR["UtilityTypeId"] = objCompassEntities.ProductMasters.Where(o => o.Id == checkRecordPresent.FKProductId).Select(o => o.UtilityId).FirstOrDefault();
                        DR["SerialNumber"] = DBNull.Value;
                        DR["TotalQty"] = checkRecordPresent.Quantity;
                        DR["GenerateBarcode"] = 0;
                        DR["BarcodeFilePath"] = DBNull.Value;
                        ObjDataTable.Rows.Add(DR);

                    }

                    var HeaderDetails = objCompassEntities.InventoryMobileStockRequests.Where(o => o.Id == checkRecordPresent.FKInventoryMobileStockRequestId).FirstOrDefault();
                    // var warehouse = objCompassEntities.WarehouseMasters.Where(o => o.WarehouseName == "OutSide Warehouse").FirstOrDefault();


                    string errormessage = "";
                    try
                    {
                        SqlConnection con = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString);
                        con.Open();
                        SqlCommand cmd = new SqlCommand("PROC_Insert_InwardFromMobile", con);
                        cmd.Parameters.AddWithValue("@tblInventory", ObjDataTable); // passing Datatable
                        cmd.Parameters.AddWithValue("@deviceDateTime", HeaderDetails.DeviceDateTime);
                        cmd.Parameters.AddWithValue("@UserId", checkRecordPresent.CreatedBy);
                        cmd.Parameters.AddWithValue("@ProjectId", HeaderDetails.FKProjectId);
                        cmd.Parameters.AddWithValue("@WarehouseId", WarehouseId);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (SqlException ex)
                    {
                        errormessage = ex != null && ex.Message != null ? ex.Message : "";
                    }
                    catch (Exception ex)
                    {
                        errormessage = ex != null && ex.Message != null ? ex.Message : "";
                    }

                    var ObjHeader = objCompassEntities.InwardHeaders.Where(o => o.Active == 1 && o.DeviceDateTime == HeaderDetails.DeviceDateTime
                        && o.CreatedBy == checkRecordPresent.CreatedBy).FirstOrDefault();
                    if (ObjHeader != null)
                    {

                        ObjHeader.DeviceIP = HeaderDetails.DeviceIP;
                        objCompassEntities.SaveChanges();


                        //Now add in outward table.


                        //PROC_Insert_NotInInventoryFromMobile

                        try
                        {
                            SqlConnection con = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString);
                            con.Open();
                            SqlCommand cmd = new SqlCommand("PROC_Insert_NotInInventoryFromMobile", con);
                            cmd.Parameters.AddWithValue("@UserId", checkRecordPresent.CreatedBy);
                            cmd.Parameters.AddWithValue("@InventoryMobileStockRequestDetailId", AddToInventorySubId);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                        catch (SqlException ex)
                        {
                            errormessage = ex != null && ex.Message != null ? ex.Message : "";
                        }
                        catch (Exception ex)
                        {
                            errormessage = ex != null && ex.Message != null ? ex.Message : "";
                        }

                        var Data = objCompassEntities.InwardDetails.Where(o => o.FKInwardHeaderId == ObjHeader.Id).FirstOrDefault();

                        if (Data != null)
                        {
                            checkRecordPresent = objCompassEntities.InventoryMobileStockRequestDetails.Where(o => o.Id == AddToInventorySubId).FirstOrDefault();

                            checkRecordPresent.FKInventoryStatusMasterId = Data.FKInventoryStatusMasterId;
                            objCompassEntities.SaveChanges();
                        }
                        _message = checkRecordPresent.InventoryStatusMaster.InventoryStatusName;




                        if (checkRecordPresent.FKInventoryStatusMasterId == 1)
                        {
                            result = true;
                        }

                    }
                    else
                    {
                        // _message = "Error.";

                        _message = errormessage;
                        result = false;

                        log.Info("Json rejected ");

                    }
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;
        }





    }

}
