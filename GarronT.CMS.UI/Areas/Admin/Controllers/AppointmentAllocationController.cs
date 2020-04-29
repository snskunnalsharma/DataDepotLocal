using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using KSPL.Indus.MSTS.UI.Filters;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using GarronT.CMS.UI.CustomHelpers;


namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class AppointmentAllocationController : Controller
    {

        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(AppointmentAllocationController));
        #endregion


        // GET: Admin/AppointmentAllocation
        public ActionResult Index()
        {
            return View();
        }
        #region Get Project List
        [HttpGet]
        public JsonResult GetPoject(string utility)
        {
            bool success = false;
            string returnMessage = "";

            var checkAdmin = Session["CurrentUserIsAdmin"].ToString();

            List<ProjectModel> projectList = new List<ProjectModel>();
            try
            {
                //Get ProjectList by Project Status
                projectList = new AppointmentAllocationDAL().GetProjectName(utility);
                success = true;

                if (checkAdmin == "0")
                {
                    var userId = new UserDetails().GetUserId(User.Identity.GetUserId());
                    var abcd = new ProjectMasterDAL().GetUserProjectListForDashBoard(utility, "Active", userId);


                    if (abcd == null || abcd.Count == 0)
                    {
                        if (projectList != null && projectList.Count() > 0)
                        {
                            projectList = new List<ProjectModel>();
                        }
                    }
                    else
                    {
                        projectList = projectList.Where(o => abcd.Contains(o.ProjectId)).ToList();
                    }

                }
            }
            catch (Exception ex)
            {
                returnMessage = "Sorry ! We are unable to process your request. Please try again.";
                log.Error(ex.Message);
            }
            return base.Json(new { projectList, success, returnMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Appointment List
        //GET: Appointment Details
        [HttpGet]
        public ActionResult GetAppointmentList(long projectID, int appointmentFlag)
        {
            bool success = false;
            string returnMessage = "";
            //List<CurrentVisitRecordRow> Row = null;
            List<AppointmentModel> AppModelVM = new List<AppointmentModel>();
            AppointmentAllocationDAL ObjAppointmentAllocationDAL = new AppointmentAllocationDAL();
            try
            {

                //Call Sp to update PassedDate Flag to update old appointment                
                int output = ObjAppointmentAllocationDAL.UpdatePassedDateFlag();

                if (appointmentFlag == 2)    //here appointmentFlag==2 means Passed Appointments
                {
                    AppModelVM = ObjAppointmentAllocationDAL.GetAppointments(projectID).Where(o => o.PassedDateFlag == 1).ToList();
                    success = true;
                }
                else
                {
                    //here appointmentFlag==0 means Upcoming Appointments
                    AppModelVM = ObjAppointmentAllocationDAL.GetAppointments(projectID).Where(o => o.PassedDateFlag == 0).ToList();
                    success = true;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                success = false;
                returnMessage = "We are unable to process your request. Please try again.";
            }
            return Json(new { AppModelVM, success, returnMessage, PassedFlag = appointmentFlag }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region View upcoming  Appointment on popup with installer  list New code according to Bharat sir coding standards
        //GET: Installer by project id
        public ActionResult GetInstaller(long appointmentid)
        {


            AppointmentModel AppModelVM = new AppointmentModel();
            CurrentVisitRecordRow Row = null;
            bool latlongPresent = true;
            List<UsersSmallModel> InstallerList = new List<UsersSmallModel>();
            long InstallerID = 0;
            if (appointmentid > 0)
            {
                using (AppointmentAllocationDAL obj = new AppointmentAllocationDAL())
                {
                    int output = obj.UpdatePassedDateFlag();

                    AppModelVM = obj.ViewAppointment(appointmentid, out latlongPresent);  // New added by sominath         

                    // AppModelVM = obj.ViewAppointment(projectId, account, appointmentid).ToList();

                    //Row = obj.GetProjectDetails(long.Parse(AppModelVM.FKProjectId.ToString()), AppModelVM.FK_UploadedId);// New added by sominath
                    //Row = obj.GetProjectDetails(projectId, account).ToList();
                    InstallerList = obj.GeInstallerListCurrentProject(long.Parse(AppModelVM.FKProjectId.ToString())).OrderBy(a => a.UserName).ToList();
                    //InstallerID = AppModelVM.InstallerID;

                }
            }

            return Json(new { InstallerList = InstallerList, details = AppModelVM, latlongPresent = latlongPresent }, JsonRequestBehavior.AllowGet);



        }
        #endregion


        #region View passed appointment on popup
        [HttpGet]
        public ActionResult ViewPassedAppointment(long appointmentid)
        {

            var objPermissionCheck = UserPermissionCheck.checkUserStatus("AppointmentAllocation", "View");

            if (objPermissionCheck)
            {
                AppointmentModel AppModelVM = new AppointmentModel();
                bool latlongPresent = true;
                try
                {
                    AppointmentAllocationDAL obj = new AppointmentAllocationDAL();

                    //Call Sp to update PassedDate Flag to update old appointment                
                    int output = obj.UpdatePassedDateFlag();

                    AppModelVM = obj.ViewAppointment(appointmentid, out latlongPresent);

                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }

                return Json(new { details = AppModelVM }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region Insert Installer allocated appointment into InstallerMapData table and send emai to Customer and installer
        [HttpPost]
        public ActionResult InserAppointmentWithInstaller(AppointmentModel _AppModelVM)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("AppointmentAllocation", "Create");

            if (objPermissionCheck)
            {
                bool success = false;
                string _OutResult = "";
                AppointmentAllocationDAL AppointmentDAL = new AppointmentAllocationDAL();
                try
                {
                    if (_AppModelVM != null)
                    {
                        success = AppointmentDAL.InsertRecord(_AppModelVM, out _OutResult);
                        // sending email to the Installer when allocated to the appointment
                        //sending email to the Customer when installer allocated
                        if (success)
                        {
                            AppointmentDAL.MailToInstaller(_AppModelVM);

                            AppointmentDAL.MailToCustomer(_AppModelVM);

                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    success = false;
                    _OutResult = "We are unable to process your request. Please try again.";

                }
                return base.Json(new { msg = success, returnMsg = _OutResult }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { msg = objPermissionCheck, returnMsg = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Create You Can Book me site url for Reschedule  Appointment
        [HttpGet]
        public ActionResult RescheduleAppointment(long projectId, string account, long appointmentid)
        {
            bool success = false;
            string strUrl = "";
            string returnMsg = "You can not reschedule your appointment.";
            AppointmentAllocationDAL _objAppointmentAllocationDAL = new AppointmentAllocationDAL();
            try
            {
                var getValues = _objAppointmentAllocationDAL.GetAppointmentDetails(projectId, account, appointmentid);
                if (getValues != null)
                {
                    var ProjectDetails = _objAppointmentAllocationDAL.GetProjectDetailInfo(projectId);// Get Project Url by project id
                    string AppointmentUrlStr = ProjectDetails.AppointmentUrl;
                    //create url
                    string phone = getValues.DayTimePhone;
                    //phone = phone.Replace("+", "%2B");
                    //phone = phone.Replace("(", "+%28");
                    //phone = phone.Replace(")", "%29+");
                    //phone = phone.Replace(" ", "");


                    strUrl = AppointmentUrlStr + "?AID=A" + getValues.FK_UploadedId + "A&ACCOUNT=" + getValues.AccountNumber + "&FNAME=" + getValues.FirstName + "&LNAME=" + getValues.LastName + "&EMAIL=" + getValues.EmailAddress + "&PHONE=" + phone + "&STREET=" + getValues.Address + "&CITY=" + getValues.City + "&STATE=" + getValues.State + "";
                    strUrl = strUrl.Replace(" ", "%20");
                    //strUrl = AppointmentUrlStr + "?AID=A" + getValues.FK_UploadedId + "A&ACCOUNT=" + getValues.AccountNumber + "&FNAME=" + getValues.FirstName + "&LNAME=" + getValues.LastName + "&EMAIL=" + getValues.EmailAddress + "&PHONE=" + phone + "&STREET=" + getValues.Address + "&CITY=" + getValues.City + "&STATE=" + getValues.State + "";
                    success = true;
                }
                else
                {
                    returnMsg = "Your appointment has been already booked.";
                    success = false;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Json(new { msg = success, returnMessage = returnMsg, redirectToUrl = strUrl }, JsonRequestBehavior.AllowGet);
        }

        #endregion


    }
}