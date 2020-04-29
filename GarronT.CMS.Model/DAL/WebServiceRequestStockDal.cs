using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace GarronT.CMS.Model.DAL
{
    public class WebServiceRequestStockDal : IDisposable
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



        public bool SaveStockNewRequest(ref MobileStockRequest objMobileStockRequest, string JsonData, DateTime DeviceRequestDate, out string _message)
        {
            bool result = false;
            long RequestId = 0;
            try
            {

                TransactionOptions options = new TransactionOptions();
                options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                options.Timeout = new TimeSpan(0, 15, 0);


                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, options))
                {
                    using (CompassEntities objCompassEntities = new CompassEntities())
                    {

                        long userId = objMobileStockRequest.UserId;
                        long RequestedFrom = objMobileStockRequest.SubSourceId;
                        var checkRequestAlreadyMade = objCompassEntities.InventoryMobileUserStockRequests.Where(o =>
                                    o.DeviceRequestDateTime == DeviceRequestDate &&
                                    o.FKRequestedFromUserId == RequestedFrom && o.CreatedBy == userId).FirstOrDefault();

                        if (checkRequestAlreadyMade != null)
                        {
                            _message = "Request already present";
                            result = false;
                        }
                        else
                        {

                            var maxId1 = objCompassEntities.InventoryMobileUserStockRequests.AsNoTracking().Where(o =>
                                 EntityFunctions.TruncateTime(o.CreatedOn) == EntityFunctions.TruncateTime(DeviceRequestDate)).Select(o => o).ToList();

                            long maxId = 0;

                            if (maxId1 != null && maxId1.Count > 0)
                            {
                                maxId = maxId1.Select(o => o.RequestId).Max();
                            }
                            string DynamicRequestId = "";


                            if (maxId > 0)
                            {
                                var MaxRecord = objCompassEntities.InventoryMobileUserStockRequests.Where(o => o.RequestId == maxId).FirstOrDefault();

                                var splitData = MaxRecord.RequestIdWeb.Split('-').ToArray();

                                var CurrentNo = long.Parse(splitData[1]);

                                DynamicRequestId = DeviceRequestDate.ToString("MMddyy-");

                                if (CurrentNo >= 1 && CurrentNo < 9)
                                {
                                    DynamicRequestId = DynamicRequestId + "00" + (CurrentNo + 1).ToString();
                                }
                                else if (CurrentNo >= 9 && CurrentNo < 99)
                                {
                                    DynamicRequestId = DynamicRequestId + "0" + (CurrentNo + 1).ToString();
                                }
                                else
                                {
                                    DynamicRequestId = DynamicRequestId + (CurrentNo + 1).ToString();
                                }
                            }
                            else
                            {
                                DynamicRequestId = DeviceRequestDate.ToString("MMddyy-") + "001";
                            }



                            InventoryMobileUserStockRequest obj = new InventoryMobileUserStockRequest();
                            obj.Active = 1;
                            obj.CreatedBy = userId;
                            obj.CreatedOn = DateTime.Now;
                            obj.DeviceGpsLocation = objMobileStockRequest.DeviceGpsLocation;
                            obj.DeviceIPAddress = objMobileStockRequest.DeviceIPAddress;
                            obj.RequestIdWeb = DynamicRequestId;
                            obj.DeviceRequestDateTime = DeviceRequestDate;
                            obj.FKInventoryRequestStatusId = 1;//pending status
                            obj.FKInventorySourceMasterId = objMobileStockRequest.SourceId;
                            obj.FKRequestedFromUserId = objMobileStockRequest.SubSourceId;
                            obj.JsonRequestData = JsonData;
                            obj.UserComment = objMobileStockRequest.UserComment;

                            obj.FKProjectId = objMobileStockRequest.ProjectId;


                            foreach (var item in objMobileStockRequest.ProductRequestList)
                            {
                                InventoryMobileUserStockRequestDetail objProduct = new InventoryMobileUserStockRequestDetail();
                                objProduct.Active = 1;
                                objProduct.CreatedBy = userId;
                                objProduct.CreatedOn = DateTime.Now;
                                objProduct.FKInventoryMobileUserStockRequestId = obj.RequestId;
                                objProduct.FKProductId = item.ProductId;
                                objProduct.TotalQty = item.Qty;
                                objProduct.CompletedRequestFlag = 0;
                                objProduct.TransferredQty = 0;
                                //objCompassEntities.InventoryMobileUserStockRequestDetails.Add(objProduct);
                                //objCompassEntities.SaveChanges();

                                obj.InventoryMobileUserStockRequestDetails.Add(objProduct);
                            }

                            objCompassEntities.InventoryMobileUserStockRequests.Add(obj);
                            objCompassEntities.SaveChanges();


                            RequestId = obj.RequestId;
                            result = true;
                            _message = "Success";
                            scope.Complete();


                        }



                    }
                }

                if (result && RequestId > 0)
                {

                    new StockRequestDAL().sendMailonRequestApproveReject(RequestId);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }





        public List<InventorySourceBO> GetStockRequestSourceData(long UserId, string userRole, long ProjectId)
        {
            List<InventorySourceBO> objInventorySourceBOList = new List<InventorySourceBO>();

            CompassEntities objCompassEntities = new CompassEntities();
            objInventorySourceBOList = objCompassEntities.InventorySourceMasters.AsNoTracking().
                Where(o => o.Active == 1 &&
                    (o.InventorySourceName == "Manager" || o.InventorySourceName == "Field Supervisor" || o.InventorySourceName == "Installer")).
                    Select(o => new InventorySourceBO { Id = o.Id, InventorySourceName = o.InventorySourceName }).ToList();


            objInventorySourceBOList.Insert(0, new InventorySourceBO { Id = 0, InventorySourceName = "All", ObjInventorySourceChildBOList = new List<InventorySourceChildBO>() });
            WebServiceUserRoleDAL objWebServiceUserRoleDAL = new WebServiceUserRoleDAL();

            foreach (var item in objInventorySourceBOList)
            {


                if (item.InventorySourceName.ToLower() == "manager")
                {
                    //manager option get manager for the current user

                    if (userRole.Contains("2"))
                    {
                        //only self record will be avilable in manager drop down. 


                        var a = objWebServiceUserRoleDAL.GetAllUsersBasedOnRoleWithSelf("Manager");
                        if (a != null || a.Count > 0)
                        {
                            //commented because manager to manager transfer will work.
                            //  a = a.Where(o => o.ChildId == UserId).ToList();
                        }

                        if (a == null || a.Count == 0)
                        {
                            a = new List<InventorySourceChildBO>();
                        }

                        //a.Insert(0, new InventorySourceChildBO { ChildId = 0, ChildName = "All" });
                        item.ObjInventorySourceChildBOList = a;
                    }
                    else if (userRole.Contains("3"))
                    {
                        // if user is field supervisor - get his manager only


                        var a = objWebServiceUserRoleDAL.GetAllUsersBasedOnRoleWithSelf("Manager");
                        if (a != null && a.Count > 0)
                        {
                            var userManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == UserId).FirstOrDefault();
                            if (userManager != null && userManager.ManagerId != null)
                            {
                                //check user present
                                a = a.Where(o => o.ChildId == userManager.ManagerId).ToList();

                            }

                        }

                        if (a == null && a.Count == 0)
                        {
                            a = new List<InventorySourceChildBO>();
                        }

                        //a.Insert(0, new InventorySourceChildBO { ChildId = 0, ChildName = "All" });
                        item.ObjInventorySourceChildBOList = a;
                    }
                    else if (userRole.Contains("1"))
                    {
                        // if user is installer - get his manager only
                        var a = objWebServiceUserRoleDAL.GetAllUsersBasedOnRoleWithSelf("Manager");
                        if (a != null && a.Count > 0)
                        {
                            var userManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == UserId).FirstOrDefault();
                            if (userManager != null && userManager.ManagerId != null)
                            {
                                //check user present
                                a = a.Where(o => o.ChildId == userManager.ManagerId).ToList();

                            }
                        }

                        if (a == null && a.Count == 0)
                        {
                            a = new List<InventorySourceChildBO>();
                        }

                        item.ObjInventorySourceChildBOList = a;
                    }


                }
                else if (item.InventorySourceName.ToLower() == "field supervisor")
                {
                    //supervisor option

                    var a = objWebServiceUserRoleDAL.GetAllUsersBasedOnRoleWithSelf("Field Supervisor");
                    if (a == null && a.Count == 0)
                    {
                        a = new List<InventorySourceChildBO>();
                    }
                    else
                    {
                        var a1 = new List<InventorySourceChildBO>();


                        // for current manager then get all field supervisor under him.
                        var CurrentUserManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == UserId).FirstOrDefault();
                        if (CurrentUserManager != null && CurrentUserManager.ManagerId != null)
                        {
                            foreach (var item1 in a)
                            {
                                var userManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == item1.ChildId
                                    && o.ManagerId == CurrentUserManager.ManagerId).FirstOrDefault();
                                if (userManager != null)
                                {
                                    a1.Add(item1);
                                }
                            }
                        }
                        else if (CurrentUserManager != null && CurrentUserManager.ManagerId == null && userRole.Contains("2"))
                        {
                            foreach (var item1 in a)
                            {
                                var userManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == item1.ChildId
                                    && o.ManagerId == CurrentUserManager.UserID).FirstOrDefault();
                                if (userManager != null)
                                {
                                    a1.Add(item1);
                                }
                            }
                        }



                        if (a1.Count > 1)
                        {
                            a1.Insert(0, new InventorySourceChildBO { ChildId = 0, ChildName = "All" });
                            item.ObjInventorySourceChildBOList = a1;
                        }
                        else
                        {
                            item.ObjInventorySourceChildBOList = a1;

                        }

                    }




                }
                else if (item.InventorySourceName.ToLower() == "installer")
                {
                    //installer option 

                    var a = objWebServiceUserRoleDAL.GetAllUsersBasedOnRoleWithSelf("Installer");
                    if (a == null && a.Count == 0)
                    {
                        a = new List<InventorySourceChildBO>();
                    }
                    var a1 = new List<InventorySourceChildBO>();


                    // for current manager then get all field supervisor under him.
                    var CurrentUserManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == UserId).FirstOrDefault();
                    if (CurrentUserManager != null && CurrentUserManager.ManagerId != null)
                    {
                        foreach (var item1 in a)
                        {
                            var userManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == item1.ChildId
                                && o.ManagerId == CurrentUserManager.ManagerId).FirstOrDefault();
                            if (userManager != null)
                            {
                                a1.Add(item1);
                            }
                        }
                    }
                    else if (CurrentUserManager != null && CurrentUserManager.ManagerId == null && userRole.Contains("2"))
                    {
                        foreach (var item1 in a)
                        {
                            var userManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == item1.ChildId
                                && o.ManagerId == CurrentUserManager.UserID).FirstOrDefault();
                            if (userManager != null)
                            {
                                a1.Add(item1);
                            }
                        }
                    }



                    if (a1.Count > 1)
                    {
                        a1.Insert(0, new InventorySourceChildBO { ChildId = 0, ChildName = "All" });
                        item.ObjInventorySourceChildBOList = a1;
                    }
                    else
                    {
                        item.ObjInventorySourceChildBOList = a1;
                    }


                }

            }

            var a2 = new List<InventorySourceChildBO>();

            foreach (var item in objInventorySourceBOList)
            {
                if (item.InventorySourceName != "All")
                {
                    foreach (var item1 in item.ObjInventorySourceChildBOList)
                    {
                        var check = a2.Where(o => o.ChildId == item1.ChildId).Count();
                        if (check == 0 && item1.ChildName != "All")
                        {
                            a2.Add(item1);
                        }

                    }

                }
            }




            if (a2.Count > 1)
            {
                a2 = a2.OrderBy(o => o.ChildName).ToList();
                a2.Insert(0, new InventorySourceChildBO { ChildId = 0, ChildName = "All" });
            }

            foreach (var item in objInventorySourceBOList)
            {
                if (item.InventorySourceName == "All")
                {
                    item.ObjInventorySourceChildBOList = a2;
                }
            }


            var CurrentProjectUsers = objCompassEntities.TblProjectInstallers.Where(o => o.Active == 1 && o.ProjectID == ProjectId).Select(o => o.tblUsers_UserID).ToList();

            //all option
            CurrentProjectUsers.Add(0);

            foreach (var item in objInventorySourceBOList)
            {
                var x = item.ObjInventorySourceChildBOList.Where(o => CurrentProjectUsers.Contains(o.ChildId)).Select(o => o).ToList();
                item.ObjInventorySourceChildBOList = x;
            }

            objCompassEntities.Dispose();
            objWebServiceUserRoleDAL.Dispose();
            return objInventorySourceBOList;
        }







        public List<InventorySourceBO> GetRequestNewInventorySourceData(long UserId, long ProjectId, string userRoles)
        {
            List<InventorySourceBO> objInventorySourceBOList = new List<InventorySourceBO>();

            CompassEntities objCompassEntities = new CompassEntities();
            objInventorySourceBOList = objCompassEntities.InventorySourceMasters.
                Where(o => o.Active == 1).Select(o => new InventorySourceBO { Id = o.Id, InventorySourceName = o.InventorySourceName }).ToList();
            objInventorySourceBOList.RemoveAt(4);
            objInventorySourceBOList.RemoveAt(0);
            WebServiceUserRoleDAL objWebServiceUserRoleDAL = new WebServiceUserRoleDAL();
            foreach (var item in objInventorySourceBOList)
            {
                if (item.InventorySourceName.ToLower() == "manager")
                {
                    //item.ObjInventorySourceChildBOList = objWebServiceUserRoleDAL.GetAllUsersBasedOnRole(UserId, "Manager");

                    var data = objWebServiceUserRoleDAL.GetAllUsersBasedOnRole(UserId, "Manager");

                    if (userRoles.Contains("2"))
                    {
                        //current user is manager - manager can make request to other manager
                        item.ObjInventorySourceChildBOList = data.Where(o => o.ChildId != UserId).ToList();
                    }
                    else if (userRoles.Contains("3"))
                    {
                        //current user is field supervisor - get only his manager

                        var userManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == UserId).FirstOrDefault();
                        if (userManager != null && userManager.ManagerId != null)
                        {
                            //check user present
                            item.ObjInventorySourceChildBOList = data.Where(o => o.ChildId == userManager.ManagerId).ToList();
                        }
                    }
                    else if (userRoles.Contains("1"))
                    {
                        //current user is installer - get only his manager
                        var userManager = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == UserId).FirstOrDefault();
                        if (userManager != null && userManager.ManagerId != null)
                        {
                            //check user present
                            item.ObjInventorySourceChildBOList = data.Where(o => o.ChildId == userManager.ManagerId).ToList();
                        }
                    }
                }
                else if (item.InventorySourceName.ToLower() == "field supervisor")
                {
                    //item.ObjInventorySourceChildBOList = objWebServiceUserRoleDAL.GetAllUsersBasedOnRole(UserId, "Field Supervisor");
                    var data = objWebServiceUserRoleDAL.GetAllUsersBasedOnRole(UserId, "Field Supervisor");


                    if (userRoles.Contains("2"))
                    {
                        //current user is manager - get all field supervisors under current manager
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
                else if (item.InventorySourceName.ToLower() == "installer")
                {
                    //item.ObjInventorySourceChildBOList = objWebServiceUserRoleDAL.GetAllUsersBasedOnRole(UserId, "Installer");

                    var data = objWebServiceUserRoleDAL.GetAllUsersBasedOnRole(UserId, "Installer");

                    if (userRoles.Contains("2"))
                    {
                        //current user is manager - get all installer under current manager
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

            }

            var CurrentProjectUsers = objCompassEntities.TblProjectInstallers.
                               Where(o => o.ProjectID == ProjectId).Select(o => o.tblUsers_UserID).ToList();

            foreach (var item in objInventorySourceBOList)
            {
                /*Filter current project users*/
                var x = item.ObjInventorySourceChildBOList.Where(o => CurrentProjectUsers.Contains(o.ChildId)).Select(o => o).ToList();

                item.ObjInventorySourceChildBOList = x;
            }



            objCompassEntities.Dispose();
            objWebServiceUserRoleDAL.Dispose();
            return objInventorySourceBOList;
        }





        public List<MobileViewResponseBo> GetRequestList(MobileViewRequestBo objMobileViewRequestBo, string UserRoles)
        {


            List<MobileViewResponseBo> viewRequestList = new List<MobileViewResponseBo>();
            try
            {

                CompassEntities objCompassEntities = new CompassEntities();
                var CurrentUser = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == objMobileViewRequestBo.UserId).FirstOrDefault();
                DateTime from = DateTime.Parse(objMobileViewRequestBo.FromDate);
                DateTime to = DateTime.Parse(objMobileViewRequestBo.Todate);

                long a = (CurrentUser.ManagerId == null ? objMobileViewRequestBo.UserId : CurrentUser.ManagerId.Value);


                var data = new List<PROC_GetAllRequestOfManager_Result>();

                if (UserRoles.Contains("2"))
                {
                    //if user is manager get all request made under that user
                    data = objCompassEntities.PROC_GetAllRequestOfManager(objMobileViewRequestBo.UserId, objMobileViewRequestBo.ProjectId, from, to).ToList();
                }
                else
                {
                    data = objCompassEntities.PROC_GetAllRequestOfManager(a, objMobileViewRequestBo.ProjectId, from, to).ToList();
                    if (data != null && data.Count > 0)
                    {
                        if (objMobileViewRequestBo.ChildId == 0)
                        {
                            data = data.Where(o => o.FKRequestedFromUserId == objMobileViewRequestBo.UserId || o.CreatedBy == objMobileViewRequestBo.UserId).ToList();
                        }
                        else
                        {
                            if (objMobileViewRequestBo.UserId != objMobileViewRequestBo.ChildId)
                            {
                                data = data.Where(o => (o.FKRequestedFromUserId == objMobileViewRequestBo.UserId && o.CreatedBy == objMobileViewRequestBo.ChildId) ||
                                    (o.FKRequestedFromUserId == objMobileViewRequestBo.ChildId && o.CreatedBy == objMobileViewRequestBo.UserId)).ToList();
                            }
                            else
                            {
                                data = data.Where(o => o.FKRequestedFromUserId == objMobileViewRequestBo.UserId || o.CreatedBy == objMobileViewRequestBo.UserId).ToList();
                            }
                        }
                    }
                }

                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        var d = objCompassEntities.InventoryMobileUserStockRequests.AsNoTracking().Where(o => o.RequestId == item.RequestId).FirstOrDefault();

                        var CreatedBy = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == d.CreatedBy).FirstOrDefault();
                        var CreatedTo = objCompassEntities.tblUsers.AsNoTracking().Where(o => o.UserID == d.FKRequestedFromUserId).FirstOrDefault();
                        if (UserRoles.Contains("2"))
                        {
                            #region Manager
                            if (objMobileViewRequestBo.Id == 0)
                            {

                                if (objMobileViewRequestBo.ChildId == 0)
                                {
                                    AddData(ref viewRequestList, ref d, item);
                                }
                                else
                                {
                                    if (d.CreatedBy == objMobileViewRequestBo.ChildId || d.FKRequestedFromUserId == objMobileViewRequestBo.ChildId)
                                    {
                                        AddData(ref viewRequestList, ref d, item);
                                    }
                                }
                            }
                            else
                            {
                                if (objMobileViewRequestBo.Id == 2)
                                {
                                    //manager
                                    if (d.CreatedBy == objMobileViewRequestBo.ChildId || d.FKRequestedFromUserId == objMobileViewRequestBo.ChildId)
                                    {
                                        AddData(ref viewRequestList, ref d, item);
                                    }

                                }
                                else if (objMobileViewRequestBo.Id == 3)
                                {
                                    //field supervisor
                                    if (objMobileViewRequestBo.ChildId == 0)
                                    {
                                        //check created by or requested to field supervisorid is null 
                                        // this is for all option
                                        if ((d.tblUser.FieldSupervisorId == null && d.tblUser.ManagerId != null) ||
                                            (d.tblUser1.FieldSupervisorId == null && d.tblUser1.ManagerId != null))
                                        {
                                            AddData(ref viewRequestList, ref d, item);
                                        }
                                    }
                                    else
                                    {
                                        if (d.FKRequestedFromUserId == objMobileViewRequestBo.ChildId || d.CreatedBy == objMobileViewRequestBo.ChildId)
                                        {
                                            AddData(ref viewRequestList, ref d, item);
                                        }
                                    }

                                }
                                else if (objMobileViewRequestBo.Id == 4)
                                {
                                    //installer

                                    //field supervisor
                                    if (objMobileViewRequestBo.ChildId == 0)
                                    {
                                        //check created by or requested to field supervisorid is null 
                                        // this is for all option
                                        if (d.tblUser.FieldSupervisorId != null || d.tblUser1.FieldSupervisorId != null)
                                        {
                                            AddData(ref viewRequestList, ref d, item);
                                        }
                                    }
                                    else
                                    {
                                        if (d.FKRequestedFromUserId == objMobileViewRequestBo.ChildId || d.CreatedBy == objMobileViewRequestBo.ChildId)
                                        {
                                            AddData(ref viewRequestList, ref d, item);
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            // User is not manager


                            if (CurrentUser.ManagerId != null)
                            {
                                if (objMobileViewRequestBo.Id == 0)
                                {

                                    if (objMobileViewRequestBo.ChildId == 0)
                                    {
                                        if (d.CreatedBy == objMobileViewRequestBo.UserId || d.FKRequestedFromUserId == objMobileViewRequestBo.UserId)
                                        {
                                            AddData(ref viewRequestList, ref d, item);
                                        }
                                    }
                                    else
                                    {
                                        if (objMobileViewRequestBo.ChildId == objMobileViewRequestBo.UserId)
                                        {
                                            if (d.CreatedBy == objMobileViewRequestBo.ChildId || d.FKRequestedFromUserId == objMobileViewRequestBo.ChildId)
                                            {
                                                AddData(ref viewRequestList, ref d, item);
                                            }

                                        }
                                        else
                                        {
                                            if ((d.CreatedBy == objMobileViewRequestBo.ChildId && d.FKRequestedFromUserId == objMobileViewRequestBo.UserId)
                                                || (d.CreatedBy == objMobileViewRequestBo.UserId && d.FKRequestedFromUserId == objMobileViewRequestBo.ChildId))
                                            {
                                                AddData(ref viewRequestList, ref d, item);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (objMobileViewRequestBo.ChildId == 0)
                                    {

                                        if (objMobileViewRequestBo.Id == 3)
                                        {

                                            if ((d.CreatedBy == CurrentUser.UserID && d.tblUser.FieldSupervisorId == null && d.tblUser.ManagerId != null)
                                               || (d.FKRequestedFromUserId == CurrentUser.UserID && d.tblUser1.FieldSupervisorId == null && d.tblUser1.ManagerId != null))
                                            {
                                                AddData(ref viewRequestList, ref d, item);
                                            }

                                        }
                                        else if (objMobileViewRequestBo.Id == 4)
                                        {

                                            if ((d.CreatedBy == CurrentUser.UserID && d.tblUser.FieldSupervisorId != null)
                                               || (d.FKRequestedFromUserId == CurrentUser.UserID && d.tblUser1.FieldSupervisorId != null))
                                            {
                                                AddData(ref viewRequestList, ref d, item);
                                            }

                                        }
                                    }
                                    else
                                    {
                                        if (objMobileViewRequestBo.ChildId == objMobileViewRequestBo.UserId)
                                        {
                                            if (d.CreatedBy == objMobileViewRequestBo.UserId || d.FKRequestedFromUserId == objMobileViewRequestBo.UserId)
                                            {
                                                AddData(ref viewRequestList, ref d, item);
                                            }
                                        }
                                        else
                                        {

                                            if ((d.CreatedBy == objMobileViewRequestBo.ChildId && d.FKRequestedFromUserId == objMobileViewRequestBo.UserId)
                                                                                           || (d.CreatedBy == objMobileViewRequestBo.UserId && d.FKRequestedFromUserId == objMobileViewRequestBo.ChildId))
                                            {
                                                AddData(ref viewRequestList, ref d, item);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }





            }
            catch (Exception ex)
            {
                throw ex;
            }
            return viewRequestList;

        }



        public void AddData(ref List<MobileViewResponseBo> viewRequestList, ref InventoryMobileUserStockRequest d, PROC_GetAllRequestOfManager_Result item)
        {
            var Obj1 = new MobileViewResponseBo();
            Obj1.Id = item.RequestId;
            Obj1.RequestStatus = item.StockRequestStatus;
            Obj1.RequestedBy = d.tblUser1.FirstName;
            Obj1.RequestedTo = d.tblUser.FirstName;
            Obj1.RequestDate = d.DeviceRequestDateTime.ToString("MMM-dd-yyyy");
            Obj1.RequestTime = d.DeviceRequestDateTime.ToString("hh:mm tt");
            Obj1.RequestUId = d.RequestIdWeb;
            viewRequestList.Add(Obj1);
        }

        public List<MobileViewResponseBo> GetRequestListManager(MobileViewRequestBo objMobileViewRequestBo)
        {


            List<MobileViewResponseBo> viewRequestList = new List<MobileViewResponseBo>();
            try
            {

                CompassEntities objCompassEntities = new CompassEntities();

                DateTime from = DateTime.Parse(objMobileViewRequestBo.FromDate);
                DateTime to = DateTime.Parse(objMobileViewRequestBo.Todate);
                //if (objMobileViewRequestBo.Id == 0 && objMobileViewRequestBo.ChildId == 0)
                //{

                viewRequestList = objCompassEntities.InventoryMobileUserStockRequests.Where(o => o.Active == 1
                    && EntityFunctions.TruncateTime(o.DeviceRequestDateTime) >= from.Date
                    && EntityFunctions.TruncateTime(o.DeviceRequestDateTime) <= to.Date).
                    Select(o => new MobileViewResponseBo
                    {
                        Id = o.RequestId,
                        RequestedBy = o.tblUser1.UserName,
                        RequestedTo = o.tblUser.UserName,
                        RequestStatus = o.InventoryRequestStatusMaster.StockRequestStatus
                    }).ToList();

                //}
                //else
                //{
                //    //
                //    viewRequestList = objCompassEntities.InventoryMobileUserStockRequests.Where(o => o.Active == 1
                //                         && o.CreatedBy == objMobileViewRequestBo.ChildId).
                //                         Select(o => new MobileViewResponseBo
                //                         {
                //                             Id = o.RequestId,
                //                             RequestedBy = o.tblUser1.UserName,
                //                             RequestedTo = o.tblUser.UserName,
                //                             RequestStatus = o.InventoryRequestStatusMaster.StockRequestStatus
                //                         }).ToList();
                //}

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return viewRequestList;

        }



        public MobileStockRequestDetails GetRequestDetails(long Id)
        {
            MobileStockRequestDetails objMobileStockRequestDetails = new MobileStockRequestDetails();
            try
            {

                CompassEntities objCompassEntities = new CompassEntities();



                var Data = objCompassEntities.InventoryMobileUserStockRequests.Where(o => o.Active == 1
                    && o.RequestId == Id).FirstOrDefault();

                objMobileStockRequestDetails = new MobileStockRequestDetails
                    {


                        Id = Data.RequestId,

                        RequestUId = Data.RequestIdWeb,
                        RequestedBy = Data.tblUser1.UserName,
                        RequestedTo = Data.tblUser.UserName,
                        RequestStatus = Data.InventoryRequestStatusMaster.StockRequestStatus,
                        DeviceRequestDate = Data.DeviceRequestDateTime.ToString(),
                        //  DeviceRequestTime = o.DeviceRequestDateTime,
                        Comment = Data.UserComment,
                        ProductRequestList = Data.InventoryMobileUserStockRequestDetails.Where(o1 => o1.Active == 1).Select(o1 => new RequestedStockDetails { ProductName = o1.ProductMaster.ProductName, Qty = o1.TotalQty }).ToList()
                    };

                var objProjectId = Data.FKProjectId.Value;

                objMobileStockRequestDetails.ProjectName = objCompassEntities.tblProjects.Where(o => o.ProjectId == objProjectId).Select(o => o.ProjectName).FirstOrDefault();



                var dateTimeValue = objMobileStockRequestDetails.DeviceRequestDate;
                objMobileStockRequestDetails.DeviceRequestDate = DateTime.Parse(dateTimeValue).ToString("MMM-dd-yyyy");
                objMobileStockRequestDetails.DeviceRequestTime = DateTime.Parse(dateTimeValue).ToString("hh:mm tt");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objMobileStockRequestDetails;
        }



        public bool UpdateNewRequest(MobileUpdateStockRequestBO obj, DateTime approvedDatetime, out string _Message)
        {

            _Message = "Success";
            bool result = false;

            try
            {

                CompassEntities objCompassEntities = new CompassEntities();

                var objMobileStockRequestDetails = objCompassEntities.InventoryMobileUserStockRequests.Where(o => o.Active == 1
                    && o.RequestId == obj.id && o.FKInventoryRequestStatusId == 1).FirstOrDefault();

                if (objMobileStockRequestDetails != null)
                {
                    objMobileStockRequestDetails.ModifiedBy = obj.userId;
                    objMobileStockRequestDetails.ModifiedOn = DateTime.Now;

                    if (obj.status == "approved")
                    {
                        objMobileStockRequestDetails.FKInventoryRequestStatusId = 3;
                        objMobileStockRequestDetails.FKApprovedBy = obj.userId;
                        objMobileStockRequestDetails.ApprovedDateTime = approvedDatetime;
                        objMobileStockRequestDetails.ApprovalComment = obj.comment;
                        objCompassEntities.SaveChanges();
                        new StockRequestDAL().sendMailonRequestApproveReject(objMobileStockRequestDetails.RequestId);
                        result = true;
                    }
                    else if (obj.status == "rejected")
                    {
                        objMobileStockRequestDetails.FKInventoryRequestStatusId = 2;
                        objMobileStockRequestDetails.FKApprovedBy = obj.userId;
                        objMobileStockRequestDetails.ApprovedDateTime = approvedDatetime;
                        objMobileStockRequestDetails.ApprovalComment = obj.comment;
                        objCompassEntities.SaveChanges();
                        new StockRequestDAL().sendMailonRequestApproveReject(objMobileStockRequestDetails.RequestId);
                        result = true;
                    }
                    else
                    {
                        _Message = "Wrong Input";
                    }
                }
                else
                {
                    _Message = "Your request failed. Request status changed for the current record.";
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
