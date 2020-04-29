using GarronT.CMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GarronT.CMS.UI.WebAPI
{
    public class AuditAllocationController : ApiController
    {

        public void AllocateAuditVisit(long InstallerId)
        {

            CompassEntities objCompassEntities = new CompassEntities();


            var DataList = objCompassEntities.AuditDatas.Where(o => o.Fk_InstallerId == InstallerId && o.AddedForAudit == 0).ToList();



            if (DataList != null && DataList.Count > 0)
            {
                var projectIdList = DataList.Select(o => o.FK_ProjectId).Distinct().ToList();

                foreach (var PId in projectIdList)
                {

                    var project = objCompassEntities.tblProjects.Where(a => a.ProjectId == PId && a.Active == 1 && a.ProjectStatus == "Active").FirstOrDefault();
                    if (project != null)
                    {

                        var AuditProject = objCompassEntities.AuditMasters.Where(o => o.FKProjectId == PId && o.Active == 1).FirstOrDefault();
                        if (AuditProject != null)
                        {
                            var AuditorList = objCompassEntities.AuditorMaps.Where(o => o.Active == 1 && o.FK_AuditId == AuditProject.Id && o.FK_UserId != InstallerId).ToList();


                            int AuditorCount = AuditorList.Count();
                            if (AuditorCount > 0)
                            {
                                if (AuditorCount == 1)
                                {
                                    foreach (var item in DataList.Where(o => o.FK_ProjectId == PId).ToList())
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
                                        objCompassEntities.SaveChanges();
                                    }
                                }
                                else
                                {

                                    int i = 0;

                                    foreach (var item in DataList.Where(o => o.FK_ProjectId == PId).ToList())
                                    {

                                        ProjectAuditAllocation obj = new ProjectAuditAllocation();

                                        obj.Active = 1;
                                        obj.CreatedOn = new CommonFunctions().ServerDate();

                                        obj.FK_AuditorId = AuditorList[i].FK_UserId;

                                        obj.Fk_ProjectId = PId;
                                        obj.FK_UploadedExcelData_Id = item.FK_UploadId;
                                        obj.FromDate = AuditProject.AuditStartDate;
                                        obj.ToDate = AuditProject.AuditEndDate;

                                        objCompassEntities.ProjectAuditAllocations.Add(obj);
                                        objCompassEntities.SaveChanges();

                                        var _data = objCompassEntities.AuditDatas.Where(o => o.Id == item.Id).FirstOrDefault();
                                        _data.AddedForAudit = 1;
                                        objCompassEntities.SaveChanges();

                                        i = i + 1 == AuditorCount ? 0 : i + 1;
                                    }
                                }

                            }
                        }


                    }
                }






            }
        }

    }
}