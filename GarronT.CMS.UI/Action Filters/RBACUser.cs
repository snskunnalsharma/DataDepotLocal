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

                // Apply user-level permission overrides (AspNetUserRolePermissions)
                // These are used by User Permission screen; RBAC checks PermissionDescription strings.
                try
                {
                    Guid aspNetGuid;
                    if (Guid.TryParse(_user.Id, out aspNetGuid))
                    {
                        var tblUserId = _data.tblUsers
                            .Where(x => x.aspnet_UserId == aspNetGuid && x.Active == 1)
                            .Select(x => x.UserID)
                            .FirstOrDefault();

                        if (tblUserId > 0)
                        {
                            var userPerms = _data.AspNetUserRolePermissions
                                .Where(x => x.Active == true && x.UserId == tblUserId)
                                .Select(x => new
                                {
                                    PermissionId = x.FKAspNetPermissionId,
                                    Allow = x.ViewOperation || x.AddOperation || x.EditOperation || x.DeleteOperation
                                })
                                .ToList();

                            if (userPerms != null && userPerms.Count > 0)
                            {
                                // Current role-granted permissions
                                var granted = new HashSet<long>(
                                    this.Roles.SelectMany(r => r.Permissions).Select(p => p.Permission_Id));

                                foreach (var up in userPerms)
                                {
                                    if (up.Allow)
                                    {
                                        granted.Add(up.PermissionId);
                                    }
                                    else
                                    {
                                        granted.Remove(up.PermissionId);
                                    }
                                }

                                var permDescriptions = _data.AspNetPermissions
                                    .Where(p => granted.Contains(p.Permission_Id))
                                    .Select(p => new RolePermission
                                    {
                                        Permission_Id = p.Permission_Id,
                                        PermissionDescription = p.PermissionDescription
                                    })
                                    .ToList();

                                // Replace with a single effective permission set
                                this.Roles.Clear();
                                this.Roles.Add(new UserRole { Role_Id = "EFFECTIVE", RoleName = "EFFECTIVE", Permissions = permDescriptions });
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Swallow to avoid breaking auth pipeline on permission override failures
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