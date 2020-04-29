using GarronT.CMS.Model;
//using KSPL.AIBC.ARM.Model;
//using KSPL.AIBC.ARM.Model.BO;
//using KSPL.AIBC.ARM.UI.Areas.Global.Models;
//using KSPL.AIBC.ARM.UI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GarronT.CMS.Model.DAL;

namespace GarronT.CMS.UI.Models
{
    public class LoginDAL
    {

        public LoginDAL()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {

        }

        public LoginDAL(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            //var uservalidator = UserManager.UserValidator as UserValidator<ApplicationUser>;
            //uservalidator.AllowOnlyAlphanumericUserNames = true;
        }

        private UserManager<ApplicationUser> UserManager { get; set; }
        private ApplicationSignInManager SignInManager { get; set; }


        #region Login

        public bool Login(string userID, string password, out long UserId, out int IsFirstLogin, out bool statusActive, out string _message, out string UserName)
        {
            statusActive = false;
            bool result = false;
            UserId = 0;
            IsFirstLogin = 0;
            _message = "";
            UserName = "";
            try
            {
                var user = UserManager.Find(userID.Trim(), password.Trim());

                if (user != null)
                {
                    result = true;
                    using (CompassEntities db = new CompassEntities())
                    {
                        var a = (from u in db.tblUsers
                                 where u.UserName == userID.Trim()
                                 select u).FirstOrDefault();

                        UserId = a.UserID;
                        UserName = a.FirstName;
                        IsFirstLogin = Convert.ToInt32((from u in db.tblUsers
                                                        where u.UserName == userID.Trim()
                                                        select u.IsFirstLogin).FirstOrDefault());
                        if (a.Active == 1)
                        {
                            statusActive = true;
                        }
                    }
                }
                else
                {
                    using (CompassEntities db = new CompassEntities())
                    {
                        var a= (from u in db.tblUsers  where u.UserName == userID.Trim() select u).FirstOrDefault();
                        if (a == null)
                        {
                            _message = "user not exist.";
                        }
                        else
                        {
                            _message = "wrong password.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public bool ForgotPassword(string userName, out bool statusActive)
        {
            bool result = false;
            statusActive = false;
            CompassEntities db = new CompassEntities();
            try
            {
                tblUser objUser = (from b in db.tblUsers where b.UserName == userName select b).FirstOrDefault();
                if (objUser != null)
                {
                    if (objUser.Active == 1)
                    {
                        statusActive = true;
                        objUser.IsFirstLogin = 1;
                        db.SaveChanges();

                        string Password = new CommonFunctions().GeneratePassword();

                        if (Password.Contains('&'))
                        {
                            Password = Password.Replace('&', 'a');
                        }

                        int i = 8 - Password.Length;

                        for (int j = 0; j < i; j++)
                        {
                            if (j == 0)
                            {
                                Password = Password + "0aD@";
                            }
                            else
                            {
                                Password = Password + j.ToString();
                            }
                            
                        }


                        var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

                        AspNetUser single = (from b in db.AspNetUsers
                                             where b.Id == objUser.aspnet_UserId.ToString()
                                             select b).FirstOrDefault();
                        if (single != null)
                        {
                            String hashedNewPassword = UserManager.PasswordHasher.HashPassword(Password);
                            single.PasswordHash = hashedNewPassword;
                            db.SaveChanges();

                            List<String> mailto = new List<String>();
                            mailto.Add(objUser.Email);

                            string name = objUser.FirstName + " " + objUser.LastName;
                            new clsMail().SendMail(mailto, null, GetRegisterMailBody(name, objUser.UserName, Password), "New Password");
                        }
                        result = true;
                    }

                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public async Task<Tuple<bool, bool, bool>> ChangePasswordforMobile(string mobileNumber, string oldPassword, string newPassword)
        {
            bool isActive = false;
            bool StatusIsFound = false;
            bool resultre = false;
            CompassEntities db = new CompassEntities();
            tblUser objUser = (from b in db.tblUsers where b.UserName == mobileNumber select b).FirstOrDefault();
            if (objUser != null)
            {
                StatusIsFound = true;
                if (objUser.Active == 1)
                {
                    isActive = true;
                    var res = UserManager.ChangePassword(objUser.aspnet_UserId.ToString(), oldPassword, newPassword);
                    if (res.Succeeded)
                    {

                        objUser.IsFirstLogin = 0;
                        db.SaveChanges();
                        resultre = true;
                    }
                }
            }

            Tuple<bool, bool, bool> tuple = new Tuple<bool, bool, bool>(resultre, StatusIsFound, isActive);
            return tuple;
        }

        public string GetRegisterMailBody(string Name, string UserName, string Password)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Hello " + Name + ",");
            sb.Append("<br /><br />Your Password has been reset. Please find below new password.");
            sb.Append("<br /><br /><table border=" + 1 + " ><tr><td>User Name</td><td>" + UserName + "</td></tr>"
                       + "<tr><td>Password</td><td>" + Password + "</td></tr></table>");
            sb.Append("<br /><br />Thanks");
            return sb.ToString();
        }
        #endregion
    }


    


}
