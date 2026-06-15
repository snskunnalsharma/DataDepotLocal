using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Net;
using System.Linq;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNet.Identity.Owin;
using GarronT.CMS.UI.Areas.Global.Models;
using GarronT.CMS.UI.Models;
using Microsoft.Owin.Security;
using GarronT.CMS.Model;
using GarronT.CMS.Model.DAL;

namespace GarronT.CMS.UI.Areas.Global.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ActionResult Index()
        {
            // LoginViewModel model = new LoginViewModel();
            // model.UserName = "sanjiv@kairee.in";
            // model.Password = "Admin@123";
            return View();//model
        }
        public LoginController()
        {
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginViewModel model, string returnUrl)
        {

            CompassEntities obj = new CompassEntities();

            var Activeuser = obj.tblUsers.Where(o => o.UserName == model.UserName).FirstOrDefault();

            if (Activeuser != null && Activeuser.Active == 1)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var user = User.Identity.Name;
                Session["Username"] = user;

                var UserRoleList = obj.ASPNET_GetUserRoles(Activeuser.UserID).ToList();

                string MaxRoleId = UserRoleList.Where(o => o.Name == "Admin").Select(o => o.RoleId).FirstOrDefault();

                if (!string.IsNullOrEmpty(MaxRoleId))
                {
                    Session["CurrentUserIsAdmin"] = "1";
                }
                else
                {
                    Session["CurrentUserIsAdmin"] = "0";
                }

                #region PermissionPage



                //var usermaster = obj.AspNetUsers.Where(r => r.UserName == model.UserName).FirstOrDefault();
                //List<string> PermissionList1 = new List<string>();
                //var AspNetRolePermissionslist = usermaster.AspNetRoles.Select(a => a.AspNetRolePermissions).ToList();

                //foreach (var AspNetRolePermissions in AspNetRolePermissionslist)
                //{
                //    foreach (var item in AspNetRolePermissions)
                //    {
                //        var permissionname = item.AspNetPermission.PermissionDescription;
                //        PermissionList1.Add(permissionname);
                //    }
                //}
                //var PermissionList = PermissionList1.Distinct().ToList();

                // Role-based effective permissions (source of truth).
                // IMPORTANT: tblUsers.UserName doesn't always match AspNetUsers.UserName.
                // Use tblUsers.aspnet_UserId as the primary lookup for identity roles.
                var aspUserId = Activeuser.aspnet_UserId != null ? Activeuser.aspnet_UserId.ToString() : null;
                var aspUser = !string.IsNullOrWhiteSpace(aspUserId)
                    ? obj.AspNetUsers.Where(u => u.Id == aspUserId).FirstOrDefault()
                    : null;

                if (aspUser == null)
                {
                    aspUser = obj.AspNetUsers.Where(u => u.UserName == model.UserName).FirstOrDefault();
                }
                var roleIds = (aspUser != null)
                    ? aspUser.AspNetRoles.Select(r => r.Id).ToList()
                    : new List<string>();

                var rolePerms = obj.AspNetRolePermissions
                    .Where(rp => roleIds.Contains(rp.Role_Id))
                    .ToList();

                // Effective ops = OR across all roles.
                var effectiveByPermission = rolePerms
                    .GroupBy(rp => rp.Permission_Id)
                    .Select(g => new
                    {
                        PermissionId = g.Key,
                        ViewOperation = g.Any(x => x.ViewOperation),
                        AddOperation = g.Any(x => x.AddOperation),
                        EditOperation = g.Any(x => x.EditOperation),
                        DeleteOperation = g.Any(x => x.DeleteOperation)
                    })
                    .Where(x => x.ViewOperation || x.AddOperation || x.EditOperation || x.DeleteOperation)
                    .ToList();

                // Apply per-user overrides (AspNetUserRolePermissions) - these are set from User Permission screen.
                var userOverrides = obj.AspNetUserRolePermissions
                    .Where(x => x.Active == true && x.UserId == Activeuser.UserID)
                    .Select(x => new
                    {
                        PermissionId = (long)x.FKAspNetPermissionId,
                        x.ViewOperation,
                        x.AddOperation,
                        x.EditOperation,
                        x.DeleteOperation
                    })
                    .ToList();

                if (userOverrides != null && userOverrides.Count > 0)
                {
                    // Convert role-effective list to dictionary for override/merge.
                    var dict = effectiveByPermission.ToDictionary(
                        x => x.PermissionId,
                        x => new
                        {
                            x.PermissionId,
                            x.ViewOperation,
                            x.AddOperation,
                            x.EditOperation,
                            x.DeleteOperation
                        });

                    foreach (var ov in userOverrides)
                    {
                        // Override means: if any op is true -> allow with those ops; if all false -> deny (remove).
                        var allow = ov.ViewOperation || ov.AddOperation || ov.EditOperation || ov.DeleteOperation;
                        if (allow)
                        {
                            dict[ov.PermissionId] = new
                            {
                                PermissionId = ov.PermissionId,
                                ViewOperation = ov.ViewOperation,
                                AddOperation = ov.AddOperation,
                                EditOperation = ov.EditOperation,
                                DeleteOperation = ov.DeleteOperation
                            };
                        }
                        else
                        {
                            if (dict.ContainsKey(ov.PermissionId))
                            {
                                dict.Remove(ov.PermissionId);
                            }
                        }
                    }

                    effectiveByPermission = dict.Values.ToList();
                }

                var allowedPermissionIds = effectiveByPermission.Select(x => x.PermissionId).ToList();
                var PermissionList1 = obj.AspNetPermissions.Where(p => allowedPermissionIds.Contains(p.Permission_Id)).ToList();

                // Preserve existing session keys used across UI.
                Session["PermissionList"] = PermissionList1;
                Session["UserPermissionList"] = effectiveByPermission.Select(x => new AspNetUserRolePermission
                {
                    UserId = Activeuser.UserID,
                    FKAspNetPermissionId = (int)x.PermissionId,
                    ViewOperation = x.ViewOperation,
                    AddOperation = x.AddOperation,
                    EditOperation = x.EditOperation,
                    DeleteOperation = x.DeleteOperation,
                    Active = true
                }).ToList();

                var PermissionList = PermissionList1.Select(p => p.PermissionDescription).ToList();




                #endregion

                var controllername = "";
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true


                //commented on 2019-02-15 - email logic does not work
               // ApplicationUser signedUser = UserManager.FindByEmail(Activeuser.Email);

                ApplicationUser signedUser = UserManager.FindById(Activeuser.aspnet_UserId.ToString());

                var result = await SignInManager.PasswordSignInAsync(signedUser.UserName, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        if (PermissionList.Count == 0)
                        {
                            controllername = "Dashboard";
                        }
                        else if (PermissionList.Contains("Dashboard"))
                        {
                            controllername = "Dashboard";
                            // return RedirectToAction("Index", "Dashboard", new { @area = "Admin" });
                        }
                        else
                        {
                            controllername = PermissionList.FirstOrDefault();
                        }
                        return RedirectToAction("Index", controllername, new { @area = "Admin" });
                    // return RedirectToAction("Index", "Dashboard", new { @area = "Admin" });
                    //return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "User not active.");
                return View(model);
            }
            // return RedirectToAction("Index", "Dashboard", new { @area = "Admin" });

        }



        private const string key = "AIzaSyAlDLwyn_uVOVOYmClToYk9h-jLSUZ9j8Y";
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        public string urlShorter(string url)
        {
            string finalURL = "";
            string post = "{\"longUrl\": \"" + url + "\"}";
            string shortUrl = url;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url?key=" + key);
            try
            {
                request.ServicePoint.Expect100Continue = false;
                request.Method = "POST";
                request.ContentLength = post.Length;
                request.ContentType = "application/json";
                request.Headers.Add("Cache-Control", "no-cache");
                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] postBuffer = Encoding.ASCII.GetBytes(post);
                    requestStream.Write(postBuffer, 0, postBuffer.Length);
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader responseReader = new StreamReader(responseStream))
                        {
                            string json = responseReader.ReadToEnd();
                            var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(json);
                            finalURL = Convert.ToString(dict["id"]);// Regex.Match(json, @"""id"": ?""(?.+)""").Groups["id"].Value;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // if Google's URL Shortener is down...
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
            return finalURL;
        }



        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string Activation)
        {

            var ss = urlShorter("https://apps.kairee.in/smt/");
            LoginViewModel model = new LoginViewModel();
            model.Return_Activation = Activation;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


        [NoCache]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            Response.AddHeader("Cache-control", "no-store, must-revalidate, private, no-cache");
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("Expires", "0");
            Response.AppendToLog("window.location.reload();");

            return RedirectToAction("Index", "Login", new { area = "Global" });
        }



        [AllowAnonymous]
        [HttpPost]
        public ActionResult ForgotPassword(string userName)
        {

            if (string.IsNullOrEmpty(userName))
            {
                return Json(new { success = false, returnMessage = "User name required" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                bool isFound = false;

                var result = new LoginDAL().ForgotPassword(userName, out isFound);
                if (result == true)
                {
                    return Json(new { success = true, returnMessage = "Password reset successfully. Please check your mail!!!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, returnMessage = "Sorry, user not exist." }, JsonRequestBehavior.AllowGet);
                }
            }


        }
    }
}






