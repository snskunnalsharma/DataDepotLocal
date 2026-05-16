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

namespace GarronT.CMS.Model.DAL
{
    public class UserRoleDAL : IDisposable
    {
        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(UserRoleDAL));
        #endregion
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

        public UserRoleDAL()
        {

        }

        public List<UserRoleModel> GetAllRoles()
        {
            try
            {
                CompassEntities objTollFreeWidgetEntities = new CompassEntities();

                List<UserRoleModel> objRolesBo = (from b in objTollFreeWidgetEntities.AspNetRoles
                                                  where b.IsSysAdmin != true
                                                  orderby b.SortingNumber
                                                  select new UserRoleModel
                                                  {
                                                      Id = b.Id,
                                                      Name = b.Name
                                                  }).ToList();

                return objRolesBo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// gets service list for selected service
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <returns></returns>
        public List<UsersSmallModel> GeInstallerListCurrentProject(long ProjectId)
        {
            try
            {
                CompassEntities CompassEntities = new CompassEntities();
                List<UsersSmallModel> UserModel = (from b in CompassEntities.TblProjectInstallers
                                                   where b.Active == 1 && b.ProjectID == ProjectId
                                                   select new UsersSmallModel
                                                   {
                                                       UserId = b.tblUsers_UserID,
                                                       UserName = CompassEntities.tblUsers.Where(a => a.UserID == b.tblUsers_UserID).Select(m => m.FirstName).FirstOrDefault()
                                                   }).ToList();


                //foreach (var item in MeterSizeModel)
                //{
                //    item.UserName = CompassEntities.tblUsers.Where(a => a.UserID == item.UserId).Select(m => m.FirstName).FirstOrDefault();
                // }
                var data = new WebServiceUserRoleDAL().GetAllUsersBasedOnRoleWithSelf("Installer").Select(o => o.ChildId).ToList();
                UserModel = UserModel.Where(o => data.Contains(o.UserId)).ToList();
                return UserModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool CreateOrUpdateMasterField(long projectId, int userId, string[] objInstallerList, out string returnMessage)
        {
            bool result = false;
            returnMessage = "Unable to Create / Update State.";

            CompassEntities CompassEntities = new CompassEntities();
            try
            {




                var oldList = CompassEntities.TblProjectInstallers.AsEnumerable().Where(o => o.ProjectID == projectId &&
                        o.Active == 1).ToList();
                if (oldList != null && oldList.Count() > 0)
                {
                    foreach (var item in oldList)
                    {
                        item.ModifiedBy = userId;
                        item.Active = 0;
                        item.ModifiedOn = new CommonFunctions().ServerDate();
                        CompassEntities.SaveChanges();

                        var getFlag = objInstallerList.Where(o => o == item.tblUsers_UserID.ToString()).FirstOrDefault();
                        if (getFlag == null)
                        {
                            //installer not in the new list.

                            var currentData = CompassEntities.UploadedExcelDatas.Where(o => o.Active == 1 && o.FKProjectId == projectId).ToList();
                            foreach (var itemRecord in currentData)
                            {
                                var getInfo = CompassEntities.tblInstallerMapDatas.Where(o => o.Active == 1
                                    && o.InstallerId == item.tblUsers_UserID && o.FK_UploadedExcelData_Id == itemRecord.Id).ToList();

                                foreach (var item1 in getInfo)
                                {
                                    var checkStatus = CompassEntities.TblProjectFieldDatas.Where(o => o.FK_UploadedExcelData_Id == item1.FK_UploadedExcelData_Id
                                        && o.InstallerId == item1.InstallerId && o.Active == 1).FirstOrDefault();
                                    if (checkStatus == null)
                                    {
                                        item1.Active = 0;
                                        CompassEntities.SaveChanges();
                                    }
                                }
                            }

                        }
                    }
                }

                foreach (var item in objInstallerList)
                {
                    long id = long.Parse(item);
                    var obj = CompassEntities.TblProjectInstallers.AsEnumerable().Where(o => o.ProjectID == projectId && o.tblUsers_UserID == id).FirstOrDefault();

                    if (obj == null)
                    {
                        obj = new TblProjectInstaller();
                        obj.tblUsers_UserID = id;
                        obj.ProjectID = projectId;
                        obj.CreatedBy = userId;
                        obj.Active = 1;
                        obj.CreatedOn = new CommonFunctions().ServerDate();
                        CompassEntities.TblProjectInstallers.Add(obj);
                        CompassEntities.SaveChanges();
                    }
                    else
                    {
                        obj.ModifiedBy = userId;
                        obj.Active = 1;
                        obj.ModifiedOn = new CommonFunctions().ServerDate();
                        CompassEntities.SaveChanges();
                    }

                    //var checkAuditRecordPresent = CompassEntities.AuditInstallerPercentages.Where(o => o.FK_ProjectId == projectId && o.FK_InstallerId == id).ToList();

                    //if (checkAuditRecordPresent == null || checkAuditRecordPresent.Count == 0)
                    //{

                    //    var project = CompassEntities.tblProjects.Where(o => o.ProjectId == projectId && o.Active == 1).FirstOrDefault();

                    //    var customerAuditSettingList = CompassEntities.CustomerAuditSettings.Where(o => o.FK_CustomerId == project.ClientId && o.Active == 1).ToList();

                    //    if (customerAuditSettingList != null && customerAuditSettingList.Count > 0)
                    //    {

                    //        foreach (var item1 in customerAuditSettingList)
                    //        {
                    //            AuditInstallerPercentage obj1 = new AuditInstallerPercentage();

                    //            obj1.Active = 1;
                    //            obj1.CreatedBy = userId;
                    //            obj1.CreatedOn = new CommonFunctions().ServerDate();
                    //            obj1.AuditPercentage = item1.AuditPercentage;
                    //            obj1.FK_CustomerId = (long)project.ClientId;
                    //            obj1.FK_InstallerId = id;
                    //            obj1.FK_ProjectId = projectId;
                    //            obj1.EndDay = item1.EndDay;
                    //            obj1.StartDay = item1.StartDay;

                    //            CompassEntities.AuditInstallerPercentages.Add(obj1);
                    //            CompassEntities.SaveChanges();

                    //        }

                    //    }
                    //}
                    //else
                    //{
                    //    foreach (var item1 in checkAuditRecordPresent)
                    //    {
                    //        if (item1.Active == 0)
                    //        {
                    //            item1.Active = 1;
                    //            item1.ModifiedBy = userId;
                    //            item1.ModifiedOn = new CommonFunctions().ServerDate();
                    //            CompassEntities.SaveChanges();
                    //        }
                    //    }
                    //}


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

        #region Createdby AniketJ on 7-Sep-2016 for Role Master

        /// <summary>
        /// Get Role List
        /// </summary>
        /// <returns></returns>
        public List<RoleBO> GetRoleList()
        {
            CompassEntities _dbcontext = new CompassEntities();
            List<RoleBO> roleList = new List<RoleBO>();
            try
            {
                roleList = _dbcontext.AspNetRoles.Select(a => new RoleBO
                {
                    Id = a.Id,
                    Name = a.Name,
                    IsAdmin = (a.IsSysAdmin == true) ? "Yes" : "No"

                }).ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return roleList;
        }

        /// <summary>
        /// Create New Role
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="returnMessage"></param>
        /// <returns></returns>
        public bool CreateOrUpdate(string roleName, out string returnMessage)
        {
            bool result = false;
            returnMessage = "Unable to Add Role.";

            CompassEntities CompassEntities = new CompassEntities();
            try
            {
                var roleStore = new RoleStore<IdentityRole>();
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                if (!roleManager.RoleExists(roleName))
                {
                    var roleresult = roleManager.Create(new IdentityRole(roleName));
                    result = true;
                    returnMessage = "Role Created Successfully";
                }
                else
                {
                    result = false;
                    returnMessage = roleName + " Role Already Exist";
                }

            }
            catch (Exception ex)
            {
                returnMessage += "<br/>" + ex.Message;
                result = false;
            }
            return result;
        }


        #endregion

    }
}
