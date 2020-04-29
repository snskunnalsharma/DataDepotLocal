using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarronT.CMS.Model.BO
{


    public class AspNetPermissionVM
    {
        public List<AspNetPermission> AspNetPermissionList { get; set; }
        public List<AspNetRole> AspNetRoleList { get; set; }

        public bool UpdatePermission { get; set; }

        public string UpdateMessage { get; set; }
        public List<AccessPermissionBo> ObjAccessPermissionBoList { get; set; }

    }


    public class PermissionRoleVM
    {
        public string PermissionID { get; set; }
        public string PermissionName { get; set; }
        public List<PermissionRole> RoleList { get; set; }

    }

    public class PermissionRole
    {
        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public string PermissionID { get; set; }
        public bool RoleChecked { get; set; }

    }




    public class MasterAccessPermissionBO
    {
        public List<AspNetRole> AspNetRoleList { get; set; }
        public List<AccessPermissionBo> ObjAccessPermissionBoList { get; set; }
    }


    public class AccessPermissionBo
    {
        public string PermissionID { get; set; }
        public string PermissionName { get; set; }
        public string RoleID { get; set; }
        public bool RoleViewChecked { get; set; }
        public bool RoleAddChecked { get; set; }
        public bool RoleEditChecked { get; set; }
        public bool RoleDeleteChecked { get; set; }
        public bool BlockUserMenu { get; set; }

    }



    public class UserAccessPermissionBo : AccessPermissionBo
    {
        public long UserId { get; set; }
    }

    public class UserAspNetPermissionVM : AspNetPermissionVM
    {
        public List<UserAccessPermissionBo> objUserAccessPermissionBo { get; set; }

    }
   
}
