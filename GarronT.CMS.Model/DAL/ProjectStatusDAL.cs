using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;


namespace GarronT.CMS.Model.DAL
{
    public class ProjectStatusDAL : IDisposable
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



        #region GetMethods


        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="InstallerID"></param>
        /// <returns></returns>
        public List<addressVisitInfo> GetProjectStaus(long projectID, long InstallerID, long formTypeId)
        {


            List<addressVisitInfo> objVisitList = new List<addressVisitInfo>();

            CompassEntities objCompassEntities = new CompassEntities();

            #region

            if (InstallerID > 0)
            {

                #region old code commented by bharat on 23 Aug 2016
                //var resultData = objCompassEntities.STP_ProjectColorStatus_Installer(projectID, InstallerID).ToList();


                //foreach (var item in resultData)
                //{
                //    var getD = objVisitList.Where(o => o.latitude == Convert.ToString(item.Latitude) && o.longitude == Convert.ToString(item.Longitude)).FirstOrDefault();
                //    if (getD == null)
                //    {
                //        var recCount = resultData.Where(o => o.Latitude == item.Latitude && o.Longitude == item.Longitude).Count();
                //        string street = string.Empty;
                //        if (recCount > 1)
                //        {
                //            var streetST = resultData.Where(o => o.Latitude == item.Latitude && o.Longitude == item.Longitude).Select(o => o.street).ToList();
                //            street = StringExtensions.GetMostCommonSubstrings(streetST).FirstOrDefault();
                //        }
                //        else
                //        {
                //            street = item.street;
                //        }
                //        street = "<strong>Installer Name:" + item.InstallerName + "</strong></br><strong>Street:</strong> " + street + "</br>" +
                //            "<strong>Total Visits:" + Convert.ToString(item.TotalRecordCount) + "</strong></br>" +
                //            "<strong>Completed Visits:" + Convert.ToString(item.TotalCompletedCount) + "</strong></br>" +
                //            "<strong>Skipped Visits:" + Convert.ToString(item.TotalSkipedCount) + "</strong></br>" +
                //            "<strong>Pending Visits:" + Convert.ToString(item.TotalRecordCount - (item.TotalCompletedCount + item.TotalSkipedCount)) + "</strong></br>" +
                //            "<strong>Last Visit Date:" + (item.VisitDate != null ? DateTime.Parse(item.VisitDate.ToString()).ToString("MMM-dd-yyyy hh:mm tt") : "") + "</strong></br>";
                //        addressVisitInfo obj1 = new addressVisitInfo();
                //        obj1.colorFlag = int.Parse(Convert.ToString(item.ColorFlag));
                //        obj1.latitude = Convert.ToString(item.Latitude);
                //        obj1.longitude = Convert.ToString(item.Longitude);
                //        obj1.pendingVisit = Convert.ToString(item.TotalRecordCount - (item.TotalCompletedCount + item.TotalSkipedCount));
                //        obj1.street = street;
                //        obj1.totalVisit = Convert.ToString(item.TotalRecordCount);
                //        obj1.completedVisit = Convert.ToString(item.TotalCompletedCount);
                //        obj1.skippedVisit = Convert.ToString(item.TotalSkipedCount);
                //        obj1.lastVisitDatetime = Convert.ToString(item.VisitDate);
                //        objVisitList.Add(obj1);
                //    }
                //}
                #endregion
                objCompassEntities.Database.CommandTimeout = 0;
                var resultData = objCompassEntities.PROC_ProjectStatus_InstallerData(projectID, InstallerID, formTypeId).Where(o => !string.IsNullOrEmpty(o.Latitude) && !string.IsNullOrEmpty(o.Longitude)).ToList();


                foreach (var item in resultData)
                {
                    var getD = objVisitList.Where(o => o.latitude == Convert.ToString(item.Latitude) && o.longitude == Convert.ToString(item.Longitude)).FirstOrDefault();
                    if (getD == null)
                    {
                        var recCount = resultData.Where(o => o.Latitude == item.Latitude && o.Longitude == item.Longitude).Count();
                        string street = string.Empty;
                        if (recCount > 1)
                        {
                            var streetST = resultData.Where(o => o.Latitude == item.Latitude && o.Longitude == item.Longitude).Select(o => o.street).ToList();
                            street = StringExtensions.GetMostCommonSubstrings(streetST).FirstOrDefault();
                        }
                        else
                        {
                            street = item.street;
                        }

                        string reason = "";
                        if (!string.IsNullOrEmpty(item.Reason))
                        {
                            reason = "<strong>" + item.Account + " | " + item.FormName + "  </br>Reason- </br>" + item.Reason + " </br>" + (string.IsNullOrEmpty(item.Comment) ? "" : "Comment- </br>" + item.Comment) + "</strong></br>";
                        }
                        street = "<div class='scrollFixMAP'><strong>Installer Name:" + item.InstallerName + "</strong></br><strong>Street:</strong> " + street + "</br>" +
                             "<strong>Cycle:</strong>" + Convert.ToString(item.Cycle) + ",&nbsp;&nbsp;&nbsp;&nbsp; <strong>Route:</strong>" + Convert.ToString(item.Route) + "</br>" +
                             "<strong>Pending :" + Convert.ToString(item.TotalPendingCount) + "</strong></br>" +
                             "<strong>Complete :" + Convert.ToString(item.TotalCompletedCount) + "</strong></br>" +
                            //"<strong>Total :" + Convert.ToString(item.TotalRecordCount) + "</strong></br>" +                          
                             "<strong>Skip :" + Convert.ToString(item.TotalSkipedCount) + "</strong></br>" +
                             "<strong>RTU :" + Convert.ToString(item.TotalRTUCount) + "</strong></br>";
                        if (item.VisitDate != null)
                        {
                            street = street +
                                "<strong>Last Visit Date:" + DateTime.Parse(item.VisitDate.ToString()).ToString("MMM-dd-yyyy hh:mm tt") + "</strong></br>";
                        }
                        street = street + reason + "</div>";
                        addressVisitInfo obj1 = new addressVisitInfo();
                        obj1.colorFlag = int.Parse(Convert.ToString(item.LatLongColorFlag));
                        obj1.latitude = Convert.ToString(item.Latitude);
                        obj1.longitude = Convert.ToString(item.Longitude);
                        obj1.pendingVisit = Convert.ToString(item.TotalPendingCount);
                        obj1.street = street;
                        obj1.totalVisit = Convert.ToString(item.TotalRecordCount);
                        obj1.completedVisit = Convert.ToString(item.TotalCompletedCount);
                        obj1.skippedVisit = Convert.ToString(item.TotalSkipedCount);
                        obj1.rtuVisit = Convert.ToString(item.TotalRTUCount);
                        obj1.lastVisitDatetime = Convert.ToString(item.VisitDate);
                        objVisitList.Add(obj1);
                    }
                    else
                    {
                        string reason = "";
                        if (!string.IsNullOrEmpty(item.Reason))
                        {
                            //reason = "<strong>" + item.Account + " | " + item.FormName + " | " + item.Reason + " | " + (string.IsNullOrEmpty(item.Comment) ? "-" : item.Comment)+"</strong></br>";
                            reason = "<strong>" + item.Account + " | " + item.FormName + "  </br>Reason- </br>" + item.Reason + " </br>" + (string.IsNullOrEmpty(item.Comment) ? "" : "Comment- </br>" + item.Comment) + "</strong></br>";
                            getD.street = getD.street.Remove((getD.street.Length - 6), 6) + reason + "</div>";
                        }
                    }
                }
            }
            else
            {
                #region old commented on 24 Aug 2016 by Bharat
                //var resultData = objCompassEntities.STP_ProjectColorStatus(projectID).ToList();


                //foreach (var item in resultData)
                //{
                //    var getD = objVisitList.Where(o => o.latitude == Convert.ToString(item.Latitude) && o.longitude == Convert.ToString(item.Longitude)).FirstOrDefault();
                //    if (getD == null)
                //    {
                //        var recCount = resultData.Where(o => o.Latitude == item.Latitude && o.Longitude == item.Longitude).Count();
                //        string street = string.Empty;
                //        if (recCount > 1)
                //        {
                //            var streetST = resultData.Where(o => o.Latitude == item.Latitude && o.Longitude == item.Longitude).Select(o => o.street).ToList();
                //            street = StringExtensions.GetMostCommonSubstrings(streetST).FirstOrDefault();
                //        }
                //        else
                //        {
                //            street = item.street;
                //        }
                //        street = "<strong>Installer Name:" + item.InstallerName + "</strong></br><strong>Street:</strong> " + street + "</br>" +
                //            "<strong>Total Visits:" + Convert.ToString(item.TotalRecordCount) + "</strong></br>" +
                //            "<strong>Completed Visits:" + Convert.ToString(item.TotalCompletedCount) + "</strong></br>" +
                //            "<strong>Skipped Visits:" + Convert.ToString(item.TotalSkipedCount) + "</strong></br>" +
                //            "<strong>Pending Visits:" + Convert.ToString(item.TotalRecordCount - (item.TotalCompletedCount + item.TotalSkipedCount)) + "</strong></br>" +
                //            "<strong>Last Visit Date:" + (item.VisitDate != null ? DateTime.Parse(item.VisitDate.ToString()).ToString("MMM-dd-yyyy hh:mm tt") : "") + "</strong></br>";
                //        addressVisitInfo obj1 = new addressVisitInfo();
                //        obj1.colorFlag = int.Parse(Convert.ToString(item.ColorFlag));
                //        obj1.latitude = Convert.ToString(item.Latitude);
                //        obj1.longitude = Convert.ToString(item.Longitude);
                //        obj1.pendingVisit = Convert.ToString(item.TotalRecordCount - (item.TotalCompletedCount + item.TotalSkipedCount));
                //        obj1.street = street;
                //        obj1.totalVisit = Convert.ToString(item.TotalRecordCount);
                //        obj1.completedVisit = Convert.ToString(item.TotalCompletedCount);
                //        obj1.skippedVisit = Convert.ToString(item.TotalSkipedCount);
                //        obj1.lastVisitDatetime = Convert.ToString(item.VisitDate);
                //        objVisitList.Add(obj1);
                //    }
                //}
                #endregion
                objCompassEntities.Database.CommandTimeout = 0;
                // var resultData = objCompassEntities.PROC_ProjectStatus_AllData(projectID, formTypeId).Where(o => !string.IsNullOrEmpty(o.Latitude) && !string.IsNullOrEmpty(o.Longitude)).ToList();
                //var resultData = objCompassEntities.PROC_ProjectStatus_AllData(projectID, formTypeId).ToList();


                List<PROC_ProjectStatus_AllData_Result> resultData = new List<PROC_ProjectStatus_AllData_Result>();

                DataTable dt = new DataTable();
                using (SqlConnection sqlcon = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("PROC_ProjectStatus_AllData", sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ProjectId", projectID);
                        cmd.Parameters.Add("@FormTypeId", formTypeId);
                        cmd.CommandTimeout = 0;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                DateTime? dt1 = null;
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {

                        resultData.Add(new PROC_ProjectStatus_AllData_Result
                        {
                            Account = Convert.ToString(item["Account"]),
                            AccountColorFlag = int.Parse(Convert.ToString(item["AccountColorFlag"])),
                            Comment = Convert.ToString(item["Comment"]),
                            Complete = int.Parse(Convert.ToString(item["Complete"])),
                            FormComplete = int.Parse(Convert.ToString(item["FormComplete"])),
                            FormCount = int.Parse(Convert.ToString(item["FormCount"])),
                            FormId = !string.IsNullOrEmpty(Convert.ToString(item["FormId"])) ? long.Parse(Convert.ToString(item["FormId"])) : 0,
                            FormName = Convert.ToString(item["FormName"]),
                            FormPending = int.Parse(Convert.ToString(item["FormPending"])),
                            FormRTU = int.Parse(Convert.ToString(item["FormRTU"])),
                            FormSkipped = int.Parse(Convert.ToString(item["FormSkipped"])),
                            ID = Convert.ToString(item["ID"]),
                            InstallerId = !string.IsNullOrEmpty(Convert.ToString(item["InstallerId"])) ? long.Parse(Convert.ToString(item["InstallerId"])) : 0,
                            InstallerMapId = !string.IsNullOrEmpty(Convert.ToString(item["InstallerMapId"])) ? long.Parse(Convert.ToString(item["InstallerMapId"])) : 0,
                            InstallerName = Convert.ToString(item["InstallerName"]),
                            Latitude = Convert.ToString(item["Latitude"]),
                            LatLongColorFlag = int.Parse(Convert.ToString(item["LatLongColorFlag"])),
                            Longitude = Convert.ToString(item["Longitude"]),
                            Pending = int.Parse(Convert.ToString(item["Pending"])),
                            ProjectId = long.Parse(Convert.ToString(item["ProjectId"])),
                            Reason = Convert.ToString(item["Reason"]),
                            RTU = int.Parse(Convert.ToString(item["RTU"])),
                            Skipped = int.Parse(Convert.ToString(item["Skipped"])),
                            street = Convert.ToString(item["street"]),
                            Cycle = Convert.ToString(item["Cycle"]),
                            Route = Convert.ToString(item["Route"]),
                            TotalCompletedCount = int.Parse(Convert.ToString(item["TotalCompletedCount"])),
                            TotalPendingCount = int.Parse(Convert.ToString(item["TotalPendingCount"])),
                            TotalRecordCount = int.Parse(Convert.ToString(item["TotalRecordCount"])),
                            TotalRTUCount = int.Parse(Convert.ToString(item["TotalRTUCount"])),
                            TotalSkipedCount = int.Parse(Convert.ToString(item["TotalSkipedCount"])),
                            VisitDate = !string.IsNullOrEmpty(Convert.ToString(item["VisitDate"])) ? DateTime.Parse(Convert.ToString(item["VisitDate"])) : dt1,

                        });
                    }

                }





                foreach (var item in resultData)
                {
                    var getD = objVisitList.Where(o => o.latitude == Convert.ToString(item.Latitude) && o.longitude == Convert.ToString(item.Longitude)).FirstOrDefault();
                    if (getD == null)
                    {
                        var recCount = resultData.Where(o => o.Latitude == item.Latitude && o.Longitude == item.Longitude).Count();
                        string street = string.Empty;
                        if (recCount > 1)
                        {
                            var streetST = resultData.Where(o => o.Latitude == item.Latitude && o.Longitude == item.Longitude).Select(o => o.street).ToList();
                            street = StringExtensions.GetMostCommonSubstrings(streetST).FirstOrDefault();
                        }
                        else
                        {
                            street = item.street;
                        }

                        string reason = "";
                        if (!string.IsNullOrEmpty(item.Reason))
                        {
                            //reason = "<strong>" + item.Account + " | " + item.FormName + " | " + item.Reason + " | " + (string.IsNullOrEmpty(item.Comment) ? "-" : item.Comment) + "</strong></br>";
                            reason = "<strong>" + item.Account + " | " + item.FormName + "  </br>Reason- </br>" + item.Reason + " </br>" + (string.IsNullOrEmpty(item.Comment) ? "" : "Comment- </br>" + item.Comment) + "</strong></br>";
                        }
                        street = "<div class='scrollFixMAP'><strong>Installer Name:" + item.InstallerName + "</strong></br><strong>Street:</strong> " + street + "</br>" +
                           //"<strong>Total Visits:" + Convert.ToString(item.TotalRecordCount) + "</strong></br>" +
                           "<strong>Cycle:</strong>" + Convert.ToString(item.Cycle) + ",&nbsp;&nbsp;&nbsp;&nbsp; <strong>Route:</strong>" + Convert.ToString(item.Route) + "</br>" +
                           "<strong>Pending:" + Convert.ToString(item.TotalPendingCount) + "</strong></br>" +
                           "<strong>Complete:" + Convert.ToString(item.TotalCompletedCount) + "</strong></br>" +
                           "<strong>Skip:" + Convert.ToString(item.TotalSkipedCount) + "</strong></br>" +
                            "<strong>RTU:" + Convert.ToString(item.TotalRTUCount) + "</strong></br>";
                        if (item.VisitDate != null)
                        {
                            street = street + "<strong>Last Visit Date:" + DateTime.Parse(item.VisitDate.ToString()).ToString("MMM-dd-yyyy hh:mm tt") + "</strong></br>";
                        }
                        street = street + reason + "</div>"; ;

                        addressVisitInfo obj1 = new addressVisitInfo();
                        obj1.colorFlag = int.Parse(Convert.ToString(item.LatLongColorFlag));
                        obj1.latitude = Convert.ToString(item.Latitude);
                        obj1.longitude = Convert.ToString(item.Longitude);
                        obj1.pendingVisit = Convert.ToString(item.TotalPendingCount);
                        obj1.street = street;
                        obj1.totalVisit = Convert.ToString(item.TotalRecordCount);
                        obj1.completedVisit = Convert.ToString(item.TotalCompletedCount);
                        obj1.skippedVisit = Convert.ToString(item.TotalSkipedCount);
                        obj1.lastVisitDatetime = Convert.ToString(item.VisitDate);
                        objVisitList.Add(obj1);
                    }
                    else
                    {
                        string reason = "";
                        if (!string.IsNullOrEmpty(item.Reason))
                        {
                            //reason = "<strong>" + item.Account + " | " + item.FormName + " | " + item.Reason + " | " + (string.IsNullOrEmpty(item.Comment) ? "-" : item.Comment) + "</strong></br>";
                            reason = "<strong>" + item.Account + " | " + item.FormName + "  </br>Reason- </br>" + item.Reason + " </br>" + (string.IsNullOrEmpty(item.Comment) ? "" : "Comment- </br>" + item.Comment) + "</strong></br>";

                            getD.street = getD.street.Remove((getD.street.Length - 6), 6) + reason + "</div>";
                        }

                    }
                }
            }
            #endregion

            return objVisitList;
        }




        /// <summary>
        /// Returns project list for given status. 
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



        public List<UsersSmallModel> GeInstallerListCurrentProject(long ProjectId, long formType)
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<UsersSmallModel> InstallerList = new List<UsersSmallModel>();

                if (formType == 2)
                {
                    InstallerList = (from b in CompassEntities.TblProjectInstallers
                                     where b.Active == 1 && b.ProjectID == ProjectId
                                     select new UsersSmallModel
                                     {
                                         UserId = b.tblUsers_UserID,
                                         UserName = CompassEntities.tblUsers.Where(a => a.UserID == b.tblUsers_UserID).
                                         Select(m => m.FirstName).FirstOrDefault()
                                     }).ToList();


                    var InstallerList1 = CompassEntities.TblProjectInstallers.AsNoTracking().Where(o => o.Active == 0 && o.ProjectID == ProjectId).ToList();

                    var data = new WebServiceUserRoleDAL().GetAllUsersBasedOnRoleWithSelf("Installer").Select(o => o.ChildId).ToList();



                    if (InstallerList1 != null && InstallerList1.Count > 0)
                    {

                        


                        foreach (var item in InstallerList1)
                        {
                            var checkExist = (from post in CompassEntities.UploadedExcelDatas
                                              join meta in CompassEntities.TblProjectFieldDatas on post.Id equals meta.FK_UploadedExcelData_Id
                                              where meta.Active == 1 && post.FKProjectId == ProjectId
                                              select new { id = meta.ID }).FirstOrDefault();

                            if (checkExist != null)
                            {
                                InstallerList.Add(
                                    new UsersSmallModel
                                    {
                                        UserId = item.tblUsers_UserID,
                                        UserName = CompassEntities.tblUsers.Where(a => a.UserID == item.tblUsers_UserID).Select(m => m.FirstName).FirstOrDefault()
                                    }
                                    );
                            }
                        }

                        InstallerList = InstallerList.Where(o => data.Contains(o.UserId)).ToList();
                    }


                }
                else
                {
                    var auditMaster = CompassEntities.AuditMasters.AsNoTracking().Where(o => o.FKProjectId == ProjectId).FirstOrDefault();

                    if (auditMaster != null)
                    {
                        InstallerList = (from b in CompassEntities.AuditorMaps
                                         where b.Active == 1 && b.FK_AuditId == auditMaster.Id
                                         select new UsersSmallModel
                                         {
                                             UserId = b.FK_UserId,
                                             UserName = CompassEntities.tblUsers.Where(a => a.UserID == b.FK_UserId).
                                             Select(m => m.FirstName).FirstOrDefault()
                                         }).ToList();


                        // var auditProjectId = CompassEntities.AuditMasters.AsNoTracking().Where(o => o.FKProjectId == ProjectId).FirstOrDefault();
                        var InstallerList1 = CompassEntities.AuditorMaps.AsNoTracking().Where(o => o.Active == 0 && o.FK_AuditId == auditMaster.Id).ToList();
                        if (InstallerList1 != null && InstallerList1.Count > 0)
                        {
                            foreach (var item in InstallerList1)
                            {
                                var checkExist = (from post in CompassEntities.AuditDatas
                                                  join meta in CompassEntities.ProjectAuditDatas on post.Id equals meta.FK_UploadedExcelData_Id
                                                  where meta.Active == 1 && post.FK_ProjectId == ProjectId
                                                  select new { id = meta.ID }).FirstOrDefault();

                                if (checkExist != null)
                                {
                                    InstallerList.Add(
                                        new UsersSmallModel
                                        {
                                            UserId = item.FK_UserId,
                                            UserName = CompassEntities.tblUsers.Where(a => a.UserID == item.FK_UserId).Select(m => m.FirstName).FirstOrDefault()
                                        }
                                        );
                                }
                            }
                        }
                    }


                }

                InstallerList = InstallerList.OrderBy(o => o.UserName).ToList();
                return InstallerList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// AddedBy AniketJ on 15-Oct-2016
        /// To get FormType List
        /// </summary>
        /// <returns></returns>
        public List<FormTypeModel> GetFormTypeList()
        {
            CompassEntities CompassEntities = new CompassEntities();
            var data = CompassEntities.tblFormTypes.AsNoTracking().Where(o => o.Active == 1 && (o.ID == 2 || o.ID == 1)).AsEnumerable().
                Select(o => new FormTypeModel
                {
                    ID = o.ID,
                    FormType = o.FormType
                }).OrderBy(o => o.FormType).ToList();

            return data;
        }

        /// <summary>
        /// CreatedBy AniketJ on 15-Oct-2016
        /// To get ProjectList
        /// </summary>
        /// <param name="UtilityType"></param>
        /// <param name="ProjectStatus"></param>
        /// <returns>ProjectList</returns>
        public List<ProjectModel> GetProjectName(string UtilityType, string ProjectStatus)
        {
            CompassEntities CompassEntities = new CompassEntities();
            string proStatus = "";
            if (ProjectStatus == "2")
            {
                proStatus = "Complete";
            }
            else
            {
                proStatus = "Active";
            }
            List<ProjectModel> data = CompassEntities.STP_GetProjectCityState().Where(a => a.Active == 1 && a.Utilitytype == UtilityType
                && a.ProjectStatus == proStatus).Select(o =>
                                       new ProjectModel
                                       {
                                           ProjectId = o.ProjectId,
                                           // ProjectName = o.ProjectName,
                                           ProjectCityState = o.ProjectCityName,
                                           stringFromDate = o.strFormDate,
                                           stringToDate = o.strToDate,
                                           ProjectStatus = o.ProjectStatus
                                       }).OrderBy(a => a.ProjectCityState).ToList();
            return data;


        }

        #endregion






        public List<addressVisitInfo> GetAuditProjectStaus(long projectID, long InstallerID, long formTypeId)
        {


            List<addressVisitInfo> objVisitList = new List<addressVisitInfo>();

            CompassEntities objCompassEntities = new CompassEntities();

            #region

            if (InstallerID > 0)
            {

                objCompassEntities.Database.CommandTimeout = 0;
                var resultData = objCompassEntities.PROC_ProjectStatus_auditorData(projectID, InstallerID, formTypeId).Where(o => !string.IsNullOrEmpty(o.Latitude) && !string.IsNullOrEmpty(o.Longitude)).ToList();


                foreach (var item in resultData)
                {
                    var getD = objVisitList.Where(o => o.latitude == Convert.ToString(item.Latitude) && o.longitude == Convert.ToString(item.Longitude)).FirstOrDefault();
                    if (getD == null)
                    {
                        var recCount = resultData.Where(o => o.Latitude == item.Latitude && o.Longitude == item.Longitude).Count();
                        string street = string.Empty;
                        if (recCount > 1)
                        {
                            var streetST = resultData.Where(o => o.Latitude == item.Latitude && o.Longitude == item.Longitude).Select(o => o.street).ToList();
                            street = StringExtensions.GetMostCommonSubstrings(streetST).FirstOrDefault();
                        }
                        else
                        {
                            street = item.street;
                        }

                        string reason = "";
                        if (!string.IsNullOrEmpty(item.Reason))
                        {
                            reason = "<strong>" + item.Account + " | " + item.FormName + "  </br>Reason- </br>" + item.Reason + " </br>" + (string.IsNullOrEmpty(item.Comment) ? "" : "Comment- </br>" + item.Comment) + "</strong></br>";
                        }
                        street = "<div class='scrollFixMAP'><strong>Auditor Name:" + item.AuditorName + "</strong></br><strong>Street:</strong> " + street + "</br>" +
                             "<strong>Pending :" + Convert.ToString(item.TotalPendingCount) + "</strong></br>" +
                               "<strong>Complete :" + Convert.ToString(item.TotalCompletedCount) + "</strong></br>" +
                            //"<strong>Total :" + Convert.ToString(item.TotalRecordCount) + "</strong></br>" +                          
                            "<strong>Skip :" + Convert.ToString(item.TotalSkipedCount) + "</strong></br>";
                        //"<strong>RTU :" + Convert.ToString(item.TotalRTUCount) + "</strong></br>";
                        if (item.VisitDate != null)
                        {
                            street = street +
                                "<strong>Last Visit Date:" + DateTime.Parse(item.VisitDate.ToString()).ToString("MMM-dd-yyyy hh:mm tt") + "</strong></br>";
                        }
                        street = street + reason + "</div>";
                        addressVisitInfo obj1 = new addressVisitInfo();
                        obj1.colorFlag = int.Parse(Convert.ToString(item.LatLongColorFlag));
                        obj1.latitude = Convert.ToString(item.Latitude);
                        obj1.longitude = Convert.ToString(item.Longitude);
                        obj1.pendingVisit = Convert.ToString(item.TotalPendingCount);
                        obj1.street = street;
                        obj1.totalVisit = Convert.ToString(item.TotalRecordCount);
                        obj1.completedVisit = Convert.ToString(item.TotalCompletedCount);
                        obj1.skippedVisit = Convert.ToString(item.TotalSkipedCount);
                        // obj1.rtuVisit = Convert.ToString(item.TotalRTUCount);
                        obj1.lastVisitDatetime = Convert.ToString(item.VisitDate);
                        objVisitList.Add(obj1);
                    }
                    else
                    {
                        string reason = "";
                        if (!string.IsNullOrEmpty(item.Reason))
                        {
                            //reason = "<strong>" + item.Account + " | " + item.FormName + " | " + item.Reason + " | " + (string.IsNullOrEmpty(item.Comment) ? "-" : item.Comment)+"</strong></br>";
                            reason = "<strong>" + item.Account + " | " + item.FormName + "  </br>Reason- </br>" + item.Reason + " </br>" + (string.IsNullOrEmpty(item.Comment) ? "" : "Comment- </br>" + item.Comment) + "</strong></br>";
                            getD.street = getD.street.Remove((getD.street.Length - 6), 6) + reason + "</div>";
                        }
                    }
                }
            }
            else
            {

                objCompassEntities.Database.CommandTimeout = 0;


                List<PROC_ProjectStatus_AllData_Auditor_Result> resultData = new List<PROC_ProjectStatus_AllData_Auditor_Result>();

                DataTable dt = new DataTable();
                using (SqlConnection sqlcon = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("PROC_ProjectStatus_AllData_Auditor", sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ProjectId", projectID);
                        cmd.Parameters.Add("@FormTypeId", formTypeId);
                        cmd.CommandTimeout = 0;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                DateTime? dt1 = null;
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {

                        resultData.Add(new PROC_ProjectStatus_AllData_Auditor_Result
                        {
                            Account = Convert.ToString(item["Account"]),
                            AccountColorFlag = int.Parse(Convert.ToString(item["AccountColorFlag"])),
                            Comment = Convert.ToString(item["Comment"]),
                            Complete = int.Parse(Convert.ToString(item["Complete"])),
                            FormComplete = int.Parse(Convert.ToString(item["FormComplete"])),
                            FormCount = int.Parse(Convert.ToString(item["FormCount"])),
                            FormId = !string.IsNullOrEmpty(Convert.ToString(item["FormId"])) ? long.Parse(Convert.ToString(item["FormId"])) : 0,
                            FormName = Convert.ToString(item["FormName"]),
                            FormPending = int.Parse(Convert.ToString(item["FormPending"])),
                            //  FormRTU = int.Parse(Convert.ToString(item["FormRTU"])),
                            FormSkipped = int.Parse(Convert.ToString(item["FormSkipped"])),
                            ID = Convert.ToString(item["ID"]),
                            AuditorId = !string.IsNullOrEmpty(Convert.ToString(item["AuditorId"])) ? long.Parse(Convert.ToString(item["AuditorId"])) : 0,
                            // InstallerMapId = !string.IsNullOrEmpty(Convert.ToString(item["InstallerMapId"])) ? long.Parse(Convert.ToString(item["InstallerMapId"])) : 0,
                            AuditorName = Convert.ToString(item["AuditorName"]),
                            Latitude = Convert.ToString(item["Latitude"]),
                            LatLongColorFlag = int.Parse(Convert.ToString(item["LatLongColorFlag"])),
                            Longitude = Convert.ToString(item["Longitude"]),
                            Pending = int.Parse(Convert.ToString(item["Pending"])),
                            ProjectId = long.Parse(Convert.ToString(item["ProjectId"])),
                            Reason = Convert.ToString(item["Reason"]),
                            //RTU = int.Parse(Convert.ToString(item["RTU"])),
                            Skipped = int.Parse(Convert.ToString(item["Skipped"])),
                            street = Convert.ToString(item["street"]),
                            TotalCompletedCount = int.Parse(Convert.ToString(item["TotalCompletedCount"])),
                            TotalPendingCount = int.Parse(Convert.ToString(item["TotalPendingCount"])),
                            TotalRecordCount = int.Parse(Convert.ToString(item["TotalRecordCount"])),
                            // TotalRTUCount = int.Parse(Convert.ToString(item["TotalRTUCount"])),
                            TotalSkipedCount = int.Parse(Convert.ToString(item["TotalSkipedCount"])),
                            VisitDate = !string.IsNullOrEmpty(Convert.ToString(item["VisitDate"])) ? DateTime.Parse(Convert.ToString(item["VisitDate"])) : dt1,

                        });
                    }

                }





                foreach (var item in resultData)
                {
                    var getD = objVisitList.Where(o => o.latitude == Convert.ToString(item.Latitude) && o.longitude == Convert.ToString(item.Longitude)).FirstOrDefault();
                    if (getD == null)
                    {
                        var recCount = resultData.Where(o => o.Latitude == item.Latitude && o.Longitude == item.Longitude).Count();
                        string street = string.Empty;
                        if (recCount > 1)
                        {
                            var streetST = resultData.Where(o => o.Latitude == item.Latitude && o.Longitude == item.Longitude).Select(o => o.street).ToList();
                            street = StringExtensions.GetMostCommonSubstrings(streetST).FirstOrDefault();
                        }
                        else
                        {
                            street = item.street;
                        }

                        string reason = "";
                        if (!string.IsNullOrEmpty(item.Reason))
                        {
                            //reason = "<strong>" + item.Account + " | " + item.FormName + " | " + item.Reason + " | " + (string.IsNullOrEmpty(item.Comment) ? "-" : item.Comment) + "</strong></br>";
                            reason = "<strong>" + item.Account + " | " + item.FormName + "  </br>Reason- </br>" + item.Reason + " </br>" + (string.IsNullOrEmpty(item.Comment) ? "" : "Comment- </br>" + item.Comment) + "</strong></br>";
                        }
                        street = "<div class='scrollFixMAP'><strong>Auditor Name:" + item.AuditorName + "</strong></br><strong>Street:</strong> " + street + "</br>" +
                            //"<strong>Total Visits:" + Convert.ToString(item.TotalRecordCount) + "</strong></br>" +
                           "<strong>Pending:" + Convert.ToString(item.TotalPendingCount) + "</strong></br>" +
                           "<strong>Complete:" + Convert.ToString(item.TotalCompletedCount) + "</strong></br>" +
                           "<strong>Skip:" + Convert.ToString(item.TotalSkipedCount) + "</strong></br>";
                        // "<strong>RTU:" + Convert.ToString(item.TotalRTUCount) + "</strong></br>";
                        if (item.VisitDate != null)
                        {
                            street = street + "<strong>Last Visit Date:" + DateTime.Parse(item.VisitDate.ToString()).ToString("MMM-dd-yyyy hh:mm tt") + "</strong></br>";
                        }
                        street = street + reason + "</div>"; ;

                        addressVisitInfo obj1 = new addressVisitInfo();
                        obj1.colorFlag = int.Parse(Convert.ToString(item.LatLongColorFlag));
                        obj1.latitude = Convert.ToString(item.Latitude);
                        obj1.longitude = Convert.ToString(item.Longitude);
                        obj1.pendingVisit = Convert.ToString(item.TotalPendingCount);
                        obj1.street = street;
                        obj1.totalVisit = Convert.ToString(item.TotalRecordCount);
                        obj1.completedVisit = Convert.ToString(item.TotalCompletedCount);
                        obj1.skippedVisit = Convert.ToString(item.TotalSkipedCount);
                        obj1.lastVisitDatetime = Convert.ToString(item.VisitDate);
                        objVisitList.Add(obj1);
                    }
                    else
                    {
                        string reason = "";
                        if (!string.IsNullOrEmpty(item.Reason))
                        {
                            //reason = "<strong>" + item.Account + " | " + item.FormName + " | " + item.Reason + " | " + (string.IsNullOrEmpty(item.Comment) ? "-" : item.Comment) + "</strong></br>";
                            reason = "<strong>" + item.Account + " | " + item.FormName + "  </br>Reason- </br>" + item.Reason + " </br>" + (string.IsNullOrEmpty(item.Comment) ? "" : "Comment- </br>" + item.Comment) + "</strong></br>";

                            getD.street = getD.street.Remove((getD.street.Length - 6), 6) + reason + "</div>";
                        }

                    }
                }
            }
            #endregion

            return objVisitList;
        }
    }
}
