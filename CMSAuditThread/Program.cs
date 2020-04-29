using GarronT.CMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMSAuditThread
{
    class Program
    {
        static void Main(string[] args)
        {

            clsErrorLogFile.writeStatus("Thread Started at -" + DateTime.Now.ToShortTimeString());


            AuditAllocation objAuditAllocation = new AuditAllocation();

            objAuditAllocation.startTheScheduler();
         


            clsErrorLogFile.writeStatus("Thread ended at -" + DateTime.Now.ToShortTimeString());
        }
    }



       

    public class AuditAllocation
    {


        public void startTheScheduler()
        {
            DateTime dt = DateTime.Now;



            while (true)
            {
                clsErrorLogFile.writeStatus("Thread Started at -" + DateTime.Now.ToShortTimeString());
                clsErrorLogFile.writeStatus("Dates -" + dt.Date.ToString() + " / " + DateTime.Now.Date.ToString());
                if (dt.Date == DateTime.Now.Date)
                {
                    try
                    {
                        clsErrorLogFile.writeStatus("Date Matched at -" + DateTime.Now.ToShortTimeString());


                        AuditAllocation objAuditAllocation = new AuditAllocation();

                        objAuditAllocation.AddToAudit();
                        objAuditAllocation.AllocateAuditVisit();


                        clsErrorLogFile.writeStatus("Thread ended at -" + DateTime.Now.ToShortTimeString());
                    }
                    catch (Exception ex)
                    {
                        clsErrorLogFile.writeStatus("OnStart Method exception occured at -" + DateTime.Now.ToShortTimeString());
                        clsErrorLogFile.writeStatus("OnStart Method exception ex message-" + ex.Message);
                        clsErrorLogFile.writeStatus("OnStart Method exception ex inner exception message-" + Convert.ToString(ex.InnerException.Message));

                    }


                    dt=dt.AddDays(1);


                }
                else { break; }

                //3 hours sleep
                //this.Sleep(10800000);

            }


        }

        /// <summary>
        /// 
        /// </summary>
        public void AllocateAuditVisit()
        {

            clsErrorLogFile.writeStatus("AllocateAuditVisit method started at -" + DateTime.Now.ToShortTimeString());
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();


                var DataList = objCompassEntities.AuditDatas.Where(o => o.AddedForAudit == 0 && o.IsTakenForCalculation != 1 && o.Active == 1).ToList();


                //check data is avilable for audit.
                if (DataList != null && DataList.Count > 0)
                {

                    var ProjectList = DataList.Select(o => o.FK_ProjectId).Distinct().ToList();

                    //projectwise push data to audit from list
                    foreach (var PId in ProjectList)
                    {

                        var project = objCompassEntities.tblProjects.Where(a => a.ProjectId == PId && a.Active == 1 && a.ProjectStatus == "Active").FirstOrDefault();
                        if (project != null)
                        {

                            var AuditProject = objCompassEntities.AuditMasters.Where(o => o.FKProjectId == PId && o.Active == 1).FirstOrDefault();
                            if (AuditProject != null)
                            {
                                long customerId = long.Parse(project.ClientId.ToString());

                                var CustomerAuditList = objCompassEntities.CustomerAuditSettings.Where(o => o.FK_CustomerId == customerId && o.Active == 1).ToList();

                                var AuditorList = objCompassEntities.AuditorMaps.Where(o => o.Active == 1 && o.FK_AuditId == AuditProject.Id).ToList();

                                int AuditorCount = AuditorList.Count();

                                if (AuditorCount > 0)
                                {
                                    if (AuditorCount == 1)
                                    {
                                        #region single auditor

                                        var currentProjectRecords = DataList.Where(o => o.FK_ProjectId == PId).ToList();

                                        var PercentageList = currentProjectRecords.Select(o => o.AuditPercent).Distinct().ToList();

                                        foreach (var itemPercent in PercentageList)
                                        {
                                            #region
                                            var CurrentPecentList = currentProjectRecords.Where(o => o.AuditPercent == itemPercent).ToList();

                                            int NoOfRecordsAsPerPercent = CurrentPecentList.Count();

                                            if (NoOfRecordsAsPerPercent > 0)
                                            {

                                                var noOfRecords = (NoOfRecordsAsPerPercent * itemPercent) / 100;

                                                int TotalRequiredRecordForAudit = Convert.ToInt32(noOfRecords);

                                                int currentRecord = 0;
                                                foreach (var item in currentProjectRecords)
                                                {
                                                    #region
                                                    if (currentRecord < TotalRequiredRecordForAudit)
                                                    {
                                                        if (AuditorList[0].FK_UserId != item.Fk_InstallerId)
                                                        {
                                                            ProjectAuditAllocation obj = new ProjectAuditAllocation();

                                                            obj.Active = 1;
                                                            obj.CreatedOn = new CommonFunctions().ServerDate();
                                                            obj.FK_AuditorId = AuditorList[0].FK_UserId;
                                                            obj.Fk_ProjectId = PId;
                                                            obj.FK_UploadedExcelData_Id = item.FK_UploadId;
                                                            obj.FromDate = AuditProject.AuditStartDate;
                                                            obj.ToDate = AuditProject.AuditEndDate;

                                                            objCompassEntities.ProjectAuditAllocations.Add(obj);
                                                            objCompassEntities.SaveChanges();


                                                            var _data = objCompassEntities.AuditDatas.Where(o => o.Id == item.Id).FirstOrDefault();
                                                            _data.AddedForAudit = 1;
                                                            _data.IsTakenForCalculation = 1;
                                                            objCompassEntities.SaveChanges();


                                                            currentRecord++;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        var _data = objCompassEntities.AuditDatas.Where(o => o.Id == item.Id).FirstOrDefault();
                                                        _data.IsTakenForCalculation = 1;
                                                        objCompassEntities.SaveChanges();
                                                    }
                                                    #endregion
                                                }
                                            }
                                            #endregion
                                        }



                                        #endregion
                                    }
                                    else
                                    {
                                        #region Multiple Auditor

                                        var projectAuditList = DataList.Where(o => o.FK_ProjectId == PId).ToList();

                                        var installerList = projectAuditList.Select(o => o.Fk_InstallerId).Distinct().ToList();


                                        //for each installer push data to audit
                                        foreach (var item in installerList)
                                        {

                                            var projectAuditData = projectAuditList.Where(o => o.Fk_InstallerId == item).ToList();


                                            var currentAuditList = AuditorList.Where(o => o.FK_UserId != item).ToList();

                                            int currentAuditorCount = currentAuditList.Count();

                                            var PercentageList = projectAuditData.Select(o => o.AuditPercent).Distinct().ToList();


                                            int i = 0;
                                            foreach (var itemPercent in PercentageList)
                                            {

                                                var CurrentPecentList = projectAuditData.Where(o => o.AuditPercent == itemPercent).ToList();

                                                int NoOfRecordsAsPerPercent = CurrentPecentList.Count();

                                                if (NoOfRecordsAsPerPercent > 0)
                                                {

                                                    var noOfRecords = (NoOfRecordsAsPerPercent * itemPercent) / 100;

                                                    int TotalRequiredRecordForAudit = Convert.ToInt32(noOfRecords);

                                                    int currentRecord = 0;

                                                    foreach (var auditItem in CurrentPecentList)
                                                    {

                                                        if (currentRecord < TotalRequiredRecordForAudit)
                                                        {


                                                            ProjectAuditAllocation obj = new ProjectAuditAllocation();

                                                            obj.FK_AuditorId = currentAuditList[i].FK_UserId;

                                                            obj.Active = 1;
                                                            obj.CreatedOn = new CommonFunctions().ServerDate();
                                                            obj.Fk_ProjectId = PId;
                                                            obj.FK_UploadedExcelData_Id = auditItem.FK_UploadId;
                                                            obj.FromDate = AuditProject.AuditStartDate;
                                                            obj.ToDate = AuditProject.AuditEndDate;

                                                            objCompassEntities.ProjectAuditAllocations.Add(obj);
                                                            objCompassEntities.SaveChanges();


                                                            var _data = objCompassEntities.AuditDatas.Where(o => o.Id == auditItem.Id).FirstOrDefault();
                                                            _data.IsTakenForCalculation = 1;
                                                            _data.AddedForAudit = 1;
                                                            objCompassEntities.SaveChanges();


                                                            i = i + 1 == currentAuditorCount ? 0 : i + 1;

                                                            currentRecord++;
                                                        }
                                                        else
                                                        {
                                                            var _data = objCompassEntities.AuditDatas.Where(o => o.Id == auditItem.Id).FirstOrDefault();
                                                            _data.IsTakenForCalculation = 1;
                                                            objCompassEntities.SaveChanges();
                                                        }
                                                    }
                                                }
                                            }

                                        }


                                        #endregion
                                    }
                                }
                            }

                        }
                    }

                }
                clsErrorLogFile.writeStatus("AllocateAuditVisit method ended at -" + DateTime.Now.ToShortTimeString());
            }
            catch (Exception ex)
            {
                clsErrorLogFile.writeStatus("AllocateAuditVisit Method exception occured at -" + DateTime.Now.ToShortTimeString());
                clsErrorLogFile.writeStatus("AllocateAuditVisit Method exception ex message-" + ex.Message);
                clsErrorLogFile.writeStatus("AllocateAuditVisit Method exception ex inner exception message-" + Convert.ToString(ex.InnerException.Message));

            }

            clsErrorLogFile.writeStatus("AllocateAuditVisit method exit at -" + DateTime.Now.ToShortTimeString());

        }
        



        /// <summary>
        /// 
        /// </summary>
        public void AddToAudit()
        {
            try
            {
                clsErrorLogFile.writeStatus("AddToAudit Started at -" + DateTime.Now.ToShortTimeString());


                CompassEntities objCompassEntities = new CompassEntities();

                var DataList = objCompassEntities.AuditDatas.Where(o => o.AddedForAudit == 0 && o.IsTakenForCalculation != 1 && o.Active == 1).ToList();


                //check data is avilable for audit.
                if (DataList != null && DataList.Count > 0)
                {

                    var ProjectList = DataList.Select(o => o.FK_ProjectId).Distinct().ToList();

                    //projectwise push data to audit from list
                    foreach (var PId in ProjectList)
                    {

                        // check audit project scheduled for the project

                        var auditProjectRecord = objCompassEntities.AuditMasters.Where(o => o.FKProjectId == PId && o.Active == 1).FirstOrDefault();
                        var formAuditPresent = objCompassEntities.tblFormMasters.Where(o => o.FormTypeID == 1 && o.Active == 1 && o.ProjectId == PId).FirstOrDefault();



                        if (auditProjectRecord == null || formAuditPresent == null)
                        {
                            // if audit project not set for the project
                            // Fire mail.



                        }
                        else
                        {
                            #region audit project & form is set
                        Outer:
                            //continue;
                            // audit project set for the  current project

                            var projectDetails = objCompassEntities.tblProjects.Where(o => o.ProjectId == PId).FirstOrDefault();

                            var InstallerList = DataList.Where(o => o.FK_ProjectId == PId).Select(o => o.Fk_InstallerId).Distinct().ToList();

                            var AuditPercentList = objCompassEntities.CustomerAuditSettings.Where(o => o.FK_CustomerId == projectDetails.ClientId && o.Active == 1).OrderBy(o => o.StartDay).ToList();

                            var FailPercentList = objCompassEntities.CustomerAuditFailureSettings.Where(o => o.FK_CustomerId == projectDetails.ClientId && o.Active == 1).ToList();

                            ///installer wise audit percent settings
                            foreach (var installerItem in InstallerList)
                            {

                                #region check data of every installer
                                //get records order by date.
                                var CurrentInstallerData = DataList.Where(o => o.FK_ProjectId == PId && o.Fk_InstallerId == installerItem).OrderBy(o => o.VisitDate).ToList();

                                var CurrentInstallerAuditPercentList = objCompassEntities.AuditInstallerPercentages.Where(o => o.Active == 1 && o.FK_ProjectId == PId
                                    && o.FK_CustomerId == projectDetails.ClientId && o.FK_InstallerId == installerItem).ToList();


                                if (CurrentInstallerAuditPercentList != null)
                                {
                                    //if (CurrentInstallerAuditPercentList.Count() == 1)
                                    //{



                                    //            foreach (var auditRecord in CurrentInstallerData)
                                    //            {
                                    //                auditRecord.AuditPercent = CurrentInstallerAuditPercentList[0].AuditPercentage;
                                    //                objCompassEntities.SaveChanges();
                                    //            }


                                    //}
                                    //else
                                    //{

                                    ///get all distinct day on which the installer worked. 
                                    var CurrentInstallerDateDistinctList = CurrentInstallerData.Select(o => o.VisitDate.Date).Distinct().ToList();


                                    //add loop for each day to set percent.
                                    foreach (var CurrentDate in CurrentInstallerDateDistinctList)
                                    {


                                        //get all data for the current date
                                        var GivenDateAuditList = CurrentInstallerData.Where(o => o.VisitDate.Date == CurrentDate.Date).ToList();


                                        //get number of worked days for installer on the current project less than current date
                                        var WorkingDayNoForInstaller = objCompassEntities.PROC_GetNoOfDays(CurrentDate, installerItem, PId).FirstOrDefault();

                                        // add 1 day to WorkingDayNoForInstaller
                                        int workDay = WorkingDayNoForInstaller == null ? 1 : int.Parse(WorkingDayNoForInstaller.ToString()) + 1;

                                        // get the audit percent record in which the current date comes
                                        var currentAuditPercentRecord = CurrentInstallerAuditPercentList.Where(o => o.EndDay >= workDay && o.StartDay <= workDay).FirstOrDefault();

                                        if (currentAuditPercentRecord != null)
                                        {
                                            // get index of current record.
                                            int indexofCurrentDayRecord = CurrentInstallerAuditPercentList.IndexOf(currentAuditPercentRecord);

                                            if (indexofCurrentDayRecord == 0)
                                            {
                                                foreach (var dayItem in GivenDateAuditList)
                                                {
                                                    dayItem.AuditPercent = currentAuditPercentRecord.AuditPercentage;
                                                    objCompassEntities.SaveChanges();
                                                }
                                            }
                                            else
                                            {

                                                //get last cycle 
                                                var oldRecord = CurrentInstallerAuditPercentList[indexofCurrentDayRecord - 1];

                                                //if last cycle as already fail percentage applied then no need to 
                                                //check the percentage directly apply current percentage.
                                                if (oldRecord.IsFailurePercentageApplied == 1)
                                                {

                                                    foreach (var dayItem in GivenDateAuditList)
                                                    {
                                                        dayItem.AuditPercent = currentAuditPercentRecord.AuditPercentage;
                                                        objCompassEntities.SaveChanges();
                                                    }

                                                }
                                                else
                                                {
                                                    ///check last cycle has audit calculated if no then calculate
                                                    if (oldRecord.EndDay == (workDay - 1) && oldRecord.IsFailurePercentageApplied != 1)
                                                    {
                                                        var CurrentDateList = objCompassEntities.PROC_Audit_GETWorkingDayDetails(PId, installerItem).ToList();

                                                        var cycleStartDate = CurrentDateList.Where(o => o.Id == oldRecord.StartDay).Select(o => o.VisitDateTime).FirstOrDefault();
                                                        var cycleEndDate = CurrentDateList.Where(o => o.Id == oldRecord.EndDay).Select(o => o.VisitDateTime).FirstOrDefault();


                                                        /// get the count of all the records needed for calculation
                                                        /// of the last selected cycle
                                                        var CurrentCycleStatus = objCompassEntities.PROC_Audit_PercentCalculation(PId, installerItem, cycleStartDate, cycleEndDate).ToList();


                                                        /// if records found for the old cycle
                                                        if (CurrentCycleStatus != null || CurrentCycleStatus.Count > 0)
                                                        {
                                                            var passed = CurrentCycleStatus.Where(o => o.Type == "Passed").Select(o => o.NoOfRecords).FirstOrDefault();
                                                            passed = passed == null ? 0 : passed;
                                                            var failed = CurrentCycleStatus.Where(o => o.Type == "Fail").Select(o => o.NoOfRecords).FirstOrDefault();
                                                            failed = failed == null ? 0 : failed;
                                                            var Complete = CurrentCycleStatus.Where(o => o.Type == "Complete").Select(o => o.NoOfRecords).FirstOrDefault();
                                                            Complete = Complete == null ? 0 : Complete;

                                                            // check record avilable for audit last cycle
                                                            if (Complete > 0)
                                                            {


                                                                //check fail greater than 0 for last audit cycle.
                                                                if (failed > 0)
                                                                {
                                                                    var failPercentage = (failed * 100 / (passed + failed));

                                                                    var checkRecordPresent = FailPercentList.Where(o => o.FailPercentageStart < failPercentage
                                                                        && o.FailPercentageEnd >= failPercentage).FirstOrDefault();
                                                                    if (checkRecordPresent == null)
                                                                    {
                                                                        // set current audit cycle percent for  all the records of the current day.
                                                                        foreach (var dayItem in GivenDateAuditList)
                                                                        {
                                                                            dayItem.AuditPercent = currentAuditPercentRecord.AuditPercentage;
                                                                            objCompassEntities.SaveChanges();
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        //increment days in last old audit cycle.
                                                                        var IncrementDays = checkRecordPresent.IncreaseByDays;


                                                                        var newDayChangeRecordList = CurrentInstallerAuditPercentList;

                                                                        //change the installer schedule by adding the fail percent. 
                                                                        for (int i = (indexofCurrentDayRecord - 1); i < CurrentInstallerAuditPercentList.Count(); i++)
                                                                        {
                                                                            long id = CurrentInstallerAuditPercentList[i].Id;
                                                                            var recordtoChange = objCompassEntities.AuditInstallerPercentages.Where(o => o.Id == id).FirstOrDefault();


                                                                            if (i == (indexofCurrentDayRecord - 1))
                                                                            {
                                                                                recordtoChange.EndDay = recordtoChange.EndDay + checkRecordPresent.IncreaseByDays;
                                                                                recordtoChange.IsFailurePercentageApplied = 1;
                                                                                objCompassEntities.SaveChanges();
                                                                            }
                                                                            else
                                                                            {
                                                                                recordtoChange.StartDay = recordtoChange.StartDay + checkRecordPresent.IncreaseByDays;
                                                                                recordtoChange.EndDay = recordtoChange.EndDay + checkRecordPresent.IncreaseByDays;
                                                                                objCompassEntities.SaveChanges();
                                                                            }
                                                                        }


                                                                        // set current audit cycle percent for  all the records of the current day.
                                                                        foreach (var dayItem in GivenDateAuditList)
                                                                        {
                                                                            dayItem.AuditPercent = oldRecord.AuditPercentage;
                                                                            objCompassEntities.SaveChanges();
                                                                        }

                                                                        /// goto outer label after the fail percentage found
                                                                        /// restart from  begining for the project.

                                                                        goto Outer;

                                                                    }
                                                                    //end of fail percentage loop.



                                                                }
                                                                else
                                                                {
                                                                    // set current audit cycle percent for  all the records of the current day.
                                                                    foreach (var dayItem in GivenDateAuditList)
                                                                    {
                                                                        dayItem.AuditPercent = currentAuditPercentRecord.AuditPercentage;
                                                                        objCompassEntities.SaveChanges();
                                                                    }
                                                                }
                                                                //check 
                                                            }
                                                            else
                                                            {

                                                                // set current audit cycle percent for  all the records of the current day.
                                                                foreach (var dayItem in GivenDateAuditList)
                                                                {
                                                                    dayItem.AuditPercent = currentAuditPercentRecord.AuditPercentage;
                                                                    objCompassEntities.SaveChanges();
                                                                }
                                                            }
                                                            //end of else loop of complete>0 

                                                        }
                                                        else
                                                        {
                                                            // set current audit cycle percent for  all the records of the current day.
                                                            foreach (var dayItem in GivenDateAuditList)
                                                            {
                                                                dayItem.AuditPercent = currentAuditPercentRecord.AuditPercentage;
                                                                objCompassEntities.SaveChanges();
                                                            }
                                                        }
                                                        /// end of else loop of old audit cycle 


                                                    }
                                                    else
                                                    {

                                                        // set current audit cycle percent for  all the records of the current day.
                                                        foreach (var dayItem in GivenDateAuditList)
                                                        {
                                                            dayItem.AuditPercent = currentAuditPercentRecord.AuditPercentage;
                                                            objCompassEntities.SaveChanges();
                                                        }

                                                    }
                                                    // end of else loop of if loop for oldRecord.IsFailurePercentageApplied==1

                                                }
                                                //end of else loop of old record fail percent not applied.


                                            }
                                            //end of else of indexofCurrentDayRecord loop


                                        }
                                        else
                                        {

                                            /// audit percent not present for the day.
                                            /// fire mail


                                            new clsMail().SendMail(PId, installerItem);

                                        }
                                        //  end of else loop of audit percent present.


                                    }//end of foreach of date loop


                                    // }
                                    //end of else of  CurrentInstallerAuditPercentList.Count()>1


                                }
                                else
                                {
                                    // no audit percent present for current installer.


                                }

                                #endregion

                            }//end of installer list for each loop


                            #endregion

                        }// end of else of audit project condition





                    }// end of project for each loop


                }
                clsErrorLogFile.writeStatus("AddToAudit Ended at -" + DateTime.Now.ToShortTimeString());
            }
            catch (Exception ex)
            {
                clsErrorLogFile.writeStatus("AddToAudit Method exception occured at -" + DateTime.Now.ToShortTimeString());
                clsErrorLogFile.writeStatus("AddToAudit Method exception ex message-" + ex.Message);
                clsErrorLogFile.writeStatus("AddToAudit Method exception ex inner exception message-" + Convert.ToString(ex.InnerException.Message));
            }


            clsErrorLogFile.writeStatus("AddToAudit method exit at -" + DateTime.Now.ToShortTimeString());

        }


    }

}
