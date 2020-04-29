using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using KSPL.Indus.MSTS.UI.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class AppointmentController : Controller
    {
        // GET: Admin/Appointment

        public ActionResult Index()
        {
            return View();
        }


        #region Returning from  You Can Book Me Website URL  after booking an appointment successfully to display the appointment details to the user
        public ActionResult Success()
        {

            AppointmentDAL appDAL = new AppointmentDAL();
            //BookingSuccessModel BookingRecord = new BookingSuccessModel();
            List<CurrentVisitRecordRow> Row = null;
            AppointmentModel appModel = new AppointmentModel();
            //Getting Reuqested URL
            string _strResult = "";
            string _result = "";
            bool result = false;
            //long ProjectId = 0;
            //long UploadID = 0;
            string Urlstr = System.Web.HttpContext.Current.Request.Url.ToString();


            try
            {
                //step1> Getting Url parameter values.
                // string Name = HttpContext.Request.Params.Get("Name");
                string UploadId = HttpContext.Request.Params.Get("AID");
                string FirstName = HttpContext.Request.Params.Get("FName");
                string LastName = HttpContext.Request.Params.Get("LName");
                string Date = HttpContext.Request.Params.Get("Date");
                string Account = HttpContext.Request.Params.Get("Account");
                string Time = HttpContext.Request.Params.Get("Time");
                string Email = HttpContext.Request.Params.Get("Email");
                string Phone = HttpContext.Request.Params.Get("Phone");
                string Street = HttpContext.Request.Params.Get("Street");
                string City = HttpContext.Request.Params.Get("City");
                string Sate = HttpContext.Request.Params.Get("State");
                appModel.City = City;
                appModel.State = Sate;


                UploadId = UploadId.Replace("A", "");

                //Date time formate 
                string dateString = Date.ToString();
                DateTime l_dateTime = DateTime.Parse(dateString);
                string da = l_dateTime.ToString("MMM-dd-yyyy");
                string NewDateFormate = l_dateTime.ToString("MMM/dd/yyyy");
                DateTime date3 = DateTime.ParseExact(NewDateFormate, "MMM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                ////check Appointment is passed or not by the uploaded id and account number , Passed Date Flag==1
                ////if yes then Deactivate this appointment and insert new appointment.
                var IsExist = appDAL.CkeckAppIsPassed(UploadId, Account);

                if (IsExist == true)
                {
                    var getList = appDAL.checkUploadIdPresent(UploadId, Account);
                    if (getList != null)
                    {

                        appModel.FKProjectId = getList.FKProjectId;
                        appModel.FK_UploadedId = getList.Id;


                        Row = appDAL.GetProjectDetails(getList.FKProjectId, Account).ToList();
                        var PrjRoutCyl = Row.Where(o => o.Account == Account).FirstOrDefault(); //for Route and cycle
                        appModel.Route = PrjRoutCyl.Route;
                        appModel.Cycle = PrjRoutCyl.Cycle;


                        var getBookInfo = appDAL.GetProjectDetails(getList.FKProjectId);
                        if (getBookInfo != null)
                        {

                            string ProjectName = appDAL.GetNameByID(getList.FKProjectId);
                            appModel.ProjectName = ProjectName;
                            appModel.UtilityType = getBookInfo.Utilitytype;


                            appModel.AccountNumber = Account;
                            appModel.FirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(FirstName);
                            appModel.LastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(LastName);
                            appModel.FullName = FirstName + " " + LastName;
                            appModel.DayTimePhone = Phone;
                            appModel.EmailAddress = Email;
                            appModel.FirstPreferedDate = date3;
                            appModel.DateForApp = da;
                            appModel.FirstTime = Time;
                            appModel.Address = Street;

                            result = appDAL.InsertRecord(appModel, out _result);


                        }



                    }

                }
                else if (IsExist == false)
                {
                    var getList = appDAL.checkUploadIdPresent(UploadId, Account);
                    if (getList != null)
                    {

                        appModel.FKProjectId = getList.FKProjectId;
                        appModel.FK_UploadedId = getList.Id;


                        Row = appDAL.GetProjectDetails(getList.FKProjectId, Account).ToList();
                        var PrjRoutCyl = Row.Where(o => o.Account == Account).FirstOrDefault(); //for Route and cycle
                        appModel.Route = PrjRoutCyl.Route;
                        appModel.Cycle = PrjRoutCyl.Cycle;


                        var getBookInfo = appDAL.GetProjectDetails(getList.FKProjectId);
                        if (getBookInfo != null)
                        {

                            string ProjectName = appDAL.GetNameByID(getList.FKProjectId);
                            appModel.ProjectName = ProjectName;
                            appModel.UtilityType = getBookInfo.Utilitytype;


                            appModel.AccountNumber = Account;
                            appModel.FirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(FirstName);
                            appModel.LastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(LastName);
                            appModel.FullName = FirstName + " " + LastName;
                            appModel.DayTimePhone = Phone;
                            appModel.EmailAddress = Email;
                            appModel.FirstPreferedDate = date3;
                            appModel.DateForApp = da;
                            appModel.FirstTime = Time;
                            appModel.Address = Street;

                            result = appDAL.InsertRecord(appModel, out _result);


                        }



                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "sorry we are unable to proccess your request, Please try again.";
                }



                //var getList = appDAL.checkUploadIdPresent(UploadId, Account);
                //if (getList != null)
                //{

                //    appModel.FKProjectId = getList.FKProjectId;
                //    appModel.FK_UploadedId = getList.Id;


                //    Row = appDAL.GetProjectDetails(getList.FKProjectId, Account).ToList();
                //    var PrjRoutCyl = Row.Where(o => o.Account == Account).FirstOrDefault(); //for Route and cycle
                //    appModel.Route = PrjRoutCyl.Route;
                //    appModel.Cycle = PrjRoutCyl.Cycle;


                //    var getBookInfo = appDAL.GetProjectDetails(getList.FKProjectId);
                //    if (getBookInfo != null)
                //    {

                //        string ProjectName = appDAL.GetNameByID(getList.FKProjectId);
                //        appModel.ProjectName = ProjectName;
                //        appModel.UtilityType = getBookInfo.Utilitytype;


                //        appModel.AccountNumber = Account;
                //        appModel.FirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(FirstName);
                //        appModel.LastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(LastName);
                //        appModel.FullName = FirstName + " " + LastName;
                //        appModel.DayTimePhone = Phone;
                //        appModel.EmailAddress = Email;
                //        appModel.FirstPreferedDate = date3;
                //        appModel.DateForApp = da;
                //        appModel.FirstTime = Time;
                //        appModel.Address = Street;

                //        //result = appDAL.InsertRecord(appModel, out _result);


                //    }



                //}

            }
            catch (Exception ex)
            {
                _strResult = ex.Message;
                //throw ex;
            }

            return View(appModel);
        }
        #endregion


        [HttpGet]
        public ActionResult GetUserDetails(long account)
        {
            return View();
        }

        #region New Appointment Module  Date 19/12/2016
        [HttpPost]
        public ActionResult GetUserDetails(string account)
        {
            AppointmentDAL ApptDAL = new AppointmentDAL();
            CurrentVisitRecordRow Row = null;
            List<Appointment> _NewList = new List<Appointment>();
            AppointmentModel appModelObj = new AppointmentModel();

            string strUrl = "";
            string returnMsg = "";
            bool success = false;

            try
            {

                string AccountNumber = account;


                var CurrentRecordBO = ApptDAL.GetProjectIDUploadID(AccountNumber);
                if (CurrentRecordBO != null)
                {


                    appModelObj.ProjectId = CurrentRecordBO.FKProjectId;
                    appModelObj.AccountNumber = AccountNumber;

                    var result = ApptDAL.CheckRecordStatus_WorkCompleted(CurrentRecordBO.Id);

                    if (result)
                    {
                        success = false;
                        returnMsg = "Sorry ! You can not book appointment. Service to this account already completed.";

                    }
                    else
                    {
                        AppointmentAllocationDAL OBJAppointmentAllocationDAL = new AppointmentAllocationDAL();
                        int output = OBJAppointmentAllocationDAL.UpdatePassedDateFlag();

                        int resultCount = ApptDAL.CheckAccount(account, CurrentRecordBO.FKProjectId, CurrentRecordBO.Id);

                        if (resultCount > 0)
                        {
                            success = false;
                            returnMsg = "Sorry ! You can not book appointment.Your appointment has been already booked";
                        }
                        else
                        {

                            var ProjectDetails = ApptDAL.GetProjectDetails(CurrentRecordBO.FKProjectId);

                            if (ProjectDetails != null)
                            {

                                var DynamicList = ApptDAL.GetDynamicRow(ProjectDetails.ProjectId, CurrentRecordBO.Id, AccountNumber);
                                Row = DynamicList.Where(o => o.Account == Convert.ToString(AccountNumber)).FirstOrDefault();



                                string AppointmentUrlStr = ProjectDetails.AppointmentUrl;  //Get Project Url for redirect to project profile on YCBM site

                                if (AppointmentUrlStr != string.Empty && AppointmentUrlStr != null)
                                {
                                    //strUrl = "https://compassmeterservices.youcanbook.me?ACCOUNT=" + AccountNumber + "&STREET=" + Street + "";
                                    strUrl = AppointmentUrlStr + "?AID=A" + Row.ID + "A&ACCOUNT=" + Row.Account + "&STREET=" + Row.Street + "&CITY=" + Row.City + "&STATE=" + Row.State + "";
                                    strUrl = strUrl.Replace(" ", "%20");
                                    success = true;
                                }
                                else
                                {
                                    //if project url is not present then send email to the Admin-(so and so project url is not created )
                                    appModelObj.ProjectId = CurrentRecordBO.FKProjectId;
                                    appModelObj.ProjectName = ProjectDetails.ProjectName;
                                    appModelObj.UtilityType = ProjectDetails.Utilitytype;
                                    ApptDAL.MailToAdmin(appModelObj);
                                    success = false;
                                    returnMsg = "Sorry ! We are unable to process your request. Appointment booking was not available for this account right now.";
                                }


                            }
                            else
                            {
                                success = false;
                                returnMsg = "Invalid Request";

                            }

                        }

                    }

                }
                else
                {
                    success = false;
                    returnMsg = "Sorry! we are unable to proccess your request.Please try again.";
                }



            }
            catch (Exception ex)
            {
                returnMsg = ex.Message;
                returnMsg = "Sorry! we are unable to proccess your request.Please try again.";
            }
            return Json(new { msg = success, returnMessage = returnMsg, redirectToUrl = strUrl }, JsonRequestBehavior.AllowGet);
        }

        #endregion


    }
}