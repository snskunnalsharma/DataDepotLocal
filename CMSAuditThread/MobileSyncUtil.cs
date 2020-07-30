using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using GarronT.CMS.Model;
using System.Data.SqlClient;
using System.Data;
using GarronT.CMS.Model.DAL;
using GarronT.CMS.Model.BO;

namespace CMSAuditThread
{
    class MobileSyncUtil
    {
        public void triggerScheduler()
        {
            DateTime dt = DateTime.Now;
            clsErrorLogFile.writeStatus("Thread Started at -" + DateTime.Now.ToShortTimeString());
            clsErrorLogFile.writeStatus("Dates -" + dt.Date.ToString() + " / " + DateTime.Now.Date.ToString());
            Console.WriteLine("Thread Started at -" + DateTime.Now.ToShortTimeString());
            try
            {
                CompleteTask completeTask = new CompleteTask();
                completeTask.setMobileSyncStatus(25);
            }
            catch (Exception ex)
            {
                clsErrorLogFile.writeStatus("OnStart Method exception occured at -" + DateTime.Now.ToShortTimeString());
                clsErrorLogFile.writeStatus("OnStart Method exception ex message-" + ex.Message);
                clsErrorLogFile.writeStatus("OnStart Method exception ex inner exception message-" + Convert.ToString(ex.InnerException.Message));
                Console.WriteLine("OnStart Method exception ex message-" + ex.Message);
            }
            clsErrorLogFile.writeStatus("Thread End at -" + DateTime.Now.ToShortTimeString());
            Console.WriteLine("Thread End at -" + DateTime.Now.ToShortTimeString());
        }
    }

    class CompleteTask
    {
        public void setMobileSyncStatus(int topActive)
        {
            CompassEntities objCompassEntities = new CompassEntities();

            try
            {
                ProjectMasterDAL projectMasterDAL = new ProjectMasterDAL();
                IEnumerable<ProjectModel> data = (from a in projectMasterDAL.GetProjectName()
                                                  orderby a.ProjectName
                                                  select a).Distinct<ProjectModel>();

                foreach (var proj in data)
                {
                    var projectId = proj.ProjectId;
                    var fromDate = proj.stringFromDate;
                    var toDate = proj.stringToDate;
                    var formId = objCompassEntities.tblFormMasters.AsNoTracking().Where(o => o.Active == 1 && o.FormTypeID == 2
                       && o.ProjectId == projectId).Select(o => o.FormId).FirstOrDefault();
                    List<UsersSmallModel> userObj = new List<UsersSmallModel>();
                    if (projectId > 0)
                    {
                        using (UserRoleDAL obj = new UserRoleDAL())
                        {
                            userObj = obj.GeInstallerListCurrentProject(projectId).OrderBy(a => a.UserName).ToList();
                        }
                        foreach (var user in userObj)
                        {
                            List<CycleMaster> cycleList = GetExistingCycles((long)projectId, user.UserId, fromDate, toDate);

                            foreach (var cycle in cycleList)
                            {
                                var cycleId = cycle.CycleId;
                                clsErrorLogFile.writeStatus("For Cycle -" + cycleId);
                                Console.WriteLine("For Cycle -" + cycleId);
                                DataTable dt = new DataTable();
                                using (SqlConnection sqlcon = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString))
                                {
                                    using (SqlCommand cmd = new SqlCommand("PROC_MobileSync_GetAfterTop", sqlcon))
                                    {
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.Add("@formId", formId);
                                        cmd.Parameters.Add("@InstallerId", user.UserId);
                                        cmd.Parameters.Add("@cycle", cycleId);
                                        cmd.Parameters.Add("@FromDate", DateTime.Parse(fromDate));
                                        cmd.Parameters.Add("@ToDate", DateTime.Parse(toDate));
                                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                                        {
                                            da.Fill(dt);
                                        }
                                    }
                                }
                                if (dt != null)
                                {
                                    var distExcelData = dt.AsEnumerable().
                                    Select(o => new
                                    {
                                        DataId = o.Field<long>("ID")
                                    }).Distinct().ToList();

                                    foreach (var dataId in distExcelData)
                                    {
                                        var installerData = objCompassEntities.tblInstallerMapDatas.Where(x => x.ID == dataId.DataId).Select(x => x.FK_UploadedExcelData_Id).FirstOrDefault();
                                        var excelData = objCompassEntities.UploadedExcelDatas.Where(o => o.Id == installerData).FirstOrDefault();
                                        clsErrorLogFile.writeStatus("For excelData Id -" + excelData.Id);
                                        Console.WriteLine("For excelData Id -" + excelData.Id);
                                        excelData.SyncEnable = 0;

                                    }
                                    objCompassEntities.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsErrorLogFile.writeStatus("SetMobileSyncStatus Method exception ex message-" + ex.Message);
                clsErrorLogFile.writeStatus("SetMobileSyncStatus Method exception ex inner exception message-" + Convert.ToString(ex.InnerException.Message));
            }

            

            /*using (SqlConnection sqlcon = new SqlConnection(objCompassEntities.Database.Connection.ConnectionString))
            {
                var userData = objCompassEntities.tblUsers.Where(o => o.Active == 1).ToList();
                foreach (var item in userData)
                {
                    var userId = item.UserID;
                    var projectList = new ProjectMasterDAL().GetUserProjectList(userId);

                    foreach(var project in projectList)
                    {
                        int active = 1;
                        GetExistingCycles((long)project, userId, "08-01-2017", "08-01-2020");
                        /*string cycleColumn = Convert.ToString((from m in objCompassEntities.TblProjectExcelMappings
                                                         join c in objCompassEntities.VW_GETTab6DropdownData on m.FieldType equals c.FieldType
                                                         where m.ProjectID == project && c.ProjectID == project && m.Active == active && c.FieldName == "CYCLE"
                                                         select m.FKUploadedExcelDataDynamicColumn).FirstOrDefault());
                        var cycleList = (from a in objCompassEntities.UploadedExcelDatas where a.Active == 1 &&
                                         a.FKProjectId == project group a by cycleColumn into b select cycleColumn).ToList();
                        DataTable dt = new DataTable();
                        var projectId = project;
                        //var cycleList = new InstallerDAL().GetCycleByProjectID((long)project);
                        foreach(var cycle in cycleList)
                        {

                        }*/
                    //}

                    
                //}
                    /*using (SqlCommand cmd = new SqlCommand("FN_GetColumnName", sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@input", "CYCLE");
                        cmd.Parameters.Add("@projectId", projectId);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }*/
            //}
        }

        public List<CycleMaster> GetExistingCycles(long projectID, long installerId, string fromDate, string toDate)
        {
            List<string> cyclelist1 = new List<string>();

            cyclelist1 = new InstallerDAL().GetDynamicFreshCycleByProjectID(projectID);
            var cyclelist2 = new InstallerDAL().GetDynamicExistingMappedCycle(projectID, installerId, DateTime.Parse(fromDate), DateTime.Parse(toDate));
            List<CycleMaster> OrignalCycleList = new List<CycleMaster>();
            //List<CycleMaster> cyclelist = new List<CycleMaster>();
            foreach (var item in cyclelist1)
            {
                OrignalCycleList.Add(new CycleMaster { CycleId = item, CycleDisabled = false });
            }


            foreach (var item in cyclelist2)
            {
                var ObjCycle = OrignalCycleList.Where(o => o.CycleId == item.Cycle).FirstOrDefault();
                if (ObjCycle == null)
                {
                    OrignalCycleList.Add(new CycleMaster { CycleId = item.Cycle, CycleDisabled = item.Disabled == 0 ? false : true });
                }
                else
                {

                    if (ObjCycle.CycleDisabled == false)
                    {
                        ObjCycle.CycleDisabled = item.Disabled == 0 ? false : true;
                    }
                }
            }

            /*foreach (var item in cyclelist2)
            {
                var ObjCycle = cyclelist.Where(o => o.CycleId == item.Cycle).FirstOrDefault();
                if (ObjCycle == null)
                {
                    cyclelist.Add(new CycleMaster { CycleId = item.Cycle, CycleDisabled = item.Disabled == 0 ? false : true });
                }
                else
                {

                    if (ObjCycle.CycleDisabled == false)
                    {
                        ObjCycle.CycleDisabled = item.Disabled == 0 ? false : true;
                    }
                }
            }

            cyclelist = cyclelist.OrderBy(o => o.CycleId).Distinct().ToList();*/
            OrignalCycleList = OrignalCycleList.OrderBy(o => o.CycleId).Distinct().ToList();
            return OrignalCycleList;
            //return base.Json(new { OrignalCycleList = OrignalCycleList, CycleList = cyclelist }, JsonRequestBehavior.AllowGet);
        }
    }
}
