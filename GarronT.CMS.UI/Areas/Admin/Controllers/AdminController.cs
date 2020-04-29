
using GarronT.CMS.Model;
using GarronT.CMS.Model.BO;
using GarronT.CMS.Model.DAL;
using GarronT.CMS.UI.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Routing;

namespace GarronT.CMS.UI.Areas.Admin.Controllers
{
    [RBAC]
    public class AdminController : Controller
    {
        //private RBAC_Model database = new RBAC_Model();

        CompassEntities database = new CompassEntities();

        #region USERS
        // GET: Admin
        public ActionResult Index()
        {
            //return View(database.USERS.Where(r => r.Inactive == false || r.Inactive == null).OrderBy(r => r.Lastname).ThenBy(r => r.Firstname).ToList());
            return View(database.AspNetUsers.OrderBy(r => r.UserName).ToList());
        }

        public ViewResult UserDetails(string id)
        {
            AspNetUser user = database.AspNetUsers.Find(id);
            SetViewBagData(id);
            return View(user);
        }

        public ActionResult UserCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserCreate(AspNetUser user)
        {
            if (user.UserName == "" || user.UserName == null)
            {
                ModelState.AddModelError(string.Empty, "Username cannot be blank");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    List<string> results = database.Database.SqlQuery<String>(string.Format("SELECT Username FROM AspNetUser WHERE UserName = '{0}'", user.UserName)).ToList();
                    bool _userExistsInTable = (results.Count > 0);

                    AspNetUser _user = null;
                    if (_userExistsInTable)
                    {
                        _user = database.AspNetUsers.Where(p => p.UserName == user.UserName).FirstOrDefault();
                        if (_user != null)
                        {
                            //if (_user.Inactive == false)
                            //{
                            //    ModelState.AddModelError(string.Empty, "USER already exists!");
                            //}
                            //else
                            //{
                            //database.Entry(_user).Entity.Inactive = false;
                            //database.Entry(_user).Entity.LastModified = System.DateTime.Now;
                            database.Entry(_user).State = EntityState.Modified;
                            database.SaveChanges();
                            return RedirectToAction("Index");
                            //}
                        }
                    }
                    else
                    {
                        _user = new AspNetUser();
                        _user.UserName = user.UserName;
                        //_user.Lastname = user.Lastname;
                        //_user.Firstname = user.Firstname;
                        //_user.Title = user.Title;
                        //_user.Initial = user.Initial;
                        //_user.EMail = user.EMail;

                        if (ModelState.IsValid)
                        {
                            //_user.Inactive = false;
                            //_user.LastModified = System.DateTime.Now;

                            database.AspNetUsers.Add(_user);
                            database.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //return base.ShowError(ex);
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult UserEdit(string id)
        {
            AspNetUser user = database.AspNetUsers.Find(id);
            SetViewBagData(id);
            return View(user);
        }

        [HttpPost]
        public ActionResult UserEdit(AspNetUser user)
        {
            AspNetUser _user = database.AspNetUsers.Where(p => p.Id == user.Id).FirstOrDefault();
            if (_user != null)
            {
                try
                {
                    database.Entry(_user).CurrentValues.SetValues(user);
                    //database.Entry(_user).Entity.LastModified = System.DateTime.Now;
                    database.SaveChanges();
                }
                catch (Exception)
                {

                }
            }
            return RedirectToAction("UserDetails", new RouteValueDictionary(new { id = user.Id }));
        }

        [HttpPost]
        public ActionResult UserDetails(AspNetUser user)
        {
            if (user.UserName == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid USER Name");
            }

            if (ModelState.IsValid)
            {
                //database.Entry(user).Entity.Inactive = user.Inactive;
                //database.Entry(user).Entity.LastModified = System.DateTime.Now;
                database.Entry(user).State = EntityState.Modified;
                database.SaveChanges();
            }
            return View(user);
        }

        [HttpGet]
        public ActionResult DeleteUserRole(int id, int userId)
        {
            AspNetRole role = database.AspNetRoles.Find(id);
            AspNetUser user = database.AspNetUsers.Find(userId);

            if (role.AspNetUsers.Contains(user))
            {
                role.AspNetUsers.Remove(user);
                database.SaveChanges();
            }
            return RedirectToAction("Details", "USER", new { id = userId });
        }

        [HttpGet]
        public PartialViewResult filter4Users(string _surname)
        {
            return PartialView("_ListUserTable", GetFilteredUserList(_surname));
        }

        [HttpGet]
        public PartialViewResult filterReset()
        {
            return PartialView("_ListUserTable", database.AspNetUsers.ToList());
        }

        [HttpGet]
        public PartialViewResult DeleteUserReturnPartialView(int userId)
        {
            try
            {
                AspNetUser user = database.AspNetUsers.Find(userId);
                if (user != null)
                {
                    //database.Entry(user).Entity.Inactive = true;
                    database.Entry(user).Entity.Id = user.Id;
                    // database.Entry(user).Entity.LastModified = System.DateTime.Now;
                    database.Entry(user).State = EntityState.Modified;
                    database.SaveChanges();
                }
            }
            catch
            {
            }
            return this.filterReset();
        }

        private IEnumerable<AspNetUser> GetFilteredUserList(string _surname)
        {
            IEnumerable<AspNetUser> _ret = null;
            try
            {
                if (string.IsNullOrEmpty(_surname))
                {
                    _ret = database.AspNetUsers.ToList();
                }
                else
                {
                    _ret = database.AspNetUsers.Where(p => p.UserName == _surname).ToList();
                }
            }
            catch
            {
            }
            return _ret;
        }

        //protected override void Dispose(bool disposing)
        //{
        //    database.Dispose();
        //    base.Dispose(disposing);
        //}

        //[HttpGet]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        //public PartialViewResult DeleteUserRoleReturnPartialView(int id, int userId)
        //{
        //    ROLE role = database.ROLES.Find(id);
        //    USER user = database.USERS.Find(userId);

        //    if (role.USERS.Contains(user))
        //    {
        //        role.USERS.Remove(user);
        //        database.SaveChanges();
        //    }
        //    SetViewBagData(userId);
        //    return PartialView("_ListUserRoleTable", database.USERS.Find(userId));
        //}

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddUserRoleReturnPartialView(string id, string userId)
        {
            AspNetRole role = database.AspNetRoles.Find(id);
            AspNetUser user = database.AspNetUsers.Find(userId);

            if (!role.AspNetUsers.Contains(user))
            {
                role.AspNetUsers.Add(user);
                database.SaveChanges();
            }
            SetViewBagData(userId);
            return PartialView("_ListUserRoleTable", database.AspNetUsers.Find(userId));
        }

        private void SetViewBagData(string _userId)
        {
            ViewBag.UserId = _userId;
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            ViewBag.RoleId = new SelectList(database.AspNetRoles.OrderBy(p => p.Name), "ID", "Name");
        }

        public List<SelectListItem> List_boolNullYesNo()
        {
            var _retVal = new List<SelectListItem>();
            try
            {
                _retVal.Add(new SelectListItem { Text = "Not Set", Value = null });
                _retVal.Add(new SelectListItem { Text = "Yes", Value = bool.TrueString });
                _retVal.Add(new SelectListItem { Text = "No", Value = bool.FalseString });
            }
            catch { }
            return _retVal;
        }
        //#endregion

        //#region ROLES
        [HttpGet]
        public ActionResult RoleIndex()
        {
            return View(database.AspNetRoles.OrderBy(r => r.Name).ToList());
        }

        public ViewResult RoleDetails(string id)
        {
            AspNetUser user = database.AspNetUsers.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();
            AspNetRole role = database.AspNetRoles.Where(r => r.Id == id)
                   .Include(a => a.AspNetRolePermissions)
                   .Include(a => a.AspNetUsers)
                   .FirstOrDefault();

            // USERS combo
            ViewBag.UserId = new SelectList(database.AspNetUsers, "Id", "UserName");
            ViewBag.RoleId = id;

            // Rights combo
            ViewBag.PermissionId = new SelectList(database.AspNetPermissions.OrderBy(a => a.PermissionDescription), "Permission_Id", "PermissionDescription");
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();

            return View(role);
        }

        public ActionResult RoleCreate()
        {
            AspNetUser user = database.AspNetUsers.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            return View();
        }

        [HttpPost]
        //public ActionResult RoleCreate(AspNetRole _role)
        //{
        //    if (_role.Name == null)
        //    {
        //        ModelState.AddModelError("Role Name", "Role Name must be entered");
        //    }

        //    AspNetUser user = database.AspNetUsers.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();
        //    if (ModelState.IsValid)
        //    {


        //        database.AspNetRoles.Add(_role);
        //        database.SaveChanges();
        //        return RedirectToAction("RoleIndex");
        //    }
        //    ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
        //    return View(_role);
        //}

        public ActionResult RoleCreate(IdentityRole Role)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            context.Roles.Add(Role);
            context.SaveChanges();
            return RedirectToAction("RoleIndex");
        }


        public ActionResult RoleEdit(string id)
        {
            AspNetUser user = database.AspNetUsers.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();

            AspNetRole _role = database.AspNetRoles.Where(r => r.Id == id)
                    .Include(a => a.AspNetRolePermissions)
                    .Include(a => a.AspNetUsers)
                    .FirstOrDefault();

            // USERS combo
            ViewBag.UserId = new SelectList(database.AspNetUsers, "Id", "UserName");
            ViewBag.RoleId = id;

            // Rights combo
            ViewBag.PermissionId = new SelectList(database.AspNetPermissions.OrderBy(a => a.PermissionDescription), "Permission_Id", "PermissionDescription");
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();

            return View(_role);
        }

        [HttpPost]
        public ActionResult RoleEdit(AspNetRole _role)
        {
            if (string.IsNullOrEmpty(_role.Name))
            {
                ModelState.AddModelError("Role Description", "Role Description must be entered");
            }

            //EntityState state = database.Entry(_role).State;
            AspNetUser user = database.AspNetUsers.Where(r => r.UserName == User.Identity.Name).FirstOrDefault();
            if (ModelState.IsValid)
            {

                database.Entry(_role).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("RoleDetails", new RouteValueDictionary(new { id = _role.Id }));
            }
            // USERS combo
            ViewBag.UserId = new SelectList(database.AspNetUsers, "Id", "UserName");

            // Rights combo
            ViewBag.PermissionId = new SelectList(database.AspNetPermissions.OrderBy(a => a.Permission_Id), "Permission_Id", "PermissionDescription");
            ViewBag.List_boolNullYesNo = this.List_boolNullYesNo();
            return View(_role);
        }


        public ActionResult RoleDelete(string id)
        {
            AspNetRole _role = database.AspNetRoles.Find(id);
            if (_role != null)
            {

                _role.AspNetUsers.Clear();

                List<AspNetRolePermission> _rolePermissions = database.AspNetRolePermissions.Where(a => a.Role_Id == _role.Id).ToList();
                foreach (var item in _rolePermissions)
                {
                    database.AspNetRolePermissions.Remove(item);
                }

                //  _role.AspNetRolePermissions.Clear();
                database.AspNetRoles.Remove(_role);
                // database.Entry(_role).State = EntityState.Deleted;
                database.SaveChanges();
            }
            return RedirectToAction("RoleIndex");
        }

        //[HttpGet]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        //public PartialViewResult DeleteUserFromRoleReturnPartialView(int id, int userId)
        //{
        //    ROLE role = database.ROLES.Find(id);
        //    USER user = database.USERS.Find(userId);

        //    if (role.USERS.Contains(user))
        //    {
        //        role.USERS.Remove(user);
        //        database.SaveChanges();
        //    }
        //    return PartialView("_ListUsersTable4Role", role);
        //}

        //[HttpGet]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        //public PartialViewResult AddUser2RoleReturnPartialView(int id, int userId)
        //{
        //    ROLE role = database.ROLES.Find(id);
        //    USER user = database.USERS.Find(userId);

        //    if (!role.USERS.Contains(user))
        //    {
        //        role.USERS.Add(user);
        //        database.SaveChanges();
        //    }
        //    return PartialView("_ListUsersTable4Role", role);
        //}

        //#endregion

        //#region PERMISSIONS

        public ViewResult PermissionIndex()
        {
            List<AspNetPermission> _permissions = database.AspNetPermissions
                               .OrderBy(wn => wn.PermissionDescription)
                               .Include(a => a.AspNetRolePermissions)
                               .ToList();
            return View(_permissions);
        }

        public ViewResult PermissionDetails(int id)
        {
            AspNetPermission _permission = database.AspNetPermissions.Find(id);
            return View(_permission);
        }

        public ActionResult PermissionCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PermissionCreate(AspNetPermission _permission)
        {
            if (_permission.PermissionDescription == null)
            {
                ModelState.AddModelError("Permission Description", "Permission Description must be entered");
            }

            if (ModelState.IsValid)
            {
                database.AspNetPermissions.Add(_permission);
                database.SaveChanges();
                return RedirectToAction("PermissionIndex");
            }
            return View(_permission);
        }

        public ActionResult PermissionEdit(int id)
        {
            AspNetPermission _permission = database.AspNetPermissions.Find(id);
            ViewBag.RoleId = new SelectList(database.AspNetRoles.OrderBy(p => p.Name), "ID", "Name");
            return View(_permission);
        }

        [HttpPost]
        public ActionResult PermissionEdit(AspNetPermission _permission)
        {
            if (ModelState.IsValid)
            {
                database.Entry(_permission).State = EntityState.Modified;
                database.SaveChanges();
                return RedirectToAction("PermissionDetails", new RouteValueDictionary(new { id = _permission.Permission_Id }));
            }
            return View(_permission);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult PermissionDelete(int id)
        {
            AspNetPermission permission = database.AspNetPermissions.Find(id);
            if (permission.AspNetRolePermissions.Count > 0)
                permission.AspNetRolePermissions.Clear();

            database.Entry(permission).State = EntityState.Deleted;
            database.SaveChanges();
            return RedirectToAction("PermissionIndex");
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddPermission2RoleReturnPartialView(string id, long permissionId)
        {
            AspNetRole role = database.AspNetRoles.Find(id);
            AspNetPermission _permission = database.AspNetPermissions.Find(permissionId);


            AspNetRolePermission _aspnetrolePermissions = database.AspNetRolePermissions.Where(a => a.Permission_Id == permissionId && a.Role_Id == id).FirstOrDefault();
            if (_aspnetrolePermissions == null)
            {
                AspNetRolePermission _aspnetrolePermissions1 = new AspNetRolePermission();
                _aspnetrolePermissions1.Role_Id = id;
                _aspnetrolePermissions1.Permission_Id = permissionId;
                database.AspNetRolePermissions.Add(_aspnetrolePermissions1);
                database.SaveChanges();
            }

            //if (!role.PERMISSIONS.Contains(_permission))
            //{
            //    role.PERMISSIONS.Add(_permission);
            //    database.SaveChanges();
            //}
            return PartialView("_ListPermissions", role);
        }

        //[HttpGet]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        //public PartialViewResult AddAllPermissions2RoleReturnPartialView(int id)
        //{
        //    ROLE _role = database.ROLES.Where(p => p.Role_Id == id).FirstOrDefault();
        //    List<PERMISSION> _permissions = database.PERMISSIONS.ToList();
        //    foreach (PERMISSION _permission in _permissions)
        //    {
        //        if (!_role.PERMISSIONS.Contains(_permission))
        //        {
        //            _role.PERMISSIONS.Add(_permission);

        //        }
        //    }
        //    database.SaveChanges();
        //    return PartialView("_ListPermissions", _role);
        //}

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeletePermissionFromRoleReturnPartialView(string id, int permissionId)
        {
            AspNetRole _role = database.AspNetRoles.Find(id);
            AspNetPermission _permission = database.AspNetPermissions.Find(permissionId);


            AspNetRolePermission _aspnetrolePermissions = database.AspNetRolePermissions.Where(a => a.Permission_Id == permissionId && a.Role_Id == id).FirstOrDefault();
            if (_aspnetrolePermissions != null)
            {

                database.AspNetRolePermissions.Remove(_aspnetrolePermissions);

                database.SaveChanges();
            }


            //if (_role.PERMISSIONS.Contains(_permission))
            //{
            //    _role.PERMISSIONS.Remove(_permission);
            //    database.SaveChanges();
            //}
            return PartialView("_ListPermissions", _role);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult DeleteRoleFromPermissionReturnPartialView(string id, long permissionId)
        {
            AspNetRole role = database.AspNetRoles.Find(id);
            AspNetPermission permission = database.AspNetPermissions.Find(permissionId);


            AspNetRolePermission _aspnetrolePermissions = database.AspNetRolePermissions.Where(a => a.Permission_Id == permissionId && a.Role_Id == id).FirstOrDefault();
            if (_aspnetrolePermissions != null)
            {

                database.AspNetRolePermissions.Remove(_aspnetrolePermissions);

                database.SaveChanges();
            }

            //if (role.PERMISSIONS.Contains(permission))
            //{
            //    role.PERMISSIONS.Remove(permission);
            //    database.SaveChanges();
            //}
            return PartialView("_ListRolesTable4Permission", permission);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public PartialViewResult AddRole2PermissionReturnPartialView(int permissionId, string roleId)
        {
            AspNetRole role = database.AspNetRoles.Find(roleId);
            AspNetPermission _permission = database.AspNetPermissions.Find(permissionId);

            AspNetRolePermission _aspnetrolePermissions = database.AspNetRolePermissions.Where(a => a.Permission_Id == permissionId && a.Role_Id == roleId).FirstOrDefault();
            if (_aspnetrolePermissions == null)
            {
                AspNetRolePermission _aspnetrolePermissions1 = new AspNetRolePermission();
                _aspnetrolePermissions1.Role_Id = roleId;
                _aspnetrolePermissions1.Permission_Id = permissionId;
                database.AspNetRolePermissions.Add(_aspnetrolePermissions1);
                database.SaveChanges();
            }

            //if (!role.PERMISSIONS.Contains(_permission))
            //{
            //    role.PERMISSIONS.Add(_permission);
            //    database.SaveChanges();
            //}
            return PartialView("_ListRolesTable4Permission", _permission);
        }

        public ActionResult PermissionsImport()
        {

            List<string> ControllerList = new List<string>() { "CustomerController", "DashboardController", "FormTypeController", "InstallerAllocationController", "MeterMakeController", "MeterSizeController", "MeterTypeController", "PictureController", "ProjectController", "ProjectStatusController", "ReportsController", "ServicesController", "UsersController", "FormManagementController" };

            //Assembly asm = Assembly.GetAssembly(typeof(MvcApplication));

            //var controlleractionlist = asm.GetTypes()
            //        .Where(type => typeof(System.Web.Mvc.Controller).IsAssignableFrom(type))
            //        .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
            //        .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
            //         //.Select(x => new { Controller = x.DeclaringType.Name, Action = x.Name })
            //         .Select(x => new { Controller = x.DeclaringType.Name }).Distinct()
            //        .OrderBy(x => x.Controller).ToList();


            //var _controllerTypes = AppDomain.CurrentDomain.GetAssemblies()
            //    .SelectMany(a => a.GetTypes())
            //    .Where(t => t != null
            //        && t.IsPublic
            //        && t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
            //        && !t.IsAbstract
            //        && typeof(IController).IsAssignableFrom(t));

            //var _controllerMethods = _controllerTypes.ToDictionary(controllerType => controllerType,
            //        controllerType => controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            //        .Where(m => typeof(ActionResult).IsAssignableFrom(m.ReturnType)));

            foreach (var _controller in ControllerList)
            {
                string _controllerName = _controller;

                if (_controllerName.EndsWith("Controller"))
                {
                    _controllerName = _controllerName.Substring(0, _controllerName.LastIndexOf("Controller"));

                    string _permissionDescriptioncontroller = string.Format("{0}", _controllerName);

                    AspNetPermission _permissioncontroller = database.AspNetPermissions.Where(p => p.PermissionDescription == _permissionDescriptioncontroller).FirstOrDefault();
                    if (_permissioncontroller == null)
                    {
                        AspNetPermission _permcontroller = new AspNetPermission();
                        _permcontroller.PermissionDescription = _permissionDescriptioncontroller;
                        database.AspNetPermissions.Add(_permcontroller);
                        database.SaveChanges();
                    }

                }

                //foreach (var _controllerAction in _controller.Value)
                //{
                //    string _controllerActionName = _controllerAction.Name;


                //    string _permissionDescription = string.Format("{0}-{1}", _controllerName, _controllerActionName);
                //    AspNetPermission _permission = database.AspNetPermissions.Where(p => p.PermissionDescription == _permissionDescription).FirstOrDefault();
                //    if (_permission == null)
                //    {
                //        if (ModelState.IsValid)
                //        {
                //            AspNetPermission _perm = new AspNetPermission();
                //            _perm.PermissionDescription = _permissionDescription;

                //            database.AspNetPermissions.Add(_perm);

                //            try
                //            {
                //                database.SaveChanges();
                //            }
                //            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                //            {
                //                Exception raise = dbEx;
                //                foreach (var validationErrors in dbEx.EntityValidationErrors)
                //                {
                //                    foreach (var validationError in validationErrors.ValidationErrors)
                //                    {
                //                        string message = string.Format("{0}:{1}",
                //                            validationErrors.Entry.Entity.ToString(),
                //                            validationError.ErrorMessage);
                //                        // raise a new exception nesting
                //                        // the current instance as InnerException
                //                        raise = new InvalidOperationException(message, raise);
                //                    }
                //                }
                //                throw raise;
                //            }
                //        }
                //    }
                //}
            }
            return RedirectToAction("PermissionIndex");
        }
        #endregion


        public ActionResult Permission()
        {
            List<AspNetPermission> _permissions = database.AspNetPermissions
                              .OrderBy(wn => wn.PermissionDescription)
                              .Include(a => a.AspNetRolePermissions)
                              .ToList();

            List<AspNetRole> _role = database.AspNetRoles
                             .OrderBy(wn => wn.Name)
                             .Include(a => a.AspNetRolePermissions)
                             .ToList();

            AspNetPermissionVM AspNetPermissionVM = new AspNetPermissionVM();

            AspNetPermissionVM.AspNetPermissionList = _permissions;

            AspNetPermissionVM.AspNetRoleList = _role;
            AspNetPermissionVM.UpdatePermission = false;
            AspNetPermissionVM.UpdateMessage = "";


            return View(AspNetPermissionVM);

        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult AssignPermissionRole(List<PermissionRoleVM> PermissionList)
        {

            if (PermissionList != null && PermissionList.Count > 0)
            {
                foreach (var permissionModel in PermissionList)
                {
                    long permissionID = Convert.ToInt64(permissionModel.PermissionID.Trim());
                    var permissionName = permissionModel.PermissionName.Trim();

                    if (permissionModel.RoleList != null && permissionModel.RoleList.Count > 0)
                    {
                        foreach (var rolemodel in permissionModel.RoleList)
                        {
                            var roleid = rolemodel.RoleID.Trim();
                            var roleName = rolemodel.RoleName.Trim();
                            var rolePermissionID = rolemodel.PermissionID.Trim();
                            var roleChecked = rolemodel.RoleChecked;

                            AspNetRolePermission AspNetRolePermission = database.AspNetRolePermissions.Where(a => a.Permission_Id == permissionID && a.Role_Id == roleid).FirstOrDefault();
                            if (AspNetRolePermission != null)
                            {
                                if (roleChecked == false)
                                {
                                    database.AspNetRolePermissions.Remove(AspNetRolePermission);
                                    database.SaveChanges();
                                }
                            }
                            else
                            {
                                if (roleChecked == true)
                                {
                                    AspNetRolePermission newAspNetRolePermission = new AspNetRolePermission();
                                    newAspNetRolePermission.Permission_Id = permissionID;
                                    newAspNetRolePermission.Role_Id = roleid;
                                    database.AspNetRolePermissions.Add(newAspNetRolePermission);
                                    database.SaveChanges();
                                }
                            }
                        }
                    }

                }



            }

            List<AspNetPermission> _permissions = database.AspNetPermissions
                                .OrderBy(wn => wn.PermissionDescription)
                                .Include(a => a.AspNetRolePermissions)
                                .ToList();

            List<AspNetRole> _role = database.AspNetRoles
                             .OrderBy(wn => wn.Name)
                             .Include(a => a.AspNetRolePermissions)
                             .ToList();

            AspNetPermissionVM AspNetPermissionVM = new AspNetPermissionVM();

            AspNetPermissionVM.AspNetPermissionList = _permissions;

            AspNetPermissionVM.AspNetRoleList = _role;

            AspNetPermissionVM.UpdatePermission = true;
            AspNetPermissionVM.UpdateMessage = "Permission Updated";

            return PartialView("_ListPermissionsRolesGrid", AspNetPermissionVM);
            //return View();
        }

        
    }



    


}