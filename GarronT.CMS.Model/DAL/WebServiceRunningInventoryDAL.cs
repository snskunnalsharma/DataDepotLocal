using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace GarronT.CMS.Model.DAL
{
    public class WebServiceRunningInventoryDAL : IDisposable
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


        public MobileRunningInventoryBO GetRunningInventory(long Id, long ProjectId, string FromDate, string Todate)
        {
            MobileRunningInventoryBO objMobileRunningInventoryBO = new MobileRunningInventoryBO();
            try
            {

                CompassEntities objCompassEntities = new CompassEntities();

                DateTime dt = DateTime.Now;


                objMobileRunningInventoryBO = objCompassEntities.PROC_GetRunningInventory(Id, ProjectId, DateTime.Parse(FromDate), DateTime.Parse(Todate)).
                    Select(o => new MobileRunningInventoryBO
                    {
                        userId = o.AllocatedTo,
                        installedInventory = o.InstalledQty,
                        returnedInventory = int.Parse(o.ReturnToWarehouseQty.ToString()),
                        transferredInventory = o.TransferredQty,
                        allocatedInventory = o.AllocatedQty
                    }).FirstOrDefault();

                if (objMobileRunningInventoryBO == null)
                {

                    objMobileRunningInventoryBO = new MobileRunningInventoryBO
                    {
                        userId = Id,
                        installedInventory = 0,
                        returnedInventory = 0,
                        transferredInventory = 0,
                        allocatedInventory = 0
                    };
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objMobileRunningInventoryBO;
        }



        public bool MobileSignOffRequest(MobileUserSignOffBO objUserData, DateTime DeviceRequestDateTime, string JsonData, out string _Message)
        {
            _Message = "";
            bool result = false;

            try
            {

                CompassEntities objCompassEntities = new CompassEntities();

                InventoryUserSignOffDetail obj = new InventoryUserSignOffDetail();
                obj.Active = 1;
                obj.CreatedBy = objUserData.userId;
                obj.CreatedOn = DateTime.Now;

                obj.DeviceDateTime = DeviceRequestDateTime;
                obj.DeviceIP = objUserData.DeviceIPAddress;
                obj.GPSLocation = objUserData.DeviceGpsLocation;
                obj.JsonRequestData = JsonData;
                objCompassEntities.InventoryUserSignOffDetails.Add(obj);
                objCompassEntities.SaveChanges();


                //fire mail to manager on user sign off. 




                _Message = "Success";
                result = true;

                sendMailonRequestApproveReject(objUserData, DeviceRequestDateTime);


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }








        public AutoSynchInventory GetAutoSynchInventory(long userId, DateTime _CurrentDate)
        {
            AutoSynchInventory objAutoSynchInventory = new AutoSynchInventory();
            try
            {

                CompassEntities objCompassEntities = new CompassEntities();
                List<TransferredSerialStock> objSerialList = new List<TransferredSerialStock>();
                List<TransferredStock> objStockList = new List<TransferredStock>();

                MobileOfflineDAL objMobile = new MobileOfflineDAL();

                var obj = objMobile.ProjectData(userId, _CurrentDate);


                var data = objCompassEntities.PROC_GetAutoSynchInventory(userId).ToList();

                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        if (!string.IsNullOrEmpty(item.SerialNumber))
                        {
                            var meterMake = "";
                            var meterSize = "";
                            var meterType = "";
                            if (item.MeterMakeId > 0)
                                meterMake = objCompassEntities.tblMeterMakes.AsNoTracking().Where(o => o.ID == item.MeterMakeId && o.Active == 1).Select(o => o.Make).FirstOrDefault();
                            if (item.MeterSizeId > 0)
                                meterSize = objCompassEntities.tblMeterSizes.AsNoTracking().Where(o => o.ID == item.MeterSizeId && o.Active == 1).Select(o => o.MeterSize).FirstOrDefault();
                            if (item.MeterTypeId > 0)
                                meterType = objCompassEntities.tblMeterTypes.AsNoTracking().Where(o => o.ID == item.MeterTypeId && o.Active == 1).Select(o => o.MeterType).FirstOrDefault();

                            objSerialList.Add(
                                new TransferredSerialStock
                                {
                                    ProjectId = item.FKProjectId.Value,
                                    ProductId = item.FKProductId,
                                    ProductName = item.ProductName,
                                    SerialNumber = item.SerialNumber,
                                    ProductQuantity = item.TransferredQty,
                                    ProductMake = meterMake,
                                    ProductSize = meterSize,
                                    ProductType = meterType

                                });

                            objAutoSynchInventory.TransferRequestId = item.TransferRequestId.Value;
                        }
                        else
                        {
                            var checkRecordPresent = objStockList.Where(o => o.ProductId == item.FKProductId && o.ProjectId == item.FKProjectId).FirstOrDefault();
                            if (checkRecordPresent == null)
                            {
                                objStockList.Add(
                                    new TransferredStock
                                    {
                                        ProjectId = item.FKProjectId.Value,
                                        ProductId = item.FKProductId,
                                        ProductName = item.ProductName,
                                        ProductQuantity = item.TransferredQty
                                    });

                                objAutoSynchInventory.TransferRequestId = item.TransferRequestId.Value;
                            }
                            else
                            {
                                checkRecordPresent.ProductQuantity = checkRecordPresent.ProductQuantity + item.TransferredQty;
                            }
                        }
                    }
                }

                objAutoSynchInventory.SerialNumberList = objSerialList;
                objAutoSynchInventory.ProductQuantityList = objStockList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objAutoSynchInventory;
        }


        public AutoSynchInventory GetAutoSynchExistingAllocatedInventory(long userId, DateTime _CurrentDate)
        {
            AutoSynchInventory objAutoSynchInventory = new AutoSynchInventory();
            try
            {

                CompassEntities objCompassEntities = new CompassEntities();



                MobileOfflineDAL objMobile = new MobileOfflineDAL();

                var obj = objMobile.ProjectData(userId, _CurrentDate);

                List<TransferredSerialStock> objSerialList = new List<TransferredSerialStock>();
                List<TransferredStock> objStockList = new List<TransferredStock>();


                foreach (var item1 in obj)
                {

                    var data = objCompassEntities.PROC_WEBService_GetAllocatedInventoryRecords(userId, item1.ProjectId).ToList();

                    if (data != null && data.Count > 0)
                    {
                        foreach (var item in data)
                        {
                            if (!string.IsNullOrEmpty(item.SerialNumber))
                            {
                                var meterMake = "";
                                var meterSize = "";
                                var meterType = "";
                                if (item.MeterMakeId > 0)
                                    meterMake = objCompassEntities.tblMeterMakes.AsNoTracking().Where(o => o.ID == item.MeterMakeId && o.Active == 1).Select(o => o.Make).FirstOrDefault();
                                if (item.MeterSizeId > 0)
                                    meterSize = objCompassEntities.tblMeterSizes.AsNoTracking().Where(o => o.ID == item.MeterSizeId && o.Active == 1).Select(o => o.MeterSize).FirstOrDefault();
                                if (item.MeterTypeId > 0)
                                    meterType = objCompassEntities.tblMeterTypes.AsNoTracking().Where(o => o.ID == item.MeterTypeId && o.Active == 1).Select(o => o.MeterType).FirstOrDefault();
                                objSerialList.Add(new TransferredSerialStock { ProjectId = item1.ProjectId, ProductId = item.FKProductId, ProductName = item.ProductName, ProductMake = meterMake, ProductSize = meterSize, ProductType=meterType, PartNumber = item.PartNumber, Base = item.Base, SerialNumber = item.SerialNumber, ProductQuantity = int.Parse(item.BalanceQty.ToString()) });

                                objAutoSynchInventory.TransferRequestId = 0;
                            }
                            else
                            {
                                var checkRecordPresent = objStockList.Where(o => o.ProductId == item.FKProductId && o.ProjectId == item1.ProjectId).FirstOrDefault();
                                if (checkRecordPresent == null)
                                {
                                    objStockList.Add(new TransferredStock { ProjectId = item1.ProjectId, ProductId = item.FKProductId, ProductName = item.ProductName, ProductQuantity = int.Parse(item.BalanceQty.ToString()) });
                                    objAutoSynchInventory.TransferRequestId = 0;
                                }
                                else
                                {
                                    checkRecordPresent.ProductQuantity = checkRecordPresent.ProductQuantity + int.Parse(item.BalanceQty.ToString());
                                }
                            }
                        }
                    }
                }


                objAutoSynchInventory.SerialNumberList = objSerialList;
                objAutoSynchInventory.ProductQuantityList = objStockList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objAutoSynchInventory;
        }



        public bool UpdateAutoSynchInventoryFlag(long userId, long TransferRequestId)
        {
            bool a = false;
            try
            {
                if (TransferRequestId == 0)
                {
                    a = true;
                }
                else
                {
                    TransactionOptions options = new TransactionOptions();
                    options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                    options.Timeout = new TimeSpan(0, 15, 0);


                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, options))
                    {
                        CompassEntities objCompassEntities = new CompassEntities();

                        var data = objCompassEntities.OutwardTransferDetails.Where(o => o.Active == 1 && o.RecordSynched != 1 &&
                            o.FKUserIdReducedStock == userId && o.TransferRequestId <= TransferRequestId).ToList();

                        foreach (var item in data)
                        {
                            item.RecordSynched = 1;
                            item.SynchedDatetime = DateTime.Now;
                        }

                        objCompassEntities.SaveChanges();
                        scope.Complete();
                        a = true;
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return a;
        }










        public void sendMailonRequestApproveReject(MobileUserSignOffBO objUserData, DateTime DeviceRequestDateTime)
        {
            #region Added code by Aniket To send mail to Requester


            List<String> mailto = new List<String>();
            CompassEntities db = new CompassEntities();

            var user = db.tblUsers.Where(o => o.UserID == objUserData.userId).FirstOrDefault();

            if (user.ManagerId != null)
            {
                string managerMailId = "";
                managerMailId = db.tblUsers.Where(o => o.UserID == user.ManagerId).Select(o => o.Email).FirstOrDefault();
                mailto.Add(managerMailId);
            }
            else
            {
                mailto.Add(user.Email);
            }



            string Subject = "CMS Team- User - " + user.FirstName + " Sign Off Details For The Day.";

            // log.Info("Installer Allocation success-" + DateTime.Now.ToString());




            Task.Factory.StartNew(() =>
            {


                string message = GetMailBody(user.FirstName, objUserData, DeviceRequestDateTime);

                // log.Info("Calling Mail send method at-" + DateTime.Now.ToString());
                new GarronT.CMS.Model.clsMail().SendMail(mailto, new List<string>(), message, Subject);
                // log.Info("end of Mail send method at-" + DateTime.Now.ToString());
            });


            #endregion
        }



        public string GetMailBody(string userName, MobileUserSignOffBO objUserData, DateTime DeviceRequestDateTime)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("<BODY style='FONT-SIZE: 14px; FONT-FAMILY: Times New Roman; BACKGROUND: #f8f8fb; PADDING-BOTTOM: 0px; PADDING-TOP: 10px; PADDING-LEFT: 10px; PADDING-RIGHT: 0px' dir=ltr>" +
                   "<div dir=ltr>" +
        "<div style='FONT-SIZE: 12pt; FONT-FAMILY:Calibri; COLOR: #000000'>" +
            "<div style='FONT-SIZE: small; TEXT-DECORATION: none; FONT-FAMILY:Calibri; FONT-WEIGHT: normal; COLOR: #000000; FONT-STYLE: normal; DISPLAY: inline'>" +
                " Hi, " +
                "<P style='FONT-SIZE: 15px'>Greetings from Data Depot....!!!</P>" +
                "<P>This is to inform you that the user- " + userName + " is Signed Off for the day.</P>" +
                "<div>Below is his stock and other details of sign off.</div>" +
                "<div>&nbsp;</div>" +
                "<div>&nbsp;</div>" +
                 "<div >" +
                  "<table style='border:collapse; width:500px;'>" +
                 "<tr>" +
                 "<td style='width:20%'><b>Date</b></td>" +
                 "<td style='width:2%'>:</td>" +
                 "<td style='width:28%'>" + DeviceRequestDateTime.ToString("MMM-dd-yyyy") + "</td>" +
                 "<td style='width:28%'><b>Time </td> " +
                  "<td style='width:2%'>:</td>" +
                 "<td style='width:20%'>" + DeviceRequestDateTime.ToString("hh:mm tt") + "</td>" +
                  "</tr>" +

                   "<tr>" +
                  "<td style='width:20%'><b>IP Address</b></td>" +
                  "<td style='width:2%'>:</td>" +
                  "<td style='width:28%'>" + objUserData.DeviceIPAddress + "</td>" +
                 "<td style='width:28%'><b> GPS Location</b> </td>" +
                  "<td style='width:2%'>:</td>" +
                "<td style='width:20%'>" + objUserData.DeviceGpsLocation + " </td>" +

                  "</tr>" +
                 "</table>" +
                 "<div>&nbsp;</div>" +
                 "<div>&nbsp;</div>" +
                "<div style='width:500px;'>" +
                    "<table style='width:100%;BORDER-COLLAPSE: collapse; COLOR: #000000; PADDING-BOTTOM: 0px; PADDING-TOP: 0px; PADDING-LEFT: 10px; PADDING-RIGHT: 0px'>" +
                      "<THEAD>" +
                    "<TR style=''>" +
                                "<TH style='width:50%;BACKGROUND-COLOR: #80b2df;BORDER-TOP: black 1px solid; BORDER-RIGHT: black 1px solid; BORDER-BOTTOM: black 1px solid; PADDING-BOTTOM: 0px; PADDING-TOP: 0px; PADDING-LEFT: 8px; BORDER-LEFT: black 1px solid; PADDING-RIGHT: 8px'>Type Of Inventory</th>" +
                                "<TH style='width:50%;BACKGROUND-COLOR: #80b2df;BORDER-TOP: black 1px solid; BORDER-RIGHT: black 1px solid; BORDER-BOTTOM: black 1px solid; PADDING-BOTTOM: 0px; PADDING-TOP: 0px; PADDING-LEFT: 8px; BORDER-LEFT: black 1px solid; PADDING-RIGHT: 8px'>Count</th>" +
                //"<th>&nbsp;</th>" +
                //  " <th>&nbsp;</th>" +
                //  "<th>&nbsp;</th>" +                                
                                "</tr>" +
                                 "</THEAD>" +
                        "<TBODY style='FONT-SIZE: 13px'>" +
                            "<TR style='FONT-SIZE: 13px; TEXT-ALIGN: center; BACKGROUND-COLOR: #c1dffa'>" +
                                "<TD style='BORDER-TOP: black 1px solid; BORDER-RIGHT: black 1px solid; WIDTH: 5%; BORDER-BOTTOM: black 1px solid; FONT-WEIGHT: 500; TEXT-ALIGN: center; BORDER-LEFT: black 1px solid'>Allocated Inventory</td>" +
                                "<TD style='BORDER-TOP: black 1px solid; BORDER-RIGHT: black 1px solid; WIDTH: 5%; BORDER-BOTTOM: black 1px solid; FONT-WEIGHT: 500; TEXT-ALIGN: center; BORDER-LEFT: black 1px solid'>" + objUserData.allocatedInventory.ToString() + "</td>" +
                                   "</tr>" +
                             "<TR style='FONT-SIZE: 13px; TEXT-ALIGN: center; BACKGROUND-COLOR: #c1dffa'>" +
                                "<TD style='BORDER-TOP: black 1px solid; BORDER-RIGHT: black 1px solid; WIDTH: 5%; BORDER-BOTTOM: black 1px solid; FONT-WEIGHT: 500; TEXT-ALIGN: center; BORDER-LEFT: black 1px solid'>Installed Inventory</td>" +
                                "<TD style='BORDER-TOP: black 1px solid; BORDER-RIGHT: black 1px solid; WIDTH: 5%; BORDER-BOTTOM: black 1px solid; FONT-WEIGHT: 500; TEXT-ALIGN: center; BORDER-LEFT: black 1px solid'>" + objUserData.installedInventory.ToString() + "</td>" +
                                   "</tr>" +
                             "<TR style='FONT-SIZE: 13px; TEXT-ALIGN: center; BACKGROUND-COLOR: #c1dffa'>" +
                                "<TD style='BORDER-TOP: black 1px solid; BORDER-RIGHT: black 1px solid; WIDTH: 5%; BORDER-BOTTOM: black 1px solid; FONT-WEIGHT: 500; TEXT-ALIGN: center; BORDER-LEFT: black 1px solid'>Transferred Inventory</td>" +
                                "<TD style='BORDER-TOP: black 1px solid; BORDER-RIGHT: black 1px solid; WIDTH: 5%; BORDER-BOTTOM: black 1px solid; FONT-WEIGHT: 500; TEXT-ALIGN: center; BORDER-LEFT: black 1px solid'>" + objUserData.transferredInventory.ToString() + "</td>" +
                                   "</tr>" +
                              "<TR style='FONT-SIZE: 13px; TEXT-ALIGN: center; BACKGROUND-COLOR: #c1dffa'>" +
                                "<TD style='BORDER-TOP: black 1px solid; BORDER-RIGHT: black 1px solid; WIDTH: 5%; BORDER-BOTTOM: black 1px solid; FONT-WEIGHT: 500; TEXT-ALIGN: center; BORDER-LEFT: black 1px solid'>Returned Inventory</td>" +
                                "<TD style='BORDER-TOP: black 1px solid; BORDER-RIGHT: black 1px solid; WIDTH: 5%; BORDER-BOTTOM: black 1px solid; FONT-WEIGHT: 500; TEXT-ALIGN: center; BORDER-LEFT: black 1px solid'>" + objUserData.returnedInventory.ToString() + "</td>" +
                                   "</tr>" +
                                    "<TR style='FONT-SIZE: 13px; TEXT-ALIGN: center; BACKGROUND-COLOR: #c1dffa'>" +
                                "<TD style='BORDER-TOP: black 1px solid; BORDER-RIGHT: black 1px solid; WIDTH: 5%; BORDER-BOTTOM: black 1px solid; FONT-WEIGHT: 500; TEXT-ALIGN: center; BORDER-LEFT: black 1px solid'>Balance Inventory</td>" +
                                "<TD style='BORDER-TOP: black 1px solid; BORDER-RIGHT: black 1px solid; WIDTH: 5%; BORDER-BOTTOM: black 1px solid; FONT-WEIGHT: 500; TEXT-ALIGN: center; BORDER-LEFT: black 1px solid'>" + objUserData.balanceInventory.ToString() + "</td>" +
                                   "</tr>" +
                        "</tbody>" +
                    "</table>" +
                "</div><BR><BR><BR>" +


        "<BR>Thank You,<BR>Data Depot Team.<BR>" +
        "<P style='TEXT-ALIGN: left'>" +
            "<A title='' href='https://www.datadepotonline.com/'" +
               "rel =home>  <IMG title='Data Depot' style='HEIGHT: 100px; WIDTH: 200px' alt='' src='https://www.datadepotonline.com/images/logo_03_2.png'>" +
            "</A>" +
        "</P>" +
        "<div style='max-width:580px; BACKGROUND: #e6e1e1'><P style='FONT-SIZE: 13px; TEXT-ALIGN: left'>" +
            "You are receiving this notification because you are registered with Data Depot ©" + DateTime.Now.ToString("yyyy") + " Data Depot" +
        "</P></div><BR>" +
        "<div style='max-width:580px; BACKGROUND: #e6e1e1'><P style='FONT-SIZE: 15px; TEXT-ALIGN: left'>" +
            "*** This Email is system generated. Please do not reply to this email ID.For any queries please contact Data Depot Team. ***" +
        "</P></div>" +
        "<div style='max-width:580px; BACKGROUND: #e6e1e1'><P style='FONT-SIZE: 13px; TEXT-ALIGN: left'>" +
            "This message (including any attachments) contains confidential information intended for a" +
            " specific individual and purpose and is protected by law. If you are not the" +
            " intended recipient, you should delete this message.Any disclosure, copying or" +
           " distribution of this message, or the taking of any action based on it, is" +
            " strictly prohibited." +
        "</P></div>" +
    "</div>" +
"</div>" +
"</div>" +
"</BODY>");

            return sb.ToString();
        }








    }
}
