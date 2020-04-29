using AutoMapper;
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
    public class AppointmentAllocationDAL : IDisposable
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

        public AppointmentAllocationDAL()
        {

        }

        #region Get Project list by utility type
        public List<ProjectModel> GetProjectName(string UtilityType)
        {
            CompassEntities CompassEntities = new CompassEntities();
            List<ProjectModel> projectList = new List<ProjectModel>();
            try
            {
                projectList = CompassEntities.STP_GetProjectCityState().Where(a => a.Active == 1 && a.Utilitytype == UtilityType && a.ProjectStatus == "Active").Select(o =>
                                             new ProjectModel
                                             {
                                                 ProjectId = o.ProjectId,
                                                 ProjectCityState = o.ProjectCityName
                                             }).OrderBy(a => a.ProjectCityState).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return projectList;
        }
        #endregion

        #region Get and View Booked appointment list and display in the grid  table
        public List<AppointmentModel> GetAppointments(long ProjectID)
        {
            List<AppointmentModel> projectList = new List<AppointmentModel>();

            using (CompassEntities CompassEntities = new CompassEntities())
            {


                // projectList = CompassEntities.Appointments.Where(o => o.FKProjectId == ProjectID).ToList();

                projectList = CompassEntities.PROC_AppointmentAllocation_GetAllRecord(ProjectID).Where(o => o.FKProjectId == ProjectID).Select(o => new AppointmentModel
                {
                    AppointmentID = o.AppointmentID,
                    FKProjectId = o.FKProjectId,
                    AccountNumber = o.AccountNumber,
                    Address = o.Street,
                    DateOnTable = DateTime.Parse(Convert.ToString(o.FirstPreferedDate)).ToString("MMM-dd-yyyy"),
                    FirstTime = o.FirstTime,
                    Cycle = o.Cycle,
                    Route = o.Route,
                    ProjectId = (long)o.FKProjectId,
                    ProjectName = o.ProjectCityName,
                    UtilityType = o.UtilityType,
                    InstallerName = o.InsatllerName,
                    InstallerID = o.InstallerId,
                    PassedDateFlag = (int)o.PassedDateFlag,

                }).OrderBy(o => o.InstallerName).ThenBy(o => o.FirstPreferedDate).ToList();

                //Get Route and Cycle  by project id



                //List<AppointmentModel> _newList = new List<AppointmentModel>();

                //foreach (var item in projectList)
                //{
                //    AppointmentModel objAppModelVM = new AppointmentModel();
                //   // CompassEntities objCompassEntities = new CompassEntities();
                //    var getBookInfo = CompassEntities.tblProjects.Where(o => o.ProjectId == ProjectID && o.ProjectStatus == "Active").FirstOrDefault();

                //    objAppModelVM.UtilityType = getBookInfo.Utilitytype;                   
                //    objAppModelVM.ProjectName = CompassEntities.STP_GetProjectCityState().Where(a => a.ProjectId == ProjectID).Select(o => o.ProjectCityName).FirstOrDefault();
                //    objAppModelVM.FirstName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.FirstName) + " " + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.LastName);
                //    //objAppModelVM.LastName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.LastName);
                //   // objAppModelVM.EmailAddress = item.EmailAddress;
                //   // objAppModelVM.DayTimePhone = item.DayTimePhone;
                //    objAppModelVM.AppointmentID = item.AppointmentID;
                //    objAppModelVM.FKProjectId = item.FKProjectId;
                //    objAppModelVM.AccountNumber = item.AccountNumber;
                //    objAppModelVM.Address = item.Address;
                //   // string appDate = item.FirstPreferedDate.ToString();
                //   // DateTime l_dateTime = DateTime.Parse(appDate);
                //  //  string da = l_dateTime.ToString("MMM-dd-yyyy");

                //    objAppModelVM.DateOnTable = DateTime.Parse(Convert.ToString(item.FirstPreferedDate)).ToString("MMM-dd-yyyy"); //da;
                //    objAppModelVM.FirstTime = item.FirstTime;
                //    objAppModelVM.Cycle = item.Cycle;
                //    objAppModelVM.Route = item.Route;
                //    var Installer = CompassEntities.tblInstallerMapDatas.Where(o => o.FKAppointmentID == objAppModelVM.AppointmentID && o.AppointmentFlag == 1 && o.Active == 1).FirstOrDefault();
                //    if (Installer != null)
                //    {
                //        var Name = CompassEntities.tblUsers.Where(o => o.UserID == Installer.InstallerId && o.Active == 1).Select(o => o.FirstName).SingleOrDefault();
                //        objAppModelVM.InstallerName = Name;
                //    }
                //    else
                //    {
                //        objAppModelVM.InstallerName = "-";
                //    }

                //    objAppModelVM.PassedDateFlag = item.PassedDateFlag;

                //    _newList.Add(objAppModelVM);


                //}
                projectList = projectList.OrderBy(o => o.InstallerName).ThenBy(o => o.FirstPreferedDate).ToList();
                return projectList;

                //return projectList.OrderBy(o => o.InstallerName).ThenBy(o => o.FirstPreferedDate).ThenBy(o=>o.AccountNumber).ToList();

            }

            // projectList.Remove("");





        }
        #endregion

        #region View passed appointment   Modified according to Bharat sir coding standard
        public AppointmentModel ViewAppointment(long appointmentID, out bool latLongStatus)
        {
            AppointmentModel appointmentRecord = new AppointmentModel();

            using (CompassEntities CompassEntities = new CompassEntities())
            {


                appointmentRecord = CompassEntities.Appointments.Where(o => o.AppointmentID == appointmentID).Select(o => new AppointmentModel
                {
                    AppointmentID = o.AppointmentID,
                    FirstName = o.FirstName,
                    LastName = o.LastName,
                    EmailAddress = o.EmailAddress,
                    DayTimePhone = o.DayTimePhone,
                    FKProjectId = o.FKProjectId,
                    FK_UploadedId = (long)o.FK_UploadedId,
                    AccountNumber = o.AccountNumber,
                    Address = o.Address,
                    City = o.City,
                    State = o.State,
                    FirstPreferedDate = o.FirstPreferedDate,
                    FirstTime = o.FirstTime,
                    Cycle = o.Cycle,
                    Route = o.Route,
                    PassedDateFlag = (int)o.PassedDateFlag

                }).FirstOrDefault();

                //Get Route and Cycle  by project id
                var uploadRecord=CompassEntities.UploadedExcelDatas.AsNoTracking().Where(o=>o.Id==appointmentRecord.FK_UploadedId).FirstOrDefault();
                latLongStatus = (uploadRecord.AddressLatitude != null && uploadRecord.AddressLatitude.Length != 0) ? true : false;

                var ProjectInfo = CompassEntities.tblProjects.Where(o => o.ProjectId == appointmentRecord.FKProjectId && o.ProjectStatus == "Active").FirstOrDefault();

                appointmentRecord.UtilityType = ProjectInfo.Utilitytype;
                string ProjName = CompassEntities.STP_GetProjectCityState().Where(a => a.ProjectId == ProjectInfo.ProjectId).Select(o => o.ProjectCityName).FirstOrDefault();
                appointmentRecord.ProjectName = ProjName;
                appointmentRecord.DateOnTable = DateTime.Parse(Convert.ToString(appointmentRecord.FirstPreferedDate)).ToString("MMM-dd-yyyy");

                var Installer = CompassEntities.tblInstallerMapDatas.Where(o => o.FKAppointmentID == appointmentRecord.AppointmentID && o.AppointmentFlag == 1 && o.Active == 1).FirstOrDefault();
                if (Installer != null)
                {
                    appointmentRecord.InstallerID = Installer.InstallerId;
                    appointmentRecord.InstallerName = CompassEntities.tblUsers.Where(o => o.UserID == Installer.InstallerId).Select(o => o.FirstName).SingleOrDefault();
                    appointmentRecord.UserName = appointmentRecord.InstallerName;
                }
                else
                {
                    appointmentRecord.InstallerName = "-";
                }


            }

            return appointmentRecord;
        }



        #endregion




        #region View passed appointment
        public List<AppointmentModel> ViewAppointment(long ProjectID, string account, long appointmentID)
        {
            List<AppointmentModel> projectList = new List<AppointmentModel>();

            using (CompassEntities CompassEntities = new CompassEntities())
            {


                // projectList = CompassEntities.Appointments.Where(o => o.FKProjectId == ProjectID).ToList();

                projectList = CompassEntities.Appointments.Where(o => o.FKProjectId == ProjectID && o.AppointmentID == appointmentID).Select(o => new AppointmentModel
                {
                    AppointmentID = o.AppointmentID,
                    FirstName = o.FirstName,
                    LastName = o.LastName,
                    EmailAddress = o.EmailAddress,
                    DayTimePhone = o.DayTimePhone,
                    FKProjectId = o.FKProjectId,
                    AccountNumber = o.AccountNumber,
                    Address = o.Address,
                    FirstPreferedDate = o.FirstPreferedDate,
                    FirstTime = o.FirstTime,
                    Cycle = o.Cycle,
                    Route = o.Route,

                    PassedDateFlag = (int)o.PassedDateFlag,

                }).OrderBy(o => o.AccountNumber).ToList();

                //Get Route and Cycle  by project id



                List<AppointmentModel> _newList = new List<AppointmentModel>();

                foreach (var item in projectList)
                {
                    AppointmentModel objAppModelVM = new AppointmentModel();
                    CompassEntities objCompassEntities = new CompassEntities();
                    var getBookInfo = objCompassEntities.tblProjects.Where(o => o.ProjectId == ProjectID && o.ProjectStatus == "Active").FirstOrDefault();

                    objAppModelVM.UtilityType = getBookInfo.Utilitytype;
                    string ProjName = GetNameByID(ProjectID);
                    objAppModelVM.ProjectName = ProjName;
                    objAppModelVM.FirstName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.FirstName) + " " + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.LastName);
                    //objAppModelVM.LastName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.LastName);
                    objAppModelVM.EmailAddress = item.EmailAddress;
                    objAppModelVM.DayTimePhone = item.DayTimePhone;
                    objAppModelVM.AppointmentID = item.AppointmentID;
                    objAppModelVM.FKProjectId = item.FKProjectId;
                    objAppModelVM.AccountNumber = item.AccountNumber;
                    objAppModelVM.Address = item.Address;
                    string appDate = item.FirstPreferedDate.ToString();
                    DateTime l_dateTime = DateTime.Parse(appDate);
                    string da = l_dateTime.ToString("MMM-dd-yyyy");
                    objAppModelVM.DateOnTable = da;
                    objAppModelVM.FirstTime = item.FirstTime;
                    objAppModelVM.Cycle = item.Cycle;
                    objAppModelVM.Route = item.Route;
                    var Installer = objCompassEntities.tblInstallerMapDatas.Where(o => o.FKAppointmentID == objAppModelVM.AppointmentID && o.AppointmentFlag == 1 && o.Active == 1).FirstOrDefault();
                    if (Installer != null)
                    {
                        objAppModelVM.UserID = Installer.InstallerId;
                        var Name = objCompassEntities.tblUsers.Where(o => o.UserID == Installer.InstallerId && o.Active == 1).Select(o => o.FirstName).SingleOrDefault();
                        objAppModelVM.InstallerName = Name;
                        objAppModelVM.UserName = objAppModelVM.InstallerName;
                    }
                    else
                    {
                        objAppModelVM.InstallerName = "-";
                    }

                    objAppModelVM.PassedDateFlag = item.PassedDateFlag;

                    _newList.Add(objAppModelVM);


                }





                return _newList.OrderBy(o => o.FirstPreferedDate).ThenBy(o => o.AccountNumber).ToList();

            }
        }
        #endregion

        #region Get installer list by project
        public List<UsersSmallModel> GeInstallerListCurrentProject(long ProjectId)
        {
            List<UsersSmallModel> UserList = null;

            try
            {
                CompassEntities CompassEntities = new CompassEntities();




                UserList = (from b in CompassEntities.TblProjectInstallers
                            where b.Active == 1 && b.ProjectID == ProjectId
                            select new UsersSmallModel
                            {
                                UserId = b.tblUsers_UserID,
                                UserName = CompassEntities.tblUsers.Where(a => a.UserID == b.tblUsers_UserID).Select(m => m.FirstName).FirstOrDefault()
                            }).ToList();



                return UserList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Get Project Details by Project ID
        public tblProject GetProjectDetailInfo(long projectId)
        {
            tblProject objProj = new tblProject();
            using (CompassEntities objCompass = new CompassEntities())
            {
                objProj = objCompass.tblProjects.Where(o => o.ProjectId == projectId && o.ProjectStatus == "Active" && o.Active == 1).FirstOrDefault();
            }
            return objProj;
        }

        #endregion


        //#region Get Dynnamic List By ProjectID and uploaded id New  method according to bharat sir coding standard
        //public CurrentVisitRecordRow GetProjectDetails(long projectID, long uploadId)
        //{

        //    using (CompassEntities dbcontext = new CompassEntities())
        //    {

        //        List<CurrentVisitRecordRow> objCurrentVisitList = new List<CurrentVisitRecordRow>();
        //        SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
        //        SqlCommand cmd = new SqlCommand();
        //        SqlDataReader reader;

        //        cmd.CommandText = "PROC_DynamicRow_ByProject";
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Connection = sqlConnection1;
        //        cmd.Parameters.Add("@ProjectId", projectID);


        //        sqlConnection1.Open();

        //        reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            objCurrentVisitList.Add(new CurrentVisitRecordRow
        //            {
        //                ID = long.Parse(reader["Id"].ToString()),
        //                // OldMeterNo = Convert.ToString(reader["OldMeterNo"]),
        //                // OldMeterRadioNo = Convert.ToString(reader["OldMeterRadioNo"]),
        //                Account = Convert.ToString(reader["Account"]),
        //                Customer = Convert.ToString(reader["Customer"]),
        //                //FromDate = DateTime.Parse(reader["FromDate"].ToString()),
        //                // ToDate = DateTime.Parse(reader["ToDate"].ToString()),
        //                Cycle = Convert.ToString(reader["Cycle"]),
        //                Route = Convert.ToString(reader["Route"]),
        //                Street = Convert.ToString(reader["Street"])
        //            });

        //        }

        //        sqlConnection1.Close();
        //        reader.Close();
        //        reader.Dispose();
        //        cmd.Dispose();
        //        sqlConnection1.Dispose();

        //        return objCurrentVisitList.Where(o => o.ID == uploadId).FirstOrDefault();
        //    }
        //}
        //#endregion





        //#region Get Dynnamic List By ProjectID
        //public List<CurrentVisitRecordRow> GetProjectDetails(long ptojrctID, string account)
        //{

        //    using (CompassEntities dbcontext = new CompassEntities())
        //    {

        //        List<CurrentVisitRecordRow> objCurrentVisitList = new List<CurrentVisitRecordRow>();
        //        SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
        //        SqlCommand cmd = new SqlCommand();
        //        SqlDataReader reader;

        //        cmd.CommandText = "PROC_DynamicRow_ByProject";
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Connection = sqlConnection1;
        //        cmd.Parameters.Add("@ProjectId", ptojrctID);


        //        sqlConnection1.Open();

        //        reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            objCurrentVisitList.Add(new CurrentVisitRecordRow
        //            {
        //                ID = long.Parse(reader["Id"].ToString()),
        //                // OldMeterNo = Convert.ToString(reader["OldMeterNo"]),
        //                // OldMeterRadioNo = Convert.ToString(reader["OldMeterRadioNo"]),
        //                Account = Convert.ToString(reader["Account"]),
        //                Customer = Convert.ToString(reader["Customer"]),
        //                //FromDate = DateTime.Parse(reader["FromDate"].ToString()),
        //                // ToDate = DateTime.Parse(reader["ToDate"].ToString()),
        //                Cycle = Convert.ToString(reader["Cycle"]),
        //                Route = Convert.ToString(reader["Route"]),
        //                Street = Convert.ToString(reader["Street"])
        //            });

        //        }

        //        sqlConnection1.Close();
        //        reader.Close();
        //        reader.Dispose();
        //        cmd.Dispose();
        //        sqlConnection1.Dispose();

        //        return objCurrentVisitList.Where(o => o.Account == account).ToList();
        //    }
        //}
        //#endregion

        #region Get project route and cycle
        public List<CurrentVisitRecordRow> GetProjectRouteCycle(long ptojrctID)
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

                return objCurrentVisitList.ToList();
            }


        }
        #endregion

        #region Insert Appointment with Installer in Installer Map Data table and in  history table
        public bool InsertRecord(AppointmentModel appVM, out string _result)
        {
            bool output = false;

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

            var list = objCompassEntities.Appointments.Where(o => o.AppointmentID == appVM.FK_AppointmentID && o.Active == 1).FirstOrDefault();
            long FK_ExcelUploadID = (long)list.FK_UploadedId;
            //check record allready presents in the Intaller Map Data
            //Presents then  update(Active=0 previously) and .   if not presents then Insert newely.
            var existinRecord = objCompassEntities.tblInstallerMapDatas.Where(o => o.FK_UploadedExcelData_Id == FK_ExcelUploadID && o.Active == 1).FirstOrDefault();

            if (existinRecord != null)
            {
                existinRecord.Active = 0;
                existinRecord.BatchNumber = null;
                existinRecord.MobileResponse = null;
                objCompassEntities.SaveChanges();
                _result = "Record Updated successfully.";
                output = true;
            }
            var uploadRecord = objCompassEntities.UploadedExcelDatas.Where(o => o.Id == FK_ExcelUploadID).FirstOrDefault();
            var projectDetails = objCompassEntities.tblProjects.Where(o => o.ProjectId == uploadRecord.FKProjectId).FirstOrDefault();
            var existinRecord1 = objCompassEntities.tblInstallerMapDatas.Where(o => o.FK_UploadedExcelData_Id == FK_ExcelUploadID && o.InstallerId == appVM.InstallerID).FirstOrDefault();

            if (existinRecord1 == null)
            {
                tblInstallerMapData oblInstallerDB = new tblInstallerMapData();
                oblInstallerDB.InstallerId = appVM.InstallerID;

                oblInstallerDB.FKAppointmentID = appVM.FK_AppointmentID;
                oblInstallerDB.FK_UploadedExcelData_Id = FK_ExcelUploadID;
                oblInstallerDB.AppointmentFlag = 1;

                oblInstallerDB.FromDate = Convert.ToDateTime(projectDetails.FromDate.ToString());
                oblInstallerDB.ToDate = Convert.ToDateTime(projectDetails.ToDate.ToString());
                oblInstallerDB.Active = 1;
                oblInstallerDB.CreatedBy = userId;
                oblInstallerDB.CreatedOn = dt;

                objCompassEntities.tblInstallerMapDatas.Add(oblInstallerDB);
                objCompassEntities.SaveChanges();

            }
            else
            {



                existinRecord1.FKAppointmentID = appVM.FK_AppointmentID;
                existinRecord1.AppointmentFlag = 1;

                existinRecord1.FromDate = Convert.ToDateTime(dt.Date.ToString());
                existinRecord1.ToDate = Convert.ToDateTime(list.FirstPreferedDate.ToString());
                existinRecord1.Active = 1;
                existinRecord1.ModifiedBy = userId;
                existinRecord1.ModifiedOn = dt;

                existinRecord1.BatchNumber = null;
                existinRecord1.MobileResponse = null;
                existinRecord1.MobileResponseDate = null;
                existinRecord1.SynchCheckSumString = null;
                existinRecord1.SynchWithMobileDate = null;
                existinRecord1.SynchWithMobileFlag = null;


                objCompassEntities.SaveChanges();
            }

            _result = "Record Updated successfully.";
            output = true;


            return output;



        }

        #endregion

        #region Get Inataller Name by ProjectId and Appointment ID
        public string GetInstaller(string Account)
        {
            tblInstallerMapData result = null;
            string InstallerName = "";
            using (CompassEntities objCompass = new CompassEntities())
            {

                var Applist = objCompass.Appointments.Where(o => o.AccountNumber == Account).FirstOrDefault();
                //chnged sominath Active==1
                result = objCompass.tblInstallerMapDatas.Where(o => o.FKAppointmentID == (long)Applist.AppointmentID && o.Active == 1).FirstOrDefault();
                if (result != null)
                {
                    var _Result = objCompass.tblUsers.Where(o => o.UserID == result.InstallerId).FirstOrDefault();
                    InstallerName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_Result.FirstName);

                }

            }
            return InstallerName;
        }

        #endregion

        #region Update Passed Date Flag
        public int UpdatePassedDateFlag()
        {
            int result = 0;
            CompassEntities objCompass = new CompassEntities();
            try
            {
                result = objCompass.PROC_Appointment_UpdatePassedDateFlag();

            }
            catch (Exception ex)
            {

            }
            return result;

        }

        #endregion

        #region Get Project Name with City Sate
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

        #region Send mail to Installer when installer is allocated

        public string MailToInstaller(AppointmentModel _appModelVM)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            string InstallerName = "";



            var list = objCompassEntities.Appointments.Where(o => o.AppointmentID == _appModelVM.FK_AppointmentID && o.Active == 1).FirstOrDefault();
            long FK_ExcelUploadID = (long)list.FK_UploadedId;

            // var getBookInfo = objCompassEntities.tblProjects.Where(o => o.ProjectId == list.FKProjectId && o.ProjectStatus == "Active" && o.Active == 1).FirstOrDefault();

            //var getState = GetActiveStateRecords().Where(o => o.StateID == getBookInfo.StateId).FirstOrDefault();
            _appModelVM.State = list.State;
            //  var cityName = GetCityByIDs((int)getBookInfo.StateId).Where(o => o.CityID == getBookInfo.CityId).FirstOrDefault();
            _appModelVM.City = list.City;



            var AppList = objCompassEntities.tblInstallerMapDatas.Where(o => o.FKAppointmentID == _appModelVM.FK_AppointmentID && o.Active == 1).FirstOrDefault();
            long InstallerID = AppList.InstallerId;
            //get Installer Name

            var _Result = objCompassEntities.tblUsers.Where(o => o.UserID == InstallerID && o.Active == 1).FirstOrDefault();
            string installerEmail = _Result.Email;
            InstallerName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_Result.FirstName);

            //Set Mail Body
            string MailBody = "";
            StringBuilder sb = new StringBuilder();

            string Message = GetIntallerMessage();                           //"You have following appointment.Please see the appointment details.";

            sb.Append("<body style='padding: 10px 0px 0px 10px; background: #f8f8fb; font-family:Times New Roman; font-size:14px;'>");
            sb.Append("Dear " + InstallerName + ",");
            sb.Append("<p style='font-size:15px;'>Greetings from Data Depot....!!!</p>");
            sb.Append("<div style='background: #e6e1e1; width:100%;'>" + Message + "</div><br/>");
            sb.Append("<div style='background: #e6e1e1; width:100%;'><table style='width:100%;background: #e6e1e1;border:none;padding: 0px 0px 0px 10px;'>" +
                    "<tr>" +
                              "<td style='border:none;'><b>Appointment Number</b></td>" +
                              "<td style='border:none;'><b>:</b></td>" +
                              "<td style='border:none;'>" + _appModelVM.FK_AppointmentID + "</td>" +
                      "<tr/>" +
                       "<tr>" +
                              "<td style='border:none;'><b>Account Number</b></td>" +
                              "<td style='border:none;'><b>:</b></td>" +
                              "<td style='border:none;'>" + _appModelVM.AccountNumber + "</td>" +
                       "<tr/>" +
                          "<tr>" +
                              "<td style='border:none;'><b>Address</b></td>" +
                              "<td style='border:none;'><b>:</b></td>" +
                              "<td style='border:none;' colspan='3'>" + _appModelVM.Address + "</td>" +
                       "<tr/>" +
                      "<tr>" +
                              "<td style='border:none;'><b>Project Name</b></td>" +
                              "<td style='border:none;'><b>:</b></td>" +
                              "<td style='border:none;'>" + _appModelVM.ProjectName + "</td>" +
                      "<tr/>" +
                      "<tr>" +
                              "<td style='border:none;'><b>Utility Type</b></td>" +
                              "<td style='border:none;'><b>:</b></td>" +
                              "<td style='border:none;'>" + _appModelVM.UtilityType + "</td>" +
                      "<tr/>" +
                      "<tr>" +
                              "<td style='border:none;'><b>Cycle</b></td>" +
                              "<td style='border:none;'><b>:</b></td>" +
                              "<td style='border:none;'>" + _appModelVM.Cycle + "</td>" +
                      "<tr/>" +
                         "<tr>" +
                              "<td style='border:none;'><b>Route</b></td>" +
                              "<td style='border:none;'><b>:</b></td>" +
                              "<td style='border:none;'>" + _appModelVM.Route + "</td>" +
                      "<tr/>" +
                       "<tr>" +
                              "<td style='border:none;'><b>City</b></td>" +
                              "<td style='border:none;'><b>:</b></td>" +
                              "<td style='border:none;'>" + _appModelVM.City + "</td>" +
                      "<tr/>" +
                          "<tr>" +
                              "<td style='border:none;'><b>State</b></td>" +
                              "<td style='border:none;'><b>:</b></td>" +
                              "<td style='border:none;'>" + _appModelVM.State + "</td>" +
                      "<tr/>" +


                        "<tr>" +
                              "<td style='border:none;'><b>Appointment Date</b></td>" +
                              "<td style='border:none;'><b>:</b></td>" +
                              "<td style='border:none;' colspan='3'>" + _appModelVM.DateForApp + "</td>" +
                       "<tr/>" +

                        "<tr>" +
                              "<td style='border:none;'><b>Appointment Time</b></td>" +
                              "<td style='border:none;'><b>:</b></td>" +
                              "<td style='border:none;' colspan='3'>" + _appModelVM.FirstTime + "</td>" +
                       "<tr/>" +

                         "<tr>" +
                              "<td style='border:none;'><b>Customer  Name</b></td>" +
                              "<td style='border:none;'><b>:</b></td>" +
                              "<td style='border:none;' colspan='3'>" + _appModelVM.FirstName + "</td>" +
                       "<tr/>" +

                       "<tr>" +
                              "<td style='border:none;'><b>Customer Email</b></td>" +
                              "<td style='border:none;'><b>:</b></td>" +
                              "<td style='border:none;' colspan='3'>" + _appModelVM.EmailAddress + "</td>" +
                       "<tr/>" +

                       "<tr>" +
                              "<td style='border:none;'><b>Customer Phone</b></td>" +
                              "<td style='border:none;'><b>:</b></td>" +
                              "<td style='border:none;' colspan='3'>" + _appModelVM.DayTimePhone + "</td>" +
                       "<tr/>" +


                     "</table></div>");
            sb.Append("<br/><div></div>");

            sb.Append("Thank You,<br/>Data Depot Team.<br>" +
                                           "<p style='text-align:center;'> <a href='https://www.datadepotonline.com/' title='' rel='home'>" +
                                           "<img src='https://www.datadepotonline.com/images/logo_03_2.png' title='Data Depot' style='height:50px; width:75px;'>" +
                                           "</a> </p>" +
                                           "<p style='text-align:center; font-size:13px;'>You are receiving this notification because you are registered with Data Depot<br/>" +
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
            mailto.Add(installerEmail);

            List<String> CCEmailto = new List<String>();
            CCEmailto = CCEmailToInstaller();

            string Subject = GetInstallerSubject(); //"New Appointment";

            new GarronT.CMS.Model.clsMail().SendMail(mailto, CCEmailto, MailBody, Subject);


            return "Mail send successfully";
        }
        #endregion

        #region Send mail to Customer when installer is allocated
        public string MailToCustomer(AppointmentModel _appViewModel)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder PrintIDCard = new StringBuilder();
            try
            {
                using (CompassEntities objCompassEntities = new CompassEntities())
                {
                    string CustomerEmail = _appViewModel.EmailAddress;
                    //Set Mail Body
                    string MailBody = "";


                    var list = objCompassEntities.Appointments.Where(o => o.AppointmentID == _appViewModel.FK_AppointmentID && o.Active == 1).FirstOrDefault();
                    _appViewModel.City = list.City;
                    _appViewModel.State = list.State;
                    // I am writing to confirm the appointment that we have made for June 1, 2010, at 2 pm
                    string Message = GetMessage();




                    //Create Installer ID Card  by installer Id

                    //Get


                    //long cityID = objCompassEntities.TblCityMasters.Where(o => o.CityName == _appViewModel.City && o.Active == 1).Select(o=>o.CityId).FirstOrDefault();
                    //long stateId = objCompassEntities.tblStateMasters.Where(o => o.StateName == _appViewModel.State).Select(o => o.StateId).FirstOrDefault();


                    //string VehicleModel = objCompassEntities.tblVehicleModelMasters.Where(o => o.ID == vehicleInfo.ModelID && o.Active == 1).Select(o=>o.Model).FirstOrDefault();



                    //Create Element 
                    sb.Append("<div style='background: #e6e1e1; width:100%;'>" + Message + "</div><br/>");









                    //"Thanks, You have booked the appointment successfully.This is an email to confirm the appointment that you have made for "+ _appViewModel.DateForApp + " at " + _appViewModel.FirstTime +"." +"<br/>Please see the following appointment details.";
                    // sb.Append("<body style='padding: 10px 0px 0px 10px; background: #f8f8fb; font-family:Times New Roman; font-size:14px;'>");
                    //sb.Append("Dear " + _appViewModel.FirstName + ",");
                    //sb.Append("<p style='font-size:15px;'>Greetings from CMS....!</p>");

                    sb.Append("<div style='background: #e6e1e1; width:100%;'><table style='width:100%;background: #e6e1e1;border:none;padding: 0px 0px 0px 10px;'>" +
                            "<tr>" +
                                      "<td style='border:none;'><b>Appointment Number</b></td>" +
                                      "<td style='border:none;'><b>:</b></td>" +
                                      "<td style='border:none;'>" + _appViewModel.FK_AppointmentID + "</td>" +
                              "<tr/>" +
                               "<tr>" +
                                      "<td style='border:none;'><b>Appointment Date</b></td>" +
                                      "<td style='border:none;'><b>:</b></td>" +
                                      "<td style='border:none;' colspan='3'>" + _appViewModel.DateForApp + "</td>" +
                               "<tr/>" +

                                "<tr>" +
                                      "<td style='border:none;'><b>Appointment Time</b></td>" +
                                      "<td style='border:none;'><b>:</b></td>" +
                                      "<td style='border:none;' colspan='3'>" + _appViewModel.FirstTime + "</td>" +
                               "<tr/>" +


                                 "<tr>" +
                                      "<td style='border:none;'><b>Account Number</b></td>" +
                                      "<td style='border:none;'><b>:</b></td>" +
                                      "<td style='border:none;'>" + _appViewModel.AccountNumber + "</td>" +
                               "<tr/>" +
                                "<tr>" +
                                      "<td style='border:none;'><b>Address</b></td>" +
                                      "<td style='border:none;'><b>:</b></td>" +
                                      "<td style='border:none;' colspan='3'>" + _appViewModel.Address + "</td>" +
                               "<tr/>" +

                              //"<tr>" +
                        //        "<td style='border:none;'><b>Project Name</b></td>" +
                        //        "<td style='border:none;'><b>:</b></td>" +
                        //        "<td style='border:none;'>" + _appViewModel.ProjectName + "</td>" +
                        //"<tr/>" +
                        //"<tr>" +
                        //        "<td style='border:none;'><b>Utility Type</b></td>" +
                        //        "<td style='border:none;'><b>:</b></td>" +
                        //        "<td style='border:none;'>" + _appViewModel.UtilityType + "</td>" +
                        //"<tr/>" +

                                "<tr>" +
                                      "<td style='border:none;'><b>City</b></td>" +
                                      "<td style='border:none;'><b>:</b></td>" +
                                      "<td style='border:none;'>" + _appViewModel.City + "</td>" +
                              "<tr/>" +
                                  "<tr>" +
                                      "<td style='border:none;'><b>State</b></td>" +
                                      "<td style='border:none;'><b>:</b></td>" +
                                      "<td style='border:none;'>" + _appViewModel.State + "</td>" +
                              "<tr/>" +



                              //   "<tr>" +
                        //        "<td style='border:none;'><b>Cycle</b></td>" +
                        //        "<td style='border:none;'><b>:</b></td>" +
                        //        "<td style='border:none;'>" + _appViewModel.Cycle + "</td>" +
                        //"<tr/>" +
                        //   "<tr>" +
                        //        "<td style='border:none;'><b>Route</b></td>" +
                        //        "<td style='border:none;'><b>:</b></td>" +
                        //        "<td style='border:none;'>" + _appViewModel.Route + "</td>" +
                        //"<tr/>" +


                              //   "<tr>" +
                        //        "<td style='border:none;'><b>Customer  Name</b></td>" +
                        //        "<td style='border:none;'><b>:</b></td>" +
                        //        "<td style='border:none;' colspan='3'>" + _appViewModel.FirstName + "</td>" +
                        // "<tr/>" +

                              // "<tr>" +
                        //        "<td style='border:none;'><b>Customer Email</b></td>" +
                        //        "<td style='border:none;'><b>:</b></td>" +
                        //        "<td style='border:none;' colspan='3'>" + _appViewModel.EmailAddress + "</td>" +
                        // "<tr/>" +

                              // "<tr>" +
                        //        "<td style='border:none;'><b>Customer Phone</b></td>" +
                        //        "<td style='border:none;'><b>:</b></td>" +
                        //        "<td style='border:none;' colspan='3'>" + _appViewModel.DayTimePhone + "</td>" +
                        // "<tr/>" +


                             "</table></div>");



                    sb.Append("<br/><br/>");

                    var CompanyLogo = "https://www.datadepotonline.com/images/logo_03_2.png";

                    var existInstaller = objCompassEntities.tblUsers.Where(o => o.UserID == _appViewModel.InstallerID && o.Active == 1).FirstOrDefault();
                    var vehicleInfo = objCompassEntities.tblVehicleMasters.Where(o => o.UserId == _appViewModel.InstallerID && o.Active == 1).FirstOrDefault();
                    var projectInfo = objCompassEntities.tblProjects.Where(o => o.ProjectId == list.FKProjectId && o.Active == 1).FirstOrDefault();

                    var getCityLogoInfo = objCompassEntities.TblStateCityLogoes.Where(o => o.FK_CityId == projectInfo.CityId
                           && o.FK_StateId == projectInfo.StateId && o.Active == 1 && o.FK_CustomerId == projectInfo.ClientId).FirstOrDefault();

                    var checkUserIcards = objCompassEntities.UserIcards.Where(o => o.FKStateId == projectInfo.StateId &&
                        o.FKCityId == projectInfo.CityId && o.FKCustomerId == projectInfo.ClientId && o.Active == 1
                        && o.FKUserId == existInstaller.UserID).FirstOrDefault();
                    if (existInstaller.InstallerImage != null && getCityLogoInfo != null && checkUserIcards != null)
                    {
                        string InstallerName = existInstaller.FirstName;
                        string profileImage = existInstaller.InstallerImage;
                        string VehicleLicenseNo = vehicleInfo.LicensePlate;

                        var vehicleMakeName = objCompassEntities.tblVehicleMakeMasters.Where(o => o.MakeId == vehicleInfo.MakeID).Select(o => o.Make).FirstOrDefault();
                        var vehicleModelName = objCompassEntities.tblVehicleModelMasters.Where(o => o.ID == vehicleInfo.ModelID).Select(o => o.Model).FirstOrDefault();
                        string VehicleColor = vehicleInfo.Color;

                        vehicleMakeName = vehicleMakeName == null ? "" : vehicleMakeName;
                        vehicleModelName = vehicleModelName == null ? "" : vehicleModelName;

                        //Create ID Card Desining to sent it with mail body.
                        //Get web url path from web.congig
                        string webPath = System.Configuration.ConfigurationManager.AppSettings["WebURL"];
                        //string webPath = "https://localhost:6069/";
                        //  string userProfileImage = webPath + "/Uploads/UserProfile/" + _appViewModel.InstallerID + "/" + profileImage;
                        string userProfileImage = webPath + profileImage;
                        //Get city Logo by city name



                        string CityLogoName = getCityLogoInfo.ImageName == null ? "" : getCityLogoInfo.ImageName;
                        string CityImageLogoPath = getCityLogoInfo.CityLogoImagePath == null ? "" : getCityLogoInfo.CityLogoImagePath;
                        string CustomerCityLogo = webPath + "Uploads/UploadCityLogo/" + CityLogoName + "/" + CityImageLogoPath;
                        CustomerCityLogo = CustomerCityLogo.Replace("~", "");

                        //get image text from table UserIcard by city id


                        string ImageText = objCompassEntities.UserIcards.Where(o => o.FKCityId == projectInfo.CityId && o.FKStateId == projectInfo.StateId && o.Active == 1).Select(o => o.ICardMessage).FirstOrDefault();



                        sb.Append("Our employee will visit on given time, please check his details below.");
                        sb.Append("<br/><br/>");


                        sb.Append(" <table style='border: 2px solid; border-radius: 8px; display: block;width:430px;margin-left:150px;'> <tr> ");

                        sb.Append(" <td colspan='2' style='position: relative;text-align: center; width:100%; margin-right:10px;margin-top:5px;'> " +

                            "<img src='" + CustomerCityLogo + "' width='100' height='40'></td> </tr>");

                        sb.Append(" <tr>");
                        sb.Append(" <td  style='text-align:left;margin-left:5px;float:left;margin-right:5px;'>  ");

                        sb.Append("<img src='" + userProfileImage + "' width='150' height='170' />");

                        sb.Append("</td>");

                        sb.Append(" <td style='width:260px;float: right;'> ");

                        sb.Append(" <table>");

                        sb.Append(" <tr>");
                        sb.Append(" <td >  ");
                        sb.Append("<label id='lblPrtEmployeeName' style='font-weight: bold;font-size:15px;'>" + InstallerName + "</label>");
                        sb.Append("</td>");
                        sb.Append(" </tr>");


                        sb.Append(" <tr>");
                        sb.Append(" <td > ");
                        sb.Append(" <label id='lblPrtRoleName' style='margin-top:5px; font-size:12px;'>" + "Installer" + "</label>");
                        sb.Append("</td>");
                        sb.Append("  </tr>");


                        sb.Append(" <tr>");
                        sb.Append(" <td > ");
                        sb.Append(" <span style='font-size:11px;'>Vehicle Licence No :<label id='lblPrtEmployeeLicenceNo' style='margin-top:5px;'>" + VehicleLicenseNo + "</label></span>");
                        sb.Append("</td>");
                        sb.Append("  </tr>");


                        sb.Append(" <tr>");
                        sb.Append(" <td > ");
                        sb.Append(" <span style='font-size:11px;'>Make & Model :<label id='lblPrtEmployeeVehicleType' style='margin-top:5px;'>" + vehicleMakeName + " - " + vehicleModelName + "</label></span>");
                        sb.Append("</td>");
                        sb.Append("  </tr>");

                        sb.Append(" <tr>");
                        sb.Append(" <td > ");
                        sb.Append("  <span style='font-size:11px;'>Vehicle Color :<label id='lblPrtEmployeeVehicleColor' style='margin-top:5px;'>" + VehicleColor + "</label></span>");
                        sb.Append("</td>");
                        sb.Append("  </tr>");


                        sb.Append(" <tr>");
                        sb.Append(" <td > ");
                        sb.Append("  <div id='DivPrintMessage' style='padding:0px 4px; font-size:11px;'>" + (checkUserIcards != null ? checkUserIcards.ICardMessage : "") + "</div>");
                        sb.Append("</td>");
                        sb.Append("  </tr>");


                        sb.Append(" </table>");
                        sb.Append(" </td > ");
                        sb.Append("  </tr>");

                       

                        sb.Append(" <tr>");
                        sb.Append(" <td colspan='2' style='border-top: 1px solid; text-align:center;margin-bottom:10px; font-size:11px; font-weight:bold;'> ");
                        sb.Append("©" + DateTime.Now.ToString("yyyy") + " Data Depot");
                        sb.Append("<img src='" + CompanyLogo + "' width='80' height='60' alt='DataDepotLogo'>");
                        sb.Append("</td>");
                        sb.Append("  </tr>");


                        sb.Append(" </table>");



                        



                        sb.Append("<br/><br/>");

                    }



                    sb.Append("Please if you have any query free feel to contact our administration.");

                    sb.Append("<br/><br/>");

                    sb.Append("Thank You,<br/>Data Depot Team.<br>" +
                                                   "<p style='text-align:center;'> <a href='https://www.datadepotonline.com/' title='' rel='home'>" +
                                                   "<img src='" + CompanyLogo + "' title='Data Depot' alt='' style='height:100px; width:200px;'>" +
                                                   "</a> </p>" +
                                                   "<p style='text-align:center; font-size:13px;'>You are receiving this notification because you are registered with Data Depot <br/>" +
                                                   " ©" + DateTime.Now.ToString("yyyy") + " Data Depot</p> <br>");
                    sb.Append("<p style='text-align:center; color:red; font-size:15px;'>*** This Email is system generated. Please do not reply to this email ID." +
                                "For any queries please contact Data Depot Team. ***</p>");
                    sb.Append("<p style='text-align:center; color:red; font-size:13px;'>This message (including any attachments) contains confidential information" +
                                " intended for a specific individual and purpose and is protected by law." +
                                " If you are not the intended recipient, you should delete this message." +
                                " Any disclosure, copying or distribution of this message, or the taking of any action based on it, is strictly prohibited.</p>");




                    MailBody = sb.ToString();// + DIcard.ToString();

                    List<String> mailto = new List<String>();
                    mailto.Add(CustomerEmail);

                    List<String> CCEmailto = new List<String>();
                    CCEmailto = CCEmailToInstaller();


                    string Subject = GetSubject(); //"Appointment Confirmation."; 

                    new GarronT.CMS.Model.clsMail().SendMail(mailto, CCEmailto, MailBody, Subject);

                }

            }
            catch (Exception ex)
            {

            }
            return "Mail send successfully.";

        }
        #endregion

        #region Get subject for customer mail
        public string GetSubject()
        {
            string subject = "";
            CompassEntities dbcontext = new CompassEntities();
            var ObjTblEmailConfigure = (from b in dbcontext.TblAlertMasters
                                        join c in dbcontext.TblEmailConfigures on b.AlertId equals c.AlertTableID
                                        where b.AlertId == 7
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

        #region Get Message for customer mail
        public string GetMessage()
        {
            string Message = "";
            CompassEntities dbcontext = new CompassEntities();
            var ObjTblEmailConfigure = (from b in dbcontext.TblAlertMasters
                                        join c in dbcontext.TblEmailConfigures on b.AlertId equals c.AlertTableID
                                        where b.AlertId == 7
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

        #region Get CC  Email List for customer
        public List<String> CCEmailTo()
        {
            List<String> List = new List<String>();
            CompassEntities db = new CompassEntities();
            var CCEmailId = (from b in db.TblAlertMasters
                             join c in db.TblEmailConfigures on b.AlertId equals c.AlertTableID
                             join d in db.TblEmailConfigureUserDetails on c.ID equals d.TblEmailConfigureID
                             where b.AlertId == 7
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

        #region Get Installer Message body
        public string GetIntallerMessage()
        {
            string Message = "";
            CompassEntities dbcontext = new CompassEntities();
            var ObjTblEmailConfigure = (from b in dbcontext.TblAlertMasters
                                        join c in dbcontext.TblEmailConfigures on b.AlertId equals c.AlertTableID
                                        where b.AlertId == 6
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

        #region Get Subject for Installer
        public string GetInstallerSubject()
        {
            string subject = "";
            CompassEntities dbcontext = new CompassEntities();
            var ObjTblEmailConfigure = (from b in dbcontext.TblAlertMasters
                                        join c in dbcontext.TblEmailConfigures on b.AlertId equals c.AlertTableID
                                        where b.AlertId == 6
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

        #region Get CC  Email List for Installer
        public List<String> CCEmailToInstaller()
        {
            List<String> List = new List<String>();
            CompassEntities db = new CompassEntities();
            var CCEmailId = (from b in db.TblAlertMasters
                             join c in db.TblEmailConfigures on b.AlertId equals c.AlertTableID
                             join d in db.TblEmailConfigureUserDetails on c.ID equals d.TblEmailConfigureID
                             where b.AlertId == 6
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

        #region get appointment record for rescheduling

        public Appointment GetAppointmentDetails(long projectId, string account, long appointmentID)
        {
            Appointment result = new Appointment();
            using (CompassEntities _ObjCompass = new CompassEntities())
            {
                result = _ObjCompass.Appointments.Where(o => o.AccountNumber == account && o.AppointmentID == appointmentID && o.FKProjectId == projectId && o.PassedDateFlag == 1).FirstOrDefault();
            }

            return result;
        }
        #endregion

    }
}
