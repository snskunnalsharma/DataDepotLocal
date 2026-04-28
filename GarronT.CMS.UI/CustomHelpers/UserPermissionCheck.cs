using GarronT.CMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GarronT.CMS.UI.CustomHelpers
{
    public class UserPermissionCheck
    {
        public static string PerMessage = "You do not have permission to access the requested resource due to security restrictions. In order to gain access, please speak to your system administrator.";
        public static bool checkUserStatus(string strcurrentController, string operationName)
        {

            bool UserAccessPermission = false;
            var checkAdmin= HttpContext.Current.Session["CurrentUserIsAdmin"].ToString();
            if(checkAdmin=="1")
            {
                UserAccessPermission = true;
            }
            else
            {
                var UserAccess = (List<AspNetUserRolePermission>)HttpContext.Current.Session["UserPermissionList"];

                var PermissionList = (List<AspNetPermission>)HttpContext.Current.Session["PermissionList"];

                string strMessage = "";

                if (PermissionList == null || UserAccess == null)
                {
                    return false;
                }

                var currentController = PermissionList.Where(o => o.PermissionDescription == strcurrentController).FirstOrDefault();
                if (currentController == null)
                {
                    return false;
                }

                var currentUserAccess = UserAccess.Where(o => o.FKAspNetPermissionId == currentController.Permission_Id).FirstOrDefault();

                if (currentUserAccess != null)
                {
                    if (operationName == "Add")
                    {
                        UserAccessPermission = currentUserAccess.AddOperation == true ? true : false;
                    }
                    if (operationName == "Edit")
                    {
                        UserAccessPermission = currentUserAccess.EditOperation == true ? true : false;
                    }
                    if (operationName == "Delete")
                    {
                        UserAccessPermission = currentUserAccess.DeleteOperation == true ? true : false;
                    }
                    if (operationName == "View")
                    {
                        UserAccessPermission = currentUserAccess.ViewOperation == true ? true : false;
                    }

                }
            }

           
            return UserAccessPermission;

        }


    }
}