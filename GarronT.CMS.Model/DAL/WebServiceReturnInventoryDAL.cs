using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.DAL
{
    public class WebServiceReturnInventoryDAL : IDisposable
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




        public List<InventorySourceBO> GetReturnInventorySourceData(long UserId, long ProjectId)
        {
            List<InventorySourceBO> objInventorySourceBOList = new List<InventorySourceBO>();
            try
            {

                CompassEntities objCompassEntities = new CompassEntities();


                objInventorySourceBOList = objCompassEntities.InventorySourceMasters.
                    Where(o => o.Active == 1 &&
                        (o.InventorySourceName == "Manager" || o.InventorySourceName == "Warehouse")).
                        Select(o => new InventorySourceBO { Id = o.Id, InventorySourceName = o.InventorySourceName }).ToList();


                foreach (var item in objInventorySourceBOList)
                {
                    if (item.InventorySourceName.ToLower() == "warehouse")
                    {

                        WebServiceProductDAL objWebServiceProductDAL = new WebServiceProductDAL();

                        var ObjWarehouseList = objWebServiceProductDAL.GetWarehouseListNew(ProjectId);

                        if (ObjWarehouseList != null)
                        {
                            item.ObjInventorySourceChildBOList = ObjWarehouseList.Select(o => new InventorySourceChildBO { ChildId = o.Id, ChildName = o.WarehouseName }).ToList();
                        }
                        else
                        {
                            item.ObjInventorySourceChildBOList = new List<InventorySourceChildBO>();
                        }
                        //item.ObjInventorySourceChildBOList = new WareHouseDAL().GetWareHouseList();
                    }
                    else if (item.InventorySourceName.ToLower() == "manager")
                    {
                        //manager option

                        var userManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == UserId).FirstOrDefault();

                        if (userManager != null && userManager.ManagerId != null)
                        {
                            var data = GetAllUsersBasedOnRole(UserId, "Manager");
                            item.ObjInventorySourceChildBOList = data.Where(o => o.ChildId == userManager.ManagerId).ToList();

                            var CurrentProjectUsers = objCompassEntities.TblProjectInstallers.
                                Where(o => o.ProjectID == ProjectId).Select(o => o.tblUsers_UserID).ToList();

                            /*Filter current project users*/
                            var x = item.ObjInventorySourceChildBOList.Where(o => CurrentProjectUsers.Contains(o.ChildId)).Select(o => o).ToList();
                            item.ObjInventorySourceChildBOList = x;

                        }

                    }

                }


                objCompassEntities.Dispose();



            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objInventorySourceBOList;
        }



        public List<InventorySourceChildBO> GetAllUsersBasedOnRole(long UserId, string RoleName)
        {
            List<InventorySourceChildBO> objInventorySourceChildBOList = new List<InventorySourceChildBO>();
            CompassEntities objCompassEntities = new CompassEntities();


            var manager = new ApplicationDbContext.UserManager();

            var UseNames = manager.GetUsersInRole(RoleName);

            if (UseNames != null)
            {
                foreach (var item in UseNames)
                {
                    var record = objCompassEntities.tblUsers.Where(o => o.Active == 1 && o.aspnet_UserId.ToString() == item.Id.ToString() && o.UserID != UserId).
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





        public bool ManageReturnStock(ref MobileReturnStockBO objMobileReturnStockBO, string JsonData, DateTime DeviceRequestDate, out string _message)
        {
            bool result = false;

            try
            {

                CompassEntities objCompassEntities = new CompassEntities();
                long userId = objMobileReturnStockBO.UserId;

                long projectId = objMobileReturnStockBO.ProjectId;
                var checkRequestAlreadyMade = objCompassEntities.InventoryMobileStockReturns.Where(o =>
                    o.DeviceDateTime == DeviceRequestDate && o.CreatedBy == userId && o.FKProjectId == projectId).FirstOrDefault();
                string errormessage = "";


                if (checkRequestAlreadyMade == null)
                {



                    DataTable ObjDataTable = new DataTable();

                    ObjDataTable.Columns.Add("SourceId", typeof(long));
                    ObjDataTable.Columns.Add("SubSourceId", typeof(long));
                    ObjDataTable.Columns.Add("ProductId", typeof(long));
                    ObjDataTable.Columns.Add("ProductName", typeof(string));
                    ObjDataTable.Columns.Add("IsNewInventory", typeof(int));
                    ObjDataTable.Columns.Add("IsSerialNumber", typeof(bool));
                    ObjDataTable.Columns.Add("SerialNumber", typeof(string));
                    ObjDataTable.Columns.Add("TotalQty", typeof(int));


                    if (objMobileReturnStockBO.SerialNumberList != null)
                    {
                        foreach (var item in objMobileReturnStockBO.SerialNumberList)
                        {

                            DataRow DR = ObjDataTable.NewRow();
                            DR["SourceId"] = objMobileReturnStockBO.Id;
                            DR["SubSourceId"] = objMobileReturnStockBO.ChildId;
                            //DR["ProductId"] = DBNull.Value;
                            DR["IsSerialNumber"] = true;
                            DR["IsNewInventory"] = 1;
                            DR["SerialNumber"] = item.SerialNumber;
                            DR["TotalQty"] = 1;
                            ObjDataTable.Rows.Add(DR);
                        }
                    }


                    if (objMobileReturnStockBO.ProductQuantityList != null)
                    {
                        foreach (var item in objMobileReturnStockBO.ProductQuantityList)
                        {

                            DataRow DR = ObjDataTable.NewRow();
                            DR["SourceId"] = objMobileReturnStockBO.Id;
                            DR["SubSourceId"] = objMobileReturnStockBO.ChildId;
                            DR["ProductId"] = item.ProductId;
                            DR["IsSerialNumber"] = false;
                            DR["IsNewInventory"] = 1;
                            DR["TotalQty"] = item.TotalQty;
                            ObjDataTable.Rows.Add(DR);
                        }
                    }
                    if (objMobileReturnStockBO.OldProductQuantityList != null)
                    {
                        foreach (var item in objMobileReturnStockBO.OldProductQuantityList)
                        {

                            DataRow DR = ObjDataTable.NewRow();
                            DR["SourceId"] = objMobileReturnStockBO.Id;
                            DR["SubSourceId"] = objMobileReturnStockBO.ChildId;
                            if (item.ProductId != 0)
                            {
                                DR["ProductId"] = item.ProductId;
                            }
                            DR["ProductName"] = item.ProductName;
                            DR["IsSerialNumber"] = false;
                            DR["IsNewInventory"] = 0;
                            DR["TotalQty"] = item.TotalQty;
                            ObjDataTable.Rows.Add(DR);
                        }
                    }

                    if (objMobileReturnStockBO.Id == 1)
                    {
                        /*Warehouse*/
                        try
                        {
                            SqlConnection con = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString);
                            con.Open();
                            SqlCommand cmd = new SqlCommand("PROC_Insert_ReturnFromMobile", con);
                            cmd.Parameters.AddWithValue("@tblStockReturn", ObjDataTable); // passing Datatable
                            cmd.Parameters.AddWithValue("@deviceDateTime", DeviceRequestDate);
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.Parameters.AddWithValue("@ProjectId", projectId);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                        catch (SqlException ex)
                        {
                            errormessage = ex.Message;
                        }

                    }
                    else if (objMobileReturnStockBO.Id == 2 && objMobileReturnStockBO.ChildId > 0)
                    {

                        /*Manager*/
                        try
                        {
                            SqlConnection con = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString);
                            con.Open();
                            SqlCommand cmd = new SqlCommand("PROC_Insert_ReturnToManagerFromMobile", con);
                            cmd.Parameters.AddWithValue("@tblStockReturn", ObjDataTable); // passing Datatable
                            cmd.Parameters.AddWithValue("@deviceDateTime", DeviceRequestDate);
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.Parameters.AddWithValue("@AllocatedTo", objMobileReturnStockBO.ChildId);
                            cmd.Parameters.AddWithValue("@ProjectId", projectId);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                        catch (SqlException ex)
                        {
                            errormessage = ex.Message;
                        }


                    }

                }


                var ObjHeader = objCompassEntities.InventoryMobileStockReturns.Where(o => o.Active == 1 && o.DeviceDateTime == DeviceRequestDate
                    && o.CreatedBy == userId).FirstOrDefault();

                if (ObjHeader == null)
                {
                    _message = errormessage;
                    result = false;

                    //iTextSharp.text.log.Info("Json rejected ");
                }
                else
                {
                    //objMobileReturnStockBO
                    ObjHeader.JsonRequestData = JsonData;
                    ObjHeader.DeviceIP = objMobileReturnStockBO.DeviceIPAddress;
                    ObjHeader.GPSLocation = objMobileReturnStockBO.DeviceGpsLocation;
                    objCompassEntities.SaveChanges();

                    string returnTo = "";
                    if (objMobileReturnStockBO.Id == 1)
                    {
                        var warehouseId = objMobileReturnStockBO.ChildId;

                        returnTo = objCompassEntities.WarehouseMasters.Where(o => o.Id == warehouseId).
                            Select(o => (o.WarehouseName + ", " + o.WarehouseLocation + ", " + o.TblCityMaster.CityName + ", " + o.tblStateMaster.StateName)).FirstOrDefault();
                    }
                    else
                    {
                        var returnUserId = objMobileReturnStockBO.ChildId;

                        returnTo = objCompassEntities.tblUsers.Where(o => o.UserID == returnUserId).
                            Select(o => o.UserName).FirstOrDefault();
                    }

                    objMobileReturnStockBO.ReturnRequestId = ObjHeader.Id;
                    objMobileReturnStockBO.RequestDateTime = DeviceRequestDate.ToString("MMM/dd/yyyy HH:mm tt");
                    objMobileReturnStockBO.ReturnedTo = returnTo;

                    var ProductList = objCompassEntities.ProductMasters.ToList();

                    var objList = objCompassEntities.InventoryMobileStockReturnDetails.Where(o => o.Active == 1 && o.FKInventoryMobileStockReturnId == ObjHeader.Id).ToList();

                    foreach (var item in objList)
                    {
                        if (item.IsNewInventory == 1)
                        {
                            if (item.IsSerialNumber)
                            {
                                var obj = objMobileReturnStockBO.SerialNumberList.Where(o => o.SerialNumber == item.SerialNumber).FirstOrDefault();
                                obj.TotalQty = 1;
                                obj.ServerReponseStatus = item.FKInventoryStatusMasterId != null ? item.InventoryStatusMaster.InventoryStatusName : "Failed";
                            }
                            else
                            {
                                var obj = objMobileReturnStockBO.ProductQuantityList.Where(o => o.ProductId == item.FKProductId).FirstOrDefault();
                                obj.ProductName = ProductList.Where(o => o.Id == obj.ProductId).Select(o => o.ProductName).FirstOrDefault();
                                obj.ServerReponseStatus = item.FKInventoryStatusMasterId != null ? item.InventoryStatusMaster.InventoryStatusName : "Failed";
                            }
                        }
                        else
                        {
                            var obj = objMobileReturnStockBO.OldProductQuantityList.Where(o => o.ProductName == item.ProductName).FirstOrDefault();
                            if (item.FKProductId != null)
                            {
                                obj.ProductName = ProductList.Where(o => o.Id == obj.ProductId).Select(o => o.ProductName).FirstOrDefault();
                            }

                            obj.ServerReponseStatus = item.FKInventoryStatusMasterId != null ? item.InventoryStatusMaster.InventoryStatusName : "Failed";
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


    }
}
