using GarronT.CMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNMPUploadUtility
{
    //internal class AccountStatus
    class AccountStatus
    {
        public bool SkipAccountMovedToUnAllocate(long ProjectId)
        {
            try
            {
                #region getting skipped data
                ErrorLogFile.writeStatus("Getting Skipped Data - " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Getting Skipped Data");

                CompassEntities dbcontext = new CompassEntities();
                var SkippedDataList = (from pf in dbcontext.TblProjectFieldDatas
                                       join u in dbcontext.UploadedExcelDatas
                                       on pf.FK_UploadedExcelData_Id equals u.Id
                                       where u.FKProjectId == ProjectId && u.Active == 1 && pf.Active == 1 && pf.IsSkipped == true
                                       select new SkippedList
                                       {
                                           AccountId = u.Id,
                                           InstallerId = pf.InstallerId
                                       }).Distinct().ToList();

                ErrorLogFile.writeStatus("Geted Skipped Count " + SkippedDataList.Count() + "- " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Geted Skipped Count " + SkippedDataList.Count() + "");
                int i = 0;
                foreach (var item in SkippedDataList)
                {
                    bool flag=true;
                    var objTPFD = dbcontext.TblProjectFieldDatas.Where(o => o.FK_UploadedExcelData_Id == item.AccountId && o.Active == 1 && o.InstallerId == item.InstallerId && o.IsSkipped == true).OrderByDescending(o => o.ID).ToList();
                    foreach (var itemRow in objTPFD)
                    {
                        itemRow.Active = 0;
                        itemRow.ModifiedOn = new CommonFunctions().ServerDate();
                        dbcontext.SaveChanges();

                        #region history inserted
                        if (flag)       // Last record insert in History  Table
                        {
                            TblProjectFieldDataHistory objTblProjectFieldDataHistory = new TblProjectFieldDataHistory();
                            objTblProjectFieldDataHistory.Active = 1;
                            objTblProjectFieldDataHistory.CreatedBy = 1;
                            objTblProjectFieldDataHistory.CreatedOn = new CommonFunctions().ServerDate();
                            objTblProjectFieldDataHistory.FormId = itemRow.FormId;
                            objTblProjectFieldDataHistory.InstallerId = itemRow.InstallerId;
                            objTblProjectFieldDataHistory.InstallerMapId = itemRow.InstallerId;
                            objTblProjectFieldDataHistory.IsReAssign = itemRow.IsReAssign;
                            objTblProjectFieldDataHistory.IsSkipped = itemRow.IsSkipped;
                            objTblProjectFieldDataHistory.JsonDataVal = itemRow.JsonDataVal;
                            objTblProjectFieldDataHistory.SkipComment = itemRow.SkipComment;
                            objTblProjectFieldDataHistory.SkipId = itemRow.SkipId;
                            objTblProjectFieldDataHistory.SkippedDatetime = itemRow.SkippedDatetime;
                            objTblProjectFieldDataHistory.SkippedReason = itemRow.SkippedReason;
                            // objTblProjectFieldDataHistory.tblUploadedData_Id = objTblProjectFieldData.tblUploadedData_Id;
                            objTblProjectFieldDataHistory.FK_UploadedExcelData_Id = itemRow.FK_UploadedExcelData_Id;
                            objTblProjectFieldDataHistory.visitdatetime = itemRow.visitdatetime;

                            dbcontext.TblProjectFieldDataHistories.Add(objTblProjectFieldDataHistory);
                            dbcontext.SaveChanges();

                            TblUnAllocatedSkipHistory objTblUnAllocatedSkipHistory = new TblUnAllocatedSkipHistory();
                            objTblUnAllocatedSkipHistory.FK_UploadedExcelData_Id= itemRow.FK_UploadedExcelData_Id;
                            objTblUnAllocatedSkipHistory.TblProjectFieldData_Id = itemRow.ID;
                            objTblUnAllocatedSkipHistory.InstallerId = itemRow.InstallerId;
                            objTblUnAllocatedSkipHistory.InstallerName = dbcontext.tblUsers.Where(o => o.UserID == itemRow.InstallerId).Select(o => o.FirstName).FirstOrDefault();
                            objTblUnAllocatedSkipHistory.visitdatetime=itemRow.visitdatetime;
                            objTblUnAllocatedSkipHistory.SkippedReason = itemRow.SkippedReason;
                            objTblUnAllocatedSkipHistory.SkipComment = itemRow.SkipComment;
                            objTblUnAllocatedSkipHistory.Active = 1;
                            objTblUnAllocatedSkipHistory.CreatedBy = 1;
                            objTblUnAllocatedSkipHistory.CreatedOn= new CommonFunctions().ServerDate();
                            dbcontext.TblUnAllocatedSkipHistories.Add(objTblUnAllocatedSkipHistory);
                            dbcontext.SaveChanges();

                            flag = false;
                        }
                        #endregion
                    }
                    var objIMD = dbcontext.tblInstallerMapDatas.Where(o => o.FK_UploadedExcelData_Id == item.AccountId && o.Active == 1 && o.InstallerId == item.InstallerId).ToList();
                    foreach (var objItemRow in objIMD)
                    {
                        objItemRow.Active = 0;
                        objItemRow.BatchNumber = null;
                        objItemRow.MobileResponse = null;
                        objItemRow.ModifiedOn = new CommonFunctions().ServerDate();
                        dbcontext.SaveChanges();
                    }
                    i++;
                }
                ErrorLogFile.writeStatus("Number of account moved to UnAllocate Count:- " + i + "- " + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Number of account moved to UnAllocate Count:- " + i + "");
                return true;
                #endregion
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                ErrorLogFile.writeStatus("Error:- " + ex.Message.ToString() + DateTime.Now.ToShortTimeString());
                Console.WriteLine("Error:- " + ex.Message.ToString());
                return false;
            }
        }
    }

    class SkippedList
    {
        public long AccountId { get; set; }
        public long InstallerId { set; get; }
    }
}
