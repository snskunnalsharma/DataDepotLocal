using GarronT.CMS.Model;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class RBACUser
{
    public string User_Id { get; set; }
    public bool IsSysAdmin { get; set; }
    public string Username { get; set; }
    private List<UserRole> Roles = new List<UserRole>();

    public RBACUser(string _username)
    {
        this.Username = _username;
        this.IsSysAdmin = false;
        GetDatabaseUserRolesPermissions();
    }

    private void GetDatabaseUserRolesPermissions()
    {

        //using (RBAC_Model _data = new RBAC_Model())
        using (CompassEntities _data = new CompassEntities())
        {
            AspNetUser _user = _data.AspNetUsers.Where(u => u.UserName == this.Username).FirstOrDefault();
            if (_user != null)
            {
                this.User_Id = _user.Id;
                foreach (AspNetRole _role in _user.AspNetRoles)
                {
                    UserRole _userRole = new UserRole { Role_Id = _role.Id, RoleName = _role.Name };
                    foreach (AspNetRolePermission _permission in _role.AspNetRolePermissions)
                    {
                        _userRole.Permissions.Add(new RolePermission { Permission_Id = _permission.Permission_Id, PermissionDescription = _permission.AspNetPermission.PermissionDescription });
                    }
                    this.Roles.Add(_userRole);

                    if (!this.IsSysAdmin)
                        this.IsSysAdmin = _role.IsSysAdmin == null ? false : _role.IsSysAdmin.Value;
                }
            }
        }
    }

    public bool HasPermission(string requiredPermission)
    {
        bool bFound = false;
        foreach (UserRole role in this.Roles)
        {
            bFound = (role.Permissions.Where(p => p.PermissionDescription.ToLower() == requiredPermission.ToLower()).ToList().Count > 0);
            if (bFound)
                break;
        }
        return bFound;
    }

    public bool HasRole(string role)
    {
        return (Roles.Where(p => p.RoleName == role).ToList().Count > 0);
    }

    public bool HasRoles(string roles)
    {
        bool bFound = false;
        string[] _roles = roles.ToLower().Split(';');
        foreach (UserRole role in this.Roles)
        {
            try
            {
                bFound = _roles.Contains(role.RoleName.ToLower());
                if (bFound)
                    return bFound;
            }
            catch (Exception)
            {
            }
        }
        return bFound;
    }
}

public class UserRole
{
    public string Role_Id { get; set; }
    public string RoleName { get; set; }
    public List<RolePermission> Permissions = new List<RolePermission>();
}

public class RolePermission
{
    public long Permission_Id { get; set; }
    public string PermissionDescription { get; set; }
}