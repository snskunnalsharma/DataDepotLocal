using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace GarronT.CMS.Model.DAL
{
    public class PermissionBAL
    {

        public AspNetPermissionVM GetPermissionData()
        {
            CompassEntities database1 = new CompassEntities();



            List<AspNetPermission> _permissions = database1.AspNetPermissions
                              .OrderBy(wn => wn.Permission_Id)
                              .Include(a => a.AspNetRolePermissions)
                              .ToList();



            List<AspNetRole> _role = database1.AspNetRoles
                             .OrderBy(wn => wn.SortingNumber)
                             .Include(a => a.AspNetRolePermissions)
                             .ToList();







            List<AccessPermissionBo> objAccessPermissionBo = new List<AccessPermissionBo>();
            var ObjList = database1.AspNetRolePermissions.ToList();
            foreach (var item in ObjList)
            {
                var Obj = new AccessPermissionBo();
                Obj.RoleAddChecked = item.AddOperation;
                Obj.RoleViewChecked = item.ViewOperation;
                Obj.RoleEditChecked = item.EditOperation;
                Obj.RoleDeleteChecked = item.DeleteOperation;
                Obj.PermissionID = item.Permission_Id.ToString();
                Obj.RoleID = item.Role_Id;
                objAccessPermissionBo.Add(Obj);
            }


            AspNetPermissionVM AspNetPermissionVM = new AspNetPermissionVM();

            AspNetPermissionVM.AspNetPermissionList = _permissions;
            AspNetPermissionVM.ObjAccessPermissionBoList = objAccessPermissionBo;

            AspNetPermissionVM.AspNetRoleList = _role;
            AspNetPermissionVM.UpdatePermission = false;
            AspNetPermissionVM.UpdateMessage = "";
            database1.Dispose();
            return AspNetPermissionVM;
        }


        public bool SavePermissionData(List<AccessPermissionBo> PermissionList, int currentUserId, out string Message)
        {
            CompassEntities database = new CompassEntities();
            try
            {


                var ObjOldPermissionList = database.AspNetRolePermissions.Select(o =>
                    new AccessPermissionBo
                    {
                        PermissionID = o.Permission_Id.ToString(),
                        RoleID = o.Role_Id,
                        RoleAddChecked = o.AddOperation,
                        RoleEditChecked = o.EditOperation,
                        RoleDeleteChecked = o.DeleteOperation,
                        RoleViewChecked = o.ViewOperation
                    }).ToList();


                foreach (var item in PermissionList)
                {
                    long permissionID = Convert.ToInt64(item.PermissionID.Trim());
                    //  var permissionName = permissionModel.PermissionName.Trim();
                    var roleid = item.RoleID.Trim();



                    AspNetRolePermission Obj = database.AspNetRolePermissions.Where(a => a.Permission_Id == permissionID && a.Role_Id == roleid).FirstOrDefault();
                    if (Obj != null)
                    {
                        Obj.AddOperation = item.RoleAddChecked;
                        Obj.ViewOperation = item.RoleViewChecked;
                        Obj.EditOperation = item.RoleEditChecked;
                        Obj.DeleteOperation = item.RoleDeleteChecked;
                    }
                    else
                    {
                        Obj = new AspNetRolePermission();
                        Obj.AddOperation = item.RoleAddChecked;
                        Obj.ViewOperation = item.RoleViewChecked;
                        Obj.EditOperation = item.RoleEditChecked;
                        Obj.DeleteOperation = item.RoleDeleteChecked;
                        Obj.Permission_Id = permissionID;
                        Obj.Role_Id = roleid;
                        database.AspNetRolePermissions.Add(Obj);
                    }




                }
                database.SaveChanges();


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



                var ABCUserList = database.tblUsers.ToList();
                /*Check for every user*/
                foreach (var item in ABCUserList)
                {
                    var UserRoleList = database.ASPNET_GetUserRoles(item.UserID).ToList();
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
                    var UserAccessList = database.AspNetUserRolePermissions.Where(o => o.Active == true && o.UserId == item.UserID).ToList();

                    var CurrentUserPermissionList = ObjNewPermissionList.Where(o => o.RoleID == MaxRoleId).ToList();

                    foreach (var item2 in CurrentUserPermissionList)
                    {
                        /*
                         * Check user permission 1 by 1 per form.
                         * Compare with old list & if the permission seems updated then check status for user & update.                              
                         */

                        var abcOldPermission = ObjOldPermissionList.Where(o => o.PermissionID == item2.PermissionID && o.RoleID == MaxRoleId).FirstOrDefault();

                        var abcItem = UserAccessList.Where(o => o.FKAspNetPermissionId.ToString() == item2.PermissionID).FirstOrDefault();


                        if (abcItem != null)
                        {

                            /*Compare if changes found then only update the record else not required to update*/
                            if (abcOldPermission.RoleAddChecked != item2.RoleAddChecked
                                || abcOldPermission.RoleDeleteChecked != item2.RoleDeleteChecked
                                || abcOldPermission.RoleEditChecked != item2.RoleEditChecked
                                || abcOldPermission.RoleViewChecked != item2.RoleViewChecked)
                            {

                                /*If add permission given*/
                                if (abcOldPermission.RoleAddChecked != item2.RoleAddChecked)
                                {
                                    abcItem.AddOperation = item2.RoleAddChecked;
                                }

                                /*If delete permission given*/
                                if (abcOldPermission.RoleDeleteChecked != item2.RoleDeleteChecked)
                                {
                                    abcItem.DeleteOperation = item2.RoleDeleteChecked;
                                }

                                /*If edit permission given*/
                                if (abcOldPermission.RoleEditChecked != item2.RoleEditChecked)
                                {
                                    abcItem.EditOperation = item2.RoleEditChecked;
                                }

                                /*If view permission given*/
                                if (abcOldPermission.RoleViewChecked != item2.RoleViewChecked)
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
                            abcItem.UserId = item.UserID;
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
                Message = "Record saved successfully!";
                return true;

            }
            catch (Exception ex)
            {
                Message = "Save failed. Please try again!";
                // return Json(new { result = false, resultMessage = "Save failed. Please try again!" }, JsonRequestBehavior.AllowGet);
                return false;
            }
            finally
            {
                database.Dispose();
            }
        }


    }
}
