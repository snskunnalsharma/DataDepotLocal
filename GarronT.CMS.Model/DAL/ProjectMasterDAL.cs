using AutoMapper;
using GarronT.CMS.Model.BO;
using GoogleMaps.LocationServices;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml;
using System.Xml.Linq;


namespace GarronT.CMS.Model.DAL
{
    public class ProjectMasterDAL : IDisposable
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




        public List<ProjectModel> GetProjectNamebyClientId(int ClientId)
        {
            // DateTime selDatenew = Convert.ToDateTime(selDate);
            CompassEntities CompassEntities = new CompassEntities();
            List<ProjectModel> data = (from b in CompassEntities.tblProjects
                                       where b.ClientId == ClientId & b.Active == 1
                                       //&& (b.FromDate.Value.Date >= selDatenew.Date && b.ToDate.Value.Date <= selDatenew.Date)
                                       select new ProjectModel
                                       {
                                           ProjectId = b.ProjectId,
                                           ProjectName = b.ProjectName
                                       }).ToList();
            return data;

            //List<ProjectModel> li = new List<ProjectModel>();
            //li.Add(new ProjectModel { ProjectName = "Project Abbott", ID = 1 });
            //li.Add(new ProjectModel { ProjectName = "Project Ace", ID = 4 });


            //return li;
        }

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

        public List<CityModel> GetActiveCityRecords(long stateId, string cityNameLike)
        {
            var list = new List<CityModel>();
            CompassEntities CompassEntities = new CompassEntities();

            list = CompassEntities.TblCityMasters.Where(o => o.StateId == stateId).Select(x => new CityModel { CityID = x.CityId, CityName = x.CityName }).ToList();
            return list;
        }



        public List<ProjectModel> GetProjectName()
        {
            // DateTime selDatenew = Convert.ToDateTime(selDate);
            CompassEntities CompassEntities = new CompassEntities();
            //List<ProjectModel> data = (from b in CompassEntities.tblProjects
            //                           where b.Active == 1
            //                           //&& (b.FromDate.Value.Date >= selDatenew.Date && b.ToDate.Value.Date <= selDatenew.Date)
            //                           select new ProjectModel
            //                           {
            //                               ProjectId = b.ProjectId,
            //                               ProjectName = b.ProjectName
            //                           }).ToList();
            //return data;
            DateTime currentdate = new CommonFunctions().ServerDate();
            //&& a.ToDate.Value.Date >= currentdate.Date
            List<ProjectModel> data = CompassEntities.STP_GetProjectCityState().Where(a => a.Active == 1).Select(o =>
                                      new ProjectModel
                                      {
                                          ProjectId = o.ProjectId,
                                          // ProjectName = o.ProjectName,
                                          ProjectCityState = o.ProjectCityName,
                                          stringFromDate = o.strFormDate,
                                          stringToDate = o.strToDate
                                      }).OrderBy(a => a.ProjectCityState).ToList();
            return data;

            //List<ProjectModel> li = new List<ProjectModel>();
            //li.Add(new ProjectModel { ProjectName = "Project Abbott", ID = 1 });
            //li.Add(new ProjectModel { ProjectName = "Project Ace", ID = 4 });


            //return li;
        }


        public List<ProjectModel> GetProjectNameFOrDashBoard(string UtilityType, string ProjectStatus)
        {
            CompassEntities CompassEntities = new CompassEntities();

            List<ProjectModel> data = CompassEntities.STP_GetProjectCityState().Where(a => a.Active == 1 && a.Utilitytype == UtilityType
                && a.ProjectStatus == ProjectStatus).Select(o =>
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

        public tblProject GetProjectOnDashboard(DateTime currentDate)
        {
            CompassEntities CompassEntities = new CompassEntities();

            tblProject data = CompassEntities.tblProjects.Where(o => o.FromDate <= currentDate.Date && currentDate.Date <= o.ToDate).FirstOrDefault();

            return data;
        }



        public List<ProjectModel> GetAllProjectName()
        {
            // DateTime selDatenew = Convert.ToDateTime(selDate);
            CompassEntities CompassEntities = new CompassEntities();
            //List<ProjectModel> data = (from b in CompassEntities.tblProjects
            //                           where b.Active == 1
            //                           //&& (b.FromDate.Value.Date >= selDatenew.Date && b.ToDate.Value.Date <= selDatenew.Date)
            //                           select new ProjectModel
            //                           {
            //                               ProjectId = b.ProjectId,
            //                               ProjectName = b.ProjectName
            //                           }).ToList();
            //return data;

            List<ProjectModel> data = CompassEntities.STP_GetProjectCityState().Select(o =>
                                        new ProjectModel
                                        {
                                            ProjectId = o.ProjectId,
                                            // ProjectName = o.ProjectName,
                                            ProjectCityState = o.ProjectCityName,
                                            stringFromDate = o.strFormDate,
                                            stringToDate = o.strToDate
                                        }).OrderBy(a => a.ProjectCityState).ToList();
            return data;

            //List<ProjectModel> li = new List<ProjectModel>();
            //li.Add(new ProjectModel { ProjectName = "Project Abbott", ID = 1 });
            //li.Add(new ProjectModel { ProjectName = "Project Ace", ID = 4 });


            //return li;
        }


        public List<ProjectModel> GetProjectNamebyDate(string fromDate, string toDate)
        {
            DateTime frmDatenew = Convert.ToDateTime(fromDate);
            DateTime toDatenew = Convert.ToDateTime(toDate);

            CompassEntities CompassEntities = new CompassEntities();
            List<ProjectModel> data = (from b in CompassEntities.tblProjects
                                       where b.Active == 1
                                        //&& (DbFunctions.TruncateTime(b.FromDate.Value) <= DbFunctions.TruncateTime(frmDatenew) && DbFunctions.TruncateTime(b.ToDate.Value) >= DbFunctions.TruncateTime(frmDatenew))
                                        && ((DbFunctions.TruncateTime(b.FromDate.Value) <= DbFunctions.TruncateTime(frmDatenew) && DbFunctions.TruncateTime(b.ToDate.Value) >= DbFunctions.TruncateTime(frmDatenew)) ||
                                          (DbFunctions.TruncateTime(b.FromDate.Value) <= DbFunctions.TruncateTime(toDatenew) && DbFunctions.TruncateTime(b.ToDate.Value) >= DbFunctions.TruncateTime(toDatenew)))
                                       select new ProjectModel
                                       {
                                           ProjectId = b.ProjectId,
                                           ProjectName = b.ProjectName
                                       }).ToList();
            return data;

            //List<ProjectModel> li = new List<ProjectModel>();
            //li.Add(new ProjectModel { ProjectName = "Project Abbott", ID = 1 });
            //li.Add(new ProjectModel { ProjectName = "Project Ace", ID = 4 });


            //return li;
        }
        public List<tblUploadedData> GetDataByAddress3(string address, long? id)
        {
            CompassEntities CompassEntities = new CompassEntities();

            List<tblUploadedData> d = CompassEntities.tblUploadedDatas.Where(x => x.Street.Contains(address) && x.ProjectId == id).OrderBy(f => f.Street).ToList();
            return d;

        }

        public List<tblUploadedData> GetAddress(List<string> Root, List<string> Cycle, long Projectid)
        {

            CompassEntities CompassEntities = new CompassEntities();
            List<tblUploadedData> objaddress = new List<tblUploadedData>();
            if (Root != null)
            {

                objaddress = CompassEntities.STP_GetVisitForProjectInstallation(Projectid).AsParallel().
                    Where(x => Root.Contains(x.Route) && Cycle.Contains(x.Cycle)).
                Select(o => new tblUploadedData
                {
                    Latitude = o.Latitude,
                    Longitude = o.Longitude,
                    ID = o.id,
                    Street = o.Street,
                }).OrderBy(a => a.Latitude).ThenBy(a => a.Longitude).ThenBy(a => a.Street).ToList();
                //objaddress.AddRange(data);


            }

            return objaddress;

        }

        public List<tblUploadedData> GetInstallerAddress(long projectid, List<string> Root, List<string> Cycle, DateTime fromDate, DateTime toDate, long installerId)
        {

            CompassEntities CompassEntities = new CompassEntities();

            List<tblUploadedData> data = CompassEntities.STP_GetVisitForUserInstallation(fromDate, toDate, installerId, projectid).ToList().
                  Where(x => Root.Contains(x.Route) && Cycle.Contains(x.Cycle)).
                Select(o => new tblUploadedData { ID = o.id, Street = o.Street, Latitude = o.Latitude, Longitude = o.Longitude }).ToList();

            return data;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ActiveStatus"></param>
        /// <returns></returns>
        public List<ProjectModel> GetProjectList(bool ActiveStatus)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            List<ProjectModel> data1 = new List<ProjectModel>();


            var data = objCompassEntities.VW_Project.Where(o => o.Active == (ActiveStatus == true ? 1 : 0)).AsEnumerable().OrderByDescending(o => o.CreatedOn).ThenByDescending(o => o.FromDate).ThenByDescending(o => o.ToDate).
                Select(o => new ProjectModel
                {
                    ProjectId = o.ProjectId,
                    ProjectName = o.ProjectName,
                    ClientName = o.ClientName,
                    /*CityName = o.cityname,
                    StateName = o.statename,   */ // But Sate and city name comming from procedure
                    stringFromDate = o.FromDate != null ? DateTime.Parse(o.FromDate.ToString()).ToString("MMM-dd-yyyy") : "",
                    stringToDate = o.ToDate != null ? DateTime.Parse(o.ToDate.ToString()).ToString("MMM-dd-yyyy") : "",
                    ProjectStatus = o.ProjectStatus,
                    IsCloneProject = o.IsCloned != null ? o.IsCloned.Value : false
                }).ToList();


            foreach (var item in data)  // Get all records or null lat or lon records
            {
                item.TotalRecords = objCompassEntities.UploadedExcelDatas.Where(o => o.FKProjectId == item.ProjectId && o.Active == 1).Count();
                var latLongCount = objCompassEntities.UploadedExcelDatas.Where(o => o.FKProjectId == item.ProjectId && o.Active == 1 && o.AddressLatitude != null
                    && o.AddressLongitude != null).Count();
                item.LatLongStatus = Convert.ToString(latLongCount) + " / " + Convert.ToString(item.TotalRecords);

                // Getting state and City name hare
                var proDetail = objCompassEntities.tblProjects.Where(o => o.ProjectId == item.ProjectId).FirstOrDefault();
                item.StateName = objCompassEntities.tblStateMasters.Where(o => o.StateId == proDetail.StateId).Select(o => o.StateName).FirstOrDefault();
                item.CityName = objCompassEntities.TblCityMasters.Where(o => o.CityId == proDetail.CityId && o.Active == 1).Select(o => o.CityName).FirstOrDefault();
            }

            data1 = data;
            return data1;
        }





        public bool CreateOrUpdate(ProjectModel objProject, out long proId, out string returnMessage)
        {
            bool result = false;
            returnMessage = "Unable to Create / Update State.";
            proId = 0;
            CompassEntities CompassEntities = new CompassEntities();
            try
            {
                ///insert
                if (objProject.ProjectId == 0 || objProject.CloneId > 0)
                {
                    #region Insert

                    var CheckProjectName = CompassEntities.tblProjects.Where(o => o.ClientId == objProject.ClientId && o.ProjectName == objProject.ProjectName).FirstOrDefault();

                    if (CheckProjectName != null)
                    {
                        returnMessage = "Save failed!. Project name already in use.";
                        result = false;
                    }
                    else
                    {
                        tblProject objtblProject = new tblProject();
                        objtblProject.ProjectName = objProject.ProjectName;
                        objtblProject.ProjectDecsription = objProject.ProjectDescription;
                        objtblProject.IsLatLongCalculated = 0;  //new added
                        objtblProject.ClientId = objProject.ClientId;
                        objtblProject.FromDate = objProject.FromDate;
                        objtblProject.ToDate = objProject.ToDate;
                        objtblProject.StateId = objProject.StateId;
                        objtblProject.CityId = objProject.CityId;
                        objtblProject.ContactId = objProject.ContactId;
                        objtblProject.Utilitytype = objProject.Utilitytype == "1" ? "Electric" : (objProject.Utilitytype == "2" ? "Gas" : "Water");
                        objtblProject.CreatedBy = objProject.CreatedBy;
                        objtblProject.Active = 1;
                        objtblProject.CreatedOn = objtblProject.CreatedOn;
                        objtblProject.ProjectStatus = "Active";
                        objtblProject.DataSyncConnection = objProject.ConnectionType;
                        objtblProject.AutoDataSyncInterval = objProject.dataSynch;
                        objtblProject.AppointmentUrl = objProject.ProjectAppointmentUrl;   //Added by sominath on 23/12/2016
                        objtblProject.PartialSynch = (objProject.SyncType == 1 ? 1 : 0);
                        objtblProject.mapBy = objProject.valueMapBy;


                        /// added by bharat to manage the inventory on off for the project 
                        objtblProject.ManageInventory = objProject.ManageInventory;

                        if (objProject.objWarehouseArray != null && objProject.objWarehouseArray.Count() > 0)
                        {

                            foreach (var item1 in objProject.objWarehouseArray)
                            {
                                ProjectWarehouseRelation obj = new ProjectWarehouseRelation();
                                obj.FKProjectId = objtblProject.ProjectId;
                                obj.FKWarehouseId = item1;
                                obj.Active = 1;
                                obj.CreatedOn = objtblProject.CreatedOn;
                                obj.CreatedBy = objtblProject.CreatedBy;
                                objtblProject.ProjectWarehouseRelations.Add(obj);
                            }


                        }

                        if (objProject.CloneId > 0)
                        {
                            objtblProject.IsCloned = true;
                            objtblProject.CloneProjectId = objtblProject.ProjectId;
                        }
                        else
                        {
                            objtblProject.IsCloned = false;
                        }

                        CompassEntities.tblProjects.Add(objtblProject);
                        CompassEntities.SaveChanges();

                        if (objProject.CloneId > 0)
                        {
                            SaveClone(objtblProject.ProjectId, objProject.CloneId, objProject.CreatedBy);
                        }

                        returnMessage = CommonFunctions.strRecordCreated;
                        result = true;
                        proId = objtblProject.ProjectId;

                    }






                    #endregion
                }
                else
                {
                    #region  update

                    /*First check inventory present for the project if yes & inventory manage flag is false then return false*/

                    ///update
                    string utility = objProject.Utilitytype == "1" ? "Electric" : (objProject.Utilitytype == "2" ? "Gas" : "Water");

                    tblProject objtblProject = CompassEntities.tblProjects.Where(o => o.ProjectId == objProject.ProjectId).FirstOrDefault();


                    var InventroyCheck = true;




                    if (objtblProject.ManageInventory == "Yes" && objProject.ManageInventory == "No")
                    {

                        var CheckCount = CompassEntities.StockDetails.Where(o => o.Active == 1 && o.FKProjectId == objtblProject.ProjectId).FirstOrDefault();

                        if (CheckCount != null)
                        {
                            InventroyCheck = false;
                            returnMessage = "Save failed. You can not change the inventory status for the project.";
                        }
                    }
                    else if (objtblProject.ManageInventory == "Yes" && objProject.ManageInventory == "No")
                    {
                        var CheckCount = CompassEntities.ProjectInventoryChangeStatus(objtblProject.ProjectId).FirstOrDefault();

                        if (CheckCount == null || CheckCount == 0)
                        {
                            InventroyCheck = false;
                            returnMessage = "Save failed. You can not change the inventory status for the project. Please remove allocated visit first & then try.";
                        }
                    }

                    var CheckProjectName = CompassEntities.tblProjects.Where(o => o.ClientId == objProject.ClientId && o.ProjectName == objProject.ProjectName
                        && o.ProjectId != objProject.ProjectId).FirstOrDefault();

                    if (CheckProjectName != null)
                    {
                        returnMessage = "Update failed!. Project name already in use.";
                        InventroyCheck = false;
                    }

                    if (InventroyCheck == false)
                    {
                        result = false;
                    }
                    else
                    {


                        var objAuditProject = CompassEntities.AuditMasters.Where(o => o.FKProjectId == objProject.ProjectId).FirstOrDefault();

                        if (objtblProject.ClientId != objProject.ClientId && objAuditProject != null)
                        {
                            result = false;
                            returnMessage = "Save failed. Customer can not be changed as the Audit project created for the current project.";

                        }
                        else
                        {
                            objtblProject.ProjectName = objProject.ProjectName;
                            objtblProject.ProjectDecsription = objProject.ProjectDescription;
                            objtblProject.ClientId = objProject.ClientId;
                            objtblProject.FromDate = objProject.FromDate;
                            objtblProject.ToDate = objProject.ToDate;
                            objtblProject.StateId = objProject.StateId;
                            objtblProject.CityId = objProject.CityId;
                            objtblProject.ContactId = objProject.ContactId;
                            objtblProject.ModifiedBy = objProject.ModifiedBy;
                            objtblProject.Utilitytype = objProject.Utilitytype == "1" ? "Electric" : (objProject.Utilitytype == "2" ? "Gas" : "Water");
                            objtblProject.Active = 1;
                            objtblProject.ModifiedOn = objtblProject.CreatedOn;
                            objtblProject.DataSyncConnection = objProject.ConnectionType;
                            objtblProject.AutoDataSyncInterval = objProject.dataSynch;
                            objtblProject.PartialSynch = (objProject.SyncType == 1 ? 1 : 0);
                            objtblProject.AppointmentUrl = objProject.ProjectAppointmentUrl;   //Added by sominath on 23/12/2016
                            objtblProject.mapBy = objProject.valueMapBy;

                            /// added by bharat to manage the inventory on off for the project
                            objtblProject.ManageInventory = objProject.ManageInventory;

                            foreach (var item in objtblProject.ProjectWarehouseRelations.Where(o => o.Active == 1).ToList())
                            {
                                item.Active = 0;
                                item.ModifiedOn = objtblProject.ModifiedOn;
                                item.ModifiedBy = objtblProject.ModifiedBy;
                            }

                            if (objProject.objWarehouseArray != null && objProject.objWarehouseArray.Count() > 0)
                            {

                                foreach (var item1 in objProject.objWarehouseArray)
                                {
                                    var existingObj = objtblProject.ProjectWarehouseRelations.Where(o => o.FKWarehouseId == item1).FirstOrDefault();

                                    if (existingObj == null)
                                    {
                                        ProjectWarehouseRelation obj = new ProjectWarehouseRelation
                                        {
                                            FKProjectId = objtblProject.ProjectId,
                                            FKWarehouseId = item1,
                                            Active = 1,
                                            CreatedOn = objtblProject.CreatedOn,
                                            CreatedBy = objtblProject.CreatedBy
                                        };

                                        objtblProject.ProjectWarehouseRelations.Add(obj);
                                    }
                                    else
                                    {
                                        existingObj.Active = 1;
                                        existingObj.ModifiedOn = objtblProject.ModifiedOn;
                                        existingObj.ModifiedBy = objtblProject.ModifiedBy;
                                    }
                                }


                            }
                            CompassEntities.SaveChanges();
                            result = true;
                            returnMessage = CommonFunctions.strRecordUpdated;
                        }
                    }
                    proId = objtblProject.ProjectId;

                    #endregion

                }

            }
            catch (Exception ex)
            {

                returnMessage += "<br/>" + ex.Message;
                result = false;
            }
            return result;
        }




        public bool SaveClone(long NewProjectId, long OldProjectId, int currentUserId)
        {


            try
            {



                /*Tab 2 Get & Save methds*/
                var objMeterTypeList = getMeterTypeListCurrentProject(OldProjectId).Select(o => o.ID.ToString()).ToArray();
                var objMeterSizeList = getMetersizeListCurrentProject(OldProjectId).Select(o => o.ID.ToString()).ToArray();
                var objMeterMakeModelList = getMeterMakeListCurrentProject(OldProjectId).Select(o => o.ID.ToString()).ToArray();
                var objServiceList = getServiceListCurrentProject(OldProjectId).Select(o => o.ID.ToString()).ToArray();



                string result;
                bool a = CreateOrUpdateMasterField(1, NewProjectId, currentUserId, objMeterSizeList, out result);

                bool b = CreateOrUpdateMasterField(2, NewProjectId, currentUserId, objMeterTypeList, out result);

                bool c = CreateOrUpdateMasterField(3, NewProjectId, currentUserId, objServiceList, out result);

                bool e = CreateOrUpdateMasterField(4, NewProjectId, currentUserId, objMeterMakeModelList, out result);

                using (UserRoleDAL userRoleDAL = new UserRoleDAL())
                {
                    var objUsersSmallModelList = (
                        from a1 in userRoleDAL.GeInstallerListCurrentProject(OldProjectId)
                        select a1.UserId.ToString()).ToArray();

                    bool d = new UserRoleDAL().CreateOrUpdateMasterField(NewProjectId, currentUserId, objUsersSmallModelList, out result);
                }

                // add new  line
                bool f = CreateOrUpdateMeterSizePayRete(5, NewProjectId, currentUserId, objMeterSizeList, out result);

                /*Tab 3 Get & Save Methods*/


                var objList1 = GetActiveServiceRecords(OldProjectId);
                var objList2 = GetInActiveServiceRecords(OldProjectId);
                //var objList4 = GetActiveMeterSizePayRates(OldProjectId);

                Tab3BO objTab3BO = new Tab3BO();
                objTab3BO.Servicedata = new List<ProjectServiceBO>();
                foreach (var item in objList1)
                {
                    var serviceObject = new ProjectServiceBO
                    {
                        serviceId = item.ServiceId,
                        servicePrice = decimal.Parse(item.ServiceBasePrice),
                        imageCaptureRequired = item.ImageFlag.Value.ToString(),
                        AssignedProductList = item.productArray
                    };
                    objTab3BO.Servicedata.Add(serviceObject);
                }
                foreach (var item in objList2)
                {
                    var serviceObject = new ProjectServiceBO
                    {
                        serviceId = item.ServiceId,
                        servicePrice = decimal.Parse(item.ServiceBasePrice),
                        imageCaptureRequired = item.ImageFlag.Value.ToString(),
                        AssignedProductList = item.productArray
                    };
                    objTab3BO.Servicedata.Add(serviceObject);
                }

                objTab3BO.projectId = NewProjectId;

                var Tab3Result = SaveTab3Records(objTab3BO, currentUserId);



                /*Tab 4 Get & Save Methods*/


                string[] objListTab4List = GetSelectedStandardFields(OldProjectId).Select(o => o.StandardFieldName).ToArray();

                bool Tab4Result = CreateOrUpdateStandardFields(NewProjectId, 1, objListTab4List, out result);




                /*Tab 5 Get & Save Methods*/

                List<CustomFieldModel> objTab5List = GetCustomFieldRecordList(true, OldProjectId);

                foreach (var item in objTab5List)
                {

                    TblProjectCustomeFieldReletion obj = new TblProjectCustomeFieldReletion();
                    obj.Active = 1;
                    obj.CreatedBy = currentUserId;
                    obj.CreatedOn = new CommonFunctions().ServerDate();
                    obj.FieldName = item.FieldName;
                    obj.ProjectID = NewProjectId;


                    bool saveresult = C_U_D_CustomFields(obj, 1, out result);
                }

                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<MeterMakeModel> getMeterMakeList()
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<MeterMakeModel> objMeterMakeModel = (from b in CompassEntities.tblMeterMakes
                                                          where b.Active == 1
                                                          select new MeterMakeModel
                                                          {
                                                              ID = b.ID,
                                                              Make = b.Make
                                                          }).OrderBy(t => t.Make).ToList();

                return objMeterMakeModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public List<MeterMakeModel> getMeterMakeListCurrentProject(long ProjectId)
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<MeterMakeModel> MeterMakeModel = (from b in CompassEntities.TblProjectMasterFieldRelations
                                                       where b.Active == 1 && b.ProjectID == ProjectId && b.MasterTableName == "tblMeterMake"
                                                       select new MeterMakeModel
                                                       {
                                                           ID = b.MasterFieldID
                                                       }).ToList();

                return MeterMakeModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MeterTypeModel> getMeterTypeList()
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<MeterTypeModel> objMeterTypeModel = (from b in CompassEntities.tblMeterTypes
                                                          where b.Active == 1
                                                          select new MeterTypeModel
                                                          {
                                                              ID = b.ID,
                                                              MeterType = b.MeterType
                                                          }).OrderBy(t => t.MeterType).ToList();

                return objMeterTypeModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public List<MeterTypeModel> getMeterTypeListCurrentProject(long ProjectId)
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<MeterTypeModel> MeterSizeModel = (from b in CompassEntities.TblProjectMasterFieldRelations
                                                       where b.Active == 1 && b.ProjectID == ProjectId && b.MasterTableName == "tblMeterType"
                                                       select new MeterTypeModel
                                                       {
                                                           ID = b.MasterFieldID
                                                       }).ToList();

                return MeterSizeModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<MobileWarehouseBO> GetWarehouseList()
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<MobileWarehouseBO> MobileWarehouseBOModel = CompassEntities.WarehouseMasters.AsNoTracking().Where(o => o.Active == 1).
                                                Select(o => new MobileWarehouseBO
                                                {
                                                    Id = o.Id,
                                                    WarehouseName = o.WarehouseName
                                                }).OrderBy(t => t.WarehouseName).ToList();

                return MobileWarehouseBOModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<MeterSizeModel> getMetersizeList()
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<MeterSizeModel> MeterSizeModel = (from b in CompassEntities.tblMeterSizes
                                                       where b.Active == 1
                                                       select new MeterSizeModel
                                                       {
                                                           ID = b.ID,
                                                           MeterSize = b.MeterSize
                                                       }).OrderBy(t => t.MeterSize).ToList();

                return MeterSizeModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public List<MeterSizeModel> getMetersizeListCurrentProject(long ProjectId)
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<MeterSizeModel> MeterSizeModel = (from b in CompassEntities.TblProjectMasterFieldRelations
                                                       where b.Active == 1 && b.ProjectID == ProjectId && b.MasterTableName == "tblMeterSize"
                                                       select new MeterSizeModel
                                                       {
                                                           ID = b.MasterFieldID
                                                       }).ToList();

                return MeterSizeModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ServicesModel> getServiceList()
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<ServicesModel> ServicesModel = (from b in CompassEntities.tblServices
                                                     where b.Active == 1
                                                     select new ServicesModel
                                                     {
                                                         ID = b.ID,
                                                         Service = b.Service
                                                     }).OrderBy(t => t.Service).ToList();

                return ServicesModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UsersSmallModel> GetUserList()
        {
            List<UsersSmallModel> list = new List<UsersSmallModel>();
            CompassEntities compassEntities = new CompassEntities();
            list = (from o in compassEntities.tblUsers
                    where o.Active == 1
                    select new UsersSmallModel { UserId = o.UserID, UserName = o.FirstName }).ToList();

            return list;

        }



        /// <summary>
        /// gets service list for selected service
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public List<ServicesModel> getServiceListCurrentProject(long ProjectId)
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<ServicesModel> MeterSizeModel = (from b in CompassEntities.TblProjectMasterFieldRelations
                                                      where b.Active == 1 && b.ProjectID == ProjectId && b.MasterTableName == "tblService"
                                                      && b.IsAdditionalService == false
                                                      select new ServicesModel
                                                      {
                                                          ID = b.MasterFieldID
                                                      }).ToList();

                return MeterSizeModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public List<ClientModel> GetActiveClientRecords()
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<ClientModel> objClentModelBo = (from b in CompassEntities.tblClients
                                                     where b.Active == 1
                                                     select new ClientModel
                                                     {
                                                         ClientId = b.ClientId,
                                                         ClientName = b.ClientName
                                                     }).OrderBy(t => t.ClientName).ToList();

                return objClentModelBo;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public List<ContactModelDDL> GetActiveContactRecords(long ClientId)
        {
            try
            {
                CompassEntities objCompassEntities = new CompassEntities();
                List<ContactModelDDL> objContactModelBo = objCompassEntities.tblClientContactDetails.AsEnumerable().
                    Where(o => o.Active == 1 && o.tblClient_ClientId == ClientId).Select(o => new ContactModelDDL
                    {
                        ContactId = o.ID,
                        ContactName = o.FirstName + " " + o.LastName
                    }).OrderBy(t => t.ContactName).ToList();
                return objContactModelBo;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<ServiceSmallModel> GetActiveServiceRecords(long projectId)
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<ServiceSmallModel> objServiceSmallModelBo = CompassEntities.TblProjectMasterFieldRelations.AsEnumerable().
                    Where(o => o.Active == 1 && o.ProjectID == projectId && o.MasterTableName == "tblService" && o.IsAdditionalService == false)
                    .Select(o =>
                        new ServiceSmallModel
                        {
                            ServiceId = o.ID,
                            MasterFieldId = o.MasterFieldID,
                            ServiceName = CompassEntities.tblServices.Where(o1 => o1.ID == o.MasterFieldID).Select(o1 => o1.Service).FirstOrDefault(),
                            ServiceBasePrice = o.BasePrice.ToString(),
                            ImageFlag = o.ImageFlag
                        }).OrderBy(t => t.ServiceName).ToList();

                foreach (var item in objServiceSmallModelBo)
                {
                    var a = CompassEntities.TblProjectMasterFieldRelations.Where(o => o.ID == item.ServiceId).FirstOrDefault();
                    var picList = CompassEntities.VW_GetPictures.Where(o => o.tblService_ServiceId == a.MasterFieldID).OrderBy(o => o.Picture).Select(o => o.Picture).ToList();
                    //item.isImageMapped = picList.Count == 0 ? false : true;

                    if (picList.Count == 0)
                    {
                        item.isImageMapped = false;
                    }
                    else
                    {
                        item.isImageMapped = true;
                    }
                    item.productArray = CompassEntities.ProjectServiceProductRelations.AsNoTracking().
                      Where(o => o.Active == 1 && o.FKProjectID == projectId && o.FKServiceId == item.MasterFieldId).Select(o => o.FKProductId).ToArray();
                }

                return objServiceSmallModelBo;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<ServiceNotSelectedModel> GetInActiveServiceRecords(long projectId)
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<ServiceNotSelectedModel> objServiceSmallModelBo = CompassEntities.TblProjectMasterFieldRelations.AsNoTracking().
                    Where(b => b.Active == 1 && b.ProjectID == projectId && b.MasterTableName == "tblService" && b.IsAdditionalService == true).
                    Select(b =>
                        new ServiceNotSelectedModel
                        {
                            ServiceId = b.ID,
                            MasterFieldId = b.MasterFieldID,
                            ServiceName = CompassEntities.tblServices.Where(o => o.ID == b.MasterFieldID).Select(o => o.Service).FirstOrDefault(),
                            ServiceBasePrice = b.BasePrice.ToString(),
                            ImageFlag = b.ImageFlag
                        }).OrderBy(t => t.ServiceName).ToList();

                foreach (var item in objServiceSmallModelBo)
                {
                    var a = CompassEntities.TblProjectMasterFieldRelations.Where(o => o.ID == item.ServiceId).FirstOrDefault();
                    var picList = CompassEntities.VW_GetPictures.Where(o => o.tblService_ServiceId == a.MasterFieldID).OrderBy(o => o.Picture).Select(o => o.Picture).ToList();
                    //item.isImageMapped = picList.Count == 0 ? false : true;
                    if (picList.Count == 0)
                    {
                        item.isImageMapped = false;
                    }
                    else
                    {
                        item.isImageMapped = true;
                    }

                    item.productArray = CompassEntities.ProjectServiceProductRelations.AsNoTracking().
                     Where(o => o.Active == 1 && o.FKProjectID == projectId && o.FKServiceId == item.MasterFieldId).Select(o => o.FKProductId).ToArray();
                }


                return objServiceSmallModelBo;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<MeterSizePayRateBo> GetActiveMeterSizePayRates(long projectId)
        {
            try
            {

                CompassEntities dbcontext = new CompassEntities();
                SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                List<MeterSizePayRateBo> objMeterSizePayRateBo = new List<MeterSizePayRateBo>();
                sqlConnection1.Open();
                SqlCommand cnn = new SqlCommand("PROC_GetMeterSizeOrder", sqlConnection1);
                cnn.Parameters.AddWithValue("@projectId", projectId);
                cnn.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sda = new SqlDataAdapter(cnn);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        objMeterSizePayRateBo.Add(new MeterSizePayRateBo
                        {
                            Id = long.Parse(row["ID"].ToString()),
                            MeterSize = row["MeterDisplayName"].ToString(),
                            Pay = row["Pay"].ToString()
                        });
                    }
                }
                sqlConnection1.Close();
                /*CompassEntities CompassEntities = new CompassEntities();
                List<MeterSizePayRateBo> objMeterSizePayRateBo = CompassEntities.tblMeterPays.AsEnumerable().
                    Where(o => o.Active == 1 && o.FK_Project_Id == projectId)
                    .Select(o =>
                        new MeterSizePayRateBo
                        {
                            Id = o.ID,
                            MeterSize =o.MeterDisplayName,// CompassEntities.tblMeterSizes.Where(o1 => o1.ID == o.FK_MeterSize_Id).Select(o1 => o1.MeterSize).FirstOrDefault(),
                            Pay = Convert.ToString(o.Pay)
                        }).OrderBy(t => t.Id).ToList();*/
                return objMeterSizePayRateBo;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<ProductServiceBo> GetProductServiceList(long projectId)
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                var project = CompassEntities.tblProjects.Where(o => o.ProjectId == projectId).FirstOrDefault();
                var utility = CompassEntities.UtilityMasters.Where(o => o.UtilityName == project.Utilitytype).FirstOrDefault();
                List<ProductServiceBo> objServiceSmallModelBo = CompassEntities.VW_GetProductData.Where(o => o.UtilityId == utility.Id).Select(o =>
                                                                        new ProductServiceBo
                                                                        {
                                                                            productId = o.Id,
                                                                            productName = o.ProductName,
                                                                            Make = o.Make,
                                                                            Type = o.MeterType,
                                                                            Size = o.MeterSize,
                                                                            UtilityName = o.UtilityName,
                                                                            SerialNumberRequired = o.SerialNumberRequired
                                                                        }).OrderBy(t => t.productName).ToList();
                foreach (var item in objServiceSmallModelBo)
                {
                    item.productName = item.productName + ", " + Convert.ToString(item.Make) + "," + Convert.ToString(item.Type);
                    //var a = CompassEntities.TblProjectMasterFieldRelations.Where(o => o.ID == item.ServiceId).FirstOrDefault();
                    //var picList = CompassEntities.VW_GetPictures.Where(o => o.tblService_ServiceId == a.MasterFieldID).OrderBy(o => o.Picture).Select(o => o.Picture).ToList();
                    ////item.isImageMapped = picList.Count == 0 ? false : true;
                    //if (picList.Count == 0)
                    //{
                    //    item.isImageMapped = false;
                    //}
                    //else
                    //{
                    //    item.isImageMapped = true;
                    //}
                }


                return objServiceSmallModelBo;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<tblUploadedData> GetActiveProjectData(long projectId)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            var tblData = objCompassEntities.tblUploadedDatas.Where(o => o.ProjectId == projectId && o.Active == 1).ToList();

            return tblData;
        }

        public bool SaveTab3Records(long projectId, string[] idList, string[] IdPrice, string[] IdImageFlag, int UserId)
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();

                int i = 0;
                foreach (var item in idList)
                {
                    long a = long.Parse(item.ToString());
                    decimal b = decimal.Parse(IdPrice[i].ToString());
                    bool c = bool.Parse(IdImageFlag[i].ToString());
                    var obj = CompassEntities.TblProjectMasterFieldRelations.Where(o => o.ID == a).FirstOrDefault();
                    obj.BasePrice = b;
                    obj.ImageFlag = c;
                    obj.ModifiedBy = UserId;
                    obj.ModifiedOn = new CommonFunctions().ServerDate();
                    CompassEntities.SaveChanges();
                    i++;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool SaveTab3Records(Tab3BO objTab3BO, int currentUserId)
        {
            try
            {
                TransactionOptions options = new TransactionOptions();
                options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                options.Timeout = new TimeSpan(0, 15, 0);


                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, options))
                {
                    CompassEntities CompassEntities = new CompassEntities();

                    foreach (var item in objTab3BO.Servicedata)
                    {


                        var obj = CompassEntities.TblProjectMasterFieldRelations.Where(o => o.ID == item.serviceId).FirstOrDefault();
                        obj.BasePrice = item.servicePrice;
                        obj.ImageFlag = item.imageCaptureRequired == "true" ? true : false;
                        obj.ModifiedBy = currentUserId;
                        obj.ModifiedOn = new CommonFunctions().ServerDate();
                        CompassEntities.SaveChanges();



                        var objList = CompassEntities.ProjectServiceProductRelations.Where(o => o.Active == 1 && o.FKProjectID == objTab3BO.projectId
                            && o.FKServiceId == obj.MasterFieldID).ToList();

                        foreach (var item1 in objList)
                        {
                            item1.ModifiedOn = DateTime.Now;
                            item1.ModifiedBy = currentUserId;
                            item1.Active = 1;

                            CompassEntities.SaveChanges();
                        }



                        if (item.AssignedProductList != null && item.AssignedProductList.Count() > 0)
                        {
                            foreach (var itemProductList in item.AssignedProductList)
                            {

                                var obj1 = CompassEntities.ProjectServiceProductRelations.Where(o => o.FKProjectID == objTab3BO.projectId
                                    && o.FKProductId == itemProductList && o.FKServiceId == obj.MasterFieldID).FirstOrDefault();

                                if (obj1 == null)
                                {
                                    obj1 = new ProjectServiceProductRelation();
                                    obj1.CreatedOn = DateTime.Now;
                                    obj1.CreatedBy = currentUserId;
                                    obj1.FKProjectID = objTab3BO.projectId;
                                    obj1.FKProductId = itemProductList;
                                    obj1.FKServiceId = obj.MasterFieldID;
                                    obj1.Quantity = 1;
                                    obj1.Active = 1;
                                    CompassEntities.ProjectServiceProductRelations.Add(obj1);
                                    CompassEntities.SaveChanges();
                                }
                                else
                                {
                                    obj1.Active = 1;
                                    CompassEntities.SaveChanges();
                                }


                            }
                        }

                    }
                    scope.Complete();

                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool SaveTab3MeterPayRateRecords(Tab3BOMeterPayRate objTab3BOPayRate, int currentUserId)
        {
            try
            {
                TransactionOptions options = new TransactionOptions();
                options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                options.Timeout = new TimeSpan(0, 15, 0);


                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, options))
                {
                    CompassEntities CompassEntities = new CompassEntities();

                    foreach (var item in objTab3BOPayRate.newMeterPayRate)
                    {
                        var obj = CompassEntities.tblMeterPays.Where(o => o.ID == item.Id).FirstOrDefault();
                        obj.Pay = item.Pay;
                        obj.ModifiedBy = currentUserId;
                        obj.ModifiedOn = new CommonFunctions().ServerDate();
                        CompassEntities.SaveChanges();
                    }
                    scope.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CreateOrUpdateMasterField(int type, long projectId, int userId, string[] objMeterSizeList, out string returnMessage)
        {
            bool result = false;
            returnMessage = "Unable to Create / Update State.";

            CompassEntities CompassEntities = new CompassEntities();
            try
            {
                var oldList = CompassEntities.TblProjectMasterFieldRelations.AsEnumerable().Where(o => o.ProjectID == projectId &&
                        o.MasterTableName == (type == 1 ? "tblMeterSize" : (type == 2 ? "tblMeterType" : (type == 3 ? "tblService" : "tblMeterMake")))).ToList();
                if (oldList != null && oldList.Count() > 0)
                {
                    foreach (var item in oldList)
                    {
                        item.ModifiedBy = userId;
                        item.Active = 0;
                        item.ModifiedOn = new CommonFunctions().ServerDate();
                        CompassEntities.SaveChanges();
                    }
                }

                foreach (var item in objMeterSizeList)
                {
                    var obj = CompassEntities.TblProjectMasterFieldRelations.AsEnumerable().Where(o => o.ProjectID == projectId && o.MasterFieldID == long.Parse(item) &&
                        o.MasterTableName == (type == 1 ? "tblMeterSize" : (type == 2 ? "tblMeterType" : (type == 3 ? "tblService" : "tblMeterMake")))).FirstOrDefault();

                    if (obj == null)
                    {
                        obj = new TblProjectMasterFieldRelation();
                        obj.MasterFieldID = long.Parse(item);
                        obj.MasterTableName = (type == 1 ? "tblMeterSize" : (type == 2 ? "tblMeterType" : (type == 3 ? "tblService" : "tblMeterMake")));
                        if (type == 3)
                        {
                            long i = long.Parse(item.ToString());
                            obj.BasePrice = CompassEntities.tblServices.Where(o => o.ID == i).Select(o => o.ServiceBasePrice).FirstOrDefault();
                            obj.IsAdditionalService = false;

                            var imageCount = CompassEntities.VW_GetPictures.Where(o => o.tblService_ServiceId == obj.MasterFieldID).Count();
                            obj.ImageFlag = false;//imageCount > 0 ? true : false;
                        }
                        obj.ProjectID = projectId;
                        obj.CreatedBy = userId;
                        obj.Active = 1;
                        obj.CreatedOn = new CommonFunctions().ServerDate();
                        CompassEntities.TblProjectMasterFieldRelations.Add(obj);
                        CompassEntities.SaveChanges();
                    }
                    else
                    {
                        obj.IsAdditionalService = false;
                        obj.ModifiedBy = userId;
                        obj.Active = 1;
                        obj.ModifiedOn = new CommonFunctions().ServerDate();
                        CompassEntities.SaveChanges();
                    }
                }

                if (type == 3)
                {
                    var a = CompassEntities.TblProjectMasterFieldRelations.AsEnumerable().Where(o => o.Active == 1 && o.IsAdditionalService == false
                        && o.ProjectID == projectId && o.MasterTableName == "tblService").Select(o => o.MasterFieldID).ToList();

                    var b = CompassEntities.tblServices.Where(o => o.Active == 1).ToList();

                    if (b != null)
                    {
                        foreach (var item in a)
                        {
                            var c = b.Where(o => o.ID == item).FirstOrDefault();
                            if (c != null)
                            {
                                b.Remove(c);
                            }
                        }
                    }

                    foreach (var item in b)
                    {
                        var obj = CompassEntities.TblProjectMasterFieldRelations.AsEnumerable().Where(o => o.ProjectID == projectId && o.MasterFieldID == item.ID &&
                       o.MasterTableName == "tblService").FirstOrDefault();

                        if (obj == null)
                        {
                            obj = new TblProjectMasterFieldRelation();
                            obj.MasterFieldID = item.ID;
                            obj.MasterTableName = "tblService";

                            long i = long.Parse(item.ID.ToString());
                            obj.BasePrice = item.ServiceBasePrice;
                            obj.IsAdditionalService = true;
                            var imageCount = CompassEntities.VW_GetPictures.Where(o => o.tblService_ServiceId == obj.MasterFieldID).Count();
                            obj.ImageFlag = false;//imageCount > 0 ? true : false;
                            obj.ProjectID = projectId;
                            obj.CreatedBy = userId;
                            obj.Active = 1;
                            obj.CreatedOn = new CommonFunctions().ServerDate();
                            CompassEntities.TblProjectMasterFieldRelations.Add(obj);
                            CompassEntities.SaveChanges();
                        }
                        else
                        {
                            obj.IsAdditionalService = true;
                            obj.ModifiedBy = userId;
                            obj.Active = 1;
                            obj.ModifiedOn = new CommonFunctions().ServerDate();
                            CompassEntities.SaveChanges();
                        }
                    }

                }

                returnMessage = CommonFunctions.strRecordCreated;
                result = true;
            }
            catch (Exception ex)
            {
                returnMessage += "<br/>" + ex.Message;
                result = false;
            }
            return result;
        }

        public bool CreateOrUpdateMeterSizePayRete(int type, long projectId, int userId, string[] objMeterSizeList, out string returnMessage)
        {
            bool result = false;
            returnMessage = "Unable to Create / Update State.";

            CompassEntities CompassEntities = new CompassEntities();
            try
            {
                // Add new code insert or update meter size pay rate from table ==> tblMeterPAy
                if (type == 5)
                {
                    var objOldList = CompassEntities.tblMeterPays.AsEnumerable().Where(o => o.FK_Project_Id == projectId).ToList();
                    if (objOldList != null && objOldList.Count() > 0)
                    {
                        foreach (var item in objOldList)
                        {
                            item.ModifiedBy = userId;
                            item.Active = 0;
                            item.ModifiedOn = new CommonFunctions().ServerDate();
                            CompassEntities.SaveChanges();
                        }
                    }
                    foreach (var item in objMeterSizeList)
                    {
                        long mid = long.Parse(item);
                        //var obj = CompassEntities.tblMeterPays.AsEnumerable().Where(o => o.FK_Project_Id == projectId && o.FK_MeterSize_Id == long.Parse(item)).FirstOrDefault();
                        var newobj = (from pm in CompassEntities.ProductMasters
                                      join vw in CompassEntities.VW_GetProductData on pm.PartNumber equals vw.PartNumber
                                      join ps in CompassEntities.ProjectServiceProductRelations on pm.Id equals ps.FKProductId
                                      where pm.MeterSizeId == mid && ps.FKProjectID == projectId
                                      && pm.Active == 1 && ps.Active == 1
                                      select new Meterpayratedetail
                                      {
                                          MeterSizeId = (long)pm.MeterSizeId,
                                          MeterSize = vw.MeterSize,
                                          MeterTypeId = (long)pm.MeterTypeId,
                                          MeterType = vw.MeterType
                                      }).Distinct().ToList();

                        if (newobj != null)
                        {
                            foreach (var _iten in newobj)
                            {
                                var exist = CompassEntities.tblMeterPays.AsEnumerable().Where(o => o.FK_Project_Id == projectId && o.FK_MeterSize_Id == _iten.MeterSizeId && o.FK_MeterType_Id==_iten.MeterTypeId).FirstOrDefault();
                                if (exist == null)
                                {
                                    exist = new tblMeterPay();
                                    exist.FK_Project_Id = projectId;
                                    exist.FK_MeterSize_Id = _iten.MeterSizeId;
                                    exist.FK_MeterType_Id = _iten.MeterTypeId;
                                    exist.MeterSize = _iten.MeterSize;
                                    exist.MeterType = _iten.MeterType;
                                    exist.MeterDisplayName = _iten.MeterSize + " " + _iten.MeterType + " Meters";
                                    exist.Description = "";   // don't required description
                                    exist.Pay = 0;            // defualt set 0 and then set meter pay rete next page
                                    exist.Active = 1;
                                    exist.CreatedBy = userId;
                                    exist.CreatedOn = new CommonFunctions().ServerDate();
                                    CompassEntities.tblMeterPays.Add(exist);
                                    CompassEntities.SaveChanges();
                                }
                                else
                                {
                                    exist.ModifiedBy = userId;
                                    exist.Active = 1;
                                    exist.ModifiedOn = new CommonFunctions().ServerDate();
                                    CompassEntities.SaveChanges();
                                }
                            }
                            
                        }
                       /* if (obj == null)
                        {
                            obj = new tblMeterPay();
                            obj.FK_Project_Id = projectId;
                            obj.FK_MeterSize_Id = long.Parse(item);
                            obj.Description = "";   // don't required description
                            obj.Pay = 0;            // defualt set 0 and then set meter pay rete next page
                            obj.Active = 1;
                            obj.CreatedBy = userId;
                            obj.CreatedOn = new CommonFunctions().ServerDate();
                            CompassEntities.tblMeterPays.Add(obj);
                            CompassEntities.SaveChanges();
                        }
                        else
                        {
                            obj.ModifiedBy = userId;
                            obj.Active = 1;
                            obj.ModifiedOn = new CommonFunctions().ServerDate();
                            CompassEntities.SaveChanges();
                        }*/
                    }
                }
                //
                returnMessage = CommonFunctions.strRecordCreated;
                result = true;
            }
            catch (Exception ex)
            {
                returnMessage += "<br/>" + ex.Message;
                result = false;
            }
            return result;
        }

        public List<tblStandardFieldMaster> GetActiveStandardFields()
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<tblStandardFieldMaster> objtblStandardFieldMaster = (from b in CompassEntities.tblStandardFieldMasters
                                                                          where b.Active == 1
                                                                          select b
                                                    ).OrderBy(t => t.StandardFieldName).ToList();

                return objtblStandardFieldMaster;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<tblStandardFieldMaster> GetSelectedStandardFields(long ProjectId)
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();


                var a = (from b in CompassEntities.TblProjectStandardFieldRelations
                         where b.Active == 1 && b.ProjectID == ProjectId
                         select b
                                                    ).ToList();

                List<tblStandardFieldMaster> objtblStandardFieldMaster = new List<tblStandardFieldMaster>();
                if (a != null && a.Count > 0)
                {
                    foreach (var item in a)
                    {
                        var b = CompassEntities.tblStandardFieldMasters.Where(o => o.Active == 1 &&
                            o.StandardFieldId == item.FieldID).Select(o => o).FirstOrDefault();
                        objtblStandardFieldMaster.Add(b);
                    }
                }
                return objtblStandardFieldMaster;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CreateOrUpdateStandardFields(long projectId, int userId, string[] objStandardFieldList, out string returnMessage)
        {
            bool result = false;
            returnMessage = "Unable to Create / Update State.";

            CompassEntities CompassEntities = new CompassEntities();
            try
            {

                var a = CompassEntities.TblProjectStandardFieldRelations.Where(o => o.ProjectID == projectId).Select(o => o).ToList();



                var checkFieldPresent = CompassEntities.TblProjectExcelMappings.Where(o => o.ProjectID == projectId && o.Active == 1 && o.FieldType == 2).ToList();
                List<string> excelMapFields = new List<string>();

                var checkStandarFields = CompassEntities.VW_GETTab6DropdownData.Where(o => o.ProjectID == projectId && o.FieldType == 2).ToList();

                foreach (var item in checkFieldPresent)
                {
                    var ab = checkStandarFields.Where(o => o.tabelId == item.FieldID).Select(o => o.FieldName).FirstOrDefault();

                    if (ab != null)
                    {
                        excelMapFields.Add(ab);
                    }
                }

                bool ExcelMappingFieldPresent = true;
                string field = "";
                foreach (var item in excelMapFields)
                {
                    var abc = objStandardFieldList.Where(o => o == item).FirstOrDefault();
                    if (abc == null)
                    {
                        field = item;
                        ExcelMappingFieldPresent = false;
                        break;
                    }
                }



                if (!ExcelMappingFieldPresent)
                {
                    returnMessage = "Save failed. Can not remove the field " + field;
                    result = false;
                }
                else
                {


                    foreach (var item in a)
                    {
                        item.Active = 0;
                        item.ModifiedOn = new CommonFunctions().ServerDate();
                        item.ModifiedBy = userId;
                        CompassEntities.SaveChanges();
                    }


                    foreach (var item in objStandardFieldList)
                    {
                        long stanardFieldId = CompassEntities.tblStandardFieldMasters.Where(o => o.StandardFieldName == item).Select(o => o.StandardFieldId).FirstOrDefault();
                        TblProjectStandardFieldRelation obj = CompassEntities.TblProjectStandardFieldRelations.Where(o => o.ProjectID == projectId
                            && o.FieldID == stanardFieldId).Select(o => o).FirstOrDefault();


                        if (obj == null)
                        {
                            obj = new TblProjectStandardFieldRelation();
                            obj.Active = 1;
                            obj.CreatedBy = userId;
                            obj.CreatedOn = new CommonFunctions().ServerDate();
                            obj.FieldID = stanardFieldId;
                            obj.ProjectID = projectId;
                            CompassEntities.TblProjectStandardFieldRelations.Add(obj);
                            CompassEntities.SaveChanges();
                        }
                        else
                        {
                            obj.Active = 1;
                            obj.ModifiedOn = new CommonFunctions().ServerDate();
                            obj.ModifiedBy = userId;
                            CompassEntities.SaveChanges();
                        }

                    }


                    returnMessage = CommonFunctions.strRecordCreated;
                    result = true;
                }

            }
            catch (Exception ex)
            {

                returnMessage += "<br/>" + ex.Message;
                result = false;
            }
            return result;
        }




        public bool C_U_D_CustomFields(TblProjectCustomeFieldReletion obj1, int status, out string returnMessage)
        {
            bool result = false;
            returnMessage = status == 1 ? "Unable to create custom field." : status == 2 ? "Unable to update custom field." : "Unable to dactivate custom field.";

            CompassEntities CompassEntities = new CompassEntities();
            try
            {
                if (status == 1)
                {
                    var a = GetViewData(obj1.ProjectID);
                    var res = a.Where(o => o.FieldName.ToLower().Trim() == obj1.FieldName.ToLower().Trim() && o.FieldType != 3).FirstOrDefault();
                    if (res != null)
                    {
                        returnMessage = "Save failed. The field is already present in Standard or Master field.";
                        return result;
                    }
                    if (obj1.FieldName.ToLower().Trim() == "services")
                    {
                        returnMessage = "Save failed. The field is already present in Standard or Master field.";
                        return result;
                    }

                    TblProjectCustomeFieldReletion obj = CompassEntities.TblProjectCustomeFieldReletions.
                        Where(o => o.FieldName.ToLower().Trim() == obj1.FieldName.ToLower().Trim() && o.ProjectID == obj1.ProjectID).FirstOrDefault();
                    if (obj == null)
                    {
                        obj = new TblProjectCustomeFieldReletion();

                        obj.Active = 1;
                        obj.CreatedBy = obj1.CreatedBy;
                        obj.CreatedOn = new CommonFunctions().ServerDate();
                        obj.ProjectID = obj1.ProjectID;
                        obj.FieldName = obj1.FieldName;
                        CompassEntities.TblProjectCustomeFieldReletions.Add(obj);
                        CompassEntities.SaveChanges();
                        returnMessage = CommonFunctions.strRecordCreated;
                        result = true;
                    }
                    else
                    {
                        returnMessage = obj.Active == 0 ? "Save failed. The field is already present in deactivated custom field." :
                            "Save failed. The field is already present in activat custom field.";
                    }
                }
                else if (status == 2)
                {
                    TblProjectCustomeFieldReletion objDuplicate = CompassEntities.TblProjectCustomeFieldReletions.
                      Where(o => o.FieldName.ToLower().Trim() == obj1.FieldName.ToLower().Trim() && o.ProjectID == obj1.ProjectID).FirstOrDefault();

                    if (objDuplicate != null && objDuplicate.ID != obj1.ID)
                    {
                        returnMessage = objDuplicate.Active == 0 ? "Save failed. The field is already present in deactivated custom field." :
                           "Save failed. The field is already present in activat custom field.";
                    }
                    else
                    {
                        TblProjectCustomeFieldReletion obj = CompassEntities.TblProjectCustomeFieldReletions.Where(o => o.ID == obj1.ID).FirstOrDefault();
                        obj.Active = 1;
                        obj.ModifiedBy = obj1.ModifiedBy;
                        obj.ModifiedOn = new CommonFunctions().ServerDate();
                        obj.FieldName = obj1.FieldName;
                        CompassEntities.SaveChanges();
                        returnMessage = CommonFunctions.strRecordUpdated;
                        result = true;
                    }

                }



            }
            catch (Exception ex)
            {

                returnMessage += "<br/>" + ex.Message;
                result = false;
            }
            return result;
        }



        public List<CustomFieldModel> GetCustomFieldRecordList(bool status, long projectId)
        {
            CompassEntities CompassEntities = new CompassEntities();
            var objSubMenuesBo = CompassEntities.TblProjectCustomeFieldReletions.AsEnumerable().Where(o => o.ProjectID == projectId && o.Active == (status == true ? 1 : 0)).
                                  Select(o => new CustomFieldModel
                                  {
                                      ID = o.ID,
                                      FieldName = o.FieldName
                                  }).OrderBy(t => t.FieldName).ToList();

            return objSubMenuesBo;
        }



        public CustomFieldModel GetCustomFieldRecord(long customId)
        {
            CompassEntities CompassEntities = new CompassEntities();
            var objSubMenuesBo = CompassEntities.TblProjectCustomeFieldReletions.AsEnumerable().Where(o => o.ID == customId && o.Active == 1).
                                  Select(o => new CustomFieldModel
                                  {
                                      ID = o.ID,
                                      FieldName = o.FieldName
                                  }).FirstOrDefault();

            return objSubMenuesBo;
        }

        public ProjectModel GetProjectDetail(long projectId)
        {
            CompassEntities CompassEntities = new CompassEntities();
            var objSubMenuesBo = CompassEntities.tblProjects.AsNoTracking().AsEnumerable().Where(o => o.ProjectId == projectId && o.Active == 1).
                                  Select(o => new ProjectModel
                                  {
                                      ProjectId = o.ProjectId,
                                      ProjectName = o.ProjectName,
                                      ProjectDescription = o.ProjectDecsription,
                                      ClientId = long.Parse(o.ClientId.ToString()),
                                      /*StateId = long.Parse(o.StateId.ToString()),
                                      CityId = long.Parse(o.CityId.ToString()),*/
                                      StateId = o.StateId.ToString() == null ? 0 : (long.Parse(o.StateId.ToString())),
                                      CityId = o.CityId.ToString() == null ? 0 : (long.Parse(o.CityId.ToString())),
                                      ContactId = o.ContactId,
                                      Utilitytype = o.Utilitytype,
                                      stringFromDate = DateTime.Parse(o.FromDate.ToString()).ToString("MMM-dd-yyyy"),
                                      stringToDate = DateTime.Parse(o.ToDate.ToString()).ToString("MMM-dd-yyyy"),
                                      ConnectionType = o.DataSyncConnection == null ? 0 : int.Parse(o.DataSyncConnection.ToString()),
                                      dataSynch = o.AutoDataSyncInterval == null ? 0 : int.Parse(o.AutoDataSyncInterval.ToString()),
                                      SyncType = o.PartialSynch == null ? 2 : (int.Parse(o.PartialSynch.ToString()) == 1 ? 1 : 2),
                                      ProjectAppointmentUrl = Convert.ToString(o.AppointmentUrl),
                                      ManageInventory = o.ManageInventory,
                                      valueMapBy = Convert.ToInt32(o.mapBy)
                                  }).FirstOrDefault();


            objSubMenuesBo.objStateList = CompassEntities.tblStateMasters.AsNoTracking().Select(o => new StateModel
            {
                StateID = o.StateId,
                StateName = o.StateName
            }).OrderBy(t => t.StateName).ToList();


            objSubMenuesBo.objCityList = CompassEntities.TblCityMasters.AsNoTracking().Where(o => o.StateId == objSubMenuesBo.StateId && o.Active == 1).Select(x => new CityModel { CityID = x.CityId, CityName = x.CityName }).ToList();

            objSubMenuesBo.objClientModelList = CompassEntities.tblClients.AsNoTracking().Where(o => o.Active == 1).
                                                Select(o => new ClientModel
                                                {
                                                    ClientId = o.ClientId,
                                                    ClientName = o.ClientName
                                                }).OrderBy(t => t.ClientName).ToList();

            objSubMenuesBo.objContactModelDDLList = CompassEntities.tblClientContactDetails.AsNoTracking().
                                                    Where(o => o.Active == 1 && o.tblClient_ClientId == objSubMenuesBo.ClientId)
                                                    .Select(o => new ContactModelDDL
                                                    {
                                                        ContactId = o.ID,
                                                        ContactName = o.FirstName + " " + o.LastName
                                                    }).OrderBy(t => t.ContactName).ToList();


            objSubMenuesBo.objMobileWarehouseBOList = CompassEntities.WarehouseMasters.AsNoTracking().Where(o => o.Active == 1).
                                                Select(o => new MobileWarehouseBO
                                                {
                                                    Id = o.Id,
                                                    WarehouseName = o.WarehouseName
                                                }).OrderBy(t => t.WarehouseName).ToList();

            objSubMenuesBo.objWarehouseArray = CompassEntities.ProjectWarehouseRelations.AsNoTracking().
                                           Where(o => o.Active == 1 && o.FKProjectId == projectId).Select(o => o.FKWarehouseId).ToArray();



            objSubMenuesBo.objMeterMakeModelList = (from b in CompassEntities.tblMeterMakes.AsNoTracking()
                                                    where b.Active == 1
                                                    select new MeterMakeModel { ID = b.ID, Make = b.Make }).OrderBy(t => t.Make).ToList(); ;
            objSubMenuesBo.objMeterSizeList = (from b in CompassEntities.tblMeterSizes.AsNoTracking()
                                               where b.Active == 1
                                               select new MeterSizeModel { ID = b.ID, MeterSize = b.MeterSize }).OrderBy(t => t.MeterSize).ToList(); ;
            objSubMenuesBo.objMeterTypeList = (from b in CompassEntities.tblMeterTypes.AsNoTracking()
                                               where b.Active == 1
                                               select new MeterTypeModel { ID = b.ID, MeterType = b.MeterType }).OrderBy(t => t.MeterType).ToList();
            objSubMenuesBo.objServiceList = (from b in CompassEntities.tblServices.AsNoTracking()
                                             where b.Active == 1
                                             select new ServicesModel { ID = b.ID, Service = b.Service }).OrderBy(t => t.Service).ToList();




            objSubMenuesBo.objUsersSmallModelList = (from o in CompassEntities.tblUsers.AsNoTracking()
                                                     where o.Active == 1
                                                     select new UsersSmallModel { UserId = o.UserID, UserName = o.FirstName }).ToList();



            objSubMenuesBo.objTypeArray = (from b in CompassEntities.TblProjectMasterFieldRelations.AsNoTracking()
                                           where b.Active == 1 && b.ProjectID == projectId && b.MasterTableName == "tblMeterType"
                                           select b.MasterFieldID).ToArray();
            objSubMenuesBo.objSizeArray = (from b in CompassEntities.TblProjectMasterFieldRelations.AsNoTracking()
                                           where b.Active == 1 && b.ProjectID == projectId && b.MasterTableName == "tblMeterSize"
                                           select b.MasterFieldID).ToArray();

            objSubMenuesBo.objMakeArray = (from b in CompassEntities.TblProjectMasterFieldRelations.AsNoTracking()
                                           where b.Active == 1 && b.ProjectID == projectId && b.MasterTableName == "tblMeterMake"
                                           select b.MasterFieldID).ToArray();
            objSubMenuesBo.objServiceArray = (from b in CompassEntities.TblProjectMasterFieldRelations.AsNoTracking()
                                              where b.Active == 1 && b.ProjectID == projectId && b.MasterTableName == "tblService"
                                              && b.IsAdditionalService == false
                                              select b.MasterFieldID).ToArray();


            objSubMenuesBo.objUsersArray = (from b in CompassEntities.TblProjectInstallers.AsNoTracking()
                                            where b.Active == 1 && b.ProjectID == projectId
                                            select b.tblUsers_UserID).ToArray();



            return objSubMenuesBo;
        }


        public bool DeactivateCustomFieldRecord(long customId, int userId, out string returnMessage)
        {
            CompassEntities CompassEntities = new CompassEntities();

            var checkFieldPresent = CompassEntities.TblProjectExcelMappings.Where(o => o.FieldID == customId && o.Active == 1).FirstOrDefault();

            if (checkFieldPresent == null)
            {

                TblProjectCustomeFieldReletion obj = CompassEntities.TblProjectCustomeFieldReletions.Where(o => o.ID == customId).FirstOrDefault();
                obj.Active = 0;
                obj.ModifiedBy = userId;
                obj.ModifiedOn = new CommonFunctions().ServerDate();
                CompassEntities.SaveChanges();
                returnMessage = CommonFunctions.strRecordDeactivated;
                return true;
            }
            else
            {
                returnMessage = "Deactivate failed as Field is already used in excel mapping.";
                return false;
            }
        }

        public bool ActivateCustomFieldRecord(long customId, int userId, out string returnMessage)
        {
            CompassEntities CompassEntities = new CompassEntities();
            TblProjectCustomeFieldReletion obj = CompassEntities.TblProjectCustomeFieldReletions.Where(o => o.ID == customId).FirstOrDefault();
            obj.Active = 1;
            obj.ModifiedBy = userId;
            obj.ModifiedOn = new CommonFunctions().ServerDate();
            CompassEntities.SaveChanges();
            returnMessage = CommonFunctions.strRecordActivated;
            return true;
        }


        public bool A_Or_D_Project(long projectId, int userId, bool status, out string returnMessage)
        {
            CompassEntities CompassEntities = new CompassEntities();
            tblProject obj = CompassEntities.tblProjects.Where(o => o.ProjectId == projectId).FirstOrDefault();

            obj.Active = status == true ? 1 : 0;
            obj.ModifiedBy = userId;
            obj.ModifiedOn = new CommonFunctions().ServerDate();
            CompassEntities.SaveChanges();

            returnMessage = status == true ? CommonFunctions.strRecordActivated : CommonFunctions.strRecordDeactivated;
            return true;
        }

        //public void setGroupIdVisitId(long projectId)
        //{
        //    CompassEntities objCompassEntities = new CompassEntities();
        //    objCompassEntities.STP_SetGroupIDVisitId(projectId);
        //}



        /// <summary>
        /// 
        /// </summary>
        /// <param name="objTblProjectExcelMapping"></param>
        /// <param name="ProjectID"></param>
        /// <param name="objExcelSheetBOList"></param>
        /// <param name="userId"></param>
        /// <param name="dt"></param>
        /// <param name="returnMessage"></param>
        /// <param name="dt1"></param>
        /// <param name="IsLatLongCalculated"></param>
        /// <returns></returns>
        public bool CreateOrUpdateExcelMappingData(TblProjectExcelMapping objTblProjectExcelMapping, long ProjectID,
            List<ExcelSheetBO> objExcelSheetBOList, int userId, DataTable dt, out string returnMessage, out DataTable dt1, out bool IsLatLongCalculated,
            out int newRecordsCount, out int skipedRecordsCount, out int updatedRecordsCount, out string skipedAccountsNo)
        {
            IsLatLongCalculated = false;
            bool result = false;
            returnMessage = "Unable to Save Records.";
            dt1 = null;
            newRecordsCount = 0;
            skipedRecordsCount = 0;
            updatedRecordsCount = 0;
            skipedAccountsNo = "Skipped Account No.";

            CompassEntities objCompassEntities = new CompassEntities();
            try
            {

                var projectObject = objCompassEntities.tblProjects.Where(x => x.ProjectId == ProjectID).FirstOrDefault();
                projectObject.IsLatLongCalculated = 0;
                objCompassEntities.SaveChanges();
                string state = objCompassEntities.tblStateMasters.Where(x => x.StateId == projectObject.StateId).Select(x => x.StateName).FirstOrDefault();
                string city = objCompassEntities.TblCityMasters.Where(x => x.CityId == projectObject.CityId).Select(x => x.CityName).FirstOrDefault();

                List<TblProjectExcelMapping> objCurrentExcelColumns = new List<TblProjectExcelMapping>();



                #region excel mapping data
                if (objExcelSheetBOList != null && objExcelSheetBOList.Count > 1)
                {
                    int dynamicColNo = 1;
                    foreach (var item in objExcelSheetBOList.Where(o => o.FieldID != 0).ToList())
                    {

                        var alreadyMapped = objCompassEntities.TblProjectExcelMappings.Where(x => x.ProjectID == ProjectID &&
                            x.FieldType == item.FieldType && x.FieldID == item.FieldID).FirstOrDefault();

                        if (alreadyMapped != null)
                        {
                            alreadyMapped.ExcelFieldName = item.FieldName;
                            alreadyMapped.ModifiedBy = objTblProjectExcelMapping.CreatedBy;
                            alreadyMapped.Active = 1;
                            alreadyMapped.ModifiedOn = new CommonFunctions().ServerDate();
                            objCompassEntities.SaveChanges();
                            objCurrentExcelColumns.Add(alreadyMapped);
                        }
                        else
                        {

                            string dynamicColumn = "RefCol" + dynamicColNo.ToString();


                            var alreadyUsed = objCompassEntities.TblProjectExcelMappings.Where(x => x.ProjectID == ProjectID &&
                            x.FKUploadedExcelDataDynamicColumn == dynamicColumn).FirstOrDefault();



                            while (alreadyUsed != null)
                            {
                                dynamicColNo++;
                                dynamicColumn = "RefCol" + dynamicColNo.ToString();
                                alreadyUsed = objCompassEntities.TblProjectExcelMappings.Where(x => x.ProjectID == ProjectID &&
                                        x.FKUploadedExcelDataDynamicColumn == dynamicColumn).FirstOrDefault();
                            }




                            TblProjectExcelMapping objTblProjectExcelMappingData = new TblProjectExcelMapping();
                            objTblProjectExcelMappingData.FieldType = item.FieldType;
                            objTblProjectExcelMappingData.FieldID = item.FieldID;
                            objTblProjectExcelMappingData.ExcelFieldName = item.FieldName;
                            objTblProjectExcelMappingData.ProjectID = ProjectID;
                            objTblProjectExcelMappingData.FKUploadedExcelDataDynamicColumn = dynamicColumn;
                            objTblProjectExcelMappingData.CreatedBy = objTblProjectExcelMapping.CreatedBy;
                            objTblProjectExcelMappingData.Active = 1;
                            objTblProjectExcelMappingData.CreatedOn = new CommonFunctions().ServerDate();
                            objCompassEntities.TblProjectExcelMappings.Add(objTblProjectExcelMappingData);
                            objCompassEntities.SaveChanges();
                            objCurrentExcelColumns.Add(objTblProjectExcelMappingData);

                        }


                        returnMessage = CommonFunctions.strRecordCreated;
                        result = true;
                        dynamicColNo++;
                    }
                }
                #endregion

                if (IsLatLongCalculated)
                {
                    objCompassEntities = new CompassEntities();
                    var projectObject1 = objCompassEntities.tblProjects.Where(x => x.ProjectId == ProjectID).FirstOrDefault();
                    projectObject1.IsLatLongCalculated = 1;
                    objCompassEntities.SaveChanges();
                }

                // new dynamic column mapping
                if (dt != null && dt.Rows.Count > 0)
                {
                    #region
                    /// itrated through all data of excel sheet to save information in database.
                    CompassEntities objCompassEntities1 = new CompassEntities();
                    var GetExcelColumnMapList = objCurrentExcelColumns;//objCompassEntities1.TblProjectExcelMappings.Where(o => o.ProjectID == ProjectID && o.Active == 1).ToList();

                    var a2 = objCompassEntities.tblStandardFieldMasters.Where(o => o.Active == 1).ToList();
                    var a1 = objCompassEntities.TblProjectStandardFieldRelations.Where(o => o.ProjectID == ProjectID && o.Active == 1).ToList();


                    string sAddressLatitude = objExcelSheetBOList.Where(o => o.FieldID > 0 && o.FieldID == (a1.Where(o1 =>
                        o1.FieldID == (a2.Where(o2 => o2.StandardFieldName.ToLower() == "ADDRESS LATITUDE".ToLower())
                        .Select(o2 => o2.StandardFieldId).FirstOrDefault()))).Select(o1 => o1.ID).FirstOrDefault()).Select(o => o.FieldName).FirstOrDefault();

                    string sAddressLongitude = objExcelSheetBOList.Where(o => o.FieldID > 0 && o.FieldID == (a1.Where(o1 =>
                        o1.FieldID == (a2.Where(o2 => o2.StandardFieldName.ToLower() == "ADDRESS LONGITUDE".ToLower())
                        .Select(o2 => o2.StandardFieldId).FirstOrDefault()))).Select(o1 => o1.ID).FirstOrDefault()).Select(o => o.FieldName).FirstOrDefault();

                    string sStreet = objExcelSheetBOList.Where(o => o.FieldID > 0 && o.FieldID == (a1.Where(o1 =>
                       o1.FieldID == (a2.Where(o2 => o2.StandardFieldName.ToLower() == "STREET".ToLower())
                       .Select(o2 => o2.StandardFieldId).FirstOrDefault()))).Select(o1 => o1.ID).FirstOrDefault()).Select(o => o.FieldName).FirstOrDefault();

                    string sAccount = objExcelSheetBOList.Where(o => o.FieldID > 0 && o.FieldID == (a1.Where(o1 =>
                     o1.FieldID == (a2.Where(o2 => o2.StandardFieldName.ToLower() == "ACCOUNT".ToLower())
                     .Select(o2 => o2.StandardFieldId).FirstOrDefault()))).Select(o1 => o1.ID).FirstOrDefault()).Select(o => o.FieldName).FirstOrDefault();

                    string sOldMeterNo = objExcelSheetBOList.Where(o => o.FieldID > 0 && o.FieldID == (a1.Where(o1 =>
                     o1.FieldID == (a2.Where(o2 => o2.StandardFieldName.ToLower() == "OLD METER NUMBER".ToLower())
                     .Select(o2 => o2.StandardFieldId).FirstOrDefault()))).Select(o1 => o1.ID).FirstOrDefault()).Select(o => o.FieldName).FirstOrDefault();

                    if (!string.IsNullOrEmpty(sAddressLatitude) && !string.IsNullOrEmpty(sAddressLongitude))
                    {
                        IsLatLongCalculated = true;
                    }

                    int rowNumber = 1;
                    dt.Columns.Add("ROWNO");
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        dtRow["ROWNO"] = rowNumber;
                        rowNumber++;
                    }


                    // List<dynamic> dynamicDt = dt.ToDynamic();
                    if (IsLatLongCalculated)
                    {
                        /// check if lat long is avilable then validate the same address with different


                        foreach (DataRow dtRow in dt.Rows)
                        {
                            var checkLatLon = dt.AsEnumerable().Where(o => o.Field<string>("ROWNO") != (string)dtRow["ROWNO"] && o.Field<string>(sStreet) == Convert.ToString(dtRow[sStreet]) &&
                                (o.Field<string>(sAddressLatitude) != Convert.ToString(dtRow[sAddressLatitude]) || o.Field<string>(sAddressLongitude) != Convert.ToString(dtRow[sAddressLongitude]))).Count();
                            if (checkLatLon > 0)
                            {
                                //if same street & different lat long avilable then add # and number infront of existing street name.
                                bool notExist = false;
                                int streetNumber = 1;
                                string newStreet = "";
                                while (notExist == false)
                                {
                                    newStreet = Convert.ToString(dtRow[sStreet]) + " #" + streetNumber.ToString();
                                    var checkNewStreet = dt.AsEnumerable().Where(o => o.Field<string>("ROWNO") != (string)dtRow["ROWNO"] && o.Field<string>(sStreet) == newStreet).Count();
                                    if (checkNewStreet == 0)
                                    {
                                        notExist = true;
                                    }
                                    else
                                    {
                                        streetNumber++;
                                    }
                                }
                                dtRow[sStreet] = newStreet;
                            }
                        }
                    }



                    var standardFieldAccount = objCompassEntities1.VW_GETTab6DropdownData.Where(o => o.ProjectID == ProjectID && o.FieldName == "ACCOUNT" && o.FieldType == 2).FirstOrDefault();
                    var standardFieldOldMeter = objCompassEntities1.VW_GETTab6DropdownData.Where(o => o.ProjectID == ProjectID && o.FieldName == "OLD METER NUMBER" && o.FieldType == 2).FirstOrDefault();
                    if (standardFieldAccount != null && standardFieldOldMeter != null)
                    {
                        string accountColumn = GetExcelColumnMapList.Where(o => o.FieldID == standardFieldAccount.tabelId
                            && o.FieldType == 2).Select(o => o.FKUploadedExcelDataDynamicColumn).FirstOrDefault();

                        string oldMeterColumn = GetExcelColumnMapList.Where(o => o.FieldID == standardFieldOldMeter.tabelId
                            && o.FieldType == 2).Select(o => o.FKUploadedExcelDataDynamicColumn).FirstOrDefault();

                        if (!string.IsNullOrEmpty(accountColumn) && !string.IsNullOrEmpty(oldMeterColumn))
                        {
                            //var watch1 = System.Diagnostics.Stopwatch.StartNew();//*****
                            //var watch2 = System.Diagnostics.Stopwatch.StartNew();//*****

                            foreach (DataRow dtRow in dt.Rows)
                            {

                                #region
                                try
                                {
                                    bool flagInsert = false;
                                    string account = Convert.ToString(dtRow[sAccount]).Trim();
                                    string oldMeterNo = Convert.ToString(dtRow[sOldMeterNo]).Trim();

                                    var list = objCompassEntities1.UploadedExcelDatas.Where(o => o.FKProjectId == ProjectID && o.Active == 1).ToList();
                                    bool visitDone = false;
                                    if (list != null && list.Count > 0)
                                    {
                                        var oldObject = list.Where(o => (string)o.GetType().GetProperty(accountColumn).GetValue(o) == account
                                          && (string)o.GetType().GetProperty(oldMeterColumn).GetValue(o) == oldMeterNo).FirstOrDefault();



                                        /// checked if data is already present in the database. 
                                        /// If yes then checked the record is visited or not in the field data table.
                                        if (oldObject == null)
                                        {
                                            oldObject = new UploadedExcelData();
                                            flagInsert = true;
                                        }
                                        else
                                        {
                                            var checkDataSubmitted = objCompassEntities1.TblProjectFieldDatas.Where(o => o.tblUploadedData_Id == oldObject.Id && o.Active == 1).Count();

                                            if (checkDataSubmitted > 0)
                                            {
                                                skipedRecordsCount++;   // Count Skipt records
                                                skipedAccountsNo += "<br />"+ Convert.ToString(dtRow[sAccount]) + "";
                                                visitDone = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        flagInsert = true;
                                    }


                                    if (visitDone == false)
                                    {
                                        int i = 0;
                                        //updatedRecordsCount++;
                                        skipedRecordsCount++;   // Count Skipt records only count skip records
                                        UploadedExcelData objRecord = new UploadedExcelData();

                                        objRecord.FKProjectId = ProjectID;
                                        objRecord.Street12 = Convert.ToString(dtRow[sStreet]);
                                        objRecord.Account12 = Convert.ToString(dtRow[sAccount]);

                                        Type tClass = objRecord.GetType();
                                        PropertyInfo[] pClass = tClass.GetProperties();

                                        foreach (DataColumn dtCol in dt.Columns)
                                        {
                                            var mappedColumnName = GetExcelColumnMapList.Where(o => o.ExcelFieldName == dtCol.ColumnName).FirstOrDefault();
                                            if (mappedColumnName != null)
                                            {
                                                string sCurrentData = Convert.ToString(dtRow[dtCol]);
                                                PropertyInfo currentProperty = pClass.Where(o => o.Name == mappedColumnName.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                                                currentProperty.SetValue(objRecord, sCurrentData, null);
                                            }
                                        }


                                        if (flagInsert)
                                        {
                                            i++;
                                            objRecord.Active = 1;
                                            objRecord.CreatedOn = new CommonFunctions().ServerDate();
                                            objRecord.CreatedBy = long.Parse(userId.ToString());
                                            objRecord.SyncEnable = 1;

                                            objCompassEntities1.Configuration.AutoDetectChangesEnabled = false;
                                            objCompassEntities1.Configuration.AutoDetectChangesEnabled = false;
                                            objCompassEntities1.Configuration.ValidateOnSaveEnabled = false;
                                            objCompassEntities1.UploadedExcelDatas.Add(objRecord);
                                            objCompassEntities1.SaveChanges();

                                            bool statusCycleOfficeMerge = Boolean.Parse(ConfigurationManager.AppSettings["mergeCycleOfficeColumn"]);
                                            if (statusCycleOfficeMerge)     // This Changes applied for TNMP ==> Electric City Project
                                            {
                                                mergeColumnData(ProjectID, objRecord.Id);
                                            }

                                            skipedRecordsCount--;
                                            newRecordsCount++;

                                        }
                                        if (i == 0)
                                        {
                                            skipedAccountsNo += "<br />" + Convert.ToString(dtRow[sAccount]) + "";
                                        }
                                        result = true;
                                    }
                                }
                                catch
                                {


                                }
                                #endregion
                            }
                            //objCompassEntities1.SaveChanges();
                        }
                    }
                    dt1 = dt;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                returnMessage += "<br/>" + ex.Message;
                result = false;
            }
            return result;
        }

        public void mergeColumnData(long projetID, long uploadedId)
        {
            try
            {
                CompassEntities dbcontext = new CompassEntities();
                InstallerDAL inDal = new InstallerDAL();
                string dyCycleColName = inDal.getDynamicColumnName("CYCLE", Convert.ToString(projetID));
                string dyOfficeColName = inDal.getDynamicColumnName("OFFICE", Convert.ToString(projetID));
                string dyStreetColName = inDal.getDynamicColumnName("STREET", Convert.ToString(projetID));
                string dyUnitColName = inDal.getDynamicColumnName("UNIT", Convert.ToString(projetID));

                var objRecord = dbcontext.UploadedExcelDatas.Where(o => o.FKProjectId == projetID && o.Id == uploadedId).FirstOrDefault();
                Type tClass = objRecord.GetType();
                PropertyInfo[] pClass = tClass.GetProperties();
                PropertyInfo currentProperty;
                if (!string.IsNullOrEmpty(dyCycleColName) && !string.IsNullOrEmpty(dyOfficeColName))
                {
                    var propertyInfoCycle = objRecord.GetType().GetProperty(dyCycleColName);
                    var cycleValue = propertyInfoCycle.GetValue(objRecord);

                    var propertyInfoOffice = objRecord.GetType().GetProperty(dyOfficeColName);
                    var officeValue = propertyInfoOffice.GetValue(objRecord);

                    currentProperty = pClass.Where(o => o.Name == dyCycleColName).FirstOrDefault();
                    currentProperty.SetValue(objRecord, (officeValue+" "+cycleValue), null);        // Merge value Cycle and Office
                    /*currentProperty = pClass.Where(o => o.Name == dyOfficeColName).FirstOrDefault();
                    currentProperty.SetValue(objRecord, officeValue, null);*/
                }
                if (!string.IsNullOrEmpty(dyStreetColName) && !string.IsNullOrEmpty(dyUnitColName))
                {
                    var propertyInfoStreet = objRecord.GetType().GetProperty(dyStreetColName);
                    var streetValue = propertyInfoStreet.GetValue(objRecord);

                    var propertyInfoUnit = objRecord.GetType().GetProperty(dyUnitColName);
                    var unitValue = propertyInfoUnit.GetValue(objRecord);

                    if (!string.IsNullOrEmpty(unitValue.ToString()))
                    {
                        currentProperty = pClass.Where(o => o.Name == dyStreetColName).FirstOrDefault();
                        currentProperty.SetValue(objRecord, (streetValue+" "+ unitValue), null);     // Merge value Street and Unit
                        objRecord.Street12 = (streetValue + " " + unitValue).ToString(); // Street/ Address value stored in two columns in table
                    }
                }
                dbcontext.SaveChanges();

            }
            catch (Exception ex)
            {
            }

        }

        protected void FindCoordinates(string Address, out string Lat, out string lng)
        {
            Lat = lng = "";

            string googleKey = System.Configuration.ConfigurationManager.AppSettings["googleapiKey"].ToString();
            XDocument xDoc = XDocument.Load("https://maps.googleapis.com/maps/api/geocode/xml?address=" + Address + "&sensor=false");
            //XDocument xDoc = XDocument.Load("https://maps.googleapis.com/maps/api/geocode/xml?key=AIzaSyDkVNe1laxoqcdWflfTbWgcb50Nr5XSVv8&address=" + Address + "&sensor=false");
            var resultElements = (from result in xDoc.Descendants("GeocodeResponse").Elements("result")
                                  select new
                                  {
                                      TrunkPointName = result.Elements("address_component").FirstOrDefault().Element("long_name").Value,
                                      Address = result.Element("formatted_address").Value,
                                      Lat = Convert.ToDecimal(result.Element("geometry").Element("location").Element("lat").Value),
                                      lng = Convert.ToDecimal(result.Element("geometry").Element("location").Element("lng").Value)
                                  }).ToList();
            if (resultElements != null)
            {
                if (resultElements.Count > 0)
                {

                    Lat = Convert.ToString(resultElements.FirstOrDefault().Lat);
                    lng = Convert.ToString(resultElements.FirstOrDefault().lng);
                    if (Lat == null || Lat == "")
                    {

                    }
                }
            }
            //var result = new System.Net.WebClient().DownloadString(Address);
            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(result);
            //XmlNodeList parentNode = doc.GetElementsByTagName("location");

            //foreach (XmlNode childrenNode in parentNode)
            //{
            //    Lat = childrenNode.SelectSingleNode("lat").InnerText;
            //    lng = childrenNode.SelectSingleNode("lng").InnerText;
            //}

            //var address = Address;

            //var locationService = new GoogleLocationService();
            //var point = locationService.GetLatLongFromAddress(Address);

            //if (point != null)
            //{
            //    Lat = Convert.ToString(point.Latitude);
            //    lng = Convert.ToString(point.Longitude);
            //}
        }

        public List<ExcelSheetBO> GetOldExcelMappingdata(int ProjectID, out string returnMessage)
        {
            CompassEntities CompassEntities = new CompassEntities();
            List<ExcelSheetBO> GetoldRecords = new List<ExcelSheetBO>();
            returnMessage = "Unable to Get Records";
            try
            {
                GetoldRecords = (from tbl in CompassEntities.TblProjectExcelMappings
                                 where tbl.Active == 1 && tbl.ProjectID == ProjectID
                                 select new ExcelSheetBO
                                 {
                                     FieldID = tbl.FieldID,
                                     FieldName = tbl.ExcelFieldName,
                                     FieldType = tbl.FieldType

                                 }).ToList();
                return GetoldRecords;
            }
            catch (Exception ex)
            {
                returnMessage += "<br/>" + ex.Message;
            }
            return GetoldRecords;
        }


        public List<VW_GETTab6DropdownData> GetViewData(long ProjectID)
        {
            List<VW_GETTab6DropdownData> objMeterTypeModel = new List<VW_GETTab6DropdownData>();

            CompassEntities CompassEntities = new CompassEntities();
            objMeterTypeModel = (from b in CompassEntities.VW_GETTab6DropdownData
                                 where b.ProjectID == ProjectID
                                 select b).Distinct().OrderBy(o => o.FieldName).ToList();



            return objMeterTypeModel;
        }


        public List<SP_GetFieldInfoForProject_Result> GetFieldInfoForProject(long ProjectID)
        {
            List<SP_GetFieldInfoForProject_Result> objMeterTypeModel = new List<SP_GetFieldInfoForProject_Result>();

            CompassEntities CompassEntities = new CompassEntities();
            var resultdata = CompassEntities.SP_GetFieldInfoForProject(ProjectID).Where(a => a.FieldName != "CYCLE" && a.FieldName != "ROUTE").ToList();


            foreach (var item in resultdata)
            {
                if (item.FieldName == "ADDITIONAL SERVICES")
                {
                    item.isMappedToExcel = 1;
                }
            }

            return resultdata;
        }

        public List<string> GetPictureListData(long ServiceId)
        {
            List<string> picList = new List<string>();

            CompassEntities CompassEntities = new CompassEntities();
            var a = CompassEntities.TblProjectMasterFieldRelations.Where(o => o.ID == ServiceId).FirstOrDefault();
            picList = CompassEntities.VW_GetPictures.Where(o => o.tblService_ServiceId == a.MasterFieldID).OrderBy(o => o.Picture).Select(o => o.Picture).ToList();

            return picList;
        }


        public bool GetProjectStatusData(long projectId)
        {
            List<string> picList = new List<string>();

            CompassEntities CompassEntities = new CompassEntities();
            var result = CompassEntities.tblProjects.Where(o => o.ProjectId == projectId).FirstOrDefault();
            result.ProjectStatus = "Complete";
            CompassEntities.SaveChanges();
            return true;
        }


        //Created on 9-Jun-2016 by Aniket Jadhav
        //to retrieve uploaded excel list data by projectid
        public List<ProjectEditModel> GetUploadedDatabyProjectID(long ProjectID)
        {
            List<ProjectEditModel> uploadedDataList = new List<ProjectEditModel>();
            try
            {
                using (CompassEntities dbcontext = new CompassEntities())
                {
                    //uploadedDataList = CompassEntities.tblUploadedDatas.Where(a => a.Active == 1 && a.ProjectId == ProjectID).ToList();
                    SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = "PROC_ProjectMaster_EditAllData";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = sqlConnection1;
                    cmd.Parameters.Add("@projectId", ProjectID);
                    cmd.CommandTimeout = sqlConnection1.ConnectionTimeout;

                    sqlConnection1.Open();

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        uploadedDataList.Add(new ProjectEditModel
                        {
                            ID = long.Parse(reader["ID"].ToString()),
                            Account = reader["Account"].ToString(),
                            Cycle = reader["Cycle"].ToString(),
                            Latitude = reader["Latitude"].ToString(),
                            Longitude = reader["Longitude"].ToString(),
                            Route = reader["Route"].ToString(),
                            Street = reader["Street"].ToString(),
                            CreatedOn = reader["CreatedOn"].ToString()
                        });
                    }
                    // Data is accessible through the DataReader object here.

                    sqlConnection1.Close();
                    reader.Close();
                    reader.Dispose();
                    cmd.Dispose();
                    sqlConnection1.Dispose();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return uploadedDataList;
        }


        public bool GetLatLongCalculation(long ProjectID)
        {
            using (CompassEntities dbcontext = new CompassEntities())
            {
                var TotalRecords = dbcontext.UploadedExcelDatas.Where(o => o.FKProjectId == ProjectID && o.Active == 1).Count();
                var latLongCount = dbcontext.UploadedExcelDatas.Where(o => o.FKProjectId == ProjectID && o.Active == 1 && o.AddressLatitude != null
                    && o.AddressLongitude != null).Count();

                return (TotalRecords == latLongCount ? false : true);
            }
        }


        //to retrieve uploaded excel data by uploadid
        // don't use this method
        public ProjectEditModel GetUploadedDatabyUploadID_old(long uploadId)
        {
            ProjectEditModel uploadedData = new ProjectEditModel();
            try
            {
                using (CompassEntities dbcontext = new CompassEntities())
                {
                    SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = "PROC_ProjectMaster_EditSingleData";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = sqlConnection1;
                    cmd.Parameters.Add("@UploadId", uploadId);


                    sqlConnection1.Open();

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        uploadedData = new ProjectEditModel
                        {
                            ID = long.Parse(reader["ID"].ToString()),
                            Latitude = reader["Latitude"].ToString(),
                            Longitude = reader["Longitude"].ToString(),
                            Street = reader["Street"].ToString(),
                            Account = reader["Account"].ToString(),
                            Cycle = reader["Cycle"].ToString(),
                            Route = reader["Route"].ToString(),
                            OldMeterType = reader["OldMeterType"].ToString(),
                            OldMeterMake = reader["OldMeterMake"].ToString(),
                            OldMeterNumber = reader["OldMeterNumber"].ToString(),
                            OldMeterSize = reader["OldMeterSize"].ToString(),
                            OldMeterRadioNumber = reader["OldMeterRadioNumber"].ToString(),
                            Customer = reader["Customer"].ToString()
                        };
                    }
                    // Data is accessible through the DataReader object here.

                    sqlConnection1.Close();
                    reader.Close();
                    reader.Dispose();
                    cmd.Dispose();
                    sqlConnection1.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return uploadedData;
        }

        public string getDynamicColValue(long projectId, long id, string DynamicColumnName)
        {
            try
            {
                string dycColValue = "";
                CompassEntities dbcontext = new CompassEntities();
                SqlConnection _sqlCon = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                _sqlCon.Open();
                SqlCommand _cmd = new SqlCommand("Select " + DynamicColumnName + " from UploadedExcelData where id='" + id + "' and FKProjectId='" + projectId + "'", _sqlCon);
                _cmd.CommandType = CommandType.Text;
                SqlDataAdapter _sda = new SqlDataAdapter(_cmd);
                DataTable _dts = new DataTable();
                _sda.Fill(_dts);
                if (_dts.Rows.Count > 0)
                {
                    dycColValue = _dts.Rows[0][0].ToString();
                }
                _sqlCon.Close();
                return dycColValue;
            }
            catch
            {
                return "";
            }

        }
        //to retrieve uploaded excel data by uploadid
        public string GetUploadedDatabyUploadID(long uploadId)
        {
            ProjectEditModel uploadedData = new ProjectEditModel();
            StringBuilder htmlData = new StringBuilder();
            try
            {
                using (CompassEntities dbcontext = new CompassEntities())
                {
                    var getData = dbcontext.UploadedExcelDatas.Where(o => o.Id == uploadId && o.Active == 1).FirstOrDefault();

                    var getDynamicColumnName = dbcontext.TblProjectExcelMappings.Where(o => o.ProjectID == getData.FKProjectId && o.Active == 1).ToList();

                    htmlData.Append("<input type='hidden' name='Id' id='"+ getData.Id + "' value="+ getData.Id + "  class='inputclass form-control' />");

                    // Note:- Two times loop applied because data show order wise Account, Cycle And Route
                    foreach (var item in getDynamicColumnName)
                    {
                        string dynamicColValue = getDynamicColValue(getData.FKProjectId, uploadId, item.FKUploadedExcelDataDynamicColumn);
                        if (item.ExcelFieldName.ToUpper() == "ACCOUNT" || item.ExcelFieldName.ToUpper() == "CYCLE" || item.ExcelFieldName.ToUpper() == "ROUTE")
                        {
                            //dynamicColValue = dynamicColValue.Replace("'", "&#39;");  // Single quote ' replace srting
                            dynamicColValue = dynamicColValue.Replace(@"""", "&#34;");  // Single quote " replace srting
                            htmlData.Append("<div class='form-group'>");
                            htmlData.Append("<label for='name' class='col-lg-4 col-sm-4 col-xs-12 control-label'>" + item.ExcelFieldName + "</label>");
                            htmlData.Append("<div class='col-lg-8 col-sm-9 col-xs-9'>");
                            htmlData.Append("<input type ='text' class='inputclass form-control class55' " +
                                           "name='" + item.FKUploadedExcelDataDynamicColumn + "' readonly maxlength='50' " +
                                           "id='" + item.FKUploadedExcelDataDynamicColumn +
                                           "' value=\"" + dynamicColValue + "\"></input>" +
                                           "</div>" +
                                           "</div>");

                        }
                    }
                    foreach (var item in getDynamicColumnName)
                    {
                        string dynamicColValue = getDynamicColValue(getData.FKProjectId, uploadId, item.FKUploadedExcelDataDynamicColumn);
                        if (item.ExcelFieldName.ToUpper() == "STREET")
                        {
                            //dynamicColValue = dynamicColValue.Replace("'", "&#39;");  // Single quote ' replace srting
                            dynamicColValue = dynamicColValue.Replace(@"""", "&#34;");  // Single quote " replace srting
                            htmlData.Append("<div class='form-group'>");
                            htmlData.Append("<label for='name' class='col-lg-4 col-sm-4 col-xs-12 control-label'>" + item.ExcelFieldName + "</label>");
                            htmlData.Append("<div class='col-lg-8 col-sm-9 col-xs-9'>");
                            htmlData.Append("<input type ='text' class='inputclass form-control' " +
                                            "name='" + "STREET_" + item.FKUploadedExcelDataDynamicColumn + "' maxlength='500' " +
                                            "id='" + item.FKUploadedExcelDataDynamicColumn +
                                            "' value=\"" + dynamicColValue + "\"></input>" +
                                            "</div>" +
                                            "</div>");
                        }
                    }

                    // Add lat & Long
                    htmlData.Append("<div class='form-group'>");
                    htmlData.Append("<label for='name' class='col-lg-4 col-sm-4 col-xs-12 control-label'>Address Latitude</label>");
                    htmlData.Append("<div class='col-lg-8 col-sm-9 col-xs-9'>");
                    htmlData.Append("<input type ='text' class='inputclass form-control' " +
                                    "name='AddressLatitude' maxlength='500' " +
                                    "id='AddressLatitude'" +
                                    "' value=\"" + getData.AddressLatitude + "\"></input>" +
                                    "</div>" +
                                    "</div>");

                    htmlData.Append("<div class='form-group'>");
                    htmlData.Append("<label for='name' class='col-lg-4 col-sm-4 col-xs-12 control-label'>Address Longitude</label>");
                    htmlData.Append("<div class='col-lg-8 col-sm-9 col-xs-9'>");
                    htmlData.Append("<input type ='text' class='inputclass form-control' " +
                                    "name='AddressLongitude' maxlength='500' " +
                                    "id='AddressLongitude'" +
                                    "' value=\"" + getData.AddressLongitude + "\"></input>" +
                                    "</div>" +
                                    "</div>");
                    //end hare
                    foreach (var item in getDynamicColumnName)
                    {
                        string dynamicColValue = getDynamicColValue(getData.FKProjectId, uploadId, item.FKUploadedExcelDataDynamicColumn);
                        if (item.ExcelFieldName.ToUpper() != "ACCOUNT" && item.ExcelFieldName.ToUpper() != "CYCLE" && item.ExcelFieldName.ToUpper() != "ROUTE" && item.ExcelFieldName.ToUpper() != "STREET")
                        {
                            //dynamicColValue = dynamicColValue.Replace("'", "&#39;");  // Single quote ' replace srting
                            dynamicColValue = dynamicColValue.Replace(@"""", "&#34;");  // Single quote " replace srting
                            htmlData.Append("<div class='form-group'>");
                            htmlData.Append("<label for='name' class='col-lg-4 col-sm-4 col-xs-12 control-label'>" + item.ExcelFieldName + "</label>");
                            htmlData.Append("<div class='col-lg-8 col-sm-9 col-xs-9'>");
                            htmlData.Append("<input type ='text' class='inputclass form-control' " +
                                            "name='" + item.FKUploadedExcelDataDynamicColumn + "' maxlength='500' " +
                                            "id='" + item.FKUploadedExcelDataDynamicColumn +
                                            "' value=\"" + dynamicColValue + "\"></input>" +
                                            "</div>" +
                                            "</div>");
                            
                        }
                    }


                    /*SqlConnection sqlConnection1 = new SqlConnection(dbcontext.Database.Connection.ConnectionString);
                    SqlCommand cmd = new SqlCommand();
                    SqlDataReader reader;

                    cmd.CommandText = "PROC_ProjectMaster_EditSingleData";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = sqlConnection1;
                    cmd.Parameters.Add("@UploadId", uploadId);


                    sqlConnection1.Open();

                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        uploadedData = new ProjectEditModel
                        {
                            ID = long.Parse(reader["ID"].ToString()),
                            Latitude = reader["Latitude"].ToString(),
                            Longitude = reader["Longitude"].ToString(),
                            Street = reader["Street"].ToString(),
                            Account = reader["Account"].ToString(),
                            Cycle = reader["Cycle"].ToString(),
                            Route = reader["Route"].ToString(),
                            OldMeterType = reader["OldMeterType"].ToString(),
                            OldMeterMake = reader["OldMeterMake"].ToString(),
                            OldMeterNumber = reader["OldMeterNumber"].ToString(),
                            OldMeterSize = reader["OldMeterSize"].ToString(),
                            OldMeterRadioNumber = reader["OldMeterRadioNumber"].ToString(),
                            Customer = reader["Customer"].ToString()
                        };
                    }
                    // Data is accessible through the DataReader object here.

                    sqlConnection1.Close();
                    reader.Close();
                    reader.Dispose();
                    cmd.Dispose();
                    sqlConnection1.Dispose();*/
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            string _result = htmlData.ToString();
            return _result;
            //return uploadedData;
        }

        #region Insert,Update & Delete

        // This method don't use
        public bool UpdateUploadData(ProjectEditModel ObjectToCreaetOrUpdate, bool StreetChanged, out string Message)
        {
            bool result = false;
            Message = "Unable to Update Record.";
            string Citylat, Citylng;
            string address, lat, lng;
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                UploadedExcelData objChk = CompassEntities.UploadedExcelDatas.Where(o => o.Id == ObjectToCreaetOrUpdate.ID && o.Active == 1).FirstOrDefault();

                var getData = CompassEntities.VW_GETTab6DropdownData.Where(o => o.ProjectID == objChk.FKProjectId && o.FieldName == "STREET").FirstOrDefault();
                var streetColumnname = CompassEntities.TblProjectExcelMappings.Where(o => o.Active == 1 && o.FieldID == getData.tabelId && o.FieldType == getData.FieldType).FirstOrDefault();

                if (objChk != null)
                {
                    if (!StreetChanged)
                    {
                        objChk.AddressLatitude = ObjectToCreaetOrUpdate.Latitude;
                        objChk.AddressLongitude = ObjectToCreaetOrUpdate.Longitude;
                        objChk.Street12 = ObjectToCreaetOrUpdate.Street;
                        //objChk.Account12 = ObjectToCreaetOrUpdate.Account;
                        //objChk.AddressLatitude = ObjectToCreaetOrUpdate.Latitude;
                        //objChk.AddressLongitude = ObjectToCreaetOrUpdate.Longitude;

                        Type tClass = objChk.GetType();
                        PropertyInfo[] pClass = tClass.GetProperties();
                        PropertyInfo currentProperty = pClass.Where(o => o.Name == streetColumnname.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                        currentProperty.SetValue(objChk, ObjectToCreaetOrUpdate.Street, null);
                    }
                    else
                    {
                        var projectcitystate = (from p in CompassEntities.tblProjects
                                                join c in CompassEntities.TblCityMasters on p.CityId equals c.CityId
                                                join s in CompassEntities.tblStateMasters on p.StateId equals s.StateId
                                                where p.ProjectId == objChk.FKProjectId && p.Active == 1 && c.Active == 1
                                                select new
                                                {
                                                    CityName = c.CityName,
                                                    StateName = s.StateName
                                                }).FirstOrDefault();

                        FindCityCoordinates(Convert.ToString(projectcitystate.CityName) + ", " + projectcitystate.StateName + ", USA", out Citylat, out Citylng);

                        if (Convert.ToString(ObjectToCreaetOrUpdate.Street).Trim().Contains('#'))
                        {
                            int startIndex = 0;
                            int endIndex = ObjectToCreaetOrUpdate.Street.IndexOf('#'); ;
                            int length = endIndex - startIndex - 1;
                            String computeddress = ObjectToCreaetOrUpdate.Street.Substring(0, length);
                            address = computeddress + ", " + Convert.ToString(projectcitystate.CityName) + ", " + projectcitystate.StateName + ", USA";
                        }
                        else
                        {
                            address = ObjectToCreaetOrUpdate.Street + ", " + Convert.ToString(projectcitystate.CityName) + ", " + projectcitystate.StateName + ", USA";
                        }
                        if (!string.IsNullOrEmpty(address))
                        {
                            System.Threading.Thread.Sleep(100);
                            FindCoordinates(address, out lat, out lng, Citylat, Citylng);
                            if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lng))
                            {
                                objChk.AddressLatitude = lat;
                                objChk.AddressLongitude = lng;
                                objChk.Street12 = ObjectToCreaetOrUpdate.Street;
                                //objChk.Street = ObjectToCreaetOrUpdate.Street;
                                Type tClass = objChk.GetType();
                                PropertyInfo[] pClass = tClass.GetProperties();
                                PropertyInfo currentProperty = pClass.Where(o => o.Name == streetColumnname.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                                currentProperty.SetValue(objChk, ObjectToCreaetOrUpdate.Street, null);
                            }
                        }
                    }

                    //uploaded more data edit
                    // get dymanic colulmn name
                    //var dycColCycle = CompassEntities.VW_GETTab6DropdownData.Where(o => o.ProjectID == objChk.FKProjectId && o.FieldName == "CYCLE").FirstOrDefault();
                    //var dycColRoute = CompassEntities.VW_GETTab6DropdownData.Where(o => o.ProjectID == objChk.FKProjectId && o.FieldName == "ROUTE").FirstOrDefault();
                    // Note Cycle and Rouet don't update because these field view page only readonly
                    Type tClassColName = objChk.GetType();
                    PropertyInfo[] proClass = tClassColName.GetProperties();

                    var getOldMeterType = CompassEntities.VW_GETTab6DropdownData.Where(o => o.ProjectID == objChk.FKProjectId && o.FieldName == "OLD METER TYPE").FirstOrDefault();
                    var OldMeterTypeColumnname = CompassEntities.TblProjectExcelMappings.Where(o => o.Active == 1 && o.FieldID == getOldMeterType.tabelId && o.FieldType == getOldMeterType.FieldType).FirstOrDefault();
                    PropertyInfo curProOldMeterType = proClass.Where(o => o.Name == OldMeterTypeColumnname.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                    curProOldMeterType.SetValue(objChk, ObjectToCreaetOrUpdate.OldMeterType, null);

                    var getOldMeterMake = CompassEntities.VW_GETTab6DropdownData.Where(o => o.ProjectID == objChk.FKProjectId && o.FieldName == "OLD METER MAKE").FirstOrDefault();
                    var OldMeterMakeColumnname = CompassEntities.TblProjectExcelMappings.Where(o => o.Active == 1 && o.FieldID == getOldMeterMake.tabelId && o.FieldType == getOldMeterMake.FieldType).FirstOrDefault();
                    PropertyInfo curProOldMeterMake = proClass.Where(o => o.Name == OldMeterMakeColumnname.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                    curProOldMeterMake.SetValue(objChk, ObjectToCreaetOrUpdate.OldMeterMake, null);

                    var getOldMeterNumber = CompassEntities.VW_GETTab6DropdownData.Where(o => o.ProjectID == objChk.FKProjectId && o.FieldName == "OLD METER NUMBER").FirstOrDefault();
                    var OldMeterNumberColumnname = CompassEntities.TblProjectExcelMappings.Where(o => o.Active == 1 && o.FieldID == getOldMeterNumber.tabelId && o.FieldType == getOldMeterNumber.FieldType).FirstOrDefault();
                    PropertyInfo curProOldMeterNumber = proClass.Where(o => o.Name == OldMeterNumberColumnname.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                    curProOldMeterNumber.SetValue(objChk, ObjectToCreaetOrUpdate.OldMeterNumber, null);

                    var getOldMeterSize = CompassEntities.VW_GETTab6DropdownData.Where(o => o.ProjectID == objChk.FKProjectId && o.FieldName == "OLD METER SIZE").FirstOrDefault();
                    var OldMeterSizeColumnname = CompassEntities.TblProjectExcelMappings.Where(o => o.Active == 1 && o.FieldID == getOldMeterSize.tabelId && o.FieldType == getOldMeterSize.FieldType).FirstOrDefault();
                    PropertyInfo curProOldMeterSize = proClass.Where(o => o.Name == OldMeterSizeColumnname.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                    curProOldMeterSize.SetValue(objChk, ObjectToCreaetOrUpdate.OldMeterSize, null);

                    var getOldMeterRadioNumber = CompassEntities.VW_GETTab6DropdownData.Where(o => o.ProjectID == objChk.FKProjectId && o.FieldName == "OLD METER RADIO NUMBER").FirstOrDefault();
                    var OldMeterRadioNumberColumnname = CompassEntities.TblProjectExcelMappings.Where(o => o.Active == 1 && o.FieldID == getOldMeterRadioNumber.tabelId && o.FieldType == getOldMeterRadioNumber.FieldType).FirstOrDefault();
                    PropertyInfo curProOldMeterRadioNumber = proClass.Where(o => o.Name == OldMeterRadioNumberColumnname.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                    curProOldMeterRadioNumber.SetValue(objChk, ObjectToCreaetOrUpdate.OldMeterRadioNumber, null);

                    var getCustomer = CompassEntities.VW_GETTab6DropdownData.Where(o => o.ProjectID == objChk.FKProjectId && o.FieldName == "CUSTOMER").FirstOrDefault();
                    var CustomerColumnname = CompassEntities.TblProjectExcelMappings.Where(o => o.Active == 1 && o.FieldID == getCustomer.tabelId && o.FieldType == getCustomer.FieldType).FirstOrDefault();
                    PropertyInfo curProCustomer = proClass.Where(o => o.Name == CustomerColumnname.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                    curProCustomer.SetValue(objChk, ObjectToCreaetOrUpdate.Customer, null);
                    //ends hare

                    CompassEntities.SaveChanges();
                    // CompassEntities.STP_SetGroupIDVisitId_UploadID(ObjectToCreaetOrUpdate.ID);
                    CompassEntities.PROC_SetGroupIDVisitId_UploadID(ObjectToCreaetOrUpdate.ID);
                    Message = CommonFunctions.strRecordUpdated;
                    result = true;

                }
                else
                {
                    Message += "<br/>" + "Record not found";
                }

            }
            catch (Exception ex)
            {

                Message += "<br/>" + ex.Message;
            }

            return result;
        }

        #endregion

        // Update lat long 
        public bool UpdateLatLong(UploadedExcelData objChk1, string newStreetValue)
        {
            bool result = false;
            string Citylat, Citylng;
            string address, lat, lng;
            CompassEntities CompassEntities = new CompassEntities();
            UploadedExcelData objChk = CompassEntities.UploadedExcelDatas.Where(o => o.Id == objChk1.Id && o.Active == 1).FirstOrDefault();
            var getData = CompassEntities.VW_GETTab6DropdownData.Where(o => o.ProjectID == objChk.FKProjectId && o.FieldName == "STREET").FirstOrDefault();
            var streetColumnname = CompassEntities.TblProjectExcelMappings.Where(o => o.Active == 1 && o.FieldID == getData.tabelId && o.FieldType == getData.FieldType).FirstOrDefault();
            try
            {
                var projectcitystate = (from p in CompassEntities.tblProjects
                                        join c in CompassEntities.TblCityMasters on p.CityId equals c.CityId
                                        join s in CompassEntities.tblStateMasters on p.StateId equals s.StateId
                                        where p.ProjectId == objChk.FKProjectId && p.Active == 1 && c.Active == 1
                                        select new
                                        {
                                            CityName = c.CityName,
                                            StateName = s.StateName
                                        }).FirstOrDefault();

                FindCityCoordinates(Convert.ToString(projectcitystate.CityName) + ", " + projectcitystate.StateName + ", USA", out Citylat, out Citylng);

                if (Convert.ToString(newStreetValue).Trim().Contains('#'))
                {
                    int startIndex = 0;
                    int endIndex = newStreetValue.IndexOf('#'); ;
                    int length = endIndex - startIndex - 1;
                    String computeddress = newStreetValue.Substring(0, length);
                    address = computeddress + ", " + Convert.ToString(projectcitystate.CityName) + ", " + projectcitystate.StateName + ", USA";
                }
                else
                {
                    address = newStreetValue + ", " + Convert.ToString(projectcitystate.CityName) + ", " + projectcitystate.StateName + ", USA";
                }
                if (!string.IsNullOrEmpty(address))
                {
                    System.Threading.Thread.Sleep(100);
                    FindCoordinates(address, out lat, out lng, Citylat, Citylng);
                    if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lng))
                    {
                        objChk.AddressLatitude = lat;
                        objChk.AddressLongitude = lng;
                        objChk.Street12 = newStreetValue;
                        Type tClass = objChk.GetType();
                        PropertyInfo[] pClass = tClass.GetProperties();
                        PropertyInfo currentProperty = pClass.Where(o => o.Name == streetColumnname.FKUploadedExcelDataDynamicColumn).FirstOrDefault();
                        currentProperty.SetValue(objChk, newStreetValue, null);
                    }
                }
                CompassEntities.SaveChanges();
                CompassEntities.PROC_SetGroupIDVisitId_UploadID(objChk.Id);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        //

        public bool SaveProductServiceList(long ProjectID, long serviceId, long productid, int quantity, int projectServiceId)
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();

                var serviceId1 = CompassEntities.TblProjectMasterFieldRelations.Where(a => a.ID == serviceId && a.Active == 1).Select(a => a.MasterFieldID).FirstOrDefault();

                var projectService = CompassEntities.ProjectServiceProductRelations.Where(a => a.ID == projectServiceId && a.Active == 1).FirstOrDefault();
                if (projectService == null)
                {
                    ProjectServiceProductRelation objmodel = new ProjectServiceProductRelation();
                    objmodel.FKProductId = productid;
                    objmodel.FKProjectID = ProjectID;
                    objmodel.FKServiceId = serviceId1; 
                    objmodel.Quantity = quantity;
                    objmodel.Active = 1;
                    objmodel.CreatedBy = 1;
                    objmodel.CreatedOn = new CommonFunctions().ServerDate();

                    CompassEntities.ProjectServiceProductRelations.Add(objmodel);
                    CompassEntities.SaveChanges();
                }
                else
                {
                    projectService.FKProductId = productid;
                    projectService.Quantity = quantity;
                    CompassEntities.SaveChanges();
                }

                return true;


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<ProductServiceBo> GetProductServiceListbyProjectId(long ProjectID, long serviceId)
        {
            CompassEntities CompassEntities = new CompassEntities();
            try
            {
                //var objServiceSmallModelBo2 = CompassEntities.ProjectServiceProductRelations.Where(a => a.FKProjectID == ProjectID).ToList();

                var objServiceSmallModelBo = (from m in CompassEntities.ProjectServiceProductRelations
                                              join c in CompassEntities.TblProjectMasterFieldRelations on m.FKServiceId equals c.MasterFieldID
                                              join p in CompassEntities.VW_GetProductData on m.FKProductId equals p.Id
                                              where m.Active == 1 && m.FKProjectID == ProjectID && c.ID == serviceId
                                              select new ProductServiceBo
                                              {
                                                  ProjectServiceId = m.ID,
                                                  productId = p.Id,
                                                  productName = p.ProductName,
                                                  Make = p.Make,
                                                  Type = p.MeterType,
                                                  Size = p.MeterSize,
                                                  UtilityName = p.UtilityName,
                                                  Quantity = m.Quantity == null ? 0 : (int)m.Quantity,
                                                  SerialNumberRequired = p.SerialNumberRequired
                                              }).ToList();

                //var serviceId1 = CompassEntities.TblProjectMasterFieldRelations.AsNoTracking().Where(o => o.ID == serviceId).FirstOrDefault();

                //var objServiceSmallModelBo = CompassEntities.ProjectServiceProductRelations.Where(o => o.FKProjectID == ProjectID 
                //&& o.FKServiceId == serviceId1.MasterFieldID && o.Active == 1).ToList();


                return objServiceSmallModelBo;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public bool DeleteProductService(long serviceId)
        {
            bool success = false;
            CompassEntities CompassEntities = new CompassEntities();
            try
            {
                var projectService = CompassEntities.ProjectServiceProductRelations.Where(a => a.ID == serviceId && a.Active == 1).FirstOrDefault();
                if (projectService != null)
                {
                    projectService.Active = 0;
                    projectService.ModifiedBy = 1;
                    projectService.CreatedOn = new CommonFunctions().ServerDate();
                    CompassEntities.SaveChanges();
                    success = true;
                }
            }
            catch (Exception)
            {

                throw;
            }

            return success;
        }


        protected void FindCityCoordinates(string Address, out string Lat, out string lng)
        {
            Lat = lng = "";


            // XDocument xDoc = XDocument.Load("https://maps.googleapis.com/maps/api/geocode/xml?address=" + Address + "&sensor=false");
            XDocument xDoc = XDocument.Load("https://maps.googleapis.com/maps/api/geocode/xml?key=" + Convert.ToString(ConfigurationManager.AppSettings["googleapiKey"]) + "&address=" + Address + "&sensor=true");
            var resultElements = (from result in xDoc.Descendants("GeocodeResponse").Elements("result")
                                  select new
                                  {
                                      TrunkPointName = result.Elements("address_component").FirstOrDefault().Element("long_name").Value,
                                      Address = result.Element("formatted_address").Value,
                                      Lat = Convert.ToDecimal(result.Element("geometry").Element("location").Element("lat").Value),
                                      lng = Convert.ToDecimal(result.Element("geometry").Element("location").Element("lng").Value)
                                  }).ToList();
            if (resultElements != null)
            {
                if (resultElements.Count > 0)
                {

                    Lat = Convert.ToString(resultElements.FirstOrDefault().Lat);
                    lng = Convert.ToString(resultElements.FirstOrDefault().lng);
                    if (Lat == null || Lat == "")
                    {

                    }
                }
            }
            //var result = new System.Net.WebClient().DownloadString(Address);
            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(result);
            //XmlNodeList parentNode = doc.GetElementsByTagName("location");

            //foreach (XmlNode childrenNode in parentNode)
            //{
            //    Lat = childrenNode.SelectSingleNode("lat").InnerText;
            //    lng = childrenNode.SelectSingleNode("lng").InnerText;
            //}

            //var address = Address;

            //var locationService = new GoogleLocationService();
            //var point = locationService.GetLatLongFromAddress(Address);

            //if (point != null)
            //{
            //    Lat = Convert.ToString(point.Latitude);
            //    lng = Convert.ToString(point.Longitude);
            //}
        }

        protected void FindCoordinates(string Address, out string Lat, out string lng, String Citylat, String Citylng)
        {
            Lat = lng = "";
            double calculatedDistance = 0;

            // XDocument xDoc = XDocument.Load("https://maps.googleapis.com/maps/api/geocode/xml?address=" + Address + "&sensor=false");
            XDocument xDoc = XDocument.Load("https://maps.googleapis.com/maps/api/geocode/xml?key=" + Convert.ToString(ConfigurationManager.AppSettings["googleapiKey"]) + " &address=" + Address + "&sensor=false");
            var resultElements = (from result in xDoc.Descendants("GeocodeResponse").Elements("result")
                                  select new
                                  {
                                      TrunkPointName = result.Elements("address_component").FirstOrDefault().Element("long_name").Value,
                                      Address = result.Element("formatted_address").Value,
                                      Lat = Convert.ToDecimal(result.Element("geometry").Element("location").Element("lat").Value),
                                      lng = Convert.ToDecimal(result.Element("geometry").Element("location").Element("lng").Value)
                                  }).ToList();
            if (resultElements != null)
            {
                if (resultElements.Count > 0)
                {
                    //added by sanjiv to get lat long within 15vmiles of city
                    foreach (var result in resultElements)
                    {


                        Lat = Convert.ToString(result.Lat);
                        lng = Convert.ToString(result.lng);
                        if (Lat == null || Lat == "")
                        {

                        }
                        else
                        {

                            calculatedDistance = distance(Convert.ToDouble(result.Lat), Convert.ToDouble(result.lng), Convert.ToDouble(Citylat), Convert.ToDouble(Citylng), 'M');
                            if (calculatedDistance < 15)
                            {
                                Lat = Convert.ToString(result.Lat);
                                lng = Convert.ToString(result.lng);
                                break;
                            }
                            else
                            {
                                Lat = "";
                                lng = "";
                            }

                        }


                    }
                }
            }

        }


        private double distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }




        public List<long?> GetUserProjectListForDashBoard(string UtilityType, string ProjectStatus, long userId)
        {
            CompassEntities db = new CompassEntities();

            var ObjProjectList = db.PROC_GETUserActiveProject(userId, ProjectStatus, UtilityType).ToList();
            return ObjProjectList;
        }


        public List<long?> GetUserProjectList(long userId)
        {
            CompassEntities db = new CompassEntities();

            var ObjProjectList = db.PROC_GETUserActiveProject(userId, "Active", "Water").ToList();
            var ObjProjectList1 = db.PROC_GETUserActiveProject(userId, "Active", "Electric").ToList();
            var ObjProjectList2 = db.PROC_GETUserActiveProject(userId, "Active", "Gas").ToList();

            var projectList = new List<long?>();
            if (ObjProjectList != null && ObjProjectList.Count > 0)
            {
                projectList.AddRange(ObjProjectList);
            }
            if (ObjProjectList1 != null && ObjProjectList1.Count > 0)
            {
                projectList.AddRange(ObjProjectList1);
            }
            if (ObjProjectList2 != null && ObjProjectList2.Count > 0)
            {
                projectList.AddRange(ObjProjectList2);
            }

            return projectList;
        }
    }


    public static class DataTableExtensions
    {
        public static List<dynamic> ToDynamic(this DataTable dt)
        {
            var dynamicDt = new List<dynamic>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic dyn = new ExpandoObject();
                dynamicDt.Add(dyn);
                foreach (DataColumn column in dt.Columns)
                {
                    var dic = (IDictionary<string, object>)dyn;
                    dic[column.ColumnName] = row[column];
                }
            }
            return dynamicDt;
        }
    }
}
