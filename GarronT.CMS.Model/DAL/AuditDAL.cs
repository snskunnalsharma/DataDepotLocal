using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.DAL
{
    public class AuditDAL : IDisposable
    {

        bool disposed;
        protected virtual void Dispose(bool disposing)
        {
            //if (!disposed)
            //{
            //    if (disposing)
            //    {
            //        //dispose managed ressources

            //    }
            //}
            //dispose unmanaged ressources
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public long GetAuditRecordProjectId(int auditId)
        {
            long a = 0;
            CompassEntities objCompassEntities = new CompassEntities();

            a = objCompassEntities.AuditMasters.Where(o => o.Id == auditId).FirstOrDefault().FKProjectId;
            objCompassEntities.Dispose();
            return a;
        }


        /// <summary>
        /// Returns audit project list
        /// </summary>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public List<AuditBO> GetAuditProjectList(int isActive)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            var obj = objCompassEntities.PROC_AuditMaster().Where(o => o.Active == isActive).
                Select(o => new AuditBO
                {
                    Id = o.Id,
                    ProjectName = o.ProjectName + ", " + o.CityName + ", " + o.StateName,
                    StateName = o.StateName,
                    CityName = o.CityName,
                    AuditEndDate = o.AuditEndDate.ToString("MMM-dd-yyyy"),
                    AuditStartDate = o.AuditStartDate.ToString("MMM-dd-yyyy"),
                    ClientName = o.ClientName,
                    Utilitytype = o.Utilitytype
                }).ToList();

            return obj;

        }



        /// <summary>
        /// Activate or deactivate the record.
        /// </summary>
        /// <param name="auditId"></param>
        /// <param name="isActive"></param>
        /// <param name="UserId"></param>
        /// <param name="resultMessage"></param>
        /// <returns></returns>
        public bool ActiveDeactivateProject(long auditId, bool isActive, int UserId, out string resultMessage)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            var obj = objCompassEntities.AuditMasters.Where(o => o.Id == auditId).FirstOrDefault();

            obj.Active = isActive == true ? 1 : 0;
            obj.ModifiedBy = UserId;
            obj.ModifiedOn = new CommonFunctions().ServerDate();
            objCompassEntities.SaveChanges();

            resultMessage = isActive == true ? "Record activated successfully...!" : "Record deactivated successfully...!";
            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditId"></param>
        /// <param name="isActive"></param>
        /// <param name="UserId"></param>
        /// <param name="resultMessage"></param>
        /// <returns></returns>
        public bool CreateUpdate(long auditId, bool isActive, int UserId, out string resultMessage)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            var obj = objCompassEntities.AuditMasters.Where(o => o.Id == auditId).FirstOrDefault();

            obj.Active = isActive == true ? 1 : 0;
            obj.ModifiedBy = UserId;
            obj.ModifiedOn = new CommonFunctions().ServerDate();
            objCompassEntities.SaveChanges();

            resultMessage = isActive == true ? "Record activated successfully...!" : "Record deactivated successfully...!";
            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="UtilityType"></param>
        /// <returns></returns>
        public List<ProjectModel> GetProjectName(string UtilityType)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            string proStatus = "";
            //if (ProjectStatus == "2")
            //{
            //    proStatus = "Complete";
            //}
            //else
            //{
            //    proStatus = "Active";
            //}
            List<ProjectModel> data = objCompassEntities.STP_GetProjectCityState().Where(a => a.Active == 1 && a.Utilitytype == UtilityType
                && a.ProjectStatus == "Active").Select(o =>
                                       new ProjectModel
                                       {
                                           ProjectId = o.ProjectId,
                                           ProjectCityState = o.ProjectCityName
                                       }).OrderBy(a => a.ProjectCityState).ToList();

            var currentAuditProjectList = objCompassEntities.AuditMasters.ToList();

            foreach (var item in currentAuditProjectList)
            {
                var a = data.Where(o => o.ProjectId == item.FKProjectId).FirstOrDefault();
                data.Remove(a);
            }

            return data;


        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<UsersSmallModel> GetUserList()
        {
            CompassEntities objCompassEntities = new CompassEntities();

            List<UsersSmallModel> data = objCompassEntities.VW_AuditorList.Select(o => new UsersSmallModel
            {
                UserId = o.UserID,
                UserName = o.FirstName
            }).OrderBy(o => o.UserName).ToList();
            return data;
        }

        public List<long> GetCurrentUserProjectList(long ProjectId)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            List<long> data = objCompassEntities.TblProjectInstallers.AsNoTracking().
                Where(o => o.ProjectID == ProjectId).Select(o => o.tblUsers_UserID).ToList();
            return data;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="resultMessage"></param>
        /// <returns></returns>
        public bool GetProjectDetails(long projectId, out string resultMessage)
        {
            bool status = false;
            resultMessage = "";
            CompassEntities objCompassEntities = new CompassEntities();

            var data = objCompassEntities.AuditMasters.Where(o => o.FKProjectId == projectId).FirstOrDefault();

            if (data != null && data.Active == 1)
            {
                status = true;
                resultMessage = "Record already present in active record list. Operation cancelled.";
            }
            else if (data != null && data.Active == 0)
            {
                status = true;
                resultMessage = "Record already present in deactive record list. Operation cancelled.";
            }

            return status;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ProjectSmallModel GetProjectDetails(long projectId)
        {
            ProjectSmallModel status = new ProjectSmallModel();

            CompassEntities objCompassEntities = new CompassEntities();

            var data = objCompassEntities.tblProjects.Where(o => o.ProjectId == projectId).FirstOrDefault();
            //var proDetails = objCompassEntities.tblProjects.Where(o => o.ProjectId == projectId).FirstOrDefault();
            var customerDetails = objCompassEntities.tblClients.Where(o => o.ClientId == data.ClientId).FirstOrDefault();


            // List<UsersSmallModel> list = GetUserList();

            //List<long> data1 = objCompassEntities.TblProjectInstallers.AsNoTracking().
            // Where(o => o.ProjectID == projectId).Select(o => o.tblUsers_UserID).ToList();
            //list = list.Where(o=>data1.Contains(o));

            status = new ProjectSmallModel
            {
                stringFromDate = DateTime.Parse(data.FromDate.ToString()).ToString("MMM-dd-yyyy"),
                stringToDate = DateTime.Parse(data.ToDate.ToString()).ToString("MMM-dd-yyyy"),
                CustomerName = customerDetails.ClientName
            };

            return status;
        }


        public bool ProjectAuditReadyValidate(long projectId, out string Message)
        {

            bool result = true;
            Message = "";
            CompassEntities objCompassEntities = new CompassEntities();

            var data = objCompassEntities.tblFormMasters.Where(o => o.ProjectId == projectId && o.FormTypeID == 1).FirstOrDefault();

            if (data == null)
            {
                result = false;
                Message = "Please setup the audit form before setting the audit project.";
            }

            //if (result)
            //{
            //    var proDetails = objCompassEntities.tblProjects.Where(o => o.ProjectId == projectId).FirstOrDefault();
            //    var customerDetails = objCompassEntities.tblClients.Where(o => o.ClientId == proDetails.ClientId).FirstOrDefault();

            //    var countAuditSettings = objCompassEntities.CustomerAuditSettings.Where(o => o.Active == 1 && o.FK_CustomerId == customerDetails.ClientId).Count();

            //    if (countAuditSettings == 0)
            //    {
            //        result = false;
            //        Message = "Please setup the audit settings in customer master for the current customer first & then try to create audit project.";
            //    }
            // }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditId"></param>
        /// <returns></returns>
        public ProjectSmallModel GetProjectFromAuditId(long auditId)
        {
            ProjectSmallModel status = new ProjectSmallModel();

            CompassEntities objCompassEntities = new CompassEntities();
            var data1 = objCompassEntities.AuditMasters.Where(o => o.Id == auditId).FirstOrDefault();

            var data = objCompassEntities.tblProjects.Where(o => o.ProjectId == data1.FKProjectId).FirstOrDefault();

            status = new ProjectSmallModel
            {
                stringFromDate = DateTime.Parse(data.FromDate.ToString()).ToString("MMM-dd-yyyy"),
                stringToDate = DateTime.Parse(data.ToDate.ToString()).ToString("MMM-dd-yyyy")
            };

            return status;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="userId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="auditorList"></param>
        /// <returns></returns>
        //public bool SaveData(long projectId, int userId, string startDate, string endDate, string[] auditorList)
        //{
        public bool SaveData(AuditCreateBO objAuditCreateBO, int userId)
        {

            try
            {
                bool result = false;
                CompassEntities objCompassEntities = new CompassEntities();

                var data = objCompassEntities.AuditMasters.Where(o => o.FKProjectId == objAuditCreateBO.projectId).FirstOrDefault();

                if (data == null)
                {
                    #region insert


                    data = new AuditMaster();
                    data.Active = 1;
                    data.AuditEndDate = DateTime.Parse(objAuditCreateBO.endDate);
                    data.AuditStartDate = DateTime.Parse(objAuditCreateBO.startDate);
                    data.CreatedBy = userId;
                    data.CreatedOn = new CommonFunctions().ServerDate();
                    data.FKProjectId = objAuditCreateBO.projectId;

                    objCompassEntities.AuditMasters.Add(data);
                    objCompassEntities.SaveChanges();


                    foreach (var item in objAuditCreateBO.auditorList)
                    {
                        AuditorMap objAuditorMap = new AuditorMap();
                        objAuditorMap.Active = 1;
                        objAuditorMap.CreatedBy = userId;
                        objAuditorMap.CreatedOn = new CommonFunctions().ServerDate();
                        objAuditorMap.FK_AuditId = data.Id;
                        objAuditorMap.FK_UserId = long.Parse(item);
                        objCompassEntities.AuditorMaps.Add(objAuditorMap);
                    }

                    objCompassEntities.SaveChanges();



                    var project = objCompassEntities.tblProjects.Where(o => o.ProjectId == objAuditCreateBO.projectId).FirstOrDefault();
                    var objData = objAuditCreateBO.objList.Where(o => o.Active == 1).OrderBy(o => o.InstallerId).ThenBy(o => o.AuditStartDate).ToList();
                    foreach (var item1 in objData)
                    {
                        AuditInstallerPercentage obj = new AuditInstallerPercentage();
                        obj.Active = 1;
                        obj.CreatedBy = userId;
                        obj.CreatedOn = new CommonFunctions().ServerDate();
                        obj.AuditPercentage = item1.AuditPercentage;
                        obj.FK_CustomerId = (long)project.ClientId;
                        obj.FK_InstallerId = item1.InstallerId;
                        obj.FK_ProjectId = objAuditCreateBO.projectId;
                        obj.EndDay = item1.AuditEndDate;
                        obj.StartDay = item1.AuditStartDate;

                        objCompassEntities.AuditInstallerPercentages.Add(obj);
                    }
                    objCompassEntities.SaveChanges();


                    if (objAuditCreateBO.objAuditProjectFailBOList != null && objAuditCreateBO.objAuditProjectFailBOList.Count > 0)
                    {
                        foreach (var item1 in objAuditCreateBO.objAuditProjectFailBOList)
                        {
                            AuditFailPercentage obj = new AuditFailPercentage();
                            obj.Active = 1;
                            obj.CreatedBy = userId;
                            obj.CreatedOn = new CommonFunctions().ServerDate();
                            obj.FailPercentageStart = item1.FailPercentageFrom;
                            obj.FailPercentageEnd = item1.FailPercentageTo;
                            obj.FKAuditProjectId = data.Id;
                            obj.IncreaseByDays = item1.IncreaseByDays;
                            objCompassEntities.AuditFailPercentages.Add(obj);
                        }
                        objCompassEntities.SaveChanges();
                    }


                    result = true;
                    #endregion
                }

                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="auditorList"></param>
        /// <returns></returns>
        //public bool UpdateData(long auditId, string startDate, string endDate, string[] auditorList)
        //{

        //    bool result = false;
        //    CompassEntities objCompassEntities = new CompassEntities();

        //    var data = objCompassEntities.AuditMasters.Where(o => o.Id == auditId).FirstOrDefault();


        //    #region update


        //    data.AuditEndDate = DateTime.Parse(endDate);
        //    data.AuditStartDate = DateTime.Parse(startDate);
        //    data.ModifiedBy = 1;
        //    data.ModifiedOn = new CommonFunctions().ServerDate();
        //    objCompassEntities.SaveChanges();


        //    var GetCurrentList = objCompassEntities.AuditorMaps.Where(o => o.FK_AuditId == data.Id).ToList();

        //    foreach (var item in GetCurrentList)
        //    {
        //        item.Active = 0;
        //        item.ModifiedBy = 1;
        //        item.ModifiedOn = new CommonFunctions().ServerDate();
        //        objCompassEntities.SaveChanges();
        //    }

        //    foreach (var item in auditorList)
        //    {
        //        long userId = long.Parse(item);
        //        var objAuditorMap = objCompassEntities.AuditorMaps.Where(o => o.FK_AuditId == data.Id && o.FK_UserId == userId).FirstOrDefault();

        //        if (objAuditorMap == null)
        //        {
        //            objAuditorMap = new AuditorMap();
        //            objAuditorMap.Active = 1;
        //            objAuditorMap.CreatedBy = 1;
        //            objAuditorMap.CreatedOn = new CommonFunctions().ServerDate();
        //            objAuditorMap.FK_AuditId = data.Id;
        //            objAuditorMap.FK_UserId = userId;
        //            objCompassEntities.AuditorMaps.Add(objAuditorMap);
        //            objCompassEntities.SaveChanges();
        //        }
        //        else
        //        {
        //            objAuditorMap.Active = 1;
        //            objAuditorMap.ModifiedBy = 1;
        //            objAuditorMap.ModifiedOn = new CommonFunctions().ServerDate();
        //            objCompassEntities.SaveChanges();
        //        }
        //    }


        //    var objCurrentDeactiveLists = objCompassEntities.AuditorMaps.Where(o => o.FK_AuditId == data.Id && o.Active == 0).ToList();



        //    #endregion

        //    result = true;

        //    return result;
        //}

        public bool UpdateData(AuditCreateBO objAuditCreateBO, int userId, out string strMessage)
        {

            try
            {


                strMessage = "";
                bool result = false;
                CompassEntities objCompassEntities = new CompassEntities();

                var data = objCompassEntities.AuditMasters.Where(o => o.Id == objAuditCreateBO.auditProjectId).FirstOrDefault();

                var project = objCompassEntities.tblProjects.Where(o => o.ProjectId == data.FKProjectId).FirstOrDefault();


                /*First check if any auditor removed & it holds the visit.*/

                var AuditorRemoveSuccess = true;
                long FailUserId = 0;

                var CurrentAuditorList = objCompassEntities.AuditorMaps.Where(o => o.FK_AuditId == data.Id).Select(o => o.FK_UserId).ToList();

                foreach (var item in CurrentAuditorList)
                {
                    var a = objAuditCreateBO.auditorList.Where(o => o == item.ToString()).FirstOrDefault();


                    if (a == null)
                    {
                        var Obj = objCompassEntities.ProjectAuditAllocations.Where(o => o.Fk_ProjectId == project.ProjectId && o.FK_AuditorId == item && o.Active == 1).FirstOrDefault();

                        if (Obj != null)
                        {
                            FailUserId = item;
                            AuditorRemoveSuccess = false;
                            break;
                        }
                    }

                }



                if (AuditorRemoveSuccess == false)
                {
                    var ab = objCompassEntities.tblUsers.Where(o => o.UserID == FailUserId).FirstOrDefault();
                    strMessage = "Update failed. Can not remove the auditor " + ab.FirstName + " as it has audit visit assigned";
                }
                else
                {

                    #region update


                    data.AuditEndDate = DateTime.Parse(objAuditCreateBO.endDate);
                    data.AuditStartDate = DateTime.Parse(objAuditCreateBO.startDate);
                    data.ModifiedBy = 1;
                    data.ModifiedOn = new CommonFunctions().ServerDate();
                    objCompassEntities.SaveChanges();


                    var GetCurrentList = objCompassEntities.AuditorMaps.Where(o => o.FK_AuditId == data.Id).ToList();

                    //foreach (var item in GetCurrentList)
                    //{
                    //    item.Active = 0;
                    //    item.ModifiedBy = 1;
                    //    item.ModifiedOn = new CommonFunctions().ServerDate();
                    //    objCompassEntities.SaveChanges();
                    //}

                    foreach (var item in objAuditCreateBO.auditorList)
                    {
                        long auditorId = long.Parse(item);
                        var objAuditorMap = objCompassEntities.AuditorMaps.Where(o => o.FK_AuditId == data.Id && o.FK_UserId == auditorId).FirstOrDefault();

                        if (objAuditorMap == null)
                        {
                            objAuditorMap = new AuditorMap();
                            objAuditorMap.Active = 1;
                            objAuditorMap.CreatedBy = userId;
                            objAuditorMap.CreatedOn = new CommonFunctions().ServerDate();
                            objAuditorMap.FK_AuditId = data.Id;
                            objAuditorMap.FK_UserId = auditorId;
                            objCompassEntities.AuditorMaps.Add(objAuditorMap);
                            objCompassEntities.SaveChanges();
                        }
                        else
                        {
                            objAuditorMap.Active = 1;
                            objAuditorMap.ModifiedBy = userId;
                            objAuditorMap.ModifiedOn = new CommonFunctions().ServerDate();
                            objCompassEntities.SaveChanges();
                        }
                    }


                    foreach (var item in objAuditCreateBO.objList)
                    {
                        if (item.Id != null && item.Id > 0)
                        {

                            var objAuditInstallationcycle = objCompassEntities.AuditInstallerPercentages.Where(o => o.Id == item.Id).FirstOrDefault();
                            if (item.Active == 1)
                            {
                                objAuditInstallationcycle.StartDay = item.AuditStartDate;
                                objAuditInstallationcycle.EndDay = item.AuditEndDate;
                                objAuditInstallationcycle.ModifiedBy = userId;
                                objAuditInstallationcycle.ModifiedOn = DateTime.Now;
                                objAuditInstallationcycle.AuditPercentage = item.AuditPercentage;
                                objCompassEntities.SaveChanges();
                            }
                            else
                            {
                                objAuditInstallationcycle.Active = 0;
                                objAuditInstallationcycle.ModifiedBy = userId;
                                objAuditInstallationcycle.ModifiedOn = DateTime.Now;
                                objCompassEntities.SaveChanges();
                            }
                        }
                        else
                        {
                            var objAuditInstallationcycle = new AuditInstallerPercentage();
                            objAuditInstallationcycle.FK_ProjectId = data.FKProjectId;
                            objAuditInstallationcycle.FK_InstallerId = item.InstallerId;
                            objAuditInstallationcycle.FK_CustomerId = project.ClientId.Value;
                            objAuditInstallationcycle.StartDay = item.AuditStartDate;
                            objAuditInstallationcycle.EndDay = item.AuditEndDate;
                            objAuditInstallationcycle.AuditPercentage = item.AuditPercentage;
                            objAuditInstallationcycle.Active = 1;
                            objAuditInstallationcycle.CreatedBy = userId;
                            objAuditInstallationcycle.CreatedOn = DateTime.Now;
                            objCompassEntities.AuditInstallerPercentages.Add(objAuditInstallationcycle);
                            objCompassEntities.SaveChanges();
                        }
                    }



                    if (objAuditCreateBO.objAuditProjectFailBOList != null && objAuditCreateBO.objAuditProjectFailBOList.Count > 0)
                    {
                        foreach (var item1 in objAuditCreateBO.objAuditProjectFailBOList)
                        {
                            if (item1.Id == 0)
                            {
                                if (item1.Active == 1)
                                {
                                    AuditFailPercentage obj = new AuditFailPercentage();
                                    obj.Active = 1;
                                    obj.CreatedBy = userId;
                                    obj.CreatedOn = new CommonFunctions().ServerDate();
                                    obj.FailPercentageStart = item1.FailPercentageFrom;
                                    obj.FailPercentageEnd = item1.FailPercentageTo;
                                    obj.FKAuditProjectId = data.Id;
                                    obj.IncreaseByDays = item1.IncreaseByDays;
                                    objCompassEntities.AuditFailPercentages.Add(obj);
                                    objCompassEntities.SaveChanges();
                                }
                            }
                            else
                            {

                                var objAuditFailcycle = objCompassEntities.AuditFailPercentages.Where(o => o.Id == item1.Id).FirstOrDefault();

                                if (item1.Active == 0)
                                {
                                    objAuditFailcycle.Active = 0;
                                    objAuditFailcycle.ModifiedBy = userId;
                                    objAuditFailcycle.ModifiedOn = new CommonFunctions().ServerDate();
                                }
                                else
                                {
                                    objAuditFailcycle.FailPercentageStart = item1.FailPercentageFrom;
                                    objAuditFailcycle.FailPercentageEnd = item1.FailPercentageTo;
                                    objAuditFailcycle.IncreaseByDays = item1.IncreaseByDays;
                                    objAuditFailcycle.ModifiedBy = userId;
                                    objAuditFailcycle.ModifiedOn = new CommonFunctions().ServerDate();
                                }

                                objCompassEntities.SaveChanges();
                            }


                        }
                    }






                    // var objCurrentDeactiveLists = objCompassEntities.AuditorMaps.Where(o => o.FK_AuditId == data.Id && o.Active == 0).ToList();



                    #endregion

                    result = true;


                }
                return result;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ProjectSmallModel GetProjectforUpdate(long auditId)
        {
            ProjectSmallModel status = new ProjectSmallModel();

            CompassEntities objCompassEntities = new CompassEntities();


            var data = objCompassEntities.AuditMasters.Where(o => o.Id == auditId).FirstOrDefault();

            var data1 = objCompassEntities.STP_GetProjectCityState().Where(a => a.Active == 1 && a.ProjectId == data.FKProjectId).FirstOrDefault();
            var proDetails = objCompassEntities.tblProjects.Where(o => o.ProjectId == data1.ProjectId).FirstOrDefault();
            var customerDetails = objCompassEntities.tblClients.Where(o => o.ClientId == proDetails.ClientId).FirstOrDefault();
            status = new ProjectSmallModel
            {
                stringFromDate = DateTime.Parse(data.AuditStartDate.ToString()).ToString("MMM-dd-yyyy"),
                stringToDate = DateTime.Parse(data.AuditEndDate.ToString()).ToString("MMM-dd-yyyy"),
                ProjectName = data1.ProjectCityName,
                Utility = data1.Utilitytype,
                CustomerName = customerDetails.ClientName,
                ProjectId = data1.ProjectId
            };
            status.objPresentList = GetUserList();
            status.objLongList = GetProjectUserRecords(auditId);

            var Data2 = objCompassEntities.AuditInstallerPercentages.Where(o => o.Active == 1 && o.FK_ProjectId == data.FKProjectId).ToList();

            status.objList = Data2.Select(o =>
                 new AuditCycleBO
                 {
                     AuditFailed = o.IsFailurePercentageApplied == 0 ? "No" : "Yes",
                     AuditPercentage = o.AuditPercentage,
                     AuditStartDate = o.StartDay,
                     AuditEndDate = o.EndDay,
                     Id = o.Id,
                     InstallerName = objCompassEntities.tblUsers.Where(x => x.UserID == o.FK_InstallerId).Select(x => x.FirstName).FirstOrDefault(),
                     InstallerId = o.FK_InstallerId,
                     Active = 1
                 }).ToList().OrderBy(o => o.InstallerName).ThenBy(o => o.AuditStartDate).ToList();



            var Data3 = objCompassEntities.AuditFailPercentages.Where(o => o.Active == 1 && o.FKAuditProjectId == data.Id).ToList();

            status.objAuditProjectFailBOList = Data3.Select(o =>
                 new AuditProjectFailBO
                 {
                     FailPercentageFrom = o.FailPercentageStart,
                     FailPercentageTo = o.FailPercentageEnd,
                     IncreaseByDays = o.IncreaseByDays,
                     Id = o.Id,
                     Active = 1
                 }).ToList().OrderBy(o => o.FailPercentageFrom).ToList();

            return status;
        }


        public List<long> GetProjectUserRecords(long auditId)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            var data = objCompassEntities.AuditorMaps.Where(o => o.FK_AuditId == auditId && o.Active == 1).Select(o => o.FK_UserId).ToList();
            return data;
        }



        public List<InstallerAuditCycleBO> GetInstallerAuditCycleData(long projectId)
        {

            CompassEntities objCompassEntities = new CompassEntities();

            var Data = objCompassEntities.AuditInstallerPercentages.Where(o => o.Active == 1 && o.FK_ProjectId == projectId).ToList();

            List<InstallerAuditCycleBO> objList = Data.Select(o =>
                new InstallerAuditCycleBO
                {
                    AuditFailReached = o.IsFailurePercentageApplied == 0 ? "No" : "Yes",
                    AuditPercentage = o.AuditPercentage,
                    EndDay = o.EndDay,
                    Id = o.Id,
                    InstallerName = objCompassEntities.tblUsers.Where(x => x.UserID == o.FK_InstallerId).Select(x => x.FirstName).FirstOrDefault(),
                    StartDay = o.StartDay
                }).ToList().OrderBy(o => o.InstallerName).ThenBy(o => o.StartDay).ToList();
            return objList;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditId"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool ActiveDeactivateRecord(long auditId, bool val)
        {
            bool result = false;
            CompassEntities objCompassEntities = new CompassEntities();

            var Data = objCompassEntities.AuditMasters.Where(o => o.Id == auditId).FirstOrDefault();
            Data.Active = val == true ? 1 : 0;
            objCompassEntities.SaveChanges();
            result = true;
            return result;
        }




        public InstallerAuditCycleBO GetCustData(long auditCycleId)
        {

            CompassEntities objCompassEntities = new CompassEntities();

            var Data = objCompassEntities.AuditInstallerPercentages.Where(o => o.Active == 1 && o.Id == auditCycleId).FirstOrDefault();

            var result = new InstallerAuditCycleBO
                  {
                      AuditFailReached = Data.IsFailurePercentageApplied == 0 ? "No" : "Yes",
                      AuditPercentage = Data.AuditPercentage,
                      EndDay = Data.EndDay,
                      Id = Data.Id,
                      InstallerName = objCompassEntities.tblUsers.Where(x => x.UserID == Data.FK_InstallerId).Select(x => x.FirstName).FirstOrDefault(),
                      StartDay = Data.StartDay
                  };
            return result;

        }



        public bool UpdateInstallerAuditCycles(long auditId, decimal auditPercent, int startDay, int endDay, int UserId, out string Message, out long projectId)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            Message = "";
            bool resultFlag = false;
            var Data = objCompassEntities.AuditInstallerPercentages.Where(o => o.Active == 1 && o.Id == auditId).FirstOrDefault();
            projectId = 0;
            if (Data == null)
            {
                Message = "Save failed. Seems data is updated before your try to update. Please refresh the page & then try...!!! ";

            }
            else
            {

                var list = objCompassEntities.AuditInstallerPercentages.Where(o => o.Active == 1 && o.FK_ProjectId == Data.FK_ProjectId
                    && o.FK_InstallerId == Data.FK_InstallerId && o.FK_CustomerId == Data.FK_CustomerId && o.Id != Data.Id
                    && ((startDay <= o.StartDay && endDay >= o.StartDay) ||
                        (startDay <= o.EndDay && endDay >= o.EndDay))
                    ).FirstOrDefault();

                if (list == null)
                {
                    Data.Active = 0;
                    Data.ModifiedBy = UserId;
                    Data.ModifiedOn = new CommonFunctions().ServerDate();
                    objCompassEntities.SaveChanges();

                    AuditInstallerPercentage objAuditInstallerPercentage = new AuditInstallerPercentage();
                    objAuditInstallerPercentage.FK_ProjectId = Data.FK_ProjectId;
                    objAuditInstallerPercentage.FK_InstallerId = Data.FK_InstallerId;
                    objAuditInstallerPercentage.FK_CustomerId = Data.FK_CustomerId;
                    objAuditInstallerPercentage.Active = 1;
                    objAuditInstallerPercentage.AuditPercentage = auditPercent;
                    objAuditInstallerPercentage.CreatedBy = UserId;
                    objAuditInstallerPercentage.CreatedOn = new CommonFunctions().ServerDate();
                    objAuditInstallerPercentage.EndDay = endDay;
                    objAuditInstallerPercentage.IsFailurePercentageApplied = Data.IsFailurePercentageApplied;
                    objAuditInstallerPercentage.StartDay = startDay;

                    objCompassEntities.AuditInstallerPercentages.Add(objAuditInstallerPercentage);
                    objCompassEntities.SaveChanges();
                    Message = "Update failed.";
                    resultFlag = true;

                    projectId = Data.FK_ProjectId;
                }
                else
                {
                    Message = "Update failed. Start Day or End Day overlaps other cycle for the Installer.";
                }

            }

            return resultFlag;
        }




        //public bool ActiveDeactivateInstallerRecord(long Id, bool val)
        //{
        //    bool result = false;
        //    CompassEntities objCompassEntities = new CompassEntities();

        //    var Data = objCompassEntities.AuditInstallerPercentages.Where(o => o.Id == Id).FirstOrDefault();
        //    Data.Active = val == true ? 1 : 0;
        //    objCompassEntities.SaveChanges();
        //    result = true;
        //    return result;
        //}


        //public bool updateInstallerRecord(long Id, bool val)
        //{
        //    bool result = false;
        //    CompassEntities objCompassEntities = new CompassEntities();

        //    var Data = objCompassEntities.AuditInstallerPercentages.Where(o => o.Id == Id).FirstOrDefault();
        //    Data.Active = val == true ? 1 : 0;
        //    objCompassEntities.SaveChanges();
        //    result = true;
        //    return result;

        //}



        public List<InventorySourceChildBO> GetInstallerForProject(long ProjectId)
        {
            List<InventorySourceChildBO> ObjInventorySourceChildBOList = new List<InventorySourceChildBO>();

            CompassEntities objCompassEntities = new CompassEntities();

            //installer option 
            WebServiceUserRoleDAL objWebServiceUserRoleDAL = new WebServiceUserRoleDAL();
            var a = objWebServiceUserRoleDAL.GetAllUsersBasedOnRoleWithSelf("Installer");
            if (a == null && a.Count == 0)
            {
                a = new List<InventorySourceChildBO>();
            }

            //  var obj = objCompassEntities.AuditMasters.Where(o => o.Id == ProjectId).FirstOrDefault();
            var CurrentProjectUsers = objCompassEntities.TblProjectInstallers.Where(o => o.Active == 1 && o.ProjectID == ProjectId).Select(o => o.tblUsers_UserID).ToList();


            ObjInventorySourceChildBOList = a.Where(o => CurrentProjectUsers.Contains(o.ChildId)).Select(o => o).ToList();




            objCompassEntities.Dispose();
            objWebServiceUserRoleDAL.Dispose();
            return ObjInventorySourceChildBOList;
        }

    }



}
