using AutoMapper;
using KSPL.AIBC.ARM.Model.BO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;

namespace GarronT.CMS.Model.BO
{
    public class UserDAL
    {
        public UserDAL()
        {
            //
            // TODO: Add constructor logic here
            //
        }


        public bool ActivateRecord(int Id, out string msg)
        {
            try
            {
                msg = "";
                CompassEntities CompassEntities = new CompassEntities();

                tblUser ObjFaq = (from b in CompassEntities.tblUsers
                                  where b.UserID == Id
                                  select b).FirstOrDefault();

                if (ObjFaq != null)
                {
                    ObjFaq.Active = 1;
                    //ObjFaq.ModifiedBy = Convert.ToInt64(HttpContext.Current.Session["UserId"].ToString());
                    ObjFaq.ModifiedOn = new CommonFunctions().ServerDate();
                    CompassEntities.SaveChanges();

                    msg = "Record activated.";
                    return true;
                    // ts.Complete();
                }
                else
                {
                    msg = "Unable to Activate Record.";
                    return false;
                }
                //}


            }
            catch (Exception)
            {

                throw;
            }
        }


        public bool DeActivateRecord(int Id, out string msg)
        {
            try
            {

                msg = "";
                CompassEntities CompassEntities = new CompassEntities();

                int count = int.Parse(Convert.ToString(CompassEntities.PROC_UserUpComingProjects1(Id).FirstOrDefault()));

                bool ProjectMapExist = false;

                if (count > 0)
                {
                    ProjectMapExist = true;
                }
                else
                {

                    count = int.Parse(Convert.ToString(CompassEntities.PROC_UserUpComingProjects2(Id).FirstOrDefault()));

                    if (count > 0)
                    {
                        ProjectMapExist = true;
                    }
                }

                if (ProjectMapExist == false)
                {
                    tblUser ObjFaq = (from b in CompassEntities.tblUsers where b.UserID == Id select b).FirstOrDefault();

                    if (ObjFaq != null)
                    {
                        ObjFaq.Active = 0;
                        //ObjFaq.ModifiedBy = Convert.ToInt64(HttpContext.Current.Session["UserId"].ToString());
                        ObjFaq.ModifiedOn = new CommonFunctions().ServerDate();
                        CompassEntities.SaveChanges();

                        msg = "Record deactivated.";
                        return true;
                        // ts.Complete();
                    }
                    else
                    {
                        msg = "Unable to deactivate record.";
                        return false;
                    }
                }
                else
                {
                    msg = "Fail to deactivate record. User is active in current projects.";
                    return false;
                }





                //}


            }
            catch (Exception)
            {

                throw;
            }
        }


        public List<UserRoleModel> GetusersreDeatils(int id)
        {
            CompassEntities CompassEntities = new CompassEntities();
            var list = new List<UserRoleModel>();

            var objSubMenuesBo = (from pd in CompassEntities.tblUsers
                                  join od in CompassEntities.AspNetRoles on pd.RoleID equals od.Id
                                  where pd.UserID == id
                                  orderby od.Id
                                  select new
                                  {
                                      od.Id,
                                      od.Name
                                  }).ToList();

            foreach (var obj in objSubMenuesBo)
            {

                UserRoleModel myobj = new UserRoleModel();
                //     myobj.TempID = i;
                myobj.Id = obj.Id;
                myobj.Name = obj.Name;

                list.Add(myobj);
            }
            return list;
        }

        public UsersModel GetUserById(long Id)
        {

            CompassEntities objCompassEntities = new CompassEntities();
            UsersModel objClientModel = objCompassEntities.tblUsers.AsNoTracking().Where(b => b.UserID == Id).
                Select(b =>
                                          new UsersModel
                                         {
                                             UserId = b.UserID,
                                             LogInId = b.UserName,
                                             UserName = b.UserName,
                                             FirstName = b.FirstName,
                                             Email = b.Email,
                                             MobileNo = b.MobileNo,
                                             Address = b.Address,
                                             RoleID = b.RoleID,
                                             Password = b.Password,
                                             Confirmpassword = b.Password,
                                             aspnet_UserId = b.aspnet_UserId,

                                             StateID = b.StateID,
                                             CityID = b.CityID,
                                             ZipCode = b.ZipCode,
                                             Gender = b.Gender,
                                             HomePhoneNumber = b.HomePhoneNumber,
                                             InstallerImage = b.InstallerImage,

                                             //vehicle
                                             //VehicleMake = b.VehicleMake,
                                             //VehicleModel = b.VehicleModel,
                                             //VehicleYear = b.VehicleYear,
                                             //VehicleLicensePlateNumber = b.VehicleLicensePlateNumber,
                                             //Vehicle = b.Vehicle,
                                             //VehicleColor = b.VehicleColor,
                                             //ImageofVehicle = b.ImageofVehicle,

                                             JobType = b.JobType,
                                             FormType = b.FormType,
                                             //PasswordExpiryDays=Convert.ToInt32(b.PasswordExpiryDays),
                                             Active = b.Active,
                                             ManagerId = b.ManagerId,
                                             FieldSupervisorId = b.FieldSupervisorId,
                                             ManagerApprovalRequired = b.ManagerApprovalRequired

                                         }).FirstOrDefault();

            objClientModel.objWarehouseUserList = objCompassEntities.WarehouseUserRelations.AsNoTracking().Where(b => b.Active == 1 && b.FKUserId == Id).
                Select(o => new WarehouseUserBO { Id = o.Id, FKUserId = o.FKUserId, FKWareHouseId = o.FKWarehouseId }).ToList();

            return objClientModel;

        }

        public List<UserFormTypeModel> GetUserFormTypeId(long Id)
        {

            CompassEntities objCompassEntities = new CompassEntities();
            List<UserFormTypeModel> objClientModel = (from b in objCompassEntities.tblUserFormTypeRelations
                                                      where b.UserID == Id && b.Active == 1
                                                      select new UserFormTypeModel
                                                      {
                                                          UserID = b.UserID,
                                                          FormTypeID = b.FormTypeID
                                                      }).ToList();

            return objClientModel;

        }


        public string gebyaspuserid(long id)
        {
            CompassEntities objCompassEntities = new CompassEntities();
            string users = Convert.ToString(objCompassEntities.tblUsers.Where(o => o.UserID == id).Select(o => o.aspnet_UserId).FirstOrDefault());
            return users;
        }

        public List<ProjectInfo> CheckAssignedProjects(int ID)
        {
            var list = new List<ProjectInfo>();
            CompassEntities CompassEntities = new CompassEntities();
            list = CompassEntities.TblProjectInstallers.Where(o => o.tblUsers_UserID == ID && o.Active == 1).Select(a => new ProjectInfo
            {
                project_Id = a.ProjectID,
                projectName = CompassEntities.tblProjects.Where(b => b.ProjectId == a.ProjectID && b.ProjectStatus == "Active" && a.Active == 1).Select(b => b.ProjectName).FirstOrDefault()

            }).ToList();
            return list;
        }

        public List<tblUser> GetUserByID(int ID)
        {
            var list = new List<tblUser>();
            CompassEntities CompassEntities = new CompassEntities();
            list = CompassEntities.tblUsers.Where(o => o.UserID == ID && o.Active == 1).ToList();
            return list;
        }


        public List<tblUser> GetUserRecords()
        {
            var list = new List<tblUser>();
            CompassEntities CompassEntities = new CompassEntities();
            list = CompassEntities.tblUsers.AsNoTracking().Where(o => o.Active == 1).ToList();
            return list;
        }
        public List<tblUser> GetDeActiveRecords()
        {
            var list = new List<tblUser>();
            CompassEntities CompassEntities = new CompassEntities();
            var list1 = CompassEntities.tblUsers.Where(o => o.Active == 0).ToList();
            if (list1 != null && list1.Count > 0)
            {
                list = list1.Select(o => new tblUser { UserName = o.UserName, FirstName = o.FirstName, RoleID = o.RoleID, Email = o.Email, MobileNo = o.MobileNo, UserID = o.UserID }).ToList();
            }
            return list;
        }

        //public bool CreateOrUpdate(UsersModel ObjectToCreaetOrUpdate, List<UsersServiceModel> UsersServiceList, VehicleModelData VehicleModelData, out string Message)
        public bool CreateOrUpdate(UsersModel ObjectToCreaetOrUpdate, UserViewModel UserViewModel, out string Message, ref long userid, int currentUserId)
        {
            bool result = false;
            Message = "Unable to Create / Update User.";
            try
            {
                CompassEntities objTollFreeWidgetEntities = new CompassEntities();

                tblUser objChk = (from b in objTollFreeWidgetEntities.tblUsers
                                  where b.UserName.ToLower().Trim() == ObjectToCreaetOrUpdate.UserFullName.ToLower().Trim()
                                      && b.UserID != ObjectToCreaetOrUpdate.UserId && b.Active == 1
                                  select b).FirstOrDefault();

                if (objChk == null)//No duplicate records
                {
                    if (ObjectToCreaetOrUpdate.UserId == 0) //new record for insert
                    {
                        tblUser createOrUpdate = new tblUser();
                        Mapper.DynamicMap<UsersModel, tblUser>(ObjectToCreaetOrUpdate, createOrUpdate);

                        createOrUpdate.CreatedBy = currentUserId;
                        objTollFreeWidgetEntities.tblUsers.Add(createOrUpdate);
                        objTollFreeWidgetEntities.SaveChanges();
                        userid = createOrUpdate.UserID;

                        long UserId = userid;

                        if (UserViewModel.VehicleModelData != null)
                        {
                            tblVehicleMaster tblVehicleMaster = new tblVehicleMaster();
                            tblVehicleMaster.MakeID = UserViewModel.VehicleModelData.VehicleMake;
                            tblVehicleMaster.ModelID = UserViewModel.VehicleModelData.VehicleModel;
                            tblVehicleMaster.Color = UserViewModel.VehicleModelData.VehicleColor;
                            tblVehicleMaster.LicensePlate = UserViewModel.VehicleModelData.VehicleLicensePlateNumber;
                            tblVehicleMaster.Year = UserViewModel.VehicleModelData.VehicleYear;
                            tblVehicleMaster.Active = 1;
                            tblVehicleMaster.UserId = createOrUpdate.UserID;

                            objTollFreeWidgetEntities.tblVehicleMasters.Add(tblVehicleMaster);
                            objTollFreeWidgetEntities.SaveChanges();
                        }


                        if (UserViewModel.ServiceRatesList != null)
                        {
                            foreach (var item in UserViewModel.ServiceRatesList)
                            {
                                tblInstallerServiceRate tblInstallerServiceRate = new tblInstallerServiceRate();
                                tblInstallerServiceRate.UserID = createOrUpdate.UserID;
                                tblInstallerServiceRate.ServiceID = item.ServiceID;
                                tblInstallerServiceRate.RatesUnit = item.RatesUnit;
                                tblInstallerServiceRate.ServiceRates = item.ServiceRates;
                                tblInstallerServiceRate.Active = 1;
                                objTollFreeWidgetEntities.tblInstallerServiceRates.Add(tblInstallerServiceRate);
                                objTollFreeWidgetEntities.SaveChanges();
                            }

                        }

                        if (UserViewModel.objFormTypeList != null && UserViewModel.objFormTypeList.Count > 0)
                        {
                            foreach (var item in UserViewModel.objFormTypeList)
                            {
                                tblUserFormTypeRelation tblUserFormTypeRelation = new tblUserFormTypeRelation();
                                tblUserFormTypeRelation.UserID = createOrUpdate.UserID;
                                tblUserFormTypeRelation.FormTypeID = item;
                                tblUserFormTypeRelation.Active = 1;
                                objTollFreeWidgetEntities.tblUserFormTypeRelations.Add(tblUserFormTypeRelation);
                                objTollFreeWidgetEntities.SaveChanges();
                            }

                        }





                        Message = CommonFunctions.strRecordCreated;

                        result = true;
                    }
                    else //update records
                    {
                        tblUser createOrUpdate = (from b in objTollFreeWidgetEntities.tblUsers
                                                  where b.Active == 1 && b.UserID == ObjectToCreaetOrUpdate.UserId
                                                  select b).FirstOrDefault();
                        if (createOrUpdate != null)
                        {
                            // Mapper.DynamicMap<UsersModel, tblUser>(ObjectToCreaetOrUpdate, createOrUpdate);

                            createOrUpdate.UserID = ObjectToCreaetOrUpdate.UserId;
                            createOrUpdate.UserName = ObjectToCreaetOrUpdate.UserName;
                            createOrUpdate.Password = ObjectToCreaetOrUpdate.Password;
                            createOrUpdate.FirstName = ObjectToCreaetOrUpdate.FirstName;
                            //createOrUpdate.LastName=ObjectToCreaetOrUpdate.La
                            // createOrUpdate.RegionID=ObjectToCreaetOrUpdate.re
                            createOrUpdate.RoleID = ObjectToCreaetOrUpdate.RoleID;
                            createOrUpdate.IsFirstLogin = ObjectToCreaetOrUpdate.IsFirstLogin;
                            createOrUpdate.PasswordExpiryDays = ObjectToCreaetOrUpdate.PasswordExpiryDays;
                            createOrUpdate.MobileNo = ObjectToCreaetOrUpdate.MobileNo;
                            createOrUpdate.Email = ObjectToCreaetOrUpdate.Email;
                            createOrUpdate.aspnet_UserId = ObjectToCreaetOrUpdate.aspnet_UserId;
                            createOrUpdate.Address = ObjectToCreaetOrUpdate.Address;
                            createOrUpdate.StateID = ObjectToCreaetOrUpdate.StateID;
                            createOrUpdate.CityID = ObjectToCreaetOrUpdate.CityID;
                            createOrUpdate.ZipCode = ObjectToCreaetOrUpdate.ZipCode;
                            createOrUpdate.Gender = ObjectToCreaetOrUpdate.Gender;
                            createOrUpdate.HomePhoneNumber = ObjectToCreaetOrUpdate.HomePhoneNumber;
                            createOrUpdate.ImageofVehicle = ObjectToCreaetOrUpdate.ImageofVehicle;
                            //createOrUpdate.InstallerImage = ObjectToCreaetOrUpdate.InstallerImage;

                            if (ObjectToCreaetOrUpdate.ManagerId == null || ObjectToCreaetOrUpdate.ManagerId == 0)
                            {
                                createOrUpdate.ManagerId = null;
                            }
                            else
                            {
                                createOrUpdate.ManagerId = ObjectToCreaetOrUpdate.ManagerId;
                            }

                            if (ObjectToCreaetOrUpdate.ManagerId == null || ObjectToCreaetOrUpdate.ManagerId == 0)
                            {
                                createOrUpdate.FieldSupervisorId = null;
                            }
                            else
                            {
                                createOrUpdate.FieldSupervisorId = ObjectToCreaetOrUpdate.FieldSupervisorId;
                            }

                            createOrUpdate.ManagerApprovalRequired = ObjectToCreaetOrUpdate.ManagerApprovalRequired;
                            objTollFreeWidgetEntities.SaveChanges();



                        }




                        tblVehicleMaster VehicleMaster = (from b in objTollFreeWidgetEntities.tblVehicleMasters
                                                          where b.Active == 1 && b.UserId == ObjectToCreaetOrUpdate.UserId
                                                          select b).FirstOrDefault();
                        if (VehicleMaster == null)
                        {
                            tblVehicleMaster tblVehicleMaster = new tblVehicleMaster();
                            tblVehicleMaster.MakeID = UserViewModel.VehicleModelData.VehicleMake;
                            tblVehicleMaster.ModelID = UserViewModel.VehicleModelData.VehicleModel;
                            tblVehicleMaster.Color = UserViewModel.VehicleModelData.VehicleColor;
                            tblVehicleMaster.LicensePlate = UserViewModel.VehicleModelData.VehicleLicensePlateNumber;
                            tblVehicleMaster.Year = UserViewModel.VehicleModelData.VehicleYear;
                            tblVehicleMaster.UserId = createOrUpdate.UserID;
                            if (UserViewModel.VehicleModelData.ImageofVehicle != null && UserViewModel.VehicleModelData.ImageofVehicle != "")
                            {
                                //tblVehicleMaster.ImageofVehicle = "Uploads/Vehicle/" + createOrUpdate.ImageofVehicle;

                            }
                            tblVehicleMaster.Active = 1;
                            objTollFreeWidgetEntities.tblVehicleMasters.Add(tblVehicleMaster);
                            objTollFreeWidgetEntities.SaveChanges();

                        }
                        else
                        {
                            VehicleMaster.MakeID = UserViewModel.VehicleModelData.VehicleMake;
                            VehicleMaster.ModelID = UserViewModel.VehicleModelData.VehicleModel;
                            VehicleMaster.Color = UserViewModel.VehicleModelData.VehicleColor;
                            VehicleMaster.LicensePlate = UserViewModel.VehicleModelData.VehicleLicensePlateNumber;
                            VehicleMaster.Year = UserViewModel.VehicleModelData.VehicleYear;



                            userid = createOrUpdate.UserID;
                            objTollFreeWidgetEntities.SaveChanges();
                        }




                        if (UserViewModel.ServiceRatesList != null)
                        {
                            foreach (var item in UserViewModel.ServiceRatesList)
                            {
                                tblInstallerServiceRate tblInstallerServiceRate = (from b in objTollFreeWidgetEntities.tblInstallerServiceRates
                                                                                   where b.Active == 1 && b.UserID == ObjectToCreaetOrUpdate.UserId && b.ID == item.ID
                                                                                   select b).FirstOrDefault();
                                if (tblInstallerServiceRate != null)
                                {
                                    tblInstallerServiceRate.UserID = createOrUpdate.UserID;
                                    tblInstallerServiceRate.ServiceID = item.ServiceID;
                                    tblInstallerServiceRate.RatesUnit = item.RatesUnit;
                                    tblInstallerServiceRate.ServiceRates = item.ServiceRates;
                                    //objTollFreeWidgetEntities.tblInstallerServiceRates.Add(tblInstallerServiceRate);
                                    objTollFreeWidgetEntities.SaveChanges();
                                }
                                else
                                {
                                    tblInstallerServiceRate tblInstallerServiceRateCreate = new tblInstallerServiceRate();
                                    tblInstallerServiceRateCreate.UserID = createOrUpdate.UserID;
                                    tblInstallerServiceRateCreate.ServiceID = item.ServiceID;
                                    tblInstallerServiceRateCreate.RatesUnit = item.RatesUnit;
                                    tblInstallerServiceRateCreate.ServiceRates = item.ServiceRates;
                                    tblInstallerServiceRateCreate.Active = 1;
                                    objTollFreeWidgetEntities.tblInstallerServiceRates.Add(tblInstallerServiceRateCreate);
                                    objTollFreeWidgetEntities.SaveChanges();
                                }
                            }
                        }

                        var FormTypeList = (from b in objTollFreeWidgetEntities.tblUserFormTypeRelations
                                            where b.Active == 1 && b.UserID == ObjectToCreaetOrUpdate.UserId
                                            select b).ToList();

                        if (FormTypeList.Count > 0)
                        {
                            foreach (var item in FormTypeList)
                            {
                                item.Active = 0;
                                objTollFreeWidgetEntities.SaveChanges();
                            }
                        }

                        if (UserViewModel.objFormTypeList != null && UserViewModel.objFormTypeList.Count > 0)
                        {
                            foreach (var item in UserViewModel.objFormTypeList)
                            {
                                var tblUserFormTypeRelation = (from b in objTollFreeWidgetEntities.tblUserFormTypeRelations
                                                               where b.Active == 0 && b.UserID == ObjectToCreaetOrUpdate.UserId && b.FormTypeID == item
                                                               select b).FirstOrDefault();

                                if (tblUserFormTypeRelation != null)
                                {
                                    tblUserFormTypeRelation.Active = 1;
                                }
                                else
                                {
                                    tblUserFormTypeRelation tblUserFormTypeRelationdata = new Model.tblUserFormTypeRelation();
                                    tblUserFormTypeRelationdata.UserID = createOrUpdate.UserID;
                                    tblUserFormTypeRelationdata.FormTypeID = item;
                                    tblUserFormTypeRelationdata.Active = 1;
                                    objTollFreeWidgetEntities.tblUserFormTypeRelations.Add(tblUserFormTypeRelationdata);
                                    objTollFreeWidgetEntities.SaveChanges();
                                }
                            }

                        }


                        objTollFreeWidgetEntities.SaveChanges();


                     


                        Message = CommonFunctions.strRecordUpdated;
                        result = true;
                    }
                }
                else
                {
                    if (objChk.Active == 1)
                    {
                        Message = CommonFunctions.strActiveRecordExists;
                        return false;
                    }
                    else if (objChk.Active == 0)
                    {
                        Message = CommonFunctions.strDeactiveRecordExists;
                        return false;
                    }
                    else
                    {
                        Message = CommonFunctions.strDefaultAdd;
                        return false;
                    }

                    //change message accroding to your requirement
                    // Message = "Sub Menu already exists.";
                    result = false;
                }
            }
            catch (Exception ex)
            {

                Message += "<br/>" + ex.Message;
            }

            return result;
        }


        public List<UsersServiceModel> UsersServiceModelList(long Id)
        {

            var list = new List<UsersServiceModel>();
            CompassEntities CompassEntities = new CompassEntities();

            //list = (from m in CompassEntities.tblServices
            //        join s in CompassEntities.tblInstallerServiceRates on m.ID equals s.ServiceID into joinedservice
            //        from s in joinedservice.DefaultIfEmpty()
            //        where (s.UserID == Id || s.UserID == null) && m.Active == 1

            //        select new UsersServiceModel
            //            {
            //                ID = s != null ? (Nullable<long>)s.ID : null,
            //                UserID = s != null ? (Nullable<long>)s.UserID : null,
            //                ServiceID = s != null ? (Nullable<long>)s.ServiceID : m.ID,
            //                ServiceName = m.Service,
            //                RatesUnit = s != null ? s.RatesUnit : null,
            //                ServiceRates = s != null ? s.ServiceRates : null,
            //                Active = 1


            //            }).ToList();



            var installerservice = (from m in CompassEntities.tblServices
                                    join s in CompassEntities.tblInstallerServiceRates on m.ID equals s.ServiceID
                                    where s.UserID == Id && m.Active == 1 && s.Active == 1
                                    select new UsersServiceModel
                                    {
                                        ID = s.ID,
                                        UserID = s.UserID,
                                        ServiceID = s.ServiceID,
                                        ServiceName = m.Service,
                                        RatesUnit = s.RatesUnit,
                                        ServiceRates = s.ServiceRates,
                                        Active = 1


                                    }).ToList();

            var servicelist = (from m in CompassEntities.tblServices.ToList()
                               where !installerservice.Any(c => c.ServiceID == m.ID) && m.Active == 1
                               select new UsersServiceModel
                               {
                                   ID = null,
                                   UserID = null,
                                   ServiceID = m.ID,
                                   ServiceName = m.Service,
                                   RatesUnit = null,
                                   ServiceRates = null,
                                   Active = 1


                               }).ToList();


            installerservice.AddRange(servicelist);
            return installerservice;

        }


        /// <summary>
        /// Returns List<long> of all usersids who are in role & active.
        /// </summary>
        /// <returns></returns>
        public List<long?> GetUserRecords(string roleName)
        {

            CompassEntities CompassEntities = new CompassEntities();
            var list = CompassEntities.aspnet_UsersInRoles(roleName).ToList();
            return list;
        }

        //Added by sominath to create ID card


        public List<StateVM> GetActiveStateRecords(long customerId)
        {
            CompassEntities CompassEntities = new CompassEntities();
            //List<StateVM> objSubMenuesBo = (from b in CompassEntities.tblStateMasters
            //                                select new StateVM
            //                                {
            //                                    StateID = b.StateId,
            //                                    StateName = b.StateName
            //                                }).OrderBy(t => t.StateName).ToList();
            List<StateVM> objSubMenuesBo = new List<StateVM>();
            var Customerlist = CompassEntities.TblStateCityLogoes.Where(o => o.Active == 1 && o.FK_CustomerId == customerId).Select(o => o.FK_StateId).Distinct().ToList();

            if (Customerlist != null)
            {
                objSubMenuesBo = CompassEntities.tblStateMasters.Where(o => Customerlist.Contains(o.StateId)).Select(o => new StateVM
                                  {
                                      StateID = o.StateId,
                                      StateName = o.StateName
                                  }).OrderBy(o => o.StateName).ToList();
            }


            return objSubMenuesBo;
        }


        public List<CustomerVM> GetActiveCustomerRecords()
        {
            var list = new List<CustomerVM>();
            CompassEntities CompassEntities = new CompassEntities();

            var Customerlist = CompassEntities.TblStateCityLogoes.Where(o => o.Active == 1).Select(o => o.FK_CustomerId).Distinct().ToList();

            if (Customerlist != null)
            {
                list = CompassEntities.tblClients.AsNoTracking().Where(o => o.Active == 1 && Customerlist.Contains(o.ClientId)).
                    Select(x => new CustomerVM { CustomerID = x.ClientId, CustomerName = x.ClientName }).ToList();
            }
            return list;
        }




        public List<CityVM> GetActiveCityRecords(long stateId, long customerId)
        {
            var list = new List<CityVM>();
            CompassEntities CompassEntities = new CompassEntities();

            var Customerlist = CompassEntities.TblStateCityLogoes.Where(o => o.Active == 1 && o.FK_StateId == stateId && o.FK_CustomerId == customerId).Select(o => o.FK_CityId).Distinct().ToList();

            if (Customerlist != null)
            {
                list = CompassEntities.TblCityMasters.Where(o => o.StateId == stateId && Customerlist.Contains(o.CityId)).Select(x => new CityVM { CityID = x.CityId, CityName = x.CityName }).ToList();
            }
            return list;
        }





        public List<CityImageNameList> GetCityImageListByID(long CityID)
        {
            CompassEntities CompassEntities = new CompassEntities();

            List<CityImageNameList> list = CompassEntities.TblStateCityLogoes.Where(o => o.FK_CityId == CityID).Select(o => new CityImageNameList
            {
                CityId = o.CityLogoID,
                ImageName = o.ImageName
            }).OrderBy(o => o.ImageName).ToList();

            return list;
        }




        public string GetLogoNameByImageName(long cityId, long customerId, out string ImageName)
        {
            string LogoName = "";
            using (CompassEntities _objCompass = new CompassEntities())
            {
                LogoName = _objCompass.TblStateCityLogoes.Where(o => o.Active == 1
                    && o.FK_CityId == cityId && o.FK_CustomerId == customerId).Select(o => o.CityLogoImagePath).FirstOrDefault();
                ImageName = _objCompass.TblStateCityLogoes.Where(o => o.Active == 1
                    && o.FK_CityId == cityId && o.FK_CustomerId == customerId).Select(o => o.ImageName).FirstOrDefault();
                return LogoName;
            }
        }




        public bool saveLogoMessage(long UserId, string RoleId, long CustomerId, long stateId, long cityId, string imageMessage)
        {
            using (CompassEntities _objCompass = new CompassEntities())
            {
                var LogoName = _objCompass.UserIcards.Where(o => o.Active == 1 && o.FKCityId == cityId
                    && o.FKStateId == stateId && o.FKUserId == UserId && o.FKRoleID == RoleId && o.FKCustomerId == CustomerId).FirstOrDefault();
                if (LogoName != null)
                {
                    LogoName.ICardMessage = imageMessage;
                    _objCompass.SaveChanges();
                }
                else
                {
                    LogoName = new UserIcard();
                    LogoName.FKCustomerId = CustomerId;
                    LogoName.FKCityId = cityId;
                    //LogoName.FKLogoID = LogoId;
                    LogoName.FKRoleID = RoleId;
                    LogoName.FKStateId = stateId;
                    LogoName.FKUserId = UserId;
                    LogoName.ICardMessage = imageMessage;
                    LogoName.Active = 1;
                    _objCompass.UserIcards.Add(LogoName);
                    _objCompass.SaveChanges();
                }
                return true;
            }
        }

        public string GETLogoMessage(long UserId, string RoleId, long CustomerId, long stateId, long cityId)
        {

            using (CompassEntities _objCompass = new CompassEntities())
            {
                var LogoName = _objCompass.UserIcards.Where(o => o.Active == 1 && o.FKCityId == cityId
                  && o.FKStateId == stateId && o.FKUserId == UserId && o.FKRoleID == RoleId && o.FKCustomerId == CustomerId).FirstOrDefault();
                if (LogoName != null)
                {
                    return LogoName.ICardMessage;
                }
                else
                {
                    return "";
                }
            }

        }





        public UserAspNetPermissionVM GetUserPermissionDetails(long UserId, out string userName)
        {
            userName = "";
            CompassEntities database1 = new CompassEntities();
            UserAspNetPermissionVM AspNetPermissionVM = new UserAspNetPermissionVM();
            try
            {
                var userDetail = database1.tblUsers.Where(o => o.UserID == UserId).FirstOrDefault();
                userName = userDetail.FirstName;
                var UserRoleList = database1.ASPNET_GetUserRoles(userDetail.UserID).ToList();

                string MaxRoleId = UserRoleList.Where(o => o.Name == "Manager").Select(o => o.RoleId).FirstOrDefault();
                if (string.IsNullOrEmpty(MaxRoleId))
                {
                    MaxRoleId = UserRoleList.Where(o => o.Name == "Field Supervisor").Select(o => o.RoleId).FirstOrDefault();
                }
                if (string.IsNullOrEmpty(MaxRoleId))
                {
                    MaxRoleId = UserRoleList.Where(o => o.Name == "Installer").Select(o => o.RoleId).FirstOrDefault();
                }
                if (string.IsNullOrEmpty(MaxRoleId))
                {
                    MaxRoleId = UserRoleList.Where(o => o.Name == "Auditor").Select(o => o.RoleId).FirstOrDefault();
                }
                if (string.IsNullOrEmpty(MaxRoleId))
                {
                    MaxRoleId = UserRoleList.Where(o => o.Name == "Warehouse Manager").Select(o => o.RoleId).FirstOrDefault();
                }
                if (string.IsNullOrEmpty(MaxRoleId))
                {
                    MaxRoleId = UserRoleList.Where(o => o.Name == "Reporter").Select(o => o.RoleId).FirstOrDefault();
                }

                if (string.IsNullOrEmpty(MaxRoleId))
                {
                    MaxRoleId = UserRoleList.Where(o => o.Name == "Customer").Select(o => o.RoleId).FirstOrDefault();
                }

                if (string.IsNullOrEmpty(MaxRoleId))
                {
                    MaxRoleId = UserRoleList.Where(o => o.Name == "IT Access").Select(o => o.RoleId).FirstOrDefault();
                }


                List<AspNetPermission> _permissions = database1.AspNetPermissions
                                  .OrderBy(wn => wn.Permission_Id)
                                  .Include(a => a.AspNetRolePermissions)
                                  .ToList();



                List<AspNetRole> _role = database1.AspNetRoles.Where(o => o.Id == MaxRoleId)
                                 .OrderBy(wn => wn.Name)
                                 .Include(a => a.AspNetRolePermissions)
                                 .ToList();
                List<AccessPermissionBo> objAccessPermissionBo = new List<AccessPermissionBo>();
                var ObjList = database1.AspNetRolePermissions.Where(o => o.Role_Id == MaxRoleId).ToList();
                foreach (var item in ObjList)
                {
                    var Obj = new AccessPermissionBo();
                    Obj.RoleAddChecked = item.AddOperation;
                    Obj.RoleViewChecked = item.ViewOperation;
                    Obj.RoleEditChecked = item.EditOperation;
                    Obj.RoleDeleteChecked = item.DeleteOperation;

                    if (item.AddOperation == false && item.ViewOperation == false && item.DeleteOperation == false && item.EditOperation == false)
                    {
                        Obj.BlockUserMenu = false;
                    }
                    else
                    {
                        Obj.BlockUserMenu = true;
                    }

                    Obj.PermissionID = item.Permission_Id.ToString();
                    Obj.RoleID = item.Role_Id;
                    objAccessPermissionBo.Add(Obj);
                }

                List<UserAccessPermissionBo> objUserAccessPermissionBo = new List<UserAccessPermissionBo>();

                var ObjList1 = database1.AspNetUserRolePermissions.Where(o => o.Active == true && o.UserId == UserId).ToList();
                foreach (var item in ObjList1)
                {
                    var Obj = new UserAccessPermissionBo();
                    Obj.RoleAddChecked = item.AddOperation;
                    Obj.RoleViewChecked = item.ViewOperation;
                    Obj.RoleEditChecked = item.EditOperation;
                    Obj.RoleDeleteChecked = item.DeleteOperation;
                    Obj.PermissionID = item.FKAspNetPermissionId.ToString();
                    objUserAccessPermissionBo.Add(Obj);
                }


                AspNetPermissionVM.AspNetPermissionList = _permissions;
                AspNetPermissionVM.ObjAccessPermissionBoList = objAccessPermissionBo;

                AspNetPermissionVM.objUserAccessPermissionBo = objUserAccessPermissionBo;

                AspNetPermissionVM.AspNetRoleList = _role;
                AspNetPermissionVM.UpdatePermission = false;
                AspNetPermissionVM.UpdateMessage = "";

              

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                database1.Dispose();
            }

            return AspNetPermissionVM;
        }





        public bool AssignPermissionRole(int currentUserId, List<UserAccessPermissionBo> PermissionList)
        {

            bool varResult = false;


            CompassEntities database = new CompassEntities();
            try
            {
                foreach (var item in PermissionList)
                {
                    long permissionID = Convert.ToInt64(item.PermissionID.Trim());

                    //  var permissionName = permissionModel.PermissionName.Trim();
                    //var roleid = item.RoleID.Trim();



                    AspNetUserRolePermission Obj = database.AspNetUserRolePermissions.Where(a => a.FKAspNetPermissionId == permissionID
                        && a.UserId == item.UserId && a.Active == true).FirstOrDefault();
                    if (Obj != null)
                    {
                        Obj.AddOperation = item.RoleAddChecked;
                        Obj.ViewOperation = item.RoleViewChecked;
                        Obj.EditOperation = item.RoleEditChecked;
                        Obj.DeleteOperation = item.RoleDeleteChecked;
                        Obj.ModifiedBy = currentUserId;
                        Obj.ModifiedOn = DateTime.Now;

                    }
                }
                database.SaveChanges();

                varResult = true;


            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                database.Dispose();
            }

            return varResult;

        }


        public void AddUserPermission(long UserId, int currentUserId)
        {

            CompassEntities database = new CompassEntities();

            var UserRoleList = database.ASPNET_GetUserRoles(UserId).ToList();
            /*Set user role priority*/
            string MaxRoleId = UserRoleList.Where(o => o.Name == "Manager").Select(o => o.RoleId).FirstOrDefault();
            if (string.IsNullOrEmpty(MaxRoleId))
            {
                MaxRoleId = UserRoleList.Where(o => o.Name == "Field Supervisor").Select(o => o.RoleId).FirstOrDefault();
            }
            if (string.IsNullOrEmpty(MaxRoleId))
            {
                MaxRoleId = UserRoleList.Where(o => o.Name == "Installer").Select(o => o.RoleId).FirstOrDefault();
            }
            if (string.IsNullOrEmpty(MaxRoleId))
            {
                MaxRoleId = UserRoleList.Where(o => o.Name == "Auditor").Select(o => o.RoleId).FirstOrDefault();
            }
            if (string.IsNullOrEmpty(MaxRoleId))
            {
                MaxRoleId = UserRoleList.Where(o => o.Name == "Warehouse Manager").Select(o => o.RoleId).FirstOrDefault();
            }
            if (string.IsNullOrEmpty(MaxRoleId))
            {
                MaxRoleId = UserRoleList.Where(o => o.Name == "Reporter").Select(o => o.RoleId).FirstOrDefault();
            }

            if (string.IsNullOrEmpty(MaxRoleId))
            {
                MaxRoleId = UserRoleList.Where(o => o.Name == "Customer").Select(o => o.RoleId).FirstOrDefault();
            }

            if (string.IsNullOrEmpty(MaxRoleId))
            {
                MaxRoleId = UserRoleList.Where(o => o.Name == "IT Access").Select(o => o.RoleId).FirstOrDefault();
            }

            var ObjNewPermissionList = database.AspNetRolePermissions.Select(o =>
                                         new AccessPermissionBo
                                         {
                                             PermissionID = o.Permission_Id.ToString(),
                                             RoleID = o.Role_Id,
                                             RoleAddChecked = o.AddOperation,
                                             RoleEditChecked = o.EditOperation,
                                             RoleDeleteChecked = o.DeleteOperation,
                                             RoleViewChecked = o.ViewOperation
                                         }).ToList();

            /*Get user existing role permissions*/
            var CurrentUserPermissionList = ObjNewPermissionList.Where(o => o.RoleID == MaxRoleId).ToList();

            foreach (var item2 in CurrentUserPermissionList)
            {

                /*If add permission given*/

                var abcItem = new AspNetUserRolePermission();
                abcItem.FKAspNetPermissionId = int.Parse(item2.PermissionID);
                abcItem.UserId = UserId;
                abcItem.AddOperation = item2.RoleAddChecked;
                abcItem.DeleteOperation = item2.RoleDeleteChecked;
                abcItem.EditOperation = item2.RoleEditChecked;
                abcItem.ViewOperation = item2.RoleViewChecked;
                abcItem.Active = true;
                abcItem.CreatedBy = currentUserId;
                abcItem.CreatedOn = DateTime.Now;
                database.AspNetUserRolePermissions.Add(abcItem);
                database.SaveChanges();



            }
        }



        public void UpdateUserPermission(long UserId, int currentUserId)
        {
            CompassEntities database = new CompassEntities();



            var UserRoleList = database.ASPNET_GetUserRoles(UserId).ToList();
            /*Set user role priority*/
            string MaxRoleId = UserRoleList.Where(o => o.Name == "Manager").Select(o => o.RoleId).FirstOrDefault();
            if (string.IsNullOrEmpty(MaxRoleId))
            {
                MaxRoleId = UserRoleList.Where(o => o.Name == "Field Supervisor").Select(o => o.RoleId).FirstOrDefault();
            }
            if (string.IsNullOrEmpty(MaxRoleId))
            {
                MaxRoleId = UserRoleList.Where(o => o.Name == "Installer").Select(o => o.RoleId).FirstOrDefault();
            }
            if (string.IsNullOrEmpty(MaxRoleId))
            {
                MaxRoleId = UserRoleList.Where(o => o.Name == "Auditor").Select(o => o.RoleId).FirstOrDefault();
            }
            if (string.IsNullOrEmpty(MaxRoleId))
            {
                MaxRoleId = UserRoleList.Where(o => o.Name == "Warehouse Manager").Select(o => o.RoleId).FirstOrDefault();
            }
            if (string.IsNullOrEmpty(MaxRoleId))
            {
                MaxRoleId = UserRoleList.Where(o => o.Name == "Reporter").Select(o => o.RoleId).FirstOrDefault();
            }

            if (string.IsNullOrEmpty(MaxRoleId))
            {
                MaxRoleId = UserRoleList.Where(o => o.Name == "Customer").Select(o => o.RoleId).FirstOrDefault();
            }

            if (string.IsNullOrEmpty(MaxRoleId))
            {
                MaxRoleId = UserRoleList.Where(o => o.Name == "IT Access").Select(o => o.RoleId).FirstOrDefault();
            }


            /*Get user existing role permissions*/
            var UserAccessList = database.AspNetUserRolePermissions.Where(o => o.Active == true && o.UserId == UserId).ToList();

            var CurrentRolePermissionList = database.AspNetRolePermissions.Where(o => o.Role_Id == MaxRoleId).Select(o =>
                  new AccessPermissionBo
                  {
                      PermissionID = o.Permission_Id.ToString(),
                      RoleID = o.Role_Id,
                      RoleAddChecked = o.AddOperation,
                      RoleEditChecked = o.EditOperation,
                      RoleDeleteChecked = o.DeleteOperation,
                      RoleViewChecked = o.ViewOperation
                  }).ToList();

            foreach (var item2 in CurrentRolePermissionList)
            {
                /*
                 * Check user permission 1 by 1 per form.
                 * Compare with old list & if the permission seems updated then check status for user & update.                              
                 */

                var abcItem = UserAccessList.Where(o => o.FKAspNetPermissionId.ToString() == item2.PermissionID).FirstOrDefault();


                if (abcItem != null)
                {

                    /*Compare if changes found then only update the record else not required to update*/
                    if (abcItem.AddOperation != item2.RoleAddChecked
                        || abcItem.DeleteOperation != item2.RoleDeleteChecked
                        || abcItem.EditOperation != item2.RoleEditChecked
                        || abcItem.ViewOperation != item2.RoleViewChecked)
                    {

                        /*If add permission given*/
                        if (abcItem.AddOperation != item2.RoleAddChecked)
                        {
                            abcItem.AddOperation = item2.RoleAddChecked;
                        }

                        /*If delete permission given*/
                        if (abcItem.DeleteOperation != item2.RoleDeleteChecked)
                        {
                            abcItem.DeleteOperation = item2.RoleDeleteChecked;
                        }

                        /*If edit permission given*/
                        if (abcItem.EditOperation != item2.RoleEditChecked)
                        {
                            abcItem.EditOperation = item2.RoleEditChecked;
                        }

                        /*If view permission given*/
                        if (abcItem.ViewOperation != item2.RoleViewChecked)
                        {
                            abcItem.ViewOperation = item2.RoleViewChecked;
                        }

                        abcItem.ModifiedBy = currentUserId;
                        abcItem.ModifiedOn = DateTime.Now;
                        database.SaveChanges();
                    }


                }
                else
                {
                    /*If add permission given*/

                    abcItem = new AspNetUserRolePermission();
                    abcItem.FKAspNetPermissionId = int.Parse(item2.PermissionID);
                    abcItem.UserId = UserId;
                    abcItem.AddOperation = item2.RoleAddChecked;
                    abcItem.DeleteOperation = item2.RoleDeleteChecked;
                    abcItem.EditOperation = item2.RoleEditChecked;
                    abcItem.ViewOperation = item2.RoleViewChecked;
                    abcItem.Active = true;
                    abcItem.CreatedBy = currentUserId;
                    abcItem.CreatedOn = DateTime.Now;
                    database.AspNetUserRolePermissions.Add(abcItem);
                    database.SaveChanges();
                }
            }
        }



    }
}
