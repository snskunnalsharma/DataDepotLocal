using GarronT.CMS.Model.BO;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.DAL
{
    public class StockRequestDAL
    {
        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(StockRequestDAL));
        #endregion

        /// <summary>
        /// Aniket
        /// 01-Aug-2017
        /// GetStockRequest header list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<StockRequestModel> GetStockRequest()
        {
            IEnumerable<StockRequestModel> stockRequestList = null;
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();
                stockRequestList = objCompassEntities.VW_ManageStockRequest.AsEnumerable().Select(a => new StockRequestModel
                {
                    RequestId = a.RequestId,
                    ProjectId = a.ProjectId,
                    ProjectName = a.ProjectName,
                    RequestIdWeb = a.RequestIdWeb,
                    FKRequestedFromUserId = a.FKRequestedFromUserId,
                    RequestFrom = a.RequestFrom,
                    CreatedBy = a.CreatedBy,
                    RequestTo = a.RequestTo,
                    DeviceRequestDateTime = a.DeviceRequestDateTime,
                    DeviceDateTime = a.DeviceRequestDateTime.ToString("MMM-dd-yyyy"),
                    DeviceTime = a.DeviceRequestDateTime.ToString("h:mm tt"),
                    FKInventoryRequestStatusId = a.FKInventoryRequestStatusId,
                    StockRequestStatus = a.StockRequestStatus,
                    DeviceIPAddress = a.DeviceIPAddress,
                    DeviceGpsLocation = a.DeviceGpsLocation,
                    UserComment = a.UserComment,
                    CreatedOn = a.createdon
                }).OrderByDescending(a => a.CreatedOn).ToList();


            }
            catch (Exception ex)
            {
                throw;
            }
            return stockRequestList;
        }

        /// <summary>
        /// Aniket
        /// 01-Aug-2017
        /// Approve or reject record
        /// </summary>
        /// <returns></returns>
        public bool ApproveRejectRecord(long userId, long requestId, int action, string comment, ref string errorMessage)
        {
            CompassEntities _dbContext = new CompassEntities();
            bool success = false;
            errorMessage = "Unable to Approve/Reject record";
            try
            {
                InventoryMobileUserStockRequest editRecord = _dbContext.InventoryMobileUserStockRequests.Where(a => a.RequestId == requestId).FirstOrDefault();
                if (editRecord != null)
                {
                    editRecord.FKInventoryRequestStatusId = action == 1 ? 3 : 2;
                    editRecord.ApprovalComment = comment;
                    editRecord.ModifiedBy = userId;
                    editRecord.ModifiedOn = new CommonFunctions().ServerDate();
                    _dbContext.SaveChanges();
                    success = true;
                    if (action == 1)
                    {
                        errorMessage = "Record Approved.";
                    }
                    else
                    {
                        errorMessage = "Record Rejected.";
                    }

                    sendMailonRequestApproveReject(editRecord);
                    //#region Added code by Aniket To send mail to Requester
                    //if (success)
                    //{

                    //    List<String> mailto = new List<String>();
                    //    CompassEntities db = new CompassEntities();
                    //    var UserEmailIdfrom = db.tblUsers.Where(n => n.UserID == editRecord.CreatedBy).FirstOrDefault();
                    //    var UserEmailIdto = db.tblUsers.Where(n => n.UserID == editRecord.FKRequestedFromUserId).FirstOrDefault();

                    //    var manager = db.tblUsers.Where(n => n.UserID == UserEmailIdfrom.ManagerId).FirstOrDefault();


                    //    string Subject = "";
                    //    if (action == 1)
                    //    {
                    //        Subject = "CMS Team-New Stock request approved";
                    //    }
                    //    else
                    //    {
                    //        Subject = "CMS Team-New Stock request rejected";
                    //    }
                    //    // log.Info("Installer Allocation success-" + DateTime.Now.ToString());

                    //    List<String> CCEmailTo = new List<String>();
                    //    if (manager != null)
                    //    {
                    //        CCEmailTo.Add(manager.Email);
                    //    }


                    //    Task.Factory.StartNew(() =>
                    //    {
                    //        mailto.Add(UserEmailIdfrom.Email);
                    //        mailto.Add(UserEmailIdto.Email);

                    //        string message = GetMailBody(requestId, UserEmailIdto.UserName);

                    //        log.Info("Calling Mail send method at-" + DateTime.Now.ToString());
                    //        new GarronT.CMS.Model.clsMail().SendMail(mailto, CCEmailTo, message, Subject);
                    //        log.Info("end of Mail send method at-" + DateTime.Now.ToString());
                    //    });

                    //}
                    //#endregion
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return success;
        }

        public void sendMailonRequestApproveReject(InventoryMobileUserStockRequest editRecord)
        {
            #region Added code by Aniket To send mail to Requester


            List<String> mailto = new List<String>();
            CompassEntities db = new CompassEntities();
            var UserEmailIdfrom = db.tblUsers.Where(n => n.UserID == editRecord.CreatedBy).FirstOrDefault();
            var UserEmailIdto = db.tblUsers.Where(n => n.UserID == editRecord.FKRequestedFromUserId).FirstOrDefault();

            var manager = db.tblUsers.Where(n => n.UserID == UserEmailIdfrom.ManagerId).FirstOrDefault();


            string Subject = "";
            if (editRecord.FKInventoryRequestStatusId == 3)
            {
                Subject = "Data Depot Team-New Stock request approved";
            }
            else if (editRecord.FKInventoryRequestStatusId == 2)
            {
                Subject = "Data Depot Team-New Stock request rejected";
            }
            else if (editRecord.FKInventoryRequestStatusId == 1)
            {
                Subject = "Data Depot Team-New Stock request";
            }
            else
            {
                return;
            }
            // log.Info("Installer Allocation success-" + DateTime.Now.ToString());

            List<String> CCEmailTo = new List<String>();
            if (manager != null)
            {
                CCEmailTo.Add(manager.Email);
            }


            Task.Factory.StartNew(() =>
            {
                mailto.Add(UserEmailIdfrom.Email);
                mailto.Add(UserEmailIdto.Email);

                string message = GetMailBody(editRecord.RequestId, UserEmailIdto.UserName);

                log.Info("Calling Mail send method at-" + DateTime.Now.ToString());
                new GarronT.CMS.Model.clsMail().SendMail(mailto, CCEmailTo, message, Subject);
                log.Info("end of Mail send method at-" + DateTime.Now.ToString());
            });


            #endregion
        }


        public void sendMailonRequestApproveReject(long RequestId)
        {
            #region Added code by Aniket To send mail to Requester


            List<String> mailto = new List<String>();
            CompassEntities db = new CompassEntities();

            InventoryMobileUserStockRequest editRecord = db.InventoryMobileUserStockRequests.Where(o => o.RequestId == RequestId).FirstOrDefault();
            var UserEmailIdfrom = db.tblUsers.Where(n => n.UserID == editRecord.CreatedBy).FirstOrDefault();
            var UserEmailIdto = db.tblUsers.Where(n => n.UserID == editRecord.FKRequestedFromUserId).FirstOrDefault();

            var manager = db.tblUsers.Where(n => n.UserID == UserEmailIdfrom.ManagerId).FirstOrDefault();


            string Subject = "";
            if (editRecord.FKInventoryRequestStatusId == 3)
            {
                Subject = "Data Depot Team-New Stock request approved";
            }
            else if (editRecord.FKInventoryRequestStatusId == 2)
            {
                Subject = "Data depot Team-New Stock request rejected";
            }
            else if (editRecord.FKInventoryRequestStatusId == 1)
            {
                Subject = "Data Depot Team-New Stock request";
            }
            else
            {
                return;
            }
            // log.Info("Installer Allocation success-" + DateTime.Now.ToString());

            List<String> CCEmailTo = new List<String>();
            if (manager != null)
            {
                CCEmailTo.Add(manager.Email);
            }


            Task.Factory.StartNew(() =>
            {
                mailto.Add(UserEmailIdfrom.Email);
                mailto.Add(UserEmailIdto.Email);

                string message = GetMailBody(editRecord.RequestId, UserEmailIdto.UserName);

                log.Info("Calling Mail send method at-" + DateTime.Now.ToString());
                new GarronT.CMS.Model.clsMail().SendMail(mailto, CCEmailTo, message, Subject);
                log.Info("end of Mail send method at-" + DateTime.Now.ToString());
            });


            #endregion
        }



        public string GetMailBody(long requestId, string username)
        {
            //StockRequestDetailVM StockRequestDetailVM = new StockRequestDetailVM();
            var StockRequestModel = new StockRequestDAL().GetStockRequest().Where(a => a.RequestId == requestId).FirstOrDefault();
            var StockRequestDetailModelList = GetStockDetailRequest(requestId);
            var requestStatus = "";
            if (StockRequestModel.FKInventoryRequestStatusId == 3)
            {
                requestStatus = "Approve";
            }
            else if (StockRequestModel.FKInventoryRequestStatusId == 2)
            {
                requestStatus = "Rejected";
            }
            else if (StockRequestModel.FKInventoryRequestStatusId == 1)
            {
                requestStatus = "Pending";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("<BODY style='FONT-SIZE: 14px; FONT-FAMILY: Times New Roman; BACKGROUND: #f8f8fb; PADDING-BOTTOM: 0px; PADDING-TOP: 10px; PADDING-LEFT: 10px; PADDING-RIGHT: 0px' dir=ltr>" +
                   "<div dir=ltr>" +
        "<div style='FONT-SIZE: 12pt; FONT-FAMILY:Calibri; COLOR: #000000'>" +
            "<div style='FONT-SIZE: small; TEXT-DECORATION: none; FONT-FAMILY:Calibri; FONT-WEIGHT: normal; COLOR: #000000; FONT-STYLE: normal; DISPLAY: inline'>" +
                "Dear " + username +
                ",<P style='FONT-SIZE: 15px'>Greetings from Data Depot....!!!</P>" +
                "<P>Please check manager feedback on your new stock request</P>" +
                "<div>&nbsp;</div>" +
                "<div style='WIDTH: 100%; BACKGROUND: #e6e1e1'>" +
                    "<table style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; COLOR: #000000; PADDING-BOTTOM: 0px; PADDING-TOP: 0px; PADDING-LEFT: 10px; BORDER-LEFT: medium none; PADDING-RIGHT: 0px'>" +
                        "<tbody>" +
                             "<tr>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'><B>RequestId</B></td>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'><B>:</B></td>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'>" + StockRequestModel.RequestIdWeb + "</td>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'><B>Request Status</B></td>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'><B>:</B></td>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'>" + requestStatus + "</td>" +
                            "</tr>" +


                            "<tr>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'><B>From</B></td>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'><B>:</B></td>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'>" + StockRequestModel.RequestFrom + "</td>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'><B>To</B></td>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'><B>:</B></td>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'>" + StockRequestModel.RequestTo + "</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<TD style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'><B>Device Date</B></TD>" +
                                "<TD style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'><B>:</B></TD>" +
                                "<TD style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'>" + StockRequestModel.DeviceDateTime + "</TD>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'><B>Device Time</B></td>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'><B>:</B></td>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'>" + StockRequestModel.DeviceTime + "</td>" +
                            "</tr>" +
                            "<tr>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'><B>Device IP</B></td>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'><B>:</B></td>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'>" + StockRequestModel.DeviceIPAddress + "</td>" +
                                "<td style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'><B>GPS Location</B></td>" +
                                "<TD style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'><B>:</B></TD>" +
                                "<TD style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'>" + StockRequestModel.DeviceGpsLocation + "</TD>" +
                            "</tr>" +
                            "<TR>" +
                                "<TD style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'><B>Comments</B> </TD>" +
                                "<TD style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'><B>:</B></TD>" +
                                "<TD style='BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none'>" + StockRequestModel.ApprovalComment + "</TD>" +
                            "</TR>" +
                        "</tbody>" +
                    "</table>" +
                "</div><BR><B>Product Stocks</B><BR><BR>" +

                "<TABLE style='BORDER-COLLAPSE: collapse; COLOR: #000000; PADDING-BOTTOM: 0px; PADDING-TOP: 0px; PADDING-LEFT: 10px; PADDING-RIGHT: 0px'>" +
                    "<THEAD>" +
                        "<TR style='BACKGROUND-COLOR: #80b2df'>" +
                            "<TH style='BORDER-TOP: black 1px solid; BORDER-RIGHT: black 1px solid; BORDER-BOTTOM: black 1px solid; PADDING-BOTTOM: 0px; PADDING-TOP: 0px; PADDING-LEFT: 8px; BORDER-LEFT: black 1px solid; PADDING-RIGHT: 8px'>Sr.No</TH>" +
                            "<TH style='BORDER-TOP: black 1px solid; BORDER-RIGHT: black 1px solid; BORDER-BOTTOM: black 1px solid; PADDING-BOTTOM: 0px; PADDING-TOP: 0px; PADDING-LEFT: 8px; BORDER-LEFT: black 1px solid; PADDING-RIGHT: 8px'>Product Name</TH>" +
                            "<TH style='BORDER-TOP: black 1px solid; BORDER-RIGHT: black 1px solid; BORDER-BOTTOM: black 1px solid; PADDING-BOTTOM: 0px; PADDING-TOP: 0px; PADDING-LEFT: 10px; BORDER-LEFT: black 1px solid; PADDING-RIGHT: 10px'>Quantity</TH>" +
                        "</TR>" +
                    "</THEAD>" +
                    "<TBODY style='FONT-SIZE: 13px'>");
            int index = 1;
            foreach (var item in StockRequestDetailModelList)
            {
                sb.Append("<TR style='FONT-SIZE: 13px; TEXT-ALIGN: center; BACKGROUND-COLOR: #c1dffa'>" +
                            "<TD style='BORDER-TOP: black 1px solid; BORDER-RIGHT: black 1px solid; WIDTH: 5%; BORDER-BOTTOM: black 1px solid; FONT-WEIGHT: 500; TEXT-ALIGN: center; BORDER-LEFT: black 1px solid'>" +
                              index +
                            "</TD>" +
                            "<TD style='BORDER-TOP: black 1px solid; BORDER-RIGHT: black 1px solid; WIDTH: 5%; BORDER-BOTTOM: black 1px solid; FONT-WEIGHT: 500; TEXT-ALIGN: center; BORDER-LEFT: black 1px solid'>" +
                               item.ProductName +
                            "</TD>" +
                            "<TD style='BORDER-TOP: black 1px solid; BORDER-RIGHT: black 1px solid; WIDTH: 5%; BORDER-BOTTOM: black 1px solid; FONT-WEIGHT: 500; TEXT-ALIGN: center; BORDER-LEFT: black 1px solid'>" +
                              item.Quantity +
                            "</TD>" +
                        "</TR>");
                index = index + 1;
            }

            sb.Append("</TBODY>" +
        "</TABLE>" +
          "<BR>Thank You,<BR>Data Depot Team.<BR>" +
        "<P style='TEXT-ALIGN: left'><A title='' href='https://www.datadepotonline.com/' rel =home> <IMG title='Data Depot'" +
                     "style='HEIGHT: 100px; WIDTH: 200px' alt='' src='https://www.datadepotonline.com/images/logo_03_2.png'> </A> </P>" +
        "<div style='max-width:580px; BACKGROUND: #e6e1e1'><P style='FONT-SIZE: 13px; TEXT-ALIGN: left'>" +
            "You are receiving this notification because you are registered with Data Depot ©" + DateTime.Now.ToString("yyyy") + " Data Depot" +
        "</P></div><BR>" +
        "<div style='max-width:580px; BACKGROUND: #e6e1e1'><P style='FONT-SIZE: 15px; TEXT-ALIGN: left'>" +
            "*** This Email is system generated. Please do not reply to this email ID.For any queries please contact Data Depot Team. ***" +
        "</P></div>" +
        "<div style='max-width:580px; BACKGROUND: #e6e1e1'><P style='FONT-SIZE: 13px; TEXT-ALIGN: left'>" +
            "This message (including any attachments) contains confidential information intended for a" +
            " specific individual and purpose and is protected by law. If you are not the" +
            " intended recipient, you should delete this message. Any disclosure, copying or" +
           " distribution of this message, or the taking of any action based on it, is" +
            " strictly prohibited." +
        "</P></div>" +
    "</div>" +
"</div>" +
"</div>" +
"</BODY>");

            return sb.ToString();
        }


        public List<StockRequestDetailModel> GetStockDetailRequest(long requestId)
        {
            List<StockRequestDetailModel> stockdetailRequestList = new List<StockRequestDetailModel>();
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();
                stockdetailRequestList = objCompassEntities.InventoryMobileUserStockRequestDetails.Where(a => a.FKInventoryMobileUserStockRequestId == requestId).Select(a => new StockRequestDetailModel
                {
                    ProductName = objCompassEntities.ProductMasters.Where(b => b.Id == a.FKProductId).FirstOrDefault().ProductName,
                    Quantity = a.TotalQty
                }).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
            return stockdetailRequestList;
        }
    }
}
