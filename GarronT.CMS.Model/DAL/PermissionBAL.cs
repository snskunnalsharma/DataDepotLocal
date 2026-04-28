using GarronT.CMS.Model.BO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace GarronT.CMS.Model.DAL
{
    public class PermissionBAL
    {
        /// <summary>
        /// Orders permissions for Role Rights / User Permission grids: main app sections first (through Form Management),
        /// then <b>Inventory Reports</b>, then <b>Reports</b> (Duplicate/Export/…), then <b>Other Reports</b> (Time Clock), then any remainder.
        /// Hides <c>InventoryTransfer</c> so Role Rights shows only <c>InventoryBulkTransfer</c> (same UI label: Inventory Transfer).
        /// Uses Permission_Id &lt; 27 as the split before "Duplicate Report" (standard CMS schema).
        /// </summary>
        public static List<AspNetPermission> ReorderAspNetPermissionsForRightsGrid(IEnumerable<AspNetPermission> source)
        {
            var list = source.OrderBy(p => p.Permission_Id)
                .Where(p => p.PermissionDescription != "InventoryTransfer")
                .ToList();

            var inventoryNames = new HashSet<string>(StringComparer.Ordinal)
            {
                "Warehouse Stock Report",
                "Inventory Stock Report",
                "Product Movement Report",
                "Inventory Transfer",
                "Warehouse Transfer",
                "INV Installer Report"
            };

            int InventorySortKey(AspNetPermission p)
            {
                switch (p.PermissionName)
                {
                    case "Warehouse Stock Report": return 0;
                    case "Inventory Stock Report": return 1;
                    case "Product Movement Report": return 2;
                    case "Inventory Transfer": return 3;
                    case "Warehouse Transfer": return 4;
                    case "INV Installer Report": return 5;
                    default: return 99;
                }
            }

            var invOrdered = list.Where(p => inventoryNames.Contains(p.PermissionName))
                .OrderBy(InventorySortKey)
                .ThenBy(p => p.Permission_Id)
                .ToList();

            var reportNames = new HashSet<string>(StringComparer.Ordinal)
            {
                "Duplicate Report",
                "Export Report",
                "Installer Report",
                "Auditor Report",
                "Skip History Report"
            };

            var reportOrdered = list.Where(p => reportNames.Contains(p.PermissionName))
                .OrderBy(p => p.Permission_Id)
                .ToList();

            var duplicateReport = list.FirstOrDefault(p => p.PermissionName == "Duplicate Report");
            var firstReportsCutoff = duplicateReport != null ? duplicateReport.Permission_Id : 27L;
            var firstBlock = list.Where(p =>
                    p.Permission_Id < firstReportsCutoff
                    && !inventoryNames.Contains(p.PermissionName)
                    && !reportNames.Contains(p.PermissionName)
                    && p.PermissionName != "Time Clock Report"
                    && p.PermissionName != "Test")
                .OrderBy(p => p.Permission_Id)
                .ToList();

            var timeClockItems = list.Where(p => p.PermissionName == "Time Clock Report")
                .OrderBy(p => p.Permission_Id)
                .ToList();

            var testItems = list.Where(p => p.PermissionName == "Test")
                .OrderBy(p => p.Permission_Id)
                .ToList();

            var assigned = new HashSet<long>(
                firstBlock.Select(p => p.Permission_Id)
                    .Concat(invOrdered.Select(p => p.Permission_Id))
                    .Concat(reportOrdered.Select(p => p.Permission_Id))
                    .Concat(timeClockItems.Select(p => p.Permission_Id))
                    .Concat(testItems.Select(p => p.Permission_Id)));

            var remainder = list.Where(p => !assigned.Contains(p.Permission_Id))
                .OrderBy(p => p.Permission_Id)
                .ToList();

            return firstBlock
                .Concat(invOrdered)
                .Concat(reportOrdered)
                .Concat(timeClockItems)
                .Concat(testItems)
                .Concat(remainder)
                .ToList();
        }

        /// <summary>
        /// If the same permission was inserted twice in DB (same <see cref="AspNetPermission.PermissionDescription"/>),
        /// keep the first row in display order (lowest Permission_Id) so Role Rights does not show duplicates.
        /// </summary>
        public static List<AspNetPermission> DedupeAspNetPermissionsByDescription(IEnumerable<AspNetPermission> ordered)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var result = new List<AspNetPermission>();
            foreach (var p in ordered)
            {
                var key = (p.PermissionDescription ?? string.Empty).Trim();
                if (string.IsNullOrEmpty(key))
                    key = "id:" + p.Permission_Id.ToString(CultureInfo.InvariantCulture);
                if (seen.Add(key))
                    result.Add(p);
            }
            return result;
        }

        public AspNetPermissionVM GetPermissionData()
        {
            CompassEntities database1 = new CompassEntities();



            List<AspNetPermission> _permissions = database1.AspNetPermissions
                              .OrderBy(wn => wn.Permission_Id)
                              .Include(a => a.AspNetRolePermissions)
                              .ToList();

            _permissions = DedupeAspNetPermissionsByDescription(ReorderAspNetPermissionsForRightsGrid(_permissions));

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
                if (PermissionList != null && PermissionList.Count > 0)
                {
                    PermissionList = PermissionList
                        .Where(p => p != null && !string.IsNullOrWhiteSpace(p.PermissionID) && !string.IsNullOrWhiteSpace(p.RoleID))
                        .GroupBy(p => (p.PermissionID.Trim() + "|" + p.RoleID.Trim()), StringComparer.OrdinalIgnoreCase)
                        .Select(g => g.First())
                        .ToList();
                }

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
                // Role rights are the source of truth; do not sync per-user permissions here.
                // This keeps updates fast and avoids forcing manual SQL fixes per user.
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
