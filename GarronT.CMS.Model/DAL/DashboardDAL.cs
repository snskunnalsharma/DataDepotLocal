using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;


namespace GarronT.CMS.Model.DAL
{
    public class DashboardDAL
    {


        public string GetRunningProjectUtility()
        {
            CompassEntities CompassEntities = new CompassEntities();
            DateTime currentDate = new CommonFunctions().ServerDate();
            var data = CompassEntities.tblProjects.Where(o => o.FromDate <= currentDate.Date && currentDate.Date <= o.ToDate).
                 Select(o => o.Utilitytype).FirstOrDefault();

            return data;
        }


        #region GetMethods



        // Installations per month new method
        public List<InstallationModel> GetInstallationspermonthNew(long projectID, out int totaluploadedRecords, out int totalPending)
        {
            #region
            //string[] names = DateTimeFormatInfo.CurrentInfo.MonthNames;
            //List<string> monthlist = new List<string>();
            //foreach (var item in names)
            //{
            //    if (item != "")
            //    {
            //        monthlist.Add(DateTime.ParseExact(item, "MMMM", CultureInfo.CurrentCulture)
            //                                .ToString("MMM"));
            //    }
            //}
            //totaluploadedRecords = 0;
            //int pendingcount;
            //int totalRecords;
            //var list = new List<InstallationModel>();
            //var list2 = new List<STP_GetPendingInstallationsDashboard_Result>();
            //CompassEntities CompassEntities = new CompassEntities();
            //var list1 = new List<STP_DashboardInstallationsPerMonth_Result>();
            //List<tblInstallerMapData> installermappedrecords;
            //if (projectID != 0)
            //{
            //    list1 = CompassEntities.STP_DashboardInstallationsPerMonth().Where(a => a.ProjectId == projectID).OrderByDescending(o => o.Dates).ToList();
            //    list2 = CompassEntities.STP_GetPendingInstallationsDashboard().Where(a => a.projectid == projectID).OrderByDescending(o => o.Dates).ToList();

            //    var Uploadedrecords = CompassEntities.tblUploadedDatas.Where(a => a.ProjectId == projectID).ToList();
            //    totaluploadedRecords = Uploadedrecords.Count;
            //    installermappedrecords = (from m in CompassEntities.tblUploadedDatas
            //                              join im in CompassEntities.tblInstallerMapDatas on m.ID equals im.tblUploadedData_ID
            //                              where m.ProjectId == projectID && m.Active == 1 && im.Active == 1
            //                              select im).ToList();

            //    pendingcount = Uploadedrecords.Where(s => !installermappedrecords.Where(es => es.tblUploadedData_ID == s.ID).Any()).ToList().Count;
            //    totalRecords = installermappedrecords.Count;

            //}
            //else
            //{
            //    list1 = CompassEntities.STP_DashboardInstallationsPerMonth().OrderByDescending(o => o.Dates).ToList();
            //    list2 = CompassEntities.STP_GetPendingInstallationsDashboard().OrderByDescending(o => o.Dates).ToList();

            //    var Uploadedrecords = CompassEntities.tblUploadedDatas.ToList();
            //    totaluploadedRecords = Uploadedrecords.Count;
            //    installermappedrecords = (from m in CompassEntities.tblUploadedDatas
            //                              join im in CompassEntities.tblInstallerMapDatas on m.ID equals im.tblUploadedData_ID

            //                              select im).ToList();

            //    pendingcount = Uploadedrecords.Where(s => !installermappedrecords.Where(es => es.tblUploadedData_ID == s.ID).Any()).ToList().Count;

            //}


            //InstallationModel InstallationModelpending = new InstallationModel
            //{
            //    Month = "Not Allocated",
            //    Total = 0,
            //    Complete = 0,
            //    Pending = pendingcount,
            //    Skipped = 0
            //};
            //list.Add(InstallationModelpending);
            //if (list1.Count > -1)
            //{
            //    foreach (var item in names)
            //    {
            //        if (item != "")
            //        {
            //            InstallationModel InstallationModel = new InstallationModel
            //            {
            //                Month = item.Substring(0, 3),
            //                Value = (from b in list1 where b.Months == item select b).ToList().Count,

            //                Complete = (from b in list1
            //                            where b.Months == item && b.initialfieldname == "WORK COMPLETED" && b.fieldvalue == "true"
            //                            select b).ToList().Count,

            //                Pending1 = (from b in list2 where b.Months == item select b).ToList().Count,

            //                Pending = ((from b in list1
            //                            where b.Months == item && (b.initialfieldname == "WORK COMPLETED" && b.fieldvalue == "false") || b.initialfieldname == null && b.IsSkipped == false
            //                            select b).ToList().Count) + ((from b in list2
            //                                                          where b.Months == item
            //                                                          select b).ToList().Count),

            //                Skipped = (from b in list1 where b.Months == item && b.IsSkipped == true select b).ToList().Count,

            //            };
            //            list.Add(InstallationModel);
            //        }
            //    }

            //}

            ////if (list1.Count > -1)
            ////{
            ////    list2 = (from p in list1
            ////             group p by p.Months into d
            ////             select new InstallationModel() { Month = d.Key, Value = d.ToList().Count }).ToList();

            ////}
            //return list;
            #endregion

            var list = new List<InstallationModel>();

            CompassEntities objCompassEntities = new CompassEntities();

            var result = objCompassEntities.STP_ProjectMonthSummary(projectID).ToList();

            totaluploadedRecords = (result != null && result.Count > 0) ? int.Parse(Convert.ToString(result[0].TotalRecords)) : 0;

            var a = result.Sum(o => o.CompletedRecords + o.SkippedRecords);
            totalPending = a != null ? totaluploadedRecords - int.Parse(Convert.ToString(a)) : 0;
            int notMappedRecords = (result != null && result.Count > 0) ? int.Parse(Convert.ToString(result[0].NotMappedRecords)) : 0;

            foreach (var item in result)
            {
                decimal complete = decimal.Parse(!string.IsNullOrEmpty(Convert.ToString(item.CompletedRecords)) ? Convert.ToString(item.CompletedRecords) : "0");
                decimal skippped = decimal.Parse(!string.IsNullOrEmpty(Convert.ToString(item.SkippedRecords)) ? Convert.ToString(item.SkippedRecords) : "0");

                var ab = result.Where(o => o.MonthId < item.MonthId).Sum(o => o.CompletedRecords + o.SkippedRecords);
                decimal pendingRecords = decimal.Parse(Convert.ToString(totaluploadedRecords - (ab + complete + skippped)));

                InstallationModel obj1 = new InstallationModel();
                obj1.Complete = complete;
                obj1.Month = item.VisitMonth;
                if (complete != 0 && skippped != 0)
                {
                    obj1.Pending = pendingRecords;
                }
                obj1.Skipped = skippped;
                obj1.Total = totaluploadedRecords;
                list.Add(obj1);

            }



            InstallationModel InstallationModelpending = new InstallationModel
            {
                Month = "Not Allocated",
                Total = 0,
                Complete = 0,
                Pending = notMappedRecords,
                Skipped = 0
            };

            list.Insert(0, InstallationModelpending);
            return list;
        }

        // Installations per installer  column chart
        public List<InstallationModel> GetInstallationPerInstaller(long projectID, out int totaluploadedRecords)
        {

            var list = new List<InstallationModel>();
            totaluploadedRecords = 0;
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();
                var Result = objCompassEntities.PROC_Dashboard_InstallationsPerInstaller(projectID, 2).ToList();
                list = Result.Select(o =>
                    new InstallationModel
                    {
                        Complete = Convert.ToDecimal(o.CompletedRecords),
                        InstallerID = Convert.ToInt64(o.InstallerId),
                        Pending = Convert.ToDecimal(o.PendingRecords),
                        Skipped = Convert.ToDecimal(o.SkippedRecords),
                        RTU = Convert.ToDecimal(o.RTURecords),
                        Total = Convert.ToDecimal(o.TotalMappedRecords),
                        Installer = o.InstallerName
                    }).ToList();
                totaluploadedRecords = Convert.ToInt32(Result[0].TotalRecords);

            }
            catch (Exception ex)
            {
                //throw;
            }
            return list;
        }

        // Installations per project
        public List<ProjectInstallationModel> GetProjectInsatallation()
        {
            var list = new List<ProjectInstallationModel>();
            CompassEntities CompassEntities = new CompassEntities();
            var list1 = CompassEntities.STP_GetReportTable().OrderByDescending(o => o.Dates).ToList();

            if (list1.Count > 0)
            {
                list = (from p in list1
                        group p by p.ProjectId into d
                        select new ProjectInstallationModel()
                        {
                            Project = (from p in CompassEntities.tblProjects
                                       where p.ProjectId == d.Key
                                       select p.ProjectName).FirstOrDefault(),
                            Sales = d.ToList().Count
                        }).ToList();

            }
            return list;
        }

        //Pie chart
        public List<InstallationModel> GetInsatallationPerInstaller(long projectID)
        {
            CompassEntities CompassEntities = new CompassEntities();
            int pendingcount;
            var installerList = new List<InstallationModel>();
            if (projectID != 0)
            {
                //installerList = CompassEntities.STP_DashboardInstallationsPerMonth().Where(a => a.ProjectId == projectID).OrderBy(o => o.FirstName).ToList();

                installerList = GeInstallerListbyprojectID(projectID).OrderBy(a => a.Installer).ToList();

                var Uploadedrecords = CompassEntities.tblUploadedDatas.Where(a => a.ProjectId == projectID).ToList();
                var installermappedrecords = (from m in CompassEntities.tblUploadedDatas
                                              join im in CompassEntities.tblInstallerMapDatas on m.ID equals im.tblUploadedData_ID
                                              where m.ProjectId == projectID
                                              select im).ToList();

                pendingcount = Uploadedrecords.Where(s => !installermappedrecords.Where(es => es.tblUploadedData_ID == s.ID).Any() && s.ProjectId == projectID).ToList().Count;
            }
            else
            {
                // installerList = CompassEntities.STP_DashboardInstallationsPerMonth().OrderBy(o => o.FirstName).ToList();

                installerList = GeInstallerList().OrderBy(a => a.Installer).ToList();

                var Uploadedrecords = CompassEntities.tblUploadedDatas.ToList();
                var installermappedrecords = (from m in CompassEntities.tblUploadedDatas
                                              join im in CompassEntities.tblInstallerMapDatas on m.ID equals im.tblUploadedData_ID

                                              select im).ToList();

                pendingcount = Uploadedrecords.Where(s => !installermappedrecords.Where(es => es.tblUploadedData_ID == s.ID).Any()).ToList().Count;
            }

            var list = new List<InstallationModel>();
            var list2 = new List<STP_GetPendingInstallationsDashboard_Result>();

            var list1 = new List<STP_DashboardInstallationsPerMonth_Result>();
            if (projectID != 0)
            {
                list1 = CompassEntities.STP_DashboardInstallationsPerMonth().Where(a => a.ProjectId == projectID).OrderBy(o => o.FirstName).ToList();
                list2 = CompassEntities.STP_GetPendingInstallationsDashboard().Where(a => a.projectid == projectID).OrderByDescending(o => o.Dates).ToList();
            }
            else
            {
                list1 = CompassEntities.STP_DashboardInstallationsPerMonth().OrderBy(o => o.FirstName).ToList();
                list2 = CompassEntities.STP_GetPendingInstallationsDashboard().ToList();
            }

            InstallationModel InstallationModelpending = new InstallationModel
            {
                Installer = "Not Allocated",
                Complete = 0,
                Pending = pendingcount,
                Skipped = 0
            };
            list.Add(InstallationModelpending);
            return list;
        }


        // completion vs pending
        public List<CompletionModel> GetCompleteVsPendingChartData(long projectID)
        {

            var list = new List<CompletionModel>();


            try
            {
                CompassEntities objCompassEntities = new CompassEntities();
                
                var result = objCompassEntities.PROC_Dashboard_CompVsPendingChart(projectID, 2).FirstOrDefault();


                int UnMappedRecords = result.UnMappedRecords != null ? Convert.ToInt32(result.UnMappedRecords) : 0;
                int MappedRecords = result.MappedRecords != null ? Convert.ToInt32(result.MappedRecords) : 0;
                int SkippedRecords = result.SkippedRecords != null ? Convert.ToInt32(result.SkippedRecords) : 0;
                int RtuRecords = result.RtuRecords != null ? Convert.ToInt32(result.RtuRecords) : 0;
                int CompletedRecords = result.CompletedRecords != null ? Convert.ToInt32(result.CompletedRecords) : 0;
                int TotalRecords = result.TotalRecords != null ? Convert.ToInt32(result.TotalRecords) : 0;
                int PendingRecords = MappedRecords - (CompletedRecords + SkippedRecords + RtuRecords);
                int UnAllocatedSkip = result.UnAllocatedSkip != null ? Convert.ToInt32(result.UnAllocatedSkip) : 0;
                CompletionModel obj = new CompletionModel();
                obj.Status = "Completed";
                obj.Installations = CompletedRecords;
                obj.percentage = (int)Math.Round((double)(100 * CompletedRecords) / TotalRecords);
                list.Add(obj);

                CompletionModel obj1 = new CompletionModel();
                obj1.Status = "Not Allocated";
                obj1.Installations = UnMappedRecords;
                obj1.percentage = (int)Math.Round((double)(100 * UnMappedRecords) / TotalRecords);
                list.Add(obj1);


                CompletionModel obj2 = new CompletionModel();
                obj2.Status = "Pending";
                obj2.Installations = PendingRecords;
                obj2.percentage = (int)Math.Round((double)(100 * PendingRecords) / TotalRecords);
                list.Add(obj2);

                CompletionModel obj3 = new CompletionModel();
                obj3.Status = "Skipped";
                obj3.Installations = SkippedRecords;
                obj3.percentage = (int)Math.Round((double)(100 * SkippedRecords) / TotalRecords);
                list.Add(obj3);

                CompletionModel obj4 = new CompletionModel();
                obj4.Status = "RTU";
                obj4.Installations = RtuRecords;
                obj4.percentage = (int)Math.Round((double)(100 * RtuRecords) / TotalRecords);
                list.Add(obj4);

                CompletionModel obj5 = new CompletionModel();
                obj5.Status = "UnAllocated Skip";
                obj5.Installations = UnAllocatedSkip;
                obj5.percentage = (int)Math.Round((double)(100 * UnAllocatedSkip) / TotalRecords);
                list.Add(obj5);

            }
            catch (Exception ex)
            {
            }

            return list;
        }


        public List<DashboardHeader> GetCityListHeader()
        {
            List<DashboardHeader> CityList = new List<DashboardHeader>();

            using (CompassEntities _dbContext = new CompassEntities())
            {
                CityList = (from c in _dbContext.TblCityMasters
                            join p in _dbContext.tblProjects on c.CityId equals p.CityId
                            where p.Active == 1
                            select new DashboardHeader
                            {
                                CityID = c.CityId,
                                CityName = c.CityName

                            }).Distinct().OrderBy(c => c.CityName).ToList();

            }

            return CityList;

        }


        public List<DashboardHeader> GetProjectByCityID(long CityID)
        {
            List<DashboardHeader> CityList = new List<DashboardHeader>();

            using (CompassEntities _dbContext = new CompassEntities())
            {
                CityList = (from c in _dbContext.tblProjects
                            where c.CityId == CityID && c.Active == 1
                            select new DashboardHeader
                            {
                                ProjectID = c.ProjectId,
                                ProjectName = c.ProjectName

                            }).Distinct().OrderBy(c => c.ProjectName).ToList();

            }

            return CityList;

        }



        public List<InstallationModel> GeInstallerList()
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<InstallationModel> MeterSizeModel = (from b in CompassEntities.TblProjectInstallers
                                                          where b.Active == 1
                                                          select new InstallationModel
                                                        {
                                                            InstallerID = b.tblUsers_UserID
                                                        }).Distinct().ToList();


                foreach (var item in MeterSizeModel)
                {
                    item.Installer = CompassEntities.tblUsers.Where(a => a.UserID == item.InstallerID).Select(m => m.FirstName).FirstOrDefault();
                }

                return MeterSizeModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<InstallationModel> GetProjectList()
        {

            DateTime currentdate = new CommonFunctions().ServerDate();
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<InstallationModel> projectList = (from b in CompassEntities.tblProjects
                                                       where b.Active == 1 && b.ToDate > currentdate && b.IsLatLongCalculated == 1
                                                       select new InstallationModel
                                                       {
                                                           Installer = b.ProjectName,
                                                           InstallerID = b.ProjectId
                                                       }).Distinct().ToList();




                return projectList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<InstallationModel> GeInstallerListbyprojectID(long projectID)
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<InstallationModel> MeterSizeModel = (from b in CompassEntities.TblProjectInstallers
                                                          where b.Active == 1 && b.ProjectID == projectID
                                                          select new InstallationModel
                                                        {
                                                            InstallerID = b.tblUsers_UserID
                                                        }).Distinct().ToList();


                foreach (var item in MeterSizeModel)
                {
                    item.Installer = CompassEntities.tblUsers.Where(a => a.UserID == item.InstallerID).Select(m => m.FirstName).FirstOrDefault();
                }

                return MeterSizeModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<CompletionModel> ProjectCompletionInPercentGraph(string utilityType)
        {
            #region old code commented by Bharat
            //try
            //{
            //    CompassEntities CompassEntities = new CompassEntities();


            //    var ProjectList = new List<InstallationModel>();

            //    ProjectList = GetProjectList().OrderBy(a => a.Installer).ToList();



            //    var list = new List<CompletionModel>();
            //    var list2 = new List<STP_GetPendingInstallationsDashboard_Result>();

            //    var list1 = new List<STP_DashboardInstallationsPerMonth_Result>();

            //    list1 = CompassEntities.STP_DashboardInstallationsPerMonth().OrderBy(o => o.FirstName).ToList();

            //    if (list1.Count > -1)
            //    {
            //        foreach (var item in ProjectList)
            //        {
            //            if (item != null)
            //            {
            //                var Uploadedrecords = CompassEntities.tblUploadedDatas.Where(a => a.ProjectId == item.InstallerID).ToList();

            //                CompletionModel CompletionModel = new BO.CompletionModel();
            //                var Complete = (from b in list1
            //                                where b.ProjectId == item.InstallerID && b.initialfieldname == "WORK COMPLETED" && b.IsSkipped == false && b.fieldvalue == "true"
            //                                select b).ToList().Count;
            //                if (Uploadedrecords.Count == 0)
            //                {
            //                    CompletionModel.percentage = 0;
            //                }
            //                else
            //                {
            //                    var percentage = (int)Math.Round((double)(100 * Complete) / Uploadedrecords.Count);
            //                    CompletionModel.percentage = percentage;
            //                    CompletionModel.projectName = item.Installer;

            //                    list.Add(CompletionModel);
            //                }



            //            }
            //        }

            //    }


            //    return list;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            #endregion

            var list = new List<CompletionModel>();
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();
                var result = objCompassEntities.PROC_DashBoard_ProjectCompletePercent(utilityType, 2).ToList();


                list = result.Select(o => new CompletionModel
                {
                    Installations = Convert.ToInt32(o.TotalRecords),
                    percentage = Convert.ToInt32(o.Percentage),
                    projectName = o.ProjectName,
                    Status = o.ProStatus,
                    projectDisplayName = o.ProjectName.Length > 20 ? o.ProjectName.Substring(0, 20) + "..." : o.ProjectName,
                    projectId=o.ProjectId.Value
                }).ToList();

            }
            catch (Exception ex)
            {
            }

            return list;
        }


        #endregion

        #region Get Total Installation Audit Count by SP
        public List<PROC_DashBoard_Audit_TotalInstallationCount_Result> GetInstallatioCount(long projectId)
        {
            List<PROC_DashBoard_Audit_TotalInstallationCount_Result> _List = new List<PROC_DashBoard_Audit_TotalInstallationCount_Result>();
            try
            {
                CompassEntities objCompass = new CompassEntities();
                _List = objCompass.PROC_DashBoard_Audit_TotalInstallationCount(projectId).ToList();
            }
            catch (Exception ex)
            {

            }
            return _List;
        }

        #endregion

        #region Get Total Assigned Audit Count by SP
        public List<PROC_DashBoard_Audit_TotalAssignedAuditCount_Result> GetAssignedAudit(long projectId)
        {
            List<PROC_DashBoard_Audit_TotalAssignedAuditCount_Result> _assingedList = new List<PROC_DashBoard_Audit_TotalAssignedAuditCount_Result>();
            try
            {
                CompassEntities objCompass = new CompassEntities();
                _assingedList = objCompass.PROC_DashBoard_Audit_TotalAssignedAuditCount(projectId).ToList();
            }
            catch (Exception ex)
            {

            }
            return _assingedList;
        }

        #endregion GetFailedAudit

        #region Get Total Passed Audit Count by SP
        public List<PROC_DashBoard_Audit_TotalPassedAuditCount_Result> GetPassedAudit(long projectId)
        {
            List<PROC_DashBoard_Audit_TotalPassedAuditCount_Result> _passedList = new List<PROC_DashBoard_Audit_TotalPassedAuditCount_Result>();
            try
            {
                CompassEntities objCompass = new CompassEntities();

                _passedList = objCompass.PROC_DashBoard_Audit_TotalPassedAuditCount(projectId).ToList();



            }
            catch (Exception ex)
            {

            }
            return _passedList;
        }

        #endregion

        #region Get Total Failed Audit Count by SP
        public List<PROC_DashBoard_Audit_TotalFailedAuditCount_Result> GetFailedAudit(long projectId)
        {
            List<PROC_DashBoard_Audit_TotalFailedAuditCount_Result> _failList = new List<PROC_DashBoard_Audit_TotalFailedAuditCount_Result>();
            try
            {
                CompassEntities objCompass = new CompassEntities();

                _failList = objCompass.PROC_DashBoard_Audit_TotalFailedAuditCount(projectId).ToList();



            }
            catch (Exception ex)
            {

            }
            return _failList;
        }

        #endregion
    }
}
