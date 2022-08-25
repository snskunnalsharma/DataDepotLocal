using System;
using System.Web;
using System.Linq;
//using System.Web.Mvc;
using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.SqlClient;
using System.Data;

namespace GarronT.CMS.Model.DAL
{
    /*
   * -------------------------------------------------------------------------------
   * Create By     : Aniket Jadhav 
   * Created On    : 07-Sep-2016 
   * Description   : DAL for Installer Map Data History
   *--------------------------------------------------------------------------------
   */
    public class InstallerMapHistoryDAL : IDisposable
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

        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(InstallerMapHistoryDAL));
        #endregion
        public InstallerMapHistoryDAL()
        {

        }

        /// <summary>
        /// Get InstallerMapData History
        /// </summary>
        /// <returns></returns>
        public List<InstallerMapHistoryVM> GetInstallerMapHistoryList(long projectID, long installerID,string SkipReason, string FromDate, string ToDate)
        {       
            List<InstallerMapHistoryVM> installerHistoryList = new List<InstallerMapHistoryVM>();

            try
            {
                CompassEntities _dbcontext = new CompassEntities();

                SqlConnection sqlConnection1 = new SqlConnection(_dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                DateTime? fromdate = Convert.ToDateTime(FromDate);
                DateTime? todate = Convert.ToDateTime(ToDate);

                cmd.CommandText = "PROC_ProjectHistory_GetRecords";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.AddWithValue("@projectId", projectID);
                cmd.Parameters.AddWithValue("@SkipReason", SkipReason);
                cmd.Parameters.AddWithValue("@fromDate", fromdate);
                cmd.Parameters.AddWithValue("@toDate", todate);
                sqlConnection1.Open();

                reader = cmd.ExecuteReader();
                DateTime? sampleDt = null;
                while (reader.Read())
                {
                    installerHistoryList.Add(new InstallerMapHistoryVM
                    {
                        ProjectFieldDataId = long.Parse(reader["ProjectFieldDataId"].ToString()),
                        ID = long.Parse(reader["ID"].ToString()),
                        InstallerName = Convert.ToString(reader["InstallerName"]),
                        InstallerId = long.Parse(reader["InstallerId"].ToString()),
                        projectname = Convert.ToString(reader["projectname"]),
                        ProjectId = long.Parse(Convert.ToString(reader["FKProjectId"])),
                        Street = Convert.ToString(reader["Street"]),
                        FormName = Convert.ToString(reader["FormName"]),
                        Account = Convert.ToString(reader["Account"]),
                        Cycle = Convert.ToString(reader["Cycle"]),
                        Route = Convert.ToString(reader["Route"]),
                        SkippedReason = Convert.ToString(reader["SkippedReason"]),
                        SkipComment = Convert.ToString(reader["SkipComment"]),
                        VisitedOn = !string.IsNullOrEmpty(Convert.ToString(reader["visitdatetime"])) ? DateTime.Parse(Convert.ToString(reader["visitdatetime"])) : sampleDt,
                        VisitedDate = !string.IsNullOrEmpty(Convert.ToString(reader["visitdatetime"])) ? DateTime.Parse(Convert.ToString(reader["visitdatetime"])).ToString("MMM-dd-yyyy hh:mm tt") : ""
                    });

                }

                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();
                if (projectID > 0 && installerID > 0)
                {
                    #region --Get Records--

                    installerHistoryList = installerHistoryList.Where(a => a.ProjectId == projectID && a.InstallerId == installerID).ToList();
                    #endregion
                }
                else if (projectID > 0 && installerID == 0)
                {
                    #region --Get Records--
                    //   installerHistoryList = _dbcontext.PROC_Get_InstallerMapDataHistory().Where(a => a.projectid == projectID).Select(a => new InstallerMapHistoryVM
                    installerHistoryList = installerHistoryList.Where(a => a.ProjectId == projectID).ToList();
                    #endregion }
                }
                else
                {
                    #region --Get Records--
                    //installerHistoryList = _dbcontext.PROC_Get_InstallerMapDataHistory().Select(a => new InstallerMapHistoryVM
                    //{

                    //    ID = a.ID,
                    //    InstallerName = a.InstallerName,
                    //    InstallerId = a.InstallerId,
                    //    projectname = a.projectname,
                    //    ProjectId = a.projectid,
                    //    Street = a.street,
                    //    FormName = a.FormName,
                    //    Account = a.Account,
                    //    Cycle = a.Cycle,
                    //    Route = a.Route,
                    //    SkippedReason = a.SkippedReason,
                    //    SkipComment = a.SkipComment,
                    //    VisitedOn = a.visitdatetime,
                    //    VisitedDate = (a.visitdatetime != null ? a.visitdatetime.Value.ToString("MMM-dd-yyyy hh:mm tt") : "")
                    //}).ToList();
                    #endregion }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw;
            }
            return installerHistoryList;
        }


        //Created by Sominath to get Skipped  records by FK_UploadedDataID and Project ID on Date 25/11/2016
        public List<InstallerMapHistoryVM> GetAllInstallerMapHistoryList(long projectID, long installerID)
        {
            List<InstallerMapHistoryVM> installerHistoryList = new List<InstallerMapHistoryVM>();

            try
            {
                CompassEntities _dbcontext = new CompassEntities();

                SqlConnection sqlConnection1 = new SqlConnection(_dbcontext.Database.Connection.ConnectionString);
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                //cmd.CommandText = "PROC_ProjectHistory_GetRecords";
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Connection = sqlConnection1;
                //cmd.Parameters.Add("@projectId", projectID);

                cmd.CommandText = "PROC_ProjectHistory_GetAllRecords";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnection1;
                cmd.Parameters.Add("@projectId", projectID);


                sqlConnection1.Open();

                reader = cmd.ExecuteReader();
                DateTime? sampleDt = null;
                while (reader.Read())
                {
                    installerHistoryList.Add(new InstallerMapHistoryVM
                    {
                        ProjectFieldDataId = long.Parse(reader["ProjectFieldDataId"].ToString()),
                        ID = long.Parse(reader["ID"].ToString()),
                        InstallerName = Convert.ToString(reader["InstallerName"]),
                        InstallerId = long.Parse(reader["InstallerId"].ToString()),
                        projectname = Convert.ToString(reader["projectname"]),
                        ProjectId = long.Parse(Convert.ToString(reader["FKProjectId"])),
                        Street = Convert.ToString(reader["Street"]),
                        FormName = Convert.ToString(reader["FormName"]),
                        FormId = long.Parse(reader["FormId"].ToString()),
                        Account = Convert.ToString(reader["Account"]),
                        Cycle = Convert.ToString(reader["Cycle"]),
                        Route = Convert.ToString(reader["Route"]),
                        SkippedReason = Convert.ToString(reader["SkippedReason"]),
                        SkipComment = Convert.ToString(reader["SkipComment"]),
                        VisitedOn = !string.IsNullOrEmpty(Convert.ToString(reader["visitdatetime"])) ? DateTime.Parse(Convert.ToString(reader["visitdatetime"])) : sampleDt,
                        VisitedDate = !string.IsNullOrEmpty(Convert.ToString(reader["visitdatetime"])) ? DateTime.Parse(Convert.ToString(reader["visitdatetime"])).ToString("MMM-dd-yyyy hh:mm tt") : ""
                    });

                }

                sqlConnection1.Close();
                reader.Close();
                reader.Dispose();
                cmd.Dispose();
                sqlConnection1.Dispose();
                if (projectID > 0 && installerID > 0)
                {
                    #region --Get Records--

                    installerHistoryList = installerHistoryList.Where(a => a.ProjectId == projectID && a.InstallerId == installerID).ToList();
                    #endregion
                }
                else if (projectID > 0 && installerID == 0)
                {
                    #region --Get Records--
                    //   installerHistoryList = _dbcontext.PROC_Get_InstallerMapDataHistory().Where(a => a.projectid == projectID).Select(a => new InstallerMapHistoryVM
                    installerHistoryList = installerHistoryList.Where(a => a.ProjectId == projectID).ToList();
                    #endregion }
                }
                else
                {
                    #region --Get Records--
                    //installerHistoryList = _dbcontext.PROC_Get_InstallerMapDataHistory().Select(a => new InstallerMapHistoryVM
                    //{

                    //    ID = a.ID,
                    //    InstallerName = a.InstallerName,
                    //    InstallerId = a.InstallerId,
                    //    projectname = a.projectname,
                    //    ProjectId = a.projectid,
                    //    Street = a.street,
                    //    FormName = a.FormName,
                    //    Account = a.Account,
                    //    Cycle = a.Cycle,
                    //    Route = a.Route,
                    //    SkippedReason = a.SkippedReason,
                    //    SkipComment = a.SkipComment,
                    //    VisitedOn = a.visitdatetime,
                    //    VisitedDate = (a.visitdatetime != null ? a.visitdatetime.Value.ToString("MMM-dd-yyyy hh:mm tt") : "")
                    //}).ToList();
                    #endregion }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw;
            }
            return installerHistoryList;
        }
        //Created By Sominath on Date 25/11/2016
        // Method used to get the skiprtu model along with image paths
        public List<SkipRtuAttachmentModel> getSkipRtuImages(long projectFieldDataId)
        {

            CompassEntities dbcontext = new CompassEntities();



            List<SkipRtuAttachmentModel> paths = new List<SkipRtuAttachmentModel>();

            paths = dbcontext.TblProjectFieldDataAttachments.AsNoTracking()
                     .Where(o => o.ProjectFieldDataId == projectFieldDataId
                     ).Select(o => new SkipRtuAttachmentModel
                     {
                         id = o.ID,
                         IsAudioVideoImage = o.IsAudioVideoImage,
                         strFilePath = o.strFilePath
                     }).OrderBy(o => o.IsAudioVideoImage).ToList();

            return paths;
        }

        /// <summary>
        /// Get ProjectList by Project Status
        /// </summary>
        /// <param name="projectStatus"></param>
        /// <returns></returns>
        public List<ProjectModel> GetProjectName(string projectStatus)
        {
            CompassEntities CompassEntities = new CompassEntities();
            List<ProjectModel> projectList = new List<ProjectModel>();
            try
            {
                projectList = CompassEntities.STP_GetProjectCityState().Where(a => a.Active == 1 && a.ProjectStatus == projectStatus).Select(o =>
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



        public long GetProjectIdForCurrent(long uploadId)
        {
            CompassEntities CompassEntities = new CompassEntities();
            long proId = 0;
            try
            {
                // var a = CompassEntities.TblProjectFieldDataHistories.Where(o => o.FK_UploadedExcelData_Id == uploadId).FirstOrDefault();

                var a = CompassEntities.TblProjectFieldDatas.Where(o => o.ID == uploadId).FirstOrDefault();
                var b = CompassEntities.UploadedExcelDatas.Where(o => o.Id == a.FK_UploadedExcelData_Id).FirstOrDefault();
                proId = b.FKProjectId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return proId;
        }

        public string GetFormName(long formId)
        {
            CompassEntities CompassEntities = new CompassEntities();
            string proName = "";
            try
            {
                var a = CompassEntities.tblFormMasters.Where(o => o.FormId == formId).Select(o => o.FormName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return proName;
        }






        /// <summary>
        /// Get Installer List by ProjectID
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public List<UsersSmallModel> GeInstallerListbyProjectID(long ProjectId)
        {
            CompassEntities CompassEntities = new CompassEntities();
            List<UsersSmallModel> installerList = new List<UsersSmallModel>();
            try
            {
                var installerIdList = (from post in CompassEntities.UploadedExcelDatas
                                       join meta in CompassEntities.TblProjectFieldDataHistories on post.Id equals meta.FK_UploadedExcelData_Id
                                       where meta.Active == 1 && post.FKProjectId == ProjectId
                                       select new { id = meta.InstallerId }).Distinct().ToList();

                if (installerIdList != null)
                {
                    foreach (var item in installerIdList)
                    {

                        installerList.Add(new UsersSmallModel
                                                {
                                                    UserId = item.id,
                                                    UserName = CompassEntities.tblUsers.Where(o => o.UserID == item.id).Select(m => m.FirstName).FirstOrDefault()
                                                });
                    }
                }

                installerList = installerList.OrderBy(a => a.UserName).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return installerList;
        }
    }
}
