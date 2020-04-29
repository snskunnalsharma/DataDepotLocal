using AutoMapper;
using GarronT.CMS.Model.BO;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Security;

namespace GarronT.CMS.Model.DAL
{
    public class AppointmentDAL : IDisposable
    {
        bool disposed;

        public object WebSecurity { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Get ProjectID and Uploaded Id by Account
        public UploadedExcelData GetProjectIDUploadID(string account)
        {
            UploadedExcelData _objUpload = new UploadedExcelData();
            using (CompassEntities ObjCompass = new CompassEntities())
            {
                _objUpload = ObjCompass.UploadedExcelDatas.Where(o => o.Account12 == account && o.Active == 1).FirstOrDefault();
            }
            return _objUpload;
        }

        public UploadedExcelData checkUploadIdPresent(string uploadId,string account)
        {
            UploadedExcelData _objUpload = new UploadedExcelData();
            using (CompassEntities ObjCompass = new CompassEntities())
            {
                long UID = long.Parse(uploadId);
                _objUpload = ObjCompass.UploadedExcelDatas.Where(o => o.Id == UID && o.Account12 == account && o.Active == 1).FirstOrDefault();
            }
            return _objUpload;
        }

        #endregion

        #region Get Project Details by Project ID
        public tblProject GetProjectDetails(long projectId)
        {
            tblProject objProj = new tblProject();
            using (CompassEntities objCompass = new CompassEntities())
            { 
                objProj = objCompass.tblProjects.Where(o => o.ProjectId == projectId && o.ProjectStatus == "Active" && o.Active == 1).FirstOrDefault();
            }
            return objProj;
        }

        #endregion

        #region Insert  Booking appointment in the DB
        public bool InsertRecord(AppointmentModel appVM, out string _result)
        {
            DateTime dt = new CommonFunctions().ServerDate();
            CompassEntities objCompassEntities = new CompassEntities();

            //Find Logged User ID
            var name = System.Web.HttpContext.Current.User.Identity.Name;
            var uID = objCompassEntities.tblUsers.Where(x => x.UserName == name && x.Active == 1).FirstOrDefault();
            int userId = 1;
            if (uID != null)
            {
                userId = (int)uID.UserID;
            }




            if (appVM != null)
            {
                Appointment objAppointment = new Appointment();
                Mapper.DynamicMap<AppointmentModel, Appointment>(appVM, objAppointment);
                objAppointment.AccountNumber = appVM.AccountNumber;
                objAppointment.FK_UploadedId = appVM.FK_UploadedId;
                objAppointment.PassedDateFlag = 0;
                objAppointment.FirstName = appVM.FirstName;
                objAppointment.LastName = appVM.LastName;
                objAppointment.EmailAddress = appVM.EmailAddress;
                objAppointment.DayTimePhone = appVM.DayTimePhone;
                objAppointment.Address = appVM.Address;
                objAppointment.FKProjectId = appVM.FKProjectId;              
                objAppointment.FirstPreferedDate = appVM.FirstPreferedDate;
                objAppointment.FirstTime = appVM.FirstTime;           
                objAppointment.Cycle = appVM.Cycle;                
                objAppointment.Route = appVM.Route;
                objAppointment.City = appVM.City;
                objAppointment.State = appVM.State;
                objAppointment.Active = 1;
                objAppointment.CreatedBy = userId;
                objAppointment.CreatedOn = dt;
                objAppointment.ModifiedBy = userId;
                objAppointment.ModifiedOn = dt;            

                objCompassEntities.Appointments.Add(objAppointment);
                objCompassEntities.SaveChanges();

                _result = "Appointment booked successfuly.";
                return true;
            }
            else
            {
                _result = "We are unable to fullfil your request. Please try again";
                return false;
            }



        }


        #endregion        

        #region Get Project List
        public List<ProjectModel> GetProjectName()
        {
            CompassEntities CompassEntities = new CompassEntities();

            List<ProjectModel> data = CompassEntities.STP_GetProjectCityState().Where(a => a.Active == 1 && a.ProjectStatus == "Active").Select(o =>
                                       new ProjectModel
                                       {
                                           ProjectId = o.ProjectId,
                                           ProjectCityState = o.ProjectCityName
                                       }).OrderBy(a => a.ProjectCityState).ToList();
            return data;
        }


        #endregion

        #region Get State List
        public List<StateModel> GetActiveStateRecords()
        {
            CompassEntities CompassEntities = new CompassEntities();
            List<StateModel> objSubMenuesBo = (from b in CompassEntities.tblStateMasters


                                               select new StateModel
                                               {
                                                   StateID = b.StateId,
                                                   StateName = b.StateName
                                               }).OrderBy(t => t.StateName).ToList();

            return objSubMenuesBo;
        }



        #endregion

        #region Get City List DDL By State ID   
        public List<CityModel> GetCityByIDs(int id)
        {
            CompassEntities CompassEntities = new CompassEntities();
            List<CityModel> objCityModel = (from b in CompassEntities.TblCityMasters
                                            where b.StateId == id && b.Active == 1
                                            select new CityModel
                                            {
                                                CityID = b.CityId,
                                                CityName = b.CityName,
                                                // StateID = b.StateId,

                                                Active = b.Active
                                            }).OrderBy(t => t.CityName).ToList();

            List<CityModel> uniqueIDs = objCityModel.GroupBy(x => x.CityName).Select(x => x.First()).ToList();
            return uniqueIDs;


        }

        #endregion

        #region  Get All Booked Appointment to display but not used yet
        //public List<AppointmentModel> GetAppointmentList()
        //{
        //    CompassEntities objCompassEntities = new CompassEntities();
        //    List<AppointmentModel> list = (from A in objCompassEntities.Appointments
        //                                   select new AppointmentModel
        //                                   {
        //                                       AppointmentID=A.AppointmentID,
        //                                       AccountNumber=A.AccountNumber,
        //                                       FirstName=A.FirstName,
        //                                       LastName=A.LastName,
        //                                       EmailAddress=A.EmailAddress,
        //                                       DayTimePhone=A.DayTimePhone,
        //                                       Address=A.Address,
        //                                       FKStateId=A.FKStateId,
        //                                       FKProjectId =A.FKProjectId,
        //                                       FKCityId=A.FKCityId,
        //                                       ZipCode=A.ZipCode,
        //                                       FirstPreferedDate=A.FirstPreferedDate,
        //                                       FirstTime=A.FirstTime,
        //                                       SecondPreferedDate=A.SecondPreferedDate,
        //                                       SecondTime=A.SecondTime,
        //                                       ThirdPreferedDate=A.ThirdPreferedDate,
        //                                       ThirdTime=A.ThirdTime,
        //                                       MessageBody=A.MessageBody,
        //                                       MayContactByEmail=A.MayContactByEmail

        //                                   }).ToList();
        //    return list;
        //}

        #endregion

        #region Send a mail to the user after appointment booking

        public string MailToUser(AppointmentModel BookingRecord)
        {
            if (BookingRecord != null)
            {
                string Name = BookingRecord.FirstName;
                string MailBody = "";

                StringBuilder sb = new StringBuilder();
                string Message = "Thanks " + Name + ",<br/><br/>" + "Your Appointment has been booked successfully.Please see the following appointment deatails:";

                sb.Append("<body style='padding: 10px 0px 0px 10px; background: #f8f8fb; font-family:Times New Roman; font-size:14px;'>");
                sb.Append("Dear " + Name + ",");
                sb.Append("  <p style='font-size:15px;'>Greetings from CMS....!</p>");
                sb.Append("<div style='background: #e6e1e1; width:100%;'>" + Message + "</div><br/>");
                sb.Append("<div style='background: #e6e1e1; width:100%;'><table style='width:100%;background: #e6e1e1;border:none;padding: 0px 0px 0px 10px;'>" +
                         "<tr>" +
                                  "<td style='border:none;'><b>Project Name</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;'>" + BookingRecord.ProjectName + "</td>" +
                          "<tr/>" +
                          "<tr>" +
                                  "<td style='border:none;'><b>Utility Type</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;'>" + BookingRecord.UtilityType + "</td>" +
                          "<tr/>" +
                       

                          "<tr>" +
                                  "<td style='border:none;'><b>Account Number</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;'>" + BookingRecord.AccountNumber + "</td>" +
                           "<tr/>" +

                           "<tr>" +
                                  "<td style='border:none;'><b>Customer Name</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;' colspan='3'>" + BookingRecord.FirstName + "</td>" +
                           "<tr/>" +

                           "<tr>" +
                                  "<td style='border:none;'><b>Customer Email</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;' colspan='3'>" + BookingRecord.EmailAddress + "</td>" +
                           "<tr/>" +

                           "<tr>" +
                                  "<td style='border:none;'><b>Customer Phone</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;' colspan='3'>" + BookingRecord.DayTimePhone + "</td>" +
                           "<tr/>" +


                          "<tr>" +
                                  "<td style='border:none;'><b>Appointment Date</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;' colspan='3'>" + BookingRecord.DateForApp + "</td>" +
                           "<tr/>" +

                            "<tr>" +
                                  "<td style='border:none;'><b>Appointment Time</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;' colspan='3'>" + BookingRecord.FirstTime + "</td>" +
                           "<tr/>" +                          

                            "<tr>" +
                                  "<td style='border:none;'><b>Street</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;' colspan='3'>" + BookingRecord.Address + "</td>" +
                           "<tr/>" +
                             "<tr>" +
                                  "<td style='border:none;'><b>City</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;'>" + BookingRecord.City + "</td>" +
                          "<tr/>" +
                              "<tr>" +
                                  "<td style='border:none;'><b>State</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;'>" + BookingRecord.State + "</td>" +
                          "<tr/>" +
                        
                         "</table></div>");
                sb.Append("<br/><div></div>");

                sb.Append("Thank You,<br/>Data Depot Team.<br>" +
                                               "<p style='text-align:center;'> <a href='https://www.datadepotonline.com/' title='' rel='home'>" +
                                               "<img src='https://www.datadepotonline.com/images/logo_03_2.png' title='Data Depot' alt='' style='height:100px; width:200px;'>" +
                                               "</a> </p>" +
                                               "<p style='text-align:center; font-size:13px;'>You are receiving this notification because you are registered with Data Depot <br/>" +
                                               " ©" + DateTime.Now.ToString("yyyy") + " Data Depot</p> <br>");
                sb.Append("<p style='text-align:center; color:red; font-size:15px;'>*** This Email is system generated. Please do not reply to this email ID." +
                            "For any queries please contact Data Depot Team. ***</p>");
                sb.Append("<p style='text-align:center; color:red; font-size:13px;'>This message (including any attachments) contains confidential information" +
                            " intended for a specific individual and purpose and is protected by law." +
                            " If you are not the intended recipient, you should delete this message." +
                            " Any disclosure, copying or distribution of this message, or the taking of any action based on it, is strictly prohibited.</p>");
                sb.Append("</body>");


                MailBody = sb.ToString();



                List<String> mailto = new List<String>();
                mailto.Add(BookingRecord.EmailAddress);

                string Subject = "Appointment Booking Confirmation";

                List<String> CCEmailto = new List<String>();

                new GarronT.CMS.Model.clsMail().SendMail(mailto, CCEmailto, MailBody, Subject);

                return "Mail Sent Successfully. ";
            }

            return "Record Not Presents.Error in sending  mail";
        }

        #endregion

        #region Sent email to Administrator  if project profile was not created
        public string MailToAdmin(AppointmentModel BookingRecord )
        {
            if (BookingRecord != null)
            {
                //Get Appointment id
                CompassEntities objCompass = new CompassEntities();

                //var list = objCompass.Appointments.Where(o => o.AccountNumber == BookingRecord.AccountNumber && o.PassedDateFlag == 0 && o.Active == 1 && o.FKProjectId == BookingRecord.FKProjectId).FirstOrDefault();
                //BookingRecord.AppointmentID = list.AppointmentID;
                //string Name = BookingRecord.FirstName +" "+ BookingRecord.LastName;
                string MailBody = "";

                StringBuilder sb = new StringBuilder();
                string Message =GetMessage();              
                string Subject =GetSubject();   
                sb.Append("<body style='padding: 10px 0px 0px 10px; background: #f8f8fb; font-family:Times New Roman; font-size:14px;'>");
                //sb.Append("<div style='background: #e6e1e1; width:100%;'>Dear Admin,<br/><br/>");                
                //sb.Append("<p style='font-size:15px;'>Greetings from CMS....!</p>");
                sb.Append("<div style='background: #e6e1e1; width:100%;'>" + Message + "</div><br/>");
                sb.Append("<div style='background: #e6e1e1; width:100%;'><table style='width:100%;background: #e6e1e1;border:none;padding: 0px 0px 0px 10px;'>" +
                          
                           "<tr>" +
                                  "<td style='border:none;'><b>Project ID</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;'>" + BookingRecord.ProjectId + "</td>" +
                          "<tr/>" +
                           "<tr>" +
                                  "<td style='border:none;'><b>Project Name</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;'>" + BookingRecord.ProjectName + "</td>" +
                          "<tr/>" +
                          "<tr>" +
                                  "<td style='border:none;'><b>Utility Type</b></td>" +
                                  "<td style='border:none;'><b>:</b></td>" +
                                  "<td style='border:none;'>" + BookingRecord.UtilityType + "</td>" +
                          "<tr/>" +                                                                    
                            
                        
                         "</table></div>");
                sb.Append("<br/><div></div>");

                sb.Append("Thank You,<br/>Data Depot Team.<br>" +
                                               "<p style='text-align:center;'> <a href='https://www.datadepotonline.com/' title='' rel='home'>" +
                                               "<img src='https://www.datadepotonline.com/images/logo_03_2.png' title='Data Depot' alt='' style='height:100px; width:200px;'>" +
                                               "</a> </p>" +
                                               "<p style='text-align:center; font-size:13px;'>You are receiving this notification because you are registered with Data Depot <br/>" +
                                               " ©" + DateTime.Now.ToString("yyyy") + " Data Depot</p> <br>");
                sb.Append("<p style='text-align:center; color:red; font-size:15px;'>*** This Email is system generated. Please do not reply to this email ID." +
                            "For any queries please contact Data Depot Team. ***</p>");
                sb.Append("<p style='text-align:center; color:red; font-size:13px;'>This message (including any attachments) contains confidential information" +
                            " intended for a specific individual and purpose and is protected by law." +
                            " If you are not the intended recipient, you should delete this message." +
                            " Any disclosure, copying or distribution of this message, or the taking of any action based on it, is strictly prohibited.</p>");
                sb.Append("</body>");


                MailBody = sb.ToString();

                string adminEmail = WebConfigurationManager.AppSettings["WebMasterMailId"];

                List<String> mailto = new List<String>();
                mailto.Add(adminEmail);               

                List<String> CCEmailto = new List<String>();
                CCEmailto = CCEmailTo(); //null;
               new GarronT.CMS.Model.clsMail().SendMail(mailto, CCEmailto, MailBody, Subject);

                return "Mail Sent Successfully. ";
            }

            return "Sending mail Failed";
        }


        #endregion
        //Created by sominath for sending email to admin on Date 17/12/2016
        #region Get Subject
        public string GetSubject()
        {
            string subject = "";
            CompassEntities dbcontext = new CompassEntities();
            var ObjTblEmailConfigure = (from b in dbcontext.TblAlertMasters
                                        join c in dbcontext.TblEmailConfigures on b.AlertId equals c.AlertTableID
                                        where b.AlertId == 8
                                        select new EmailConfigureBO
                                        {
                                            Subject = c.Subject
                                        }).FirstOrDefault();

            if (ObjTblEmailConfigure != null)
            {
                subject = ObjTblEmailConfigure.Subject;
            }
            else
            {
                subject = null;
            }
            return subject;
        }
        #endregion
        //Created by sominath for sending email to admin on Date 17/12/2016
        #region Get Message 
        public string GetMessage()
        {
            string Message = "";
            CompassEntities dbcontext = new CompassEntities();
            var ObjTblEmailConfigure = (from b in dbcontext.TblAlertMasters
                                        join c in dbcontext.TblEmailConfigures on b.AlertId equals c.AlertTableID
                                        where b.AlertId == 8
                                        select new EmailConfigureBO
                                        {
                                            Message = c.Message
                                        }).FirstOrDefault();

            if (ObjTblEmailConfigure != null)
            {
                Message = ObjTblEmailConfigure.Message;
            }
            else
            {
                Message = null;
            }
            return Message;
        }
        #endregion
        //Created by sominath for sending email to admin on Date 17/12/2016
        #region Get CC  Email List
        public List<String> CCEmailTo()
        {
            List<String> List = new List<String>();
            CompassEntities db = new CompassEntities();
            var CCEmailId = (from b in db.TblAlertMasters
                             join c in db.TblEmailConfigures on b.AlertId equals c.AlertTableID
                             join d in db.TblEmailConfigureUserDetails on c.ID equals d.TblEmailConfigureID
                             where b.AlertId == 8
                             //select new TblEmailConfigureUserDetail
                             //{
                             //    UserID = d.UserID
                             //}
                             select d.UserID
                             )
                             .ToList();
            if (CCEmailId.Count > 0)
            {
                foreach (var item in CCEmailId)
                {
                    var UserEmailId = db.tblUsers.Where(n => n.UserID == item).FirstOrDefault();
                    if (UserEmailId != null)
                    {
                        List.Add(UserEmailId.Email);
                    }
                }
            }
            return List;
        }

        #endregion

        #region Get Project Name
        public string GetNameByID(long ProjectId)
        {
            string ProjName = "";
            ProjectModel _newObj = new ProjectModel();
            using (CompassEntities _objCompass = new CompassEntities())
            {
                _newObj = _objCompass.STP_GetProjectCityState().Where(a => a.ProjectId == ProjectId && a.ProjectStatus == "Active" && a.Active == 1).Select(o =>
                                                  new ProjectModel
                                                  {
                                                      ProjectId = o.ProjectId,
                                                      ProjectCityState = o.ProjectCityName
                                                  }).FirstOrDefault();
                ProjName = _newObj.ProjectCityState;
            }

            return ProjName;

        }

        #endregion

        #region Get Dynnamic List By ProjectID and AccountNumber
        public List<CurrentVisitRecordRow> GetDynamicRow(long ptojrctID, long UploadID,string account )
        {

            using (CompassEntities dbcontext = new CompassEntities())
            {

                List<CurrentVisitRecordRow> objCurrentVisitList = new List<CurrentVisitRecordRow>();
                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "PROC_Appointment_DynamicRow_ByAccountNo";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.Add("@ProjectId", ptojrctID);
                cmd.Parameters.Add("@UploadID", UploadID);
                cmd.Parameters.Add("@AccountNo", account);

                sqlConnection1.Open();

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    objCurrentVisitList.Add(new CurrentVisitRecordRow
                    {
                        ID = long.Parse(reader["Id"].ToString()),                       
                        Account = Convert.ToString(reader["Account"]),
                        Customer = Convert.ToString(reader["Customer"]),
                        City = Convert.ToString(reader["City"]),  //Changed on 28/12/2016
                        State = Convert.ToString(reader["State"]),                     
                        Street = Convert.ToString(reader["Street"])
                    });

                }

                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();

                return objCurrentVisitList;
            }
        }
        #endregion

        #region Get Dynnamic List By ProjectID 
        public List<CurrentVisitRecordRow> GetProjectDetails(long ptojrctID, string account)
        {

            using (CompassEntities dbcontext = new CompassEntities())
            {

                List<CurrentVisitRecordRow> objCurrentVisitList = new List<CurrentVisitRecordRow>();
                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "PROC_DynamicRow_ByProject";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.Add("@ProjectId", ptojrctID);


                sqlConnection1.Open();

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    objCurrentVisitList.Add(new CurrentVisitRecordRow
                    {
                        ID = long.Parse(reader["Id"].ToString()),
                        // OldMeterNo = Convert.ToString(reader["OldMeterNo"]),
                        // OldMeterRadioNo = Convert.ToString(reader["OldMeterRadioNo"]),
                        Account = Convert.ToString(reader["Account"]),
                        Customer = Convert.ToString(reader["Customer"]),
                        //FromDate = DateTime.Parse(reader["FromDate"].ToString()),
                        // ToDate = DateTime.Parse(reader["ToDate"].ToString()),
                        Cycle = Convert.ToString(reader["Cycle"]),
                        Route = Convert.ToString(reader["Route"]),
                        Street = Convert.ToString(reader["Street"])
                    });

                }

                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();

                return objCurrentVisitList.Where(o => o.Account == account).ToList();
            }
        }
        #endregion

        #region Check the record status is wrok completed  
        public bool CheckRecordStatus_WorkCompleted(long UplodedID)
        {
            CompassEntities objCompass = new CompassEntities();
            bool result = false;

            try
            {
                var _result = objCompass.PROC_Appointment_CheckRecordStatus_WorkCompleted((long?)UplodedID).FirstOrDefault();               
                if(_result==null)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }


            return result;


        }
        #endregion

        #region check appointment releted to the account number aleready presents  and it is not passed
        public int CheckAccount(string account,long  ProjectId, long UploadID)
        {
            int resultCount = 0;
            try
            {
                using (CompassEntities _objCompass = new CompassEntities())
                {
                    resultCount = _objCompass.Appointments.Where(o => o.AccountNumber == account && o.FKProjectId == ProjectId && o.PassedDateFlag == 0).Count();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return resultCount;
        }
        #endregion


        #region check appointment is paased by Uploaded id and account number
        public bool CkeckAppIsPassed(string UploadId,string Account)
        {
            bool success = false;
            DateTime dt = new CommonFunctions().ServerDate();
            using (CompassEntities _objCompass = new CompassEntities())
            {
                long uploadId = Convert.ToInt64(UploadId);
                var result = _objCompass.Appointments.Where(o => o.FK_UploadedId == uploadId && o.AccountNumber == Account && o.PassedDateFlag == 1 && o.Active == 1).FirstOrDefault();
                
                //make appointment Deactve 


                if(result!=null)
                {
                    long appointmentid = result.AppointmentID;
                    result.Active = 0;
                    result.ModifiedOn = dt;
                    _objCompass.SaveChanges();
                    success = true;
                }
                else
                {
                    success = false;
                }
                return success;
            }
        }

        #endregion



    }
}
