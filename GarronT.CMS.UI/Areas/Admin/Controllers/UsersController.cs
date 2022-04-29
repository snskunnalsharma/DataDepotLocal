using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using GarronT.CMS.UI.Controllers;
using GarronT.CMS.UI.Models;
using KSPL.AIBC.ARM.Model.BO;
using KSPL.Indus.MSTS.UI.Filters;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using GarronT.CMS.UI.CustomHelpers;
using System.Data.SqlClient;
using System.Data;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [OutputCache(Duration = 0, VaryByParam = "none")]
    [SessionExpire]
    [Authorize]
    [RBAC]
    public class UsersController : Controller
    {

        #region Create Object ILog for Logger write
        private static readonly ILog log = LogManager.GetLogger(typeof(UsersController));
        #endregion

        //
        // GET: /Admin/Users/
        public ActionResult Index()
        {
            ViewBag.Form = "Index";

            return View();
        }


        [HttpGet]
        public ActionResult Create()
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Users", "Add");

            if (objPermissionCheck)
            {
                ViewBag.Form = "Create";
                return View();
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult EditUser(int id)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Users", "Edit");

            if (objPermissionCheck)
            {
                var list1 = new UserDAL().GetUserByID(id);
                if (list1.Count > 0)
                {
                    ViewBag.UserID = id;
                    ViewBag.Form = "Edit";
                    return View();
                }
                else
                {
                    return Json(new { success = objPermissionCheck, returnMessage = "" }, JsonRequestBehavior.AllowGet);
                }
                //return View("Index");
                //return RedirectToAction("Index");
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckAssignedProjects(int id)
        {
            var projectList = new UserDAL().CheckAssignedProjects(id);


            var jsonResult = Json(projectList, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;

        }


        List<UsersModel> list = new List<UsersModel>();
        string role;
        string RoleName;
        public ActionResult GetUserList()
        {
            try
            {
                long currentUserId = (long)new UserDetails().GetUserId(User.Identity.GetUserId());
                //var list1 = new UserDAL().GetUserRecords(currentUserId);
                var list1 = new UserDAL().GetUserRecordsOfProject(currentUserId);

                var manager = new ApplicationDbContext.UserManager();
                manager.UserValidator = new UserValidator<ApplicationUser>(manager) { AllowOnlyAlphanumericUserNames = false };

                CompassEntities et = new CompassEntities();

                foreach (var item in list1)
                {

                    var Roleresult = manager.FindById(item.aspnet_UserId.ToString());
                    List<string> roleId = Roleresult.Roles.Select(a => a.RoleId).ToList();

                    foreach (var item12 in roleId)
                    {
                        List<string> OldRoleName = et.AspNetRoles.Where(a => a.Id == item12).Select(b => b.Name).ToList();
                        RoleName += OldRoleName[0] + ",";
                    }
                    RoleName = RoleName.TrimEnd(',');
                    item.RoleID = RoleName;
                    RoleName = "";
                }

                list1 = list1.Select(o => new tblUser { UserName = o.UserName, FirstName = o.FirstName, RoleID = o.RoleID, Email = o.Email, MobileNo = o.MobileNo, UserID = o.UserID }).ToList();

                var jsonResult = Json(list1, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;

                //var jsonResult = JsonConvert.SerializeObject(list1, Formatting.None,
                //                new JsonSerializerSettings()
                //                {
                //                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                //                });
                // return Content(jsonResult, "application/json");
            }
            catch (Exception ex)
            {

                throw ex;

            }

        }



        [HttpGet]
        public ActionResult GetDeActiveRecords()
        {
            var list = new UserDAL().GetDeActiveRecords();


            foreach (var item in list)
            {
                CompassEntities et = new CompassEntities();
                item.RoleID = et.AspNetRoles.Where(o => o.Id == item.RoleID).Select(o => o.Name).FirstOrDefault();
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }




        public ActionResult GetRoleList()
        {
            List<UserRoleModel> list = new List<UserRoleModel>();

            try
            {
                list = new UserRoleDAL().GetAllRoles();

            }
            catch (Exception ex)
            {

                //throw ex;
                log.Info("User Controller  GetRoleList ERROR at " + DateTime.Now.ToString());
                log.Info("User Controller  GetRoleList ERROR  ex.Message - " + ex.Message);

                log.Info("User Controller  GetRoleList ERROR  ex.InnerException.Message - " + Convert.ToString(ex.InnerException.Message));


            }
            return Json(list, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult TmpListClear()
        {
            List<UsersModel> tempdata = TempData["UsersList"] as List<UsersModel>;
            TempData.Keep("UsersList");
            if (tempdata != null)
            {
                tempdata.Clear();
            }
            TempData.Keep("UsersList");
            return Json(tempdata, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult TmpListRemove(int id)
        {

            List<UsersModel> tempdata = TempData["UsersList"] as List<UsersModel>;
            if (tempdata != null)
            {
                if (id != 0)
                {

                    var item = tempdata.SingleOrDefault(x => x.UserId == id);
                    if (item != null)
                        tempdata.Remove(item);

                }
            }

            TempData["UsersList"] = tempdata;
            TempData.Keep("UsersList");
            if (tempdata == null)
            {
                tempdata = new List<UsersModel>();
            }
            return Json(tempdata, JsonRequestBehavior.AllowGet);
        }


        // 
        [HttpPost]
        //public ActionResult Create(UsersModel UserModel, string objroleList)
        public ActionResult Create1(UserViewModel UserViewModel)
        {
            try
            {
                int currentUserId = (int)new UserDetails().GetUserId(User.Identity.GetUserId());
                string returnMessage = "";
                bool success = false;
                string MSg = "";
                long UserId = 0;
                string aspuserid = "";

                if (UserViewModel.UserModel.UserId == 0)
                {
                    ModelState["UserModel.UserId"].Errors.Clear();
                }

                //if (ModelState.IsValid)
                //{
                if (UserViewModel.UserModel.UserId == 0)
                {
                    UserViewModel.UserModel.CreatedBy = null;
                    UserViewModel.UserModel.Active = 1;
                }
                else
                {
                    UserViewModel.UserModel.Active = 1;
                    UserViewModel.UserModel.ModifiedBy = null;
                    aspuserid = new UserDAL().gebyaspuserid(UserViewModel.UserModel.UserId);
                }
                if (UserViewModel.UserModel.UserId == 0)
                {
                    string Password = UserViewModel.UserModel.Confirmpassword;
                    var manager = new ApplicationDbContext.UserManager();
                    var user = new ApplicationUser() { UserName = UserViewModel.UserModel.LogInId, Email = UserViewModel.UserModel.Email, EmailConfirmed = false };
                    var uservalidator = manager.UserValidator as UserValidator<ApplicationUser>;

                    String pass1 = Guid.NewGuid().ToString();
                    IdentityResult result = manager.Create(user, Password);
                    if (result.Succeeded)
                    {

                        UsersModel usermaster = new UsersModel();
                        usermaster.aspnet_UserId = new Guid(user.Id);
                        // usermaster.aspnet_UserId =new Guid(roleManager.FindByName(UserModel.Role));
                        usermaster.UserName = UserViewModel.UserModel.LogInId;
                        usermaster.FirstName = UserViewModel.UserModel.UserFullName;
                        usermaster.Email = UserViewModel.UserModel.Email;
                        usermaster.MobileNo = UserViewModel.UserModel.MobileNo;
                        usermaster.Address = UserViewModel.UserModel.Address;
                        usermaster.IsFirstLogin = 1;
                        usermaster.Active = 1;
                        usermaster.Password = UserViewModel.UserModel.Confirmpassword;
                        usermaster.RoleID = UserViewModel.UserModel.PrimaryRole;
                        usermaster.PasswordExpiryDays = 15;

                        usermaster.StateID = UserViewModel.UserModel.StateID;
                        usermaster.CityID = UserViewModel.UserModel.CityID;
                        usermaster.ZipCode = UserViewModel.UserModel.ZipCode;
                        usermaster.Gender = UserViewModel.UserModel.Gender;
                        usermaster.HomePhoneNumber = UserViewModel.UserModel.HomePhoneNumber;


                        usermaster.InstallerImage = UserViewModel.UserModel.InstallerImage;

                        usermaster.ManagerId = UserViewModel.UserModel.ManagerId;
                        usermaster.FieldSupervisorId = UserViewModel.UserModel.FieldSupervisorId;
                        usermaster.ManagerApprovalRequired = UserViewModel.UserModel.ManagerApprovalRequired == null ? 2 : UserViewModel.UserModel.ManagerApprovalRequired;

                        


                        //Employment Details
                        usermaster.JobType = UserViewModel.UserModel.JobType;
                        usermaster.FormType = UserViewModel.UserModel.FormType;

                        //success = new UserDAL().CreateOrUpdate(usermaster, UserViewModel.ServiceRatesList,UserViewModel.VehicleModelData, out returnMessage);

                        success = new UserDAL().CreateOrUpdate(usermaster, UserViewModel, out returnMessage, ref UserId, currentUserId);

                        if (success == true)
                        {
                            #region Profile pic save

                            if (usermaster.InstallerImage != null && usermaster.InstallerImage != "" && usermaster.profilepicfolder != null && usermaster.profilepicfolder != "")
                            {
                                var sourcepath = "~/Uploads/UserProfile/Temp/" + usermaster.profilepicfolder;

                                var path = UserId;
                                var DestinationPath = "~/Uploads/UserProfile/" + path;
                                bool exists = System.IO.Directory.Exists(Server.MapPath(DestinationPath));

                                if (!exists)
                                    System.IO.Directory.CreateDirectory(Server.MapPath(DestinationPath));

                                string[] files = System.IO.Directory.GetFiles(Server.MapPath(sourcepath));

                                foreach (string file in files)
                                {
                                    System.IO.File.Copy(file, System.IO.Path.Combine(Server.MapPath(DestinationPath), System.IO.Path.GetFileName(file)));
                                }

                                CompassEntities objTollFreeWidgetEntities = new CompassEntities();
                                tblUser objChk = (from b in objTollFreeWidgetEntities.tblUsers where b.UserID == UserId && b.Active == 1 select b).FirstOrDefault();
                                if (objChk != null)
                                {
                                    objChk.InstallerImage = "Uploads/UserProfile/" + path + "/" + usermaster.InstallerImage;
                                    objTollFreeWidgetEntities.SaveChanges();
                                }

                            }



                            #endregion


                            #region Vehicle pic save

                            if (UserViewModel.vehiclepicfolder != null && UserViewModel.vehiclepicfolder != "" && UserViewModel.VehicleModelData.ImageofVehicle != null && UserViewModel.VehicleModelData.ImageofVehicle != "")
                            {
                                var sourcepath = "~/Uploads/Vehicle/Temp/" + UserViewModel.vehiclepicfolder;

                                var path = UserId;
                                var DestinationPath = "~/Uploads/Vehicle/" + path;
                                bool exists = System.IO.Directory.Exists(Server.MapPath(DestinationPath));

                                if (!exists)
                                    System.IO.Directory.CreateDirectory(Server.MapPath(DestinationPath));

                                string[] files = System.IO.Directory.GetFiles(Server.MapPath(sourcepath));

                                foreach (string file in files)
                                {
                                    System.IO.File.Copy(file, System.IO.Path.Combine(Server.MapPath(DestinationPath), System.IO.Path.GetFileName(file)));
                                }

                                CompassEntities objTollFreeWidgetEntities = new CompassEntities();


                                tblVehicleMaster VehicleMaster = (from b in objTollFreeWidgetEntities.tblVehicleMasters
                                                                  where b.Active == 1 && b.UserId == UserId
                                                                  select b).FirstOrDefault();
                                if (VehicleMaster != null)
                                {
                                    VehicleMaster.ImageofVehicle = "Uploads/Vehicle/" + path + "/" + UserViewModel.VehicleModelData.ImageofVehicle;
                                    objTollFreeWidgetEntities.SaveChanges();
                                }

                            }



                            #endregion
                        }

                        //List<string> roles = UserViewModel.objroleList.Split(',').ToList<string>();
                        List<string> roles = UserViewModel.objroleList;
                        foreach (var item in roles)
                        {
                            CompassEntities et = new CompassEntities();
                            role = et.AspNetRoles.Where(o => o.Id == item).Select(o => o.Name).FirstOrDefault();
                            var Roleresult = manager.AddToRole(user.Id, role);
                        }

                        new UserDAL().AddUserPermission(UserId, currentUserId);

                    }
                    else
                    {
                        success = false;
                        returnMessage = "Login Id already exist";
                        return Json(new { success = success, returnMessage = returnMessage }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    CompassEntities objScoreMyTeamsEntities = new CompassEntities();
                    UsersModel usermaster = new UsersModel();
                    var manager = new ApplicationDbContext.UserManager();

                    Guid g = new Guid(aspuserid);




                    var ExistingRoleresult = manager.FindById(g.ToString());
                    if (ExistingRoleresult != null)
                    {
                        List<string> roleIds = ExistingRoleresult.Roles.Select(a => a.RoleId).ToList();
                        if (roleIds != null)
                        {

                            foreach (var roleId in roleIds)
                            {

                                List<string> OldRoleName = objScoreMyTeamsEntities.AspNetRoles.Where(a => a.Id == roleId).Select(b => b.Name).ToList();
                                if (OldRoleName.Contains("Installer") == true)
                                {
                                    var projectAssigned = objScoreMyTeamsEntities.PROC_CheckProjectsAssignedToUser(roleId, UserViewModel.UserModel.UserId).FirstOrDefault();
                                    if (projectAssigned != null)
                                    {

                                        var installerRoleId = roleId;
                                        var installerIsExist = UserViewModel.objroleList.Where(a => a == roleId).FirstOrDefault();
                                        if (installerIsExist == null)
                                        {
                                            success = false;
                                            returnMessage = "Please first remove the user from the installer project";
                                            return Json(new { success = success, returnMessage = returnMessage }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                }
                                if (OldRoleName.Contains("Auditor") == true)
                                {



                                    var projectAssigned = objScoreMyTeamsEntities.PROC_CheckAuditProjectsAssignedToUser(UserViewModel.UserModel.UserId).FirstOrDefault();
                                    if (projectAssigned != null)
                                    {

                                        var installerRoleId = roleId;
                                        var installerIsExist = UserViewModel.objroleList.Where(a => a == roleId).FirstOrDefault();
                                        if (installerIsExist == null)
                                        {
                                            success = false;
                                            returnMessage = "Please first remove the user from the auditor project";
                                            return Json(new { success = success, returnMessage = returnMessage }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                }


                            }



                        }
                    }




                    UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
                    //2019-02-15 below is code for change password commented by bharat as it not working
                    //userManager.RemovePassword(aspuserid);
                    //userManager.AddPassword(aspuserid, UserViewModel.UserModel.Confirmpassword);
                    var user = userManager.FindById(aspuserid);

                    //2019-02-15 below  code for change password
                    var user1 = manager.FindById(user.Id);
                    UserStore<ApplicationUser> store = new UserStore<ApplicationUser>();
                    store.SetPasswordHashAsync(user1, manager.PasswordHasher.HashPassword(UserViewModel.UserModel.Confirmpassword));


                   
                    //user.UserName = UserViewModel.UserModel.LogInId;
                    //user.Email = UserViewModel.UserModel.Email;
                    //userManager.Update(user);

                    if (user.UserName != UserViewModel.UserModel.LogInId)
                    {
                        var aspnetUser = objScoreMyTeamsEntities.AspNetUsers.Where(o => o.UserName == UserViewModel.UserModel.LogInId).FirstOrDefault();
                        if (aspnetUser != null)
                        {
                            success = false;
                            returnMessage = "Update failed. New user name already in use.....";
                            return Json(new { success = success, returnMessage = returnMessage }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            //aspnetUser = objScoreMyTeamsEntities.AspNetUsers.Where(o => o.UserName == user.UserName).FirstOrDefault();
                            //aspnetUser.UserName = UserViewModel.UserModel.LogInId;
                            //objScoreMyTeamsEntities.AspNetUsers.
                            //objScoreMyTeamsEntities.SaveChanges();

                            //using (var context = new CompassEntities())
                            //{
                            //    // Get the membership record from the database
                            //    var currentUserNameLowered = context.AspNetUsers.Where(o => o.UserName == user.UserName).FirstOrDefault();
                            //    currentUserNameLowered.UserName = UserViewModel.UserModel.LogInId;
                            //    context.SaveChanges();


                            //}

                            string oldUsername = user.UserName;
                            string newUsername = UserViewModel.UserModel.LogInId;

                            using (SqlConnection connection = new SqlConnection(objScoreMyTeamsEntities.Database.Connection.ConnectionString))
                            {
                                connection.Open();

                                using (SqlCommand command = connection.CreateCommand())
                                {
                                    command.CommandText = "UPDATE AspNetUsers SET UserName=@NewUsername WHERE UserName=@OldUsername";

                                    SqlParameter parameter = new SqlParameter("@OldUsername", SqlDbType.VarChar,50);
                                    parameter.Value = oldUsername;
                                    command.Parameters.Add(parameter);

                                    parameter = new SqlParameter("@NewUsername", SqlDbType.VarChar, 50);
                                    parameter.Value = newUsername;
                                    command.Parameters.Add(parameter);
                                    int i=command.ExecuteNonQuery();
                                }
                            }
                        }

                    }

                    if (user.Email != UserViewModel.UserModel.Email)
                    {
                        var aspnetUser = objScoreMyTeamsEntities.AspNetUsers.Where(o => o.Email == UserViewModel.UserModel.Email).FirstOrDefault();
                        if (aspnetUser != null)
                        {
                            success = false;
                            returnMessage = "Update failed. New email already in use.....";
                            return Json(new { success = success, returnMessage = returnMessage }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            aspnetUser = objScoreMyTeamsEntities.AspNetUsers.Where(o => o.UserName == user.UserName).FirstOrDefault();
                            aspnetUser.Email = UserViewModel.UserModel.Email;
                            objScoreMyTeamsEntities.SaveChanges();
                        }

                    }


                    usermaster.aspnet_UserId = g;
                    usermaster.UserId = UserViewModel.UserModel.UserId;
                    usermaster.UserName = UserViewModel.UserModel.LogInId;
                    usermaster.FirstName = UserViewModel.UserModel.UserFullName;
                    usermaster.Email = UserViewModel.UserModel.Email;
                    usermaster.MobileNo = UserViewModel.UserModel.MobileNo;
                    usermaster.Address = UserViewModel.UserModel.Address;
                    usermaster.IsFirstLogin = 1;
                    usermaster.Active = 1;
                    usermaster.Password = UserViewModel.UserModel.Confirmpassword;
                    usermaster.RoleID = UserViewModel.UserModel.PrimaryRole;
                    usermaster.PasswordExpiryDays = 15;


                    usermaster.StateID = UserViewModel.UserModel.StateID;
                    usermaster.CityID = UserViewModel.UserModel.CityID;
                    usermaster.ZipCode = UserViewModel.UserModel.ZipCode;
                    usermaster.Gender = UserViewModel.UserModel.Gender;
                    usermaster.HomePhoneNumber = UserViewModel.UserModel.HomePhoneNumber;
                    usermaster.InstallerImage = UserViewModel.UserModel.InstallerImage;
                    //usermaster.profilepicfolder = UserViewModel.profilepicfolder;   //commented by sominath
                    //vehicle
                    //usermaster.VehicleMake = UserViewModel.VehicleModelData.VehicleMake;
                    //usermaster.VehicleModel = UserViewModel.VehicleModelData.VehicleModel;
                    //usermaster.VehicleYear = UserViewModel.VehicleModelData.VehicleYear;
                    //usermaster.VehicleLicensePlateNumber = UserViewModel.VehicleModelData.VehicleLicensePlateNumber;
                    //usermaster.Vehicle = UserViewModel.VehicleModelData.Vehicle;
                    //usermaster.VehicleColor = UserViewModel.VehicleModelData.VehicleColor;
                    //usermaster.ImageofVehicle = UserViewModel.VehicleModelData.ImageofVehicle;

                    //Employment Details
                    usermaster.JobType = UserViewModel.UserModel.JobType;

                    usermaster.FormType = UserViewModel.UserModel.FormType;



                    usermaster.ManagerId = UserViewModel.UserModel.ManagerId;
                    usermaster.FieldSupervisorId = UserViewModel.UserModel.FieldSupervisorId;
                    usermaster.ManagerApprovalRequired = UserViewModel.UserModel.ManagerApprovalRequired == null ? 2 : UserViewModel.UserModel.ManagerApprovalRequired;


                    //success = new UserDAL().CreateOrUpdate(usermaster, UserViewModel.ServiceRatesList, UserViewModel.VehicleModelData, out returnMessage);
                    success = new UserDAL().CreateOrUpdate(usermaster, UserViewModel, out returnMessage, ref UserId, currentUserId);

                    if (success == true)
                    {
                        #region Profile pic save

                        if (usermaster.InstallerImage != null && usermaster.InstallerImage != "" && usermaster.profilepicfolder != null && usermaster.profilepicfolder != "")
                        {
                            var sourcepath = "~/Uploads/UserProfile/Temp/" + usermaster.profilepicfolder;

                            var path = UserId;
                            var DestinationPath = "~/Uploads/UserProfile/" + path;
                            bool exists = System.IO.Directory.Exists(Server.MapPath(DestinationPath));

                            if (!exists)
                                System.IO.Directory.CreateDirectory(Server.MapPath(DestinationPath));

                            string[] files = System.IO.Directory.GetFiles(Server.MapPath(sourcepath));

                            foreach (string file in files)
                            {
                                System.IO.File.Copy(file, System.IO.Path.Combine(Server.MapPath(DestinationPath), System.IO.Path.GetFileName(file)), true);
                            }

                            CompassEntities objTollFreeWidgetEntities = new CompassEntities();
                            tblUser objChk = (from b in objTollFreeWidgetEntities.tblUsers where b.UserID == UserId && b.Active == 1 select b).FirstOrDefault();
                            if (objChk != null)
                            {
                                objChk.InstallerImage = "Uploads/UserProfile/" + path + "/" + usermaster.InstallerImage;
                                objTollFreeWidgetEntities.SaveChanges();
                            }

                        }


                        #endregion

                        #region Vehicle pic save

                        if (UserViewModel.vehiclepicfolder != null && UserViewModel.vehiclepicfolder != "" && UserViewModel.VehicleModelData.ImageofVehicle != null && UserViewModel.VehicleModelData.ImageofVehicle != "")
                        {
                            var sourcepath = "~/Uploads/Vehicle/Temp/" + UserViewModel.vehiclepicfolder;

                            var path = UserId;
                            var DestinationPath = "~/Uploads/Vehicle/" + path;
                            bool exists = System.IO.Directory.Exists(Server.MapPath(DestinationPath));

                            if (!exists)
                                System.IO.Directory.CreateDirectory(Server.MapPath(DestinationPath));

                            string[] files = System.IO.Directory.GetFiles(Server.MapPath(sourcepath));

                            foreach (string file in files)
                            {
                                System.IO.File.Copy(file, System.IO.Path.Combine(Server.MapPath(DestinationPath), System.IO.Path.GetFileName(file)), true);

                            }

                            CompassEntities objTollFreeWidgetEntities = new CompassEntities();
                            tblVehicleMaster VehicleMaster = (from b in objTollFreeWidgetEntities.tblVehicleMasters
                                                              where b.Active == 1 && b.UserId == UserId
                                                              select b).FirstOrDefault();
                            if (VehicleMaster != null)
                            {
                                VehicleMaster.ImageofVehicle = "Uploads/Vehicle/" + path + "/" + UserViewModel.VehicleModelData.ImageofVehicle;
                                objTollFreeWidgetEntities.SaveChanges();
                            }

                        }



                        #endregion
                    }
                    //List<string> roles = UserViewModel.objroleList.Split(',').ToList<string>();
                    List<string> roles = UserViewModel.objroleList;
                    if (success == true)
                    {

                        //var user = objScoreMyTeamsEntities.tblUserMasters.Where(u => u.UserName.Equals(UserModel.EmailId)).FirstOrDefault();
                        // var user = new ApplicationUser() { UserName = UserModel.EmailId, IsEmailConfirmed = 0 };
                        //add user Role

                        manager.UserValidator = new UserValidator<ApplicationUser>(manager) { AllowOnlyAlphanumericUserNames = false };

                        var Roleresult = manager.FindById(g.ToString());
                        if (Roleresult != null)
                        {
                            List<string> roleId = Roleresult.Roles.Select(a => a.RoleId).ToList();
                            if (roleId != null)
                            {
                                foreach (var item in roleId)
                                {
                                    List<string> OldRoleName = objScoreMyTeamsEntities.AspNetRoles.Where(a => a.Id == item).Select(b => b.Name).ToList();

                                    foreach (var item2 in OldRoleName)
                                    {
                                        if (item2 != null && item2 != UserViewModel.UserModel.RoleID)
                                        {
                                            var r1 = manager.RemoveFromRole(g.ToString().ToLower(), item2);
                                        }
                                    }

                                }

                                //List<string> roles1 = UserViewModel.objroleList.Split(',').ToList<string>();
                                List<string> roles1 = UserViewModel.objroleList;
                                foreach (var item1 in roles1)
                                {
                                    CompassEntities et1 = new CompassEntities();
                                    role = et1.AspNetRoles.Where(o => o.Id == item1).Select(o => o.Name).FirstOrDefault();
                                    var r2 = manager.AddToRole(g.ToString().ToLower(), role);
                                }
                            }

                            new UserDAL().UpdateUserPermission(UserId, currentUserId);
                        }

                    }
                }
                return Json(new { success = success, returnMessage = returnMessage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return View();
            }
        }


        public ActionResult Delete(int id)
        {
            return View();
        }
        bool success;

        [HttpPost]
        public ActionResult Activate(string id, string msg)
        {
            string returnMessage = "";
            try
            {
                if (msg == "Activate")
                {
                    var UserModel = new UserDAL().GetUserById(Convert.ToInt64(id));
                    UserModel.Active = 1;

                    success = new UserDAL().ActivateRecord(Convert.ToInt32(id), out returnMessage);
                    if (success)
                    {
                        returnMessage = CommonFunctions.strRecordActivated;
                    }
                    else
                    {
                        returnMessage = CommonFunctions.strRecordActivatingError;
                    }
                }
                else
                {
                    //CompassEntities cmp = new CompassEntities();
                    //long usrid = Convert.ToInt64(id);
                    //var userinproject = cmp.TblProjectInstallers.Where(o => o.tblUsers_UserID == usrid && o.Active == 1).FirstOrDefault();
                    //if (userinproject == null)
                    //{
                    //    UsersModel userModel = new UsersModel();
                    //    userModel = new UserDAL().GetUserById(Convert.ToInt64(id));
                    //    userModel.Active = 0;
                    //    long TerritoryID = 0;
                    //    UserViewModel UserViewModel = new Model.BO.UserViewModel();
                    //    //success = false;


                    //    // success = new CompanyDAL().ActivateRecord(Convert.ToInt32(model.PlanID), out returnMessage);
                    //  //  success = new UserDAL().CreateOrUpdate(userModel, UserViewModel.ServiceRatesList, UserViewModel.VehicleModelData, out returnMessage);
                    //    success = new UserDAL().CreateOrUpdate(userModel, UserViewModel, out returnMessage);
                    //    if (success)
                    //    {
                    //        returnMessage = CommonFunctions.strRecordDeactivated;
                    //    }
                    //    else
                    //    {
                    //        returnMessage = returnMessage;
                    //    }

                    //}
                    //else
                    //{
                    //    success = false;
                    //    returnMessage = "User is used in Active Project";
                    //}
                }

                return Json(new { success = success, returnMessage = returnMessage }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Deactivate(string id, string msg)
        {
            var objPermissionCheck = UserPermissionCheck.checkUserStatus("Users", "Delete");

            if (objPermissionCheck)
            {
                string returnMessage = "";
                try
                {



                    if (msg == "Deactivate")
                    {
                        //var UserModel = new UserDAL().GetUserById(Convert.ToInt64(id));
                        //UserModel.Active = 0;

                        success = new UserDAL().DeActivateRecord(Convert.ToInt32(id), out returnMessage);
                        if (success)
                        {
                            returnMessage = CommonFunctions.strRecordDeactivated;
                        }
                        //else
                        //{
                        //    returnMessage = returnMessage;// CommonFunctions.strRecordDeactivatingError;
                        //}

                    }
                    else
                    {
                        //CompassEntities cmp = new CompassEntities();
                        //long usrid = Convert.ToInt64(id);
                        //var userinproject = cmp.TblProjectInstallers.Where(o => o.tblUsers_UserID == usrid && o.Active == 1).FirstOrDefault();
                        //if (userinproject == null)
                        //{
                        //    UsersModel userModel = new UsersModel();
                        //    userModel = new UserDAL().GetUserById(Convert.ToInt64(id));
                        //    userModel.Active = 0;
                        //    long TerritoryID = 0;
                        //    UserViewModel UserViewModel = new Model.BO.UserViewModel();
                        //    //success = false;


                        //    // success = new CompanyDAL().ActivateRecord(Convert.ToInt32(model.PlanID), out returnMessage);
                        //    //  success = new UserDAL().CreateOrUpdate(userModel, UserViewModel.ServiceRatesList, UserViewModel.VehicleModelData, out returnMessage);
                        //    success = new UserDAL().CreateOrUpdate(userModel, UserViewModel, out returnMessage);
                        //    if (success)
                        //    {
                        //        returnMessage = CommonFunctions.strRecordDeactivated;
                        //    }
                        //    else
                        //    {
                        //        returnMessage = returnMessage;
                        //    }

                        //}
                        //else
                        //{
                        //    success = false;
                        //    returnMessage = "User is used in Active Project";
                        //}
                    }

                    return Json(new { success = success, returnMessage = returnMessage }, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                var strMessage = objPermissionCheck == false ? UserPermissionCheck.PerMessage : "";
                return Json(new { success = objPermissionCheck, returnMessage = strMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Edit(int id)
        {
            //id1 = id;
            DateTime dt = new CommonFunctions().ServerDate();
            UsersModel modelData = new UsersModel();
            modelData = new UserDAL().GetUserById(id);

            modelData.InstallerImage = !string.IsNullOrEmpty(modelData.InstallerImage) ? modelData.InstallerImage : "";

            var vehicleData = new VehicleDAL().GetVehicleById(id);
            if (vehicleData != null)
            {
                modelData.VehicleMake = vehicleData.MakeID;
                modelData.VehicleModel = vehicleData.ModelID;
                modelData.VehicleColor = vehicleData.Color;
                modelData.VehicleLicensePlateNumber = vehicleData.LicensePlate;
                modelData.VehicleYear = vehicleData.Year;
                modelData.ImageofVehicle = vehicleData.ImageofVehicle;
            }

            List<UsersServiceModel> ServiceRatesList = new List<UsersServiceModel>();
            ServiceRatesList = new UserDAL().UsersServiceModelList(id);
            var list11 = new List<UserRoleModel>();
            List<UserRoleModel> ModelCD = new List<UserRoleModel>();

            var list1 = new UserDAL().GetUserByID(id);

            var manager = new ApplicationDbContext.UserManager();
            manager.UserValidator = new UserValidator<ApplicationUser>(manager) { AllowOnlyAlphanumericUserNames = false };

            foreach (var item in list1)
            {
                CompassEntities et = new CompassEntities();
                var Roleresult = manager.FindById(item.aspnet_UserId.ToString());
                List<string> roleId = Roleresult.Roles.Select(a => a.RoleId).ToList();

                foreach (var item12 in roleId)
                {
                    var rols = et.AspNetRoles.Where(a => a.Id == item12).Select(b => b.Name).ToList();
                    UserRoleModel myobj = new UserRoleModel();
                    myobj.Id = item12;
                    myobj.Name = rols[0];
                    list11.Add(myobj);
                }
            }

            ModelCD = list11;


            return Json(new { modelData = modelData, ModelCD = ModelCD, ServiceRatesList = ServiceRatesList }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetFormTypeList()
        {
            List<FormTypeModel> FormTypeModelList = new FormDAL().GetFormTypeList();

            return Json(FormTypeModelList, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetUserFormTypeList(long UserID)
        {
            List<UserFormTypeModel> FormTypeModelList = new UserDAL().GetUserFormTypeId(UserID);

            return Json(FormTypeModelList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Logoupload()
        {

            var UserId = Request.Form["UserId"];
            bool result = false;
            string strfilepath = "";
            // var currentDatetime = new CommonFunctions().ServerDate().ToString("Mddyyhhmmss");
            var profilepath = "~/Uploads/UserProfile/" + Convert.ToString(UserId);

            try
            {
                if (Directory.Exists(Server.MapPath(profilepath)))
                {
                    List<string> sPathList = new List<string>();

                    foreach (var fi in new DirectoryInfo(Server.MapPath(profilepath)).GetFiles())
                    {
                        sPathList.Add(fi.FullName);
                    }

                    foreach (var item in sPathList)
                    {
                        System.IO.File.Delete(item);
                    }
                }

                bool exists = System.IO.Directory.Exists(Server.MapPath(profilepath));

                if (!exists)
                    System.IO.Directory.CreateDirectory(Server.MapPath(profilepath));


                log.Info("Profile image path " + profilepath);



                var file = Request.Files[0];
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath(profilepath), fileName);
                file.SaveAs(path);

                CompassEntities etCompassEntities = new CompassEntities();
                long UserId1 = long.Parse(UserId);
                var tblUserObj = etCompassEntities.tblUsers.Where(o => o.UserID == UserId1 && o.Active == 1).FirstOrDefault();
                strfilepath = "/Uploads/UserProfile/" + tblUserObj.UserID + "/" + fileName;
                tblUserObj.InstallerImage = strfilepath;                                                         //tblUserObj.InstallerImage = fileName;

                etCompassEntities.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                log.Info("User Controller  Logoupload ERROR at " + DateTime.Now.ToString());
                log.Info("User Controller  Logoupload ERROR  ex.Message - " + ex.Message);

                log.Info("User Controller  Logoupload ERROR  ex.InnerException.Message - " + Convert.ToString(ex.InnerException.Message));

            }




            return Json(new { Msg = result, strfilepath = strfilepath }, JsonRequestBehavior.AllowGet);

        }
        #region Remove profile Image
        [HttpGet]
        public ActionResult RemoveProfileImage(long userid)
        {
            bool success = false;
            string returnMsg = "We are unable to remove profile image.";
            try
            {

                var profilepath = "~/Uploads/UserProfile/" + Convert.ToString(userid);

                if (Directory.Exists(Server.MapPath(profilepath)))
                {
                    List<string> sPathList = new List<string>();

                    foreach (var fi in new DirectoryInfo(Server.MapPath(profilepath)).GetFiles())
                    {
                        sPathList.Add(fi.FullName);
                    }

                    foreach (var item in sPathList)
                    {
                        System.IO.File.Delete(item);
                    }
                }

                CompassEntities etCompassEntities = new CompassEntities();

                var tblUserObj = etCompassEntities.tblUsers.Where(o => o.UserID == userid && o.Active == 1).FirstOrDefault();
                tblUserObj.InstallerImage = "";
                etCompassEntities.SaveChanges();
                success = true;
                returnMsg = "Profile image deleted successfully.";
            }
            catch (Exception ex)
            {
                success = false;
                log.Info("User Controller  RemoveProfileImage ERROR at " + DateTime.Now.ToString());
                log.Info("User Controller  RemoveProfileImage ERROR  ex.Message - " + ex.Message);

                log.Info("User Controller  RemoveProfileImage ERROR  ex.InnerException.Message - " + Convert.ToString(ex.InnerException.Message));

            }

            return Json(new { msg = success, returnMsg = returnMsg }, JsonRequestBehavior.AllowGet);
        }


        #endregion


        [HttpPost]
        public ActionResult LogouploadVehicle()
        {
            var UserId = Request.Form["UserId"];
            bool result = false;
            string strfilepath = "";
            //var deletepath = "~/Uploads/Vehicle/";   //"~/Uploads/Vehicle/Temp";

            //foreach (var fi in new DirectoryInfo(Server.MapPath(deletepath)).GetDirectories().OrderByDescending(x => x.LastWriteTime).Skip(5))
            //    fi.Delete(true);

            //var currentDatetime = new CommonFunctions().ServerDate().ToString("Mddyyhhmmss");

            var Vehiclefilepath = "~/Uploads/Vehicle/" + Convert.ToString(UserId);
            try
            {
                if (Directory.Exists(Server.MapPath(Vehiclefilepath)))
                {
                    List<string> sPathList = new List<string>();

                    foreach (var fi in new DirectoryInfo(Server.MapPath(Vehiclefilepath)).GetFiles())
                    {
                        sPathList.Add(fi.FullName);
                    }

                    foreach (var item in sPathList)
                    {
                        System.IO.File.Delete(item);
                    }
                }

                bool exists = System.IO.Directory.Exists(Server.MapPath(Vehiclefilepath));

                if (!exists)
                    System.IO.Directory.CreateDirectory(Server.MapPath(Vehiclefilepath));

                log.Info("Vehicle image path " + Vehiclefilepath);


                var file = Request.Files[0];
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath(Vehiclefilepath), fileName);
                file.SaveAs(path);

                CompassEntities etCompassEntities = new CompassEntities();
                long UserId1 = long.Parse(UserId);
                var tblVehicleObj = etCompassEntities.tblVehicleMasters.Where(o => o.UserId == UserId1 && o.Active == 1).FirstOrDefault();
                strfilepath = "/Uploads/Vehicle/" + tblVehicleObj.UserId + "/" + fileName;
                tblVehicleObj.ImageofVehicle = strfilepath;
                etCompassEntities.SaveChanges();
                result = true;
                //return strfilepath;

            }
            catch (Exception ex)
            {
                result = false;
                log.Info("User Controller  LogouploadVehicle ERROR at " + DateTime.Now.ToString());
                log.Info("User Controller  LogouploadVehicle ERROR  ex.Message - " + ex.Message);

                log.Info("User Controller  LogouploadVehicle ERROR  ex.InnerException.Message - " + Convert.ToString(ex.InnerException.Message));

            }



            return Json(new { Msg = result, strfilepath = strfilepath }, JsonRequestBehavior.AllowGet);
        }


        #region Remove vehicle Image
        [HttpGet]
        public ActionResult RemoveVehicleImage(long userid)
        {
            bool success = false;
            string returnMsg = "We are unable to remove vehicle image.";
            try
            {

                var vehiclefilepath = "~/Uploads/Vehicle/" + Convert.ToString(userid);

                if (Directory.Exists(Server.MapPath(vehiclefilepath)))
                {
                    List<string> sPathList = new List<string>();

                    foreach (var fi in new DirectoryInfo(Server.MapPath(vehiclefilepath)).GetFiles())
                    {
                        sPathList.Add(fi.FullName);
                    }

                    foreach (var item in sPathList)
                    {
                        System.IO.File.Delete(item);
                    }
                }

                CompassEntities etCompassEntities = new CompassEntities();

                var tblVehicleObj = etCompassEntities.tblVehicleMasters.Where(o => o.UserId == userid && o.Active == 1).FirstOrDefault();

                tblVehicleObj.ImageofVehicle = "";
                etCompassEntities.SaveChanges();
                success = true;
                returnMsg = "Vehicle image deleted successfully.";
            }
            catch (Exception ex)
            {
                success = false;
                log.Info("User Controller  RemoveVehicleImage ERROR at " + DateTime.Now.ToString());
                log.Info("User Controller  RemoveVehicleImage ERROR  ex.Message - " + ex.Message);

                log.Info("User Controller  RemoveVehicleImage ERROR  ex.InnerException.Message - " + Convert.ToString(ex.InnerException.Message));

            }

            return Json(new { msg = success, returnMsg = returnMsg }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        public ActionResult GetVehicleMakeList()
        {
            List<VehicleMakeModel> list = new List<VehicleMakeModel>();
            try
            {
                list = new VehicleDAL().GetVehicleMakeList();
                //return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {


                log.Info("User Controller  GetVehicleMakeList ERROR at " + DateTime.Now.ToString());
                log.Info("User Controller  GetVehicleMakeList ERROR  ex.Message - " + ex.Message);

                log.Info("User Controller  GetVehicleMakeList ERROR  ex.InnerException.Message - " + Convert.ToString(ex.InnerException.Message));

            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVehicleModelList(long VehicleMakeID)
        {
            List<VehicleModelVM> list = new List<VehicleModelVM>();
            try
            {
                list = new VehicleDAL().GetVehicleModelList(VehicleMakeID);

            }
            catch (Exception ex)
            {

                // throw ex;
                log.Info("User Controller  GetVehicleModelList ERROR at " + DateTime.Now.ToString());
                log.Info("User Controller  GetVehicleModelList ERROR  ex.Message - " + ex.Message);

                log.Info("User Controller  GetVehicleModelList ERROR  ex.InnerException.Message - " + Convert.ToString(ex.InnerException.Message));

            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        #region Code to create ID Card added by sominath
        public ActionResult GetStateList(long customerId)
        {
            List<StateVM> objList1 = new List<StateVM>();
            UserDAL obj = new UserDAL();

            objList1 = obj.GetActiveStateRecords(customerId);


            return Json(objList1, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetCustomerList()
        {
            List<CustomerVM> objList1 = new List<CustomerVM>();
            UserDAL obj = new UserDAL();

            objList1 = obj.GetActiveCustomerRecords();


            return Json(objList1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCityListByID(long stateId, long customerId)
        {
            List<CityVM> objList1 = new List<CityVM>();
            UserDAL obj = new UserDAL();
            objList1 = obj.GetActiveCityRecords(stateId, customerId);


            return Json(objList1, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult GetImagenameListByCityID(long CityID)
        //{
        //    UserDAL obj = new UserDAL();
        //    List<CityImageNameList> _objImageNameList = new List<CityImageNameList>();
        //    _objImageNameList = obj.GetCityImageListByID(CityID);

        //    return Json(_objImageNameList, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetCityLogoNameByImageName(string imageName)
        //{
        //    string logoName = new UserDAL().GetLogoNameByImageName(imageName);
        //    return Json(new { LogoName = logoName }, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult GetClientLogoImagePath(long cityId, long customerId)
        {
            string ImageName = "";
            string logoName = new UserDAL().GetLogoNameByImageName(cityId, customerId, out ImageName);
            return Json(new { ImageName = ImageName, LogoName = logoName }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [ValidateInput(false)]
        public ActionResult SaveLogoMessage(long UserId, string RoleId, long CustomerId, long stateId, long cityId, string imageMessage)
        {
            var logoName = new UserDAL().saveLogoMessage(UserId, RoleId, CustomerId, stateId, cityId, imageMessage);

            var modelData = new UserDAL().GetUserById(UserId);
            string profileImage = modelData.InstallerImage;


            return Json(new { Status = logoName, profileImage = profileImage }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetLogoMessage(long UserId, string RoleId, long CustomerId, long stateId, long cityId)
        {
            string imageMessage = new UserDAL().GETLogoMessage(UserId, RoleId, CustomerId, stateId, cityId);


            return Json(new { imageMessage = imageMessage }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetManagerList(string RoleName)
        {

            CompassEntities objCompassEntities = new CompassEntities();
            // RoleName = "Manager";

            List<InventorySourceChildBO> objInventorySourceChildBOList = new List<InventorySourceChildBO>();
            var manager = new ApplicationDbContext.UserManager();

            var UseNames = manager.GetUsersInRole(RoleName);

            if (UseNames != null)
            {
                foreach (var item in UseNames)
                {
                    var record = objCompassEntities.tblUsers.Where(o => o.Active == 1 && o.aspnet_UserId.ToString() == item.Id.ToString()).
                        Select(o => new InventorySourceChildBO { ChildId = o.UserID, ChildName = o.FirstName }).FirstOrDefault();
                    if (record != null)
                    {
                        objInventorySourceChildBOList.Add(record);
                    }
                }
                objInventorySourceChildBOList = objInventorySourceChildBOList.OrderBy(o => o.ChildName).ToList();
            }

            objCompassEntities.Dispose();

            return Json(objInventorySourceChildBOList, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetSuperVisorList(long ManagerId)
        {

            CompassEntities objCompassEntities = new CompassEntities();
            // RoleName = "Manager";

            List<InventorySourceChildBO> objInventorySourceChildBOList = new List<InventorySourceChildBO>();
            var manager = new ApplicationDbContext.UserManager();

            var UseNames = manager.GetUsersInRole("Field Supervisor");

            if (UseNames != null)
            {
                foreach (var item in UseNames)
                {
                    var record = objCompassEntities.tblUsers.Where(o => o.Active == 1 && o.aspnet_UserId.ToString() == item.Id.ToString() && o.ManagerId == ManagerId).
                        Select(o => new InventorySourceChildBO { ChildId = o.UserID, ChildName = o.FirstName }).FirstOrDefault();
                    if (record != null)
                    {
                        objInventorySourceChildBOList.Add(record);
                    }
                }
                objInventorySourceChildBOList = objInventorySourceChildBOList.OrderBy(o => o.ChildName).ToList();
            }

            objCompassEntities.Dispose();

            return Json(objInventorySourceChildBOList, JsonRequestBehavior.AllowGet);
        }



        public ActionResult UserPermission(long UserId)
        {

            string userName = "";
            var obj = new UserDAL();
            var AspNetPermissionVM = obj.GetUserPermissionDetails(UserId, out userName);

            ViewBag.UserId = UserId;
            ViewBag.userName = userName;
            return View(AspNetPermissionVM);

        }



        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult AssignPermissionRole(List<UserAccessPermissionBo> PermissionList)
        {

            int currentUserId = (int)new UserDetails().GetUserId(User.Identity.GetUserId());

            if (PermissionList != null && PermissionList.Count > 0)
            {

                try
                {

                    var obj = new UserDAL();
                    var result = obj.AssignPermissionRole(currentUserId, PermissionList);

                    return Json(new { result = result, resultMessage = "Record saved successfully!" }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    return Json(new { result = false, resultMessage = "Save failed. Please try again!" }, JsonRequestBehavior.AllowGet);
                }


            }
            else
            {
                return Json(new { result = false, resultMessage = "Save failed. Please try again!" }, JsonRequestBehavior.AllowGet);
            }

        }





    }


}